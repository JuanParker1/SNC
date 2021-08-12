using AutoMapper;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class CartController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private static string _generatedDelivaryId
        {
            get
            {
                return ShopNow.Helpers.DRC.GenerateDelivaryBoy();
            }
        }

        public CartController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<Order, CartListViewModel.CartList>();
                config.CreateMap<Order, CartDetailsViewModel>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNCARL001")]
        public ActionResult List(int shopId = 0)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dt = DateTime.Now;
            var model = new CartListViewModel();
            model.List = db.Orders.Where(i => (shopId != 0 ? i.ShopId == shopId : true) && i.Status != 6 && i.Status != 7 && i.Status != 0)
           // .AsEnumerable()
            .Select(i => new CartListViewModel.CartList
            {
                Id = i.Id,
                ShopName = i.ShopName,
                OrderNumber = i.OrderNumber,
                DeliveryAddress = i.DeliveryAddress,
                PhoneNumber = i.ShopPhoneNumber,
                CartStatus = i.Status,
                DeliveryBoyName = i.DeliveryBoyName ?? "N/A",
                DateEncoded = i.DateEncoded,
               // Date = i.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt")
            }).OrderBy(i => i.CartStatus).OrderByDescending(i => i.DateEncoded).ToList();
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCARPE016")]
        public ActionResult Pending(int shopId = 0)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartListViewModel();
            model.List = db.Orders.Where(i => (shopId !=0 ? i.ShopId == shopId : true) && i.Status == 2)
                           .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                          // .AsEnumerable()
                           .Select(i => new CartListViewModel.CartList
                           {
                              Id = i.c.Id,
                              ShopName = i.c.ShopName,
                              OrderNumber = i.c.OrderNumber,
                              DeliveryAddress = i.c.DeliveryAddress,
                              PhoneNumber = i.c.ShopOwnerPhoneNumber,
                              CartStatus = i.c.Status,
                              DeliveryBoyName = i.c.DeliveryBoyName,
                              DateEncoded = i.c.DateEncoded,
                            //  Date = i.c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt"),
                              Price = i.c.TotalPrice,
                              RefundAmount = i.p.RefundAmount ?? 0,
                              RefundRemark = i.p.RefundRemark ?? "",
                              PaymentMode = i.p.PaymentMode,
                    }).OrderByDescending(i => i.DateEncoded).ToList();
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCAROP017")]
        public ActionResult OrderPrepared(int shopId = 0) //Order Processing
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartListViewModel();
            model.List = db.Orders.Where(i => (shopId != 0 ? i.ShopId == shopId : true) && (i.Status == 3 || i.Status == 4))
                .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                .AsEnumerable()
                .Select(i => new CartListViewModel.CartList
                {
                    Id = i.c.Id,
                    ShopName = i.c.ShopName,
                    OrderNumber = i.c.OrderNumber,
                    DeliveryAddress = i.c.DeliveryAddress,
                    PhoneNumber = i.c.ShopPhoneNumber,
                    CartStatus = i.c.Status,
                    DeliveryBoyName = i.c.DeliveryBoyName,
                    DateEncoded = i.c.DateEncoded,
                    Date = i.c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt"),
                    RefundAmount = i.p.RefundAmount ?? 0,
                    RefundRemark = i.p.RefundRemark ?? "",
                    PaymentMode = i.p.PaymentMode,
                    DeliveryPhoneNumber = i.c.DeliveryBoyPhoneNumber ?? "Not Assigned",
                    Price = i.p.Amount - (i.p.RefundAmount ?? 0)
                }).OrderByDescending(i => i.DateEncoded).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCARDA018")]
        public ActionResult DeliveryAgentAssigned(int shopId = 0)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartListViewModel();

                model.List = db.Orders
                    .Join(db.DeliveryBoys, c => c.DeliveryBoyId, d => d.Id, (c, d) => new { c, d })
                    .Join(db.Payments, c => c.c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                    .Where(i =>(shopId!=0? i.c.c.ShopId == shopId:true) && i.c.c.Status == 4 && i.c.d.isAssign == 1 && i.c.d.OnWork == 0)
                    .AsEnumerable().Select(i => new CartListViewModel.CartList
                    {
                        Id = i.c.c.Id,
                        ShopName = i.c.c.ShopName,
                        OrderNumber = i.c.c.OrderNumber,
                        DeliveryAddress = i.c.c.DeliveryAddress,
                        PhoneNumber = i.c.d.PhoneNumber,
                        CartStatus = i.c.c.Status,
                        DeliveryBoyName = i.c.c.DeliveryBoyName,
                        DeliveryBoyId = i.c.c.DeliveryBoyId,
                        DateEncoded = i.c.c.DateEncoded,
                        Date = i.c.c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt"),
                        RefundAmount = i.p.RefundAmount ?? 0,
                        RefundRemark = i.p.RefundRemark ?? "",
                        PaymentMode = i.p.PaymentMode,
                    }).OrderByDescending(i => i.DateEncoded).ToList();
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCARWP021")]
        public ActionResult WaitingForPickup(int shopId=0)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartListViewModel();
           
            model.List = db.Orders.Where(i=>i.Status == 4 && i.Status ==0 && (shopId != 0 ? i.ShopId == shopId : true))
                .Join(db.DeliveryBoys.Where(i=>i.isAssign ==1 && i.OnWork == 1), c => c.DeliveryBoyId, d => d.Id, (c, d) => new { c, d })
                .Join(db.Payments,c=>c.c.OrderNumber,p=>p.OrderNumber,(c,p)=>new { c,p})
                .AsEnumerable()
                 .Select(i => new CartListViewModel.CartList
                    {
                     Id = i.c.c.Id,
                     ShopName = i.c.c.ShopName,
                     OrderNumber = i.c.c.OrderNumber,
                     DeliveryAddress = i.c.c.DeliveryAddress,
                     PhoneNumber = i.c.d.PhoneNumber,
                     CartStatus = i.c.c.Status,
                     DeliveryBoyName = i.c.c.DeliveryBoyName,
                     DeliveryBoyId = i.c.c.DeliveryBoyId,
                     DateEncoded = i.c.c.DateEncoded,
                     Date = i.c.c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt"),
                     RefundAmount = i.p.RefundAmount ?? 0,
                     RefundRemark = i.p.RefundRemark ?? "",
                     PaymentMode = i.p.PaymentMode,
                     Amount = i.p.Amount - (i.p.RefundAmount ?? 0),
                    }).OrderByDescending(i => i.DateEncoded).ToList();
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCAROT007")]
        public ActionResult OnTheWay(int shopId=0)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartListViewModel();
                model.List = db.Orders.Where(i => (shopId !=0? i.ShopId == shopId : true) && i.Status == 5)
                    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                    .AsEnumerable()
                    .Select(i => new CartListViewModel.CartList
                    {
                        Id = i.c.Id,
                        ShopName = i.c.ShopName,
                        OrderNumber = i.c.OrderNumber,
                        DeliveryAddress = i.c.DeliveryAddress,
                        PhoneNumber = i.c.ShopOwnerPhoneNumber,
                        CartStatus = i.c.Status,
                        DeliveryBoyName = i.c.DeliveryBoyName,
                        DateEncoded = i.c.DateEncoded,
                        Date = i.c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt"),
                        Price = i.c.TotalPrice,
                        RefundAmount = i.p.RefundAmount ?? 0,
                        RefundRemark = i.p.RefundRemark ?? "",
                        PaymentMode = i.p.PaymentMode,
                    }).OrderByDescending(i => i.DateEncoded).ToList();
            
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCARDL006")]
        public ActionResult Delivered(int shopId = 0)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartListViewModel();
            model.List = db.Orders.Where(i => (shopId != 0 ? i.ShopId == shopId : true) && i.Status == 6)
                .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                .AsEnumerable()
                .Select(i => new CartListViewModel.CartList
                {
                    Id = i.c.Id,
                    ShopName = i.c.ShopName,
                    OrderNumber = i.c.OrderNumber,
                    DeliveryAddress = i.c.DeliveryAddress,
                    PhoneNumber = i.c.ShopOwnerPhoneNumber,
                    CartStatus = i.c.Status,
                    DeliveryBoyName = i.c.DeliveryBoyName,
                    DateEncoded = i.c.DateEncoded,
                    Date = i.c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt"),
                    Price = i.c.TotalPrice,
                    RefundAmount = i.p.RefundAmount ?? 0,
                    RefundRemark = i.p.RefundRemark ?? "",
                    PaymentMode = i.p.PaymentMode,
                }).OrderByDescending(i => i.DateEncoded).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCARDR010")]
        public ActionResult DeliveredReport(CartReportViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.StartDate = model.StartDate == null ? DateTime.Now : model.StartDate;

            model.DeliveredReportLists = db.Orders.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(model.StartDate) && i.Status == 6)
            .Select(i => new CartReportViewModel.DeliveredReportList
            {
                Id = i.Id,
                ShopName = i.ShopName,
                OrderNumber = i.OrderNumber,
                DeliveryAddress = i.DeliveryAddress,
                PhoneNumber = i.CustomerPhoneNumber,
                DateEncoded = i.DateEncoded
            }).OrderByDescending(i => i.DateEncoded).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNCARCA019")]
        public ActionResult Cancelled(int shopId = 0)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartListViewModel();
            model.List = db.Orders.Where(i => (shopId != 0 ? i.ShopId == shopId : true) && i.Status == 6)
            .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
            .AsEnumerable()
            .Select(i => new CartListViewModel.CartList
            {
                Id = i.c.Id,
                ShopName = i.c.ShopName,
                OrderNumber = i.c.OrderNumber,
                DeliveryAddress = i.c.DeliveryAddress,
                PhoneNumber = i.c.ShopOwnerPhoneNumber,
                CartStatus = i.c.Status,
                DeliveryBoyName = i.c.DeliveryBoyName,
                DateEncoded = i.c.DateEncoded,
                Date = i.c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt"),
            }).OrderByDescending(i => i.DateEncoded).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCARCR020")]
        public ActionResult CancelledReport(CartReportViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.StartDate = model.StartDate == null ? DateTime.Now : model.StartDate;

            model.CancelledReportLists = db.Orders.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(model.StartDate) && i.Status == 7)
            .Select(i => new CartReportViewModel.CancelledReportList
            {
                Id = i.Id,
                ShopName = i.ShopName,
                OrderNumber = i.OrderNumber,
                DeliveryAddress = i.DeliveryAddress,
                PhoneNumber = i.CustomerPhoneNumber,
                DateEncoded = i.DateEncoded
            }).OrderByDescending(i => i.DateEncoded).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNCARD005")]
        public ActionResult Details(int id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            Order order = db.Orders.FirstOrDefault(i => i.Id == id);
            var model = new CartDetailsViewModel();
            _mapper.Map(order, model);
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNCARPS025")]
        public ActionResult PickupSlip(int OrderNumber, string id)
        {
            var dInt = AdminHelpers.DCodeLong(id);
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dInt.ToString()))
                return HttpNotFound();
            var cart = db.Orders.FirstOrDefault(i => i.Id == dInt);
            var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == OrderNumber);
            var model = new CartListViewModel();
            if (cart != null)
            {
                model.Id = cart.Id;
                model.OrderNumber = OrderNumber;
                model.CustomerId = cart.CustomerId;
                model.CustomerName = cart.CustomerName;
                model.CartStatus = cart.Status;
                model.ShopName = cart.ShopName;
                model.DeliveryAddress = cart.DeliveryAddress;
                model.PhoneNumber = cart.CustomerPhoneNumber;
                model.DeliveryBoyName = cart.DeliveryBoyName;
                model.DateEncoded = cart.DateEncoded;
                var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == cart.DeliveryBoyId);
                if (deliveryBoy != null)
                {
                    model.isAssign = deliveryBoy.isAssign;
                    model.OnWork = deliveryBoy.OnWork;
                }
            }
            if (payment != null)
            {
                model.Amount = payment.Amount;
                model.PackagingCharge = payment.PackingCharge;
                model.ConvenientCharge = payment.ConvenientCharge;
                model.DelivaryCharge = payment.DeliveryCharge;
            }
                model.List = db.OrderItems.Where(i => i.OrdeNumber == OrderNumber && i.Status == 0).Select(i => new CartListViewModel.CartList
            {
                Id = i.Id,
                BrandName = i.BrandName,
                CategoryName = i.CategoryName,
                ShopName = cart.ShopName,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Qty = i.Quantity,
                Price = i.Price,
                CartStatus = cart.Status,
                PhoneNumber = cart.CustomerPhoneNumber,
                ImagePath = i.ImagePath == "N/A" ? null : i.ImagePath,
                SinglePrice = i.UnitPrice
            }).ToList();
            return View(model);            
        }
         
        [AccessPolicy(PageCode = "SHNCARE004")]
        public ActionResult Edit(int OrderNumber, string id)
        {
            var dInt = AdminHelpers.DCodeLong(id);
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var cart = db.Orders.FirstOrDefault(i => i.Id == dInt);
            var model = new CartListViewModel();
            if (cart != null)
            {
                model.Id = cart.Id;
                model.OrderNumber = OrderNumber;
                model.CustomerId = cart.CustomerId;
                model.CustomerName = cart.CustomerName;
                model.CartStatus = cart.Status;
                model.ShopName = cart.ShopName;
                model.DeliveryAddress = cart.DeliveryAddress;
                model.PhoneNumber = cart.CustomerPhoneNumber;
                model.DeliveryBoyName = cart.DeliveryBoyName;
                model.DateEncoded = cart.DateEncoded;
                var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == cart.DeliveryBoyId);
                if (deliveryBoy != null)
                {
                    model.isAssign = deliveryBoy.isAssign;
                    model.OnWork = deliveryBoy.OnWork;
                }
                model.Latitude = cart.Latitude;
                model.Longtitude = cart.Longitude;
            }
            model.List = db.OrderItems.Where(i => i.OrdeNumber == OrderNumber && i.Status == 0).Select(i => new CartListViewModel.CartList
            {
                Id = i.Id,
                BrandName = i.BrandName,
                CategoryName = i.CategoryName,
                ShopName = cart.ShopName,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Qty = i.Quantity,
                Price = i.Price,
                CartStatus = cart.Status,
                PhoneNumber = cart.CustomerPhoneNumber,
                ImagePath = i.ImagePath == "N/A" ? null : i.ImagePath,
                SinglePrice = i.UnitPrice
            }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNCARAD015")]
        public ActionResult AssignDeliveryBoy(int OrderNumber)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var order = db.Orders.Where(i => i.OrderNumber == OrderNumber).FirstOrDefault();
            var shop = db.Shops.FirstOrDefault(i => i.Id == order.ShopId);
            var model = new CartAssignDeliveryBoyViewModel();
            model.OrderId = order.Id;
            DateTime date = DateTime.Now;
            
            var amount = (from i in db.Orders
                                   where i.OrderNumber == OrderNumber && i.Status == 0 && i.Status == 6 && i.DateUpdated.Year == date.Year && i.DateUpdated.Month == date.Month && i.DateUpdated.Day == date.Day
                                   select (Double?)i.DeliveryCharge).Sum() ?? 0;

            model.Lists = db.DeliveryBoys
               .Where(i => i.OnWork == 0 && i.isAssign == 0 && i.Active == 1 && i.Status == 0)
                 .AsEnumerable()
                 .Select(i => new CartAssignDeliveryBoyViewModel.CartAssignList
                 {
                     Id = i.Id,
                     //  Name = "D" + _generatedDelivaryId,
                     Name = i.Name,
                     Status = i.Status,
                     Amount = amount,
                     Meters = (((Math.Acos(Math.Sin((shop.Latitude * Math.PI / 180)) * Math.Sin((i.Latitude * Math.PI / 180)) + Math.Cos((shop.Latitude * Math.PI / 180)) * Math.Cos((i.Latitude * Math.PI / 180))
                 * Math.Cos(((shop.Longitude - i.Longitude) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344)
                 }).Where(i => i.Meters < 8000 && i.Status == 0).ToList();
           
            return View(model);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SHNCARAD015")]
        public ActionResult AssignDeliveryBoy(CartAssignDeliveryBoyViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var cart = db.Orders.FirstOrDefault(i => i.Id == model.OrderId);
            if (cart != null && model.DeliveryBoyId != 0)
            {
                var cartList = db.OrderItems.Where(i => i.OrderId == cart.Id).ToList();
                var delivary = db.DeliveryBoys.FirstOrDefault(i => i.Id == model.DeliveryBoyId);

                cart.DeliveryBoyId = delivary.Id;
                cart.DeliveryBoyName = delivary.Name;
                cart.DeliveryBoyPhoneNumber = delivary.PhoneNumber;
                cart.Status = 4;
                cart.DateUpdated = DateTime.Now;
                db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                delivary.isAssign = 1;
                delivary.DateUpdated = DateTime.Now;
                db.Entry(delivary).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var fcmToken = (from c in db.Customers
                                where c.Id == delivary.CustomerId
                                select c.FcmTocken ?? "").FirstOrDefault().ToString();
                Helpers.PushNotification.SendbydeviceId("You have received new order. Accept Soon", "ShopNowChat", "a.mp3", fcmToken.ToString());

               //Customer
                var fcmTokenCustomer = (from c in db.Customers
                                        where c.Id == cart.CustomerId
                                        select c.FcmTocken ?? "").FirstOrDefault().ToString();
                Helpers.PushNotification.SendbydeviceId("Delivery Boy is Assigned for your Order.", "ShopNowChat", "../../assets/b.mp3", fcmToken.ToString());

                return RedirectToAction("List");
            }
            else
            {
                return View(model);
            }

        }

        [AccessPolicy(PageCode = "")]
        public ActionResult DeliveryBoyCashHandoverReport(DateTime? StartDate, DateTime? EndDate, int deliveryboyId = 0)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartReportViewModel();

            if (deliveryboyId != 0)
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.List = db.Payments.Join(db.Orders, p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
                    .Where(i => i.p.DateEncoded >= startDatetFilter && i.p.DateEncoded <= endDateFilter && i.p.Status == 0
                    && i.c.DeliveryBoyId == deliveryboyId && i.c.Status == 6 && i.p.PaymentMode == "Cash On Hand")
                     .Select(i => new CartReportViewModel.CartReportList
                     {
                         Id = i.c.Id,
                         OrderNumber = i.p.OrderNumber,
                         DeliveryBoyPhoneNumber =i.c.DeliveryBoyPhoneNumber,
                         DeliveryBoyId = i.c.DeliveryBoyId,
                         DeliveryBoyName = i.c.DeliveryBoyName,
                         Amount = i.p.Amount - (i.p.RefundAmount ?? 0),
                         DateUpdated = i.c.DateUpdated,
                         DeliveryOrderPaymentStatus = i.c.DeliveryOrderPaymentStatus
                     }).OrderByDescending(i => i.DateUpdated).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    model.List = db.Payments
                        .Join(db.Orders, p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
                      .Where(i => i.p.Status == 0 && i.c.DeliveryBoyId == deliveryboyId && i.c.Status == 6 && i.p.PaymentMode == "Cash On Hand")
                     .Select(i => new CartReportViewModel.CartReportList
                     {
                         Id = i.c.Id,
                         OrderNumber = i.p.OrderNumber,
                         DeliveryBoyPhoneNumber = i.c.DeliveryBoyPhoneNumber,
                         DeliveryBoyId = i.c.DeliveryBoyId,
                         DeliveryBoyName = i.c.DeliveryBoyName,
                         Amount = i.p.Amount - (i.p.RefundAmount ?? 0),
                         DateUpdated = i.c.DateUpdated,
                         DeliveryOrderPaymentStatus = i.c.DeliveryOrderPaymentStatus
                     }).OrderByDescending(i => i.DateUpdated).ToList();
                }
            }
            else
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.List = db.Payments
                        .Join(db.Orders, p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
                    .Where(i => i.p.DateEncoded >= startDatetFilter && i.p.DateEncoded <= endDateFilter && i.p.Status == 0
                    && i.c.Status == 6 && i.p.PaymentMode == "Cash On Hand")
                     .Select(i => new CartReportViewModel.CartReportList
                     {
                         Id = i.c.Id,
                         OrderNumber = i.p.OrderNumber,
                         DeliveryBoyPhoneNumber = i.c.DeliveryBoyPhoneNumber,
                         DeliveryBoyId = i.c.DeliveryBoyId,
                         DeliveryBoyName = i.c.DeliveryBoyName,
                         Amount = i.p.Amount - (i.p.RefundAmount ?? 0),
                         DateUpdated = i.c.DateUpdated,
                         DeliveryOrderPaymentStatus = i.c.DeliveryOrderPaymentStatus
                     }).OrderByDescending(i => i.DateUpdated).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    model.List = db.Payments.Join(db.Orders, p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
                       .Where(i => i.p.Status == 0 && i.c.Status == 6 && i.p.PaymentMode == "Cash On Hand")
                     .Select(i => new CartReportViewModel.CartReportList
                     {
                         Id = i.c.Id,
                         OrderNumber = i.p.OrderNumber,
                         DeliveryBoyPhoneNumber = i.c.DeliveryBoyPhoneNumber,
                         DeliveryBoyId = i.c.DeliveryBoyId,
                         DeliveryBoyName = i.c.DeliveryBoyName,
                         Amount = i.p.Amount - (i.p.RefundAmount ?? 0),
                         DateUpdated = i.c.DateUpdated,
                         DeliveryOrderPaymentStatus = i.c.DeliveryOrderPaymentStatus
                     }).OrderByDescending(i => i.DateUpdated).ToList();
                }
            }
            return View(model);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult DeliveryBoyPaymentStatus(DateTime? StartDate, DateTime? EndDate, int DeliveryBoyId = 0)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartReportViewModel();
            if (DeliveryBoyId != 0)
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.DeliveryBoyPaymentStatusLists = db.Orders
                    .Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.Status == 6 && i.DeliveryBoyId == DeliveryBoyId)
                        .Select(i => new CartReportViewModel.DeliveryBoyPaymentStatusList
                        {
                            Id = i.Id,
                            DateEncoded = i.DateEncoded,
                            OrderNumber = i.OrderNumber,
                            DeliveryBoyId = i.DeliveryBoyId,
                            DeliveryBoyName = i.DeliveryBoyName,
                            DeliveryBoyPhoneNumber = i.DeliveryBoyPhoneNumber,
                            DeliveryCharge = i.DeliveryCharge,
                            DeliveryBoyPaymentStatus = i.DeliveryBoyPaymentStatus,
                            Distance = i.Distance
                        }).OrderBy(i => i.DeliveryBoyName).ToList();
                }
                else
                {
                    model.DeliveryBoyPaymentStatusLists = db.Orders
                      .Where(i => i.Status == 6 && i.DeliveryBoyId == DeliveryBoyId)
                          .Select(i => new CartReportViewModel.DeliveryBoyPaymentStatusList
                          {
                              Id = i.Id,
                              DateEncoded = i.DateEncoded,
                              OrderNumber = i.OrderNumber,
                              DeliveryBoyId = i.DeliveryBoyId,
                              DeliveryBoyName = i.DeliveryBoyName,
                              DeliveryBoyPhoneNumber = i.DeliveryBoyPhoneNumber,
                              DeliveryCharge = i.DeliveryCharge,
                              DeliveryBoyPaymentStatus = i.DeliveryBoyPaymentStatus,
                              Distance = i.Distance
                          }).OrderBy(i => i.DeliveryBoyName).ToList();
                }
            }
            else
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.DeliveryBoyPaymentStatusLists = db.Orders
                    .Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.Status == 6)
                        .Select(i => new CartReportViewModel.DeliveryBoyPaymentStatusList
                        {
                            Id = i.Id,
                            DateEncoded = i.DateEncoded,
                            OrderNumber = i.OrderNumber,
                            DeliveryBoyId = i.DeliveryBoyId,
                            DeliveryBoyName = i.DeliveryBoyName,
                            DeliveryBoyPhoneNumber = i.DeliveryBoyPhoneNumber,
                            DeliveryCharge = i.DeliveryCharge,
                            DeliveryBoyPaymentStatus = i.DeliveryBoyPaymentStatus,
                            Distance = i.Distance
                        }).OrderBy(i => i.DeliveryBoyName).ToList();
                }
                else
                {
                    model.DeliveryBoyPaymentStatusLists = db.Orders
                      .Where(i => i.Status == 6)
                          .Select(i => new CartReportViewModel.DeliveryBoyPaymentStatusList
                          {
                              Id = i.Id,
                              DateEncoded = i.DateEncoded,
                              OrderNumber = i.OrderNumber,
                              DeliveryBoyId = i.DeliveryBoyId,
                              DeliveryBoyName = i.DeliveryBoyName,
                              DeliveryBoyPhoneNumber = i.DeliveryBoyPhoneNumber,
                              DeliveryCharge = i.DeliveryCharge,
                              DeliveryBoyPaymentStatus = i.DeliveryBoyPaymentStatus,
                              Distance = i.Distance
                          }).OrderBy(i => i.DeliveryBoyName).ToList();
                }
            }
            return View(model);
        }

        DeliveryBoy getDBoy(int id)
        {
            var deliveryBoy = db.DeliveryBoys.Where(d => d.Id == id).FirstOrDefault();
            return deliveryBoy;
        }
        public int getDeliveryBoy(int id)
        {
            var deliveryBoy = db.DeliveryBoys.Where(d => d.Id == id).FirstOrDefault();
            return deliveryBoy.Id;
        }
        
        double GetMeters(Double Latitudes, Double Longitudes, Double Latitude, Double Longitude)
        {
            return (((Math.Acos(Math.Sin((Latitude * Math.PI / 180)) * Math.Sin((Latitudes * Math.PI / 180)) + Math.Cos((Latitude * Math.PI / 180)) * Math.Cos((Latitudes * Math.PI / 180))
                    * Math.Cos(((Longitude - Longitudes) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344);
        }
        [AccessPolicy(PageCode = "SHNCARL001")]
        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetDeliveryBoySelect2(string q = "")
        {
            var model = await db.DeliveryBoys.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCARAC011")]
        public JsonResult Accept(int OrderNumber)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            if (OrderNumber != 0)
            {
                var order = db.Orders.FirstOrDefault(i => i.OrderNumber == OrderNumber);
                order.Status = 3;
                order.UpdatedBy = user.Name;
                order.DateUpdated = DateTime.Now;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //Customer
                var fcmToken = (from c in db.Customers
                                where c.Id == order.CustomerId
                                select c.FcmTocken ?? "").FirstOrDefault().ToString();
                Helpers.PushNotification.SendbydeviceId("Your order has been accepted by shop.", "ShopNowChat", "a.mp3", fcmToken.ToString());

                return Json(new { message = "Order Confirmed!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "Failed to Confirm the Order!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [AccessPolicy(PageCode = "SHNCARR012")]
        public JsonResult Cancel(int OrderNumber, int customerId, int? status)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            if (OrderNumber != 0 && customerId != 0 && status != 0)
            {
                var customer = db.Customers.FirstOrDefault(i => i.Id == customerId);

                var order = db.Orders.FirstOrDefault(i => i.OrderNumber ==OrderNumber);
                order.Status = 7;
                order.UpdatedBy = user.Name;
                order.DateUpdated = DateTime.Now;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var orderItemList = db.OrderItems.Where(i => i.OrderId == order.Id);
                foreach (var item in orderItemList)
                {
                    //Product Stock Update
                    var product = db.Products.FirstOrDefault(i => i.Id == item.ProductId);
                    product.HoldOnStok -= Convert.ToInt32(item.Quantity);
                    db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    
                }
                //Refund
                var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == order.OrderNumber);
                payment.RefundAmount = payment.Amount;
                payment.RefundRemark = "Your order has been cancelled by shop.";
                payment.UpdatedBy = customer.Name;
                payment.DateUpdated = DateTime.Now;
                db.Entry(payment).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var fcmToken = (from c in db.Customers
                                where c.Id == order.CustomerId
                                select c.FcmTocken ?? "").FirstOrDefault().ToString();
                //order cancel
                Helpers.PushNotification.SendbydeviceId("Shop has rejected your order. Kindly contact shop for details or try another order.", "ShopNowChat", "a.mp3", fcmToken.ToString());

                //Refund notification
                if (payment.PaymentMode == "Online Payment")
                    Helpers.PushNotification.SendbydeviceId($"Your refund of amount {payment.Amount} for order no {payment.OrderNumber} is for {payment.RefundRemark} initiated and you will get credited with in 7 working days.", "ShopNowChat", "a.mp3", fcmToken.ToString());

                return Json(new { message = "Order Cancelled!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "Failed to Cancel the Order!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult ShopPay(int OrderNumber)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var order = db.Orders.FirstOrDefault(i => i.OrderNumber == OrderNumber && i.ShopPaymentStatus == 0);
            order.ShopPaymentStatus = 1;
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult ShopNowChatPay(int OrderNumber)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var order = db.Orders.FirstOrDefault(i => i.OrderNumber == OrderNumber && i.DeliveryBoyPaymentStatus == 0);
            order.DeliveryBoyPaymentStatus = 1;
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult DeliveryBoyPay(int OrderNumber)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var order = db.Orders.FirstOrDefault(i => i.OrderNumber == OrderNumber && i.DeliveryBoyPaymentStatus == 0);
            order.DeliveryBoyPaymentStatus = 1;
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult DeliveryBoyReject(int OrderNumber)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            if (OrderNumber != 0)
            {
                var order =  db.Orders.FirstOrDefault(i => i.OrderNumber == OrderNumber);
                order.DeliveryBoyId = 0;
                order.DeliveryBoyName = null;
                order.DeliveryBoyPhoneNumber = null;
                order.Status = 3;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var dboy = db.DeliveryBoys.FirstOrDefault(i => i.Id == order.DeliveryBoyId);
                dboy.isAssign = 0;
                dboy.OnWork = 0;
                dboy.DateUpdated = DateTime.Now;
                db.Entry(dboy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        

        public ActionResult UnAssignDeliveryBoy(int OrderNumber)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var order = db.Orders.FirstOrDefault(i => i.OrderNumber == OrderNumber);
            var deliveryboy = db.DeliveryBoys.FirstOrDefault(i => i.Id == order.DeliveryBoyId);
            deliveryboy.isAssign = 0;
            deliveryboy.OnWork = 0;
            deliveryboy.DateUpdated = DateTime.Now;
            db.Entry(deliveryboy).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            order.DeliveryBoyId = 0;
            order.DeliveryBoyName = string.Empty;
            order.DeliveryBoyPhoneNumber = string.Empty;
            order.Status = 3;
            order.DateUpdated = DateTime.Now;
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("DeliveryAgentAssigned");
        }

        public ActionResult AddRefundFromShopOrderProcessing(int id, double amount, string remark, int redirection = 0)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);

            var order = db.Orders.FirstOrDefault(i => i.Id == id);
            //Refund
            var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == order.OrderNumber);
            payment.RefundAmount = amount;
            payment.RefundRemark = remark;
            payment.UpdatedBy = user.Name;
            payment.DateUpdated = DateTime.Now;
            db.Entry(payment).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            var fcmToken = (from c in db.Customers
                            where c.Id == order.CustomerId
                            select c.FcmTocken ?? "").FirstOrDefault().ToString();
            if (payment.PaymentMode == "Online Payment")
                Helpers.PushNotification.SendbydeviceId($"Your refund of amount {payment.RefundAmount} for order no {payment.OrderNumber} is for {payment.RefundRemark} initiated and you will get credited with in 7 working days.", "ShopNowChat", "a.mp3", fcmToken.ToString());
            else
                Helpers.PushNotification.SendbydeviceId($"Your order is reduced with {payment.RefundAmount} amount for {payment.RefundRemark}", "ShopNowChat", "a.mp3", fcmToken.ToString());

            if (redirection == 0)
                return RedirectToAction("Pending");
            else if (redirection == 1)
                return RedirectToAction("OrderPrepared");
            else if (redirection == 2)
                return RedirectToAction("DeliveryAgentAssigned");
            else if (redirection == 3)
                return RedirectToAction("WaitingForPickup");
            else if (redirection == 4)
                return RedirectToAction("OntheWay");
            else
                return RedirectToAction("Delivered");
        }

        public ActionResult DeliveryBoyAccept(int OrderNumber, long id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var order = db.Orders.FirstOrDefault(i => i.Id == id);
            var delivaryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == order.DeliveryBoyId && i.Status == 0);
            delivaryBoy.OnWork = 1;
            delivaryBoy.UpdatedBy = user.Name;
            delivaryBoy.DateUpdated = DateTime.Now;
            db.Entry(delivaryBoy).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            
            return RedirectToAction("Edit", "Cart", new { OrderNumber = OrderNumber, id = AdminHelpers.ECodeLong(id )});
        }

        public ActionResult DeliveryBoyPickup(int OrderNumber, int id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);

            var order = db.Orders.FirstOrDefault(i => i.Id == id);
            order.Status = 5;
            order.UpdatedBy = user.Name;
            order.DateUpdated = DateTime.Now;
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            var orderItemList = db.OrderItems.Where(i => i.OrderId == order.Id).ToList();
            foreach (var item in orderItemList)
            {
                //Product Stock Update
                var product = db.Products.FirstOrDefault(i => i.Id == item.ProductId);
                product.HoldOnStok -= Convert.ToInt32(item.Quantity);
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }            

            var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == OrderNumber);
            if (payment.PaymentMode == "Online Payment" && payment.Amount > 1000)
            {
                var otpVerification = new OtpVerification();
                otpVerification.ShopId = order.ShopId;
                otpVerification.Id = user.Id;
                otpVerification.CustomerName = user.Name;
                otpVerification.PhoneNumber = order.DeliveryBoyPhoneNumber;
                otpVerification.Otp = Helpers.DRC.GenerateOTP();
                otpVerification.ReferenceCode = Helpers.DRC.Generate("");
                otpVerification.Verify = false;
                otpVerification.OrderNo = OrderNumber;
                otpVerification.CreatedBy = user.Name;
                otpVerification.UpdatedBy = user.Name;
                otpVerification.DateUpdated = DateTime.Now;
                db.OtpVerifications.Add(otpVerification);
                db.SaveChanges();
            }
            var fcmToken = (from c in db.Customers
                            where c.Id == order.CustomerId
                            select c.FcmTocken ?? "").FirstOrDefault().ToString();
            Helpers.PushNotification.SendbydeviceId("Your order is on the way.", "ShopNowChat", "a.mp3", fcmToken.ToString());
            return RedirectToAction("Edit", "Cart", new { OrderNumber = OrderNumber, id = AdminHelpers.ECodeLong(id) });
        }

        public ActionResult MarkAsDelivered(int OrderNumber, int id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);

            var otpVerify = db.OtpVerifications.FirstOrDefault(i => i.OrderNo == OrderNumber);

            var order = db.Orders.FirstOrDefault(i => i.Id == id);

            var delivaryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == order.DeliveryBoyId && i.Status == 0);
            delivaryBoy.OnWork = 0;
            delivaryBoy.isAssign = 0;
            delivaryBoy.DateUpdated = DateTime.Now;
            db.Entry(delivaryBoy).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            if (otpVerify != null)
            {
                otpVerify.Verify = true;
                otpVerify.UpdatedBy = user.Name;
                otpVerify.DateUpdated = DateTime.Now;
                db.Entry(otpVerify).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }


            order.Status = 6;
            order.UpdatedBy = delivaryBoy.CustomerName;
            order.DateUpdated = DateTime.Now;
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            var fcmToken = (from c in db.Customers
                            where c.Id == order.CustomerId
                            select c.FcmTocken ?? "").FirstOrDefault().ToString();
            Helpers.PushNotification.SendbydeviceId("Your order has been delivered.", "ShopNowChat", "a.mp3", fcmToken.ToString());
            return RedirectToAction("Edit", "Cart", new { OrderNumber = OrderNumber, id = AdminHelpers.ECodeLong(id) });
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

