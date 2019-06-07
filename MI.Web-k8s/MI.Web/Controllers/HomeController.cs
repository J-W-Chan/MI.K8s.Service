using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MI.Web.ApiModel.PictureService.Request;
using MI.Web.ApiModel.PictureService.Response;
using MI.Web.Common;
using MI.Web.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MI.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IPictureService pictureService;

        public HomeController(IConfiguration configuration, IPictureService pictureService)
        {
            this.configuration = configuration;
            this.pictureService = pictureService;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.UserName = string.Empty;
            if (Request.Cookies["MIUserName"] != null)
            {
                if(Request.Cookies.TryGetValue("MIUserName",out string userName))
                {
                    ViewBag.UserName = userName;
                }
            }

            QueryStartProductRequest request = new QueryStartProductRequest { PageIndex = 1, PageNum = 5 };
            QueryStartProductResponse response = null;
            try
            {
                response = await pictureService.QueryStartProduct(request);
                if (response.Successful)
                {
                    //轮播图
                    var querySlideImgResponse= await pictureService.QuerySlideImg();
                    ViewData["SlideImg"] = querySlideImgResponse.SlideImgList;

                    //查询智能硬件表数据
                    var queryHadrWareResponse = await pictureService.QueryHadrWare();
                    ViewData["HradWare"] = queryHadrWareResponse.HadrWareList;
                    return View(response.StartProductList);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            
            return View();
        }

        #region 明星产品模块 上翻 下翻功能
        public async Task<JsonResult> StartSlide(int StartIndex)
        {
            QueryStartProductRequest request = new QueryStartProductRequest { PageIndex = StartIndex, PageNum = 5 };
            var response = await pictureService.QueryStartProduct(request);
            return Json(response.StartProductList);
        }
        #endregion
    }
}