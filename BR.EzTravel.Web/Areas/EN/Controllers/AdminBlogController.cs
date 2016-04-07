using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BR.EzTravel.Web.Models.Admin;
using BR.EzTravel.Web.Models;
using BR.EzTravel.Web.Helpers;
using BR.EzTravel.Web.Properties;
using System.IO;

namespace BR.EzTravel.Web.Areas.EN.Controllers
{
    public class AdminBlogController : BaseEnController
    {
        public ActionResult Index()
        {
            var blogs =
                db.trnblogs
                    .Where(a => a.Language == lang && !a.CancelDT.HasValue)
                    .OrderByDescending(a => a.ID)
                    .Take(20)
                    .Select(a => new AdminBlogIndexItem
                    {
                        ID = a.ID,
                        Title = a.Title,
                        Active = a.Active,
                        LastUpdateDT = a.UpdateDT.HasValue ? a.UpdateDT.Value : a.CreateDT
                    })
                    .ToList();
            var viewModel = new AdminBlogIndexViewModel { Blogs = blogs };

            return View(viewModel);
        }

        public ActionResult Create()
        {
            return View(new BlogCreateViewModel()
            {
                Categories = GetList(ListType.Category)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase file, BlogCreateViewModel viewModel)
        {
            var blog = new trnblog()
            {
                Body = viewModel.Body,
                CreateDT = DateTime.Now,
                CategoryID = viewModel.CategoryID,
                Language = lang,
                MemberID = 1,
                PublishDT = DateTime.Now,
                Title = viewModel.Title,
                Active = true
            };

            if (file != null)
            {
                var fileName = FileUploadManager.UploadAndSave(file);
                blog.ThumbnailImagePath = fileName;
            }

            db.trnblogs.Add(blog);
            db.SaveChanges();

            var emailBody = string.Format(@"Hi EZ Management, <br /><br />
                    New blog <b>{0}</b> has been created. <br /><br />
                    
                    http://www.ezgoholiday.com/EN/Blog/Details/{1}",
                    viewModel.Title, blog.ID);

            Util.SendEmail("Welcome to EZ Go Holiday!", emailBody, Properties.Settings.Default.EmailFrom, "", "");

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var blog = db.trnblogs.Single(a => a.ID == id);
            var viewModel = new BlogEditViewModel
            {
                ID = blog.ID,
                Title = blog.Title,
                Body = blog.Body,
                CategoryID = blog.CategoryID,
                ThumbnailImagePath = string.IsNullOrEmpty(blog.ThumbnailImagePath) ?
                                        "" : Path.Combine(Settings.Default.ImageUploadPath, blog.ThumbnailImagePath),
                Categories = GetList(ListType.Category),
                Active = blog.Active
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HttpPostedFileBase file, BlogEditViewModel viewModel)
        {
            var blog = db.trnblogs.Single(a => a.ID == viewModel.ID);
            blog.CategoryID = viewModel.CategoryID;
            blog.Title = viewModel.Title;
            blog.Body = viewModel.Body;
            blog.UpdateDT = DateTime.Now;
            blog.Active = viewModel.Active;

            if (file != null)
            {
                var fileName = FileUploadManager.UploadAndSave(file);
                blog.ThumbnailImagePath = fileName;
            }

            db.SaveChanges();

            var emailBody = string.Format(@"Hi EZ Management, <br /><br />
                    Blog <b>{0}</b> has been updated. <br /><br />
                    
                    http://www.ezgoholiday.com/EN/Blog/Details/{1}",
                    viewModel.Title, blog.ID);

            Util.SendEmail("Welcome to EZ Go Holiday!", emailBody, Properties.Settings.Default.EmailFrom, "", "");

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var blog = db.trnblogs.Single(a => a.ID == id);
            blog.Active = false;
            blog.UpdateDT = DateTime.Now;
            //blog.MemberID = 1;

            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}