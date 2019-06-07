using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Web.Models
{
    public class IdentityTokenModel
    {
        public string access_token { get; set; }
        public int? expires_in { get; set; }
        public string token_type { get; set; }
    }
}
