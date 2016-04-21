using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BR.EzTravel.Web.Models;
using BR.EzTravel.Web.Helpers;
using BR.EzTravel.Web;

namespace BR.EzTravel.Web.Areas.EN.Controllers
{
    public class ContactUsController : BaseEnController
    {
        // GET: EN/ContactUs
        public ActionResult Index()
        {
            return View(new ContactUsIndexViewModel());
        }

        [HttpPost]
        public ActionResult Index(ContactUsIndexViewModel viewModel)
        {
            var contactUs = new trnroi()
            {
                Comment = viewModel.Comments,
                ContatNo = viewModel.Contact,
                CreateDT = DateTime.Now,
                Email = viewModel.Email,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Language = lang,
                Subject = "-"
            };

            db.trnrois.Add(contactUs);
            db.SaveChanges();

            var emailBody = string.Format(@"Hi EZ Management, <br /><br />
                    First Name <b>{0}</b> <br /><br />
                    Last Name <b>{1}</b> <br /><br />
                    Contact <b>{2}</b> <br /><br />
                    Email <b>{3}</b> <br /><br />
                    Comment <b>{4}</b> <br /><br />",
                    viewModel.FirstName, viewModel.LastName, viewModel.Contact, viewModel.Email, viewModel.Comments);

            Util.SendEmail("EZ Go Holiday Contact Us", emailBody, "sales@ezgoholiday.com", "", "");

            return RedirectToAction("Index");
        }
    }
}