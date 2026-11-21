using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PracticaModel.Producto;
using PracticaModel.Seguridad;
using WebApplication1.Extensions;
using WebApplication1.Middlewares;
using WebApplication1.Middlewares.AccuracyMinimalAPI.Middleware;
using WebApplication1.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);



var app = builder.Build();
string connString = builder.Configuration.GetConnectionString("SQLDBConnection");


app.UseMiddleware<ValidationMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.MapPost("/api/productos", async ([FromBody] ProductoRequest filter,HttpContext context) =>
    {
        try {
            PracticaBussiness.Producto.ProductoBL prod = new PracticaBussiness.Producto.ProductoBL();
            var list = await prod.getProductos(filter, connString);
            return Helper.Ok(context,list);
        }
        catch (Exception ex)
        {
            return Helper.BadRequest(context,ex);
        }

    }).WithValidator<ProductoRequest>();

app.MapSecurityPost("/login",async ([FromBody] LoginRequest req, HttpContext context) =>
{
    try
    {
        PracticaData.SeguridadDA.SeguridadDa seg = new PracticaData.SeguridadDA.SeguridadDa();
        var token = await seg.login(req, connString);
        return Helper.Ok(context, token);
    }
    catch (Exception ex)
    {
        return Helper.BadRequest(context, ex);
    }
}).WithValidator<LoginRequest>();



app.Run();

