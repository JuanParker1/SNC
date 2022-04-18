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
        public ActionResult Index(string message = "", int type = 0)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new PushNotificationIndexViewModel();
            model.ListItems = db.Customers.Where(i => !string.IsNullOrEmpty(i.FcmTocken) && !string.IsNullOrEmpty(i.DistrictName) && i.FcmTocken != "NULL" && i.DistrictName != "NULL")
                .GroupBy(i => i.DistrictName).OrderBy(i => i.FirstOrDefault().DistrictName)
                .Select(i => new PushNotificationIndexViewModel.ListItem
                {
                    Count = i.Count(),
                    DistrictName = i.FirstOrDefault().DistrictName,
                    Index = 0
                }).OrderByDescending(i => i.Count).ToList();

            if (type == 1)
                ViewBag.Message = message;
            else if (type == 2)
                ViewBag.ErrorMessage = message;

            return View(model);
        }

        public ActionResult List()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new PushNotificationListViewModel();
            model.ListItems = db.PushNotifications
                .Select(i => new PushNotificationListViewModel.ListItem
                {
                    DateEncoded = i.DateEncoded,
                    Description = i.Description,
                    DistrictName = i.Type == 1 ? "All Customer" : i.District,
                    EncodedBy = i.EncodedBy,
                    Title = i.Title
                }).ToList();
            int counter = 1;
            model.ListItems.ForEach(x => x.Index = counter++);
            return View(model);
        }

        [HttpPost]
        public ActionResult SendBulk(string title, string message, string[] district, int type,DateTime? scheduleDateTime, string imagePath = "")
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            try
            {
                var pushNotification = new PushNotification
                {
                    DateEncoded = DateTime.Now,
                    Description = message,
                    District = string.Join(",", district),
                    EncodedBy = user.Name,
                    ImageUrl = imagePath,
                    RedirectUrl = "",
                    Status = scheduleDateTime == null ? 0 : 1,
                    Title = title,
                    Type = type,
                    ScheduleDateTime = scheduleDateTime
                };
                db.PushNotifications.Add(pushNotification);
                db.SaveChanges();

                if (type == 1 && scheduleDateTime == null)
                {
                    var fcmTokenList = db.Customers.OrderBy(i => i.Id).Where(i => !string.IsNullOrEmpty(i.FcmTocken) && i.FcmTocken != "NULL").Select(i => i.FcmTocken).ToArray();
                    var count = Math.Ceiling((double)fcmTokenList.Count() / 1000);
                    for (int i = 0; i < count; i++)
                    {
                        Helpers.PushNotification.SendBulk(message, title, "SpecialOffer", imagePath, fcmTokenList.Skip(i * 1000).Take(1000).ToArray(), "tune2.caf", "", "", pushNotification.Id);
                    }
                    //Helpers.PushNotification.SendBulk(message, title, "SpecialOffer", imagePath, fcmTokenList.Take(1000).ToArray());
                    //Helpers.PushNotification.SendBulk(message, title, "SpecialOffer", imagePath, fcmTokenList.Skip(1000).Take(1000).ToArray());
                    //Helpers.PushNotification.SendBulk(message, title, "SpecialOffer", imagePath, fcmTokenList.Skip(2000).Take(1000).ToArray());
                }
                if (type == 2 && scheduleDateTime == null)
                {
                    var fcmTokenList = db.Customers.Where(i => !string.IsNullOrEmpty(i.FcmTocken) && i.FcmTocken != "NULL" && district.Contains(i.DistrictName)).Select(i => i.FcmTocken).ToArray();
                    var count = Math.Ceiling((double)fcmTokenList.Count() / 1000);
                    for (int i = 0; i < count; i++)
                    {
                        Helpers.PushNotification.SendBulk(message, title, "SpecialOffer", imagePath, fcmTokenList.Skip(i * 1000).Take(1000).ToArray(), "tune2.caf", "", "", pushNotification.Id);
                    }
                    //Helpers.PushNotification.SendBulk(message, title, "SpecialOffer", imagePath, fcmTokenList.Take(1000).ToArray());
                    //Helpers.PushNotification.SendBulk(message, title, "SpecialOffer", imagePath, fcmTokenList.Skip(1000).Take(1000).ToArray());
                    //Helpers.PushNotification.SendBulk(message, title, "SpecialOffer", imagePath, fcmTokenList.Skip(2000).Take(1000).ToArray());
                }
                if (type == 3 && scheduleDateTime == null)
                {
                    var latestVersion = db.AppDetails.FirstOrDefault().Version;
                    var fcmTokenList = db.Customers.Where(i => !string.IsNullOrEmpty(i.FcmTocken))
                        .Join(db.CustomerAppInfoes.Where(i => i.Version != latestVersion), c => c.Id, ca => ca.CustomerId, (c, ca) => new { c, ca })
                        .Select(i => i.c.FcmTocken).ToArray();
                    var count = Math.Ceiling((double)fcmTokenList.Count() / 1000);
                    for (int i = 0; i < count; i++)
                    {
                        Helpers.PushNotification.SendBulk(message, title, "SpecialOffer", imagePath, fcmTokenList.Skip(i * 1000).Take(1000).ToArray(), "tune2.caf", "", "", 0);
                    }
                    //Helpers.PushNotification.SendBulk(message, title, "SpecialOffer", imagePath, fcmTokenList.Take(1000).ToArray());
                    //Helpers.PushNotification.SendBulk(message, title, "SpecialOffer", imagePath, fcmTokenList.Skip(1000).Take(1000).ToArray());
                    //Helpers.PushNotification.SendBulk(message, title, "SpecialOffer", imagePath, fcmTokenList.Skip(2000).Take(1000).ToArray());
                }

               
                string alertmessage = "";
                alertmessage = scheduleDateTime == null ? "Notification Send Successfully!" : "Notification Scheduled Successfully!";
                return RedirectToAction("Index", new { message = alertmessage, type = 1 });
            }
            catch
            {
                return RedirectToAction("Index", new { message = "Something went wrong!", type = 2 });
            }
        }

        public ActionResult NotificationLogin()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new NotificationLoginViewModel();
            model.NotificationLists = db.NotificationLogins.Where(i => i.Status == 0).Select(i => new NotificationLoginViewModel.NotificationList
            {
                Id = i.Id,
                Name = i.Name,
                PhoneNumber = i.PhoneNumber,
                Password = i.Password,
                EncodedBy = i.EncodedBy,
                DateEncoded = i.DateEncoded
            }).ToList();
            return View(model.NotificationLists);
        }

        public JsonResult Save(string Name, string Phonenumber, string Password)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var notificationlogin = new NotificationLogin();
            notificationlogin.Name = Name;
            notificationlogin.PhoneNumber = Phonenumber;
            notificationlogin.Password = Password;
            notificationlogin.Status = 0;
            notificationlogin.DateEncoded = DateTime.Now;
            notificationlogin.DateUpdated = DateTime.Now;
            notificationlogin.EncodedBy = user.Name;
            notificationlogin.UpdatedBy = user.Name;
            db.NotificationLogins.Add(notificationlogin);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Edit(int id, string name, string phonenumber, string password)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            NotificationLogin notification = db.NotificationLogins.Where(n => n.Id == id).FirstOrDefault();
            if (notification != null)
            {
                notification.Name = name;
                notification.Password = password;
                notification.PhoneNumber = phonenumber;
                notification.UpdatedBy = user.Name;
                notification.DateUpdated = DateTime.Now;
                db.Entry(notification).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var notification = db.NotificationLogins.FirstOrDefault(i => i.Id == id && i.Status == 0);
            if (notification != null)
            {
                notification.Status = 2;
                notification.UpdatedBy = user.Name;
                notification.DateUpdated = DateTime.Now;
                db.Entry(notification).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}