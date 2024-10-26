namespace MyWarehouseProject.Domain.Entities
{
    public class Warehouse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public ICollection<Stock>? Stocks { get; set; }

        // Добавлены координаты для определения местоположения склада
        public double Latitude { get; set; }  // Широта
        public double Longitude { get; set; } // Долгота

        // Новое свойство для хранения емкости склада
        public int Capacity { get; set; } // Максимальная вместимость склада
    }
}
