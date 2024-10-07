
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWarehouseProject.Application.DTOs
{
    public class AutoBalanceRequestDto
    {
        public Guid ProductId { get; set; }
        public Guid SourceWarehouseId { get; set; }
    }
}
