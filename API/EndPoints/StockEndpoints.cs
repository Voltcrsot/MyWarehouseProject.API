using MyWarehouseProject.Application.Services;
using MyWarehouseProject.Application.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

public static class StockEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/stocks");

        // Получение информации о конкретном запасе по ID
        group.MapGet("/{id:guid}", async (StockService stockService, Guid id) =>
        {
            var stock = await stockService.GetStockByIdAsync(id);
            return stock != null ? Results.Ok(stock) : Results.NotFound();
        });

        // Добавление нового запаса
        group.MapPost("/", async (StockService stockService, [FromBody] StockDto stockDto) =>
        {
            await stockService.AddStockAsync(stockDto);
            return Results.Created($"/stocks/{stockDto.Id}", stockDto);
        });

        // Обновление информации о запасе
        group.MapPut("/{id:guid}", async (StockService stockService, Guid id, [FromBody] StockDto stockDto) =>
        {
            stockDto.Id = id;
            await stockService.UpdateStockAsync(stockDto);
            return Results.NoContent();
        });
    }
}
    