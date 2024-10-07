using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWarehouseProject.Application.DTOs
{
    public class BalanceStockRequestDto
    {
        public Guid SourceWarehouseId { get; set; }
        public Guid TargetWarehouseId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
