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
        // GET: EN/Blog
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
    }
}