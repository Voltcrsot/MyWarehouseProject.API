using Microsoft.EntityFrameworkCore;
using MyWarehouseProject.Domain.Entities;

namespace MyWarehouseProject.Infrastructure.Data
{
    public class WarehouseDbContext : DbContext
    {
        public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
            : base(options)
        {
        }

        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Stock> Stocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Определение связей между сущностями
            modelBuilder.Entity<Warehouse>()
                .HasMany(w => w.Stocks)
                .WithOne(s => s.Warehouse)
                .HasForeignKey(s => s.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Создание складов с фиксированными именами, координатами и емкостью
            var warehouses = new List<Warehouse>
            {
                new Warehouse { Id = Guid.NewGuid(), Name = "Central Warehouse", Latitude = 40.7128, Longitude = -74.0060, Capacity = 1000 },
                new Warehouse { Id = Guid.NewGuid(), Name = "East Side Depot", Latitude = 41.8781, Longitude = -87.6298, Capacity = 800 },
                new Warehouse { Id = Guid.NewGuid(), Name = "West End Storage", Latitude = 34.0522, Longitude = -118.2437, Capacity = 600 },
                new Warehouse { Id = Guid.NewGuid(), Name = "North Hub", Latitude = 37.7749, Longitude = -122.4194, Capacity = 500 },
                new Warehouse { Id = Guid.NewGuid(), Name = "South Distribution Center", Latitude = 34.0522, Longitude = -118.2437, Capacity = 750 },
                new Warehouse { Id = Guid.NewGuid(), Name = "Warehouse 6", Latitude = 30.2672, Longitude = -97.7431, Capacity = 400 },
                new Warehouse { Id = Guid.NewGuid(), Name = "Warehouse 7", Latitude = 40.4406, Longitude = -79.9959, Capacity = 300 },
                new Warehouse { Id = Guid.NewGuid(), Name = "Warehouse 8", Latitude = 42.3314, Longitude = -83.0458, Capacity = 450 },
                new Warehouse { Id = Guid.NewGuid(), Name = "Warehouse 9", Latitude = 39.7392, Longitude = -104.9903, Capacity = 600 },
                new Warehouse { Id = Guid.NewGuid(), Name = "Warehouse 10", Latitude = 38.9072, Longitude = -77.0369, Capacity = 500 },
                new Warehouse { Id = Guid.NewGuid(), Name = "Warehouse 11", Latitude = 34.0522, Longitude = -118.2437, Capacity = 300 },
                new Warehouse { Id = Guid.NewGuid(), Name = "Warehouse 12", Latitude = 47.6062, Longitude = -122.3321, Capacity = 650 },
                new Warehouse { Id = Guid.NewGuid(), Name = "Warehouse 13", Latitude = 36.1699, Longitude = -115.1398, Capacity = 700 },
                new Warehouse { Id = Guid.NewGuid(), Name = "Warehouse 14", Latitude = 41.2565, Longitude = -95.9345, Capacity = 500 },
                new Warehouse { Id = Guid.NewGuid(), Name = "Warehouse 15", Latitude = 39.9612, Longitude = -82.9988, Capacity = 550 }
            };

            modelBuilder.Entity<Warehouse>().HasData(warehouses);

            // Создание общего ProductId для всех запасов
            var productId = Guid.NewGuid(); // Один productId для всех продуктов

            // Создание запасов для каждого склада с количеством, не превышающим вместимость
            var stockEntries = new List<Stock>
            {
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[0].Id, Quantity = 100 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[1].Id, Quantity = 200 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[2].Id, Quantity = 150 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[3].Id, Quantity = 250 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[4].Id, Quantity = 200 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[5].Id, Quantity = 300 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[6].Id, Quantity = 100 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[7].Id, Quantity = 250 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[8].Id, Quantity = 300 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[9].Id, Quantity = 400 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[10].Id, Quantity = 150 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[11].Id, Quantity = 200 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[12].Id, Quantity = 350 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[13].Id, Quantity = 300 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[14].Id, Quantity = 450 }
            };

            modelBuilder.Entity<Stock>().HasData(stockEntries);
        }
    }
}
