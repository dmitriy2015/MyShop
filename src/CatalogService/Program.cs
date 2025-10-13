using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Swagger для удобства тестирования
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CatalogService", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CatalogService v1");
    });
}

// Простейшее "хранилище" — заменим на Postgres позже
var products = new List<Product>
{
    new(Guid.NewGuid(), "Keyboard", 59.99m),
    new(Guid.NewGuid(), "Mouse", 29.99m)
};

app.MapGet("/health", () => Results.Ok("CatalogService OK"));
app.MapGet("/products", () => Results.Ok(products));

app.MapPost("/products", (ProductCreate dto) =>
{
    var item = new Product(Guid.NewGuid(), dto.Name, dto.Price);
    products.Add(item);
    return Results.Created($"/products/{item.Id}", item);
});

app.Run();

record Product(Guid Id, string Name, decimal Price);
record ProductCreate(string Name, decimal Price);
