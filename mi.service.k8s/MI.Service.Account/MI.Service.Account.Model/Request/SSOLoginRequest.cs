using MI.Service.Account.Model.Request;
using MI.Untity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Service.Account.Model
{
    public class SSOLoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
