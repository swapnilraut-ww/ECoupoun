using ECoupoun.Common;
using ECoupoun.Common.Helper;
using ECoupoun.Data;
using ECoupoun.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Xml;

namespace BestBuyRESTFulService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class BestBuyRESTService : DBData, IBestBuyRESTService
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
            List<APIDetail> apiDetailsList = db.APIDetails.Where(x => x.IsActive == true).ToList();
            string responseText = string.Empty;
            bool success = false;

            int count = 0;
            int failCount = 0;

            try
            {
                db.DeleteProducts();

                foreach (var apiDetail in apiDetailsList)
                {
                    responseText += string.Format("ServiceUrl = {0} \n", apiDetail.ServiceUrl);
                    var jsonObject = MakeAPICall(apiDetail.ServiceUrl);
                    switch (apiDetail.Provider.Name)
                    {
                        case "BestBuy":
                            for (int i = 1; i <= jsonObject.totalPages; i++)
                            {
                                jsonObject = MakeAPICall(apiDetail.ServiceUrl + "&page=" + i);

                                foreach (var product in jsonObject.Products)
                                {
                                    success = dbProducts.InsertProduct(apiDetail.CategoryId, apiDetail.ProviderId, product);
                                    if (success)
                                        count++;
                                    else
                                        failCount++;
                                }
                            }
                            break;

                        case "Walmart":
                            foreach (var item in jsonObject.Items)
                            {
                                Products product = new Products();
                                product.Sku = item.ItemId.ToString();
                                product.Name = item.Name;
                                product.ModelNumber = item.ModelNumber;
                                product.Image = item.MediumImage;
                                product.RegularPrice = item.msrp;
                                product.SalePrice = item.SalePrice;
                                product.Manufacturer = item.BrandName;
                                product.Url = item.ProductUrl;
                                success = dbProducts.InsertProduct(apiDetail.CategoryId, apiDetail.ProviderId, product);
                                if (success)
                                    count++;
                            }
                            break;

                        case "Amazon":
                            SignedRequestHelper helper = new SignedRequestHelper(ECoupounConstants.AccessKeyId, ECoupounConstants.SecretKey, ECoupounConstants.DESTINATION);
                            for (int p = 1; p <= 10; p++)
                            {
                                string requestUrl = helper.Sign(string.Format(apiDetail.ServiceUrl, p));

                                List<Products> amazonProductList = new List<Products>();
                                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;

                                // Get response  
                                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                                {
                                    // Get the response stream  
                                    StreamReader reader = new StreamReader(response.GetResponseStream());

                                    XmlDocument xmlDoc = new XmlDocument();
                                    xmlDoc.Load(response.GetResponseStream());
                                    XmlNodeList xmlnodelstTrack = xmlDoc.GetElementsByTagName("Item");
                                    XmlNodeList xmlnodelstTrack1 = xmlDoc.GetElementsByTagName("TotalPages");

                                    Products product = new Products();
                                    foreach (XmlNode NodeObj in xmlnodelstTrack)
                                    {
                                        product = new Products();
                                        for (int i = 0; i < NodeObj.ChildNodes.Count; i++)
                                        {
                                            if (NodeObj.ChildNodes[i].HasChildNodes)
                                            {
                                                for (int j = 0; j < NodeObj.ChildNodes[i].ChildNodes.Count; j++)
                                                {
                                                    string key = NodeObj.ChildNodes[i].ChildNodes[j].Name == "#text" ? NodeObj.ChildNodes[i].ChildNodes[j].ParentNode.Name : NodeObj.ChildNodes[i].ChildNodes[j].Name;
                                                    switch (key)
                                                    {
                                                        case "ASIN":
                                                            product.Sku = NodeObj.ChildNodes[i].ChildNodes[j].InnerText;
                                                            break;
                                                        case "DetailPageURL":
                                                            product.Url = NodeObj.ChildNodes[i].ChildNodes[j].InnerText;
                                                            break;
                                                        case "Manufacturer":
                                                            product.Manufacturer = NodeObj.ChildNodes[i].ChildNodes[j].InnerText;
                                                            break;
                                                        case "Model":
                                                            product.ModelNumber = NodeObj.ChildNodes[i].ChildNodes[j].InnerText;
                                                            break;
                                                        case "Color":
                                                            product.Color = NodeObj.ChildNodes[i].ChildNodes[j].InnerText;
                                                            break;
                                                        case "Title":
                                                            product.Name = NodeObj.ChildNodes[i].ChildNodes[j].InnerText;
                                                            break;
                                                        case "Size":
                                                            product.ScreenSizeIn = NodeObj.ChildNodes[i].ChildNodes[j].InnerText.Split('-')[0];
                                                            break;
                                                        case "ListPrice":
                                                            product.SalePrice = Convert.ToDecimal(NodeObj.ChildNodes[i].ChildNodes[j].InnerText.Split('$')[1]);
                                                            product.RegularPrice = Convert.ToDecimal(NodeObj.ChildNodes[i].ChildNodes[j].InnerText.Split('$')[1]);
                                                            break;
                                                        case "URL":
                                                            product.Image = NodeObj.ChildNodes[i].ChildNodes[j].InnerText;
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                        success = dbProducts.InsertProduct(apiDetail.CategoryId, apiDetail.ProviderId, product);
                                        if (success)
                                            count++;
                                        amazonProductList.Add(product);
                                    }
                                }
                            }
                            break;
                    }
                }

                responseText += string.Format("Inserted {0} Records.\n", count);
                responseText += string.Format("Duplicate {0} Records.\n", failCount);
            }
            catch (Exception ex)
            {
                responseText += string.Format("Inserted {0} Records. \n", count);
                responseText += string.Format("Duplicate {0} Records. \n", failCount);
                responseText += "Some exception is occurred while Inserting Data, exception is : " + ex.Message;
            }
            return responseText;
        }

        public List<Products> GetAllProducts()
        {
            List<Products> productList = dbProducts.GetAllProducts();
            return productList;
        }

        public ProductsJSON MakeAPICall(string url)
        {
            try
            {
                ProductsJSON productsJSON = null;
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

                // Get response  
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    var serializer = new JavaScriptSerializer();
                    productsJSON = serializer.Deserialize<ProductsJSON>(reader.ReadToEnd());
                }

                return productsJSON;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
