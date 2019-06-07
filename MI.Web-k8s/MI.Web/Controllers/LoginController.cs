using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MI.Web.Common;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using MI.Web.IService;
using EventBus.Abstractions;
using MI.Untity;
using MI.Service.Account.Model;
using MI.Service.Account.Model.Response;
using MI.Service.Account.Model.Request;

namespace MI.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IMemoryCache cache;
        private readonly ILogger _logger;
        private readonly IAccountService accountService;
        private readonly IEventBus _eventBus;
        public LoginController(IConfiguration configuration, IMemoryCache cache, ILogger<LoginController> logger, IAccountService accountService, IEventBus _eventBus)
        {

            this.configuration = configuration;
            this.cache = cache;
            _logger = logger;
            this.accountService = accountService;
            this._eventBus = _eventBus;
        }

        public ActionResult Login()
        {
            _logger.LogError("MI.Web测试！");
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        #region 注册
        public JsonResult RegisterUser(string UserName, string UserPwd)
        {
            try
            {
                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(UserPwd))
                {
                    RegisterRequest request = new RegisterRequest
                    {
                        UserName = UserName,
                        Password = UserPwd
                    };

                    _eventBus.Publish("UserRegister", request);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "注册失败！");
                return Json("err");
            }
            return Json("ok");
        }
        #endregion

        #region 登录功能 + public JsonResult Login(string UserName, string UserPwd)
        public async Task<JsonResult> UserLogin(string UserName, string UserPwd)
        {
            SSOLoginRequest request = new SSOLoginRequest { UserName = UserName, Password = MI.Web.Common.MD5Helper.Get_MD5(UserPwd) };
            SSOLoginResponse response = null;
            try
            {
                response = await accountService.SSOLogin(request);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "登录发生错误！");
                return Json(ex.Message);
            }
            if(response.Successful)
            {
                return Json("ok");
            }
            return Json(response.Message);
        }
        #endregion
    }
}