using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Service.ShopCar.Model.Request
{
    public class AddCountRequest
    {
        public int? CarId { get; set; }

        public int ProductId { get; set; }

        public int VersionId { get; set; }

        public int Num { get; set; }
    }
}
