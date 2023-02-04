using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Business.Dtos
{
    public class WarehouseProductDto
    {
        public int WarehouseId { get; set; }    
        public int ProductId { get; set; }
        public double Quantity { get; set; }
        public WarehouseProductDto(int warehouseId, int productId, double quantity)
        {
            WarehouseId = warehouseId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
