using AutoMapper;
using MyWarehouseProject.Application.DTOs;
using MyWarehouseProject.Domain.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Warehouse, WarehouseDto>().ReverseMap();
        CreateMap<Stock, StockDto>().ReverseMap();
    }
}
