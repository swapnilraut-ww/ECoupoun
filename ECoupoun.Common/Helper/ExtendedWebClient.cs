using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace ECoupoun.Common.Helper
{
    public partial class ExtendedWebClient : WebClient
    {
        public int Timeout { get; set; }

        public ExtendedWebClient(Uri address)
        {
            var WebClientTimeout = int.MaxValue;   //600000;//In Milli seconds
            this.Timeout = WebClientTimeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var webRequest = base.GetWebRequest(address);
            webRequest.Timeout = this.Timeout;
            return webRequest;
        }
    }
}
