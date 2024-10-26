using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWarehouseProject.Domain.DTOS
{
    public class DistributeStockRequestDto
    {
        public Guid SourceWarehouseId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public double NearestWarehousePercentage { get; set; } = 50.0; // Default to 50%
    
    }


}
