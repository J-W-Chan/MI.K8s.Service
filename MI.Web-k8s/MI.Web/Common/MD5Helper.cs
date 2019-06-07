using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace MI.Web.Common
{
    public class MD5Helper
    {
        #region MD5加密密码 + public string Get_MD5(string userPwd)
        public static string Get_MD5(string userPwd)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(userPwd));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }
        #endregion
    }
}
