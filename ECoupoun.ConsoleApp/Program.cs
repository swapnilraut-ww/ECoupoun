using ECoupoun.ConsoleApp.ServiceReference1;
using MarketplaceWebServiceProducts;
using MarketplaceWebServiceProducts.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ECoupoun.ConsoleApp
{
    class Program
    {
        private const string bestBuyURL = "http://api.remix.bestbuy.com/v1/products%28longDescription=iPhone*%29?show=sku,name,image,modelNumber,regularPrice,salePrice&apiKey=93j5ggwxgfraye8unmhvgzgv&format=json";
        private const string walmartURL = "http://api.walmartlabs.com/v1/paginated/items?category=3944_3951_1230331&apiKey=22rgbvm2cpzk22n4t8phrem9&format=json";
        private const string etsyURL = "https://openapi.etsy.com/v2/taxonomy/seller/get?api_key=5mabizpl6kyplz83tpbxuzi3";

        // Developer AWS access key
        static string accessKeyId = "AKIAJDMTVVR36GOHDB7A";

        // Developer AWS secret key
        static string secretKey = "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";

        private const string DESTINATION = "ecs.amazonaws.com";

        // The client application name
        static string appName = "ECoupon";

        // The client application version
        static string appVersion = "1.0";

        // The endpoint for region service and version (see developer guide)
        // ex: https://mws.amazonservices.com
        static string serviceURL = "https://mws.amazonservices.com";

        static string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };

        static string EscapeUriDataStringRfc3986(string value)
        {
            // Start with RFC 2396 escaping by calling the .NET method to do the work.
            // This MAY sometimes exhibit RFC 3986 behavior (according to the documentation).
            // If it does, the escaping we do that follows it will be a no-op since the
            // characters we search for to replace can't possibly exist in the string.
            StringBuilder escaped = new StringBuilder(Uri.EscapeDataString(value));

            // Upgrade the escaping to RFC 3986, if necessary.
            for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
            {
                escaped.Replace(UriRfc3986CharsToEscape[i], Uri.HexEscape(UriRfc3986CharsToEscape[i][0]));
            }

            // Return the fully-RFC3986-escaped string.
            return escaped.ToString();
        }

        public static void AmazonWCF()
        {
            // create a WCF Amazon ECS client
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.MaxReceivedMessageSize = int.MaxValue;
            AWSECommerceServicePortTypeClient client = new AWSECommerceServicePortTypeClient(
                binding,
                new EndpointAddress("https://webservices.amazon.com/onca/soap?Service=AWSECommerceService"));

            // add authentication to the ECS client
            client.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior(accessKeyId, secretKey));

            // prepare an ItemSearch request
            ItemSearchRequest request = new ItemSearchRequest();
            request.SearchIndex = "Electronics";
            request.Title = "Monitor";
            request.ResponseGroup = new string[] { "Small" };

            ItemSearch itemSearch = new ItemSearch();
            itemSearch.Request = new ItemSearchRequest[] { request };
            itemSearch.AWSAccessKeyId = accessKeyId;
            //itemSearch.AssociateTag = tab
            // issue the ItemSearch request
            ItemSearchResponse response = client.ItemSearch(itemSearch);

            // write out the results
            foreach (var item in response.Items[0].Item)
            {
                Console.WriteLine(item.ItemAttributes.Title);
            }
        }

        public static BestBuyProducts MakeWalmartAPICall(string url)
        {
            try
            {
                BestBuyProducts productsJSON = null;
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

                // Get response  
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    var serializer = new JavaScriptSerializer();
                    productsJSON = serializer.Deserialize<BestBuyProducts>(reader.ReadToEnd());
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
                            //product.Manufacturer = item.Manufacturer;
                            product.Url = item.ProductUrl;

                            //productsList.Add(product);
                        }

                        MakeWalmartAPICall(productsJSON.NextPage);
                    }
                }

                return productsJSON;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        static void Main(string[] args)
        {
            try
            {
                BestBuyProducts jsonObject = MakeWalmartAPICall(walmartURL);
                if (jsonObject != null)
                {

                }
            }
            catch (Exception ex)
            {

            }

            int total = 1479;
            int numItems = 25;
            int start = 1;
            int span = total / numItems;
            int span1 = total % numItems;
            if (span1 > 0)
            {
                span++;
            }
            //for (int i = 1; i <= span; i++)
            //{
            //    if (i > 1)
            //        start += numItems;
            //    Console.WriteLine("Start = " + start);
            //}

            HttpWebRequest request = WebRequest.Create(walmartURL) as HttpWebRequest;

            // Get response  
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                StreamReader reader = new StreamReader(response.GetResponseStream());

                var serializer = new JavaScriptSerializer();
                var jsonObject = serializer.Deserialize<BestBuyProducts>(reader.ReadToEnd());
                span = jsonObject.TotalResults / jsonObject.numItems;
                span1 = jsonObject.TotalResults % jsonObject.numItems;
                for (int i = 1; i <= span; i++)
                {
                    if (i > 1)
                        start += jsonObject.numItems;
                    HttpWebRequest request2 = WebRequest.Create(walmartURL + "&start=" + start) as HttpWebRequest;
                    using (HttpWebResponse response1 = request2.GetResponse() as HttpWebResponse)
                    {
                        // Get the response stream  
                        StreamReader reader1 = new StreamReader(response1.GetResponseStream());

                        var serializer1 = new JavaScriptSerializer();
                        var jsonObject1 = serializer.Deserialize<BestBuyProducts>(reader1.ReadToEnd());
                    }
                }
                // Console application output  
                Console.WriteLine(reader.ReadToEnd());
            }
            AmazonWCF();
            List<Products> amazonProductList = new List<Products>();
            SignedRequestHelper helper = new SignedRequestHelper(accessKeyId, secretKey, DESTINATION);
            //for (int p = 1; p <= 10; p++)
            //{
            String requestString = "Service=AWSECommerceService"
                + "&Version=2009-03-31"
                + "&Operation=ItemSearch"
                + "&SearchIndex=Electronics"
                + "&ResponseGroup=Images,ItemAttributes,Small,VariationSummary"
                + "&BrowseNode=1292115011"
                + "&Keywords=4k"
                // + "&ItemPage=" + p                    
                + "&AssociateTag=fasforles07-20";

            string requestUrl = helper.Sign(requestString);
            HttpWebRequest request1 = WebRequest.Create(requestUrl) as HttpWebRequest;

            // Get response  
            using (HttpWebResponse response = request1.GetResponse() as HttpWebResponse)
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
                                Console.WriteLine("--------------------------------------------");
                                Console.WriteLine(key);
                                Console.WriteLine(NodeObj.ChildNodes[i].ChildNodes[j].InnerText);
                                Console.WriteLine("--------------------------------------------");
                            }
                        }
                        else
                        {
                            Console.WriteLine("--------------------------------------------");
                            Console.WriteLine(NodeObj.ChildNodes[i].Name);
                            Console.WriteLine(NodeObj.ChildNodes[i].InnerText);
                            Console.WriteLine("--------------------------------------------");
                        }
                    }
                    amazonProductList.Add(product);
                }
            }
            //}


            Console.ReadLine();

            // Create a configuration object
            //MarketplaceWebServiceProductsConfig config = new MarketplaceWebServiceProductsConfig();
            //config.ServiceURL = serviceURL;
            //// Set other client connection configurations here if needed
            //// Create the client itself
            //var client = new MarketplaceWebServiceProductsClient(appName, appVersion, accessKey, secretKey, config);

            //MarketplaceWebServiceProductsSample sample = new MarketplaceWebServiceProductsSample(client);

            // Uncomment the operation you'd like to test here
            // TODO: Modify the request created in the Invoke method to be valid

            try
            {
                //IMWSResponse response = null;
                // response = sample.InvokeGetCompetitivePricingForASIN();
                // response = sample.InvokeGetCompetitivePricingForSKU();
                // response = sample.InvokeGetLowestOfferListingsForASIN();
                // response = sample.InvokeGetLowestOfferListingsForSKU();
                // response = sample.InvokeGetLowestPricedOffersForASIN();
                // response = sample.InvokeGetLowestPricedOffersForSKU();
                // response = sample.InvokeGetMatchingProduct();
                // response = sample.InvokeGetMatchingProductForId();
                // response = sample.InvokeGetMyPriceForASIN();
                // response = sample.InvokeGetMyPriceForSKU();
                // response = sample.InvokeGetProductCategoriesForASIN();
                // response = sample.InvokeGetProductCategoriesForSKU();
                // response = sample.InvokeGetServiceStatus();
                //response = sample.InvokeListMatchingProducts();
                //Console.WriteLine("Response:");
                //ResponseHeaderMetadata rhmd = response.ResponseHeaderMetadata;
                //// We recommend logging the request id and timestamp of every call.
                //Console.WriteLine("RequestId: " + rhmd.RequestId);
                //Console.WriteLine("Timestamp: " + rhmd.Timestamp);
                //string responseXml = response.ToXML();
                //Console.WriteLine(responseXml);

            }
            catch (Exception ex)
            {

            }
        }

        private readonly MarketplaceWebServiceProducts.MarketplaceWebServiceProducts client;

        public Program(MarketplaceWebServiceProducts.MarketplaceWebServiceProducts client)
        {
            this.client = client;
        }

        public GetCompetitivePricingForASINResponse InvokeGetCompetitivePricingForASIN()
        {
            // Create a request.
            GetCompetitivePricingForASINRequest request = new GetCompetitivePricingForASINRequest();
            string sellerId = "example";
            request.SellerId = sellerId;
            string mwsAuthToken = "example";
            request.MWSAuthToken = mwsAuthToken;
            string marketplaceId = "example";
            request.MarketplaceId = marketplaceId;
            ASINListType asinList = new ASINListType();
            request.ASINList = asinList;
            return this.client.GetCompetitivePricingForASIN(request);
        }

        public GetCompetitivePricingForSKUResponse InvokeGetCompetitivePricingForSKU()
        {
            // Create a request.
            GetCompetitivePricingForSKURequest request = new GetCompetitivePricingForSKURequest();
            string sellerId = "example";
            request.SellerId = sellerId;
            string mwsAuthToken = "example";
            request.MWSAuthToken = mwsAuthToken;
            string marketplaceId = "example";
            request.MarketplaceId = marketplaceId;
            SellerSKUListType sellerSKUList = new SellerSKUListType();
            request.SellerSKUList = sellerSKUList;
            return this.client.GetCompetitivePricingForSKU(request);
        }

        public GetLowestOfferListingsForASINResponse InvokeGetLowestOfferListingsForASIN()
        {
            // Create a request.
            GetLowestOfferListingsForASINRequest request = new GetLowestOfferListingsForASINRequest();
            string sellerId = "example";
            request.SellerId = sellerId;
            string mwsAuthToken = "example";
            request.MWSAuthToken = mwsAuthToken;
            string marketplaceId = "example";
            request.MarketplaceId = marketplaceId;
            ASINListType asinList = new ASINListType();
            request.ASINList = asinList;
            string itemCondition = "example";
            request.ItemCondition = itemCondition;
            bool excludeMe = true;
            request.ExcludeMe = excludeMe;
            return this.client.GetLowestOfferListingsForASIN(request);
        }

        public GetLowestOfferListingsForSKUResponse InvokeGetLowestOfferListingsForSKU()
        {
            // Create a request.
            GetLowestOfferListingsForSKURequest request = new GetLowestOfferListingsForSKURequest();
            string sellerId = "example";
            request.SellerId = sellerId;
            string mwsAuthToken = "example";
            request.MWSAuthToken = mwsAuthToken;
            string marketplaceId = "example";
            request.MarketplaceId = marketplaceId;
            SellerSKUListType sellerSKUList = new SellerSKUListType();
            request.SellerSKUList = sellerSKUList;
            string itemCondition = "example";
            request.ItemCondition = itemCondition;
            bool excludeMe = true;
            request.ExcludeMe = excludeMe;
            return this.client.GetLowestOfferListingsForSKU(request);
        }

        public GetLowestPricedOffersForASINResponse InvokeGetLowestPricedOffersForASIN()
        {
            // Create a request.
            GetLowestPricedOffersForASINRequest request = new GetLowestPricedOffersForASINRequest();
            string sellerId = "example";
            request.SellerId = sellerId;
            string mwsAuthToken = "example";
            request.MWSAuthToken = mwsAuthToken;
            string marketplaceId = "example";
            request.MarketplaceId = marketplaceId;
            string asin = "example";
            request.ASIN = asin;
            string itemCondition = "example";
            request.ItemCondition = itemCondition;
            return this.client.GetLowestPricedOffersForASIN(request);
        }

        public GetLowestPricedOffersForSKUResponse InvokeGetLowestPricedOffersForSKU()
        {
            // Create a request.
            GetLowestPricedOffersForSKURequest request = new GetLowestPricedOffersForSKURequest();
            string sellerId = "example";
            request.SellerId = sellerId;
            string mwsAuthToken = "example";
            request.MWSAuthToken = mwsAuthToken;
            string marketplaceId = "example";
            request.MarketplaceId = marketplaceId;
            string sellerSKU = "example";
            request.SellerSKU = sellerSKU;
            string itemCondition = "example";
            request.ItemCondition = itemCondition;
            return this.client.GetLowestPricedOffersForSKU(request);
        }

        public GetMatchingProductResponse InvokeGetMatchingProduct()
        {
            // Create a request.
            GetMatchingProductRequest request = new GetMatchingProductRequest();
            string sellerId = "example";
            request.SellerId = sellerId;
            string mwsAuthToken = "example";
            request.MWSAuthToken = mwsAuthToken;
            string marketplaceId = "example";
            request.MarketplaceId = marketplaceId;
            ASINListType asinList = new ASINListType();
            request.ASINList = asinList;
            return this.client.GetMatchingProduct(request);
        }

        public GetMatchingProductForIdResponse InvokeGetMatchingProductForId()
        {
            // Create a request.
            GetMatchingProductForIdRequest request = new GetMatchingProductForIdRequest();
            string sellerId = "example";
            request.SellerId = sellerId;
            string mwsAuthToken = "example";
            request.MWSAuthToken = mwsAuthToken;
            string marketplaceId = "example";
            request.MarketplaceId = marketplaceId;
            string idType = "example";
            request.IdType = idType;
            IdListType idList = new IdListType();
            request.IdList = idList;
            return this.client.GetMatchingProductForId(request);
        }

        public GetMyPriceForASINResponse InvokeGetMyPriceForASIN()
        {
            // Create a request.
            GetMyPriceForASINRequest request = new GetMyPriceForASINRequest();
            string sellerId = "example";
            request.SellerId = sellerId;
            string mwsAuthToken = "example";
            request.MWSAuthToken = mwsAuthToken;
            string marketplaceId = "example";
            request.MarketplaceId = marketplaceId;
            ASINListType asinList = new ASINListType();
            request.ASINList = asinList;
            return this.client.GetMyPriceForASIN(request);
        }

        public GetMyPriceForSKUResponse InvokeGetMyPriceForSKU()
        {
            // Create a request.
            GetMyPriceForSKURequest request = new GetMyPriceForSKURequest();
            string sellerId = "example";
            request.SellerId = sellerId;
            string mwsAuthToken = "example";
            request.MWSAuthToken = mwsAuthToken;
            string marketplaceId = "example";
            request.MarketplaceId = marketplaceId;
            SellerSKUListType sellerSKUList = new SellerSKUListType();
            request.SellerSKUList = sellerSKUList;
            return this.client.GetMyPriceForSKU(request);
        }

        public GetProductCategoriesForASINResponse InvokeGetProductCategoriesForASIN()
        {
            // Create a request.
            GetProductCategoriesForASINRequest request = new GetProductCategoriesForASINRequest();
            string sellerId = "example";
            request.SellerId = sellerId;
            string mwsAuthToken = "example";
            request.MWSAuthToken = mwsAuthToken;
            string marketplaceId = "example";
            request.MarketplaceId = marketplaceId;
            string asin = "example";
            request.ASIN = asin;
            return this.client.GetProductCategoriesForASIN(request);
        }

        public GetProductCategoriesForSKUResponse InvokeGetProductCategoriesForSKU()
        {
            // Create a request.
            GetProductCategoriesForSKURequest request = new GetProductCategoriesForSKURequest();
            string sellerId = "example";
            request.SellerId = sellerId;
            string mwsAuthToken = "example";
            request.MWSAuthToken = mwsAuthToken;
            string marketplaceId = "example";
            request.MarketplaceId = marketplaceId;
            string sellerSKU = "example";
            request.SellerSKU = sellerSKU;
            return this.client.GetProductCategoriesForSKU(request);
        }

        public GetServiceStatusResponse InvokeGetServiceStatus()
        {
            // Create a request.
            GetServiceStatusRequest request = new GetServiceStatusRequest();
            string sellerId = "example";
            request.SellerId = sellerId;
            string mwsAuthToken = "example";
            request.MWSAuthToken = mwsAuthToken;
            return this.client.GetServiceStatus(request);
        }

        public ListMatchingProductsResponse InvokeListMatchingProducts()
        {
            // Create a request.
            ListMatchingProductsRequest request = new ListMatchingProductsRequest();
            string sellerId = "example";
            request.SellerId = sellerId;
            string mwsAuthToken = "example";
            request.MWSAuthToken = mwsAuthToken;
            string marketplaceId = "example";
            request.MarketplaceId = marketplaceId;
            string query = "example";
            request.Query = query;
            string queryContextId = "example";
            request.QueryContextId = queryContextId;
            return this.client.ListMatchingProducts(request);
        }

        public GetCompetitivePricingForASINResponse GetCompetitivePricingForASIN(GetCompetitivePricingForASINRequest request)
        {
            throw new NotImplementedException();
        }

        public GetCompetitivePricingForSKUResponse GetCompetitivePricingForSKU(GetCompetitivePricingForSKURequest request)
        {
            throw new NotImplementedException();
        }

        public GetLowestOfferListingsForASINResponse GetLowestOfferListingsForASIN(GetLowestOfferListingsForASINRequest request)
        {
            throw new NotImplementedException();
        }

        public GetLowestOfferListingsForSKUResponse GetLowestOfferListingsForSKU(GetLowestOfferListingsForSKURequest request)
        {
            throw new NotImplementedException();
        }

        public GetLowestPricedOffersForASINResponse GetLowestPricedOffersForASIN(GetLowestPricedOffersForASINRequest request)
        {
            throw new NotImplementedException();
        }

        public GetLowestPricedOffersForSKUResponse GetLowestPricedOffersForSKU(GetLowestPricedOffersForSKURequest request)
        {
            throw new NotImplementedException();
        }

        public GetMatchingProductResponse GetMatchingProduct(GetMatchingProductRequest request)
        {
            throw new NotImplementedException();
        }

        public GetMatchingProductForIdResponse GetMatchingProductForId(GetMatchingProductForIdRequest request)
        {
            throw new NotImplementedException();
        }

        public GetMyPriceForASINResponse GetMyPriceForASIN(GetMyPriceForASINRequest request)
        {
            throw new NotImplementedException();
        }

        public GetMyPriceForSKUResponse GetMyPriceForSKU(GetMyPriceForSKURequest request)
        {
            throw new NotImplementedException();
        }

        public GetProductCategoriesForASINResponse GetProductCategoriesForASIN(GetProductCategoriesForASINRequest request)
        {
            throw new NotImplementedException();
        }

        public GetProductCategoriesForSKUResponse GetProductCategoriesForSKU(GetProductCategoriesForSKURequest request)
        {
            throw new NotImplementedException();
        }

        public GetServiceStatusResponse GetServiceStatus(GetServiceStatusRequest request)
        {
            throw new NotImplementedException();
        }

        public ListMatchingProductsResponse ListMatchingProducts(ListMatchingProductsRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
