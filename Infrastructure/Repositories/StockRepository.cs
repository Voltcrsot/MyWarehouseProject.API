using Microsoft.EntityFrameworkCore;
using MyWarehouseProject.Domain.Entities;
using MyWarehouseProject.Domain.Repositories;
using MyWarehouseProject.Infrastructure.Data;

namespace MyWarehouseProject.Infrastructure.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly WarehouseDbContext _context;

        public StockRepository(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task<Stock> GetStockAsync(Guid warehouseId, Guid productId)
        {
            return await _context.Stocks
                .FirstOrDefaultAsync(s => s.WarehouseId == warehouseId && s.ProductId == productId);
        }

        public async Task<Stock> GetStockByProductIdAsync(Guid productId, Guid warehouseId)
        {
            return await _context.Stocks
                .FirstOrDefaultAsync(s => s.ProductId == productId && s.WarehouseId == warehouseId);
        }

        public async Task AddStockAsync(Stock stock)
        {
            await _context.Stocks.AddAsync(stock);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStockAsync(Stock stock)
        {
            var existingStock = await GetStockAsync(stock.WarehouseId, stock.ProductId);

            if (existingStock != null)
            {
                existingStock.Quantity = stock.Quantity; // Устанавливаем новое количество
                _context.Stocks.Update(existingStock);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Запас не найден для обновления.");
            }
        }

        public async Task<Stock> GetStockByIdAsync(Guid id)
        {
            return await _context.Stocks.FindAsync(id);
        }

        public async Task DeleteStockAsync(Guid warehouseId, Guid productId)
        {
            var stock = await GetStockAsync(warehouseId, productId);
            if (stock != null)
            {
                _context.Stocks.Remove(stock);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Запас не найден для удаления.");
            }
        }
    }
}
