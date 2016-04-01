using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BR.EzTravel.Web.Models;
using BR.EzTravel.Web.Helpers;
using System.IO;
using BR.EzTravel.Web.Properties;

namespace BR.EzTravel.Web.Areas.EN.Controllers
{
    public class PackageController : BaseEnController
    {
        public ActionResult Index()
        {
            var viewModel = new PackageIndexViewModel
            {
                Criteria = new PackageSearchCriteria(),
                PackageActivities = GetList(ListType.PackageActivity)
            };

            viewModel.PackageActivities.RemoveAt(0);

            var lang = language.ToString();
            var packageCategories =
                    (from a in db.lnkmemberposts
                     join b in db.refcategories on a.CategoryID equals b.ID
                     where a.Language == lang && !a.CancelDT.HasValue && b.Active
                     group b by new { Name = b.Name, ID = b.ID } into c
                     select new PackageCategory()
                     {
                         Name = c.Key.Name,
                         ID = c.Key.ID,
                         Count = c.Count()
                     }).ToList();
            viewModel.Categories = packageCategories;

            if (!viewModel.Categories.IsEmpty())
                viewModel.Criteria.CategoryID = viewModel.Categories[0].ID.ToInt();

            viewModel.SearchResults =
                db.lnkmemberposts
                    .Where(a => !a.CancelDT.HasValue && a.CategoryID == viewModel.Criteria.CategoryID)
                    .Select(a => new PackageDetails
                    {
                        Description = a.Description,
                        ID = a.ID,
                        Price = a.Price,
                        Rate = a.Rate,
                        ThumbnailImagePath = a.ThumbnailImagePath,
                        Title = a.Title
                    })
                    .Take(Settings.Default.MaxListPerPage)
                    .ToList();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(PackageIndexViewModel viewModel)
        {
            return View(viewModel);
        }

        public ActionResult Details(int id)
        {
            var viewModel =
                (from a in db.lnkmemberposts
                 join b in db.refcategories on a.CategoryID equals b.ID
                 where a.ID == id
                 select new PackageDetailsViewModel
                 {
                     ID = a.ID,
                     Category = b.Name,
                     Description = a.Description,
                     EndDT = a.EndDT,
                     Price = a.Price,
                     Rate = a.Rate, // TODO: add new column for total rate count as reviews count
                     StartDT = a.StartDT,
                     ThumbnailImagePath = a.ThumbnailImagePath,
                     Title = a.Title,
                     Days = a.Days,
                     Nights = a.Nights
                 }).Single();


            viewModel.PopularPackages =
                (from a in db.lnkmemberposts
                 join b in db.refcategories on a.CategoryID equals b.ID
                 where a.ID == id
                 select new PopularPackage
                 {
                     ID = a.ID,
                     Price = a.Price,
                     ThumbnailImagePath = a.ThumbnailImagePath,
                     Title = a.Title,
                     Days = a.Days,
                     Nights = a.Nights,
                     Rate = a.Rate
                 })
                    .OrderByDescending(a => a.Rate)
                    .Take(5)
                    .ToList();

            return View(viewModel);
        }
    }
}