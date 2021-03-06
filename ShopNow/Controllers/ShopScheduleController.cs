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

        [AccessPolicy(PageCode = "SNCSHI267")]
        public ActionResult Index(ShopScheduleIndexViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.ShopSchedules.Where(i => i.Status == 0 && (model.FilterShopId != 0 ? i.ShopId == model.FilterShopId : true))
                .Join(db.Shops.Where(i => (!string.IsNullOrEmpty(model.FilterDistrict)) ? i.DistrictName == model.FilterDistrict : true), sc => sc.ShopId, s => s.Id, (sc, s) => new { sc, s })
                .GroupBy(i => i.sc.ShopId)
            .Select(i => new ShopScheduleIndexViewModel.ListItem
            {
                HasSchedule = i.FirstOrDefault().s.HasSchedule ?? false,
                ShopId = i.Key,
                ShopName = i.FirstOrDefault().s.Name,
                LeaveDays = i.FirstOrDefault().sc.LeaveDays,
                TimeListItems = i.Where(a => a.sc.Status == 0).Select(a => new ShopScheduleIndexViewModel.ListItem.TimeListItem
                {
                    Id = a.sc.Id,
                    OffTime = a.sc.OffTime,
                    OnTime = a.sc.OnTime
                }).ToList()
            }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCSHD268")]
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

        [AccessPolicy(PageCode = "SNCSHUS269")]
        public JsonResult UpdateSchedule(int shopid, bool hasSchedule)
        {
            //var shop = db.Shops.FirstOrDefault(i => i.Id == shopid && i.Status == 0);
            //shop.HasSchedule = hasSchedule;
            //db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            //db.SaveChanges();
            UpdateShopSchedule(shopid, hasSchedule);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SNCSHAT270")]
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
                UpdatedBy = user.Name,
                LeaveDays = db.ShopSchedules.FirstOrDefault(i=>i.ShopId == shopid).LeaveDays
            };
            db.ShopSchedules.Add(shopSchedule);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AccessPolicy(PageCode = "SNCSHUT271")]
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

        [AccessPolicy(PageCode = "SNCSHDT272")]
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

        [AccessPolicy(PageCode = "SNCSHAS273")]
        public JsonResult Add(ShopScheduleAddViewModel model)
        {
            var isExist = db.ShopSchedules.Any(i => i.ShopId == model.ShopId && i.Status == 0);
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
                        UpdatedBy = user.Name,
                        LeaveDays = string.Join(",", model.LeaveDays)
                };
                    db.ShopSchedules.Add(shopSchedule);
                    db.SaveChanges();
                }

                UpdateShopSchedule(model.ShopId, true);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);

        }

        public void UpdateShopSchedule(int shopid,bool hasSchedule)
        {
            var shop = db.Shops.FirstOrDefault(i => i.Id == shopid && i.Status == 0);
            shop.HasSchedule = hasSchedule;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public ActionResult RemoveLeaveDays(string shopId)
        {
            int dId = AdminHelpers.DCodeInt(shopId);
            var shopSchedule = db.ShopSchedules.Where(i => i.ShopId == dId && i.Status==0).ToList();
            if (shopSchedule.Count() > 0)
            {
                shopSchedule.ForEach(i => i.LeaveDays = string.Empty);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult AddUpdateLeaveDays(int shopId,string[] LeaveDays)
        {
            var shopSchedule = db.ShopSchedules.Where(i => i.ShopId == shopId && i.Status == 0).ToList();
            if (shopSchedule.Count() > 0)
            {
                shopSchedule.ForEach(i => i.LeaveDays = string.Join(",", LeaveDays));
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}