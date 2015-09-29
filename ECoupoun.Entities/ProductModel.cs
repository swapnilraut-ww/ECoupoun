using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECoupoun.Entities
{
    public class ProductModel
    {
        public int CategoryId { get; set; }
        public int ProviderId { get; set; }
        public int TotalVisitors { get; set; }
        public int SubCategoryId { get; set; }
        public int ManufacturerId { get; set; }
        public long Sku { get; set; }
        public string ProductName { get; set; }
        public string ProviderName { get; set; }
        public string ProductUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Manufacturer { get; set; }
        public string Color { get; set; }
        public decimal SalePrice { get; set; }
        public double Size { get; set; }

    }
}
