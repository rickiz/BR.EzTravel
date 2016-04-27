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
            //System.Web.Security.FormsAuthentication.SignOut();
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

            var latestBlogs = db.trnblogs
                    .GroupJoin(db.tblmembers, a => a.MemberID, b => b.ID, (a, b) => new { Blog = a, Member = b.FirstOrDefault(), })
                    .Where(a => !a.Blog.CancelDT.HasValue && a.Blog.Language == lang && a.Blog.Active)
                    .Select(a => new BlogDetails
                    {
                        ID = a.Blog.ID,
                        CategoryID = a.Blog.CategoryID,
                        Title = a.Blog.Title,
                        Body = a.Blog.Body,
                        LastEditedDate = a.Blog.UpdateDT.HasValue ? a.Blog.UpdateDT.Value : a.Blog.CreateDT,
                        CreatedBy = a.Member == null ? "EMMA STONE" : a.Member.PICName,
                        TotalComments = db.lnkblogcomments.Where(b => b.BlogID == a.Blog.ID && !b.CancelDT.HasValue).Count(),
                        ThumbnailImagePath = a.Blog.ThumbnailImagePath
                    })
                    .Take(4)
                    .ToList();

            var viewModel = new HomeViewModel
            {
                LatestPackages = latestPackages,
                PopularPackages = topPackages,
                LatestBlogs = latestBlogs,
                Countries = GetList(ListType.PackageCountry, defaultItem: false),
                PackageActivities = GetList(ListType.PackageActivity, defaultItem: false),
                Categories = GetList(ListType.Category, defaultItem: false),
                Criteria = new PackageSearchCriteria
                {
                    PriceFrom = "0",
                    PriceTo = "9999",
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(HomeViewModel viewModel)
        {
            if (viewModel.Criteria.Rates == null)
                viewModel.Criteria.Rates = new int[] { };

            if (viewModel.Criteria.PackageActivityIDs == null)
                viewModel.Criteria.PackageActivityIDs = new int[] { };

            viewModel.Criteria.PackageActivityIDs = viewModel.Criteria.PackageActivityIDs.Where(a => a > 0).ToArray();

            Session.Remove(OldSearchGuid);
            var sid = SetSessionSearchCriteria(viewModel.Criteria);
            OldSearchGuid = sid;

            return RedirectToAction("Index", "Package", new { sid = sid });
        }
    }
}