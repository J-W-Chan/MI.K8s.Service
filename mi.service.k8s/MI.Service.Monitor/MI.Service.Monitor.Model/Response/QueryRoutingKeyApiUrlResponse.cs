using MI.Untity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Service.Monitor.Model.Response
{
    public class QueryRoutingKeyApiUrlResponse : BaseResponse
    {
        public QueryRoutingKeyApiUrlResponse()
        {
            ApiUrlList = new List<string>();
        }

        public List<string> ApiUrlList { get; set; }
    }
}
