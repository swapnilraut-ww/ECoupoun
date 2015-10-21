using ECoupoun.Common;
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
        public ActionResult Index(string parentCategory, string categoryName, string q)
        {
            StaticPage staticPage = null;

            switch (parentCategory) 
            {
                case "About":
                    staticPage = db.StaticPages.Where(x => x.PageName == "About").SingleOrDefault();
                    if (staticPage != null)
                        ViewBag.PageContent = staticPage.PageHtml;
                    return View("~/Views/Home/About.cshtml");
                case "Contact":
                    staticPage = db.StaticPages.Where(x => x.PageName == "ContactUs").SingleOrDefault();
                    if (staticPage != null)
                        ViewBag.PageContent = staticPage.PageHtml;
                    return View("~/Views/Home/Contact.cshtml");
                case "Help":
                    staticPage = db.StaticPages.Where(x => x.PageName == "Help").SingleOrDefault();
                    if (staticPage != null)
                        ViewBag.PageContent = staticPage.PageHtml;
                    return View("~/Views/Home/Help.cshtml");
            }
            int startIndex = 0;
            List<ProductModel> productList = (from p in db.ProductMasters
                                              join pl in db.ProductLinks on p.ProductId equals pl.ProductId
                                              join pp in db.ProductPricings on p.ProductId equals pp.ProductId
                                              join pr in db.Providers on pp.ProviderId equals pr.ProviderId
                                              join m in db.Manufacturers on p.ManufacturerId equals m.ManufacturerId
                                              select new ProductModel
                                             {
                                                 CategoryId = p.CategoryId,
                                                 ProviderId = pr.ProviderId,
                                                 SubCategoryId = p.SubCategoryId.HasValue ? p.SubCategoryId.Value : 0,
                                                 ProductName = p.Name,
                                                 Manufacturer = m.Name,
                                                 Sku = pp.SKU,
                                                 ProviderName = pr.Name,
                                                 ProductUrl = pl.SoruceUrl,
                                                 ImageUrl = p.Image,
                                                 SalePrice = pp.SalePrice,
                                                 Color = p.Color,
                                                 Size = p.Size.HasValue ? p.Size.Value : 0
                                             }).ToList();

            if (!string.IsNullOrWhiteSpace(q))
            {
                productList = productList.Where(x => x.ProductName.ToLower().Contains(q.ToLower())).ToList();
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
                    ViewBag.SubCategories = db.Categories.Where(x => x.CategoryParentId == category.Category1.CategoryId).ToList().Where(x => x.MappingName != parentCategory.Split('_')[2]).ToList();

                    productList = productList.Where(x => x.SubCategoryId == category.CategoryId).ToList();
                }
                else
                {
                    Category category = db.Categories.ToList().Where(x => x.MappingName == parentCategory.Split('_')[1] && x.IsActive == true).SingleOrDefault();
                    ViewBag.BreadCrumb = "<p><a href='/'>Home</a> >> <a href='javascript:void(0)'>" + category.Name + "</a></p>";
                    ViewBag.SubCategories = db.Categories.Where(x => x.CategoryParentId == category.CategoryId).ToList();

                    productList = productList.Where(x => x.CategoryId == category.CategoryId).ToList();
                }
            }

            ViewBag.ManufacturerList = productList.GroupBy(x => x.Manufacturer).Select(x => new SelectListItem() { Text = x.First().Manufacturer, Value = x.First().ManufacturerId.ToString() }).Distinct();
            ViewBag.ProviderList = productList.GroupBy(x => x.ProviderName).Select(x => new SelectListItem() { Text = x.First().ProviderName, Value = x.First().ProviderId.ToString() });
            ViewBag.ColorList = productList.Where(x => x.Color != null).OrderBy(x => x.Color).GroupBy(x => x.Color).Select(x => new SelectListItem() { Text = x.First().Color, Value = x.First().Color });
            ViewBag.SizeList = productList.Where(x => x.Size != 0).OrderBy(x => x.Size).GroupBy(x => x.Size).Select(x => new SelectListItem() { Text = x.First().Size.ToString(), Value = x.First().Size.ToString() });
            Session["ProductList"] = productList;
            return View(productList.Skip(startIndex).Take(ECoupounConstants.BlockSize).ToList());
        }

        [HttpPost]
        public ActionResult InfinateScroll(int blockNumber)
        {
            List<ProductModel> productList = (List<ProductModel>)Session["ProductList"];

            int startIndex = (blockNumber - 1) * ECoupounConstants.BlockSize;
            productList = productList.Skip(startIndex).Take(ECoupounConstants.BlockSize).ToList();

            JsonModel jsonModel = new JsonModel();
            jsonModel.NoMoreData = productList.Count < ECoupounConstants.BlockSize;
            jsonModel.HTMLString = this.RenderPartialViewToString("_ProductPartial", productList);

            return Json(jsonModel);
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
        public ActionResult GetProductList(string sortBy, string manufacturer, string provider, string color, string size, string price, int blockNumber = 1)
        {
            int startIndex = (blockNumber - 1) * ECoupounConstants.BlockSize;
            List<ProductModel> productList = (List<ProductModel>)Session["ProductList"];
            List<ProductModel> filteredProducts = new List<ProductModel>();

            List<string> manufacturerList = null;
            List<string> providerList = null;
            List<string> colorList = null;
            List<string> priceList = null;
            List<double> sizeList = null;
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

            if (!string.IsNullOrWhiteSpace(color))
            {
                colorList = color.Split(',').ToList();
                productList = productList.Where(p => colorList.Contains(p.Color)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(size))
            {
                sizeList = size.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(double.Parse).ToList();
                productList = productList.Where(p => sizeList.Contains(p.Size)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(price))
            {
                priceList = price.Split(',').ToList();
                foreach (string strPrice in priceList)
                {
                    string[] priceRange = strPrice.Split('-');
                    int fromRange = Convert.ToInt32(priceRange[0]);
                    int toRange = Convert.ToInt32(priceRange[1]);
                    List<ProductModel> productsInRange = productList.Where(p => p.SalePrice >= fromRange && p.SalePrice <= toRange).ToList();
                    filteredProducts.AddRange(productsInRange);
                }

                productList = filteredProducts.Skip(startIndex).Take(ECoupounConstants.BlockSize).ToList();
            }
            else
            {
                productList = productList.Skip(startIndex).Take(ECoupounConstants.BlockSize).ToList();
            }

            JsonModel jsonModel = new JsonModel();
            jsonModel.NoMoreData = productList.Count < ECoupounConstants.BlockSize;
            jsonModel.HTMLString = this.RenderPartialViewToString("_ProductPartial", productList);

            return Json(jsonModel);
        }
    }
}