using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BR.EzTravel.Web.Models;

namespace BR.EzTravel.Web.Areas.CN.Controllers
{
    public class BaseCnController : BR.EzTravel.Web.Controllers.BaseController
    {
        public BaseCnController()
        {
            language = Language.CN;
            lang = Language.CN.ToString();
        }
    }
}