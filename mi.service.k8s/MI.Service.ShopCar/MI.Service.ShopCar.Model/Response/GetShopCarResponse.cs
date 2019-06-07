using MI.Untity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Service.ShopCar.Model.Response
{
    public class GetShopCarResponse:BaseResponse
    {
        public int? CarId { get; set; }
        public int? UserId { get; set; }
    }
}
