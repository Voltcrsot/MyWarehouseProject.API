namespace MyWarehouseProject.Application.DTOs
{
    public class StockDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid WarehouseId { get; set; }
    }

}
