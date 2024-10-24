namespace MyWarehouseProject.Domain.Entities
{
    public class Warehouse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public ICollection<Stock>? Stocks { get; set; }
    }
}
