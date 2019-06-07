using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MI.Service.Account.Entity;
using MI.Service.Account.Model.Response;
using MI.Service.Account.Model;
using MI.Service.Account.Model.Request;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;

namespace MI.Service.IdentityServer.Controllers
{
    [EnableCors("AllowCors")]
    //[Authorize]
    public class MiUserController : Controller
    {
        public MIContext _context;
        public MiUserController(MIContext _context)
        {
            this._context = _context;
        }


        public string Index()
        {
            return "Successful";
        }

        [HttpPost]
        public async Task<SSOLoginResponse> SSOLogin([FromBody]SSOLoginRequest request)
        {
            SSOLoginResponse response = new SSOLoginResponse();
            try
            {
                if (!string.IsNullOrEmpty(request.UserName) && !string.IsNullOrEmpty(request.Password))
                {
                    var user = _context.UserEntities.FirstOrDefault(a => a.CustomerPhone.Equals(request.UserName));
                    if (user == null)
                    {
                        response.Successful = false;
                        response.Message = "用户名或密码错误！";
                        return response;
                    }
                    if (user.CustomerPwd == request.Password)
                    {
                        return response;
                    }
                }
                response.Successful = false;
                response.Message = "用户名密码不能为空！";
            }
            catch (Exception ex)
            {
                response.Successful = false;
                response.Message = ex.Message;
            }
            
            return response;
        }

        [HttpPost]
        public async Task<RegisterResponse> Register([FromBody]RegisterRequest request)
        {
            RegisterResponse response = new RegisterResponse();
            response.Message="收到请求，用户名：" + request.UserName;
            try
            {
                var user = _context.UserEntities.FirstOrDefault(a => a.CustomerPhone.Equals(request.UserName));
                if (user != null)
                {
                    response.Successful = false;
                    response.Message = "该用户名已存在！";
                }

                UserEntity entity = new UserEntity
                {
                    CustomerPhone = request.UserName,
                    CustomerPwd = request.Password,
                    LastLoginTime = DateTime.Now,
                    ErrorLogin = 0
                };

                _context.UserEntities.Add(entity);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Successful = false;
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        public async Task<GetShopCarByUserNameResponse> GetUserInfoByUserNameAsync([FromBody]GetShopCarByUserNameRequest request)
        {
            GetShopCarByUserNameResponse response = new GetShopCarByUserNameResponse();
            var userInfo = await _context.UserEntities.FirstOrDefaultAsync(a => a.CustomerPhone == request.UserName);
            if(userInfo!=null)
            {
                response.userInfo.PKID = userInfo.PKID;
                response.userInfo.LastLoginTime = userInfo.LastLoginTime;
            }
            return response;
        }
    }
}