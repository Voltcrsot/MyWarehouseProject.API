using Microsoft.EntityFrameworkCore;
using MyWarehouseProject.Application.DTOs;
using MyWarehouseProject.Domain.Entities;
using MyWarehouseProject.Infrastructure.Data;
using MyWarehouseProject.Infrastructure.Repositories;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<WarehouseDbContext>(options =>
    options.UseSqlite("Data Source=warehouse.db"));

// Register repositories as scoped services.
builder.Services.AddScoped<WarehouseRepository>();

// Configure JSON serialization options.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// Add API documentation and endpoints.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Minimal API Endpoints

// Get all warehouses
app.MapGet("/api/warehouses", async (WarehouseRepository warehouseRepository) =>
{
    var warehouses = await warehouseRepository.GetAllWarehousesAsync();
    var warehouseDtos = warehouses.Select(w => new WarehouseDto
    {
        Id = w.Id,
        Name = w.Name,
        Stocks = w.Stocks.Select(s => new StockDto
        {
            Id = s.Id,
            ProductId = s.ProductId,
            Quantity = s.Quantity,
            WarehouseId = s.WarehouseId
        }).ToList()
    }).ToList();

    return Results.Ok(warehouseDtos);
});

// Get warehouse by id
app.MapGet("/api/warehouses/{id}", async (WarehouseRepository warehouseRepository, Guid id) =>
{
    var warehouse = await warehouseRepository.GetWarehouseByIdAsync(id);
    if (warehouse == null)
    {
        return Results.NotFound();
    }

    var warehouseDto = new WarehouseDto
    {
        Id = warehouse.Id,
        Name = warehouse.Name,
        Stocks = warehouse.Stocks.Select(s => new StockDto
        {
            Id = s.Id,
            ProductId = s.ProductId,
            Quantity = s.Quantity,
            WarehouseId = s.WarehouseId
        }).ToList()
    };

    return Results.Ok(warehouseDto);
});


// Get stock by id
app.MapGet("/api/stocks/{id}", async (WarehouseRepository warehouseRepository, Guid id) =>
{
    var stock = await warehouseRepository.GetStockByIdAsync(id);
    return stock is not null ? Results.Ok(stock) : Results.NotFound();
});

// Delete warehouse by id
app.MapDelete("/api/warehouses/{id}", async (WarehouseRepository warehouseRepository, Guid id) =>
{
    await warehouseRepository.DeleteWarehouseAsync(id);
    return Results.NoContent();
});

// Balance stock between warehouses
app.MapPost("/api/warehouses/balance", async (WarehouseRepository warehouseRepository, Guid sourceWarehouseId, Guid targetWarehouseId, Guid productId, int quantity) =>
{
    var sourceWarehouse = await warehouseRepository.GetWarehouseByIdAsync(sourceWarehouseId);
    var targetWarehouse = await warehouseRepository.GetWarehouseByIdAsync(targetWarehouseId);

    if (sourceWarehouse == null || targetWarehouse == null)
    {
        return Results.BadRequest("One or both warehouses not found.");
    }

    var sourceStock = sourceWarehouse.Stocks.FirstOrDefault(s => s.ProductId == productId);
    var targetStock = targetWarehouse.Stocks.FirstOrDefault(s => s.ProductId == productId);

    if (sourceStock == null || sourceStock.Quantity < quantity)
    {
        return Results.BadRequest("Not enough stock on the source warehouse.");
    }

    // Move stock from source to target
    sourceStock.Quantity -= quantity;
    if (targetStock != null)
    {
        targetStock.Quantity += quantity;
    }
    else
    {
        targetStock = new Stock
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            WarehouseId = targetWarehouseId,
            Quantity = quantity
        };
        targetWarehouse.Stocks.Add(targetStock);
    }

    await warehouseRepository.UpdateStockAsync(sourceStock);
    if (targetStock.Id == Guid.Empty)
    {
        await warehouseRepository.AddStockAsync(targetStock);
    }
    else
    {
        await warehouseRepository.UpdateStockAsync(targetStock);
    }

    return Results.NoContent();
});

// Auto-balance stock across all warehouses
app.MapPost("/api/warehouses/auto-balance/{productId}", async (WarehouseRepository warehouseRepository, Guid productId, Guid sourceWarehouseId) =>
{
    var sourceWarehouse = await warehouseRepository.GetWarehouseByIdAsync(sourceWarehouseId);
    var otherWarehouses = (await warehouseRepository.GetAllWarehousesAsync())
                          .Where(w => w.Id != sourceWarehouseId)
                          .ToList();

    if (sourceWarehouse == null || !otherWarehouses.Any())
    {
        return Results.BadRequest("Source warehouse not found or no other warehouses.");
    }

    var sourceStock = sourceWarehouse.Stocks.FirstOrDefault(s => s.ProductId == productId);

    if (sourceStock == null || sourceStock.Quantity <= 0)
    {
        return Results.BadRequest("No stock of the product on the source warehouse.");
    }

    var totalQuantity = sourceStock.Quantity;
    var quantityPerWarehouse = totalQuantity / otherWarehouses.Count;
    var remainder = totalQuantity % otherWarehouses.Count;

    foreach (var warehouse in otherWarehouses)
    {
        var targetStock = warehouse.Stocks.FirstOrDefault(s => s.ProductId == productId);

        if (targetStock != null)
        {
            targetStock.Quantity += quantityPerWarehouse;
        }
        else
        {
            targetStock = new Stock
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                WarehouseId = warehouse.Id,
                Quantity = quantityPerWarehouse
            };
            await warehouseRepository.AddStockAsync(targetStock);
        }

        await warehouseRepository.UpdateStockAsync(targetStock);
    }

    sourceStock.Quantity = remainder;
    await warehouseRepository.UpdateStockAsync(sourceStock);

    return Results.NoContent();
});

app.Run();
