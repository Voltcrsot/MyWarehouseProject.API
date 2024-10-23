using MyWarehouseProject.Application.Services;
using MyWarehouseProject.Application.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
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

        // Эндпоинт для балансировки запасов между складами
        group.MapPost("/balance", async (WarehouseService warehouseService, [FromQuery] Guid sourceWarehouseId, [FromQuery] Guid targetWarehouseId, [FromQuery] Guid productId, [FromQuery] int quantity) =>
        {
            try
            {
                await warehouseService.BalanceStockBetweenWarehouses(sourceWarehouseId, targetWarehouseId, productId, quantity);
                return Results.Ok("Stock balanced successfully.");
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        // Эндпоинт для автоматической балансировки запасов по складам
        group.MapPost("/auto-balance", async (WarehouseService warehouseService, [FromQuery] Guid productId, [FromQuery] Guid sourceWarehouseId) =>
        {
            try
            {
                await warehouseService.AutoBalanceStockAcrossWarehouses(productId, sourceWarehouseId);
                return Results.Ok("Stock auto-balanced successfully.");
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });
    }
}
