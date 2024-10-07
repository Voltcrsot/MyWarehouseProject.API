using MyWarehouseProject.Application.DTOs;
using MyWarehouseProject.Application.Interfaces;
using MyWarehouseProject.Domain.Entities;
using MyWarehouseProject.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWarehouseProject.Application.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IStockRepository _stockRepository;

        public WarehouseService(IWarehouseRepository warehouseRepository, IStockRepository stockRepository)
        {
            _warehouseRepository = warehouseRepository;
            _stockRepository = stockRepository;
        }

        public async Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync()
        {
            var warehouses = await _warehouseRepository.GetAllWarehousesAsync();
            return warehouses.Select(w => new WarehouseDto
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
            });
        }

        public async Task<WarehouseDto> GetWarehouseByIdAsync(Guid id)
        {
            var warehouse = await _warehouseRepository.GetWarehouseByIdAsync(id);
            if (warehouse == null) return null;

            return new WarehouseDto
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
        }

        public async Task AddWarehouseAsync(WarehouseDto warehouseDto)
        {
            var warehouse = new Warehouse
            {
                Id = Guid.NewGuid(),
                Name = warehouseDto.Name,
                Stocks = warehouseDto.Stocks.Select(s => new Stock
                {
                    Id = s.Id,
                    ProductId = s.ProductId,
                    Quantity = s.Quantity,
                    WarehouseId = s.WarehouseId
                }).ToList()
            };

            await _warehouseRepository.AddWarehouseAsync(warehouse);
        }

        public async Task UpdateWarehouseAsync(WarehouseDto warehouseDto)
        {
            var warehouse = new Warehouse
            {
                Id = warehouseDto.Id,
                Name = warehouseDto.Name,
                Stocks = warehouseDto.Stocks.Select(s => new Stock
                {
                    Id = s.Id,
                    ProductId = s.ProductId,
                    Quantity = s.Quantity,
                    WarehouseId = s.WarehouseId
                }).ToList()
            };

            await _warehouseRepository.UpdateWarehouseAsync(warehouse);
        }

        public async Task DeleteWarehouseAsync(Guid id)
        {
            await _warehouseRepository.DeleteWarehouseAsync(id);
        }

        public async Task BalanceStockBetweenWarehouses(Guid sourceWarehouseId, Guid targetWarehouseId, Guid productId, int quantity)
        {
            var sourceStock = await _stockRepository.GetStockAsync(sourceWarehouseId, productId);
            var targetStock = await _stockRepository.GetStockAsync(targetWarehouseId, productId);

            if (sourceStock == null || sourceStock.Quantity < quantity)
            {
                throw new InvalidOperationException("Недостаточно товара на исходном складе.");
            }

            // Обновляем количество на исходном складе
            sourceStock.Quantity -= quantity;
            await _stockRepository.UpdateStockAsync(sourceStock);

            if (targetStock != null)
            {
                // Увеличиваем количество на целевом складе
                targetStock.Quantity += quantity;
                await _stockRepository.UpdateStockAsync(targetStock);
            }
            else
            {
                // Создаем новый запас на целевом складе
                var newStock = new Stock
                {
                    WarehouseId = targetWarehouseId,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _stockRepository.AddStockAsync(newStock);
            }
        }





        public async Task AutoBalanceStockAcrossWarehouses(Guid productId, Guid sourceWarehouseId)
        {
            // Получаем исходный склад
            var sourceWarehouse = await _warehouseRepository.GetWarehouseByIdAsync(sourceWarehouseId);
            var otherWarehouses = (await _warehouseRepository.GetAllWarehousesAsync())
                                  .Where(w => w.Id != sourceWarehouseId)
                                  .ToList();

            // Проверяем наличие исходного склада и других складов
            if (sourceWarehouse == null || !otherWarehouses.Any())
            {
                throw new InvalidOperationException("Исходный склад не найден или нет других складов.");
            }

            // Получаем запасы на исходном складе
            var sourceStock = sourceWarehouse.Stocks.FirstOrDefault(s => s.ProductId == productId);

            // Проверяем наличие товара на исходном складе
            if (sourceStock == null || sourceStock.Quantity <= 0)
            {
                throw new InvalidOperationException("Нет запасов данного продукта на исходном складе.");
            }

            // Распределяем товар по другим складам
            var totalQuantity = sourceStock.Quantity;
            var quantityPerWarehouse = totalQuantity / otherWarehouses.Count;
            var remainder = totalQuantity % otherWarehouses.Count;

            foreach (var warehouse in otherWarehouses)
            {
                var targetStock = warehouse.Stocks.FirstOrDefault(s => s.ProductId == productId);

                // Если товар уже есть на складе, обновляем его количество
                if (targetStock != null)
                {
                    targetStock.Quantity += quantityPerWarehouse;
                }
                else
                {
                    // Иначе создаем новый объект Stock для этого склада
                    targetStock = new Stock
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        WarehouseId = warehouse.Id,
                        Quantity = quantityPerWarehouse
                    };
                    await _stockRepository.AddStockAsync(targetStock);
                }

                // Сохраняем изменения
                await _stockRepository.AddStockAsync(targetStock);
            }

            // Оставляем остаток товара на исходном складе
            sourceStock.Quantity = remainder;
            await _stockRepository.AddStockAsync(sourceStock);
        }

    }
}
