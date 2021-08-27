using ShopNow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopNow.Filters;
using ShopNow.ViewModels;
using ShopNow.Helpers;

namespace ShopNow.Controllers
{
    public class ShopScheduleController : Controller
    {
        private sncEntities db = new sncEntities();

        [AccessPolicy(PageCode = "")]
        public ActionResult Index(ShopScheduleIndexViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.ShopSchedules.Where(i => i.Status == 0 && model.FilterShopId !=0 ? i.ShopId == model.FilterShopId : true)
                .Join(db.Shops, sc => sc.ShopId, s => s.Id, (sc, s) => new { sc, s })
                .GroupBy(i => i.sc.ShopId)
            .Select(i => new ShopScheduleIndexViewModel.ListItem
            {
                Id = i.FirstOrDefault().sc.Id,
                HasSchedule = i.FirstOrDefault().s.HasSchedule ?? false,
                ShopId = i.Key,
                ShopName = i.FirstOrDefault().s.Name,
                TimeListItems = i.Where(a => a.sc.Status == 0).Select(a => new ShopScheduleIndexViewModel.ListItem.TimeListItem
                {
                    OffTime = a.sc.OffTime,
                    OnTime = a.sc.OnTime
                }).ToList()
            }).ToList();
            return View(model);
        }

        public JsonResult Add(ShopScheduleAddViewModel model)
        {
            var isExist = db.ShopSchedules.Any(i => i.ShopId == model.ShopId);
            if (!isExist)
            {
                var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
                foreach (var item in model.TimeListItems)
                {
                    var shopSchedule = new ShopSchedule
                    {
                        DateTimeUpdated = DateTime.Now,
                        OffTime = item.OffTime,
                        OnTime = item.OnTime,
                        ShopId = model.ShopId,
                        Status = 0,
                        UpdatedBy = user.Name
                    };
                    db.ShopSchedules.Add(shopSchedule);
                    db.SaveChanges();
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
            
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult Delete(string shopId)
        {
            int dId = AdminHelpers.DCodeInt(shopId);
            var shopSchedule = db.ShopSchedules.Where(i => i.ShopId == dId).ToList();
            if (shopSchedule.Count() > 0)
            {
                shopSchedule.ForEach(i => i.Status = 2);
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult UpdateSchedule(int shopid, bool hasSchedule)
        {
            var shop = db.Shops.FirstOrDefault(i => i.Id == shopid && i.Status == 0);
            shop.HasSchedule = hasSchedule;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult AddTiming(int shopid, TimeSpan onTime, TimeSpan offTime)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shopSchedule = new ShopSchedule
            {
                DateTimeUpdated = DateTime.Now,
                OffTime = offTime,
                OnTime = onTime,
                ShopId = shopid,
                Status = 0,
                UpdatedBy = user.Name
            };
            db.ShopSchedules.Add(shopSchedule);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult UpdateTiming(int id,TimeSpan onTime,TimeSpan offTime)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shopSchedule = db.ShopSchedules.FirstOrDefault(i => i.Id == id);
            shopSchedule.OnTime = onTime;
            shopSchedule.OffTime = offTime;
            shopSchedule.UpdatedBy = user.Name;
            db.Entry(shopSchedule).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult DeleteTiming(string id)
        {
            int dId = AdminHelpers.DCodeInt(id);
            var shopSchedule = db.ShopSchedules.FirstOrDefault(i => i.Id == dId);
            if (shopSchedule != null)
            {
                shopSchedule.Status = 2;
                db.Entry(shopSchedule).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}