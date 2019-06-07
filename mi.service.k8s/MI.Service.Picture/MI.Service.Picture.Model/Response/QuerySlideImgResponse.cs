using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Service.Picture.Model.Response
{
    public class QuerySlideImgResponse
    {
        public QuerySlideImgResponse()
        {
            SlideImgList = new List<SlideImgModel>();
        }
        public List<SlideImgModel> SlideImgList { get; private set; }
    }

    public class SlideImgModel
    {
        public int Id { get; set; }

        public string ImgName { get; set; }

        public int ProductID { get; set; }

        public string LinkPage { get; set; }

        public bool PushHome { get; set; }
    }
}
