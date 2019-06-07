using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Service.Picture.Model.Response
{
    public class QueryStartProductResponse:BaseResponse
    {
        public QueryStartProductResponse()
        {
            StartProductList = new List<StartProductModel>();
        }

        public List<StartProductModel> StartProductList { get; set; }
    }

    public class StartProductModel
    {
        public int PKID { get; set; }
        public string StartName { get; set; }
        public string Describe { get; set; }
        public int Price { get; set; }
        public string StartImg { get; set; }
        public string LinkPage { get; set; }
    }
}
