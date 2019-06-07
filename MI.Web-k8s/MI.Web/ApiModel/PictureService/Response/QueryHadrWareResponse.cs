using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Web.ApiModel.PictureService.Response
{
    public class QueryHadrWareResponse
    {
        public QueryHadrWareResponse()
        {
            HadrWareList = new List<HadrWareModel>();
        }
        public List<HadrWareModel> HadrWareList { get; private set; }
    }

    public class HadrWareModel
    {
        public int PKID { get; set; }
        public string ProductName { get; set; }
        public string Describe { get; set; }
        public int? Price { get; set; }
        public string HardWareImg { get; set; }
        public bool? Home { get; set; }
        public string LinkPage { get; set; }
        public int? VersionID { get; set; }
        public int ProductID { get; set; }
    }
}
