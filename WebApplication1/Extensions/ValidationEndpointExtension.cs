namespace WebApplication1.Extensions
{
    public sealed class ValidatedRequestMetadata
    {
        public Type RequestType { get; }
        public ValidatedRequestMetadata(Type requestType) => RequestType = requestType;
    }

    public static class ValidationEndpointExtensions
    {
        public static RouteHandlerBuilder WithValidator<T>(this RouteHandlerBuilder builder)
        {
            builder.Add(b => b.Metadata.Add(new ValidatedRequestMetadata(typeof(T))));
            return builder;
        }
    }
}
