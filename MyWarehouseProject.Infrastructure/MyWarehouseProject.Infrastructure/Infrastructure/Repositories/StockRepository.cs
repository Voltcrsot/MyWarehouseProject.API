using Microsoft.EntityFrameworkCore;
using MyWarehouseProject.Domain.Entities;
using MyWarehouseProject.Domain.Repositories;
using MyWarehouseProject.Infrastructure.Data;
using System;
using System.Threading.Tasks;

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
        }




        public Task<Stock> GetStockByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
