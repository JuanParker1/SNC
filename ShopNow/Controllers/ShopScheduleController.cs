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
        public ActionResult Index()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new ShopScheduleIndexViewModel();
            model.ListItems = db.ShopSchedules.Where(i => i.Status == 0)
                .Join(db.Shops, sc => sc.ShopId, s => s.Id, (sc, s) => new { sc, s })
                .GroupBy(i => i.sc.ShopId)
            .Select(i => new ShopScheduleIndexViewModel.ListItem
            {
                Id = i.FirstOrDefault().sc.Id,
                HasSchedule = i.FirstOrDefault().s.HasSchedule,
                ShopId = i.Key,
                ShopName = i.FirstOrDefault().s.Name,
                TimeListItems = i.Select(a => new ShopScheduleIndexViewModel.ListItem.TimeListItem
                {
                    OffTime = a.sc.OffTime,
                    OnTime = a.sc.OnTime
                }).ToList()
            }).ToList();
            return View(model);
        }

        public JsonResult Add(ShopScheduleAddViewModel model)
        {
            try
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
            catch {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult Delete(string shopId)
        {
            int dId = AdminHelpers.DCodeInt(shopId);
            var shopSchedule = db.ShopSchedules.Where(i => i.ShopId == dId).ToList();
            if (shopSchedule.Count() > 0)
            {
                shopSchedule.ForEach(i => i.Status = 2);
                db.Entry(shopSchedule).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}