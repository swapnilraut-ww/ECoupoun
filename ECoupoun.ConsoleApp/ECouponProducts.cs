using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECoupoun.ConsoleApp
{
    public class ECouponProducts
    {
        public string Provider { get; set; }
        public List<Products> Products { get; set; }
    }

    public class BestBuyProducts
    {
        public int From { get; set; }
        public int To { get; set; }
        public int Total { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public double QueryTime { get; set; }
        public double TotalTime { get; set; }
        public bool Partial { get; set; }
        public List<Products> Products { get; set; }
    }

    public class Products
    {
        public long Sku { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string ModelNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Url { get; set; }
        public string MobileUrl { get; set; }
        public string ShortDescription { get; set; }
        public decimal RegularPrice { get; set; }
        public decimal SalePrice { get; set; }
    }

    public class WalmartItemsList
    {
        public List<WalmartItems> Items { get; set; }
    }

    public class WalmartItems
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string MediumImage { get; set; }
        public string ModelNumber { get; set; }
        public string BrandName { get; set; }
        public string ProductUrl { get; set; }
        public decimal msrp { get; set; }
        public decimal SalePrice { get; set; }
    }
}
