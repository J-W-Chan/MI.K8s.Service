using MI.Untity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Service.ShopCar.Model.Response
{
    public class GetShopCarListResponse:BaseResponse
    {
        public GetShopCarListResponse()
        {
            GetShopCarListModels = new List<GetShopCarListModel>();
        }

        public List<GetShopCarListModel> GetShopCarListModels { get; set; }
    }

    public class GetShopCarListModel
    {
        public int ID { get; set; }
        public int? CarID { get; set; }
        public int ProductID { get; set; }
        public int VersionID { get; set; }
        public int Count { get; set; }
        public double UnitPrice { get; set; }
        public int? ColorID { get; set; }
        public bool? IsCheck { get; set; }
    }
}
