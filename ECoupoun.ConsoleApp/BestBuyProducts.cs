using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECoupoun.ConsoleApp
{
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
        public List<Items> Items { get; set; }
    }

    public class Products
    {
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string ModelNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Url { get; set; }
        public string MobileUrl { get; set; }
        public string ShortDescription { get; set; }
        public string Color { get; set; }
        public decimal RegularPrice { get; set; }
        public decimal SalePrice { get; set; }
        public string ScreenSizeIn { get; set; }
    }

    public class Items
    {
        public string Name { get; set; }
        public string ThumbnailImage { get; set; }
        public string ModelNumber { get; set; }
        public double msrp { get; set; }
        public double SalePrice { get; set; }
    }
}
