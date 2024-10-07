using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWarehouseProject.Domain.Entities;

namespace MyWarehouseProject.Domain.Repositories
{
    public interface IWarehouseRepository
    {
        Task<Warehouse> GetWarehouseByIdAsync(Guid id);
        Task<IEnumerable<Warehouse>> GetAllWarehousesAsync();
        Task AddWarehouseAsync(Warehouse warehouse);
        Task UpdateWarehouseAsync(Warehouse warehouse);
        Task DeleteWarehouseAsync(Guid id);
    }
}
