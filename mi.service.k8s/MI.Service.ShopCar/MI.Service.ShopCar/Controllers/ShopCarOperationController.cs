using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MI.Service.ShopCar.Entity;
using MI.Service.ShopCar.Model.Request;
using MI.Service.ShopCar.Model.Response;
using MI.Service.ShopCar.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MI.Service.ShopCar.Controllers
{
    //[Authorize]
    public class ShopCarOperationController : Controller
    {
        private readonly MIContext _context;
        private readonly IAccountService accountService;
        private readonly ILogger logger;

        public ShopCarOperationController(MIContext _context, IAccountService accountService, ILogger<ShopCarOperationController> logger)
        {
            this._context = _context;
            this.accountService = accountService;
            this.logger = logger;
        }

        public string Index()
        {
            return "Successful";
        }

        /// <summary>
        /// 添加商品到购物车
        /// </summary>
        public async Task<AddShopCarResponse> AddCarAsync([FromBody]AddShopCarRequest request)
        {
            var response = new AddShopCarResponse();
            try
            {
                var colorId = _context.ColorVersionEntitys.FirstOrDefault(a => a.VersionID == request.VersionID)?.PKID;
                if (!colorId.HasValue)
                {
                    response.Successful = false;
                    response.Message = "该版本不存在，请重新选择！";
                }
                var unitPrice = _context.PriceEntitys.FirstOrDefault(p => p.ProductID == request.ProductID && p.VersionID == request.VersionID)?.Price;
                unitPrice = unitPrice ?? 0;

                var userInfo = accountService.GetUserInfoByUserNameAsync(new Account.Model.Request.GetShopCarByUserNameRequest { UserName = "wei" });
                var shopCar = _context.ShopCarEntitys.SingleOrDefault(a => a.UserId == userInfo.userInfo.PKID);
                if (shopCar == null)
                {
                    _context.ShopCarEntitys.Add(new ShopCarEntity { UserId = userInfo.userInfo.PKID });
                    _context.SaveChanges();
                    shopCar = _context.ShopCarEntitys.SingleOrDefault(a => a.UserId == userInfo.userInfo.PKID);
                }
                var carListItem = _context.CarListEntitys.SingleOrDefault(a => a.CarID == shopCar.PKID && a.ProductID == request.ProductID && a.VersionID == request.VersionID);
                if (carListItem != null)
                {
                    carListItem.Count += 1;
                    _context.Update(carListItem);
                }
                else
                {
                    _context.CarListEntitys.Add(new CarListEntity
                    {
                        CarID = shopCar.PKID,
                        ProductID = request.ProductID,
                        VersionID = request.VersionID,
                        Count = 1,
                        UnitPrice = unitPrice.Value,
                        ColorID = colorId.Value,
                        IsCheck = true
                    });
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Successful = false;
                response.Message=ex.Message;
                logger.LogError(ex, $"AddCarAsync方法报错 Message：{JsonConvert.SerializeObject(request)}");
            }
            
            return response;
        }

        /// <summary>
        /// 修改购物车产品的选中状态
        /// </summary>
        public async Task<ChangeCheckResponse> ChangeCheckAsync([FromBody]ChangeCheckRequest request)
        {
            ChangeCheckResponse response = new ChangeCheckResponse();
            try
            {
                var model = await _context.CarListEntitys.SingleOrDefaultAsync(a => a.ProductID == request.ProductId && a.VersionID == request.VersionId);
                if (model != null)
                {
                    model.IsCheck = true ? false : true;
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }

                var checkProduct = _context.CarListEntitys.Where(a => a.IsCheck == true).ToList();
                if (checkProduct.Any())
                {
                    response.TotalCheckProice = checkProduct.Sum(a => a.Count * a.UnitPrice);
                }
            }
            catch(Exception ex)
            {
                response.Successful = false;
                response.Message = ex.Message;
                logger.LogError(ex, $"ChangeCheckAsync方法报错 Message：{JsonConvert.SerializeObject(request)}");
            }
            
            return response;
        }

        /// <summary>
        /// 获取颜色
        /// </summary>
        public async Task<GetColorResponse> GetColorAsync([FromBody]GetColorRequest request)
        {
            GetColorResponse response = new GetColorResponse();
            try
            {
                var colorItem = await _context.ColorVersionEntitys.SingleOrDefaultAsync(a => a.VersionID == request.VersionId);
                response = new GetColorResponse
                {
                    PKID = colorItem.PKID,
                    Color = colorItem.Color,
                    ColorImg = colorItem.ColorImg,
                    VersionID = colorItem.VersionID
                };
            }
            catch(Exception ex)
            {
                response.Successful = false;
                response.Message = ex.Message;
                logger.LogError(ex, $"GetColorAsync方法报错 Message：{JsonConvert.SerializeObject(request)}");
            }

            return response;
        }

        /// <summary>
        /// 根据商品ID 和 版本ID删除购物车中对应的商品
        /// </summary>
        public async Task<DelProductResponse> DelProductAsync([FromBody]DelProductRequest request)
        {
            DelProductResponse response = new DelProductResponse();
            try
            {
                var shopCarList = _context.CarListEntitys.Where(a => a.ProductID == request.ProductId && a.VersionID == request.VersionId);
                _context.CarListEntitys.RemoveRange(shopCarList);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                response.Successful = false;
                response.Message = ex.Message;
                logger.LogError(ex, $"DelProductAsync方法报错 Message：{JsonConvert.SerializeObject(request)}");
            }
            return response;
        }

        /// <summary>
        /// 根据产品ID和版本ID查询对应的产品信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetShopCarProductResponse GetShopCarProductAsync([FromBody]GetShopCarProductRequest request)
        {
            GetShopCarProductResponse response = new GetShopCarProductResponse();
            try
            {
                var query = from p in _context.ProductEntities
                            join pr in _context.PriceEntitys
                            on p.ProductID equals pr.ProductID
                            join v in _context.VersionInfoEntitys
                            on pr.VersionID equals v.VersionID
                            where pr.ProductID == request.ProductId && v.VersionID == request.VersionId
                            select new
                            {
                                ProductName = p.ProductName,
                                Price = pr.Price,
                                ProductImg = p.ProductImg,
                                VersionInfo = v.VersionInfo
                            };

                var queryItem = query.FirstOrDefault();
                response = new GetShopCarProductResponse
                {
                    CarID = null,
                    ProductID = request.ProductId,
                    ProductName = queryItem.ProductName,
                    VersionID = request.VersionId,
                    versionInfo = queryItem.ProductName,
                    Price = queryItem.Price,
                    Count = 1,
                    Subtotal = queryItem.Price,
                    ProductImg = queryItem.ProductImg
                };
            }
            catch(Exception ex)
            {
                response.Successful = false;
                response.Message = ex.Message;
                logger.LogError(ex, $"GetShopCarProductAsync方法报错 Message：{JsonConvert.SerializeObject(request)}");
            }
            
            return response;
        }

        /// <summary>
        /// 根据商品ID得到分类
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public async Task<GetCategoryResponse> GetCategoryAsync([FromBody]GetCategoryRequest request)
        {
            GetCategoryResponse response = new GetCategoryResponse();
            try
            {
                var productItem = await _context.ProductEntities.FirstOrDefaultAsync(a => a.ProductID == request.ProductId);
                response.CategoryId = productItem?.CategoryID;
            }
            catch(Exception ex)
            {
                response.Successful = false;
                response.Message = ex.Message;
                logger.LogError(ex, $"GetCategoryAsync 方法报错 Message：{JsonConvert.SerializeObject(request)}");
            }

            return response;
        }

        /// <summary>
        /// 获取购物车产品
        /// </summary>
        public async Task<GetShopCarListResponse> GetShopCarListAsync([FromBody]GetShopCarListRequest request)
        {
            GetShopCarListResponse response = new GetShopCarListResponse();
            try
            {
                var query = _context.CarListEntitys.AsQueryable();
                if (request.CarId.HasValue)
                {
                    query = query.Where(a => a.CarID == request.CarId.Value);
                }
                if (request.ProductId.HasValue)
                {
                    query = query.Where(a => a.ProductID == request.ProductId.Value);
                }
                if (request.VersionId.HasValue)
                {
                    query = query.Where(a => a.VersionID == request.VersionId.Value);
                }
                foreach (var item in query.ToList())
                {
                    response.GetShopCarListModels.Add(new GetShopCarListModel
                    {
                        ID = item.PKID,
                        CarID = item.CarID,
                        ProductID = item.ProductID,
                        VersionID = item.VersionID,
                        Count = item.Count,
                        UnitPrice = item.UnitPrice,
                        ColorID = item.ColorID,
                        IsCheck = item.IsCheck
                    });
                }
            }
            catch(Exception ex)
            {
                response.Successful = false;
                response.Message = ex.Message;
                logger.LogError(ex, $"GetShopCarListAsync 方法报错 Message：{JsonConvert.SerializeObject(request)}");
            }
            
            return response;
        }

        /// <summary>
        /// 获取商品价格信息
        /// </summary>
        public async Task<GetUnitPriceResponse> GetUnitPriceAsync([FromBody]GetUnitPriceRequest request)
        {
            GetUnitPriceResponse response = new GetUnitPriceResponse();
            try
            {
                var query = _context.PriceEntitys.AsQueryable();
                if (request.ProductId.HasValue)
                {
                    query = query.Where(a => a.ProductID == request.ProductId.Value);
                }
                if (request.VersionId.HasValue)
                {
                    query = query.Where(a => a.VersionID == request.VersionId);
                }

                foreach (var item in query.ToList())
                {
                    response.UnitPriceList.Add(new UnitPriceModel
                    {
                        ID = item.PKID,
                        ProductID = item.ProductID,
                        VersionID = item.VersionID,
                        ColorID = item.ColorID,
                        Price = item.Price
                    });
                }
            }
            catch(Exception ex)
            {
                response.Successful = false;
                response.Message = ex.Message;
                logger.LogError(ex, $"GetUnitPriceAsync 方法报错 Message：{JsonConvert.SerializeObject(request)}");
            }
            
            return response;
        }

        /// <summary>
        /// 根据用户名查询购物车ID
        /// </summary>
        public async Task<GetShopCarResponse> GetShopCarAsync([FromBody]GetShopCarRequest request)
        {
            GetShopCarResponse response = new GetShopCarResponse();
            try
            {
                var customer = _context.CustomerEntitys.FirstOrDefault(a => a.CustomerPhone == request.UserName);
                if (customer != null)
                {
                    var shopCar = await _context.ShopCarEntitys.SingleOrDefaultAsync(a => a.UserId == customer.CustomerID);
                    if (shopCar == null)
                    {
                        shopCar = new ShopCarEntity
                        {
                            UserId = customer.CustomerID
                        };
                        _context.Add(shopCar);
                        await _context.SaveChangesAsync();
                    }
                    shopCar = await _context.ShopCarEntitys.SingleOrDefaultAsync(a => a.UserId == customer.CustomerID);
                    response.UserId = customer.CustomerID;
                    response.CarId = shopCar.PKID;
                }
            }
            catch(Exception ex)
            {
                response.Successful = false;
                response.Message = ex.Message;
                logger.LogError(ex, $"GetShopCarAsync 方法报错 Message：{JsonConvert.SerializeObject(request)}");
            }
            
            return response;
        }

        /// <summary>
        /// 更改购物清单产品数量 增加 减少
        /// </summary>
        public async Task<AddCountResponse> AddCountAsync([FromBody]AddCountRequest request)
        {
            AddCountResponse response = new AddCountResponse();
            try
            {
                var model = _context.CarListEntitys.FirstOrDefault(a => a.CarID == request.CarId && a.ProductID == request.ProductId && a.VersionID == request.VersionId);
                model.Count += request.Num;
                _context.Update(model);
                await _context.SaveChangesAsync();

                response.Count = model.Count;
            }
            catch(Exception ex)
            {
                response.Successful = false;
                response.Message = ex.Message;
                logger.LogError(ex, $"AddCountAsync 方法报错 Message：{JsonConvert.SerializeObject(request)}");
            }
            
            return response;
        }

        /// <summary>
        /// 根据购物车ID获取对应的 购物清单
        /// </summary>
        [HttpPost]
        public async Task<GetShopListResponse> GetShopListAsync([FromBody]GetShopListRequest request)
        {
            GetShopListResponse response = new GetShopListResponse();
            try
            {
                var customer = _context.CustomerEntitys.FirstOrDefault(a => a.CustomerPhone == request.UserName);
                if (customer != null)
                {
                    var shopCar = _context.ShopCarEntitys.SingleOrDefault(a => a.UserId == customer.CustomerID);
                    if (shopCar == null)
                    {
                        shopCar = new ShopCarEntity
                        {
                            UserId = customer.CustomerID
                        };
                        _context.Add(shopCar);
                        _context.SaveChanges();
                    }
                    shopCar = _context.ShopCarEntitys.SingleOrDefault(a => a.UserId == customer.CustomerID);
                    ResetCheck(shopCar.PKID);
                    var query = (from cl in _context.CarListEntitys
                                 join v in _context.VersionInfoEntitys
                                 on cl.VersionID equals v.ProductID
                                 join p in _context.ProductEntities
                                 on cl.ProductID equals p.ProductID
                                 where cl.CarID == shopCar.PKID
                                 select new ShopList
                                 {
                                     CarID = cl.CarID,
                                     ProductID = p.ProductID,
                                     ProductName = p.ProductName,
                                     VersionID = cl.VersionID,
                                     versionInfo = v.VersionInfo,
                                     Price = cl.UnitPrice,
                                     Count = cl.Count,
                                     ProductImg = p.ProductImg,
                                     Subtotal = cl.UnitPrice * cl.Count
                                 }).ToList();

                    response.TotalPrice = query.Sum(a => a.Subtotal);
                    response.ShopCount = query.Sum(a => a.Count);

                    response.ShopLists.AddRange(query);
                } 
            }
            catch (Exception ex)
            {
                response.Successful = false;
                response.Message = ex.Message;
                logger.LogError(ex, $"GetShopListAsync 方法报错 Message：{JsonConvert.SerializeObject(request)}");
            }
            return response;
        }

        public void ResetCheck(int shopCarId)
        {
            var shopCarList = _context.CarListEntitys.Where(a => a.CarID == shopCarId && a.IsCheck == false).ToList();
            foreach (var item in shopCarList)
            {
                item.IsCheck = true;
            }
            _context.UpdateRange(shopCarList);
            _context.SaveChanges();
        }
    }
}