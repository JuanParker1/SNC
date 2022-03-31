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

        public ActionResult List(DailyVisitorListViewModel model)
        {
            model.DateFilter = model.DateFilter == null ? DateTime.Now : model.DateFilter;
            model.ListItems = db.DailyVisitors.Where(i => DbFunctions.TruncateTime(i.DateUpdated) == DbFunctions.TruncateTime(model.DateFilter)).GroupBy(i => i.ShopId)
                .GroupJoin(db.Orders.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(model.DateFilter) && i.Status==6), dv => dv.FirstOrDefault().ShopId, o => o.ShopId, (dv, o) => new { dv, o })
                
                .Select(i => new DailyVisitorListViewModel.ListItem
                {
                    Count = i.dv.Count(),
                    //CustomerId = i.Key.CustomerId,
                    DateUpdated = i.dv.OrderByDescending(a => a.DateUpdated).FirstOrDefault().DateUpdated,
                    ShopId = i.dv.Key,
                    ShopName = i.dv.OrderByDescending(a => a.DateUpdated).FirstOrDefault().ShopName,
                    OrderCount = i.o.Count()
                }).OrderByDescending(i => i.Count).ToList();
            return View(model);
        }
    }
}