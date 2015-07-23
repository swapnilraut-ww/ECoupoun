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
            var url = ECoupounConstants.BestBuyRESTServiceURL + ECoupounConstants.GetAllProducts;
            ExtendedWebClient client = new ExtendedWebClient(new Uri(url));
            client.Headers[ECoupounConstants.ContentTypeText] = ECoupounConstants.ContentTypeValue;

            MemoryStream stream = new MemoryStream();
            byte[] data = client.UploadData(string.Format("{0}", url), "POST", stream.ToArray());
            stream = new MemoryStream(data);

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Products>));
            serializer = new DataContractJsonSerializer(typeof(List<Products>));
            var productList = (List<Products>)serializer.ReadObject(stream);

            return View(productList);
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            var url = ECoupounConstants.BestBuyRESTServiceURL + ECoupounConstants.InsertData;
            ExtendedWebClient client = new ExtendedWebClient(new Uri(url));
            client.Headers[ECoupounConstants.ContentTypeText] = ECoupounConstants.ContentTypeValue;

            MemoryStream stream = new MemoryStream();
            byte[] data = client.UploadData(string.Format("{0}", url), "POST", stream.ToArray());
            stream = new MemoryStream(data);

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(string));
            serializer = new DataContractJsonSerializer(typeof(string));
            var responseText = (string)serializer.ReadObject(stream);

            ViewBag.ResponseText = responseText;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}