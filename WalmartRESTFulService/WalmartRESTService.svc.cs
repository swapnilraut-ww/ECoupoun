using ECoupoun.Data;
using ECoupoun.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Serialization;

namespace WalmartRESTFulService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class WalmartRESTService : IWalmartRESTService
    {
        DBProducts dbProducts = new DBProducts();

        /// <summary>
        /// Testing API for GET method 
        /// </summary>
        /// <returns>Response from GET API</returns>
        public string TestGETAPI()
        {
            var response = string.Empty;
            try
            {
                response = "This is Response from TestGETAPI";
            }
            catch (Exception ex)
            {
                response = "Some exception is occurred while calling GET service, exception is : " + ex.Message;
            }
            return response;
        }

        /// <summary>
        /// Testing API for GET method 
        /// </summary>
        /// <returns>Response from POST API</returns>
        public string TestPOSTAPI()
        {
            var response = string.Empty;
            try
            {
                response = "This is Response from TestPOSTAPI";
            }
            catch (Exception ex)
            {
                response = "Some exception is occurred while calling POST service, exception is : " + ex.Message;
            }
            return response;
        }

        /// <summary>
        /// Insert Product Details in DB
        /// </summary>
        /// <returns></returns>
        public string InsertData()
        {
            string URL = "http://api.walmartlabs.com/v1/paginated/items?format=json&apiKey=22rgbvm2cpzk22n4t8phrem9";
            string responseText = string.Empty;
            bool success = false;

            int count = 0;

            try
            {
                HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;

                // Get response  
                //using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                //{
                //    // Get the response stream  
                //    StreamReader reader = new StreamReader(response.GetResponseStream());

                //    var serializer = new JavaScriptSerializer();
                //    var jsonObject = serializer.Deserialize<ProductsJSON>(reader.ReadToEnd());

                //    foreach (var item in jsonObject.Items)
                //    {
                //        Products product = new Products();
                //        product.Sku = item.ItemId;
                //        product.Name = item.Name;
                //        product.ModelNumber = item.ModelNumber;
                //        product.ThumbnailImage = item.ThumbnailImage;
                //        product.RegularPrice = item.msrp;
                //        product.SalePrice = item.SalePrice;

                //        success = dbProducts.InsertProduct(product);
                //        if (success)
                //            count++;
                //    }
                //}

                responseText = string.Format("Inserted {0} Records.", count);
            }
            catch (Exception ex)
            {
                responseText = "Some exception is occurred while Inserting Data, exception is : " + ex.Message;
            }
            return responseText;
        }
    }
}
