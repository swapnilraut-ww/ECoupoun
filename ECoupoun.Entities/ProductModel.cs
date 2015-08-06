using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECoupoun.Entities
{
    public class ProductModel
    {
        public string ProductName { get; set; }
        public string ProviderName { get; set; }
        public string ProductUrl { get; set; }
        public string ImageUrl { get; set; }
        public decimal SalePrice { get; set; }

    }
}
