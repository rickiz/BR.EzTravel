﻿using System;
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
            };

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
    }
}