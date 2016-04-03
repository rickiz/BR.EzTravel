using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BR.EzTravel.Web.Models;

namespace BR.EzTravel.Web.Areas.EN.Controllers
{
    public class BaseEnController : BR.EzTravel.Web.Controllers.BaseController
    {
        public BaseEnController()
        {
            language = Language.EN;
            lang = Language.EN.ToString();
        }
    }
}