namespace WebApplication1.Middlewares
{
    using FluentValidation;
    using Microsoft.AspNetCore.Http.Metadata;
    using System.Reflection;
    using System.Text.Json;
    using WebApplication1.Extensions;

    namespace AccuracyMinimalAPI.Middleware
    {
        public class ValidationMiddleware
        {
            private readonly RequestDelegate _next;

            public ValidationMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                var endpoint = context.GetEndpoint();
                if (endpoint == null)
                {
                    await _next(context);
                    return;
                }

                var hasBody = HttpMethods.IsPost(context.Request.Method)
                    || HttpMethods.IsPut(context.Request.Method)
                    || HttpMethods.IsPatch(context.Request.Method);

                var ct = context.RequestAborted;

                var (modelType, model) = hasBody
                    ? await ResolveFromBodyAsync(context, endpoint, ct)
                    : ResolveFromRouteAndQuery(context, endpoint);

                if (modelType == null || model == null)
                {
                    await _next(context);
                    return;
                }

                var errors = new List<(string field, string message)>();
                await RunFluentValidationAsync(context, modelType, model, errors, ct);

                if (errors.Count > 0)
                {
                    await WriteValidationErrorAsync(context, errors, ct);
                    return;
                }

                await _next(context);
            }

            private static async Task<(Type? modelType, object? model)> ResolveFromBodyAsync(HttpContext context, Endpoint endpoint, CancellationToken ct)
            {
                var modelType = endpoint.Metadata.OfType<IAcceptsMetadata>().FirstOrDefault()?.RequestType
                    ?? endpoint.Metadata.OfType<ValidatedRequestMetadata>().FirstOrDefault()?.RequestType;

                if (modelType == null) return (null, null);

                if (!HasAnyValidator(context, modelType)) return (null, null);

                context.Request.EnableBuffering();
                try
                {
                    var model = await JsonSerializer.DeserializeAsync(
                        context.Request.Body,
                        modelType,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                        ct);
                    context.Request.Body.Position = 0;
                    return (modelType, model);
                }
                catch
                {
                    context.Request.Body.Position = 0;
                    return (null, null);
                }
            }

            private static (Type? modelType, object? model) ResolveFromRouteAndQuery(HttpContext context, Endpoint endpoint)
            {
                var modelType = endpoint.Metadata.OfType<ValidatedRequestMetadata>().FirstOrDefault()?.RequestType;
                if (modelType == null) return (null, null);

                if (!HasAnyValidator(context, modelType)) return (null, null);

                var instance = Activator.CreateInstance(modelType);
                if (instance == null) return (null, null);

                foreach (var prop in modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!prop.CanWrite) continue;

                    string? raw = null;
                    if (context.Request.RouteValues.TryGetValue(prop.Name, out var routeVal) && routeVal != null)
                        raw = routeVal.ToString();
                    else if (context.Request.Query.TryGetValue(prop.Name, out var queryVal))
                        raw = queryVal.ToString();

                    if (raw == null) continue;

                    try
                    {
                        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        var converted = targetType == typeof(string) ? raw : Convert.ChangeType(raw, targetType);
                        prop.SetValue(instance, converted);
                    }
                    catch
                    {
                        // Si no convierte, lo deja con default → la validación lo detectará
                    }
                }

                return (modelType, instance);
            }

            private static bool HasAnyValidator(HttpContext context, Type modelType)
            {
                var validatorType = typeof(IValidator<>).MakeGenericType(modelType);
                return context.RequestServices.GetService(validatorType) != null;
            }

            private static async Task RunFluentValidationAsync(HttpContext context, Type modelType, object model, List<(string field, string message)> errors, CancellationToken ct)
            {
                var validatorType = typeof(IValidator<>).MakeGenericType(modelType);
                if (context.RequestServices.GetService(validatorType) is not IValidator fluentValidator) return;

                var ctxType = typeof(FluentValidation.ValidationContext<>).MakeGenericType(modelType);
                var ctx = (IValidationContext)Activator.CreateInstance(ctxType, model)!;
                var result = await fluentValidator.ValidateAsync(ctx, ct);

                if (result.IsValid) return;

                foreach (var failure in result.Errors)
                {
                    errors.Add((failure.PropertyName, failure.ErrorMessage));
                }
            }

            private static async Task WriteValidationErrorAsync(HttpContext context, List<(string field, string message)> errors, CancellationToken ct)
            {
                var combinedMessage = string.Join(" | ", errors.Select(e => e.message));

                var errorResponse = new
                {
                    title = "Error de Validación",
                    message = combinedMessage,
                    type = "error",
                    errors = errors.Select(e => new { field = e.field, message = e.message }).ToList()
                };

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsJsonAsync(errorResponse, ct);
            }
        }
    }

}
