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
    public class PackageController : BaseEnController
    {
        

        private PackageSearchCriteria GetSessionSearchCriteria(string guid)
        {
            return Session[guid] as PackageSearchCriteria;
        }
        
        private List<PackageDetails> SeacrhPackages(PackageSearchCriteria criteria)
        {
            var query =
                db.lnkmemberposts
                    .Where(a => a.Active
                        && a.Language == lang
                        && !a.CancelDT.HasValue
                        && DbFunctions.TruncateTime(a.StartDT) <= DbFunctions.TruncateTime(DateTime.Now)
                        && (DbFunctions.TruncateTime(a.EndDT) >= DbFunctions.TruncateTime(DateTime.Now) || a.EndDT == null));

            if (criteria.CategoryID > 0)
                query = query.Where(a => a.CategoryID == criteria.CategoryID);

            if (!criteria.PackageActivityIDs.IsEmpty())
            {
                query = query.Where(a =>
                    db.lnkmemberpostpackageactivities
                        .Any(b => criteria.PackageActivityIDs.Contains(b.PackageActivityID)
                                && b.MemberPostID == a.ID
                                && b.Active));
            }

            if (criteria.CountryID > 0)
            {
                query = query.Where(a =>
                    db.lnkmemberpostcountries
                        .Any(b => b.CountryID == criteria.CountryID
                                && b.MemberPostID == a.ID
                                && b.Active));
            }

            if (!criteria.PriceFrom.IsStringEmpty())
            {
                var priceFrom = Util.ConvertPriceSearch(criteria.PriceFrom);
                query = query.Where(a => a.Price >= priceFrom);
            }

            if (!criteria.PriceTo.IsStringEmpty())
            {
                var priceTo = Util.ConvertPriceSearch(criteria.PriceTo);
                query = query.Where(a => a.Price <= priceTo);
            }

            if (!criteria.Rates.IsEmpty())
            {
                query = query.Where(a => criteria.Rates.Contains(a.LatestRate));
            }

            var pageSize = Settings.Default.MaxListPerPage;
            var rowsCount = query.Count();

            criteria.TotalPage = rowsCount / pageSize;
            if (rowsCount % pageSize > 0)
                criteria.TotalPage++;

            //If page number should be > 0 else set to first page
            if (rowsCount <= pageSize || criteria.PageNum <= 0) criteria.PageNum = 1;

            //Calculate nunber of rows to skip on pagesize
            int excludedRows = (criteria.PageNum - 1) * pageSize;

            switch (criteria.SortOrder)
            {
                case PackageIndexSort.None:
                    query = query.OrderByDescending(a => a.ID);
                    break;
                case PackageIndexSort.Price_Asc:
                    query = query.OrderBy(a => a.Price);
                    break;
                case PackageIndexSort.Price_Desc:
                    query = query.OrderByDescending(a => a.Price);
                    break;
                case PackageIndexSort.Ranking_Asc:
                    query = query.OrderBy(a => a.LatestRate);
                    break;
                case PackageIndexSort.Ranking_Desc:
                    query = query.OrderByDescending(a => a.LatestRate);
                    break;
                default:
                    break;
            }

            var results =
                query.Select(a =>
                    new PackageDetails
                    {
                        Description = a.Description,
                        ID = a.ID,
                        Price = a.Price,
                        Rate = a.LatestRate,
                        ThumbnailImagePath = a.ThumbnailImagePath,
                        Title = a.Title,
                        ReviewCount = a.NoOfReviews
                    })
                    .Skip(excludedRows)
                    .Take(pageSize)
                    .ToList();

            return results;
        }

        public ActionResult Index(int categoryID = 0, PackageIndexSort sort = PackageIndexSort.None, int p = 0, string sid = "")
        {
            var yesterdayDT = DateTime.Now.AddDays(-1);

            var viewModel = new PackageIndexViewModel
            {
                Criteria = new PackageSearchCriteria
                {
                    CategoryID = categoryID, Rates = new int[] { }, PackageActivityIDs = new int[] { },
                    SortOrder = sort, PageNum = p
                },
                PackageActivities = GetPackageActivities(),
                Categories = GetPackageCategories(),
                Countries = GetList(ListType.PackageCountry, defaultText: "All")
            };

            // Continue with previous search criteria
            if (!sid.IsStringEmpty())
                viewModel.Criteria = GetSessionSearchCriteria(sid);

            var newSearchGUID = false;

            if(categoryID > 0)
            {
                viewModel.Criteria.CategoryID = categoryID;
                newSearchGUID = true;
            }
            if (sort != PackageIndexSort.None)
            {
                viewModel.Criteria.SortOrder = sort;
                newSearchGUID = true;
            }
            if (p > 0)
            {
                viewModel.Criteria.PageNum = p;
                newSearchGUID = true;
            }

            if (newSearchGUID)
            {
                Session.Remove(OldSearchGuid);
                sid = SetSessionSearchCriteria(viewModel.Criteria);
                OldSearchGuid = sid;
            }

            viewModel.SearchGuid = sid;
            viewModel.SearchResults = SeacrhPackages(viewModel.Criteria);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(PackageIndexViewModel viewModel)
        {
            viewModel.SearchResults = SeacrhPackages(viewModel.Criteria);
            viewModel.PackageActivities = GetList(ListType.PackageActivity, defaultItem: false);
            viewModel.Categories = GetPackageCategories();
            viewModel.Countries = GetList(ListType.PackageCountry , defaultText: "All");

            if (viewModel.Criteria.Rates == null)
                viewModel.Criteria.Rates = new int[] { };

            if (viewModel.Criteria.PackageActivityIDs == null)
                viewModel.Criteria.PackageActivityIDs = new int[] { };

            Session.Remove(OldSearchGuid);
            viewModel.SearchGuid = SetSessionSearchCriteria(viewModel.Criteria);
            OldSearchGuid = viewModel.SearchGuid;

            return View(viewModel);
        }

        public ActionResult Details(int id)
        {
            var yesterdayDT = DateTime.Now.AddDays(-1);

            var viewModel =
                (from a in db.lnkmemberposts
                 join b in db.refcategories on a.CategoryID equals b.ID
                 join c in db.tblmembers on a.MemberID equals c.ID
                 where a.ID == id && a.Active
                     && DbFunctions.TruncateTime(a.StartDT) <= DbFunctions.TruncateTime(DateTime.Now)
                     && (DbFunctions.TruncateTime(a.EndDT) >= DbFunctions.TruncateTime(DateTime.Now) || a.EndDT == null)
                 select new PackageDetailsViewModel
                 {
                     ID = a.ID,
                     Category = b.Name,
                     Description = a.Description,
                     EndDT = a.EndDT,
                     Price = a.Price,
                     Rate = a.LatestRate,
                     ReviewCount = a.NoOfReviews,
                     StartDT = a.StartDT,
                     ThumbnailImagePath = a.ThumbnailImagePath,
                     Title = a.Title,
                     Days = a.Days,
                     Nights = a.Nights,
                     PackageAgency = c
                 }).Single();

            viewModel.Comments =
                (from a in db.lnkmemberpostcomments
                 join b in db.tblmembers on a.MemberID equals b.ID into tempb
                 from bb in tempb.DefaultIfEmpty()
                 join c in db.lnkmemberposts on a.MemberPostID equals c.ID 
                 where a.MemberPostID == id && !a.CancelDT.HasValue && c.Active
                     && DbFunctions.TruncateTime(c.StartDT) <= DbFunctions.TruncateTime(DateTime.Now)
                     && (DbFunctions.TruncateTime(c.EndDT) >= DbFunctions.TruncateTime(DateTime.Now) || c.EndDT == null)
                 select new PackageComment
                 {
                     Author = bb == null ? "Anonymous" : bb.PICName,
                     Comment = a.Comments,
                     CreateDT = a.CreateDT,
                     ID = a.ID
                 })
                 .OrderByDescending(a => a.ID)
                 .Take(20)
                 .ToList();
            viewModel.TotalComments = viewModel.Comments.Count;

            viewModel.PopularPackages =
                (from a in db.lnkmemberposts
                 where a.Language == lang && a.ID != id && a.Active
                     && DbFunctions.TruncateTime(a.StartDT) <= DbFunctions.TruncateTime(DateTime.Now)
                     && (DbFunctions.TruncateTime(a.EndDT) >= DbFunctions.TruncateTime(DateTime.Now) || a.EndDT == null)
                 select new PopularPackage
                 {
                     ID = a.ID,
                     Price = a.Price,
                     ThumbnailImagePath = a.ThumbnailImagePath,
                     Title = a.Title,
                     Days = a.Days,
                     Nights = a.Nights,
                     Rate = a.LatestRate
                 })
                .OrderByDescending(a => a.Rate)
                .Take(5)
                .ToList();

            viewModel.RecommendedPackages =
                        (from a in db.lnkmemberposts
                         where a.Language == lang && a.ID != id && a.Active
                     && DbFunctions.TruncateTime(a.StartDT) <= DbFunctions.TruncateTime(DateTime.Now)
                     && (DbFunctions.TruncateTime(a.EndDT) >= DbFunctions.TruncateTime(DateTime.Now) || a.EndDT == null)
                         group a by new { a.ID, a.Price, a.ThumbnailImagePath, a.Title, a.Days, a.Nights, a.LatestRate, a.NoOfReviews } into aa
                         select new RecommendedPackage
                         {
                             ID = aa.Key.ID,
                             Price = aa.Key.Price,
                             ThumbnailImagePath = aa.Key.ThumbnailImagePath,
                             Title = aa.Key.Title,
                             Days = aa.Key.Days,
                             Nights = aa.Key.Nights,
                             Rate = aa.Key.LatestRate
                         })
                        .OrderBy(a => a.Rate)
                        .Take(10)
                        .ToList();


            viewModel.Images =
                db.lnkmemberpostimages.Where(a => a.Active && a.MemberPostID == id).Select(a => a.ImagePath).ToArray();

            viewModel.Activities =
                (from a in db.lnkmemberpostpackageactivities
                 join b in db.refpackageactivities on a.PackageActivityID equals b.ID
                 where a.Active && a.MemberPostID == id
                 select b.Name).ToArray();

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Details(PostComment commentPost)
        {
            var memberComment = new lnkmemberpostcomment
            {
                Comments = commentPost.Comment,
                CreateDT = DateTime.Now,
                Language = lang,
                MemberID = Util.SessionAccess.ID,
                MemberPostID = commentPost.ID,
            };
            db.lnkmemberpostcomments.Add(memberComment);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = commentPost.ID });
        }

        [HttpPost]
        public ActionResult Rate(int id, int rate)
        {
            var package = db.lnkmemberposts.Single(a => a.ID == id);
            package.NoOfReviews++;
            package.Rate += rate;

            var averageRating = Util.CalculateAverageRating(package.Rate, package.NoOfReviews);

            package.LatestRate = averageRating;

            db.SaveChanges();

            return new JsonResult { Data = averageRating };
        }
    }
}