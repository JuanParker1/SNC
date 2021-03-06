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
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new SupportViewModel();

            //To take count for last 3days
            DateTime last3Date = DateTime.Now.AddDays(-3);

            model.ShopAcceptanceCount = db.Orders.Where(i => i.Status == 2 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(last3Date)) && (i.PickupDateTime != null ? SqlFunctions.DateDiff("minute", DateTime.Now, i.PickupDateTime) <= 35 : SqlFunctions.DateDiff("minute", i.DateUpdated, DateTime.Now) >= 5)).Count();

            model.DeliveryAcceptanceCount = db.Orders.Where(i => i.Status == 4 && i.DeliveryBoyId !=0 && i.DeliveryBoyOnWork==0 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(last3Date)) && SqlFunctions.DateDiff("minute", i.DateUpdated, DateTime.Now) >= 5).Count();

            model.ShopPickupCount = db.Orders.Where(i => i.Status == 4 && i.DeliveryBoyId != 0 && i.DeliveryBoyOnWork == 1 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(last3Date)) && SqlFunctions.DateDiff("minute", i.DateUpdated, DateTime.Now) >= 15).Count();

            model.CustomerDeliveryCount = db.Orders.Where(i => i.Status == 5 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(last3Date)) && SqlFunctions.DateDiff("minute", i.DateUpdated, DateTime.Now) >= 15).Count();

            model.OrderswithoutDeliveryboyCount = db.Orders.Where(i => i.Status == 3 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(last3Date))).Count();

            model.OrderCount = db.Orders.Where(i => i.Status != 7 && i.Status != 6 && i.Status != 0 && i.Status != -1 && i.Status != 9 && i.Status != 10 && (DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(last3Date)) && (i.PickupDateTime != null ? SqlFunctions.DateDiff("minute", DateTime.Now, i.PickupDateTime) <= 30 : true)).Count();

            //model.UnMappedCount = db.Products.Where(i => i.MasterProductId == 0)
            //                          .Join(db.OrderItems, p => p.Id, c => c.ProductId, (p, c) => new { p, c }).AsEnumerable().GroupBy(i => i.c.Id).Count();
           // DateTime start = new DateTime(2021, 10, 29);
            model.OrderMissedCount = db.Orders.Where(i => i.Status == 0 && DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(last3Date)).Count();

            model.UnMappedCount = db.Products.Where(i => i.MasterProductId == 0)
                           .Join(db.OrderItems, p => p.Id, oi => oi.ProductId, (p, oi) => new { p, oi }).GroupBy(i => i.p.Id).Count();
            model.ProductUnMappedCount = db.Products.Where(i => i.MappedDate != null && (i.MasterProductId == 0) && i.Status == 0 && i.ShopId != 0)
               .Join(db.MasterProducts, p => p.MasterProductId, m => m.Id, (p, m) => new { p, m }).Count();

            model.CustomerAadhaarVerifyCount = db.Customers.Where(i => i.AadharVerify == false && i.Status == 0 && i.ImageAadharPath != null && i.ImageAadharPath != "Rejected" && i.ImageAadharPath != "NULL").Count();
            model.ShopOnBoardingVerifyCount = db.Shops.Where(i => i.Status == 1).Count();
            model.DeliveryBoyVerifyCount = db.DeliveryBoys.Where(i => i.Status == 1).Count();
            model.BannerPendingCount = db.Banners.Where(i => i.Status == 1).Count();
           // model.ShopCount = db.Shops.Where(i => i.Status == 0 || i.Status == 6).Count();
            model.ShopCount = db.Shops.Where(i => i.Status == 0).Count();
            model.CustomerCount = db.CustomerAddresses.GroupBy(a => a.CustomerId).Count();

            model.DeliveryBoyLiveCount = db.DeliveryBoys.Where(i => i.Status == 0 && i.isAssign == 0 && i.OnWork == 0 && i.Active == 1).Count();
            model.RefundCount = db.Payments.Where(i => i.RefundAmount != 0 && i.RefundStatus == 1 && i.RefundAmount != null && i.PaymentMode == "Online Payment")
                .Join(db.PaymentsDatas, p=>p.OrderNumber,pd=>pd.OrderNumber,(p,pd)=>new { p,pd})
                .Count();
            model.ShopLowCreditCount = db.ShopCredits.Where(i => i.PlatformCredit <= 100 || i.DeliveryCredit <= 100)
                .Join(db.Shops.Where(i => i.IsTrail == false && i.Status == 0), sc => sc.CustomerId, s => s.CustomerId, (sc, s) => new { sc, s })
                .Count();
            model.CustomerPrescriptionCount = db.CustomerPrescriptions.Where(i => i.Status == 0).Count();
            model.CustomerGroceryUploadCount = db.CustomerGroceryUploads.Where(i => i.Status == 0).Count();

            model.ShopOnlineCount = db.Shops.Where(i => i.Status == 0 && i.IsOnline == true).Count();
            model.ShopOfflineCount = db.Shops.Where(i => i.Status == 0 && i.IsOnline == false).Count();
            var nglts = db.Products.Where(i => i.ShopId == 123 && i.Status == 0).Max(i => i.ItemTimeStamp);
            var palaits = db.Products.Where(i => i.ShopId == 203 && i.Status == 0).Max(i => i.ItemTimeStamp);
            var covaits = db.Products.Where(i => i.ShopId == 444 && i.Status == 0).Max(i => i.ItemTimeStamp);
            DateTime datengl = DateTime.ParseExact(nglts, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime datepalai = DateTime.ParseExact(palaits, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime datecovai = DateTime.ParseExact(covaits, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            model.JoyraNagercoil = datengl.ToString("dd-MM-yyyy hh:mm tt");
            model.JoyraPalai = datepalai.ToString("dd-MM-yyyy hh:mm tt");
            model.JoyraCovai = datecovai.ToString("dd-MM-yyyy hh:mm tt");
            //model.ServiceCount = db.Services.Where(i => i.Status == 0).Count();

            //Send PushNotification
            var pushNotificationList = db.PushNotifications.Where(i => i.Status == 1 && DbFunctions.TruncateTime(i.ScheduleDateTime.Value) == DbFunctions.TruncateTime(DateTime.Now)).ToList();
            if (pushNotificationList.Count() > 0)
            {
                foreach (var item in pushNotificationList)
                {
                    if (item.Type == 1 && item.ScheduleDateTime.Value.ToString("hh:mm") == DateTime.Now.ToString("hh:mm"))
                    {
                        var fcmTokenList = db.Customers.OrderBy(i => i.Id).Where(i => !string.IsNullOrEmpty(i.FcmTocken) && i.FcmTocken != "NULL").Select(i => i.FcmTocken).ToArray();
                        var count = Math.Ceiling((double)fcmTokenList.Count() / 1000);
                        for (int i = 0; i < count; i++)
                        {
                            Helpers.PushNotification.SendBulk(item.Description, item.Title, "SpecialOffer", item.ImageUrl, fcmTokenList.Skip(i * 1000).Take(1000).ToArray(), "tune2.caf");
                        }
                    }
                    if (item.Type == 2 && item.ScheduleDateTime.Value.ToString("hh:mm") == DateTime.Now.ToString("hh:mm"))
                    {
                        var fcmTokenList = db.Customers.Where(i => !string.IsNullOrEmpty(i.FcmTocken) && i.FcmTocken != "NULL" && item.District.Contains(i.DistrictName)).Select(i => i.FcmTocken).ToArray();
                        var count = Math.Ceiling((double)fcmTokenList.Count() / 1000);
                        for (int i = 0; i < count; i++)
                        {
                            Helpers.PushNotification.SendBulk(item.Description, item.Title, "SpecialOffer", item.ImageUrl, fcmTokenList.Skip(i * 1000).Take(1000).ToArray(), "tune2.caf");
                        }
                    }
                    if (item.Type == 3 && item.ScheduleDateTime.Value.ToString("hh:mm") == DateTime.Now.ToString("hh:mm"))
                    {
                        var latestVersion = db.AppDetails.FirstOrDefault().Version;
                        var fcmTokenList = db.Customers.Where(i => !string.IsNullOrEmpty(i.FcmTocken))
                            .Join(db.CustomerAppInfoes.Where(i => i.Version != latestVersion), c => c.Id, ca => ca.CustomerId, (c, ca) => new { c, ca })
                            .Select(i => i.c.FcmTocken).ToArray();
                        var count = Math.Ceiling((double)fcmTokenList.Count() / 1000);
                        for (int i = 0; i < count; i++)
                        {
                            Helpers.PushNotification.SendBulk(item.Description, item.Title, "SpecialOffer", item.ImageUrl, fcmTokenList.Skip(i * 1000).Take(1000).ToArray(), "tune2.caf");
                        }
                    }

                    if (item.ScheduleDateTime.Value.ToString("hh:mm") == DateTime.Now.ToString("hh:mm"))
                    {
                        var pushnotification = db.PushNotifications.FirstOrDefault(i => i.Id == item.Id);
                        pushnotification.Status = 0;
                        db.Entry(pushnotification).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCSSL304")]
        public ActionResult Signal()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [AccessPolicy(PageCode = "SNCSL311")]
        public ActionResult Live()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new SupportViewModel();
            
            DateTime last3Date = DateTime.Now.AddDays(-3);

            model.ShopAcceptanceCount = db.Orders.Where(i => i.Status == 2 /*&& SqlFunctions.DateDiff("minute", i.DateUpdated, DateTime.Now) >= 5*/)
                   .AsEnumerable().Count();

            model.DeliveryAcceptanceCount = db.Orders.Where(i => i.Status == 4 && i.DeliveryBoyId != 0 && i.DeliveryBoyOnWork == 0  /*&& SqlFunctions.DateDiff("minute", i.DateUpdated, DateTime.Now) >= 5*/)
               // .Join(db.DeliveryBoys.Where(i => i.isAssign == 1 && i.OnWork == 0), c => c.DeliveryBoyId, d => d.Id, (c, d) => new { c, d })
                      .AsEnumerable().Count();

            model.ShopPickupCount = db.Orders.Where(i => i.Status == 4 && i.DeliveryBoyId != 0 && i.DeliveryBoyOnWork == 1 /*&& SqlFunctions.DateDiff("minute", i.DateUpdated, DateTime.Now) >= 5*/)
               // .Join(db.DeliveryBoys.Where(i => i.isAssign == 1 && i.OnWork == 1), c => c.DeliveryBoyId, d => d.Id, (c, d) => new { c, d })
                      .AsEnumerable().Count();

            model.CustomerDeliveryCount = db.Orders.Where(i => i.Status == 5 /*&& SqlFunctions.DateDiff("minute", i.DateUpdated, DateTime.Now) >= 15*/)
                    .AsEnumerable().Count();

            model.OrderswithoutDeliveryboyCount = db.Orders.Where(i => i.Status == 3)
                    .AsEnumerable().Count();

            model.OrderCount = db.Orders.Where(i => i.Status != 7 && i.Status != 6 && i.Status != 0 && i.Status != -1 && i.Status != 9 && i.Status != 10).Count();

            model.UnMappedCount = db.Products.Where(i => i.MasterProductId == 0)
                                      .Join(db.OrderItems, p => p.Id, c => c.ProductId, (p, c) => new { p, c }).AsEnumerable().GroupBy(i => i.c.ProductId).Count();

            DateTime start = new DateTime(2021, 10, 29);
            model.OrderMissedCount = db.Orders.Where(i => i.Status == 0 /*&& DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(start)*/).Count();

            //model.UnMappedCount = db.Products.Where(i => i.MasterProductId == 0)
            //               .Join(db.OrderItems, p => p.Id, oi => oi.ProductId, (p, oi) => new { p, oi }).GroupBy(i => i.oi.Id).Count();
            model.ProductUnMappedCount = db.Products.Where(i => i.MappedDate != null && (i.MasterProductId == 0) && i.Status == 0 && i.ShopId != 0)
               .Join(db.MasterProducts, p => p.MasterProductId, m => m.Id, (p, m) => new { p, m }).Count();

            model.CustomerAadhaarVerifyCount = db.Customers.Where(i => i.AadharVerify == false && i.Status == 0 && i.ImageAadharPath != null && i.ImageAadharPath != "Rejected" && i.ImageAadharPath != "NULL").Count();
            model.ShopOnBoardingVerifyCount = db.Shops.Where(i => i.Status == 1).Count();
            model.DeliveryBoyVerifyCount = db.DeliveryBoys.Where(i => i.Status == 1).Count();
            model.BannerPendingCount = db.Banners.Where(i => i.Status == 1).Count();
            model.ShopCount = db.Shops.Where(i => i.Status == 0 || i.Status == 6).Count();
            model.CustomerCount = db.CustomerAddresses.GroupBy(a => a.CustomerId).Count();

            model.DeliveryBoyLiveCount = db.DeliveryBoys.Where(i => i.Status == 0 && i.isAssign == 0 && i.OnWork == 0 && i.Active == 1).Count();
            model.RefundCount = db.Payments.Where(i => i.RefundAmount != 0 && i.RefundStatus == 1 && i.RefundAmount != null && i.PaymentMode == "Online Payment").Count();
            model.ShopLowCreditCount =  db.ShopCredits.Where(i => i.PlatformCredit <= 100 || i.DeliveryCredit <= 100)
                .Join(db.Shops.Where(i => i.IsTrail == false && i.Status == 0), sc => sc.CustomerId, s => s.CustomerId, (sc, s) => new { sc, s })
                .Count();
            model.CustomerPrescriptionCount = db.CustomerPrescriptions.Where(i => i.Status == 0).Count();
            model.CustomerGroceryUploadCount = db.CustomerGroceryUploads.Where(i => i.Status == 0).Count();

            model.ShopOnlineCount = db.Shops.Where(i => i.Status == 0 && i.IsOnline == true).Count();
            model.ShopOfflineCount = db.Shops.Where(i => i.Status == 0 && i.IsOnline == false).Count();
            var nglts = db.Products.Where(i => i.ShopId == 123 && i.Status == 0).Max(i => i.ItemTimeStamp);
            var palaits = db.Products.Where(i => i.ShopId == 203 && i.Status == 0).Max(i => i.ItemTimeStamp);
            DateTime datengl = DateTime.ParseExact(nglts, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime datepalai = DateTime.ParseExact(palaits, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            model.JoyraNagercoil = datengl.ToString("dd-MM-yyyy hh:mm tt");
            model.JoyraPalai = datepalai.ToString("dd-MM-yyyy hh:mm tt");
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCSLDA293")]
        public ActionResult LiveDeliveryboyAssignment(DeliveryBoyAssignViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.DeliveryBoyList = db.DeliveryBoys.Where(i => i.isAssign == 0 && i.OnWork == 0 && i.Active == 1 && i.Status == 0)
                .Select(i => new DeliveryBoyAssignViewModel.DeliveryBoy
                {
                    Id = i.Id,
                    Name = i.Name
                }).ToList();

            model.List = db.Orders.Where(i => (i.Status == 3 || i.Status == 8) && i.ShopId != 0 && (model.ShopId != 0 ? i.ShopId == model.ShopId : true))
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

        [AccessPolicy(PageCode = "SNCSUML299")]
        public ActionResult UnMappedList(UnMappedListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.List = db.Products.Where(i => i.MasterProductId == 0 && i.ShopId != 0 && (model.ShopId != 0 ? i.ShopId == model.ShopId : true))
                           .Join(db.OrderItems, p => p.Id, oi => oi.ProductId, (p, oi) => new { p, oi })
                           .OrderByDescending(i => i.p.DateUpdated)
                           .AsEnumerable().GroupBy(i => i.p.Id)
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

        [AccessPolicy(PageCode = "SNCSOM300")]
        public ActionResult OrderMissed()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new OrderMissedListViewModel();
            model.List = db.Orders.Where(i => i.Status == 0).OrderByDescending(i => i.DateUpdated)
                .Select(i => new OrderMissedListViewModel.OrderMissedList
                {
                    Id = i.Id,
                    PaymentMode = i.PaymentMode,
                    DateEncoded = i.DateEncoded,
                    OrderNumber = i.OrderNumber,
                    PhoneNumber = i.CustomerPhoneNumber,
                    ShopName = i.ShopName,
                    TotalPrice = i.TotalPrice
                }).ToList();

            return View(model);
        }

        [AccessPolicy(PageCode = "SNCSPUML303")]
        public ActionResult ProductsUnMappedList(ProductUnMappedList model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Products.Where(i => i.MappedDate != null && (i.MasterProductId == 0) && i.Status == 0 && i.ShopId != 0 && (model.ShopId != 0 ? i.ShopId == model.ShopId : true))
               .Join(db.MasterProducts, p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
               .Select(i => new ProductUnMappedList.ListItem
               {
                   MappedDate = i.p.MappedDate,
                   Id = i.p.Id,
                   MenuPrice = i.p.MenuPrice,
                   Name = i.m.Name,
                   Quantity = i.p.Qty,
                   SellingPrice = i.p.Price,
                   ItemId = i.p.ItemId,
                   ShopId = i.p.ShopId,
                   ShopName = i.p.ShopName,
                   Status = i.p.Status
               }).ToList();
            model.CountListItems = model.ListItems
                .GroupBy(i => i.ShopId)
                .Select(i => new ProductUnMappedList.CountListItem
                {
                    Count = i.Count(),
                    ShopName = i.FirstOrDefault().ShopName,
                    ShopId = i.Key
                }).ToList();
            return View(model);
        }

        public ActionResult PaymentUpdate(int orderno)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var cart = db.Orders.FirstOrDefault(i => i.OrderNumber == orderno);
            var shop = db.Shops.FirstOrDefault(i => i.Id == cart.ShopId && i.Status == 0);
            var model = new OrderMissedListViewModel();
            model.OrderNumber = cart.OrderNumber;
            model.Amount = cart.NetTotal;
            model.TotalPrice = cart.TotalPrice;
            var billingCharge = db.BillingCharges.FirstOrDefault(i => i.ShopId == cart.ShopId && i.Status == 0);
            model.PackingCharge = billingCharge.PackingCharge; //db.Bills.Where(i => i.ShopId == cart.ShopId && i.Status == 0).Select(i => i.PackingCharge).FirstOrDefault();

            // Gross Delivery Charge

            var Distance = (((Math.Acos(Math.Sin((shop.Latitude * Math.PI / 180)) * Math.Sin((cart.Latitude * Math.PI / 180)) + Math.Cos((shop.Latitude * Math.PI / 180)) * Math.Cos((cart.Latitude * Math.PI / 180))
                 * Math.Cos(((shop.Longitude - cart.Longitude) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344) / 1000;
            var deliverybill = db.DeliveryCharges.FirstOrDefault(i => i.Type == shop.DeliveryType && i.TireType == shop.DeliveryTierType && i.Status == 0);

            if (deliverybill != null)
            {
                if (Distance < 5)
                {
                    model.DeliveryCharge = deliverybill.ChargeUpto5Km;
                }
                else
                {
                    var dist = Distance - 5;
                    var amount = dist * deliverybill.ChargePerKm;
                    model.DeliveryCharge = deliverybill.ChargeUpto5Km + amount;
                }
                model.ShopDeliveryDiscount = model.TotalPrice * (billingCharge.DeliveryDiscountPercentage / 100);
            }
            if (model.ShopDeliveryDiscount > model.DeliveryCharge)
            {
                model.NetDeliveryCharge = 0;
                model.ShopDeliveryDiscount = model.DeliveryCharge;
            }
            else
            {
                model.NetDeliveryCharge = model.DeliveryCharge - model.ShopDeliveryDiscount;
            }
            model.Distance = Distance.ToString("0.##");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PaymentUpdate(OrderMissedListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var order = db.Orders.FirstOrDefault(i => i.OrderNumber == model.OrderNumber);
            var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == model.OrderNumber);
            var paymentData = db.PaymentsDatas.FirstOrDefault(i => i.OrderNumber == model.OrderNumber);
            var perOrderAmount = db.PlatFormCreditRates.Where(s => s.Status == 0).FirstOrDefault();
            if (order != null)
            {
                order.Status = 2;
                order.UpdatedBy = user.Name;
                order.DateUpdated = DateTime.Now;
                order.RatePerOrder = perOrderAmount.RatePerOrder;
                order.NetDeliveryCharge = model.NetDeliveryCharge;
                order.DeliveryCharge = model.DeliveryCharge;
                order.ShopDeliveryDiscount = model.ShopDeliveryDiscount;
                order.Packingcharge = model.PackingCharge;
                order.NetTotal = model.Amount;
                order.PaymentModeType = model.PaymentType;
                order.PaymentMode = model.PaymentType == 1 ? "Online Payment" : "Cash On Hand";
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            if (model.PaymentType == 2)
            {
                if (payment == null)
                {
                    Payment pay = new Payment();
                    pay.CorporateID = null;
                    pay.Amount = order.NetTotal;
                    pay.PaymentMode = "Cash On Hand";
                    order.PaymentModeType = 2;
                    pay.Key = null;
                    pay.ReferenceCode = null;
                    pay.CustomerId = order.CustomerId;
                    pay.CustomerName = order.CustomerName;
                    pay.ShopId = order.ShopId;
                    pay.ShopName = order.ShopName;
                    pay.GSTINNumber = null;
                    pay.OriginalAmount = order.TotalPrice;
                    pay.GSTAmount = order.NetTotal;
                    pay.Currency = "Rupees";
                    pay.CountryName = null;
                    pay.PaymentResult = "pending";
                    pay.Credits = "N/A";
                    pay.OrderNumber = order.OrderNumber;
                    pay.PaymentCategoryType = 0;
                    pay.CreditType = 2;
                    pay.ConvenientCharge = order.Convinenientcharge;
                    pay.PackingCharge = order.Packingcharge;
                    pay.DeliveryCharge = order.DeliveryCharge;
                    pay.RatePerOrder = Convert.ToDouble(perOrderAmount.RatePerOrder);
                    pay.RefundStatus = 1;
                    pay.Status = 0;
                    pay.UpdatedBy = order.UpdatedBy;
                    pay.CreatedBy = order.CreatedBy;
                    pay.DateEncoded = order.DateEncoded;
                    pay.DateUpdated = order.DateUpdated;
                    db.Payments.Add(pay);
                    db.SaveChanges();
                }
            }
            else if (model.PaymentType == 1)
            {
                if (payment == null)
                {
                    Payment pay = new Payment();
                    pay.Amount = model.Amount;
                    pay.PaymentMode = "Online Payment";
                    pay.PaymentModeType = 1;
                    pay.Key = "Razor";
                    pay.Status = 0;
                    pay.DateEncoded = order.DateEncoded;
                    pay.DateUpdated = order.DateUpdated;
                    pay.ReferenceCode = model.PaymentId;
                    pay.CustomerId = order.CustomerId;
                    pay.CustomerName = order.CustomerName;
                    pay.ShopId = order.ShopId;
                    pay.ShopName = order.ShopName;
                    pay.OriginalAmount = order.TotalPrice;
                    pay.GSTAmount = model.Amount;
                    pay.Currency = "Rupees";
                    pay.PaymentResult = "success";
                    pay.Credits = "N/A";
                    pay.UpdatedBy = order.UpdatedBy;
                    pay.CreatedBy = order.CreatedBy;
                    pay.OrderNumber = order.OrderNumber;
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
                    if (!db.PaymentsDatas.Any(i => i.OrderNumber == model.OrderNumber))
                    {
                        db.PaymentsDatas.Add(pd);
                        db.SaveChanges();
                    }
                }
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

        //[AccessPolicy(PageCode = "SNCSCOHPU301")]
        //public ActionResult COHPaymentUpdate(int orderno)
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var cart = db.Orders.FirstOrDefault(i => i.OrderNumber == orderno);
        //    var shop = db.Shops.FirstOrDefault(i => i.Id == cart.ShopId && i.Status == 0);
        //    var model = new OrderMissedListViewModel();
        //    model.OrderNumber = cart.OrderNumber;
        //    model.Amount = cart.NetTotal;
        //    model.TotalPrice = cart.TotalPrice;
        //    model.PackingCharge = db.Bills.Where(i => i.ShopId == cart.ShopId && i.NameOfBill == 1 && i.Status == 0).Select(i => i.PackingCharge).FirstOrDefault();

        //    // Gross Delivery Charge
        //    var Distance = (((Math.Acos(Math.Sin((shop.Latitude * Math.PI / 180)) * Math.Sin((cart.Latitude * Math.PI / 180)) + Math.Cos((shop.Latitude * Math.PI / 180)) * Math.Cos((cart.Latitude * Math.PI / 180))
        //         * Math.Cos(((shop.Longitude - cart.Longitude) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344) / 1000;
        //    var deliverybill = db.Bills.Where(i => i.ShopId == cart.ShopId && i.NameOfBill == 0 && i.DeliveryRateSet == 0 && i.Status == 0).FirstOrDefault();
        //    if (deliverybill != null)
        //    {
        //        if (Distance < 5)
        //        {
        //            model.GrossDeliveryCharge = deliverybill.DeliveryChargeKM;
        //        }
        //        else
        //        {
        //            var dist = Distance - 5;
        //            var amount = dist * deliverybill.DeliveryChargeOneKM;
        //            model.GrossDeliveryCharge = deliverybill.DeliveryChargeKM + amount;
        //        }
        //        model.ShopDeliveryDiscount = model.TotalPrice * (deliverybill.DeliveryChargeCustomer / 100);
        //    }
        //    model.NetDeliveryCharge = model.GrossDeliveryCharge - model.ShopDeliveryDiscount;
        //    model.Distance = Distance.ToString("0.##");
        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessPolicy(PageCode = "SNCSCOHPU301")]
        //public ActionResult COHPaymentUpdate(OrderMissedListViewModel model)
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var order = db.Orders.FirstOrDefault(i => i.OrderNumber == model.OrderNumber);
        //    var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == model.OrderNumber);
        //    var perOrderAmount = db.PlatFormCreditRates.Where(s => s.Status == 0).FirstOrDefault();

        //    if (order != null)
        //    {
        //        order.Status = 2;
        //        order.UpdatedBy = user.Name;
        //        order.DateUpdated = DateTime.Now;
        //        order.RatePerOrder = Convert.ToDouble(perOrderAmount.RatePerOrder);
        //        order.NetDeliveryCharge = model.NetDeliveryCharge;
        //        order.DeliveryCharge = model.GrossDeliveryCharge;
        //        order.ShopDeliveryDiscount = model.ShopDeliveryDiscount;
        //        order.Packingcharge = model.PackingCharge;
        //        order.NetTotal = model.Amount;
        //        db.Entry(order).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    if (payment == null)
        //    {
        //        Payment pay = new Payment();
        //        pay.CorporateID = null;
        //        pay.Amount = order.NetTotal;
        //        pay.PaymentMode = "Cash On Hand";
        //        order.PaymentModeType = 2;
        //        pay.Key = null;
        //        pay.ReferenceCode = null;
        //        pay.CustomerId = order.CustomerId;
        //        pay.CustomerName = order.CustomerName;
        //        pay.ShopId = order.ShopId;
        //        pay.ShopName = order.ShopName;
        //        pay.GSTINNumber = null;
        //        pay.OriginalAmount = order.TotalPrice;
        //        pay.GSTAmount = order.NetTotal;
        //        pay.Currency = "Rupees";
        //        pay.CountryName = null;
        //        pay.PaymentResult = "pending";
        //        pay.Credits = "N/A";
        //        pay.OrderNumber = order.OrderNumber;
        //        pay.PaymentCategoryType = 0;
        //        pay.CreditType = 2;
        //        pay.ConvenientCharge = order.Convinenientcharge;
        //        pay.PackingCharge = order.Packingcharge;
        //        pay.DeliveryCharge = order.DeliveryCharge;
        //        pay.RatePerOrder = Convert.ToDouble(perOrderAmount.RatePerOrder);
        //        pay.RefundStatus = 1;
        //        pay.Status = 0;
        //        pay.UpdatedBy = order.UpdatedBy;
        //        pay.CreatedBy = order.CreatedBy;
        //        pay.DateEncoded = order.DateEncoded;
        //        pay.DateUpdated = order.DateUpdated;
        //        db.Payments.Add(pay);
        //        db.SaveChanges();
        //    }

        //    return RedirectToAction("OrderMissed");
        //}

        //[AccessPolicy(PageCode = "SNCSOPU302")]
        //public ActionResult OnlinePaymentUpdate(int orderno)
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var cart = db.Orders.FirstOrDefault(i => i.OrderNumber == orderno);
        //    var shop = db.Shops.FirstOrDefault(i => i.Id == cart.ShopId && i.Status == 0);
        //    var model = new OrderMissedListViewModel();
        //    model.OrderNumber = cart.OrderNumber;
        //    model.Amount = cart.NetTotal;
        //    model.TotalPrice = cart.TotalPrice;
        //    model.PackingCharge = db.Bills.Where(i => i.ShopId == cart.ShopId && i.NameOfBill == 1 && i.Status == 0).Select(i => i.PackingCharge).FirstOrDefault();

        //    // Gross Delivery Charge
        //    var Distance = (((Math.Acos(Math.Sin((shop.Latitude * Math.PI / 180)) * Math.Sin((cart.Latitude * Math.PI / 180)) + Math.Cos((shop.Latitude * Math.PI / 180)) * Math.Cos((cart.Latitude * Math.PI / 180))
        //         * Math.Cos(((shop.Longitude - cart.Longitude) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344) / 1000;
        //    var deliverybill = db.Bills.Where(i => i.ShopId == cart.ShopId && i.NameOfBill == 0 && i.DeliveryRateSet == 0 && i.Status == 0).FirstOrDefault();
        //    if (deliverybill != null)
        //    {
        //        if (Distance < 5)
        //        {
        //            model.GrossDeliveryCharge = deliverybill.DeliveryChargeKM;
        //        }
        //        else
        //        {
        //            var dist = Distance - 5;
        //            var amount = dist * deliverybill.DeliveryChargeOneKM;
        //            model.GrossDeliveryCharge = deliverybill.DeliveryChargeKM + amount;
        //        }
        //        model.ShopDeliveryDiscount = model.TotalPrice * (deliverybill.DeliveryChargeCustomer / 100);
        //    }
        //    model.NetDeliveryCharge = model.GrossDeliveryCharge - model.ShopDeliveryDiscount;
        //    model.Distance = Distance.ToString("0.##");
        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessPolicy(PageCode = "SNCSOPU302")]
        //public ActionResult OnlinePaymentUpdate(OrderMissedListViewModel model)
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var order = db.Orders.FirstOrDefault(i => i.OrderNumber == model.OrderNumber);
        //    var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == model.OrderNumber);
        //    var paymentData = db.PaymentsDatas.FirstOrDefault(i => i.OrderNumber == model.OrderNumber);
        //    var perOrderAmount = db.PlatFormCreditRates.Where(s => s.Status == 0).FirstOrDefault();
        //    if (order != null)
        //    {
        //        order.Status = 2;
        //        order.UpdatedBy = user.Name;
        //        order.DateUpdated = DateTime.Now;
        //        order.RatePerOrder = Convert.ToDouble(perOrderAmount.RatePerOrder);
        //        order.NetDeliveryCharge = model.NetDeliveryCharge;
        //        order.DeliveryCharge = model.GrossDeliveryCharge;
        //        order.ShopDeliveryDiscount = model.ShopDeliveryDiscount;
        //        order.Packingcharge = model.PackingCharge;
        //        order.NetTotal = model.Amount;
        //        db.Entry(order).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    if (payment == null)
        //    {
        //        Payment pay = new Payment();
        //        pay.Amount = model.Amount;
        //        pay.PaymentMode = "Online Payment";
        //        pay.PaymentModeType = 1;
        //        pay.Key = "Razor";
        //        pay.Status = 0;
        //        pay.DateEncoded = order.DateEncoded;
        //        pay.DateUpdated = order.DateUpdated;
        //        pay.ReferenceCode = model.PaymentId;
        //        pay.CustomerId = order.CustomerId;
        //        pay.CustomerName = order.CustomerName;
        //        pay.ShopId = order.ShopId;
        //        pay.ShopName = order.ShopName;
        //        pay.OriginalAmount = order.TotalPrice;
        //        pay.GSTAmount = model.Amount;
        //        pay.Currency = "Rupees";
        //        pay.PaymentResult = "success";
        //        pay.Credits = "N/A";
        //        pay.UpdatedBy = order.UpdatedBy;
        //        pay.CreatedBy = order.CreatedBy;
        //        pay.OrderNumber = order.OrderNumber;
        //        pay.PaymentCategoryType = 0;
        //        pay.CreditType = 2;
        //        pay.PackingCharge = model.PackingCharge;
        //        pay.RatePerOrder = Convert.ToDouble(perOrderAmount.RatePerOrder);
        //        pay.RefundStatus = 1;
        //        db.Payments.Add(pay);
        //        db.SaveChanges();
        //    }
        //    if (paymentData == null)
        //    {
        //        PaymentsData pd = new PaymentsData();
        //        pd.PaymentId = model.PaymentId;
        //        pd.OrderNumber = Convert.ToInt32(model.OrderNumber);
        //        pd.Entity = "payment";
        //        pd.Amount = Convert.ToDecimal(model.Amount);
        //        pd.Currency = "INR";
        //        pd.Status = 2;
        //        pd.Order_Id = model.Order_Id;
        //        pd.Method = model.Method;
        //        pd.Fee = model.Fee;
        //        pd.Tax = model.Tax;
        //        pd.DateEncoded = DateTime.Now;
        //        db.PaymentsDatas.Add(pd);
        //        db.SaveChanges();
        //    }

        //    return RedirectToAction("OrderMissed");
        //}

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