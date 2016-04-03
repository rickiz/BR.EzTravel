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
using System.Transactions;

namespace BR.EzTravel.Web.Areas.EN.Controllers
{
    public class AdminPackageController : BaseEnController
    {
        public ActionResult Index()
        {
            var packages =
                db.lnkmemberposts
                    .Where(a => a.Language == lang && !a.CancelDT.HasValue)
                    .OrderByDescending(a => a.ID)
                    .Take(20)
                    .Select(a => new PackageIndexItem
                    {
                        ID = a.ID,
                        Title = a.Title,
                        LastUpdateDT = a.UpdateDT.HasValue ? a.UpdateDT.Value : a.CreateDT
                    })
                    .ToList();
            var viewModel = new AdminPackageIndexViewModel { Packages = packages };

            return View(viewModel);
        }

        public ActionResult Create()
        {
            var viewModel = new PackageCreateViewModel
            {
                Categories = GetList(ListType.Category),
                Countries = GetList(ListType.Country),
                Activities = GetPackageActivities()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase file, PackageCreateViewModel viewModel)
        {
            using (var trans = new TransactionScope())
            {
                var post = new lnkmemberpost()
                {
                    Description = viewModel.Description,
                    CreateDT = DateTime.Now,
                    Language = lang,
                    MemberID = 1,
                    PublishDT = DateTime.Now,
                    Title = viewModel.Title,
                    CategoryID = viewModel.CategoryID,
                    Price = viewModel.Price,
                    StartDT = viewModel.StartDT,
                    EndDT = viewModel.EndDT,
                    Days = viewModel.Days,
                    Nights = viewModel.Nights,
                };

                if (file != null)
                {
                    var fileName = FIleUploadManager.UploadAndSave(file);
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

                foreach (var activityID in viewModel.SelectedActivities)
                {
                    var activity = new lnkmemberpostpackageactivity
                    {
                        Active = true,
                        CreateDT = DateTime.Now,
                        MemberPostID = post.ID,
                        PackageActivityID = activityID
                    };
                    db.lnkmemberpostpackageactivities.Add(activity);
                }
                db.SaveChanges();

                trans.Complete();
            }

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
                CountryID = country.CountryID,
                CategoryID = package.CategoryID,
                Categories = GetList(ListType.Category),
                Countries = GetList(ListType.Country),
                ThumbnailImagePath = string.IsNullOrEmpty(package.ThumbnailImagePath) ?
                                        "" : Path.Combine(Settings.Default.ImageUploadPath, package.ThumbnailImagePath),
                Days = package.Days,
                Nights = package.Nights,
                StartDT = package.StartDT,
                EndDT = package.EndDT
            };

            var selectedActivities = 
                db.lnkmemberpostpackageactivities.Where(a => a.MemberPostID == id && a.Active).Select(a => a.PackageActivityID).ToArray();

            viewModel.Activities = GetPackageActivities(selectedActivities);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HttpPostedFileBase file, PackageEditViewModel viewModel)
        {
            using (var trans = new TransactionScope())
            {
                var package = db.lnkmemberposts.Single(a => a.ID == viewModel.ID);
                package.Title = viewModel.Title;
                package.Description = viewModel.Description;
                package.UpdateDT = DateTime.Now;
                package.Price = viewModel.Price;
                package.CategoryID = viewModel.CategoryID;
                package.StartDT = viewModel.StartDT;
                package.EndDT = viewModel.EndDT;
                package.Days = viewModel.Days;
                package.Nights = viewModel.Nights;

                if (file != null)
                {
                    var fileName = FIleUploadManager.UploadAndSave(file);
                    package.ThumbnailImagePath = fileName;
                }

                var country = db.lnkmemberpostcountries.First(a => a.MemberPostID == viewModel.ID && a.Active);
                country.CountryID = viewModel.CountryID;

                var oldActivities = db.lnkmemberpostpackageactivities.Where(a => a.MemberPostID == viewModel.ID && a.Active).ToList();
                oldActivities.ForEach(a => a.Active = false);
                db.SaveChanges();

                foreach (var activityID in viewModel.SelectedActivities)
                {
                    var inactiveAct = db.lnkmemberpostpackageactivities
                        .SingleOrDefault(a => a.PackageActivityID == activityID && a.MemberPostID == viewModel.ID);

                    if (inactiveAct == null)
                    {
                        var activity = new lnkmemberpostpackageactivity
                        {
                            Active = true,
                            CreateDT = DateTime.Now,
                            MemberPostID = viewModel.ID,
                            PackageActivityID = activityID
                        };
                        db.lnkmemberpostpackageactivities.Add(activity);
                    }
                    else
                    {
                        inactiveAct.Active = true;
                    }
                }
                db.SaveChanges();

                trans.Complete();
            }

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