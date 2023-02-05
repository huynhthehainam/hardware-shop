using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Business.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double? Mass { get; set; }
        public double? PricePerMass { get; set; }
        public double? PercentForFamiliarCustomer { get; set; }
        public double? PercentForCustomer { get; set; }
        public double? PriceForFamiliarCustomer { get; set; }
        public double PriceForCustomer { get; set; }
        public int UnitId { get; set; }
        public string? UnitName { get; set; }
        public int[] ProductCategoryIds { get; set; } = new int[0];
        public string?[] ProductCategoryNames { get; set; } = new string?[0];
    }
}
