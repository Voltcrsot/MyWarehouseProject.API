using Microsoft.EntityFrameworkCore;
using MyWarehouseProject.Domain.Entities;
using MyWarehouseProject.Domain.Repositories;
using MyWarehouseProject.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWarehouseProject.Infrastructure.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly WarehouseDbContext _context;

        public WarehouseRepository(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task<Warehouse> GetWarehouseByIdAsync(Guid id)
        {
            return await _context.Warehouses
                                 .Include(w => w.Stocks)
                                 .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<IEnumerable<Warehouse>> GetAllWarehousesAsync()
        {
            return await _context.Warehouses
                                 .Include(w => w.Stocks)
                                 .ToListAsync();
        }

        public async Task AddWarehouseAsync(Warehouse warehouse)
        {
            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateWarehouseAsync(Warehouse warehouse)
        {
            _context.Warehouses.Update(warehouse);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteWarehouseAsync(Guid id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse != null)
            {
                _context.Warehouses.Remove(warehouse);
                await _context.SaveChangesAsync();
            }
        }

        // Метод для балансировки запасов между складами
        public async Task BalanceStockBetweenWarehouses(Guid sourceWarehouseId, Guid targetWarehouseId, Guid productId, int quantity)
        {
            var sourceStock = await GetStockAsync(sourceWarehouseId, productId);
            var targetStock = await GetStockAsync(targetWarehouseId, productId);

            if (sourceStock == null || sourceStock.Quantity < quantity)
            {
                throw new InvalidOperationException("Недостаточно товара на исходном складе.");
            }

            // Уменьшаем количество товара на исходном складе
            sourceStock.Quantity -= quantity;
            _context.Stocks.Update(sourceStock);

            // Если на целевом складе уже есть запас, увеличиваем его
            if (targetStock != null)
            {
                targetStock.Quantity += quantity;
                _context.Stocks.Update(targetStock);
            }
            else
            {
                var newStock = new Stock
                {
                    WarehouseId = targetWarehouseId,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _context.Stocks.AddAsync(newStock);
            }

            await _context.SaveChangesAsync();
        }

        // Метод для автоматической балансировки запасов по складам
        public async Task AutoBalanceStockAcrossWarehouses(Guid productId, Guid sourceWarehouseId)
        {
            var sourceWarehouse = await GetWarehouseByIdAsync(sourceWarehouseId);
            var otherWarehouses = await GetAllWarehousesAsync();

            if (sourceWarehouse == null || !otherWarehouses.Any(w => w.Id != sourceWarehouseId))
            {
                throw new InvalidOperationException("Исходный склад не найден или нет других складов.");
            }

            var sourceStock = sourceWarehouse.Stocks.FirstOrDefault(s => s.ProductId == productId);
            if (sourceStock == null || sourceStock.Quantity <= 0)
            {
                throw new InvalidOperationException("Нет запасов данного продукта на исходном складе.");
            }

            var totalQuantity = sourceStock.Quantity;
            var targetWarehouseCount = otherWarehouses.Count(w => w.Id != sourceWarehouseId);

            if (targetWarehouseCount == 0)
            {
                throw new InvalidOperationException("Нет других складов для балансировки.");
            }

            var quantityPerWarehouse = totalQuantity / targetWarehouseCount;
            var remainder = totalQuantity % targetWarehouseCount;

            foreach (var warehouse in otherWarehouses.Where(w => w.Id != sourceWarehouseId))
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
                        ProductId = productId,
                        WarehouseId = warehouse.Id,
                        Quantity = quantityPerWarehouse
                    };
                    await _context.Stocks.AddAsync(targetStock);
                }
            }

            // Обработка остатка, если есть
            if (remainder > 0)
            {
                var firstWarehouse = otherWarehouses.First(w => w.Id != sourceWarehouseId);
                var firstStock = firstWarehouse.Stocks.FirstOrDefault(s => s.ProductId == productId);
                if (firstStock != null)
                {
                    firstStock.Quantity += remainder;
                }
                else
                {
                    firstStock = new Stock
                    {
                        ProductId = productId,
                        WarehouseId = firstWarehouse.Id,
                        Quantity = remainder
                    };
                    await _context.Stocks.AddAsync(firstStock);
                }
            }

            // Уменьшаем количество товара на исходном складе
            sourceStock.Quantity = 0; // Обнуляем количество, т.к. все запасы перераспределены
            _context.Stocks.Update(sourceStock);

            await _context.SaveChangesAsync();
        }

        private async Task<Stock> GetStockAsync(Guid warehouseId, Guid productId)
        {
            return await _context.Stocks
                .FirstOrDefaultAsync(s => s.WarehouseId == warehouseId && s.ProductId == productId);
        }
    }
}
