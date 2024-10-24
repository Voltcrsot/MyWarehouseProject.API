using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace ServiceRegistrationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Создаем новый контейнер сервисов
            var services = new ServiceCollection();

            // Явная регистрация репозиториев
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            services.AddScoped<IStockRepository, StockRepository>();

            // Явная регистрация сервисов
            services.AddScoped<WarehouseService>();
            services.AddScoped<StockService>();

            // Создаем провайдер сервисов
            var serviceProvider = services.BuildServiceProvider();

            // Вывод всех зарегистрированных сервисов
            Console.WriteLine("Зарегистрированные сервисы:");
            foreach (var service in services.Where(s => s.ServiceType.Name.EndsWith("Service")))
            {
                Console.WriteLine($"- {service.ServiceType.FullName}");
            }

            // Вывод всех зарегистрированных репозиториев
            Console.WriteLine("\nЗарегистрированные репозитории:");
            foreach (var repo in services.Where(s => s.ServiceType.IsInterface && s.ServiceType.Name.EndsWith("Repository")))
            {
                Console.WriteLine($"- {repo.ServiceType.FullName} -> {repo.ImplementationType?.FullName}");
            }

            // Проверка регистрации конкретного сервиса
            var warehouseService = serviceProvider.GetService<WarehouseService>();
            Console.WriteLine(warehouseService != null ? "\nWarehouseService зарегистрирован!" : "\nWarehouseService НЕ зарегистрирован!");

            // Проверка регистрации конкретного репозитория
            var warehouseRepository = serviceProvider.GetService<IWarehouseRepository>();
            Console.WriteLine(warehouseRepository != null ? "IWarehouseRepository зарегистрирован!" : "IWarehouseRepository НЕ зарегистрирован!");
        }
    }

    // Пример сервисов и репозиториев
    public class WarehouseService { }
    public class StockService { }

    public interface IWarehouseRepository { }
    public class WarehouseRepository : IWarehouseRepository { }

    public interface IStockRepository { }
    public class StockRepository : IStockRepository { }
}
