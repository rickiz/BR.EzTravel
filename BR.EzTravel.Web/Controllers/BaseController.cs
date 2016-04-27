using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BR.EzTravel.Web.Helpers;
using BR.EzTravel.Web.Models;
using log4net;
using System.Reflection;
using System.Web.Routing;
using System.Data.Entity;

namespace BR.EzTravel.Web.Controllers
{
    public abstract partial class BaseController : Controller
    {
        protected ExHolidayEntities db = new ExHolidayEntities();
        protected Language language = Language.EN;
        protected string lang = Language.EN.ToString();
        protected static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

        public string OldSearchGuid
        {
            get
            {
                return Session["Package_Search_OldGuid"] as string;
            }

            set
            {
                Session["Package_Search_OldGuid"] = value;
            }
        }

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

        protected override void Initialize(RequestContext requestContext)
        {
            Util.CheckSessionAccess(requestContext);
            base.Initialize(requestContext);
        }

        protected List<SelectListItem> GetList(ListType type, string defaultValue = "", string defaultText = "default", bool defaultItem = true)
        {
            var resultList = new List<SelectListItem>();
            if (defaultText == "default")
                defaultText = "--Please select--";

            switch (type)
            {
                case ListType.Category:
                    var categories = db.refcategories.Where(a => a.Active && a.Language == lang).ToList();
                    resultList = categories.Select(a =>
                        new SelectListItem()
                        {
                            Text = a.Name,
                            Value = a.ID.ToString()
                        }).OrderBy(a => a.Text).ToList();
                    break;

                case ListType.Country:
                    var countries = db.refcountries.Where(a => a.Active && a.Language == lang).ToList();
                    resultList = countries.Select(a =>
                        new SelectListItem()
                        {
                            Text = a.Name,
                            Value = a.ID.ToString()
                        }).OrderBy(a => a.Text).ToList();
                    break;

                case ListType.PackageActivity:
                    var packageActivities = db.refpackageactivities.Where(a => a.Active && a.Language == lang).ToList();
                    resultList = packageActivities.Select(a =>
                        new SelectListItem()
                        {
                            Text = a.Name,
                            Value = a.ID.ToString()
                        }).OrderBy(a => a.Text).ToList();
                    break;

                case ListType.PackageCategory:
                    var packageCategories =
                        (from a in db.lnkmemberposts
                        join b in db.refcategories on a.CategoryID equals b.ID
                        where a.Language == lang && !a.CancelDT.HasValue && b.Active
                        group b by new { Name = b.Name, ID = b.ID } into c
                        select c).ToList();
                    resultList = packageCategories.Select(a =>
                        new SelectListItem()
                        {
                            Text = a.Key.Name,
                            Value = a.Key.ID.ToString()
                        }).OrderBy(a => a.Text).ToList();
                    break;

                case ListType.PackageCountry:
                    var packageCountries =
                        (from b in db.refcountries
                         join a in db.lnkmemberpostcountries on b.ID equals a.CountryID
                         where b.Active && b.Language == lang && a.Active
                         select new 
                         {
                             Name = b.Name,
                             ID = b.ID,
                         }).Distinct().ToList();
                    resultList = packageCountries.Select(a =>
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

        protected List<SelectListItem> GetPackageActivities()
        {
            var resultList = 
                db.refpackageactivities
                    .Where(a => a.Language == lang && a.Active)
                    .Select(a => new SelectListItem
                    {
                        Text = a.Name,
                        Value = a.ID.ToString()
                    }).OrderBy(a => a.Text).ToList();

            return resultList;
        }

        protected List<SelectListItem> GetPackageActivities(int[] selectedIDs)
        {
            var resultList = GetPackageActivities();

            resultList.ForEach(a => a.Selected = selectedIDs.Any(b => b == int.Parse(a.Value)));

            return resultList;
        }

        protected List<PackageCategory> GetPackageCategories()
        {
            var packageCategories =
                (from b in db.refcategories
                    join a in db.lnkmemberposts on b.ID equals a.CategoryID into tempA
                    where b.Active && b.Language == lang
                    select new PackageCategory()
                    {
                        Name = b.Name,
                        ID = b.ID,
                        Count = tempA
                            .Where(x => !x.CancelDT.HasValue
                                && DbFunctions.TruncateTime(x.StartDT) <= DbFunctions.TruncateTime(DateTime.Now)
                                && (DbFunctions.TruncateTime(x.EndDT) >= DbFunctions.TruncateTime(DateTime.Now) || x.EndDT == null))
                            .Count()
                    }).OrderBy(a => a.Name).ToList();

            return packageCategories;
        }


        protected string SetSessionSearchCriteria(PackageSearchCriteria criteria)
        {
            var guid = Guid.NewGuid().ToString();
            Session[guid] = criteria;
            return guid;
        }
    }
}