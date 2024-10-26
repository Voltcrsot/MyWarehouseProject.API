namespace MyWarehouseProject.Application.DTOs
{
    public class WarehouseDto
    {
         public Guid Id { get; set; }
        public string? Name { get; set; }
        public List<StockDto>? Stocks { get; set; } = new List<StockDto>();

        // Добавлены координаты для определения местоположения склада
        public double Latitude { get; set; }  // Широта
        public double Longitude { get; set; } // Долгота

        // Новое свойство для хранения емкости склада
        public int Capacity { get; set; } // Максимальная вместимость склада
    }

}
