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
    }

    public class Products
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string ModelNumber { get; set; }
        public double RegularPrice { get; set; }
        public double SalePrice { get; set; }
    }
}
