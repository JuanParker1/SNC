using Razorpay.Api;
using ShopNow.Base;
using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class RefundController : Controller
    {
        private sncEntities db = new sncEntities();

        [AccessPolicy(PageCode = "SNCRP242")]
        public ActionResult Pending(RefundPendingViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Payments.Where(i => i.RefundAmount != 0 && i.RefundAmount != null && i.RefundStatus == 1 && i.PaymentMode == "Online Payment" &&
               (model.OrderDate != null ? DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(model.OrderDate.Value) : true) &&
               (model.ShopId != 0 ? i.ShopId == model.ShopId : true))
                .Join(db.PaymentsDatas, p => p.OrderNumber, pd => pd.OrderNumber, (p, pd) => new { p, pd })
                .Join(db.Customers, p => p.p.CustomerId, c => c.Id, (p, c) => new { p, c })
                .Select(i => new RefundPendingViewModel.ListItem
                {
                    ShopName = i.p.p.ShopName,
                    Amount = i.p.p.RefundAmount,
                    CustomerName = i.p.p.CustomerName,
                    OrderNumber = i.p.p.OrderNumber.ToString(),
                    PaymentId = i.p.pd.PaymentId,
                    Remark = i.p.p.RefundRemark,
                    CustomerPhoneNo = i.c.PhoneNumber
                }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCRH243")]
        public ActionResult History(RefundHistoryViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Payments
               .Where(i => ((model.StartDate != null ? DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.StartDate) : true) &&
               (model.EndDate != null ? DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.EndDate) : true)) && i.RefundAmount != null && i.RefundAmount > 0)
               .GroupJoin(db.RefundsDatas, p => p.OrderNumber, r => r.OrderNumber, (p, r) => new { p, r })
               .Join(db.Customers.Where(i => model.CustomerPhoneNo != null ? i.PhoneNumber == model.CustomerPhoneNo : true), p => p.p.CustomerId, c => c.Id, (p, c) => new { p, c })
               .AsEnumerable()
               .Select(i => new RefundHistoryViewModel.ListItem
               {
                   Amount = i.p.p.RefundAmount,
                   CustomerName = i.p.p.CustomerName,
                   CustomerPhoneNo = i.c.PhoneNumber,
                   OrderDate = i.p.p.DateEncoded,
                   OrderNumber = i.p.p.OrderNumber.ToString(),
                   PaymentId = i.p.r.Any() ? i.p.r.FirstOrDefault().Payment_Id : "",
                   RefundDate = i.p.r.Any() ? i.p.r.FirstOrDefault().DateEncoded : i.p.p.DateEncoded,
                   RefundId = i.p.r.Any() ? i.p.r.FirstOrDefault().RefundId : "N/A",
                   Remark = i.p.p.RefundRemark,
                   ShopName = i.p.p.ShopName
               }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCRSR244")]
        public ActionResult SendRefund(string paymentId, double amount, int orderNo)
        {
            Refund refund = new Refund();
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                                       SecurityProtocolType.Tls11 |
                                                       SecurityProtocolType.Tls12;
                string key = BaseClass.razorpaykey; //"rzp_live_PNoamKp52vzWvR";
                string secret = BaseClass.razorpaySecretkey;// "yychwOUOsYLsSn3XoNYvD1HY";

                RazorpayClient client = new RazorpayClient(key, secret);

                Razorpay.Api.Payment payment = client.Payment.Fetch(paymentId);

                //Full Refund
                //Refund refund = payment.Refund();

                //Partial Refund
                Dictionary<string, object> data = new Dictionary<string, object>();
                amount = amount * 100;
                data.Add("amount", amount.ToString());
                refund = payment.Refund(data);
            }
            catch (Exception ex)
            {
                string message= ex.Message;
                return RedirectToAction("Pending", "Refund",new { ErrorMessage = message });
            }

            //Refund add and update payment
            RefundsData newRefund = new RefundsData();
            newRefund.Amount = amount / 100;
            newRefund.Currency = refund["currency"];
            newRefund.Entity = refund["entity"];
            newRefund.OrderNumber = orderNo;
            newRefund.Payment_Id = paymentId;
            newRefund.RefundId = refund["id"];
            newRefund.DateEncoded = DateTime.Now;
            db.RefundsDatas.Add(newRefund);
            db.SaveChanges();

            if (newRefund != null)
            {
                var paymentUpdate = db.Payments.FirstOrDefault(i => i.OrderNumber == orderNo);
                paymentUpdate.RefundStatus = 0;
                db.Entry(paymentUpdate).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("History", "Refund");
        }
    }
}