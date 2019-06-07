using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Web.ApiModel
{
    public class BaseResponse
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
    }
}
