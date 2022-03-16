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
                }).OrderByDescending(i=>i.Count).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult SendBulk(string title, string message, string[] district, int type)
        {
            if(type == 1)
            {
                var fcmTokenList = db.Customers.OrderBy(i => i.Id).Where(i => !string.IsNullOrEmpty(i.FcmTocken) && i.FcmTocken != "NULL").Select(i => i.FcmTocken).ToArray();
                if (fcmTokenList.Count() < 1000)
                    Helpers.PushNotification.SendBulk(message, title, "a.mp3", fcmTokenList.Take(1000).ToArray());
                if (fcmTokenList.Count() > 1000 && fcmTokenList.Count() <= 2000)
                    Helpers.PushNotification.SendBulk(message, title, "a.mp3", fcmTokenList.Skip(1000).Take(1000).ToArray());
                if (fcmTokenList.Count() > 2000 && fcmTokenList.Count() <= 3000)
                    Helpers.PushNotification.SendBulk(message, title, "a.mp3", fcmTokenList.Skip(2000).Take(1000).ToArray());
            }
            if (type == 2)
            {
                var fcmTokenList = db.Customers.Where(i => !string.IsNullOrEmpty(i.FcmTocken) && i.FcmTocken != "NULL" && district.Contains(i.DistrictName)).Select(i => i.FcmTocken).ToArray();
                if (fcmTokenList.Count() < 1000)
                    Helpers.PushNotification.SendBulk(message, title, "a.mp3", fcmTokenList.Take(1000).ToArray());
                if (fcmTokenList.Count() > 1000 && fcmTokenList.Count() <= 2000)
                    Helpers.PushNotification.SendBulk(message, title, "a.mp3", fcmTokenList.Skip(1000).Take(1000).ToArray());
                if (fcmTokenList.Count() > 2000 && fcmTokenList.Count() <= 3000)
                    Helpers.PushNotification.SendBulk(message, title, "a.mp3", fcmTokenList.Skip(2000).Take(1000).ToArray());
            }
            if (type == 3)
            {
                var latestVersion = db.AppDetails.FirstOrDefault().Version;
                var fcmTokenList = db.Customers.Where(i => !string.IsNullOrEmpty(i.FcmTocken))
                    .Join(db.CustomerAppInfoes.Where(i=>i.Version != latestVersion), c => c.Id, ca => ca.CustomerId, (c, ca) => new { c, ca })
                    .Select(i => i.c.FcmTocken).ToArray();
                if (fcmTokenList.Count() < 1000)
                    Helpers.PushNotification.SendBulk(message, title, "a.mp3", fcmTokenList.Take(1000).ToArray());
                if (fcmTokenList.Count() > 1000 && fcmTokenList.Count() <= 2000)
                    Helpers.PushNotification.SendBulk(message, title, "a.mp3", fcmTokenList.Skip(1000).Take(1000).ToArray());
                if (fcmTokenList.Count() > 2000 && fcmTokenList.Count() <= 3000)
                    Helpers.PushNotification.SendBulk(message, title, "a.mp3", fcmTokenList.Skip(2000).Take(1000).ToArray());

            }
            return RedirectToAction("Index");
        }

    }
}