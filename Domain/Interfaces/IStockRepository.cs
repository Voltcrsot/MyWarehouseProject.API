using MyWarehouseProject.Domain.Entities;

namespace MyWarehouseProject.Domain.Repositories
{
    public interface IStockRepository
    {
        Task<Stock> GetStockByIdAsync(Guid id);
        Task AddStockAsync(Stock stock);
        Task<Stock> GetStockAsync(Guid sourceWarehouseId, Guid productId);
        Task<Stock> GetStockByProductIdAsync(Guid productId, Guid warehouseId); // Добавьте этот метод
        Task UpdateStockAsync(Stock sourceStock);
    }
}
