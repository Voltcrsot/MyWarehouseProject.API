using AutoMapper;
using MyWarehouseProject.Application.DTOs;
using MyWarehouseProject.Domain.Entities;
using MyWarehouseProject.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWarehouseProject.Application.Services
{
    public class WarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IMapper _mapper; // Внедрение AutoMapper

        public WarehouseService(IWarehouseRepository warehouseRepository, IStockRepository stockRepository, IMapper mapper)
        {
            _warehouseRepository = warehouseRepository;
            _stockRepository = stockRepository;
            _mapper = mapper; // Инициализация AutoMapper
        }

        public async Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync()
        {
            var warehouses = await _warehouseRepository.GetAllWarehousesAsync();
            return _mapper.Map<IEnumerable<WarehouseDto>>(warehouses); // Используем AutoMapper
        }

        public async Task<WarehouseDto> GetWarehouseByIdAsync(Guid id)
        {
            var warehouse = await _warehouseRepository.GetWarehouseByIdAsync(id);
            return _mapper.Map<WarehouseDto>(warehouse); // Используем AutoMapper
        }

        public async Task AddWarehouseAsync(WarehouseDto warehouseDto)
        {
            var warehouse = _mapper.Map<Warehouse>(warehouseDto); // Используем AutoMapper
            warehouse.Id = Guid.NewGuid(); // Генерируем новый Id
            await _warehouseRepository.AddWarehouseAsync(warehouse);
        }

        public async Task UpdateWarehouseAsync(WarehouseDto warehouseDto)
        {
            var warehouse = _mapper.Map<Warehouse>(warehouseDto); // Используем AutoMapper
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

            sourceStock.Quantity -= quantity;
            await _stockRepository.UpdateStockAsync(sourceStock);

            if (targetStock != null)
            {
                targetStock.Quantity += quantity;
                await _stockRepository.UpdateStockAsync(targetStock);
            }
            else
            {
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
            var sourceWarehouse = await _warehouseRepository.GetWarehouseByIdAsync(sourceWarehouseId);
            var otherWarehouses = (await _warehouseRepository.GetAllWarehousesAsync())
                                  .Where(w => w.Id != sourceWarehouseId)
                                  .ToList();

            if (sourceWarehouse == null || !otherWarehouses.Any())
            {
                throw new InvalidOperationException("Исходный склад не найден или нет других складов.");
            }

            var sourceStock = sourceWarehouse.Stocks.FirstOrDefault(s => s.ProductId == productId);
            if (sourceStock == null || sourceStock.Quantity <= 0)
            {
                throw new InvalidOperationException("Нет запасов данного продукта на исходном складе.");
            }

            var totalQuantity = sourceStock.Quantity;
            var quantityPerWarehouse = totalQuantity / otherWarehouses.Count;
            var remainder = totalQuantity % otherWarehouses.Count;

            foreach (var warehouse in otherWarehouses)
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
                    await _stockRepository.AddStockAsync(targetStock);
                }
            }

            // Уменьшаем количество товара на исходном складе
            sourceStock.Quantity -= totalQuantity;
            await _stockRepository.UpdateStockAsync(sourceStock);
        }
    }
}
