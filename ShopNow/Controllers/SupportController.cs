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
            model.List = db.Products.Where(i =>  i.MasterProductId == 0)
                           .Join(db.OrderItems, p => p.Id, c => c.ProductId, (p, c) => new { p, c })
                           .OrderByDescending(i => i.p.DateUpdated).AsEnumerable().GroupBy(i => i.c.ProductId)
                           .Select((i, index) => new UnMappedListViewModel.UnMappedList
                           {
                               SlNo = index + 1,
                               Id = i.FirstOrDefault().p.Id,
                               Name = i.FirstOrDefault().p.Name,
                               DateUpdated = i.FirstOrDefault().p.DateUpdated,
                               ShopName = i.FirstOrDefault().p.ShopName
                           }).ToList();
            return View(model);
        }

        public ActionResult OrderMissed()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new OrderMissedListViewModel();
            DateTime start = new DateTime(2021, 10, 29);
            var list1 = db.Orders.Where(i => i.Status == 0 && DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(start)).OrderByDescending(i => i.DateUpdated)
                .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                 .AsEnumerable()
                 .GroupBy(i => i.c.OrderNumber)
                .Select((i, index) => new OrderMissedListViewModel.OrderMissedList
                {
                    SlNo = index + 1,
                    Id = i.FirstOrDefault().c.Id,
                    DateEncoded = i.FirstOrDefault().c.DateEncoded,
                    OrderNumber = i.FirstOrDefault().c.OrderNumber,
                    PaymentMode = i.FirstOrDefault().p.PaymentMode,
                    HasPayment = i.Any(),
                    PhoneNumber = i.FirstOrDefault().c.CustomerPhoneNumber,
                    ShopName = i.FirstOrDefault().c.ShopName,
                    Amount = i.FirstOrDefault().c.TotalPrice.ToString("#,#.00")
                }).ToList();

            var list2 = db.Orders.Where(i => i.Status == 0 && DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(start)).OrderByDescending(i => i.DateUpdated)
                .AsEnumerable()
                .GroupBy(i => i.OrderNumber)
               .Select((i, index) => new OrderMissedListViewModel.OrderMissedList
               {
                   SlNo = index + 1,
                   Id = i.FirstOrDefault().Id,
                   PaymentMode = "Online Payment",
                   DateEncoded = i.FirstOrDefault().DateEncoded,
                   OrderNumber = i.Key,
                   HasPayment = false,
                   PhoneNumber = i.FirstOrDefault().CustomerPhoneNumber,
                   ShopName = i.FirstOrDefault().ShopName,
                   Amount = i.FirstOrDefault().TotalPrice.ToString("#,#.00")
               }).ToList();

            model.List = list1.Union(list2).ToList();
            return View(model);
        }
        public ActionResult PaymentUpdate(int orderno)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new OrderMissedListViewModel();
            var carts = db.Orders.FirstOrDefault(i => i.OrderNumber == orderno);
            model.CartTotalPrice = carts.TotalPrice;
            model.OrderNumber = orderno;
            model.ShopName = carts.ShopName;
            var shop = db.Shops.FirstOrDefault(i => i.Id == carts.ShopId);
            model.PackingCharge = db.Bills.Where(i => i.ShopId == carts.ShopId && i.NameOfBill == 1 && i.Status == 0).Select(i => i.PackingCharge).FirstOrDefault();

            // Gross Delivery Charge
            var Distance = (((Math.Acos(Math.Sin((shop.Latitude * Math.PI / 180)) * Math.Sin((carts.Latitude * Math.PI / 180)) + Math.Cos((shop.Latitude * Math.PI / 180)) * Math.Cos((carts.Latitude * Math.PI / 180))
                 * Math.Cos(((shop.Longitude - carts.Longitude) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344) / 1000;
            var deliverybill = db.Bills.Where(i => i.ShopId == carts.ShopId && i.NameOfBill == 0 && i.DeliveryRateSet == 0 && i.Status == 0).FirstOrDefault();
            if (Distance < 5)
            {
                model.GrossDeliveryCharge = deliverybill.DeliveryChargeKM;
            }
            else
            {
                var dist = Distance - 5;
                var amount = dist * deliverybill.DeliveryChargeOneKM;
                model.GrossDeliveryCharge = deliverybill.DeliveryChargeKM + amount;
            }
            model.ShopDeliveryDiscount = model.CartTotalPrice * (deliverybill.DeliveryChargeCustomer / 100);
            model.NetDeliveryCharge = model.GrossDeliveryCharge - model.ShopDeliveryDiscount;
            model.Distance = Distance.ToString("0.##");
            return View(model);
        }

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