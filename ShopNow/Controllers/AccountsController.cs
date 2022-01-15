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

        [AccessPolicy(PageCode = "")]
        public ActionResult BillWiseReport()
        {
            //var user = ((Helpers.Sessions.User)Session["USER"]);
            //ViewBag.Name = user.Name;
            var model = new AccountsBillWiseReportViewModel();
            model.ListItems = db.Orders.Where(i=>i.Status==6 && i.DateEncoded.Month == DateTime.Now.Month && i.DateEncoded.Year == DateTime.Now.Year)
                .Join(db.Payments,o=>o.OrderNumber,p=>p.OrderNumber,(o,p)=>new { o,p})
                .Join(db.Shops,o=>o.o.ShopId,s=>s.Id,(o,s)=>new { o,s})
                .Select((i,index) => new AccountsBillWiseReportViewModel.ListItem
                {
                    SNo = index +1,
                    Date = i.o.o.DateEncoded,
                    OrderNumber = i.o.o.OrderNumber,
                    ShopName = i.o.o.ShopName,
                    GSTNumber = i.s.GSTINNumber,
                    MenuPrice = i.o.o.TotalMRPPrice,
                    ShopBillAmount = i.o.o.TotalPrice,
                    PriceDifference = i.o.o.TotalMRPPrice - i.o.o.TotalPrice,
                    CustomerPaidAmount = i.o.p.Amount,
                    RefundAmount = i.o.p.RefundAmount ??0,
                    FinalAmount = i.o.p.Amount - i.o.p.RefundAmount??0,
                    DeliveryAmountFromCustomer = i.o.o.NetDeliveryCharge,
                    DeliveryDiscount = i.o.o.ShopDeliveryDiscount,
                    TotalDeliveryCharge = i.o.o.DeliveryCharge,
                    DeliveryChargePaidToDeliveryBoy = 20,
                    DeliveryChargeProfit = i.o.o.DeliveryCharge - 20,
                    AmountProfit = i.o.o.TotalMRPPrice - i.o.o.TotalPrice
                }).ToList();
            return View(model.ListItems);
        }
    }
}