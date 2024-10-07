using MyWarehouseProject.Infrastructure.Repositories;
using MyWarehouseProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using MyWarehouseProject.Domain.Repositories;
using MyWarehouseProject.Application.Interfaces;
using MyWarehouseProject.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<WarehouseDbContext>(options =>
    options.UseSqlite("Data Source=warehouse.db"));
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();

// Register repositories as scoped services.
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IStockService, StockService>();

// Configure JSON serialization options.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

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

// Map Endpoints
StockEndpoints.MapEndpoints(app);
WarehouseEndpoints.MapEndpoints(app);

app.Run();

