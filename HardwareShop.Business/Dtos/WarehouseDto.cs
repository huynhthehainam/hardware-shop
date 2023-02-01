using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Business.Dtos
{
    public class WarehouseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public WarehouseDto(int id, string name, string? address)
        {
            Id = id;
            Name = name;
            Address = address;
        }
    }
}
