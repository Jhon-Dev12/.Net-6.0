using Microsoft.AspNetCore.Mvc;
using PracticaModel.Producto;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
string connString = builder.Configuration.GetConnectionString("SQLDBConnection");


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
            return Results.Ok(list);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }

    });

app.Run();

