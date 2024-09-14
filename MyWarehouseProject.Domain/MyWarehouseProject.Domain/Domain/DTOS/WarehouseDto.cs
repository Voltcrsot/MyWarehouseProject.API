namespace MyWarehouseProject.Application.DTOs
{
    public class WarehouseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<StockDto> Stocks { get; set; } = new List<StockDto>();
    }

}
