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
                    .Where(a => !a.CancelDT.HasValue && a.CategoryID == viewModel.Criteria.CategoryID && a.Language == lang)
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
                     Rate = a.NoOfReviews == 0 ? 0 : a.Rate / a.NoOfReviews,
                     ReviewCount = a.NoOfReviews,
                     StartDT = a.StartDT,
                     ThumbnailImagePath = a.ThumbnailImagePath,
                     Title = a.Title,
                     Days = a.Days,
                     Nights = a.Nights
                 }).Single();

            viewModel.Comments =
                (from a in db.lnkmemberpostcomments
                 join b in db.tblmembers on a.MemberID equals b.ID into tempb
                 from bb in tempb.DefaultIfEmpty()
                 where a.MemberPostID == id && !a.CancelDT.HasValue
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
                 join b in db.refcategories on a.CategoryID equals b.ID
                 where a.Language == lang && a.ID != id
                 select new PopularPackage
                 {
                     ID = a.ID,
                     Price = a.Price,
                     ThumbnailImagePath = a.ThumbnailImagePath,
                     Title = a.Title,
                     Days = a.Days,
                     Nights = a.Nights,
                     Rate = a.NoOfReviews == 0 ? 0 : a.Rate / a.NoOfReviews
                 })
                .OrderByDescending(a => a.Rate)
                .Take(5)
                .ToList();

            viewModel.RecommendedPackages =
                        (from a in db.lnkmemberposts
                         join b in db.refcategories on a.CategoryID equals b.ID
                         where a.Language == lang && a.ID != id
                         select new RecommendedPackage
                         {
                             ID = a.ID,
                             Price = a.Price,
                             ThumbnailImagePath = a.ThumbnailImagePath,
                             Title = a.Title,
                             Days = a.Days,
                             Nights = a.Nights,
                             Rate = a.NoOfReviews == 0 ? 0 : a.Rate / a.NoOfReviews
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

        [HttpPost]
        public ActionResult Details(PostComment commentPost)
        {
            var memberComment = new lnkmemberpostcomment
            {
                Comments = commentPost.Comment,
                CreateDT = DateTime.Now,
                Language = lang,
                MemberID = 0, // TODO: Link up member
                MemberPostID = commentPost.ID,
            };
            db.lnkmemberpostcomments.Add(memberComment);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = commentPost.ID });
        }
    }
}