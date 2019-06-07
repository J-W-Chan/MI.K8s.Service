using MI.Untity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Service.Account.Model.Response
{
    public class GetShopCarByUserNameResponse:BaseResponse
    {
        public GetShopCarByUserNameResponse()
        {
            userInfo = new UserInfo();
        }
        public UserInfo userInfo { get; set; }
    }

    public class UserInfo
    {
        public int PKID { get; set; }
        public DateTime? LastLoginTime { get; set; }
    }
}
