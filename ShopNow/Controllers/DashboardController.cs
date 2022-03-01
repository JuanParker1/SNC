using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    // [Authorize]
    public class DashboardController : Controller
    {
        private sncEntities _db = new sncEntities();

        [AccessPolicy(PageCode = "SNCDI112")]
        public ActionResult Index(DashboardIndexViewModel model)
        {
            ViewBag.Name = ((ShopNow.Helpers.Sessions.User)System.Web.HttpContext.Current.Session["USER"]).Name;
            ViewBag.customer = _db.Customers.Where(i => i.Status == 0).Count();
            ViewBag.Shops = _db.Shops.Where(i => i.Status == 0).Count();
            ViewBag.Delivery = _db.DeliveryBoys.Where(i => i.Status == 0).Count();
            ViewBag.Order = _db.Orders.Where(i => i.Status == 0 && i.OrderNumber != 0 && i.Status != 7 && i.Status != 6 && i.Status != 9 && i.Status != 10).Count();
            
            model.MonthFilter = model.MonthFilter != 0 ? model.MonthFilter : DateTime.Now.Month;
            model.YearFilter = model.YearFilter != 0 ? model.YearFilter : DateTime.Now.Year;

            var orders = _db.Orders.Where(i => i.DateEncoded.Year == model.YearFilter && i.Status == 6).Select(i => new { Id = i.Id, ShopId = i.ShopId, DateEncoded=i.DateEncoded }).ToList();
            model.OrderJanCount = orders.Where(i => i.DateEncoded.Month == 1).Count();
            model.OrderFebCount = orders.Where(i => i.DateEncoded.Month == 2).Count();
            model.OrderMarCount = orders.Where(i => i.DateEncoded.Month == 3).Count();
            model.OrderAprCount = orders.Where(i => i.DateEncoded.Month == 4).Count();
            model.OrderMayCount = orders.Where(i => i.DateEncoded.Month == 5).Count();
            model.OrderJunCount = orders.Where(i => i.DateEncoded.Month == 6).Count();
            model.OrderJulCount = orders.Where(i => i.DateEncoded.Month == 7).Count();
            model.OrderAugCount = orders.Where(i => i.DateEncoded.Month == 8).Count();
            model.OrderSepCount = orders.Where(i => i.DateEncoded.Month == 9).Count();
            model.OrderOctCount = orders.Where(i => i.DateEncoded.Month == 10).Count();
            model.OrderNovCount = orders.Where(i => i.DateEncoded.Month == 11).Count();
            model.OrderDecCount = orders.Where(i => i.DateEncoded.Month == 12).Count();

            var shopOrderCount = orders
                .Join(_db.Shops, o => o.ShopId, s => s.Id, (o, s) => new { o, s })
                .Where(i => i.o.DateEncoded.Month == model.MonthFilter)
                .ToList();

            model.OrderRestaurantCount = shopOrderCount.Where(i => i.s.ShopCategoryId == 1).Count();
            model.OrderMeatAndVegCount = shopOrderCount.Where(i => i.s.ShopCategoryId == 2).Count();
            model.OrderSupermarketCount = shopOrderCount.Where(i => i.s.ShopCategoryId == 3).Count();
            model.OrderMedicalCount = shopOrderCount.Where(i => i.s.ShopCategoryId == 4).Count();
            return View(model);
        }

        public JsonResult GetLiveCount()
        {
            return Json(_db.Orders.Where(i => i.Status == 2).Count(), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}