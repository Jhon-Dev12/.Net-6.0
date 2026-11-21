using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Utils
{
    public class Helper
    {
        public static IResult BadRequest(HttpContext context,Exception ex)
        {
            var errorResponse = new
            {
                title = "Aviso",
                type = "error",
                message = ex.Message.ToString()
            };
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return Results.Json(errorResponse);
        }
        public static IResult Ok(HttpContext context, object body)
        {
            var ok = new
            {
                title = "Operación exitosa",
                type = "success",
                message = "La Operacion se completo correctamente"
            };
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.Response.StatusCode = StatusCodes.Status200OK;
            return Results.Json(body);
        }




    }
}
