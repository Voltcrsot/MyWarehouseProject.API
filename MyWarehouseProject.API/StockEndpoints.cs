using MyWarehouseProject.Application.Interfaces;
using MyWarehouseProject.Application.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using Microsoft.AspNetCore.Mvc;

public static class StockEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/stocks");

        group.MapGet("/{id:guid}", async (IStockService stockService, Guid id) =>
        {
            var stock = await stockService.GetStockByIdAsync(id);
            return stock != null ? Results.Ok(stock) : Results.NotFound();
        });

        group.MapPost("/", async (IStockService stockService, [FromBody] StockDto stockDto) =>
        {
            await stockService.AddStockAsync(stockDto);
            return Results.Created($"/stocks/{stockDto.Id}", stockDto);
        });

        group.MapPut("/{id:guid}", async (IStockService stockService, Guid id, [FromBody] StockDto stockDto) =>
        {
            stockDto.Id = id;
            await stockService.UpdateStockAsync(stockDto);
            return Results.NoContent();
        });
    }
}
