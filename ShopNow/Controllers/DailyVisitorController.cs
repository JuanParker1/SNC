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
            model.ListItems = db.DailyVisitors.Where(i => DbFunctions.TruncateTime(i.DateUpdated) == DbFunctions.TruncateTime(model.DateFilter))
                .GroupBy(i => i.ShopId)
                .Select(i => new DailyVisitorListViewModel.ListItem
                {
                    Count = i.Count(),
                    //CustomerId = i.Key.CustomerId,
                    DateUpdated = i.OrderByDescending(a=>a.DateUpdated).FirstOrDefault().DateUpdated,
                    ShopId = i.Key,
                    ShopName = i.OrderByDescending(a => a.DateUpdated).FirstOrDefault().ShopName
                }).OrderByDescending(i => i.Count).ToList();
            return View(model);
        }
    }
}