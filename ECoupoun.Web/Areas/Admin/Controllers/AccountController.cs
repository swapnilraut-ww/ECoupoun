using ECoupoun.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ECoupoun.Web.Areas.Admin.Controllers
{
    public class AccountController : BaseController
    {
        //
        // GET: /Admin/Account/
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User userModel)
        {
            User user = db.Users.Where(x => x.Name == userModel.Name && x.Password == userModel.Password).SingleOrDefault();
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.Name, false);
                return RedirectToAction("Index", "Home", new { Area = "Admin" });
            }
            return View();
        }

        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}