using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BR.EzTravel.Web.Models.Admin;
using BR.EzTravel.Web.Models;
using BR.EzTravel.Web.Helpers;
using BR.EzTravel.Web.Properties;

namespace BR.EzTravel.Web.Areas.EN.Controllers
{
    [Authorize(Roles = "SA")]
    public class AdminProfileController : BaseEnController
    {
        public ActionResult Index()
        {
            var id = Util.SessionAccess.ID;

            var member = db.tblmembers.Single(a => a.ID == id);
            var viewModel = new MemberEditViewModel
            {
                ID = member.ID,
                Address = member.Address,
                BusinessRegNo = member.BusinessRegNo,
                CompanyName = member.CompanyName,
                Country = member.Country,
                PICEmail = member.PICEmail,
                PICContact = member.PICContact,
                PICName = member.PICName,
                Postcode = member.Postcode,
                Roles = member.Roles,
                State = member.State,
                ProfileImagePath = member.ProfileImagePath,
                Active = member.Active
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(HttpPostedFileBase file, MemberEditViewModel viewModel)
        {
            var member = db.tblmembers.Single(a => a.ID == viewModel.ID);
            
            member.BusinessRegNo = viewModel.BusinessRegNo;
            member.CompanyName = viewModel.CompanyName;
            member.PICName = viewModel.PICName;
            member.PICContact = viewModel.PICContact;
            member.PICEmail = viewModel.PICEmail;
            member.Address = viewModel.Address;
            member.Postcode = viewModel.Postcode;
            member.State = viewModel.State;
            member.Country = viewModel.Country;
            //member.Active = viewModel.Active;

            if (file != null)
            {
                var fileName = FileUploadManager.UploadAndSave(file);
                member.ProfileImagePath = fileName;
            }

            db.SaveChanges();

            Util.SessionAccess = member;

            return RedirectToAction("Index");
        }
    }
}