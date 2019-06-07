using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MI.Service.Picture.Entity;
using MI.Service.Picture.Model.Request;
using MI.Service.Picture.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MI.Service.Picture.Controllers
{
    //[Authorize]
    public class QueryPictureController : Controller
    {
        public MIContext _context;

        public QueryPictureController(MIContext context)
        {
            _context = context;
        }

        public string Index()
        {
            return "Successful";
        }

        public QueryStartProductResponse QueryStartProduct([FromBody]QueryStartProductRequest request)
        {
            QueryStartProductResponse response = new QueryStartProductResponse();
            List<StartProdect> list = _context.StartProdects.OrderBy(a => a.PKID).Skip((request.PageIndex - 1) * request.PageNum).Take(request.PageNum).ToList();
            foreach(var item in list)
            {
                response.StartProductList.Add(new StartProductModel
                {
                    PKID = item.PKID,
                    StartName = item.StartName,
                    Describe = item.Describe,
                    Price = item.Price,
                    StartImg = item.StartImg,
                    LinkPage = item.LinkPage
                });
            }
            return response;
        }


        public QuerySlideImgResponse QuerySlideImg()
        {
            QuerySlideImgResponse response = new QuerySlideImgResponse();
            var list= _context.SlideShowImgs.Where(s => s.PushHome == true).ToList();
            foreach(var item in list)
            {
                response.SlideImgList.Add(new SlideImgModel
                {
                    Id=item.PKID,
                    ImgName=item.ImgName,
                    ProductID=item.ProductID,
                    LinkPage=item.LinkPage,
                    PushHome=item.PushHome
                });
            }
            return response;
        }

        public QueryHadrWareResponse QueryHadrWare()
        {
            QueryHadrWareResponse response = new QueryHadrWareResponse();
            var list = _context.HardWares.Where(h => h.Home == true).ToList();
            foreach (var item in list)
            {
                response.HadrWareList.Add(new HadrWareModel
                {
                    PKID = item.PKID,
                    ProductName = item.ProductName,
                    Describe = item.Describe,
                    Price = item.Price,
                    HardWareImg = item.HardWareImg,
                    Home = item.Home,
                    LinkPage = item.LinkPage,
                    VersionID = item.VersionID,
                    ProductID = item.ProductID,
                });
            }
            return response;
        }
    }
}