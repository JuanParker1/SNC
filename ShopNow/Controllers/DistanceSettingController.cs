using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class DistanceSettingController : Controller
    {
        private sncEntities db = new sncEntities();

        [AccessPolicy(PageCode = "")]
        public ActionResult Index()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DistanceSettingIndexViewModel();
            model.ListItems = db.DistanceSettings.ToList().Where(i => i.Status == 0)
            .Select(i => new DistanceSettingIndexViewModel.ListItem
            {
                Id = i.Id,
                Distance = i.Distance,
                Time = i.Time,
                UpdatedBy = i.UpdatedBy,
                Status = i.Status,
                UpdatedDateTime = i.UpdatedDateTime,
                TimeText = DateTime.Today.Add(i.Time.Value).ToString("hh:mm tt")
            }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "")]
        [HttpPost]
        public ActionResult Create(double distance, TimeSpan? time)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var isExist = db.DistanceSettings.Any(i => i.Distance == distance && i.Time == time);
            if (!isExist)
            {
                var distanceSetting = new DistanceSetting
                {
                    Distance = distance,
                    Status = 0,
                    Time = time,
                    UpdatedDateTime = DateTime.Now,
                    UpdatedBy = user.Name
                };
                db.DistanceSettings.Add(distanceSetting);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [AccessPolicy(PageCode = "")]
        [HttpPost]
        public ActionResult Edit(int id, double distance, TimeSpan? time)
        {
             var user = ((Helpers.Sessions.User)Session["USER"]);
            var distanceSetting = db.DistanceSettings.FirstOrDefault(i => i.Id == id);
            distanceSetting.Distance = distance;
            distanceSetting.Time = time;
            distanceSetting.UpdatedDateTime = DateTime.Now;
            distanceSetting.UpdatedBy = user.Name;
            db.Entry(distanceSetting).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult Delete(string id)
        {
            int dId = AdminHelpers.DCodeInt(id);
            var distanceSetting = db.DistanceSettings.FirstOrDefault(i => i.Id == dId);
            if (distanceSetting != null)
            {
                distanceSetting.Status = 2;
                db.Entry(distanceSetting).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}