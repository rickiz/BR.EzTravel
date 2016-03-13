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
    public class AdminPackageController : BaseEnController
    {
        public ActionResult Index()
        {
            var packages =
                db.lnkmemberposts
                    .Where(a => a.Language == language.ToString() && !a.CancelDT.HasValue)
                    .OrderByDescending(a => a.ID)
                    .Take(20)
                    .Select(a => new PackageIndexItem
                    {
                        ID = a.ID,
                        Title = a.Title,
                        LastUpdateDT = a.UpdateDT.HasValue ? a.UpdateDT.Value : a.CreateDT
                    })
                    .ToList();
            var viewModel = new PackageIndexViewModel { Packages = packages };

            return View(viewModel);
        }

        public ActionResult Create()
        {
            var viewModel = new PackageCreateViewModel
            {
                Categories = GetList(ListType.Category),
                Countries = GetList(ListType.Country)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase[] files, PackageCreateViewModel viewModel)
        {
            var post = new lnkmemberpost()
            {
                Description = viewModel.Description,
                CreateDT = DateTime.Now,
                Language = language.ToString(),
                MemberID = 1,
                PublishDT = DateTime.Now,
                Title = viewModel.Title,
                CategoryID = viewModel.CategoryID,
                Price = viewModel.Price,
                StartDT = DateTime.Now,
            };

            if (!files.IsEmpty())
            {
                var fileName = FIleUploadManager.UploadAndSave(files[0]);
                post.ThumbnailImagePath = fileName;
            }

            db.lnkmemberposts.Add(post);
            db.SaveChanges();

            var country = new lnkmemberpostcountry
            {
                Active = true,
                CountryID = viewModel.CountryID,
                CreateDT = DateTime.Now,
                MemberPostID = post.ID
            };
            db.lnkmemberpostcountries.Add(country);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var package = db.lnkmemberposts.Single(a => a.ID == id);
            var country = db.lnkmemberpostcountries.First(a => a.MemberPostID == package.ID && a.Active);
            var viewModel = new PackageEditViewModel
            {
                ID = package.ID,
                Title = package.Title,
                Description = package.Description,
                Price = package.Price,
                CountryID = country.ID,
                CategoryID = package.CategoryID,
                Categories = GetList(ListType.Category),
                Countries = GetList(ListType.Country),
                ThumbnailImagePath = Path.Combine(Settings.Default.ImageUploadPath, package.ThumbnailImagePath)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HttpPostedFileBase[] files, PackageEditViewModel viewModel)
        {
            var package = db.lnkmemberposts.Single(a => a.ID == viewModel.ID);
            package.Title = viewModel.Title;
            package.Description = viewModel.Description;
            package.UpdateDT = DateTime.Now;
            package.Price = viewModel.Price;
            package.CategoryID = viewModel.CategoryID;

            if(!files.IsEmpty())
            {
                var fileName = FIleUploadManager.UploadAndSave(files[0]);
                package.ThumbnailImagePath = fileName;
            }

            var country = db.lnkmemberpostcountries.First(a => a.MemberPostID == viewModel.ID && a.Active);
            country.CountryID = viewModel.CountryID;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var record = db.lnkmemberposts.Single(a => a.ID == id);
            record.CancelDT = DateTime.Now;
            record.UpdateDT = DateTime.Now;
            //blog.MemberID = 1;

            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}