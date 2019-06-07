using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MI.Web.Controllers
{
    public class SharedController : Controller
    {
        public ActionResult PageHeader()
        {
            return PartialView();
        }
    }
}