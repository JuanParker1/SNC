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
                               MasterProductName = i.FirstOrDefault().oi.ProductName,
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

        //    //model.List = db.Carts.Where(i => i.CartStatus == 0 && DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(DateTime.Now))
        //    //    .OrderByDescending(i => i.DateUpdated)
        //    //    .GroupBy(i => i.OrderNo)
        //    //    //.AsEnumerable()
        //    //   .Select(i => new OrderMissedListViewModel.OrderMissedList
        //    //   {
        //    //       //SlNo = index + 1,
        //    //       Code = i.FirstOrDefault().Code,
        //    //       PaymentMode = i.FirstOrDefault().PaymentMode,
        //    //       DateEncoded = i.FirstOrDefault().DateEncoded,
        //    //       OrderNo = i.Key,
        //    //       HasPayment = false,
        //    //       PhoneNumber = i.FirstOrDefault().PhoneNumber,
        //    //       ShopName = i.FirstOrDefault().ShopName,
        //    //       Amount = i.Sum(j => j.Price)
        //    //   }).ToList();
        //    return View(model);
        //}
        //public ActionResult PaymentUpdate(string orderno)
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var model = new OrderMissedListViewModel();
        //    var cartlist = db.Carts.Where(i => i.OrderNo == orderno).ToList();
        //    var carts = db.Carts.FirstOrDefault(i => i.OrderNo == orderno);
        //    model.CartTotalPrice = cartlist.Sum(i => i.Price);
        //    model.OrderNo = orderno;
        //    model.ShopName = cartlist.Where(i => i.OrderNo == orderno).Select(i => i.ShopName).FirstOrDefault();
        //    var shopcode = cartlist.Where(i => i.OrderNo == orderno).Select(i => i.ShopCode).FirstOrDefault();
        //    var shop = db.Shops.FirstOrDefault(i => i.Code == shopcode);
        //    model.PackagingCharge = db.Bills.Where(i => i.ShopCode == shopcode && i.NameOfBill == 1 && i.Status == 0).Select(i => i.PackingCharge).FirstOrDefault();

        //    // Gross Delivery Charge
        //    var Distance = (((Math.Acos(Math.Sin((shop.Latitude * Math.PI / 180)) * Math.Sin((carts.Latitude * Math.PI / 180)) + Math.Cos((shop.Latitude * Math.PI / 180)) * Math.Cos((carts.Latitude * Math.PI / 180))
        //         * Math.Cos(((shop.Longitude - carts.Longitude) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344) / 1000;
        //    var deliverybill = db.Bills.Where(i => i.ShopCode == shopcode && i.NameOfBill == 0 && i.DeliveryRateSet == 0 && i.Status == 0).FirstOrDefault();
        //    if (Distance < 5)
        //    {
        //        model.GrossDeliveryCharge = deliverybill.DeliveryChargeKM;
        //    }
        //    else
        //    {
        //        var dist = Distance - 5;
        //        var amount = dist * deliverybill.DeliveryChargeOneKM;
        //        model.GrossDeliveryCharge = deliverybill.DeliveryChargeKM + amount;
        //    }
        //    model.ShopDeliveryDiscount = model.CartTotalPrice * (deliverybill.DeliveryChargeCustomer / 100);
        //    model.NetDeliveryCharge = model.GrossDeliveryCharge - model.ShopDeliveryDiscount;
        //    model.Distance = Distance.ToString("0.##");
        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult PaymentUpdate(OrderMissedListViewModel model)
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var cartlist = db.Carts.Where(i => i.OrderNo == model.OrderNo).ToList();
        //    var carts = db.Carts.FirstOrDefault(i => i.OrderNo == model.OrderNo);
        //    var payment = db.Payments.FirstOrDefault(i => i.OrderNo == model.OrderNo);
        //    int? varOrderno = Convert.ToInt32(model.OrderNo);
        //    var paymentData = db.paymentsDatas.FirstOrDefault(i => i.OrderNo == varOrderno);
        //    var shopcharge = db.ShopCharges.FirstOrDefault(i => i.OrderNo == model.OrderNo);
        //    var perOrderAmount = db.PlatFormCreditRates.Where(s => s.Status == 0).FirstOrDefault();
        //    if (cartlist != null)
        //    {
        //        foreach (var item in cartlist)
        //        {
        //            var cart = db.Carts.FirstOrDefault(i => i.Code == item.Code);
        //            if (cart != null)
        //            {
        //                cart.CartStatus = 2;
        //                cart.UpdatedBy = user.Name;
        //                cart.DateUpdated = DateTime.Now;
        //                db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
        //                db.SaveChanges();
        //            }
        //        }
        //    }
        //    if (payment == null)
        //    {
        //        Payment pay = new Payment();
        //        pay.Code = _generatedCode("PAY");
        //        pay.Amount = model.Amount;
        //        pay.PaymentMode = "Online Payment";
        //        pay.Key = "Razor";
        //        pay.Status = 0;
        //        pay.DateEncoded = carts.DateEncoded;
        //        pay.DateUpdated = carts.DateUpdated;
        //        pay.ReferenceCode = model.paymentId;
        //        pay.CustomerCode = carts.CustomerCode;
        //        pay.CustomerName = carts.CustomerName;
        //        pay.ShopCode = carts.ShopCode;
        //        pay.ShopName = carts.ShopName;
        //        pay.Address = carts.DeliveryAddress;
        //        pay.OriginalAmount = cartlist.Sum(i => i.Price);
        //        pay.GSTAmount = model.Amount;
        //        pay.Currency = "Rupees";
        //        pay.PaymentResult = "success";
        //        pay.Credits = "N/A";
        //        pay.UpdatedBy = carts.UpdatedBy;
        //        pay.CreatedBy = carts.CreatedBy;
        //        pay.OrderNo = carts.OrderNo;
        //        pay.PaymentCategoryType = 0;
        //        pay.CreditType = 2;
        //        pay.PackagingCharge = model.PackagingCharge;
        //        pay.Rateperorder = Convert.ToDouble(perOrderAmount.RatePerOrder);
        //        pay.refundStatus = 1;
        //        db.Payments.Add(pay);
        //        db.SaveChanges();
        //    }
        //    if (paymentData == null)
        //    {
        //        paymentsData pd = new paymentsData();
        //        pd.paymentId = model.paymentId;
        //        pd.OrderNo = Convert.ToInt32(model.OrderNo);
        //        pd.entity = "payment";
        //        pd.amount = Convert.ToDecimal(model.Amount);
        //        pd.currency = "INR";
        //        pd.status = 2;
        //        pd.order_id = model.order_id;
        //        pd.method = model.method;
        //        pd.fee = model.fee;
        //        pd.tax = model.tax;
        //        pd.DateEncoded = DateTime.Now;
        //        db.paymentsDatas.Add(pd);
        //        db.SaveChanges();
        //    }
        //    if (shopcharge != null)
        //    {
        //        ShopCharge sc = new ShopCharge();
        //        sc.Code = _generatedCode("SCH");
        //        sc.CustomerCode = carts.CustomerCode;
        //        sc.CustomerName = carts.CustomerName;
        //        sc.ShopCode = carts.ShopCode;
        //        sc.ShopName = carts.ShopName;
        //        sc.Status = 0;
        //        sc.DateEncoded = DateTime.Now;
        //        sc.DateUpdated = DateTime.Now;
        //        sc.CreatedBy = carts.CreatedBy;
        //        sc.UpdatedBy = carts.UpdatedBy;
        //        sc.OrderNo = model.OrderNo;
        //        sc.CartStatus = 2;
        //        sc.GrossDeliveryCharge = model.GrossDeliveryCharge;
        //        sc.ShopDeliveryDiscount = model.ShopDeliveryDiscount;
        //        sc.NetDeliveryCharge = model.NetDeliveryCharge;
        //        sc.Packingcharge = model.PackagingCharge;
        //        db.ShopCharges.Add(sc);
        //        db.SaveChanges();
        //    }

        //    return RedirectToAction("OrderMissed");
        //}

        //public ActionResult StatusUpdate(string orderno)
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var cartlist = db.Carts.Where(i => i.OrderNo == orderno).ToList();
        //    if (cartlist != null)
        //    {
        //        foreach (var item in cartlist)
        //        {
        //            var cart = db.Carts.FirstOrDefault(i => i.Code == item.Code);
        //            if (cart != null)
        //            {
        //                cart.CartStatus = 2;
        //                cart.UpdatedBy = user.Name;
        //                cart.DateUpdated = DateTime.Now;
        //                db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
        //                db.SaveChanges();
        //            }
        //        }
        //    }

        //    return RedirectToAction("OrderMissed");
        //}

        //public JsonResult RejectUpdate(string orderno)
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var cartlist = db.Carts.Where(i => i.OrderNo == orderno).ToList();
        //    if (cartlist != null)
        //    {
        //        foreach (var item in cartlist)
        //        {
        //            var cart = db.Carts.FirstOrDefault(i => i.Code == item.Code);
        //            if (cart != null)
        //            {
        //                cart.CartStatus = -1;
        //                cart.UpdatedBy = user.Name;
        //                cart.DateUpdated = DateTime.Now;
        //                db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
        //                db.SaveChanges();
        //            }
        //        }
        //    }
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ProductsUnMappedList(ProductUnMappedList model)
        {
            model.ListItems = db.Products.Where(i => i.MappedDate == null &&  i.MasterProductId != 0 && i.Status == 0 && i.ShopId != 0 && (model.ShopId != 0 ? i.ShopId == model.ShopId : true))
               .Join(db.MasterProducts, p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
               .Select(i => new ProductUnMappedList.ListItem
               {
                   Id = i.p.Id,
                   MenuPrice = i.p.MenuPrice,
                   Name = i.m.Name,
                   Quantity = i.p.Qty,
                   SellingPrice = i.p.Price,
                   ItemId = i.p.ItemId,
                   ShopName = i.p.ShopName,
                   Status = i.p.Status
               }).ToList();
            return View(model);
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