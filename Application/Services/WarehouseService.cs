using AutoMapper;
using MyWarehouseProject.Application.DTOs;
using MyWarehouseProject.Domain.DTOS;
using MyWarehouseProject.Domain.Entities;
using MyWarehouseProject.Domain.Repositories;

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

        public async Task DistributeStockAsync(Guid sourceWarehouseId, Guid productId, int quantity, double nearestWarehousePercentage)
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
            if (sourceStock == null || sourceStock.Quantity < quantity)
            {
                throw new InvalidOperationException("Недостаточно запасов данного продукта на исходном складе.");
            }

            // Шаг 1: Найти ближайший склад
            var closestWarehouse = otherWarehouses.OrderBy(w => CalculateDistance(sourceWarehouse, w)).First();

            // Шаг 2: Вычислить объемы
            int nearbyQuantity = (int)(quantity * (nearestWarehousePercentage / 100));
            int remainingQuantity = quantity - nearbyQuantity;

            // Шаг 3: Распределить ближайшему складу
            await AddStockToWarehouse(closestWarehouse, productId, nearbyQuantity);

            // Шаг 4: Распределить оставшийся объем между другими складами
            if (remainingQuantity > 0)
            {
                await DistributeRemainingStock(otherWarehouses, closestWarehouse, productId, remainingQuantity);
            }

            // Шаг 5: Обновить исходные запасы
            sourceStock.Quantity -= quantity;
            await _stockRepository.UpdateStockAsync(sourceStock);
        }


        private async Task AddStockToWarehouse(Warehouse warehouse, Guid productId, int quantity)
        {
            var stock = warehouse.Stocks.FirstOrDefault(s => s.ProductId == productId);
            if (stock != null)
            {
                stock.Quantity += quantity;
            }
            else
            {
                var newStock = new Stock
                {
                    WarehouseId = warehouse.Id,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _stockRepository.AddStockAsync(newStock);
            }
        }

        private async Task DistributeRemainingStock(IEnumerable<Warehouse> otherWarehouses, Warehouse closestWarehouse, Guid productId, int remainingQuantity)
        {
            var validWarehouses = otherWarehouses.Where(w => w.Id != closestWarehouse.Id).ToList();
            if (!validWarehouses.Any())
            {
                throw new InvalidOperationException("Нет доступных складов для распределения остатка.");
            }

            int quantityPerWarehouse = remainingQuantity / validWarehouses.Count;
            foreach (var warehouse in validWarehouses)
            {
                await AddStockToWarehouse(warehouse, productId, quantityPerWarehouse);
            }
        }

        private double CalculateDistance(Warehouse source, Warehouse target)
        {
            return CalculateDistance(source.Latitude, source.Longitude, target.Latitude, target.Longitude);
        }

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Радиус Земли в километрах
            var lat = (lat2 - lat1) * Math.PI / 180;
            var lon = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(lat / 2) * Math.Sin(lat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(lon / 2) * Math.Sin(lon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Возвращаем расстояние в километрах
        }

        public async Task BalanceStockBetweenWarehouses(Guid sourceWarehouseId, Guid targetWarehouseId, Guid productId, int quantity)
        {
            var sourceStock = await _stockRepository.GetStockAsync(sourceWarehouseId, productId);
            var targetStock = await _stockRepository.GetStockAsync(targetWarehouseId, productId);
            var sourceWarehouse = await _warehouseRepository.GetWarehouseByIdAsync(sourceWarehouseId);
            var targetWarehouse = await _warehouseRepository.GetWarehouseByIdAsync(targetWarehouseId);

            if (sourceStock == null || sourceStock.Quantity < quantity)
            {
                throw new InvalidOperationException("Недостаточно товара на исходном складе.");
            }

            // Проверяем вместимость целевого склада
            if (targetStock != null)
            {
                if (targetStock.Quantity + quantity > targetWarehouse.Capacity)
                {
                    throw new InvalidOperationException("Недостаточно места на целевом складе для добавления этого количества товара.");
                }

                targetStock.Quantity += quantity;
                await _stockRepository.UpdateStockAsync(targetStock);
            }
            else
            {
                if (quantity > targetWarehouse.Capacity)
                {
                    throw new InvalidOperationException("Недостаточно места на целевом складе для добавления этого товара.");
                }

                var newStock = new Stock
                {
                    WarehouseId = targetWarehouseId,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _stockRepository.AddStockAsync(newStock);
            }

            sourceStock.Quantity -= quantity;
            await _stockRepository.UpdateStockAsync(sourceStock);
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

                // Проверяем вместимость целевого склада перед добавлением
                if (targetStock != null)
                {
                    if (targetStock.Quantity + quantityPerWarehouse > warehouse.Capacity)
                    {
                        throw new InvalidOperationException($"Недостаточно места на складе {warehouse.Name} для добавления этого количества товара.");
                    }

                    targetStock.Quantity += quantityPerWarehouse;
                }
                else
                {
                    if (quantityPerWarehouse > warehouse.Capacity)
                    {
                        throw new InvalidOperationException($"Недостаточно места на складе {warehouse.Name} для добавления этого товара.");
                    }

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
