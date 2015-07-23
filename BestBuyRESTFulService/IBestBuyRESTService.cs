using ECoupoun.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace BestBuyRESTFulService
{
    [ServiceContract]
    public interface IBestBuyRESTService
    {
        /// <summary>
        /// Making Test for GET method is working or not
        /// </summary>
        /// <returns>Response from API</returns>
        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string TestGETAPI();

        /// <summary>
        /// Making Test for POST method is working or not
        /// </summary>
        /// <returns>Response from API</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string TestPOSTAPI();

        /// <summary>
        /// Insert Product Details in Database
        /// </summary>
        /// <returns>Response from API</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string InsertData();

        /// <summary>
        /// Get All Products
        /// </summary>
        /// <returns>Response from API</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Products> GetAllProducts();
    }
}
