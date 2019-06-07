using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Untity
{
    public class BaseResponse
    {
        public BaseResponse()
        {
            Successful = true;
        }
        public bool Successful { get; set; }
        public string Message { get; set; }
    }
}
