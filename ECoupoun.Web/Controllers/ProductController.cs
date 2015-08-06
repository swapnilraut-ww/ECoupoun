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
        public ActionResult Index()
        {
            var productList = (from p in db.ProductMasters
                               join pl in db.ProductLinks on p.ProductId equals pl.ProductId
                               join pp in db.ProductPricings on p.ProductId equals pp.ProductId
                               join pr in db.Providers on pp.ProviderId equals pr.ProviderId
                               select new ProductModel
                                {
                                    ProductName = p.Name,
                                    ProviderName = pr.Name,
                                    ProductUrl = pl.SoruceUrl,
                                    ImageUrl = p.Image,
                                    SalePrice = pp.SalePrice
                                }).ToList();
            return View(productList);
        }
    }
}