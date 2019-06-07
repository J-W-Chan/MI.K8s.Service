using MI.Untity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Service.ShopCar.Model.Response
{
    public class GetColorResponse:BaseResponse
    {
        public int PKID { get; set; }

        public int VersionID { get; set; }

        public string Color { get; set; }

        public string ColorImg { get; set; }
    }
}
