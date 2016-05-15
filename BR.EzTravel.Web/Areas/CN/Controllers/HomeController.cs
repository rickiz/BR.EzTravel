using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BR.EzTravel.Web.Areas.CN.Controllers
{
    public class HomeController : BaseCnController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}