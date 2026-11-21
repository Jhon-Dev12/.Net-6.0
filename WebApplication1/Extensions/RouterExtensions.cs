
namespace WebApplication1.Extensions
{
    static class RouterExtensions
    {
        public static RouteHandlerBuilder MapSecurityPost(this IEndpointRouteBuilder app, string pattern,Delegate handler)
        {
            return app.MapPost($"/empresa1{pattern}", handler);
        }

    }
}
