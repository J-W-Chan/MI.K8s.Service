using MI.Untity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Service.ShopCar.Model.Response
{
    public class GetShopListResponse:BaseResponse
    {
        public GetShopListResponse()
        {
            ShopLists = new List<ShopList>();
        }
        public List<ShopList> ShopLists { get; set; }
        public double TotalPrice { get; set; }
        public int ShopCount { get; set; }
    }

    public class ShopList
    {
        public int? CarID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int? VersionID { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        //小计 同一件商品
        public double Subtotal { get; set; }
        public string versionInfo { get; set; }
        public string ProductImg { get; set; }
    }
}
