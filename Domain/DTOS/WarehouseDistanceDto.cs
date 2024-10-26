using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWarehouseProject.Domain.DTOS
{
    public class WarehouseDistanceDto
    {
        public Guid WarehouseId { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Distance { get; set; } // Вычисленное расстояние до исходного склада
        public int Capacity { get; set; } // Емкость склада
    }

}
