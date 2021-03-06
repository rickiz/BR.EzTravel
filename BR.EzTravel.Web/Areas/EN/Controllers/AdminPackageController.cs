﻿using System;
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
    [Authorize(Roles = "AG")]
    public class AdminPackageController : BaseEnController
    {
        public ActionResult Index()
        {
            var query =
                db.lnkmemberposts
                    .Where(a => a.Language == lang && !a.CancelDT.HasValue)
                    .OrderByDescending(a => a.ID)
                    .Take(20)
                    .Select(a => new PackageIndexItem
                    {
                        ID = a.ID,
                        Title = a.Title,
                        Active = a.Active,
                        MemberID = a.MemberID,
                        LastUpdateDT = a.UpdateDT.HasValue ? a.UpdateDT.Value : a.CreateDT
                    });

            if (!User.IsInRole("SA"))
                query = query.Where(a => a.MemberID == Util.SessionAccess.ID);

            var packages = query.ToList();

            var viewModel = new AdminPackageIndexViewModel { Packages = packages };

            return View(viewModel);
        }

        public ActionResult Create()
        {
            var viewModel = new PackageCreateViewModel();

            viewModel.Categories = GetList(ListType.Category);
            viewModel.Countries = GetList(ListType.Country, defaultItem: false);
            viewModel.Activities = GetPackageActivities();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase file, PackageCreateViewModel viewModel)
        {
            if (!ModelState.IsValid || file == null)
            {
                if (file == null)
                {
                    ModelState.AddModelError("", "Thumbnail is required.");
                }

                viewModel.Categories = GetList(ListType.Category);
                viewModel.Countries = GetList(ListType.Country, defaultItem: false);
                viewModel.Activities = GetPackageActivities();
                viewModel.DetailImageNames = "";

                return View(viewModel);
            }

            var postID = 0;

            var isAvailable = CheckAvailablePost(Util.SessionAccess.ID);

            using (var trans = new TransactionScope())
            {
                var post = new lnkmemberpost()
                {
                    Description = viewModel.Description,
                    CreateDT = DateTime.Now,
                    Language = lang,
                    MemberID = Util.SessionAccess.ID,
                    PublishDT = DateTime.Now,
                    Title = viewModel.Title,
                    CategoryID = viewModel.CategoryID,
                    Price = viewModel.Price,
                    StartDT = viewModel.StartDT,
                    EndDT = viewModel.EndDT,
                    Days = viewModel.Days,
                    Nights = viewModel.Nights,
                    Active = isAvailable,
                };

                if (file != null)
                {
                    var fileName = FileUploadManager.UploadAndSave(file);
                    post.ThumbnailImagePath = fileName;
                }

                db.lnkmemberposts.Add(post);
                db.SaveChanges();

                if (!viewModel.CountryIDs.IsEmpty())
                {
                    var memberCountries =
                        viewModel.CountryIDs
                            .Select(a =>
                                new lnkmemberpostcountry
                                {
                                    Active = true,
                                    CountryID = a,
                                    CreateDT = DateTime.Now,
                                    MemberPostID = post.ID
                                })
                            .ToArray();

                    db.lnkmemberpostcountries.AddRange(memberCountries);
                }

                if (!viewModel.SelectedActivities.IsEmpty())
                {
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
                }

                if (!string.IsNullOrEmpty(viewModel.DetailImageNames))
                {
                    var detailImages = viewModel.DetailImageNames.Split(',');

                    foreach (var image in detailImages)
                    {
                        var linkImage = new lnkmemberpostimage
                        {
                            Active = true,
                            CreateDT = DateTime.Now,
                            ImagePath = image,
                            MemberPostID = post.ID,
                        };
                        db.lnkmemberpostimages.Add(linkImage);
                    }
                    db.SaveChanges();
                }

                trans.Complete();

                postID = post.ID;
            }

            var emailBody = string.Format(@"Hi EZ Management, <br /><br />
                    Package <b>{0}</b> has been created. <br /><br />
                    
                    http://ezgoholiday.com/EN/Package/Details/{1} <br /><br />
                    Posted: {2}",
                    viewModel.Title, postID, isAvailable);

            Util.SendEmail(viewModel.Title, emailBody, Properties.Settings.Default.EmailFrom, "", "");


            if (viewModel.Active && !isAvailable)
                return RedirectToAction("Edit", new
                {
                    id = postID,
                    errorMessage = "Insufficient available Post.\nPost created but failed to be published to website."
                });
            else
                return RedirectToAction("Index");
        }

        private bool CheckAvailablePost(int memberID)
        {
            var availablePost = db.tblmembers.Where(a => a.ID == memberID).Single().AvailablePost;
            var postedQty = db.lnkmemberposts.Where(a => a.Active && a.MemberID == memberID).Count();

            if (availablePost <= postedQty)
                return false;
            else
                return true;
        }

        public ActionResult Edit(int id, string errorMessage = "")
        {
            var package = db.lnkmemberposts.Single(a => a.ID == id);            
            var viewModel = new PackageEditViewModel
            {
                ID = package.ID,
                Title = package.Title,
                Description = package.Description,
                Price = package.Price,
                CategoryID = package.CategoryID,
                Categories = GetList(ListType.Category),
                ThumbnailImagePath = package.ThumbnailImagePath,
                Days = package.Days,
                Nights = package.Nights,
                StartDT = package.StartDT,
                EndDT = package.EndDT,
                Active = package.Active,
                ErrorMessage = errorMessage
            };

            var selectedActivities = 
                db.lnkmemberpostpackageactivities.Where(a => a.MemberPostID == id && a.Active).Select(a => a.PackageActivityID).ToArray();

            viewModel.Activities = GetPackageActivities(selectedActivities);

            var selectedCountryIDs =
                db.lnkmemberpostcountries.Where(a => a.MemberPostID == id && a.Active).Select(a => a.CountryID).ToArray();
            var countries = GetList(ListType.Country, defaultItem: false);
            countries.ForEach(a => a.Selected = selectedCountryIDs.Any(b => b == int.Parse(a.Value)));

            viewModel.Countries = countries;

            var detailImageNames = 
                db.lnkmemberpostimages.Where(a => a.Active && a.MemberPostID == id).Select(a => a.ImagePath).ToArray();

            viewModel.DetailImageNames = string.Join(",", detailImageNames);
            viewModel.MockFiles = new List<PackageEditMockFile>();

            foreach (var imageName in detailImageNames)
            {
                var mockFile = new PackageEditMockFile
                {
                    name = imageName,
                    size = FileUploadManager.GetFileSize(imageName)
                };
                viewModel.MockFiles.Add(mockFile);
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HttpPostedFileBase file, PackageEditViewModel viewModel)
        {
            var isAvailable = CheckAvailablePost(Util.SessionAccess.ID);

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
                package.Active = viewModel.Active? isAvailable: viewModel.Active;

                if (file != null)
                {
                    var fileName = FileUploadManager.UploadAndSave(file);
                    package.ThumbnailImagePath = fileName;
                }

                var oldCountries = db.lnkmemberpostcountries.Where(a => a.MemberPostID == viewModel.ID && a.Active).ToList();
                oldCountries.ForEach(a => a.Active = false);
                db.SaveChanges();

                if (!viewModel.CountryIDs.IsEmpty())
                {
                    foreach (var countryID in viewModel.CountryIDs)
                    {
                        var inactiveAct = db.lnkmemberpostcountries
                            .SingleOrDefault(a => a.CountryID == countryID && a.MemberPostID == viewModel.ID);

                        if (inactiveAct == null)
                        {
                            var country = new lnkmemberpostcountry
                            {
                                Active = true,
                                CreateDT = DateTime.Now,
                                MemberPostID = viewModel.ID,
                                CountryID = countryID
                            };
                            db.lnkmemberpostcountries.Add(country);
                        }
                        else
                        {
                            inactiveAct.Active = true;
                        }
                    }
                    db.SaveChanges();
                }

                var oldActivities = db.lnkmemberpostpackageactivities.Where(a => a.MemberPostID == viewModel.ID && a.Active).ToList();
                oldActivities.ForEach(a => a.Active = false);
                db.SaveChanges();

                if(!viewModel.SelectedActivities.IsEmpty())
                {
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
                }

                var existingDetailImages = db.lnkmemberpostimages.Where(a => a.Active && a.MemberPostID == viewModel.ID).ToList();
                var detailImages = viewModel.DetailImageNames?.Split(',').ToList() ?? new List<string>();

                foreach (var existingImage in existingDetailImages)
                {
                    if(detailImages.Contains(existingImage.ImagePath))
                    {
                        detailImages.Remove(existingImage.ImagePath);
                    }
                    else
                    {
                        existingImage.Active = false;
                        existingImage.UpdateDT = DateTime.Now;
                    }
                }

                foreach (var image in detailImages)
                {
                    var linkImage = new lnkmemberpostimage
                    {
                        Active = true,
                        CreateDT = DateTime.Now,
                        ImagePath = image,
                        MemberPostID = viewModel.ID,
                    };
                    db.lnkmemberpostimages.Add(linkImage);
                }
                db.SaveChanges();

                trans.Complete();
            }

            var emailBody = string.Format(@"Hi EZ Management, <br /><br />
                    Package <b>{0}</b> has been created. <br /><br />
                    
                    http://ezgoholiday.com/EN/Package/Details/{1}",
                    viewModel.Title, viewModel.ID);

            Util.SendEmail(viewModel.Title, emailBody, Properties.Settings.Default.EmailFrom, "", "");

            if (viewModel.Active && !isAvailable)
                return RedirectToAction("Edit", new
                {
                    id = viewModel.ID,
                    errorMessage = "Insufficient available Post.\nPost updated but failed to be published to website."
                });
            else
                return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var record = db.lnkmemberposts.Single(a => a.ID == id);
            //record.CancelDT = DateTime.Now;
            record.UpdateDT = DateTime.Now;
            record.Active = false;
            //blog.MemberID = 1;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UploadImages()
        {
            var finalFileNames = new List<string>();

            try
            {
                foreach (string fileName in Request.Files)
                {
                    var image = Request.Files[fileName];
                    var newFileName = FileUploadManager.UploadAndSave(image);

                    finalFileNames.Add(newFileName);
                }

                return new JsonResult
                {
                    Data = new { FileNames = finalFileNames }
                };
            }
            catch (Exception ex)
            {
                ex.Log();

                finalFileNames.ForEach(a => FileUploadManager.DeleteFile(a));

                return new JsonResult
                {
                    Data = new { Error = ex.Message }
                };
            }

            
        }

        [HttpPost]
        public ActionResult DeleteFile(string fileName)
        {
            FileUploadManager.DeleteFile(fileName);

            return new JsonResult();
        }
    }
}