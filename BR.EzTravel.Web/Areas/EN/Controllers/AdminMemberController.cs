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
    public class AdminMemberController : BaseEnController
    {
        // GET: EN/AdminMember
        public ActionResult Index()
        {
            var members =
               db.tblmembers
                   .Where(a => a.Language == lang)
                   .OrderByDescending(a => a.ID)
                   .Take(20)
                   .Select(a => new AdminMemberIndexItem
                   {
                       ID = a.ID,
                       Username = a.PICEmail,
                       Name = a.PICName,
                       Active = a.Active,
                       MemberSince = a.CreateDT
                   })
                   .ToList();
            var viewModel = new AdminMemberIndexViewModel { Members = members };

            return View(viewModel);
        }

        public ActionResult Create()
        {
            return View(new MemberCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MemberCreateViewModel viewModel)
        {
            try
            {
                var Member = new tblmember()
                {
                    BusinessRegNo = viewModel.BusinessRegNo,
                    CompanyName = viewModel.CompanyName,
                    PICName = viewModel.PICName,
                    PICContact = viewModel.PICContact,
                    PICEmail = viewModel.PICEmail,
                    OfficeContact = "-",
                    Address = viewModel.Address,
                    Postcode = viewModel.Postcode,
                    State = viewModel.State,
                    Country = viewModel.Country,
                    Roles = "SA",
                    Password = RandomString(),
                    CreateDT = DateTime.Now,
                    Language = "EN",
                    Active = true
                };

                db.tblmembers.Add(Member);
                db.SaveChanges();

                var emailBody = string.Format(@"Hi {0}, <br /><br />
                    Thanks for Subscribing to the EZ Go Holiday Website ! You’re joining an amazing community of folks who love nerding out about travelling. <br /><br />
                    As you wait for the next issue, check out some of our most popular posts. They’re a great place to get started. <br /><br />
                    Website: http://www.ezgoholiday.com <br />
                    Username: {1} <br />
                    Password: {2} <br /><br />

                    Have an awesome day! <br />
                    EZ Go Holiday Management",
                    viewModel.PICName, viewModel.PICEmail, Member.Password);

                Util.SendEmail("Welcome to EZ Go Holiday!", emailBody, viewModel.PICEmail, "", "");
            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
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
                Active = member.Active
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HttpPostedFileBase file, MemberEditViewModel viewModel)
        {


            var member = db.tblmembers.Single(a => a.ID == viewModel.ID);

            if (member.PICEmail != viewModel.PICEmail)
            {
                var duplicateEmail = db.tblmembers.Where(a => a.PICEmail == viewModel.PICEmail && a.ID != viewModel.ID).FirstOrDefault();
                if (duplicateEmail != null)
                {
                    viewModel.ErrorMessage = string.Format("Email address {0} already exist.", viewModel.PICEmail);

                    return View("Edit", viewModel);
                }
            }

            member.BusinessRegNo = viewModel.BusinessRegNo;
            member.CompanyName = viewModel.CompanyName;
            member.PICName = viewModel.PICName;
            member.PICContact = viewModel.PICContact;
            member.PICEmail = viewModel.PICEmail;
            member.Address = viewModel.Address;
            member.Postcode = viewModel.Postcode;
            member.State = viewModel.State;
            member.Country = viewModel.Country;
            member.Active = viewModel.Active;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(8)]).ToArray());
        }
    }
}