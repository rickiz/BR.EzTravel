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
    public class BlogController : BaseEnController
    {
        public ActionResult Index(int categoryID = 0)
        {
            var viewModel = new BlogIndexViewModel();

            viewModel.Blogs =
                db.trnblogs
                    .Where(a => !a.CancelDT.HasValue && a.Language == language.ToString() &&
                    (a.CategoryID == categoryID || categoryID == 0))
                    .Select(a => new BlogDetails
                    {
                        ID = a.ID,
                        CategoryID = a.CategoryID,
                        Title = a.Title,
                        Body = a.Body,
                        LastEditedDate = a.UpdateDT.HasValue ? a.UpdateDT.Value : a.CreateDT,
                        CreatedBy = "Tester", // TODO: Link to tblMember
                        TotalComments = db.lnkblogcomments.Where(b => b.BlogID == a.ID && !b.CancelDT.HasValue).Count()
                    })
                    .Take(Settings.Default.MaxListPerPage)
                    .ToList();

            viewModel.PopularBlogs =
                db.trnblogs
                    .GroupJoin(db.lnkblogcomments, a => a.ID, b => b.BlogID, (a, b) => new { Blog = a, BlogComments = b })
                    .Where(a => !a.Blog.CancelDT.HasValue && a.Blog.Language == language.ToString() &&
                        (a.Blog.CategoryID == categoryID || categoryID == 0))
                    .Select(a => new PopularBlog
                    {
                        ID = a.Blog.ID,
                        Author = "Tester", // TODO: Link to tblMember
                        CreateDT = a.Blog.CreateDT,
                        NoOfComments = a.BlogComments.Count(),
                        Title = a.Blog.Title
                    })
                    .OrderByDescending(a => a.NoOfComments)
                    .Take(5)
                    .ToList();

            viewModel.LatestBlogComments =
                db.trnblogs
                    .Join(db.lnkblogcomments, a => a.ID, b => b.BlogID, (a, b) => new { Blog = a, BlogComments = b })
                    .Where(a => !a.Blog.CancelDT.HasValue && a.Blog.Language == language.ToString() &&
                        (a.Blog.CategoryID == categoryID || categoryID == 0))
                    .Select(a => new LatestBlogComment
                    {
                        ID = a.Blog.ID,
                        Author = "Tester", // TODO: Link to tblMember
                        CreateDT = a.BlogComments.CreateDT,
                        Comment = a.BlogComments.Comments,
                        Title = a.Blog.Title
                    })
                    .OrderByDescending(a => a.CreateDT)
                    .Take(5)
                    .ToList();

            viewModel.Categories = db.refcategories.Where(a => a.Active).OrderBy(a => a.Name).ToList();

            return View(viewModel);
        }

        public ActionResult Details(int id)
        {
            var viewModel =
                (from a in db.trnblogs
                 join b in db.tblmembers on a.MemberID equals b.ID into tempB
                 from bb in tempB.DefaultIfEmpty()
                 where a.ID == id
                 select new BlogDetailsViewModel
                 {
                     ID = a.ID,
                     CategoryID = a.CategoryID,
                     Title = a.Title,
                     Body = a.Body,
                     LastEditedDate = a.UpdateDT.HasValue ? a.UpdateDT.Value : a.CreateDT,
                     CreatedBy = bb == null ? "EMMA STONE" : bb.PICName,
                     TotalComments = db.lnkblogcomments.Where(b => b.BlogID == a.ID && !b.CancelDT.HasValue).Count(),
                     ThumbnailImagePath = a.ThumbnailImagePath
                 }).Single();

            var comments =
                (from a in db.lnkblogcomments
                 join b in db.tblmembers on a.MemberID equals b.ID into tempb
                 from bb in tempb.DefaultIfEmpty()
                 where a.BlogID == id && !a.CancelDT.HasValue
                 select new LatestBlogComment
                 {
                     Author = bb == null ? "" : bb.PICName,
                     Comment = a.Comments,
                     CreateDT = a.CreateDT,
                     ID = a.ID
                 }).ToList();

            var lang = language.ToString();
            var categories =
                    (from a in db.trnblogs
                     join b in db.refcategories on a.CategoryID equals b.ID
                     where a.Language == lang && !a.CancelDT.HasValue && b.Active
                     group b by new { Name = b.Name, ID = b.ID } into c
                     select new BlogCategory()
                     {
                         Name = c.Key.Name,
                         ID = c.Key.ID,
                         Count = c.Count()
                     }).ToList();

            viewModel.Comments = comments;
            viewModel.Categories = categories;

            return View(viewModel);
        }
    }
}