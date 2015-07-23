using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ECoupoun.ConsoleApp
{
    class Program
    {
        private const string URL = "http://api.remix.bestbuy.com/v1/products%28longDescription=iPhone*%29?show=sku,name,image,modelNumber,regularPrice,salePrice&apiKey=93j5ggwxgfraye8unmhvgzgv&format=json";

        static void Main(string[] args)
        {
            HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;

            // Get response  
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                StreamReader reader = new StreamReader(response.GetResponseStream());

                var serializer = new JavaScriptSerializer();
                var jsonObject = serializer.Deserialize<BestBuyProducts>(reader.ReadToEnd());

                // Console application output  
                Console.WriteLine(reader.ReadToEnd());
            }

            Console.ReadLine();

        }
    }
}
