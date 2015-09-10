using ECoupoun.Common;
using ECoupoun.Common.Helper;
using ECoupoun.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.Mvc;
using ECoupoun.Data;

namespace ECoupoun.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            try
            {
                var productList = (from p in db.ProductMasters
                                   join pl in db.ProductLinks on p.ProductId equals pl.ProductId
                                   join pp in db.ProductPricings on p.ProductId equals pp.ProductId
                                   join pr in db.Providers on pp.ProviderId equals pr.ProviderId
                                   select new ProductModel
                                   {
                                       ProviderId = pr.ProviderId,
                                       ProductName = p.Name,
                                       Sku = pp.SKU,
                                       ProviderName = pr.Name,
                                       ProductUrl = pl.SoruceUrl,
                                       ImageUrl = p.Image,
                                       SalePrice = pp.SalePrice,

                                   }).ToList();
                return View(productList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult BestBuyInsertData(FormCollection form)
        {
            var url = ECoupounConstants.BestBuyRESTServiceURL + ECoupounConstants.InsertData;
            TempData["ResponseText"] = RestServiceCall(url);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult WalmartInsertData(FormCollection form)
        {
            var url = ECoupounConstants.WalmartRESTServiceURL + ECoupounConstants.InsertData;
            TempData["ResponseText"] = RestServiceCall(url);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EBayInsertData(FormCollection form)
        {
            var url = ECoupounConstants.EBayRESTServiceURL + ECoupounConstants.InsertData;
            TempData["ResponseText"] = RestServiceCall(url);

            return RedirectToAction("Index");
        }

        private string RestServiceCall(string url)
        {
            try
            {
                string responseText = string.Empty;
                ExtendedWebClient client = new ExtendedWebClient(new Uri(url));
                client.Headers[ECoupounConstants.ContentTypeText] = ECoupounConstants.ContentTypeValue;

                MemoryStream stream = new MemoryStream();
                byte[] data = client.UploadData(string.Format("{0}", url), "POST", stream.ToArray());
                stream = new MemoryStream(data);

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(string));
                serializer = new DataContractJsonSerializer(typeof(string));
                responseText = (string)serializer.ReadObject(stream);
                return responseText;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public ActionResult Menu()
        {
            List<Category> menues = db.Categories.Where(x => x.IsActive == true).ToList();
            return PartialView("_MenuPartial", menues);
        }

        public ActionResult GetSearchData(string term)
        {
            var productList = db.ProductMasters.ToList().Where(x => x.Name.ToLower().Contains(term.ToLower())).Select(x => new { id = x.ProductId, label = x.Name, value = x.Name }).ToList();
            return Json(productList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HomeCategoryPartial()
        {
            Random rnd = new Random();
            var parentCategories = db.Categories.Where(x => x.CategoryParentId == null && x.IsActive == true).ToList().Select(x => x.CategoryId.ToString()).ToList();

            var firstLevelCategories = db.Categories.Where(x => x.IsActive == true).ToList().Where(x => parentCategories.Contains(x.CategoryParentId.ToString())).OrderBy(x => rnd.Next()).Take(4).ToList();

            return PartialView("_HomeCategoryPartial", firstLevelCategories);
        }
    }
}