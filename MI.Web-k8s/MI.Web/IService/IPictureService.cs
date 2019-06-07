using MI.Web.ApiModel.PictureService.Request;
using MI.Web.ApiModel.PictureService.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Web.IService
{
    public interface IPictureService
    {
        Task<QueryStartProductResponse> QueryStartProduct(QueryStartProductRequest request);
        Task<QuerySlideImgResponse> QuerySlideImg();
        Task<QueryHadrWareResponse> QueryHadrWare();
    }
}
