using MyWarehouseProject.Application.DTOs;
using MyWarehouseProject.Application.Interfaces;
using MyWarehouseProject.Domain.Entities;
using MyWarehouseProject.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace MyWarehouseProject.Application.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;

        public StockService(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task<StockDto> GetStockByIdAsync(Guid id)
        {
            var stock = await _stockRepository.GetStockByIdAsync(id);
            return stock == null ? null : new StockDto
            {
                Id = stock.Id,
                ProductId = stock.ProductId,
                Quantity = stock.Quantity,
                WarehouseId = stock.WarehouseId
            };
        }


        public async Task AddStockAsync(StockDto stockDto)
        {
            var stock = new Stock
            {
                Id = Guid.NewGuid(),
                ProductId = stockDto.ProductId,
                Quantity = stockDto.Quantity,
                WarehouseId = stockDto.WarehouseId
            };

            await _stockRepository.AddStockAsync(stock);
        }

        public async Task UpdateStockAsync(StockDto stockDto)
        {
            var stock = new Stock
            {
                Id = stockDto.Id,
                ProductId = stockDto.ProductId,
                Quantity = stockDto.Quantity,
                WarehouseId = stockDto.WarehouseId
            };

            await _stockRepository.AddStockAsync(stock);
        }
    }
}
