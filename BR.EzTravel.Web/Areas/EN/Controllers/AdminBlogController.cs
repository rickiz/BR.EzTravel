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
                    .Where(a => a.Language == language.ToString() && !a.CancelDT.HasValue)
                    .OrderByDescending(a => a.ID)
                    .Take(20)
                    .Select(a => new AdminBlogIndexItem
                    {
                        ID = a.ID,
                        Title = a.Title,
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
                Language = language.ToString(),
                MemberID = 1,
                PublishDT = DateTime.Now,
                Title = viewModel.Title,
            };

            if (file != null)
            {
                var fileName = FIleUploadManager.UploadAndSave(file);
                blog.ThumbnailImagePath = fileName;
            }

            db.trnblogs.Add(blog);
            db.SaveChanges();

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
                Categories = GetList(ListType.Category)
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

            if (file != null)
            {
                var fileName = FIleUploadManager.UploadAndSave(file);
                blog.ThumbnailImagePath = fileName;
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var blog = db.trnblogs.Single(a => a.ID == id);
            blog.CancelDT = DateTime.Now;
            blog.UpdateDT = DateTime.Now;
            //blog.MemberID = 1;

            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}