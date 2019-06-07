using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Service.ShopCar.Model.Request
{
    public class GetShopCarListRequest
    {
        public int? CarId { get; set; }
        public int? ProductId { get; set; }
        public int? VersionId { get; set; }
    }
}
