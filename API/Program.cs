using MyWarehouseProject.Application.Services;
using MyWarehouseProject.Domain.Repositories;
using MyWarehouseProject.Infrastructure.Repositories;
using MyWarehouseProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Получаем строку подключения из appsettings.json
var connectionString = builder.Configuration.GetConnectionString("WarehouseDb");

// Регистрация контекста базы данных через метод расширения для SQLite
builder.Services.AddDbContext<WarehouseDbContext>(options =>
    options.UseSqlite(connectionString));

// Явная регистрация репозиториев
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();

// Явная регистрация сервисов
builder.Services.AddScoped<WarehouseService>();
builder.Services.AddScoped<StockService>();

// Настройка сериализации JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

// Настройка AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly); 

// Настройка Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Регистрация Endpoints
WarehouseEndpoints.MapEndpoints(app);
StockEndpoints.MapEndpoints(app);

app.Run();

