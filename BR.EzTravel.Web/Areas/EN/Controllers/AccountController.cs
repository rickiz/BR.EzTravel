using BR.EzTravel.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BR.EzTravel.Web.Helpers;

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