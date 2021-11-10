using AutoMapper;
using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class SupportController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        
        public SupportController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {

            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SNCSLP292")]
        public ActionResult LivePending()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new SupportViewModel();

            model.ShopAcceptanceCount = db.Orders.Where(i => i.Status == 2 && SqlFunctions.DateDiff("minute", i.DateUpdated, DateTime.Now) >= 5)
                   .AsEnumerable().Count();
            model.DeliveryAcceptanceCount = db.Orders.Join(db.DeliveryBoys, c => c.DeliveryBoyId, d => d.Id, (c, d) => new { c, d })
                      .Where(i => i.c.Status == 4 && i.d.isAssign == 1 && i.d.OnWork == 0 && SqlFunctions.DateDiff("minute", i.c.DateUpdated, DateTime.Now) >= 5)
                      .AsEnumerable().Count();
            model.ShopPickupCount = db.Orders.Join(db.DeliveryBoys, c => c.DeliveryBoyId, d => d.Id, (c, d) => new { c, d })
                    .Where(i => i.c.Status == 4 && i.d.isAssign == 1 && i.d.OnWork == 1 && SqlFunctions.DateDiff("minute", i.c.DateUpdated, DateTime.Now) >= 15)
                    .AsEnumerable().Count();
            model.CustomerDeliveryCount = db.Orders.Where(i => i.Status == 5 && SqlFunctions.DateDiff("minute", i.DateUpdated, DateTime.Now) >= 15)
                    .AsEnumerable().Count();
            model.OrderswithoutDeliveryboyCount = db.Orders.Where(i => i.Status == 3)
                    .AsEnumerable().Count();
            model.UnMappedCount = db.Products.Where(i => i.MasterProductId == 0)
                                      .Join(db.OrderItems, p => p.Id, c => c.ProductId, (p, c) => new { p, c }).AsEnumerable().GroupBy(i => i.c.ProductId).Count();
            DateTime start = new DateTime(2021, 10, 29);
            var count1 = db.Orders.Where(i => i.Status == 0 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(start)))
                .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p }).GroupBy(i => i.c.OrderNumber).Count();
            var count2 = db.Orders.Where(i => i.Status == 0 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(start)))
                        .GroupBy(i => i.OrderNumber).Count();
            model.OrderMissedCount = Math.Abs(count2 - count1);

            model.UnMappedCount = db.Products.Where(i => i.MasterProductId == 0)
                           .Join(db.OrderItems, p => p.Id, oi => oi.ProductId, (p, oi) => new { p, oi }).GroupBy(i => i.oi.Id).Count();
            model.ProductUnMappedCount = db.Products.Where(i => i.MappedDate == null && i.MasterProductId != 0 && i.Status == 0 && i.ShopId != 0).Count();
               

                        model.CustomerAadhaarVerifyCount = db.Customers.Where(i => i.AadharVerify == false && i.Status == 0 && i.ImageAadharPath != null && i.ImageAadharPath != "Rejected" && i.ImageAadharPath != "NULL").Count();
            model.ShopOnBoardingVerifyCount = db.Shops.Where(i => i.Status == 1).Count();
            model.DeliveryBoyVerifyCount = db.DeliveryBoys.Where(i => i.Status == 1).Count();
            model.BannerPendingCount = db.Banners.Where(i => i.Status == 1).Count();
            model.ShopCount = db.Shops.Where(i => i.Status == 0 || i.Status == 6).Count();
            model.CustomerCount = db.CustomerAddresses.GroupBy(a => a.CustomerId).Count();
            model.OrderCount = db.Orders.Where(i => i.Status != 7 && i.Status != 6 && i.Status != 0 && i.Status != -1).Count();
            model.DeliveryBoyLiveCount = db.DeliveryBoys.Where(i => i.Status == 0 && i.isAssign == 0 && i.OnWork == 0 && i.Active == 1).Count();
            model.RefundCount = db.Payments.Where(i => i.RefundAmount != 0 && i.RefundStatus == 1 && i.RefundAmount != null && i.PaymentMode == "Online Payment").Count();
            model.ShopLowCreditCount = db.ShopCredits.Where(i => i.PlatformCredit <= 200 || i.DeliveryCredit <= 250).Count();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCSLDA293")]
        public ActionResult LiveDeliveryboyAssignment(int shopId = 0)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DeliveryBoyAssignViewModel();

            model.DeliveryBoyList = db.DeliveryBoys.Where(i => i.isAssign == 0 && i.OnWork == 0 && i.Active == 1 && i.Status == 0)
                .Select(i => new DeliveryBoyAssignViewModel.DeliveryBoy
                {
                    Id = i.Id,
                    Name = i.Name
                }).ToList();

            model.List = db.Orders.Where(i => i.Status == 3 && (shopId != 0 ? i.ShopId == shopId : true))
               .Select(i => new DeliveryBoyAssignViewModel.AssignList
               {
                   Id = i.Id,
                   ShopId = i.ShopId,
                   ShopName = i.ShopName,
                   OrderNo = i.OrderNumber,
                   CartStatus = i.Status,
                   DateEncoded = i.DateEncoded
               }).ToList();

            return View(model);
        }

        public ActionResult UnMappedList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new UnMappedListViewModel();
            model.List = db.Products.Where(i => i.MasterProductId == 0)
                           .Join(db.OrderItems, p => p.Id, oi => oi.ProductId, (p, oi) => new { p, oi })
                           .OrderByDescending(i => i.p.DateUpdated).GroupBy(i => i.oi.Id)
                           .Select((i, index) => new UnMappedListViewModel.UnMappedList
                           {
                               SlNo = index + 1,
                               Id = i.FirstOrDefault().p.Id,
                               Name = i.FirstOrDefault().p.Name,
                               DateUpdated = i.FirstOrDefault().p.DateUpdated,
                              // MasterProductName = i.FirstOrDefault().oi.ProductName,
                               ShopName = i.FirstOrDefault().p.ShopName
                           }).ToList();
            return View(model);
        }

        //public ActionResult OrderMissed()
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var model = new OrderMissedListViewModel();
        //    DateTime start = new DateTime(2021, 10, 29);
        //    var list1 = db.Orders.Where(i => i.Status == 0 && DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(DateTime.Now)).OrderByDescending(i => i.DateUpdated)
        //        .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
        //         .AsEnumerable()
        //        .Select((i, index) => new OrderMissedListViewModel.OrderMissedList
        //        {
        //            SlNo = index + 1,
        //            Id = i.c.Id,
        //            PaymentMode = i.c.PaymentMode,
        //            DateEncoded = i.c.DateEncoded,
        //            OrderNo = i.c.OrderNumber,
        //            HasPayment = i.Any(),
        //            PhoneNumber = i.c.CustomerPhoneNumber,
        //            ShopName = i.c.ShopName,
        //            Amount = i.c.NetTotal.ToString("#,#.00")
        //        }).ToList();

        //    var list2 = db.Orders.Where(i => i.Status == 0 && DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(DateTime.Now)).OrderByDescending(i => i.DateUpdated)
        //        .AsEnumerable()
        //       .Select((i, index) => new OrderMissedListViewModel.OrderMissedList
        //       {
        //           SlNo = index + 1,
        //           Id = i.Id,
        //           PaymentMode = i.PaymentMode,
        //           DateEncoded = i.DateEncoded,
        //           OrderNo = i.OrderNumber,
        //           HasPayment = false,
        //           PhoneNumber = i.CustomerPhoneNumber,
        //           ShopName = i.ShopName,
        //           Amount = i.NetTotal.ToString("#,#.00")
        //       }).ToList();

        //    model.List = list1.Union(list2).ToList();

        //    return View(model);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PaymentUpdate(OrderMissedListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var cart = db.Orders.FirstOrDefault(i => i.OrderNumber == model.OrderNumber);
            var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == model.OrderNumber);
            var paymentData = db.PaymentsDatas.FirstOrDefault(i => i.OrderNumber == model.OrderNumber);
            var perOrderAmount = db.PlatFormCreditRates.Where(s => s.Status == 0).FirstOrDefault();

            if (cart != null)
            {
                cart.Status = 2;
                cart.UpdatedBy = user.Name;
                cart.DateUpdated = DateTime.Now;
                db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            
            if (payment == null)
            {
                Payment pay = new Payment();
                pay.Amount = model.Amount;
                pay.PaymentMode = "Online Payment";
                pay.Key = "Razor";
                pay.Status = 0;
                pay.DateEncoded = cart.DateEncoded;
                pay.DateUpdated = cart.DateUpdated;
                pay.ReferenceCode = model.PaymentId;
                pay.CustomerId = cart.CustomerId;
                pay.CustomerName = cart.CustomerName;
                pay.ShopId = cart.ShopId;
                pay.ShopName = cart.ShopName;
                pay.OriginalAmount = cart.TotalPrice;
                pay.GSTAmount = model.Amount;
                pay.Currency = "Rupees";
                pay.PaymentResult = "success";
                pay.Credits = "N/A";
                pay.UpdatedBy = cart.UpdatedBy;
                pay.CreatedBy = cart.CreatedBy;
                pay.OrderNumber = cart.OrderNumber;
                pay.PaymentCategoryType = 0;
                pay.CreditType = 2;
                pay.PackingCharge = model.PackingCharge;
                pay.RatePerOrder = Convert.ToDouble(perOrderAmount.RatePerOrder);
                pay.RefundStatus = 1;
                db.Payments.Add(pay);
                db.SaveChanges();
            }
            if (paymentData == null)
            {
                PaymentsData pd = new PaymentsData();
                pd.PaymentId = model.PaymentId;
                pd.OrderNumber = Convert.ToInt32(model.OrderNumber);
                pd.Entity = "payment";
                pd.Amount = Convert.ToDecimal(model.Amount);
                pd.Currency = "INR";
                pd.Status = 2;
                pd.Order_Id = model.Order_Id;
                pd.Method = model.Method;
                pd.Fee = model.Fee;
                pd.Tax = model.Tax;
                pd.DateEncoded = DateTime.Now;
                db.PaymentsDatas.Add(pd);
                db.SaveChanges();
            }

            return RedirectToAction("OrderMissed");
        }

        public ActionResult StatusUpdate(int orderno)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var cart = db.Orders.Where(i => i.OrderNumber == orderno).FirstOrDefault();
            if (cart != null)
            {
                cart.Status = 2;
                cart.UpdatedBy = user.Name;
                cart.DateUpdated = DateTime.Now;
                db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            
            return RedirectToAction("OrderMissed");
        }

        public JsonResult RejectUpdate(int orderno)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var cart = db.Orders.Where(i => i.OrderNumber == orderno).FirstOrDefault();
            if (cart != null)
            {
                cart.Status = -1;
                cart.UpdatedBy = user.Name;
                cart.DateUpdated = DateTime.Now;
                db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
                
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}