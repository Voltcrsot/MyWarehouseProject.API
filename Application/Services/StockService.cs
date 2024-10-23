using AutoMapper;
using MyWarehouseProject.Application.DTOs;
using MyWarehouseProject.Domain.Entities;
using MyWarehouseProject.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace MyWarehouseProject.Application.Services
{
    public class StockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IMapper _mapper;

        public StockService(IStockRepository stockRepository, IMapper mapper)
        {
            _stockRepository = stockRepository;
            _mapper = mapper;
        }

        public async Task<StockDto> GetStockByIdAsync(Guid id)
        {
            var stock = await _stockRepository.GetStockByIdAsync(id);
            return _mapper.Map<StockDto>(stock); // Используем AutoMapper для маппинга
        }

        public async Task AddStockAsync(StockDto stockDto)
        {
            var stock = _mapper.Map<Stock>(stockDto); // Используем AutoMapper для маппинга
            stock.Id = Guid.NewGuid(); // Генерируем новый Id
            await _stockRepository.AddStockAsync(stock);
        }

        public async Task UpdateStockAsync(StockDto stockDto)
        {
            var stock = _mapper.Map<Stock>(stockDto); // Используем AutoMapper для маппинга
            await _stockRepository.UpdateStockAsync(stock);
        }
    }
}
