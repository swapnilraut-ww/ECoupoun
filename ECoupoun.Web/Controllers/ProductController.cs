using ECoupoun.Data;
using ECoupoun.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECoupoun.Web.Controllers
{
    public class ProductController : BaseController
    {
        //
        // GET: /Product/
        //public ActionResult Index(int parentid, int catid)
        //{
        //    var productList = (from p in db.ProductMasters
        //                       join pl in db.ProductLinks on p.ProductId equals pl.ProductId
        //                       join pp in db.ProductPricings on p.ProductId equals pp.ProductId
        //                       join pr in db.Providers on pp.ProviderId equals pr.ProviderId
        //                       where p.CategoryId == catid
        //                       select new ProductModel
        //                        {
        //                            ProviderId = pr.ProviderId,
        //                            ProductName = p.Name,
        //                            Sku = pp.SKU,
        //                            ProviderName = pr.Name,
        //                            ProductUrl = pl.SoruceUrl,
        //                            ImageUrl = p.Image,
        //                            SalePrice = pp.SalePrice
        //                        }).ToList();
        //    return View(productList);
        //}

        public ActionResult Index(string parentCategory, string categoryName, string q)
        {
            List<ProductModel> productList = new List<ProductModel>();
            if (!string.IsNullOrWhiteSpace(q))
            {
                productList = (from p in db.ProductMasters
                               join pl in db.ProductLinks on p.ProductId equals pl.ProductId
                               join pp in db.ProductPricings on p.ProductId equals pp.ProductId
                               join pr in db.Providers on pp.ProviderId equals pr.ProviderId
                               join m in db.Manufacturers on p.ManufacturerId equals m.ManufacturerId
                               where p.Name.Contains(q)
                               select new ProductModel
                               {
                                   ProviderId = pr.ProviderId,
                                   ProductName = p.Name,
                                   Manufacturer = m.Name,
                                   Sku = pp.SKU,
                                   ProviderName = pr.Name,
                                   ProductUrl = pl.SoruceUrl,
                                   ImageUrl = p.Image,
                                   SalePrice = pp.SalePrice
                               }).ToList();

                //ViewBag.SubCategories = new List<Category>();
            }

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                Category category = db.Categories.Where(x => x.MappingName == categoryName && x.IsActive == true).SingleOrDefault();
                ViewBag.BreadCrumb = "<p><a href='/'>Home</a> >> <a href='javascript:void(0)'>" + category.Name + "</a></p>";
                List<Category> subCategoryList = db.Categories.Where(x => x.CategoryParentId == category.CategoryId && x.IsActive == true).ToList();
                return View("SubCategories", subCategoryList);
            }
            else
            {
                if (parentCategory == "search")
                {
                    ViewBag.BreadCrumb = "<p><a href='/'>Home</a> >> <a href='javascript:void(0)'>" + q + "</a></p>";
                }
                else if (parentCategory.Split('_').Length > 2)
                {
                    Category category = db.Categories.ToList().Where(x => x.MappingName == parentCategory.Split('_')[2] && x.IsActive == true).SingleOrDefault();
                    Category parentCategoryName = db.Categories.Where(x => x.CategoryId == category.CategoryParentId && x.IsActive == true).SingleOrDefault();
                    ViewBag.BreadCrumb = "<p><a href='/'>Home</a> >> <a href='/buy_" + parentCategoryName.MappingName + "'>" + parentCategoryName.Name + "</a>  >> <a href='javascript:void(0)'>" + category.Name + "</a></p>";
                    //ViewBag.SubCategories = db.Categories.Where(x => x.CategoryParentId == category.CategoryId).ToList();
                    productList = (from p in db.ProductMasters
                                   join pl in db.ProductLinks on p.ProductId equals pl.ProductId
                                   join pp in db.ProductPricings on p.ProductId equals pp.ProductId
                                   join pr in db.Providers on pp.ProviderId equals pr.ProviderId
                                   join m in db.Manufacturers on p.ManufacturerId equals m.ManufacturerId
                                   where p.SubCategoryId == category.CategoryId
                                   select new ProductModel
                                   {
                                       ProviderId = pr.ProviderId,
                                       ProductName = p.Name,
                                       Manufacturer = m.Name,
                                       Sku = pp.SKU,
                                       ProviderName = pr.Name,
                                       ProductUrl = pl.SoruceUrl,
                                       ImageUrl = p.Image,
                                       SalePrice = pp.SalePrice
                                   }).ToList();
                }
                else
                {
                    Category category = db.Categories.ToList().Where(x => x.MappingName == parentCategory.Split('_')[1] && x.IsActive == true).SingleOrDefault();
                    ViewBag.BreadCrumb = "<p><a href='/'>Home</a> >> <a href='javascript:void(0)'>" + category.Name + "</a></p>";
                    //ViewBag.SubCategories = db.Categories.Where(x => x.CategoryParentId == category.CategoryId).ToList();
                    productList = (from p in db.ProductMasters
                                   join pl in db.ProductLinks on p.ProductId equals pl.ProductId
                                   join pp in db.ProductPricings on p.ProductId equals pp.ProductId
                                   join pr in db.Providers on pp.ProviderId equals pr.ProviderId
                                   join m in db.Manufacturers on p.ManufacturerId equals m.ManufacturerId
                                   where p.CategoryId == category.CategoryId
                                   select new ProductModel
                                    {
                                        ProviderId = pr.ProviderId,
                                        ProductName = p.Name,
                                        ManufacturerId = m.ManufacturerId,
                                        Manufacturer = m.Name,
                                        Sku = pp.SKU,
                                        ProviderName = pr.Name,
                                        ProductUrl = pl.SoruceUrl,
                                        ImageUrl = p.Image,
                                        SalePrice = pp.SalePrice,

                                    }).ToList();
                }

            }

            ViewBag.ManufacturerList = productList.GroupBy(x => x.Manufacturer).Select(x => new SelectListItem() { Text = x.First().Manufacturer, Value = x.First().ManufacturerId.ToString() }).Distinct();
            ViewBag.ProviderList = productList.GroupBy(x => x.ProviderName).Select(x => new SelectListItem() { Text = x.First().ProviderName, Value = x.First().ProviderId.ToString() });
            Session["ProductList"] = productList;
            return View(productList);
        }

        [HttpPost]
        public ActionResult SaveProductViewDetails(int providerId, int sku, string productUrl)
        {
            try
            {
                string message = string.Empty;
                ProductViewDetail productViewDetail = db.ProductViewDetails.Where(x => x.ProviderId == providerId && x.SKU == sku && x.IsActive == true).SingleOrDefault();
                if (productViewDetail == null)
                {
                    productViewDetail = new ProductViewDetail();
                    productViewDetail.ProviderId = providerId;
                    productViewDetail.SKU = sku;
                    productViewDetail.TotalVisitors = 1;
                    productViewDetail.CreatedOn = System.DateTime.Now;
                    productViewDetail.IsActive = true;
                    db.ProductViewDetails.Add(productViewDetail);
                    message = "Product View Details added successfully";
                }
                else
                {
                    productViewDetail.TotalVisitors++;
                    message = "Product View Details updated successfully";
                }

                db.SaveChanges();
                return Json(new { Success = true, Message = message, ProductUrl = productUrl });
            }
            catch (Exception ex)
            {
                return Json(new { Success = true, Message = ex.Message, ProductUrl = productUrl });
            }
        }

        [HttpPost]
        public ActionResult GetProductList(string sortBy, string manufacturer, string provider)
        {
            List<ProductModel> productList = (List<ProductModel>)Session["ProductList"];

            List<string> manufacturerList = null;
            List<string> providerList = null;
            if (!string.IsNullOrWhiteSpace(manufacturer))
            {
                manufacturerList = manufacturer.Split(',').ToList();
                productList = productList.Where(p => manufacturerList.Contains(p.ManufacturerId.ToString())).ToList();
            }

            if (!string.IsNullOrWhiteSpace(provider))
            {
                providerList = provider.Split(',').ToList();
                productList = productList.Where(p => providerList.Contains(p.ProviderId.ToString())).ToList();
            }

            if (!string.IsNullOrWhiteSpace(sortBy) && sortBy == "low")
                productList = productList.OrderBy(x => x.SalePrice).ToList();
            else if (!string.IsNullOrWhiteSpace(sortBy) && sortBy == "high")
                productList = productList.OrderByDescending(x => x.SalePrice).ToList();

            return PartialView("_ProductPartial", productList);
        }
    }
}