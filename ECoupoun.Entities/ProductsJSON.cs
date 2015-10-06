using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECoupoun.Entities
{
    public class ProductsJSON
    {
        public int from { get; set; }
        public int to { get; set; }
        public int total { get; set; }
        public int currentPage { get; set; }
        public int totalPages { get; set; }
        public double queryTime { get; set; }
        public double totalTime { get; set; }
        public bool partial { get; set; }
        public List<Products> Products { get; set; }
        public List<WalmartItems> Items { get; set; }
        public List<EbaySearchResult> SearchResult { get; set; }
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
        public List<CategoryPath> CategoryPath { get; set; }
    }

    public class CategoryPath
    {
        public string Id { get; set; }
        public string Name { get; set; }
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

    public class EbaySearchResult
    {
        public EbayItemArray ItemArray { get; set; }
    }

    public class EbayItemArray
    {
        public List<EbayItem> Item { get; set; }
    }

    public class EbayItem
    {
        public string Title { get; set; }
        public string GalleryURL { get; set; }
        public long ItemID { get; set; }
        public ConvertedCurrentPrice ConvertedCurrentPrice { get; set; }
    }

    public class ConvertedCurrentPrice
    {
        public double Value { get; set; }
    }
}
