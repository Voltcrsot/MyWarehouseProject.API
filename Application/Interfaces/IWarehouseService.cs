using MyWarehouseProject.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyWarehouseProject.Application.Interfaces
{
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync();
        Task<WarehouseDto> GetWarehouseByIdAsync(Guid id);
        Task AddWarehouseAsync(WarehouseDto warehouseDto);
        Task UpdateWarehouseAsync(WarehouseDto warehouseDto);
        Task DeleteWarehouseAsync(Guid id);
        Task BalanceStockBetweenWarehouses(Guid sourceWarehouseId, Guid targetWarehouseId, Guid productId, int quantity);
        Task AutoBalanceStockAcrossWarehouses(Guid productId, Guid sourceWarehouseId);
    }
}
