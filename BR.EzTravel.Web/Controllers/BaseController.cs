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

        protected List<SelectListItem> GetList(ListType type, string defaultValue = "", string defaultText = "default", bool defaultItem = true)
        {
            var resultList = new List<SelectListItem>();
            if (defaultText == "default")
                defaultText = "--Please select--";

            switch (type)
            {
                case ListType.Category:
                    var categories = db.refcategories.Where(a => a.Active).ToList();
                    resultList = categories.Select(a =>
                        new SelectListItem()
                        {
                            Text = a.Name,
                            Value = a.ID.ToString()
                        }).OrderBy(a => a.Text).ToList();
                    break;

                case ListType.Country:
                    var countries = db.refcountries.Where(a => a.Active).ToList();
                    resultList = countries.Select(a =>
                        new SelectListItem()
                        {
                            Text = a.Name,
                            Value = a.ID.ToString()
                        }).OrderBy(a => a.Text).ToList();
                    break;

                default:
                    break;
            }


            if (defaultItem)
            {
                resultList.Insert(0, new SelectListItem()
                {
                    Text = defaultText,
                    Value = defaultValue,
                    Selected = true
                });
            }

            return resultList;
        }
    }
}