

namespace MyWarehouseProject.Application.DTOs
{
    public class AutoBalanceRequestDto
    {
        public Guid ProductId { get; set; }
        public Guid SourceWarehouseId { get; set; }
    }
}
