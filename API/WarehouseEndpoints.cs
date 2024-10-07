using MyWarehouseProject.Application.Interfaces;
using MyWarehouseProject.Application.DTOs;
using MyWarehouseProject.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MyWarehouseProject.Domain.Entities;
using System;
using Microsoft.AspNetCore.Mvc;

public static class WarehouseEndpoints
{
        public static void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/warehouses");

            group.MapGet("/", async (IWarehouseService warehouseService) =>
            {
                var warehouses = await warehouseService.GetAllWarehousesAsync();
                return Results.Ok(warehouses);
            });

            group.MapGet("/{id:guid}", async (IWarehouseService warehouseService, Guid id) =>
            {
                var warehouse = await warehouseService.GetWarehouseByIdAsync(id);
                return warehouse != null ? Results.Ok(warehouse) : Results.NotFound();
            });

            group.MapPost("/", async (IWarehouseService warehouseService, [FromBody] WarehouseDto warehouseDto) =>
            {
                await warehouseService.AddWarehouseAsync(warehouseDto);
                return Results.Created($"/warehouses/{warehouseDto.Id}", warehouseDto);
            });

            group.MapPut("/{id:guid}", async (IWarehouseService warehouseService, Guid id, [FromBody] WarehouseDto warehouseDto) =>
            {
                warehouseDto.Id = id;
                await warehouseService.UpdateWarehouseAsync(warehouseDto);
                return Results.NoContent();
            });

            group.MapDelete("/{id:guid}", async (IWarehouseService warehouseService, Guid id) =>
            {
                await warehouseService.DeleteWarehouseAsync(id);
                return Results.NoContent();
            });

            group.MapPost("/balance", async (IWarehouseService warehouseService, [FromQuery] Guid sourceWarehouseId, [FromQuery] Guid targetWarehouseId, [FromQuery] Guid productId, [FromQuery] int quantity) =>
            {
                await warehouseService.BalanceStockBetweenWarehouses(sourceWarehouseId, targetWarehouseId, productId, quantity);
                return Results.Ok();
            });

            group.MapPost("/auto-balance", async (IWarehouseService warehouseService, [FromQuery] Guid productId, [FromQuery] Guid sourceWarehouseId) =>
            {
                await warehouseService.AutoBalanceStockAcrossWarehouses(productId, sourceWarehouseId);
                return Results.Ok();
            });
        }
}

