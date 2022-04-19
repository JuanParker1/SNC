using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class DailyVisitorController : Controller
    {
        private sncEntities db = new sncEntities();

        [AccessPolicy(PageCode = "SNCDVL336")]
        public ActionResult List(DailyVisitorListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.StartDateFilter = model.StartDateFilter == null ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)  : model.StartDateFilter;
            model.EndDateFilter = model.EndDateFilter == null ? DateTime.Now : model.EndDateFilter;

            model.ListItems = db.DailyVisitors.Where(i => (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.StartDateFilter) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.EndDateFilter))).GroupBy(i => i.ShopId)
                .GroupJoin(db.Orders.Where(i => (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.StartDateFilter) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.EndDateFilter)) && i.Status == 6), dv => dv.FirstOrDefault().ShopId, o => o.ShopId, (dv, o) => new { dv, o })
                .Select(i => new DailyVisitorListViewModel.ListItem
                {
                    Count = i.dv.Count(),
                    //CustomerId = i.Key.CustomerId,
                    DateUpdated = i.dv.OrderByDescending(a => a.DateUpdated).FirstOrDefault().DateUpdated,
                    ShopId = i.dv.Key,
                    ShopName = i.dv.OrderByDescending(a => a.DateUpdated).FirstOrDefault().ShopName,
                    OrderCount = i.dv.Key !=0 ? i.o.Count() : db.Orders.Where(a => (DbFunctions.TruncateTime(a.DateEncoded) >= DbFunctions.TruncateTime(model.StartDateFilter) && DbFunctions.TruncateTime(a.DateEncoded) <= DbFunctions.TruncateTime(model.EndDateFilter)) && a.Status == 6).GroupBy(a=>a.CustomerId).Count(),
                    //ConversionRate = Math.Round((double)i.o.Count() / i.dv.Count() * 100,2)
                    ConversionRate = Math.Round((double)(i.dv.Key != 0 ? i.o.Count() : db.Orders.Where(a => (DbFunctions.TruncateTime(a.DateEncoded) >= DbFunctions.TruncateTime(model.StartDateFilter) && DbFunctions.TruncateTime(a.DateEncoded) <= DbFunctions.TruncateTime(model.EndDateFilter)) && a.Status == 6).GroupBy(a => a.CustomerId).Count()) / i.dv.Count() * 100,2)
                }).OrderByDescending(i => i.Count).ToList();

            model.AndroidHomeCount = db.DailyVisitors.Where(i => i.ShopId == 0 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.StartDateFilter) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.EndDateFilter)))
                .Join(db.CustomerDeviceInfoes.Where(i=>i.Platform =="android"), dv => dv.CustomerId, cdi => cdi.CustomerId, (dv, cdi) => new { dv, cdi })
                .Count();

            model.IOSHomeCount = db.DailyVisitors.Where(i => i.ShopId == 0 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.StartDateFilter) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.EndDateFilter)))
                .Join(db.CustomerDeviceInfoes.Where(i => i.Platform == "ios"), dv => dv.CustomerId, cdi => cdi.CustomerId, (dv, cdi) => new { dv, cdi })
                .Count();
            return View(model);
        }
    }
}