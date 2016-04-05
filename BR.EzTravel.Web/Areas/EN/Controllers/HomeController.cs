using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BR.EzTravel.Web.Models;
using BR.EzTravel.Web.Helpers;
using System.IO;
using BR.EzTravel.Web.Properties;
using System.Data.Entity;

namespace BR.EzTravel.Web.Areas.EN.Controllers
{
    public class HomeController : BaseEnController
    {
        public ActionResult Index()
        {
            var latestPackages =
                db.lnkmemberposts
                    .Where(a => !a.CancelDT.HasValue && a.Language == lang
                     && DbFunctions.TruncateTime(a.StartDT) <= DbFunctions.TruncateTime(DateTime.Now)
                     && (DbFunctions.TruncateTime(a.EndDT) >= DbFunctions.TruncateTime(DateTime.Now) || a.EndDT == null))
                    .Select(a => new PackageDetails
                    {
                        Description = a.Description,
                        ID = a.ID,
                        Price = a.Price,
                        Rate = a.Rate,
                        ThumbnailImagePath = a.ThumbnailImagePath,
                        Title = a.Title
                    })
                    .OrderByDescending(a => a.ID)
                    .Take(16)
                    .ToList();

            var topPackages =
                db.lnkmemberposts
                    .Where(a => !a.CancelDT.HasValue && a.Language == lang
                     && DbFunctions.TruncateTime(a.StartDT) <= DbFunctions.TruncateTime(DateTime.Now)
                     && (DbFunctions.TruncateTime(a.EndDT) >= DbFunctions.TruncateTime(DateTime.Now) || a.EndDT == null))
                    .Select(a => new PackageDetails
                    {
                        Description = a.Description,
                        ID = a.ID,
                        Price = a.Price,
                        Rate = a.Rate,
                        ThumbnailImagePath = a.ThumbnailImagePath,
                        Title = a.Title
                    })
                    .OrderByDescending(a => a.Rate)
                    .Take(9)
                    .ToList();

            var viewModel = new HomeViewModel
            {
                LatestPackages = latestPackages,
                PopularPackages = topPackages
            };

            return View(viewModel);
        }
    }
}