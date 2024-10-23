using Microsoft.EntityFrameworkCore;
using MyWarehouseProject.Domain.Entities;
using System;
using System.Collections.Generic;

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

            // Создание складов с фиксированными именами и ID
            var warehouses = new List<Warehouse>
            {
                new Warehouse { Id = Guid.NewGuid(), Name = "Central Warehouse" },
                new Warehouse { Id = Guid.NewGuid(), Name = "East Side Depot" },
                new Warehouse { Id = Guid.NewGuid(), Name = "West End Storage" },
                new Warehouse { Id = Guid.NewGuid(), Name = "North Hub" },
                new Warehouse { Id = Guid.NewGuid(), Name = "South Distribution Center" },
                new Warehouse { Id = Guid.NewGuid(), Name = "Warehouse 6" },
                new Warehouse { Id = Guid.NewGuid(), Name = "Warehouse 7" },
                new Warehouse { Id = Guid.NewGuid(), Name = "Warehouse 8" },
                new Warehouse { Id = Guid.NewGuid(), Name = "Warehouse 9" },
                new Warehouse { Id = Guid.NewGuid(), Name = "Warehouse 10" }
            };

            modelBuilder.Entity<Warehouse>().HasData(warehouses);

            // Задаём productId для всех складов
            var productId = Guid.NewGuid(); // Один productId для всех продуктов

            // Создание запасов для каждого склада с одинаковым продуктом
            var stockEntries = new List<Stock>
            {
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[0].Id, Quantity = 100 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[1].Id, Quantity = 200 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[2].Id, Quantity = 150 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[3].Id, Quantity = 300 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[4].Id, Quantity = 250 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[5].Id, Quantity = 400 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[6].Id, Quantity = 100 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[7].Id, Quantity = 500 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[8].Id, Quantity = 350 },
                new Stock { Id = Guid.NewGuid(), ProductId = productId, WarehouseId = warehouses[9].Id, Quantity = 450 }
            };

            modelBuilder.Entity<Stock>().HasData(stockEntries);

        }
    }
}
