using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyWarehouseProject.Application.Services;
using MyWarehouseProject.Domain.Repositories;
using MyWarehouseProject.Infrastructure.Data;
using System;
using System.Linq;
using System.Reflection;

namespace MyWarehouseProject.Infrastructure.Service
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWarehouseDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<WarehouseDbContext>(options =>
                options.UseSqlite(connectionString));
            return services;
        }

//        public static IServiceCollection AddServicesAutomatically(this IServiceCollection services)
//        {
//            var assembly = Assembly.GetExecutingAssembly();
//            var serviceTypes = assembly.GetTypes()
//                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"));

//            foreach (var serviceType in serviceTypes)
//            {
//                var interfaceType = serviceType.GetInterfaces()
//                    .FirstOrDefault(i => i.Name == $"I{serviceType.Name}");

//                if (interfaceType != null)
//                {
//                    services.AddScoped(interfaceType, serviceType);
//                }
//            }

////            return services;
////        }

////       public static IServiceCollection AddRepositoriesAutomatically(this IServiceCollection services)
////{
////    var assembly = Assembly.GetExecutingAssembly();
////    var interfaceTypes = assembly.GetTypes()
////        .Where(t => t.IsInterface && t.Name.StartsWith("I"));

////    foreach (var interfaceType in interfaceTypes)
////    {
////        var implementationTypes = assembly.GetTypes()
////            .Where(t => interfaceType.IsAssignableFrom(t) && !t.IsInterface);

////        foreach (var implementationType in implementationTypes)
////        {
////            services.AddScoped(interfaceType, implementationType);
////        }
////    }

////    return services;
////}

    }
}

