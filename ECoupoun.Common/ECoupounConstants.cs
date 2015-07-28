using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ECoupoun.Common
{
    public class ECoupounConstants
    {
        /// <summary>
        /// REST Service URL
        /// </summary>
        public static string BestBuyRESTServiceURL = ConfigurationManager.AppSettings["RESTServiceURL"];
        public static string WalmartRESTServiceURL = ConfigurationManager.AppSettings["WalmartRESTServiceURL"];
        public static string EBayRESTServiceURL = ConfigurationManager.AppSettings["EBayRESTServiceURL"];

        /// <summary>
        /// Content Type Text
        /// </summary>
        public const string ContentTypeText = "Content-type";

        /// <summary>
        /// Content Type for making request
        /// </summary>
        public const string ContentTypeValue = "application/json";

        public const string TestGETAPI = "TestGETAPI";
        public const string TestPOSTAPI = "TestPOSTAPI";
        public const string InsertData = "InsertData";
        public const string GetAllProducts = "GetAllProducts";
    }
}
