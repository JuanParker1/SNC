using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class WalletController : Controller
    {
        private sncEntities db = new sncEntities();
        public ActionResult Index()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        public ActionResult SendWalletAmount(WalletSendAmountViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            switch (model.CustomerGroup)
            {
                case 1:
                    var allMedicalOrders = db.Orders.Where(i => i.Status == 6).GroupBy(i => new { i.ShopId, i.CustomerId }).Select(i => new { ShopId = i.FirstOrDefault().ShopId, CustomerId = i.FirstOrDefault().CustomerId })
                .Join(db.Shops.Where(i => i.ShopCategoryId == 4), o => o.ShopId, s => s.Id, (o, s) => new { o, s })
                .Select(i => new Order { CustomerId = i.o.CustomerId, ShopId = i.s.Id }).ToList();

                    //var medicalCustomer = db.Orders.Where(a => !shops.Select(b => b.ShopId).Contains(a.ShopId) && a.Status == 6).Select(i => new { CustomerId = i.CustomerId })
                    //    .ToList();
                    break;
                default:
                    break;
            }
            return View();
        }

    }
}