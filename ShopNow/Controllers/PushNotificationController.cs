using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
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

        [AccessPolicy(PageCode = "SNCPNI237")]
        public ActionResult Index()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new PushNotificationIndexViewModel();
            model.ListItems = db.Customers.Where(i => !string.IsNullOrEmpty(i.FcmTocken) && !string.IsNullOrEmpty(i.DistrictName) && i.FcmTocken != "NULL" && i.DistrictName !="NULL")
                .GroupBy(i => i.DistrictName).OrderBy(i => i.FirstOrDefault().DistrictName)
                .Select(i=> new PushNotificationIndexViewModel.ListItem
                {
                    Count = i.Count(),
                    DistrictName = i.FirstOrDefault().DistrictName,
                    Index = 0
                }).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult SendBulk(string title,string message,string[] district)
        {
            var fcmTokenList = db.Customers.Where(i => !string.IsNullOrEmpty(i.FcmTocken) && i.FcmTocken != "NULL" && district.Contains(i.DistrictName)).Select(i=>i.FcmTocken).ToArray();
            Helpers.PushNotification.SendBulk(message, title, "a.mp3", fcmTokenList);
            return RedirectToAction("Index");
        }

    }
}