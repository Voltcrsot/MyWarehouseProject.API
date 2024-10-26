using MyWarehouseProject.Application.Services;
using MyWarehouseProject.Application.DTOs;
using MyWarehouseProject.Domain.DTOS;
using Microsoft.AspNetCore.Mvc;

public static class WarehouseEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/warehouses");

        // Получение всех складов
        group.MapGet("/", async (WarehouseService warehouseService) =>
        {
            var warehouses = await warehouseService.GetAllWarehousesAsync();
            return Results.Ok(warehouses);
        });

        // Получение информации о конкретном складе по ID
        group.MapGet("/{id:guid}", async (WarehouseService warehouseService, Guid id) =>
        {
            var warehouse = await warehouseService.GetWarehouseByIdAsync(id);
            return warehouse != null ? Results.Ok(warehouse) : Results.NotFound();
        });

        // Добавление нового склада
        group.MapPost("/", async (WarehouseService warehouseService, [FromBody] WarehouseDto warehouseDto) =>
        {
            await warehouseService.AddWarehouseAsync(warehouseDto);
            return Results.Created($"/warehouses/{warehouseDto.Id}", warehouseDto);
        });

        // Обновление информации о складе
        group.MapPut("/{id:guid}", async (WarehouseService warehouseService, Guid id, [FromBody] WarehouseDto warehouseDto) =>
        {
            warehouseDto.Id = id;
            await warehouseService.UpdateWarehouseAsync(warehouseDto);
            return Results.NoContent();
        });

        // Удаление склада по ID
        group.MapDelete("/{id:guid}", async (WarehouseService warehouseService, Guid id) =>
        {
            await warehouseService.DeleteWarehouseAsync(id);
            return Results.NoContent();
        });

        // Эндпоинт для распределения запасов
        group.MapPost("/distribute", async (WarehouseService warehouseService, [FromQuery] Guid sourceWarehouseId, [FromQuery] Guid productId, [FromQuery] int quantity, [FromQuery] double nearestWarehousePercentage) =>
        {
            try
            {
                // Вызов метода распределения запасов с отдельными параметрами
                await warehouseService.DistributeStockAsync(sourceWarehouseId, productId, quantity, nearestWarehousePercentage);
                return Results.Ok("Stock distributed successfully.");
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });


        // Эндпоинт для балансировки запасов между складами
        group.MapPost("/balance", async (WarehouseService warehouseService, [FromQuery] Guid SourceWarehouseId, [FromQuery] Guid TargetWarehouseId, [FromQuery] Guid ProductId, [FromQuery] int Quantity) =>
        {
            try
            {
                await warehouseService.BalanceStockBetweenWarehouses(SourceWarehouseId, TargetWarehouseId, ProductId, Quantity);
                return Results.Ok("Stock balanced successfully.");
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });
        group.MapPost("/auto-balance", async (WarehouseService warehouseService, [FromQuery] Guid sourceWarehouseId, [FromQuery] Guid productId, [FromQuery] int quantity, [FromQuery] double nearestWarehousePercentage) =>
        {
            try
            {


                await warehouseService.DistributeStockAsync(sourceWarehouseId, productId, quantity,nearestWarehousePercentage);
                return Results.Ok("Запасы успешно сбалансированы.");
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

    }
}
