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
            model.List = db.Orders.Where(i => i.Status == 0 && (shopId != 0 ? i.ShopId == shopId : true) && i.Status != 6 && i.Status != 7 && i.Status != 0)
            .AsEnumerable().Select(i => new CartListViewModel.CartList
            {
                Id = i.Id,
                ShopName = i.ShopName,
                OrderNumber = i.OrderNumber,
                DeliveryAddress = i.DeliveryAddress,
                PhoneNumber = i.ShopPhoneNumber,
                CartStatus = i.Status,
                DeliveryBoyName = i.DeliveryBoyName ?? "N/A",
                DateEncoded = i.DateEncoded,
                Date = i.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt")
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
        public ActionResult DeliveredReport(int shopid = 0)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartReportViewModel();

            var shop = db.Shops.FirstOrDefault(i => i.Id == shopid);
            model.List = db.Orders.Where(i => i.ShopId == shopid && i.Status == 6)
           .AsEnumerable().GroupBy(i => i.OrderNumber).Select(i => new CartReportViewModel.CartReportList
           {
               Id = i.Any() ? i.FirstOrDefault().Id : 0,
               ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
               OrderNumber = i.Any() ? i.FirstOrDefault().OrderNumber : 0,
               DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
               PhoneNumber = i.Any() ? i.FirstOrDefault().ShopOwnerPhoneNumber : "N/A",
               CartStatus = i.Any() ? i.FirstOrDefault().Status : 2,
               DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
               DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
           }).OrderByDescending(i => i.DateEncoded).ToList();
            model.ShopId = shopid;

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCARCR020")]
        public ActionResult CancelledReport(int shopid = 0)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartReportViewModel();
            var shop = db.Shops.FirstOrDefault(i => i.Id == shopid);
            model.List = db.Orders.Where(i => i.ShopId == shopid && i.Status == 7)
            .AsEnumerable().GroupBy(i => i.OrderNumber).Select(i => new CartReportViewModel.CartReportList
            {
                Id = i.Any() ? i.FirstOrDefault().Id : 0,
                ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                OrderNumber = i.Any() ? i.FirstOrDefault().OrderNumber : 0,
                DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                PhoneNumber = i.Any() ? i.FirstOrDefault().ShopOwnerPhoneNumber : "N/A",
                DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
            }).OrderByDescending(i => i.DateEncoded).ToList();
            model.ShopId = shopid;
            model.ShopName = shop.Name;
            return View(model.List);
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
        public ActionResult PickupSlip(int OrderNumber, int id)
        {
            var dInt = AdminHelpers.DCodeInt(id.ToString());
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dInt.ToString()))
                return HttpNotFound();
            var cart = db.Orders.FirstOrDefault(i => i.Id == dInt);
            var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == OrderNumber);
            var model = new CartListViewModel();
            if (cart != null)
            {
                model.Id = id;
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
                model.PackagingCharge = payment.PackagingCharge;
                model.ConvenientCharge = payment.ConvenientCharge;
                model.DelivaryCharge = payment.DelivaryCharge;
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
        public ActionResult Edit(int OrderNumber, int id)
        {
            var dInt = AdminHelpers.DCodeInt(id.ToString());
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var cart = db.Orders.FirstOrDefault(i => i.Id == dInt);
            var model = new CartListViewModel();
            if (cart != null)
            {
                model.Id = id;
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

        //[AccessPolicy(PageCode = "SHNCARPR008")]
        //public ActionResult PendingReport(DateTime? StartDate, DateTime? EndDate, string ShopCode = "")
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var model = new CartReportViewModel();

        //    if (ShopCode != "")
        //    {
        //        var shop = db.Shops.FirstOrDefault(i => i.Code == ShopCode);//Shop.Get(ShopCode);
        //        if (StartDate != null && EndDate != null)
        //        {
        //            DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
        //            DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
        //            model.List = db.Carts.Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.ShopCode == ShopCode && i.CartStatus == 2 && i.Status == 0)
        //         .AsEnumerable().GroupBy(i => i.OrderNumber)
        //          .Select(i => new CartReportViewModel.CartReportList
        //          {
        //              Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
        //              ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
        //              OrderNumber = i.Any() ? i.FirstOrDefault().OrderNumber : "N/A",
        //              DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
        //              PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
        //              CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
        //              DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
        //              DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
        //          }).OrderByDescending(i => i.DateEncoded).ToList();
        //            ViewBag.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
        //            ViewBag.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
        //            ViewBag.ShopCode = ShopCode;
        //            ViewBag.ShopName = shop.Name;
        //        }
        //        else
        //        {
        //            model.List = db.Carts.Where(i => i.ShopCode == ShopCode && i.CartStatus == 2 && i.Status == 0)
        //         .AsEnumerable().GroupBy(i => i.OrderNumber)
        //          .Select(i => new CartReportViewModel.CartReportList
        //          {
        //              Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
        //              ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
        //              OrderNumber = i.Any() ? i.FirstOrDefault().OrderNumber : "N/A",
        //              DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
        //              PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
        //              CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
        //              DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
        //              DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
        //          }).OrderByDescending(i => i.DateEncoded).ToList();
        //            ViewBag.ShopCode = ShopCode;
        //            ViewBag.ShopName = shop.Name;
        //        }

        //    }
        //    else
        //    {
        //        if (StartDate != null && EndDate != null)
        //        {
        //            DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
        //            DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
        //            model.List = db.Carts.Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.CartStatus == 2 && i.Status == 0)
        //          .AsEnumerable().GroupBy(i => i.OrderNumber)
        //           .Select(i => new CartReportViewModel.CartReportList
        //           {
        //               Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
        //               ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
        //               OrderNumber = i.Any() ? i.FirstOrDefault().OrderNumber : "N/A",
        //               DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
        //               PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
        //               CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
        //               DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
        //               DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
        //           }).OrderByDescending(i => i.DateEncoded).ToList();
        //            ViewBag.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
        //            ViewBag.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
        //        }
        //        else
        //        {
        //            model.List = db.Carts.Where(i => i.CartStatus == 2 && i.Status == 0)
        //        .AsEnumerable().GroupBy(i => i.OrderNumber)
        //         .Select(i => new CartReportViewModel.CartReportList
        //         {
        //             Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
        //             ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
        //             OrderNumber = i.Any() ? i.FirstOrDefault().OrderNumber : "N/A",
        //             DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
        //             PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
        //             CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
        //             DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
        //             DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
        //         }).OrderByDescending(i => i.DateEncoded).ToList();
        //        }
        //    }
        //    return View(model.List);
        //}

        //[AccessPolicy(PageCode = "SHNCARDAR022")]
        //public ActionResult DeliveryAgentAssignedReport(DateTime? StartDate, DateTime? EndDate, string ShopCode = "")
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var model = new CartReportViewModel();

        //    if (ShopCode != "")
        //    {
        //        var shop = db.Shops.FirstOrDefault(i => i.Code == ShopCode);//Shop.Get(ShopCode);
        //        if (StartDate != null && EndDate != null)
        //        {
        //            DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
        //            DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

        //            model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
        //            .Where(i => i.c.DateEncoded >= startDatetFilter && i.c.DateEncoded <= endDateFilter && i.c.Status == 0 && i.c.ShopCode == ShopCode && i.c.CartStatus == 4 && i.d.isAssign == 1 && i.d.OnWork == 0)
        //            .AsEnumerable().GroupBy(i => i.c.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //            {
        //                Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
        //                ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
        //                OrderNumber = i.Any() ? i.FirstOrDefault().c.OrderNumber : "N/A",
        //                DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
        //                PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "N/A",
        //                CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
        //                DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
        //                DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now
        //            }).ToList();
        //            model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
        //            model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
        //            model.ShopCode = ShopCode;
        //            model.ShopName = shop.Name;
        //        }
        //        else
        //        {
        //            model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
        //           .Where(i => i.c.Status == 0 && i.c.ShopCode == ShopCode && i.c.CartStatus == 4 && i.d.isAssign == 1 && i.d.OnWork == 0)
        //           .AsEnumerable().GroupBy(i => i.c.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //           {
        //               Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
        //               ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
        //               OrderNumber = i.Any() ? i.FirstOrDefault().c.OrderNumber : "N/A",
        //               DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
        //               PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "N/A",
        //               CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
        //               DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
        //               DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now
        //           }).ToList();
        //            model.ShopCode = ShopCode;
        //            model.ShopName = shop.Name;
        //        }
        //    }
        //    else
        //    {
        //        if (StartDate != null && EndDate != null)
        //        {
        //            DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
        //            DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
        //            model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
        //            .Where(i => i.c.DateEncoded >= startDatetFilter && i.c.DateEncoded <= endDateFilter && i.c.Status == 0 && i.c.CartStatus == 4 && i.d.isAssign == 1 && i.d.OnWork == 0)
        //            .AsEnumerable().GroupBy(i => i.c.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //            {
        //                Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
        //                ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
        //                OrderNumber = i.Any() ? i.FirstOrDefault().c.OrderNumber : "N/A",
        //                DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
        //                PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "N/A",
        //                CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
        //                DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
        //                DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now
        //            }).ToList();
        //            model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
        //            model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
        //        }
        //        else
        //        {
        //            model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
        //           .Where(i => i.c.Status == 0 && i.c.CartStatus == 4 && i.d.isAssign == 1 && i.d.OnWork == 0)
        //           .AsEnumerable().GroupBy(i => i.c.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //           {
        //               Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
        //               ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
        //               OrderNumber = i.Any() ? i.FirstOrDefault().c.OrderNumber : "N/A",
        //               DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
        //               PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "N/A",
        //               CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
        //               DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
        //               DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now
        //           }).ToList();
        //        }
        //    }
        //    return View(model.List);
        //}

        //[AccessPolicy(PageCode = "SHNCARWPR023")]
        //public ActionResult WaitingForPickupReport(DateTime? StartDate, DateTime? EndDate, string ShopCode = "")
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var model = new CartReportViewModel();

        //    if (ShopCode != "")
        //    {
        //        var shop = db.Shops.FirstOrDefault(i => i.Code == ShopCode);//Shop.Get(ShopCode);
        //        if (StartDate != null && EndDate != null)
        //        {
        //            DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
        //            DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

        //            model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
        //            .Where(i => i.c.DateEncoded >= startDatetFilter && i.c.DateEncoded <= endDateFilter && i.c.Status == 0 && i.c.ShopCode == ShopCode && i.c.CartStatus == 4 && i.d.isAssign == 1 && i.d.OnWork == 1)
        //            .AsEnumerable().GroupBy(i => i.c.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //            {
        //                Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
        //                ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
        //                OrderNumber = i.Any() ? i.FirstOrDefault().c.OrderNumber : "N/A",
        //                DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
        //                PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "N/A",
        //                CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
        //                DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
        //                DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now
        //            }).ToList();
        //            model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
        //            model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
        //            model.ShopCode = ShopCode;
        //            model.ShopName = shop.Name;
        //        }
        //        else
        //        {
        //            model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
        //           .Where(i => i.c.Status == 0 && i.c.ShopCode == ShopCode && i.c.CartStatus == 4 && i.d.isAssign == 1 && i.d.OnWork == 1)
        //           .AsEnumerable().GroupBy(i => i.c.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //           {
        //               Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
        //               ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
        //               OrderNumber = i.Any() ? i.FirstOrDefault().c.OrderNumber : "N/A",
        //               DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
        //               PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "N/A",
        //               CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
        //               DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
        //               DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now
        //           }).ToList();
        //            model.ShopCode = ShopCode;
        //            model.ShopName = shop.Name;
        //        }

        //    }
        //    else
        //    {
        //        if (StartDate != null && EndDate != null)
        //        {
        //            DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
        //            DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
        //            model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
        //            .Where(i => i.c.DateEncoded >= startDatetFilter && i.c.DateEncoded <= endDateFilter && i.c.Status == 0 && i.c.CartStatus == 4 && i.d.isAssign == 1 && i.d.OnWork == 1)
        //            .AsEnumerable().GroupBy(i => i.c.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //            {
        //                Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
        //                ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
        //                OrderNumber = i.Any() ? i.FirstOrDefault().c.OrderNumber : "N/A",
        //                DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
        //                PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "N/A",
        //                CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
        //                DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
        //                DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now
        //            }).ToList();
        //            model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
        //            model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
        //        }
        //        else
        //        {
        //            model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
        //           .Where(i => i.c.Status == 0 && i.c.CartStatus == 4 && i.d.isAssign == 1 && i.d.OnWork == 1)
        //           .AsEnumerable().GroupBy(i => i.c.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //           {
        //               Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
        //               ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
        //               OrderNumber = i.Any() ? i.FirstOrDefault().c.OrderNumber : "N/A",
        //               DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
        //               PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "N/A",
        //               CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
        //               DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
        //               DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now
        //           }).ToList();
        //        }
        //    }
        //    return View(model.List);
        //}

        //[AccessPolicy(PageCode = "SHNCAROR009")]
        //public ActionResult OntheWayReport(DateTime? StartDate, DateTime? EndDate, string ShopCode = "")
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var model = new CartReportViewModel();

        //    if (ShopCode != "")
        //    {
        //        var shop = db.Shops.FirstOrDefault(i => i.Code == ShopCode);//Shop.Get(ShopCode);
        //        if (StartDate != null && EndDate != null)
        //        {
        //            DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
        //            DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
                    
        //            model.List = db.Carts.Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.ShopCode == ShopCode && i.CartStatus == 5 && i.Status == 0)
        //           .AsEnumerable().GroupBy(i => i.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //           {
        //               Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
        //               ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
        //               OrderNumber = i.Any() ? i.FirstOrDefault().OrderNumber : "N/A",
        //               DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
        //               PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
        //               CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
        //               DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
        //               DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
        //           }).OrderByDescending(i => i.DateEncoded).ToList();
        //            model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
        //            model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
        //            model.ShopCode = ShopCode;
        //            model.ShopName = shop.Name;
        //        }
        //        else
        //        {
        //            model.List = db.Carts.Where(i => i.ShopCode == ShopCode && i.CartStatus == 5 && i.Status == 0)
        //           .AsEnumerable().GroupBy(i => i.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //           {
        //               Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
        //               ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
        //               OrderNumber = i.Any() ? i.FirstOrDefault().OrderNumber : "N/A",
        //               DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
        //               PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
        //               CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
        //               DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
        //               DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
        //           }).OrderByDescending(i => i.DateEncoded).ToList();
        //            model.ShopCode = ShopCode;
        //            model.ShopName = shop.Name;
        //        }

        //    }
        //    else
        //    {
        //        if (StartDate != null && EndDate != null)
        //        {
        //            DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
        //            DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
        //            model.List = db.Carts.Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.CartStatus == 5 && i.Status == 0)
        //           .AsEnumerable().GroupBy(i => i.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //           {
        //               Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
        //               ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
        //               OrderNumber = i.Any() ? i.FirstOrDefault().OrderNumber : "N/A",
        //               DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
        //               PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
        //               CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
        //               DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
        //               DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
        //           }).OrderByDescending(i => i.DateEncoded).ToList();
        //            model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
        //            model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
        //        }
        //        else
        //        {
        //            model.List = db.Carts.Where(i => i.CartStatus == 5 && i.Status == 0)
        //          .AsEnumerable().GroupBy(i => i.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //          {
        //              Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
        //              ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
        //              OrderNumber = i.Any() ? i.FirstOrDefault().OrderNumber : "N/A",
        //              DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
        //              PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
        //              CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
        //              DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
        //              DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
        //          }).OrderByDescending(i => i.DateEncoded).ToList();
        //        }
        //    }
        //    return View(model.List);
        //}

        //[AccessPolicy(PageCode = "SHNCARDR010")]
        //public ActionResult DeliveredReport(DateTime? StartDate, DateTime? EndDate, string ShopCode = "")
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var model = new CartReportViewModel();

        //    if (ShopCode != "")
        //    {
        //        var shop = db.Shops.FirstOrDefault(i => i.Code == ShopCode);//Shop.Get(ShopCode);
        //        if (StartDate != null && EndDate != null)
        //        {
        //            DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
        //            DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
        //            model.List = db.Carts.Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.ShopCode == ShopCode && i.CartStatus == 6 && i.Status == 0)
        //           .AsEnumerable().GroupBy(i => i.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //           {
        //               Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
        //               ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
        //               OrderNumber = i.Any() ? i.FirstOrDefault().OrderNumber : "N/A",
        //               DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
        //               PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
        //               CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
        //               DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
        //               DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
        //           }).OrderByDescending(i => i.DateEncoded).ToList();
        //            model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
        //            model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
        //            model.ShopCode = ShopCode;
        //            model.ShopName = shop.Name;
        //        }
        //        else
        //        {
        //            model.List = db.Carts.Where(i => i.ShopCode == ShopCode && i.CartStatus == 6 && i.Status == 0)
        //          .AsEnumerable().GroupBy(i => i.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //          {
        //              Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
        //              ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
        //              OrderNumber = i.Any() ? i.FirstOrDefault().OrderNumber : "N/A",
        //              DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
        //              PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
        //              CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
        //              DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
        //              DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
        //          }).OrderByDescending(i => i.DateEncoded).ToList();
        //            model.ShopCode = ShopCode;
        //            model.ShopName = shop.Name;
        //        }

        //    }
        //    else
        //    {
        //        if (StartDate != null && EndDate != null)
        //        {
        //            DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
        //            DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
        //            model.List = db.Carts.Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.CartStatus == 6 && i.Status == 0)
        //          .AsEnumerable().GroupBy(i => i.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //          {
        //              Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
        //              ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
        //              OrderNumber = i.Any() ? i.FirstOrDefault().OrderNumber : "N/A",
        //              DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
        //              PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
        //              CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
        //              DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
        //              DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
        //          }).OrderByDescending(i => i.DateEncoded).ToList();
        //            model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
        //            model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
        //        }
        //        else
        //        {
        //            model.List = db.Carts.Where(i => i.CartStatus == 6 && i.Status == 0)
        //          .AsEnumerable().GroupBy(i => i.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //          {
        //              Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
        //              ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
        //              OrderNumber = i.Any() ? i.FirstOrDefault().OrderNumber : "N/A",
        //              DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
        //              PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
        //              CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
        //              DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
        //              DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
        //          }).OrderByDescending(i => i.DateEncoded).ToList();
        //        }
        //    }
        //    return View(model.List);
        //}

        //[AccessPolicy(PageCode = "SHNCARCR020")]
        //public ActionResult CancelledReport(DateTime? StartDate, DateTime? EndDate, string ShopCode = "")
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var model = new CartReportViewModel();

        //    if (ShopCode != "")
        //    {
        //        var shop = db.Shops.FirstOrDefault(i => i.Code == ShopCode);//Shop.Get(ShopCode);
        //        if (StartDate != null && EndDate != null)
        //        {
        //            DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
        //            DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
                    
        //            model.List = db.Carts.Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.ShopCode == ShopCode && i.CartStatus == 7 && i.Status == 0)
        //            .AsEnumerable().GroupBy(i => i.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //            {
        //                Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
        //                ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
        //                OrderNumber = i.Any() ? i.FirstOrDefault().OrderNumber : "N/A",
        //                DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
        //                PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
        //                CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
        //                DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
        //                DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
        //            }).OrderByDescending(i => i.DateEncoded).ToList();
        //            model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
        //            model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
        //            model.ShopCode = ShopCode;
        //            model.ShopName = shop.Name;
        //        }
        //        else
        //        {
        //            model.List = db.Carts.Where(i =>i.ShopCode == ShopCode && i.CartStatus == 7 && i.Status == 0)
        //           .AsEnumerable().GroupBy(i => i.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //           {
        //               Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
        //               ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
        //               OrderNumber = i.Any() ? i.FirstOrDefault().OrderNumber : "N/A",
        //               DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
        //               PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
        //               CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
        //               DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
        //               DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
        //           }).OrderByDescending(i => i.DateEncoded).ToList();
        //            model.ShopCode = ShopCode;
        //            model.ShopName = shop.Name;
        //        }

        //    }
        //    else
        //    {
        //        if (StartDate != null && EndDate != null)
        //        {
        //            DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
        //            DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
        //            model.List = db.Carts.Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.CartStatus == 7 && i.Status == 0)
        //           .AsEnumerable().GroupBy(i => i.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //           {
        //               Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
        //               ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
        //               OrderNumber = i.Any() ? i.FirstOrDefault().OrderNumber : "N/A",
        //               DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
        //               PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
        //               CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
        //               DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
        //               DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
        //           }).OrderByDescending(i => i.DateEncoded).ToList();
        //            model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
        //            model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
        //        }
        //        else
        //        {
        //            model.List = db.Carts.Where(i => i.CartStatus == 7 && i.Status == 0)
        //           .AsEnumerable().GroupBy(i => i.OrderNumber).Select(i => new CartReportViewModel.CartReportList
        //           {
        //               Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
        //               ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
        //               OrderNumber = i.Any() ? i.FirstOrDefault().OrderNumber : "N/A",
        //               DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
        //               PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
        //               CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
        //               DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
        //               DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
        //           }).OrderByDescending(i => i.DateEncoded).ToList();
        //        }
        //    }
        //    return View(model.List);
        //}

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
            var cart = db.Orders.FirstOrDefault(i => i.OrderNumber == model.Id);
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
                return RedirectToAction("List");
            }
            else
            {
                return View(model);
            }

        }

        //[AccessPolicy(PageCode = "SHNCARDAR024")]
        //public ActionResult DeliveredAmountReport(DateTime? StartDate, DateTime? EndDate, string ShopCode = "")
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var model = new CartReportViewModel();

        //    if (ShopCode != "")
        //    {
        //        var shop = db.Shops.FirstOrDefault(i => i.Code == ShopCode);//Shop.Get(ShopCode);
        //        if (StartDate != null && EndDate != null)
        //        {
        //            DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
        //            DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

        //            model.List = db.Payments.Join(db.Carts, p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
        //            .Where(i => i.p.DateEncoded >= startDatetFilter && i.p.DateEncoded <= endDateFilter && i.p.Status == 0 && i.c.CartStatus == 6 && i.c.ShopCode == ShopCode && i.p.PaymentMode == "Cash On Hand")
        //             .GroupBy(i => i.p.OrderNumber)
        //             .Select(i => new CartReportViewModel.CartReportList
        //             {
        //                 Code = i.Any() ? i.FirstOrDefault().p.Code : "",
        //                 ShopName = i.Any() ? i.FirstOrDefault().p.ShopName : "",
        //                 OrderNumber = i.Any() ? i.FirstOrDefault().p.OrderNumber : "",
        //                 DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "",
        //                 PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "",
        //                 CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 0,
        //                 DateEncoded = i.Any() ? i.FirstOrDefault().p.DateEncoded : DateTime.Now,
        //                 Amount = i.Any() ? i.FirstOrDefault().p.OriginalAmount : 0.0,
        //             }).OrderByDescending(i => i.DateEncoded).ToList();
        //            model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
        //            model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
        //            model.ShopCode = ShopCode;
        //            model.ShopName = shop.Name;
        //        }
        //        else
        //        {
        //            model.List = db.Payments.Join(db.Carts, p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
        //              .Where(i => i.p.Status == 0 && i.c.CartStatus == 6 && i.c.ShopCode == ShopCode && i.p.PaymentMode == "Cash On Hand")
        //              .GroupBy(i => i.p.OrderNumber)
        //              .Select(i => new CartReportViewModel.CartReportList
        //              {
        //                  Code = i.Any() ? i.FirstOrDefault().p.Code : "",
        //                  ShopName = i.Any() ? i.FirstOrDefault().p.ShopName : "",
        //                  OrderNumber = i.Any() ? i.FirstOrDefault().p.OrderNumber : "",
        //                  DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "",
        //                  PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "",
        //                  CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 0,
        //                  DateEncoded = i.Any() ? i.FirstOrDefault().p.DateEncoded : DateTime.Now,
        //                  Amount = i.Any() ? i.FirstOrDefault().p.OriginalAmount : 0.0,
        //              }).OrderByDescending(i => i.DateEncoded).ToList();
        //            model.ShopCode = ShopCode;
        //            model.ShopName = shop.Name;
        //        }

        //    }
        //    else
        //    {
        //        if (StartDate != null && EndDate != null)
        //        {
        //            DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
        //            DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

        //            model.List = db.Payments.Join(db.Carts, p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
        //            .Where(i => i.p.DateEncoded >= startDatetFilter && i.p.DateEncoded <= endDateFilter && i.p.Status == 0 && i.c.CartStatus == 6 && i.p.PaymentMode == "Cash On Hand")
        //             .GroupBy(i => i.p.OrderNumber)
        //             .Select(i => new CartReportViewModel.CartReportList
        //             {
        //                 Code = i.Any() ? i.FirstOrDefault().p.Code : "",
        //                 ShopName = i.Any() ? i.FirstOrDefault().p.ShopName : "",
        //                 OrderNumber = i.Any() ? i.FirstOrDefault().p.OrderNumber : "",
        //                 DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "",
        //                 PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "",
        //                 CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 0,
        //                 DateEncoded = i.Any() ? i.FirstOrDefault().p.DateEncoded : DateTime.Now,
        //                 Amount = i.Any() ? i.FirstOrDefault().p.OriginalAmount : 0.0,
        //             }).OrderByDescending(i => i.DateEncoded).ToList();
        //            model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
        //            model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
        //        }
        //        else
        //        {
        //            model.List = db.Payments.Join(db.Carts, p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
        //               .Where(i => i.p.Status == 0 && i.c.CartStatus == 6 && i.p.PaymentMode == "Cash On Hand")
        //               .GroupBy(i => i.p.OrderNumber)
        //               .Select(i => new CartReportViewModel.CartReportList
        //               {
        //                   Code = i.Any() ? i.FirstOrDefault().p.Code : "",
        //                   ShopName = i.Any() ? i.FirstOrDefault().p.ShopName : "",
        //                   OrderNumber = i.Any() ? i.FirstOrDefault().p.OrderNumber : "",
        //                   DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "",
        //                   PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "",
        //                   CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 0,
        //                   DateEncoded = i.Any() ? i.FirstOrDefault().p.DateEncoded : DateTime.Now,
        //                   Amount = i.Any() ? i.FirstOrDefault().p.OriginalAmount : 0.0,
        //               }).OrderByDescending(i => i.DateEncoded).ToList();
        //        }
        //    }
        //    return View(model.List);
        //}

        //[AccessPolicy(PageCode = "")]
        //public ActionResult ShopNowChat_ShopReport(DateTime? StartDate, DateTime? EndDate, string shopcode = "")
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var model = new CartReportViewModel();

        //    if (shopcode != "")
        //    {
        //        var shop = db.Shops.FirstOrDefault(i => i.Code == shopcode);//Shop.Get(shopcode);
        //        if (StartDate != null && EndDate != null)
        //        {
        //            DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
        //            DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

        //            model.List = db.Payments.Join(db.Carts, p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
        //            .Where(i => i.p.DateEncoded >= startDatetFilter && i.p.DateEncoded <= endDateFilter 
        //            && i.p.Status == 0 && i.c.CartStatus == 6 && i.c.ShopCode == shopcode)
        //             .GroupBy(i => i.p.OrderNumber)
        //             .Select(i => new CartReportViewModel.CartReportList
        //             {
        //                 Code = i.Any() ? i.FirstOrDefault().c.Code : "",
        //                 ShopName = i.Any() ? i.FirstOrDefault().p.ShopName : "",
        //                 OrderNumber = i.Any() ? i.FirstOrDefault().p.OrderNumber : "",
        //                 CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 0,
        //                 DateEncoded = i.Any() ? i.FirstOrDefault().p.DateEncoded : DateTime.Now,
        //                 OriginalAmount = i.Any() ? i.FirstOrDefault().p.OriginalAmount : 0.0,
        //                 ShopPaymentStatus = i.Any() ? i.FirstOrDefault().c.ShopPaymentStatus : 0
        //             }).OrderByDescending(i => i.DateEncoded).ToList();
        //            model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
        //            model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
        //            model.ShopCode = shopcode;
        //            model.ShopName = shop.Name;
        //        }
        //        else
        //        {
        //            model.List = db.Payments.Join(db.Carts, p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
        //              .Where(i => i.p.Status == 0 && i.c.CartStatus == 6 && i.c.ShopCode == shopcode)
        //              .GroupBy(i => i.p.OrderNumber)
        //              .Select(i => new CartReportViewModel.CartReportList
        //              {
        //                  Code = i.Any() ? i.FirstOrDefault().c.Code : "",
        //                  ShopName = i.Any() ? i.FirstOrDefault().p.ShopName : "",
        //                  OrderNumber = i.Any() ? i.FirstOrDefault().p.OrderNumber : "",
        //                  CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 0,
        //                  DateEncoded = i.Any() ? i.FirstOrDefault().p.DateEncoded : DateTime.Now,
        //                  OriginalAmount = i.Any() ? i.FirstOrDefault().p.OriginalAmount : 0.0,
        //                  ShopPaymentStatus = i.Any() ? i.FirstOrDefault().c.ShopPaymentStatus : 0
        //              }).OrderByDescending(i => i.DateEncoded).ToList();
        //            model.ShopCode = shopcode;
        //            model.ShopName = shop.Name;
        //        }
        //    }
        //    else
        //    {
        //        if (StartDate != null && EndDate != null)
        //        {
        //            DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
        //            DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

        //            model.List = db.Payments.Join(db.Carts, p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
        //            .Where(i => i.p.DateEncoded >= startDatetFilter && i.p.DateEncoded <= endDateFilter && i.p.Status == 0 && i.c.CartStatus == 6)
        //             .GroupBy(i => i.p.OrderNumber)
        //             .Select(i => new CartReportViewModel.CartReportList
        //             {
        //                 Code = i.Any() ? i.FirstOrDefault().c.Code : "",
        //                 ShopName = i.Any() ? i.FirstOrDefault().p.ShopName : "",
        //                 OrderNumber = i.Any() ? i.FirstOrDefault().p.OrderNumber : "",
        //                 CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 0,
        //                 DateEncoded = i.Any() ? i.FirstOrDefault().p.DateEncoded : DateTime.Now,
        //                 OriginalAmount = i.Any() ? i.FirstOrDefault().p.OriginalAmount : 0.0,
        //                 ShopPaymentStatus = i.Any() ? i.FirstOrDefault().c.ShopPaymentStatus : 0
        //             }).OrderByDescending(i => i.DateEncoded).ToList();
        //            model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
        //            model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
        //        }
        //        else
        //        {
        //            model.List = db.Payments.Join(db.Carts, p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
        //               .Where(i => i.p.Status == 0 && i.c.CartStatus == 6)
        //               .GroupBy(i => i.p.OrderNumber)
        //               .Select(i => new CartReportViewModel.CartReportList
        //               {
        //                   Code = i.Any() ? i.FirstOrDefault().c.Code : "",
        //                   ShopName = i.Any() ? i.FirstOrDefault().p.ShopName : "",
        //                   OrderNumber = i.Any() ? i.FirstOrDefault().p.OrderNumber : "",
        //                   CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 0,
        //                   DateEncoded = i.Any() ? i.FirstOrDefault().p.DateEncoded : DateTime.Now,
        //                   OriginalAmount = i.Any() ? i.FirstOrDefault().p.OriginalAmount : 0.0,
        //                   ShopPaymentStatus = i.Any() ? i.FirstOrDefault().c.ShopPaymentStatus : 0
        //               }).OrderByDescending(i => i.DateEncoded).ToList();
        //        }
        //    }
        //    return View(model);
        //}

        [AccessPolicy(PageCode = "")]
        public ActionResult DeliveryBoy_ShopNowChatReport(DateTime? StartDate, DateTime? EndDate, int deliveryboyId = 0)
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

        //[AccessPolicy(PageCode = "")]
        //public ActionResult ShopNowChat_DeliveryBoyReport(DateTime? StartDate, DateTime? EndDate, string deliveryboycode = "")
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var model = new CartReportViewModel();

        //    if (deliveryboycode != "")
        //    {
        //        if (StartDate != null && EndDate != null)
        //        {
        //            DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
        //            DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

        //            model.List = db.Carts.Join(db.ShopCharges, c => c.OrderNumber, sc => sc.OrderNumber, (c, sc) => new { c, sc })
        //                 .Join(db.Payments, cp => cp.c.OrderNumber, p => p.OrderNumber, (cp, p) => new { cp, p })
        //          .Where(i => i.cp.c.DateEncoded >= startDatetFilter && i.cp.c.DateEncoded <= endDateFilter && i.cp.c.CartStatus == 6 && i.cp.c.Status == 0 && i.cp.sc.DeliveryBoyCode == deliveryboycode)
        //                .GroupBy(i => i.cp.c.OrderNumber).AsEnumerable()
        //                .Select(i => new CartReportViewModel.CartReportList
        //                {
        //                    Code = i.Any() ? i.FirstOrDefault().cp.sc.Code : "",
        //                    DateEncoded = i.Any() ? i.FirstOrDefault().cp.c.DateEncoded : DateTime.Now,
        //                    OrderNumber = i.Any() ? i.FirstOrDefault().cp.c.OrderNumber : "",
        //                    DeliveryBoyCode = i.Any() ? i.FirstOrDefault().cp.sc.DeliveryBoyCode : "",
        //                    DeliveryBoyName = i.Any() ? i.FirstOrDefault().cp.sc.DeliveryBoyName : "",
        //                    DeliveryBoyPhoneNumber = i.Any() ? i.FirstOrDefault().cp.c.DeliveryBoyPhoneNumber : "",
        //                    GrossDeliveryCharge = i.Any() ? i.Sum(j => j.cp.sc.GrossDeliveryCharge) : 0.0,
        //                    DeliveryBoyPaymentStatus = i.Any() ? i.FirstOrDefault().cp.c.DeliveryBoyPaymentStatus : 0,
        //                    DeliveryRateSet = i.Any() ? i.FirstOrDefault().p.CreditType : 0,
        //                    Kilometer = getDeliveryBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode) != null ? (((Math.Acos(Math.Sin((i.FirstOrDefault().cp.c.Latitude * Math.PI / 180))
        //                    * Math.Sin((getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Latitude * Math.PI / 180))
        //                       + Math.Cos((i.FirstOrDefault().cp.c.Latitude * Math.PI / 180)) * Math.Cos((getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Latitude * Math.PI / 180))
        //                      * Math.Cos(((i.FirstOrDefault().cp.c.Longitude - getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Longitude)
        //                      * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344) : 0
        //                }).OrderBy(i => i.DeliveryBoyName).ToList();
        //        }
        //        else
        //        {
        //            model.List = db.Carts.Join(db.ShopCharges, c => c.OrderNumber, sc => sc.OrderNumber, (c, sc) => new { c, sc })
        //                 .Join(db.Payments, cp => cp.c.OrderNumber, p => p.OrderNumber, (cp, p) => new { cp, p })
        //          .Where(i => i.cp.c.CartStatus == 6 && i.cp.c.Status == 0 && i.cp.sc.DeliveryBoyCode == deliveryboycode)
        //                .GroupBy(i => i.cp.c.OrderNumber).AsEnumerable()
        //                .Select(i => new CartReportViewModel.CartReportList
        //                {
        //                    Code = i.Any() ? i.FirstOrDefault().cp.sc.Code : "",
        //                    DateEncoded = i.Any() ? i.FirstOrDefault().cp.c.DateEncoded : DateTime.Now,
        //                    OrderNumber = i.Any() ? i.FirstOrDefault().cp.c.OrderNumber : "",
        //                    DeliveryBoyCode = i.Any() ? i.FirstOrDefault().cp.sc.DeliveryBoyCode : "",
        //                    DeliveryBoyName = i.Any() ? i.FirstOrDefault().cp.sc.DeliveryBoyName : "",
        //                    DeliveryBoyPhoneNumber = i.Any() ? i.FirstOrDefault().cp.c.DeliveryBoyPhoneNumber : "",
        //                    GrossDeliveryCharge = i.Any() ? i.Sum(j => j.cp.sc.GrossDeliveryCharge) : 0.0,
        //                    DeliveryBoyPaymentStatus = i.Any() ? i.FirstOrDefault().cp.c.DeliveryBoyPaymentStatus : 0,
        //                    DeliveryRateSet = i.Any() ? i.FirstOrDefault().p.CreditType : 0,
        //                    Kilometer = getDeliveryBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode) != null ? (((Math.Acos(Math.Sin((i.FirstOrDefault().cp.c.Latitude * Math.PI / 180))
        //                    * Math.Sin((getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Latitude * Math.PI / 180))
        //                       + Math.Cos((i.FirstOrDefault().cp.c.Latitude * Math.PI / 180)) * Math.Cos((getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Latitude * Math.PI / 180))
        //                      * Math.Cos(((i.FirstOrDefault().cp.c.Longitude - getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Longitude)
        //                      * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344) : 0
        //                }).OrderBy(i => i.DeliveryBoyName).ToList();
        //        }
        //    }
        //    else
        //    {
        //        if (StartDate != null && EndDate != null)
        //        {
        //            DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
        //            DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

        //            model.List = db.Carts.Join(db.ShopCharges, c => c.OrderNumber, sc => sc.OrderNumber, (c, sc) => new { c, sc })
        //                .Join(db.Payments, cp => cp.c.OrderNumber, p => p.OrderNumber, (cp, p) => new { cp, p })
        //         .Where(i => i.cp.c.DateEncoded >= startDatetFilter && i.cp.c.DateEncoded <= endDateFilter && i.cp.c.CartStatus == 6 && i.cp.c.Status == 0)
        //               .GroupBy(i => i.cp.c.OrderNumber).AsEnumerable()
        //               .Select(i => new CartReportViewModel.CartReportList
        //               {
        //                   Code = i.Any() ? i.FirstOrDefault().cp.sc.Code : "",
        //                   DateEncoded = i.Any() ? i.FirstOrDefault().cp.c.DateEncoded : DateTime.Now,
        //                   OrderNumber = i.Any() ? i.FirstOrDefault().cp.c.OrderNumber : "",
        //                   DeliveryBoyCode = i.Any() ? i.FirstOrDefault().cp.sc.DeliveryBoyCode : "",
        //                   DeliveryBoyName = i.Any() ? i.FirstOrDefault().cp.sc.DeliveryBoyName : "",
        //                   DeliveryBoyPhoneNumber = i.Any() ? i.FirstOrDefault().cp.c.DeliveryBoyPhoneNumber : "",
        //                   GrossDeliveryCharge = i.Any() ? i.Sum(j => j.cp.sc.GrossDeliveryCharge) : 0.0,
        //                   DeliveryBoyPaymentStatus = i.Any() ? i.FirstOrDefault().cp.c.DeliveryBoyPaymentStatus : 0,
        //                   DeliveryRateSet = i.Any() ? i.FirstOrDefault().p.CreditType : 0,
        //                   Kilometer = getDeliveryBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode) != null ? (((Math.Acos(Math.Sin((i.FirstOrDefault().cp.c.Latitude * Math.PI / 180))
        //                    * Math.Sin((getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Latitude * Math.PI / 180))
        //                       + Math.Cos((i.FirstOrDefault().cp.c.Latitude * Math.PI / 180)) * Math.Cos((getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Latitude * Math.PI / 180))
        //                      * Math.Cos(((i.FirstOrDefault().cp.c.Longitude - getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Longitude)
        //                      * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344) : 0
        //               }).OrderBy(i => i.DeliveryBoyName).ToList();
        //        }
        //        else
        //        {
        //            model.List = db.Carts.Join(db.ShopCharges, c => c.OrderNumber, sc => sc.OrderNumber, (c, sc) => new { c, sc })
        //                .Join(db.Payments, cp=> cp.c.OrderNumber, p=> p.OrderNumber, (cp,p)=> new { cp,p})
        //          .Where(i => i.cp.c.CartStatus == 6 && i.cp.c.Status == 0)
        //                .GroupBy(i => i.cp.c.OrderNumber)
        //                 .AsEnumerable()
        //                .Select(i => new CartReportViewModel.CartReportList
        //                {
        //                    Code = i.Any() ? i.FirstOrDefault().cp.sc.Code : "",
        //                    DateEncoded = i.Any() ? i.FirstOrDefault().cp.c.DateEncoded : DateTime.Now,
        //                    OrderNumber = i.Any() ? i.FirstOrDefault().cp.c.OrderNumber : "",
        //                    DeliveryBoyCode = i.Any() ? i.FirstOrDefault().cp.sc.DeliveryBoyCode : "",
        //                    DeliveryBoyName = i.Any() ? i.FirstOrDefault().cp.sc.DeliveryBoyName : "",
        //                    DeliveryBoyPhoneNumber = i.Any() ? i.FirstOrDefault().cp.c.DeliveryBoyPhoneNumber : "",
        //                    GrossDeliveryCharge = i.Any() ? i.Sum(j => j.cp.sc.GrossDeliveryCharge) : 0.0,
        //                    DeliveryBoyPaymentStatus = i.Any() ? i.FirstOrDefault().cp.c.DeliveryBoyPaymentStatus : 0,
        //                    DeliveryRateSet = i.Any() ? i.FirstOrDefault().p.CreditType : 0,
        //                    Kilometer = getDeliveryBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode) != null?(((Math.Acos(Math.Sin((i.FirstOrDefault().cp.c.Latitude * Math.PI / 180)) 
        //                    * Math.Sin((getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Latitude * Math.PI / 180))
        //                       + Math.Cos((i.FirstOrDefault().cp.c.Latitude * Math.PI / 180)) * Math.Cos((getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Latitude * Math.PI / 180))
        //                      * Math.Cos(((i.FirstOrDefault().cp.c.Longitude - getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Longitude)
        //                      * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344): 0
        //                }).OrderBy(i => i.DeliveryBoyName).ToList();
        //        }
        //    }
        //    return View(model);
        //}

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

        public ActionResult DeliveryBoyAccept(int OrderNumber, int id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var order = db.Orders.FirstOrDefault(i => i.Id == id);
            var delivaryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == order.DeliveryBoyId && i.Status == 0);
            delivaryBoy.OnWork = 1;
            delivaryBoy.UpdatedBy = user.Name;
            delivaryBoy.DateUpdated = DateTime.Now;
            db.Entry(delivaryBoy).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            //var shopCharge = db.ShopCharges.FirstOrDefault(i => i.OrderNumber == OrderNumber);
            //var shop = db.Shops.FirstOrDefault(i => i.Id == shopCharge.ShopId);
            //var topup = db.TopUps.OrderByDescending(q => q.Id).FirstOrDefault(i => i.CustomerCode == shop.CustomerCode && i.CreditType == 1 && i.Status == 0);
            //if (topup != null)
            //{
            //    topup.CreditAmount = topup.CreditAmount - shopCharge.GrossDeliveryCharge;
            //    topup.DateUpdated = DateTime.Now;
            //    db.Entry(topup).State = System.Data.Entity.EntityState.Modified;
            //    db.SaveChanges();
            //}
            return RedirectToAction("Edit", "Cart", new { OrderNumber = OrderNumber, id = id });
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
            return RedirectToAction("Edit", "Cart", new { OrderNumber = OrderNumber, id = id });
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
            return RedirectToAction("Edit", "Cart", new { OrderNumber = OrderNumber, id = id });
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

