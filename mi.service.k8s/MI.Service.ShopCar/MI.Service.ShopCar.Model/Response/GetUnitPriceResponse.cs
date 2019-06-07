using MI.Untity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Service.ShopCar.Model.Response
{
    public class GetUnitPriceResponse:BaseResponse
    {
        public GetUnitPriceResponse()
        {
            UnitPriceList = new List<UnitPriceModel>();
        }
        public List<UnitPriceModel> UnitPriceList { get; set; }
    }

    public class UnitPriceModel
    {
        public int ID { get; set; }
        public int? ProductID { get; set; }
        public int? VersionID { get; set; }
        public int? ColorID { get; set; }
        public double Price { get; set; }
    }
}
