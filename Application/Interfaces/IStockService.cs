using MyWarehouseProject.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace MyWarehouseProject.Application.Interfaces
{
    public interface IStockService
    {
        Task<StockDto> GetStockByIdAsync(Guid id);
      
        Task AddStockAsync(StockDto stockDto);
        Task UpdateStockAsync(StockDto stockDto);
    }
}
