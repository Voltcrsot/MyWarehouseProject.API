﻿namespace MyWarehouseProject.Domain.Entities
{
    public class Stock
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid WarehouseId { get; set; }
        public int Quantity { get; set; }

        public Warehouse Warehouse { get; set; }
    }
}
