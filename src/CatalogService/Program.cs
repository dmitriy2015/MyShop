using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Простой in-memory список (позже заменим на Postgres + EF Core)
var products = new List<Product>
{
    new(Guid.NewGuid(), "Keyboard", 59.99m),
    new(Guid.NewGuid(), "Mouse", 29.99m)
};

// Healthcheck
app.MapGet("/health", () => Results.Ok(new { status = "OK", service = "CatalogService" }));

// Получить список товаров
app.MapGet("/products", () => Results.Ok(products));

// Создать товар
app.MapPost("/products", (ProductDto dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name) || dto.Price <= 0)
        return Results.BadRequest(new { error = "Name required and Price > 0" });

    var item = new Product(Guid.NewGuid(), dto.Name.Trim(), dto.Price);
    products.Add(item);
    return Results.Created($"/products/{item.Id}", item);
});

// Получить товар по Id
app.MapGet("/products/{id:guid}", Results<Ok<Product>, NotFound> (Guid id) =>
{
    var item = products.FirstOrDefault(p => p.Id == id);
    return item is null ? TypedResults.NotFound() : TypedResults.Ok(item);
});

// Удалить товар
app.MapDelete("/products/{id:guid}", (Guid id) =>
{
    var removed = products.RemoveAll(p => p.Id == id) > 0;
    return removed ? Results.NoContent() : Results.NotFound();
});

app.Run();

record Product(Guid Id, string Name, decimal Price);
record ProductDto(string Name, decimal Price);
