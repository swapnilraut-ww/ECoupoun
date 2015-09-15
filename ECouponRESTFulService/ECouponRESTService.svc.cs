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

namespace ECouponRESTFulService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class ECouponRESTService : IECouponRESTService
    {
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

        public List<ECouponProducts> GetProducts(string providerNames)
        {
            string[] providerList = providerNames.Split(',');
            List<Products> productsList = new List<Products>();
            List<ECouponProducts> eCouponProductsList = new List<ECouponProducts>();
            ECouponProducts eCouponProducts = null;
            foreach (var provider in providerList)
            {
                switch (provider.ToLower())
                {
                    case "bestbuy":
                        {
                            eCouponProducts = new ECouponProducts();
                            string bestBuyURL = "https://api.bestbuy.com/v1/products?apiKey=93j5ggwxgfraye8unmhvgzgv&sort=regularPrice.asc&show=regularPrice,salePrice,Image,manufacturer,modelNumber,name,sku,mobileUrl,url,categoryPath.id,categoryPath.name,shortDescription&pageSize=100&format=json";
                            var bestBuyProducts = MakeAPICall<BestBuyProducts>(bestBuyURL);
                            foreach (var product in bestBuyProducts.Products)
                            {
                                productsList.Add(product);
                            }

                            eCouponProducts.Provider = "BestBuy";
                            eCouponProducts.Products = productsList;
                            eCouponProductsList.Add(eCouponProducts);
                            break;
                        }
                    case "walmart":
                        {
                            eCouponProducts = new ECouponProducts();
                            string bestBuyURL = "http://api.walmartlabs.com/v1/paginated/items?apiKey=22rgbvm2cpzk22n4t8phrem9&format=json";
                            var walmartItemsList = MakeAPICall<WalmartItemsList>(bestBuyURL);
                            List<Products> walmartProducts = new List<Products>();
                            foreach (var item in walmartItemsList.Items)
                            {
                                Products product = new Products();
                                product.Sku = item.ItemId;
                                product.Name = item.Name;
                                product.ModelNumber = item.ModelNumber;
                                product.Image = item.MediumImage;
                                product.RegularPrice = item.msrp;
                                product.SalePrice = item.SalePrice;
                                product.Manufacturer = item.BrandName;
                                product.Url = item.ProductUrl;

                                walmartProducts.Add(product);
                            }

                            eCouponProducts.Provider = "Walmart";
                            eCouponProducts.Products = walmartProducts;
                            eCouponProductsList.Add(eCouponProducts);
                            break;
                        }
                }

            }

            return eCouponProductsList;
        }

        public static T MakeAPICall<T>(string url) where T : new()
        {
            dynamic productsJSON = new T();
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            // Get response  
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                StreamReader reader = new StreamReader(response.GetResponseStream());

                var serializer = new JavaScriptSerializer();
                productsJSON = serializer.Deserialize<T>(reader.ReadToEnd());
            }

            return (T)productsJSON;
        }
    }
}
