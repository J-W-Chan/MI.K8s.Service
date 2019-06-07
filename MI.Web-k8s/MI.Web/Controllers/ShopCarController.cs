using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus.Abstractions;
using MI.Service.ShopCar.Model.Request;
using MI.Service.ShopCar.Model.Response;
using MI.Web.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MI.Web.Controllers
{
    public class ShopCarController : Controller
    {
        private readonly IShopCarService shopCarService;
        private readonly IEventBus eventBus;
        private readonly ILogger logger;

        public ShopCarController(IShopCarService shopCarService, IEventBus eventBus, ILogger<ShopCarController> logger)
        {
            this.shopCarService = shopCarService;
            this.eventBus = eventBus;
            this.logger = logger;
        }

        public async Task<IActionResult> ShopCar()
        {
            GetShopListRequest request = new GetShopListRequest { UserName = "wei" };
            var getShopListResponse = await shopCarService.GetShopList(request);

            ViewBag.Total = getShopListResponse.TotalPrice;
            ViewBag.Count = getShopListResponse.ShopCount;

            if (!getShopListResponse.ShopLists.Any())
            {
                return Content("购物车为空");
            }
            else
            {
                return View(getShopListResponse.ShopLists);
            }
        }

        public async Task<JsonResult> CountChange(int? CarID, int ProductID, int VersionID, string Operate)
        {
            var request = new AddCountRequest { CarId = CarID, ProductId = ProductID, VersionId = VersionID };
            if (Operate == "reduce")
            {
                request.Num = -1;
            }
            else
            {
                request.Num = 1;
            }

            try
            {
                eventBus.Publish("ShopCarCountChange", request);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "修改购物车数量出错！");
                return Json("error");
            }
            //var response = await shopCarService.AddCountAsync(request);
            return Json(0);
        }

        public async Task<JsonResult> AddCar(int ProductID, int VersionID)
        {
            var getColorResponse = await shopCarService.GetColorAsync(new GetColorRequest { VersionId = VersionID });
            //发送新增购物车MQ
            var addShopCarRequest = new AddShopCarRequest { ProductID = ProductID, VersionID = VersionID };
            try
            {
                eventBus.Publish("AddShopCar", addShopCarRequest);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "添加购物车方法出错！");
                return Json("error");
            }

            return Json("ok");
        }

        public async Task<JsonResult> DelProduct(int ProductID, int VersionID)
        {
            var response = await shopCarService.DelProductAsync(new DelProductRequest { ProductId = ProductID, VersionId = VersionID });
            if (response.Successful)
            {
                return Json("ok");
            }
            return Json("err");
        }

        public async Task<JsonResult> ChangeCheck(int ProductID, int VersionID)
        {
            var response = await shopCarService.ChangeCheckAsync(new ChangeCheckRequest { ProductId = ProductID, VersionId = VersionID });
            return Json(response.TotalCheckProice);
        }
    }
}