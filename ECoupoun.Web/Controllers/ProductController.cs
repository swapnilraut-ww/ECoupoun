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

        public ActionResult Index(string parentCategory, string categoryName)
        {
            
            if (!string.IsNullOrWhiteSpace(categoryName))
            {                
                Category category = db.Categories.Where(x => x.MappingName == categoryName).SingleOrDefault();
                ViewBag.BreadCrumb = "<p><a href='/'>Home</a> >> <a href='javascript:void(0)'>" + category.Name + "</a></p>";
                List<Category> subCategoryList = db.Categories.Where(x => x.CategoryParentId == category.CategoryId).ToList();
                return View("SubCategories", subCategoryList);
            }
            else
            {
                Category category = db.Categories.ToList().Where(x => x.MappingName == parentCategory.Split('_')[1]).SingleOrDefault();
                ViewBag.BreadCrumb = "<p><a href='/'>Home</a> >> <a href='javascript:void(0)'>" + category.Name + "</a></p>";
                ViewBag.SubCategories = db.Categories.Where(x => x.CategoryParentId == category.CategoryId).ToList();
                var productList = (from p in db.ProductMasters
                                   join pl in db.ProductLinks on p.ProductId equals pl.ProductId
                                   join pp in db.ProductPricings on p.ProductId equals pp.ProductId
                                   join pr in db.Providers on pp.ProviderId equals pr.ProviderId
                                   where p.CategoryId == category.CategoryId
                                   select new ProductModel
                                    {
                                        ProviderId = pr.ProviderId,
                                        ProductName = p.Name,
                                        Sku = pp.SKU,
                                        ProviderName = pr.Name,
                                        ProductUrl = pl.SoruceUrl,
                                        ImageUrl = p.Image,
                                        SalePrice = pp.SalePrice
                                    }).ToList();
                return View(productList);
            }
        }

        [HttpPost]
        public ActionResult SaveProductViewDetails(int providerId, int sku)
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
                return Json(new { Success = true, Message = message });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }
    }
}