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
        Task BalanceStockBetweenWarehouses(Guid sourceWarehouseId, Guid targetWarehouseId, Guid productId, int quantity);
        Task AutoBalanceStockAcrossWarehouses(Guid productId, Guid sourceWarehouseId);
    }
}
