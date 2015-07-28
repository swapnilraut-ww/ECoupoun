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

namespace ECoupoun.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<Products> productList = new List<Products>();
            try
            {
                var url = ECoupounConstants.BestBuyRESTServiceURL + ECoupounConstants.GetAllProducts;
                ExtendedWebClient client = new ExtendedWebClient(new Uri(url));
                client.Headers[ECoupounConstants.ContentTypeText] = ECoupounConstants.ContentTypeValue;

                MemoryStream stream = new MemoryStream();
                byte[] data = client.UploadData(string.Format("{0}", url), "POST", stream.ToArray());
                stream = new MemoryStream(data);

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Products>));
                serializer = new DataContractJsonSerializer(typeof(List<Products>));
                productList = (List<Products>)serializer.ReadObject(stream);

                ViewBag.ResponseText = TempData["ResponseText"];
                return View(productList);
            }
            catch (Exception ex)
            {
                ViewBag.ResponseText = ex.Message;
                return View(productList);
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
    }
}