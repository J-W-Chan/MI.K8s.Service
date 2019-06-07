using MI.Untity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Web.ApiModel.PictureService.Request
{
    public class QueryStartProductRequest
    {
        public int PageIndex { get; set; }
        public int PageNum { get; set; }
    }
}
