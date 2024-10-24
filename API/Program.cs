using MyWarehouseProject.Application.Services;
using MyWarehouseProject.Domain.Repositories;
using MyWarehouseProject.Infrastructure.Repositories;
using MyWarehouseProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// �������� ������ ����������� �� appsettings.json
var connectionString = builder.Configuration.GetConnectionString("WarehouseDb");

// ����������� ��������� ���� ������ ����� ����� ���������� ��� SQLite
builder.Services.AddDbContext<WarehouseDbContext>(options =>
    options.UseSqlite(connectionString));

// ����� ����������� ������������
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();

// ����� ����������� ��������
builder.Services.AddScoped<WarehouseService>();
builder.Services.AddScoped<StockService>();

// ��������� ������������ JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

// ��������� AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly); 

// ��������� Swagger
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

// ����������� Endpoints
WarehouseEndpoints.MapEndpoints(app);
StockEndpoints.MapEndpoints(app);

app.Run();

