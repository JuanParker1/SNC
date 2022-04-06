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
    public class AccountsController : Controller
    {
        private sncEntities db = new sncEntities();

        [AccessPolicy(PageCode = "SNCABWR314")]
        public ActionResult BillWiseReport(AccountsBillWiseReportViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.MonthFilter = model.MonthFilter != 0 ? model.MonthFilter : DateTime.Now.Month;
            model.YearFilter = model.YearFilter != 0 ? model.YearFilter : DateTime.Now.Year;
            model.ListItems = db.Orders.Where(i => i.Status == 6 && i.DateEncoded.Month == model.MonthFilter && i.DateEncoded.Year == model.YearFilter)
                .Join(db.Payments, o => o.OrderNumber, p => p.OrderNumber, (o, p) => new { o, p })
                .Join(db.Shops, o => o.o.ShopId, s => s.Id, (o, s) => new { o, s })
                .AsEnumerable()
                .Select(i => new AccountsBillWiseReportViewModel.ListItem
                {
                    Date = i.o.o.DateEncoded,
                    OrderNumber = i.o.o.OrderNumber.ToString(),
                    ShopName = i.o.o.ShopName,
                    GSTNumber = i.s.GSTINNumber,
                    MenuPrice = i.o.o.TotalShopPrice == 0 ? i.o.o.TotalPrice : i.o.o.TotalShopPrice,
                    ShopBillAmount = Math.Round(i.o.o.TotalPrice, MidpointRounding.AwayFromZero),
                    PriceDifference = i.o.o.TotalShopPrice != 0 ? Math.Round(i.o.o.TotalPrice - i.o.o.TotalShopPrice, MidpointRounding.AwayFromZero) : 0,
                    CustomerPaidAmount = i.o.p.Amount,
                    RefundAmount = i.o.p.RefundAmount ?? 0,
                    FinalAmount = i.o.p.Amount - (i.o.p.RefundAmount ?? 0),
                    DeliveryAmountFromCustomer = i.o.o.NetDeliveryCharge,
                    DeliveryDiscount = i.o.o.ShopDeliveryDiscount,
                    TotalDeliveryCharge = i.o.o.DeliveryCharge,     
                    //DeliveryChargePaidToDeliveryBoy = (i.o.o.Distance <= 5) ? 20 : 20 + (i.o.o.IsPickupDrop == false ? ((i.o.o.Distance - 5) * 6) : i.o.o.Distance <= 15 ? i.o.o.DeliveryCharge - 50 : (60 + ((i.o.o.Distance - 15) * 8))),
                    DeliveryChargePaidToDeliveryBoy = ((i.o.o.Distance <= 5) ? 20 + ((i.o.o.DeliveryRatePercentage / 100) * 20) : 20 + ((i.o.o.DeliveryRatePercentage / 100) * 20) + (i.o.o.IsPickupDrop == false ? ((i.o.o.Distance - 5) * 6 + ((i.o.o.DeliveryRatePercentage / 100) * 6)) : i.o.o.Distance <= 15 ? i.o.o.DeliveryCharge - 50 : (60 + ((i.o.o.Distance - 15) * 8)))),
                    //DeliveryChargeProfit = i.o.o.DeliveryCharge - ((i.o.o.Distance <= 5) ? 20 : 20 + (i.o.o.IsPickupDrop == false ? ((i.o.o.Distance - 5) * 6) : i.o.o.Distance <= 15 ? i.o.o.DeliveryCharge - 50 : (60 + ((i.o.o.Distance - 15) * 8)))),
                    DeliveryChargeProfit = i.o.o.DeliveryCharge - ((i.o.o.Distance <= 5) ? 20 : 20 + (i.o.o.IsPickupDrop == false ? ((i.o.o.Distance - 5) * 6) : i.o.o.Distance <= 15 ? i.o.o.DeliveryCharge - 50 : (60 + ((i.o.o.Distance - 15) * 8)))),
                    AmountProfit = i.o.o.TotalShopPrice != 0 ? Math.Round(i.o.o.TotalPrice - i.o.o.TotalShopPrice, MidpointRounding.AwayFromZero) + i.o.o.Convinenientcharge : i.o.o.Convinenientcharge
                }).OrderBy(i => i.Date).ToList();

            int counter = 1;
            model.ListItems.ForEach(x => x.SNo = counter++);
            return View(model);
        }
    }
}