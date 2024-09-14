using Microsoft.EntityFrameworkCore;
using MyWarehouseProject.Domain.Entities;
using MyWarehouseProject.Domain.Repositories;
using MyWarehouseProject.Infrastructure.Data;
using System;
using System.Collections.Generic;
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
    }
}
