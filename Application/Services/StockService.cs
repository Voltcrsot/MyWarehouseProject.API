using AutoMapper;
using MyWarehouseProject.Application.DTOs;
using MyWarehouseProject.Domain.Entities;
using MyWarehouseProject.Domain.Repositories;

namespace MyWarehouseProject.Application.Services
{
    public class StockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IWarehouseRepository _warehouseRepository; // Добавляем IWarehouseRepository
        private readonly IMapper _mapper;

        public StockService(IStockRepository stockRepository, IWarehouseRepository warehouseRepository, IMapper mapper)
        {
            _stockRepository = stockRepository;
            _warehouseRepository = warehouseRepository; // Инициализация IWarehouseRepository
            _mapper = mapper;
        }

        public async Task<StockDto> GetStockByIdAsync(Guid id)
        {
            var stock = await _stockRepository.GetStockByIdAsync(id);
            return _mapper.Map<StockDto>(stock); // Используем AutoMapper для маппинга
        }

        public async Task AddStockAsync(StockDto stockDto)
        {
            // Получаем склад, чтобы проверить его вместимость
            var warehouse = await _warehouseRepository.GetWarehouseByIdAsync(stockDto.WarehouseId); // Используем IWarehouseRepository
            if (warehouse == null)
            {
                throw new Exception("Склад не найден.");
            }

            var existingStock = await _stockRepository.GetStockByProductIdAsync(stockDto.ProductId, stockDto.WarehouseId);
            if (existingStock != null)
            {
                // Если запас уже существует, увеличиваем его количество
                int newQuantity = existingStock.Quantity + stockDto.Quantity;

                // Проверяем, не превышает ли новое количество вместимость склада
                if (newQuantity > warehouse.Capacity)
                {
                    throw new Exception("Недостаточно места на складе для добавления этого количества товара.");
                }

                existingStock.Quantity = newQuantity;
                await _stockRepository.UpdateStockAsync(existingStock);
            }
            else
            {
                // Если запас не существует, создаём новый
                var stock = _mapper.Map<Stock>(stockDto);
                stock.Id = Guid.NewGuid(); // Генерируем новый Id

                // Проверяем вместимость при добавлении нового запаса
                if (stock.Quantity > warehouse.Capacity)
                {
                    throw new Exception("Недостаточно места на складе для добавления этого товара.");
                }

                await _stockRepository.AddStockAsync(stock);
            }
        }

        public async Task UpdateStockAsync(StockDto stockDto)
        {
            var stock = _mapper.Map<Stock>(stockDto); // Используем AutoMapper для маппинга

            // Получаем склад, чтобы проверить его вместимость
            var warehouse = await _warehouseRepository.GetWarehouseByIdAsync(stock.WarehouseId); // Используем IWarehouseRepository
            if (warehouse == null)
            {
                throw new Exception("Склад не найден.");
            }

            var existingStock = await _stockRepository.GetStockByIdAsync(stock.Id);
            if (existingStock == null)
            {
                throw new Exception("Запас не найден.");
            }

            // Проверяем, не превышает ли новое количество вместимость склада
            if (stock.Quantity > warehouse.Capacity)
            {
                throw new Exception("Недостаточно места на складе для обновления этого количества товара.");
            }

            await _stockRepository.UpdateStockAsync(stock);
        }
    }
}
