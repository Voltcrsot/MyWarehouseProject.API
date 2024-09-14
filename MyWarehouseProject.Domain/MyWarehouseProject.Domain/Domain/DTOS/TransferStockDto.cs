namespace MyWarehouseProject.Application.DTOs
{
    public class TransferStockDto
    {
        public Guid ProductId { get; set; }
        public Guid SourceWarehouseId { get; set; }
        public Guid TargetWarehouseId { get; set; }
        public int Quantity { get; set; }
    }
}
