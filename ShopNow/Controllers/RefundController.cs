using Razorpay.Api;
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
        private ShopnowchatEntities db = new ShopnowchatEntities();

        [AccessPolicy(PageCode = "SHNRFNP001")]
        public ActionResult Pending(RefundPendingViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Payments.Where(i => (i.refundAmount != 0 && i.refundStatus == 1 &&i.refundAmount != null) && i.PaymentMode == "Online Payment" &&
               (model.OrderDate != null ? DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(model.OrderDate.Value) : true) &&
              (model.ShopId != 0 ? i.ShopId == model.ShopId : true))
                .Join(db.PaymentsDatas, p => p.OrderNo, pd => pd.OrderNo.ToString(), (p, pd) => new { p, pd })
                .Join(db.Customers, p => p.p.CustomerId, c => c.Id, (p, c) => new { p, c })
                .Select(i => new RefundPendingViewModel.ListItem
                {
                    Amount = i.p.p.refundAmount,
                    CustomerName = i.p.p.CustomerName,
                    OrderNo = i.p.p.OrderNo,
                    PaymentId = i.p.pd.paymentId,
                    Remark = i.p.p.refundRemark,
                    CustomerPhoneNo = i.c.PhoneNumber
                }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNRFNH002")]
        public ActionResult History(RefundHistoryViewModel model)
        {

            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
           
            model.ListItems = db.Payments
               .Where(i => ((model.StartDate != null ? DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.StartDate) : true) &&
               (model.EndDate != null ? DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.EndDate) : true)) && i.refundAmount != null && i.refundAmount > 0)
               .GroupJoin(db.RefundsDatas, p => p.OrderNo, r => r.orderNo.ToString(), (p, r) => new { p, r })
               .Join(db.Customers.Where(i => model.CustomerPhoneNo != null ? i.PhoneNumber == model.CustomerPhoneNo : true), p => p.p.CustomerCode, c => c.Code, (p, c) => new { p, c })
               .AsEnumerable()
               .Select(i => new RefundHistoryViewModel.ListItem
               {
                   Amount = i.p.p.refundAmount,
                   CustomerName = i.p.p.CustomerName,
                   CustomerPhoneNo = i.c.PhoneNumber,
                   OrderDate = i.p.p.DateEncoded,
                   OrderNo = Convert.ToInt32(i.p.p.OrderNo),
                   PaymentId = i.p.r.Any() ? i.p.r.FirstOrDefault().payment_id : "",
                   RefundDate = i.p.r.Any() ? i.p.r.FirstOrDefault().DateEncoded : i.p.p.DateEncoded,
                   RefundId = i.p.r.Any() ? i.p.r.FirstOrDefault().refundid : "N/A",
                   Remark = i.p.p.refundRemark,
                   ShopName = i.p.p.ShopName
               }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNRFNS003")]
        public ActionResult SendRefund(string paymentId, double amount, int orderNo)
        {
            Refund refund = new Refund();
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                                       SecurityProtocolType.Tls11 |
                                                       SecurityProtocolType.Tls12;
                string key = "rzp_live_PNoamKp52vzWvR";
                string secret = "yychwOUOsYLsSn3XoNYvD1HY";

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
            newRefund.amount = amount / 100;
            newRefund.currency = refund["currency"];
            newRefund.enity = refund["entity"];
            newRefund.orderNo = orderNo;
            newRefund.payment_id = paymentId;
            newRefund.refundid = refund["id"];
            newRefund.DateEncoded = DateTime.Now;
            db.RefundsDatas.Add(newRefund);
            db.SaveChanges();

            if (newRefund != null)
            {
                var paymentUpdate = db.Payments.FirstOrDefault(i => i.OrderNo == orderNo);
                paymentUpdate.refundStatus = 0;
                db.Entry(paymentUpdate).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("History", "Refund");
        }
    }
}