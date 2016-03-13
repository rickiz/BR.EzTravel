using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BR.EzTravel.Web.Helpers;
using BR.EzTravel.Web.Models;

namespace BR.EzTravel.Web.Controllers
{
    public abstract partial class BaseController : Controller
    {
        protected ExHolidayEntities db = new ExHolidayEntities();
        protected Language language = Language.EN;

        protected override void OnException(ExceptionContext filterContext)
        {
            var exception = filterContext.Exception;

            exception.Log();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    db.Dispose();
                }
                catch (Exception ex)
                {
                    ex.Log();
                }
            }

            base.Dispose(disposing);
        }
    }
}