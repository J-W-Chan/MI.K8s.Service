using MI.Untity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Service.ShopCar.Model.Response
{
    public class GetShopCarByUserNameResponse:BaseResponse
    {
        public int CarID { get; set; }
        public int? UserID { get; set; }
    }
}
