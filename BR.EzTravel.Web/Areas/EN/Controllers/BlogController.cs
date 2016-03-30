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
                        Title = a.Title,
                        Body = a.Body,
                        LastEditedDate = a.UpdateDT.HasValue ? a.UpdateDT.Value : a.CreateDT,
                        CreatedBy = "Tester", // TODO: Link to tblMember
                        TotalComments = db.lnkblogcomments.Where(b => b.BlogID == a.ID && !b.CancelDT.HasValue).Count()
                    })
                    .Take(Settings.Default.MaxListPerPage)
                    .ToList();

            viewModel.Categories = db.refcategories.Where(a => a.Active).OrderBy(a => a.Name).ToList();

            return View(viewModel);
        }
    }
}