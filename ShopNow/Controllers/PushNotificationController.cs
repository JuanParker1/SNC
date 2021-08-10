using ShopNow.Filters;
using ShopNow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class PushNotificationController : Controller
    {
        private sncEntities db = new sncEntities();

        [AccessPolicy(PageCode = "")]
        public ActionResult Index()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [AccessPolicy(PageCode = "")]
        [HttpPost]
        public ActionResult SendBulk(string title,string message,string[] district)
        {
            var fcmTokenList = db.Customers.Where(i => !string.IsNullOrEmpty(i.FcmTocken) && i.FcmTocken != "NULL" && district.Contains(i.DistrictName)).Select(i=>i.FcmTocken).ToArray();
            Helpers.PushNotification.SendBulk(message, title, "a.mp3", fcmTokenList);
            return RedirectToAction("Index");
        }
    }
}