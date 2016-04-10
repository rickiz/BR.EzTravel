using BR.EzTravel.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BR.EzTravel.Web.Helpers;
using BR.EzTravel.Web.Models.Admin;

namespace BR.EzTravel.Web.Areas.EN.Controllers
{
    public class AccountController : BaseEnController
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var incorrectUsernamePasswordMessage = "The user name or password provided is incorrect.";

            var hashPassword = Util.GetMD5Hash(model.Password);
            var access = db.tblmembers
                .SingleOrDefault(a => a.Active && a.PICEmail == model.Email && a.Password == hashPassword);

            if (access == null)
            {
                ModelState.AddModelError("", incorrectUsernamePasswordMessage);
                return View(model);
            }

            Util.SessionAccess = access;
            FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);

            return RedirectToLocal(returnUrl);
        }

        [Authorize]
        public ActionResult Logout()
        {
            Util.SessionAccess = null;
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(HttpPostedFileBase file, RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var member = new tblmember()
            {
                PICName = viewModel.Name,
                PICEmail = viewModel.Email,
                Roles = "NU",
                Password = Util.GetMD5Hash(viewModel.Password),
                CreateDT = DateTime.Now,
                Language = "EN",
                Active = true
            };

            if (file != null)
            {
                var fileName = FileUploadManager.UploadAndSave(file);
                member.ProfileImagePath = fileName;
            }

            db.tblmembers.Add(member);
            db.SaveChanges();

            var emailBody = string.Format(@"Hi {0}, <br /><br />
                    Thanks for Subscribing to the EZ Go Holiday Website ! You’re joining an amazing community of folks who love nerding out about travelling. <br /><br />
                    As you wait for the next issue, check out some of our most popular posts. They’re a great place to get started. <br /><br />
                    Website: http://www.ezgoholiday.com <br />
                    Username: {1} <br />
                    Password: {2} <br /><br />

                    Have an awesome day! <br />
                    EZ Go Holiday Management",
                viewModel.Name, viewModel.Email, viewModel.Password);

            Util.SendEmail("Welcome to EZ Go Holiday!", emailBody, viewModel.Email, "", "");

            Util.SessionAccess = member;
            FormsAuthentication.SetAuthCookie(viewModel.Email, false);

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var member = db.tblmembers.SingleOrDefault(a => a.PICEmail == viewModel.Email && a.Active);

            if (member != null)
            {
                var resetPassword = Util.RandomString();
                var emailBody = string.Format(@"Hi {0}, <br /><br />
                    Your account login details. <br /><br />

                    Username: {1} <br />
                    New Password: {2} <br /><br />

                    Have an awesome day! <br />
                    EZ Go Holiday Management",
                member.PICName, member.PICEmail, resetPassword);

                member.Password = Util.GetMD5Hash(resetPassword);
                db.SaveChanges();

                Util.SendEmail("EZ Go Holiday - Forgot Password", emailBody, viewModel.Email, "", "");
            }

            return View("ForgotPasswordConfirmation");
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ResetPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var member = db.tblmembers.Single(a => a.ID == Util.SessionAccess.ID && a.Active);
            if (member != null)
            {
                var oldPassword = Util.GetMD5Hash(viewModel.OldPassword);

                if(oldPassword != member.Password)
                {
                    ModelState.AddModelError("", "Invalid old password!");
                    return View(viewModel);
                }

                member.Password = Util.GetMD5Hash(viewModel.Password);
                db.SaveChanges();
            }

            return RedirectToAction("ChangePasswordConfirmation", "Account");
        }

        [AllowAnonymous]
        public ActionResult ChangePasswordConfirmation()
        {
            return View();
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}