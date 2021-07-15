using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Net;

namespace ShopNow.Controllers
{
    // [AuthorizeUser(Roles = "Admin")]
    public class HomeController : Controller
    {

        private ShopnowchatEntities _db = new ShopnowchatEntities();
        UploadContent uc = new UploadContent();

        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            if (CheckAlreadyLoggedIn())
                return Redirect(returnUrl != null ? returnUrl : "/dashboard/index");
            else
            {
                LoginCreateViewModel loginModel = new LoginCreateViewModel();
                loginModel.PhoneNumber = Request.Cookies["PhoneNumber"] != null ? Request.Cookies["PhoneNumber"].Value : "";
                loginModel.Password = Request.Cookies["Password"] != null ? Request.Cookies["Password"].Value : "";
                ViewBag.ReturnUrl = returnUrl;
                return View(loginModel);
            }
        }


        private bool CheckAlreadyLoggedIn()
        {
            bool alreadyLoggedIn = false;
            if (Session["PhoneNumber"] != null)
            {
                string userId = Convert.ToString(Session["PhoneNumber"]);
                var user = _db.Customers.FirstOrDefault(i => i.PhoneNumber == userId && i.Status == 0);
                if (user != null)
                {
                    Response.Cookies["Name"].Value = user.Name;

                    alreadyLoggedIn = true;
                }
            }
            return alreadyLoggedIn;
        }



        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Index(LoginCreateViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                string ip = GetIP();

                var user = _db.Customers.FirstOrDefault(i => i.PhoneNumber == model.PhoneNumber && i.Password == model.Password && i.Status == 0 && i.Position == 4);

                if (user != null && string.IsNullOrEmpty(user.IpAddress))
                {
                    Helpers.Sessions.Set(model.PhoneNumber);
                    Session["PhoneNumber"] = user.PhoneNumber;
                    Response.Cookies["Name"].Value = user.Name;

                    if (Request["RememberMe1"] == "on")
                    {
                        Response.Cookies["PhoneNumber"].Value = user.PhoneNumber;
                        Response.Cookies["Password"].Value = user.Password;
                    }
                    else
                    {
                        Response.Cookies["PhoneNumber"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["Password"].Expires = DateTime.Now.AddDays(-1);
                    }

                    return RedirectToAction(returnUrl != null ? returnUrl : "LivePending", "Support");
                    //}
                }
                else if (user != null && !string.IsNullOrEmpty(user.IpAddress))
                {
                    string[] ipAddressArray = user.IpAddress.Split(',');
                    if (ipAddressArray.Contains(ip))
                    {
                        Helpers.Sessions.Set(model.PhoneNumber);
                        Session["PhoneNumber"] = user.PhoneNumber;
                        Response.Cookies["Name"].Value = user.Name;

                        if (Request["RememberMe1"] == "on")
                        {
                            Response.Cookies["PhoneNumber"].Value = user.PhoneNumber;
                            Response.Cookies["Password"].Value = user.Password;
                        }
                        else
                        {
                            Response.Cookies["PhoneNumber"].Expires = DateTime.Now.AddDays(-1);
                            Response.Cookies["Password"].Expires = DateTime.Now.AddDays(-1);
                        }

                        return RedirectToAction(returnUrl != null ? returnUrl : "LivePending", "Support");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Access Denied!";
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid Phone Number or Password";
                    ModelState.AddModelError("Password", "Invalid email address or password");
                }

            }
            return View(model);
        }


        [HttpGet]
        [AccessPolicy(PageCode = "SHNHOMCP002")]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNHOMCP002")]
        public ActionResult ChangePassword(ChangePasswordViewModel cpm)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var ExistingDetails = _db.Customers.FirstOrDefault(i => i.Id == user.Id && i.Status == 0);
            if (cpm.OldPassword == ExistingDetails.Password)
            {
                ExistingDetails.Password = cpm.NewPassword;
                _db.SaveChanges();
                ViewBag.PasswordChangeMsg = "Password changed successfully";
            }
            else
                ModelState.AddModelError("OldPassword", "Current password is wrong");
            return View();
        }

        [AccessPolicy(PageCode = "SHNHOML001")]
        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }

        public ActionResult Access()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }
        public ActionResult ImageDenied()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        public ActionResult ErrorPage()
        {
            return View();
        }


        public JsonResult ApiAccess()
        {
            return Json("Bad Authorization", JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }



        public string GetIP()
        {
            string strHostName = System.Net.Dns.GetHostName();

            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

            IPAddress[] addr = ipEntry.AddressList;

            string Ip = addr[addr.Length - 1].ToString();

            return addr[addr.Length - 1].ToString();

        }

    }
}
