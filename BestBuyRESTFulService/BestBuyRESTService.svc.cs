using ECoupoun.Common;
using ECoupoun.Common.Helper;
using ECoupoun.Data;
using ECoupoun.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Xml;

namespace BestBuyRESTFulService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class BestBuyRESTService : IBestBuyRESTService
    {
        DBProducts dbProducts = new DBProducts();
        string responseText = string.Empty;
        int walmartCount = 0;
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
            responseText = string.Empty;
            foreach (ConnectionStringSettings ConnectionStrings in ConfigurationManager.ConnectionStrings)
            {
                walmartCount = 0;
                int bestbuyCount = 0;
                int amazonCount = 0;
                using (var db = new ECoupounEntities(ConnectionStrings.ConnectionString))
                {
                    List<APIDetail> apiDetailsList = db.APIDetails.Where(x => x.IsActive == true && x.Provider.Name == "Walmart").ToList();

                    ProductsJSON jsonObject = null;
                    List<Products> productsList = null;
                    try
                    {
                        db.DeleteProducts();

                        foreach (var apiDetail in apiDetailsList)
                        {
                            switch (apiDetail.Provider.Name)
                            {
                                case "BestBuy":
                                    jsonObject = MakeAPICall(apiDetail.ServiceUrl);
                                    if (jsonObject != null)
                                    {
                                        for (int i = 1; i <= jsonObject.totalPages; i++)
                                        {
                                            jsonObject = MakeAPICall(apiDetail.ServiceUrl + "&page=" + i);
                                            productsList = new List<Products>();
                                            if (jsonObject != null)
                                            {
                                                foreach (var product in jsonObject.Products)
                                                {
                                                    productsList.Add(product);
                                                }

                                                bestbuyCount += dbProducts.InsertProduct(apiDetail.CategoryId, apiDetail.ProviderId, productsList, db);
                                            }
                                        }
                                    }

                                    break;
                                case "Walmart":                                  
                                    MakeWalmartCallAndProcess(apiDetail.ServiceUrl, apiDetail.CategoryId, apiDetail.ProviderId, db);

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
                                            productsList = new List<Products>();

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

                                                productsList.Add(product);
                                            }

                                            amazonCount += dbProducts.InsertProduct(apiDetail.CategoryId, apiDetail.ProviderId, productsList, db);
                                        }
                                    }
                                    break;
                            }
                        }

                        responseText += string.Format("\nBesyBuy {0} Records.\n", bestbuyCount);
                        responseText += string.Format("Walmart {0} Records.\n", walmartCount);
                        responseText += string.Format("Amazon {0} Records.\n", amazonCount);
                        responseText += string.Format("Total {0} Records.\n", bestbuyCount + walmartCount + amazonCount);
                    }
                    catch (Exception ex)
                    {
                        responseText += string.Format("\nBesyBuy {0} Records.\n", bestbuyCount);
                        responseText += string.Format("Walmart {0} Records.\n", walmartCount);
                        responseText += string.Format("Amazon {0} Records.\n", amazonCount);
                        responseText += string.Format("Total {0} Records.\n", bestbuyCount + walmartCount + amazonCount);

                        responseText += "Some exception is occurred while Inserting Data, exception is : " + ex.Message;
                    }
                }
            }

            return responseText;
        }

        public List<Products> GetAllProducts()
        {
            List<Products> productList = dbProducts.GetAllProducts();
            return productList;
        }

        public void MakeWalmartCallAndProcess(string url, int categoryId, int providerId, ECoupounEntities db)
        {
            try
            {
                responseText += "\n" + url;

                ProductsJSON productsJSON = null;
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                List<Products> productsList = null;
                // Get response  
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    var serializer = new JavaScriptSerializer();
                    productsJSON = serializer.Deserialize<ProductsJSON>(reader.ReadToEnd());
                    productsList = new List<Products>();
                    if (productsJSON != null && productsJSON.Items != null)
                    {
                        foreach (var item in productsJSON.Items)
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

                            productsList.Add(product);
                        }

                        walmartCount += dbProducts.InsertProduct(categoryId, providerId, productsList, db);
                        productsList = new List<Products>();
                        MakeWalmartCallAndProcess("http://api.walmartlabs.com" + productsJSON.NextPage, categoryId, providerId, db);
                    }
                }
            }
            catch (Exception ex)
            {
            }
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
                responseText += string.Format("Some exception is occurred while Parsing API '{0}', exception is : {1}", url, ex.Message);
                return null;
            }
        }
    }
}
