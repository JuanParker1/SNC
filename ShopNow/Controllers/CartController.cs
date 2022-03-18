using AutoMapper;
using Razorpay.Api;
using ShopNow.Base;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Net;
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
                config.CreateMap<Models.Order, CartListViewModel.CartList>();
                config.CreateMap<Models.Order, CartDetailsViewModel>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SNCCL059")]
        public ActionResult List(OrderListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Orders.Where(i => (model.Status == 0 ? i.Status != 6 && i.Status != 7 && i.Status != 9 && i.Status != 10 && i.Status != 0 && i.Status != -1 : i.Status == model.Status) && (model.ShopId != 0 ? i.ShopId == model.ShopId : true) && (model.IsPickupDrop == true ? i.IsPickupDrop == true : true) && (i.PickupDateTime != null ? SqlFunctions.DateDiff("minute", DateTime.Now, i.PickupDateTime) <= 30 : true))
                .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                .Join(db.Shops.Where(i => (!string.IsNullOrEmpty(model.District) ? i.DistrictName == model.District : true)), c => c.c.ShopId, s => s.Id, (c, s) => new { c, s })
            .Select(i => new OrderListViewModel.ListItem
            {
                Id = i.c.c.Id,
                ShopName = i.c.c.ShopName,
                OrderNumber = i.c.c.OrderNumber.ToString(),
                DeliveryAddress = i.c.c.DeliveryAddress,
                ShopPhoneNumber = i.c.c.ShopPhoneNumber,
                Status = i.c.c.Status,
                DeliveryBoyName = i.c.c.DeliveryBoyName ?? "N/A",
                PaymentMode = i.c.c.PaymentMode,
                Amount = i.c.c.IsPickupDrop == true ? (i.c.p.RefundAmount != null && i.c.p.RefundAmount != 0) ? i.c.c.NetTotal - (i.c.p.RefundAmount ?? 0) : i.c.c.TotalPrice : i.c.p.Amount - (i.c.p.RefundAmount ?? 0),
                CustomerPhoneNumber = i.c.c.CustomerPhoneNumber,
                RefundAmount = i.c.p.RefundAmount ?? 0,
                RefundRemark = i.c.p.RefundRemark ?? "",
                DateEncoded = i.c.c.DateEncoded,
                IsPickupDrop = i.c.c.IsPickupDrop,
                ShopDistrict = i.s.DistrictName,
                TotalPrice = i.c.c.TotalPrice,
                Distance = i.c.c.Distance
            }).OrderBy(i => i.Status).OrderByDescending(i => i.DateEncoded).ToList();
            int counter = 1;
            model.ListItems.ForEach(x => x.No = counter++);
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCP060")]
        public ActionResult Pending(CartListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.CartPendingLists = db.Orders.Where(i => (model.ShopId != 0 ? i.ShopId == model.ShopId : true) && i.Status == 2)
                           .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                           .Select(i => new CartListViewModel.CartPendingList
                           {
                               Id = i.c.Id,
                               ShopName = i.c.ShopName,
                               OrderNumber = i.c.OrderNumber.ToString(),
                               DeliveryAddress = i.c.DeliveryAddress,
                               ShopOwnerPhoneNumber = i.c.ShopOwnerPhoneNumber,
                               Status = i.c.Status,
                               DeliveryBoyName = i.c.DeliveryBoyName,
                               DateEncoded = i.c.DateEncoded,
                               Price = i.c.IsPickupDrop == true ? i.c.TotalPrice : i.c.NetTotal,
                               RefundAmount = i.p.RefundAmount ?? 0,
                               RefundRemark = i.p.RefundRemark ?? "",
                               PaymentMode = i.p.PaymentMode,
                               IsPickupDrop = i.c.IsPickupDrop
                           }).OrderByDescending(i => i.DateEncoded).ToList();
            int counter = 1;
            model.CartPendingLists.ForEach(x => x.No = counter++);
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCPL061")]
        public ActionResult PendingList(CartListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.CartPendingLists = db.Orders.Where(i => (model.ShopId != 0 ? i.ShopId == model.ShopId : true) && i.Status == 2 && SqlFunctions.DateDiff("minute", i.DateUpdated, DateTime.Now) >= 5)
                           .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                           .Select(i => new CartListViewModel.CartPendingList
                           {
                               Id = i.c.Id,
                               ShopName = i.c.ShopName,
                               OrderNumber = i.c.OrderNumber.ToString(),
                               DeliveryAddress = i.c.DeliveryAddress,
                               ShopOwnerPhoneNumber = i.c.ShopOwnerPhoneNumber,
                               Status = i.c.Status,
                               DeliveryBoyName = i.c.DeliveryBoyName,
                               DateEncoded = i.c.DateEncoded,
                               Price = i.c.IsPickupDrop == true ? i.c.TotalPrice : i.c.NetTotal,
                               RefundAmount = i.p.RefundAmount ?? 0,
                               RefundRemark = i.p.RefundRemark ?? "",
                               PaymentMode = i.p.PaymentMode,
                               IsPickupDrop = i.c.IsPickupDrop
                           }).OrderByDescending(i => i.DateEncoded).ToList();
            int counter = 1;
            model.CartPendingLists.ForEach(x => x.No = counter++);
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCOP062")]
        public ActionResult OrderPrepared(CartListViewModel model) //Order Processing
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.CartPreparedLists = db.Orders.Where(i => (model.ShopId != 0 ? i.ShopId == model.ShopId : true) && (i.Status == 3 || i.Status == 4 || i.Status == 8))
                .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                .Select(i => new CartListViewModel.CartPreparedList
                {
                    Id = i.c.Id,
                    ShopName = i.c.ShopName,
                    OrderNumber = i.c.OrderNumber.ToString(),
                    DeliveryAddress = i.c.DeliveryAddress,
                    ShopPhoneNumber = i.c.ShopPhoneNumber,
                    Status = i.c.Status,
                    DeliveryBoyName = i.c.DeliveryBoyName,
                    DateEncoded = i.c.DateEncoded,
                    RefundAmount = i.p.RefundAmount ?? 0,
                    RefundRemark = i.p.RefundRemark ?? "",
                    PaymentMode = i.p.PaymentMode,
                    DeliveryBoyPhoneNumber = i.c.DeliveryBoyPhoneNumber ?? "Not Assigned",
                    Price = i.c.IsPickupDrop == true ? (i.p.RefundAmount != null && i.p.RefundAmount != 0) ? i.c.NetTotal - (i.p.RefundAmount ?? 0) : i.c.TotalPrice : i.p.Amount - (i.p.RefundAmount ?? 0),
                    IsPickupDrop = i.c.IsPickupDrop
                }).OrderByDescending(i => i.DateEncoded).ToList();
            int counter = 1;
            model.CartPreparedLists.ForEach(x => x.No = counter++);
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCDA063")]
        public ActionResult DeliveryAgentAssigned(CartListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.DeliveryAgentAssignedLists = db.Orders.Where(i => (model.ShopId != 0 ? i.ShopId == model.ShopId : true) && i.Status == 4 && i.DeliveryBoyId != 0 && i.DeliveryBoyOnWork == 0)
                //.Join(db.DeliveryBoys.Where(i => i.isAssign == 1 && i.OnWork == 0), c => c.DeliveryBoyId, d => d.Id, (c, d) => new { c, d })
                .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                .Select(i => new CartListViewModel.DeliveryAgentAssignedList
                {
                    Id = i.c.Id,
                    ShopName = i.c.ShopName,
                    OrderNumber = i.c.OrderNumber.ToString(),
                    Status = i.c.Status,
                    DeliveryBoyName = i.c.DeliveryBoyName,
                    DateEncoded = i.c.DateEncoded,
                    RefundAmount = i.p.RefundAmount ?? 0,
                    RefundRemark = i.p.RefundRemark ?? "",
                    PaymentMode = i.p.PaymentMode,
                    DeliveryBoyPhoneNumber = i.c.DeliveryBoyPhoneNumber,
                    IsPickupDrop = i.c.IsPickupDrop
                }).OrderByDescending(i => i.DateEncoded).ToList();
            int counter = 1;
            model.DeliveryAgentAssignedLists.ForEach(x => x.No = counter++);
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCDAL064")]
        public ActionResult DeliveryAgentAssignedList(CartListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.DeliveryAgentAssignedLists = db.Orders.Where(i => (model.ShopId != 0 ? i.ShopId == model.ShopId : true) && i.Status == 4 && i.DeliveryBoyId != 0 && i.DeliveryBoyOnWork == 0 && SqlFunctions.DateDiff("minute", i.DateUpdated, DateTime.Now) >= 5)
                // .Join(db.DeliveryBoys.Where(i => i.isAssign == 1 && i.OnWork == 0), c => c.DeliveryBoyId, d => d.Id, (c, d) => new { c, d })
                .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                .Select(i => new CartListViewModel.DeliveryAgentAssignedList
                {
                    Id = i.c.Id,
                    ShopName = i.c.ShopName,
                    OrderNumber = i.c.OrderNumber.ToString(),
                    Status = i.c.Status,
                    DeliveryBoyName = i.c.DeliveryBoyName,
                    DateEncoded = i.c.DateEncoded,
                    RefundAmount = i.p.RefundAmount ?? 0,
                    RefundRemark = i.p.RefundRemark ?? "",
                    PaymentMode = i.p.PaymentMode,
                    DeliveryBoyPhoneNumber = i.c.DeliveryBoyPhoneNumber,
                    Price = i.c.IsPickupDrop == true ? (i.p.RefundAmount != null && i.p.RefundAmount != 0) ? i.c.NetTotal - (i.p.RefundAmount ?? 0) : i.c.TotalPrice : i.p.Amount - (i.p.RefundAmount ?? 0),
                    IsPickupDrop = i.c.IsPickupDrop
                }).OrderByDescending(i => i.DateEncoded).ToList();
            int counter = 1;
            model.DeliveryAgentAssignedLists.ForEach(x => x.No = counter++);
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCWP065")]
        public ActionResult WaitingForPickup(CartListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.PickupLists = db.Orders.Where(i => i.Status == 4 && i.DeliveryBoyId != 0 && i.DeliveryBoyOnWork == 1 && (model.ShopId != 0 ? i.ShopId == model.ShopId : true))
                //.Join(db.DeliveryBoys.Where(i => i.isAssign == 1 && i.OnWork == 1), c => c.DeliveryBoyId, d => d.Id, (c, d) => new { c, d })
                .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                 .Select(i => new CartListViewModel.PickupList
                 {
                     Id = i.c.Id,
                     ShopName = i.c.ShopName,
                     OrderNumber = i.c.OrderNumber.ToString(),
                     DeliveryBoyPhoneNumber = i.c.DeliveryBoyPhoneNumber,
                     Status = i.c.Status,
                     DeliveryBoyName = i.c.DeliveryBoyName,
                     DateEncoded = i.c.DateEncoded,
                     RefundAmount = i.p.RefundAmount ?? 0,
                     RefundRemark = i.p.RefundRemark ?? "",
                     PaymentMode = i.p.PaymentMode,
                     Amount = i.c.IsPickupDrop == true ? (i.p.RefundAmount != null && i.p.RefundAmount != 0) ? i.c.NetTotal - (i.p.RefundAmount ?? 0) : i.c.TotalPrice : i.p.Amount - (i.p.RefundAmount ?? 0),
                     IsPickupDrop = i.c.IsPickupDrop
                 }).OrderByDescending(i => i.DateEncoded).ToList();
            int counter = 1;
            model.PickupLists.ForEach(x => x.No = counter++);
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCWPL066")]
        public ActionResult WaitingForPickupList(CartListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.PickupLists = db.Orders.Where(i => i.Status == 4 && i.DeliveryBoyId != 0 && i.DeliveryBoyOnWork == 1 && (model.ShopId != 0 ? i.ShopId == model.ShopId : true) && SqlFunctions.DateDiff("minute", i.DateUpdated, DateTime.Now) >= 15)
               //.Join(db.DeliveryBoys.Where(i => i.isAssign == 1 && i.OnWork == 1), c => c.DeliveryBoyId, d => d.Id, (c, d) => new { c, d })
               .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                .Select(i => new CartListViewModel.PickupList
                {
                    Id = i.c.Id,
                    ShopName = i.c.ShopName,
                    OrderNumber = i.c.OrderNumber.ToString(),
                    DeliveryBoyPhoneNumber = i.c.DeliveryBoyPhoneNumber,
                    Status = i.c.Status,
                    DeliveryBoyName = i.c.DeliveryBoyName,
                    DateEncoded = i.c.DateEncoded,
                    RefundAmount = i.p.RefundAmount ?? 0,
                    RefundRemark = i.p.RefundRemark ?? "",
                    PaymentMode = i.p.PaymentMode,
                   // Amount = i.c.IsPickupDrop == true ? i.c.TotalPrice - (i.p.RefundAmount ?? 0) : i.p.Amount - (i.p.RefundAmount ?? 0),
                   Amount = i.c.IsPickupDrop == true ? (i.p.RefundAmount != null && i.p.RefundAmount != 0) ? i.c.NetTotal - (i.p.RefundAmount ?? 0) : i.c.TotalPrice : i.p.Amount - (i.p.RefundAmount ?? 0),
                    IsPickupDrop = i.c.IsPickupDrop
                }).OrderByDescending(i => i.DateEncoded).ToList();
            int counter = 1;
            model.PickupLists.ForEach(x => x.No = counter++);
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCOW067")]
        public ActionResult OnTheWay(CartListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.OntheWayLists = db.Orders.Where(i => (model.ShopId != 0 ? i.ShopId == model.ShopId : true) && i.Status == 5)
                .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                .Select(i => new CartListViewModel.OntheWayList
                {
                    Id = i.c.Id,
                    ShopName = i.c.ShopName,
                    OrderNumber = i.c.OrderNumber.ToString(),
                    DeliveryBoyPhoneNumber = i.c.DeliveryBoyPhoneNumber,
                    Status = i.c.Status,
                    DateEncoded = i.c.DateEncoded,
                    Amount = i.c.IsPickupDrop == true ? (i.p.RefundAmount != null && i.p.RefundAmount != 0) ? i.c.NetTotal - (i.p.RefundAmount ?? 0) : i.c.TotalPrice : i.p.Amount - (i.p.RefundAmount ?? 0),
                    RefundAmount = i.p.RefundAmount ?? 0,
                    RefundRemark = i.p.RefundRemark ?? "",
                    PaymentMode = i.p.PaymentMode,
                    IsPickupDrop = i.c.IsPickupDrop
                }).OrderByDescending(i => i.DateEncoded).ToList();
            int counter = 1;
            model.OntheWayLists.ForEach(x => x.No = counter++);
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCOWL068")]
        public ActionResult OnTheWayList(CartListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.OntheWayLists = db.Orders.Where(i => (model.ShopId != 0 ? i.ShopId == model.ShopId : true) && i.Status == 5 && SqlFunctions.DateDiff("minute", i.DateUpdated, DateTime.Now) >= 15)
                .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                .Select(i => new CartListViewModel.OntheWayList
                {
                    Id = i.c.Id,
                    ShopName = i.c.ShopName,
                    OrderNumber = i.c.OrderNumber.ToString(),
                    DeliveryBoyPhoneNumber = i.c.DeliveryBoyPhoneNumber,
                    Status = i.c.Status,
                    DateEncoded = i.c.DateEncoded,
                    RefundAmount = i.p.RefundAmount ?? 0,
                    RefundRemark = i.p.RefundRemark ?? "",
                    PaymentMode = i.p.PaymentMode,
                    Amount = i.c.IsPickupDrop == true ? (i.p.RefundAmount != null && i.p.RefundAmount != 0) ? i.c.NetTotal - (i.p.RefundAmount ?? 0) : i.c.TotalPrice : i.p.Amount - (i.p.RefundAmount ?? 0),
                    IsPickupDrop = i.c.IsPickupDrop
                }).OrderByDescending(i => i.DateEncoded).ToList();
            int counter = 1;
            model.OntheWayLists.ForEach(x => x.No = counter++);
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCD069")]
        public ActionResult Delivered(CartListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.DeliveredLists = db.Orders.Where(i => (model.ShopId != 0 ? i.ShopId == model.ShopId : true) && i.Status == 6 && (model.IsPickupDrop == true ? i.IsPickupDrop == true : true))
                .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                .AsEnumerable()
                .Select(i => new CartListViewModel.DeliveredList
                {
                    Id = i.c.Id,
                    ShopName = i.c.ShopName,
                    OrderNumber = i.c.OrderNumber.ToString(),
                    CustomerPhoneNumber = i.c.CustomerPhoneNumber,
                    Status = i.c.Status,
                    DateEncoded = i.c.DateEncoded,
                    DeliveredTime = i.c.DeliveredTime == null ? i.c.DateUpdated : i.c.DeliveredTime,
                    Amount = i.c.IsPickupDrop == true ? (i.p.RefundAmount != null && i.p.RefundAmount != 0) ? i.c.NetTotal - (i.p.RefundAmount ?? 0) : i.c.TotalPrice : i.p.Amount - (i.p.RefundAmount ?? 0),
                    RefundAmount = i.p.RefundAmount ?? 0,
                    RefundRemark = i.p.RefundRemark ?? "",
                    PaymentMode = i.p.PaymentMode,
                    // OrderPeriod = Math.Round((i.c.DateUpdated - i.c.DateEncoded).TotalMinutes),
                    OrderPeriod = i.c.DeliveredTime != null ? Math.Round((i.c.DeliveredTime.Value - i.c.DateEncoded).TotalMinutes) : Math.Round((i.c.DateUpdated - i.c.DateEncoded).TotalMinutes),
                    ShopAcceptedTime = i.c.ShopAcceptedTime != null ? Math.Round((i.c.ShopAcceptedTime.Value - i.c.DateEncoded).TotalMinutes) : 0,
                    IsPickupDrop = i.c.IsPickupDrop
                }).OrderByDescending(i => i.DateEncoded).ToList();
            int counter = 1;
            model.DeliveredLists.ForEach(x => x.No = counter++);
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCDR070")]
        public ActionResult DeliveredReport(CartReportViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.StartDate = model.StartDate == null ? DateTime.Now : model.StartDate;
            model.DeliveredReportLists = db.Orders.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(model.StartDate) && i.Status == 6)
                .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                .AsEnumerable()
            .Select(i => new CartReportViewModel.DeliveredReportList
            {
                Id = i.c.Id,
                ShopName = i.c.ShopName,
                OrderNumber = i.c.OrderNumber.ToString(),
                DeliveryAddress = i.c.DeliveryAddress,
                PhoneNumber = i.c.CustomerPhoneNumber,
                DateEncoded = i.c.DateEncoded,
                PaymentMode = i.c.PaymentMode,
                Amount = i.c.IsPickupDrop == true ? (i.p.RefundAmount != null && i.p.RefundAmount != 0) ? i.c.NetTotal - (i.p.RefundAmount ?? 0) : i.c.TotalPrice : i.p.Amount - (i.p.RefundAmount ?? 0),
            }).OrderByDescending(i => i.DateEncoded).ToList();
            int counter = 1;
            model.DeliveredReportLists.ForEach(x => x.No = counter++);
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCC071")]
        public ActionResult Cancelled(CartListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.CancelledLists = db.Orders.Where(i => i.Status == 7 && (model.ShopId != 0 ? i.ShopId == model.ShopId : true))
                        .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                        .AsEnumerable()
                        .Select(i => new CartListViewModel.CancelledList
                        {
                            Id = i.c.Id,
                            ShopName = i.c.ShopName,
                            OrderNumber = i.c.OrderNumber.ToString(),
                            CustomerPhoneNumber = i.c.CustomerPhoneNumber,
                            Status = i.c.Status,
                            PaymentMode = i.p.PaymentMode,
                            //Amount = i.p.Amount,
                            Amount = i.c.IsPickupDrop == true ? i.c.TotalPrice : i.p.Amount,
                            DateEncoded = i.c.DateEncoded,
                            ShopCancelledTime = i.c.ShopAcceptedTime,
                            ShopCancelPeriod = i.c.ShopAcceptedTime != null ? Math.Round((i.c.ShopAcceptedTime.Value - i.c.DateEncoded).TotalMinutes) : 0,
                            CancelledRemark = i.c.CancelledRemark
                        }).OrderByDescending(i => i.DateEncoded).ToList();
            int counter = 1;
            model.CancelledLists.ForEach(x => x.No = counter++);
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCCR072")]
        public ActionResult CancelledReport(CartReportViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.StartDate = model.StartDate == null ? DateTime.Now : model.StartDate;

            model.CancelledReportLists = db.Orders.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(model.StartDate) && i.Status == 7)
            .Select(i => new CartReportViewModel.CancelledReportList
            {
                Id = i.Id,
                ShopName = i.ShopName,
                OrderNumber = i.OrderNumber.ToString(),
                DeliveryAddress = i.DeliveryAddress,
                PhoneNumber = i.CustomerPhoneNumber,
                DateEncoded = i.DateEncoded,
                CancelledRemark = i.CancelledRemark
            }).OrderByDescending(i => i.DateEncoded).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCCC073")]
        public ActionResult CustomerCancelled(CartListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["User"]);
            ViewBag.Name = user.Name;
            model.CustomerCancelledLists = db.Orders.Where(i => i.Status == 9 && ((model.ShopId != 0) ? i.ShopId == model.ShopId : true))
                .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                .AsEnumerable()
                .Select(i => new CartListViewModel.CustomerCancelledList
                {
                    Id = i.c.Id,
                    OrderNumber = i.c.OrderNumber.ToString(),
                    ShopName = i.c.ShopName,
                    CustomerPhoneNumber = i.c.CustomerPhoneNumber,
                    Status = i.c.Status,
                    Amount = i.c.IsPickupDrop == true ? (i.p.RefundAmount != null && i.p.RefundAmount != 0) ? i.c.NetTotal - (i.p.RefundAmount ?? 0) : i.c.TotalPrice : i.p.Amount - (i.p.RefundAmount ?? 0),
                    PaymentMode = i.p.PaymentMode,
                    DateEncoded = i.c.DateEncoded,
                    CustomerCancelledTime = i.c.DateUpdated
                }).OrderByDescending(i => i.DateEncoded).ToList();
            int counter = 1;
            model.CustomerCancelledLists.ForEach(x => x.No = counter++);
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCCNP074")]
        public ActionResult CustomerNotPickupList(CartListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.CustomerNotPickupLists = db.Orders.Where(i => i.Status == 10 && ((model.ShopId != 0) ? i.ShopId == model.ShopId : true))
                .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                .AsEnumerable()
                .Select(i => new CartListViewModel.CustomerNotPickupList
                {
                    Id = i.c.Id,
                    ShopName = i.c.ShopName,
                    OrderNumber = i.c.OrderNumber.ToString(),
                    CustomerPhoneNumber = i.c.CustomerPhoneNumber,
                    Status = i.c.Status,
                    DateEncoded = i.c.DateEncoded,
                    DateUpdated = i.c.DateUpdated,
                    Amount = i.c.IsPickupDrop == true ? (i.p.RefundAmount != null && i.p.RefundAmount != 0) ? i.c.NetTotal - (i.p.RefundAmount ?? 0) : i.c.TotalPrice : i.p.Amount - (i.p.RefundAmount ?? 0),
                    RefundAmount = i.p.RefundAmount ?? 0,
                    RefundRemark = i.p.RefundRemark ?? "",
                    PaymentMode = i.p.PaymentMode,
                    OrderPeriod = Math.Round((i.c.DateUpdated - i.c.DateEncoded).TotalMinutes),
                    ShopAcceptedTime = i.c.ShopAcceptedTime != null ? Math.Round((i.c.ShopAcceptedTime.Value - i.c.DateEncoded).TotalMinutes) : 0,

                }).OrderByDescending(i => i.DateEncoded).ToList();
            int counter = 1;
            model.CustomerNotPickupLists.ForEach(x => x.No = counter++);
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCD075")]
        public ActionResult Details(string id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dId = AdminHelpers.DCodeLong(id);
            Models.Order order = db.Orders.FirstOrDefault(i => i.Id == dId);
            var model = new CartDetailsViewModel();
            if (order != null)
            {
                var shop = db.Shops.FirstOrDefault(i => i.Id == order.ShopId);
                _mapper.Map(order, model);
                model.ImagePathLists = db.CustomerPrescriptionImages.Where(i => i.CustomerPrescriptionId == order.CustomerPrescriptionId)
                      .Select(i => new CartDetailsViewModel.ImagePathList
                      {
                          ImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath
                      }).ToList();
                model.ShopAddress = shop.Address;
                //var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == order.DeliveryBoyId);
                //if (deliveryBoy != null)
                //{
                //    model.DeliveryBoyIsAssign = deliveryBoy.isAssign;
                //    model.DeliveryBoyOnWork = deliveryBoy.OnWork;
                //}
                model.ListItems = db.OrderItems.Where(i => i.OrderId == order.Id)
                    .Select(i => new CartDetailsViewModel.ListItem
                    {
                        Id = i.Id,
                        BrandName = i.BrandName,
                        CategoryName = i.CategoryName,
                        ImagePath = i.ImagePath,
                        Price = i.Price,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        AddonType = i.AddOnType,
                        HasAddon = i.HasAddon,
                        UpdatedBy = i.UpdatedBy,
                        UpdatedTime = i.UpdatedTime,
                        UpdateRemarks = i.UpdateRemarks,
                        AddonListItems = db.OrderItemAddons.Where(a => a.OrderItemId == i.Id)
                        .Select(a => new CartDetailsViewModel.ListItem.AddonListItem
                        {
                            AddonName = a.AddonName,
                            AddonPrice = a.AddonPrice,
                            CrustName = a.CrustName,
                            PortionName = a.PortionName,
                            PortionPrice = a.PortionPrice
                        }).ToList()
                    }).ToList();

                var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == order.OrderNumber);
                if (payment != null)
                {
                    model.RefundAmount = payment.RefundAmount;
                    model.RefundRemark = payment.RefundRemark;
                    model.PaymentMode = payment.PaymentMode;
                }

                if (order.IsPrescriptionOrder)
                {
                    var prescription = db.CustomerPrescriptions.FirstOrDefault(i => i.Id == order.CustomerPrescriptionId);
                    if (prescription != null)
                        model.CustomerPrescriptionRemarks = prescription.Remarks;
                }
            }
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCPS076")]
        public ActionResult PickupSlip(int OrderNumber, string id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dInt = AdminHelpers.DCodeLong(id);
            if (string.IsNullOrEmpty(dInt.ToString()))
                return HttpNotFound();
            var cart = db.Orders.FirstOrDefault(i => i.Id == dInt);
            var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == OrderNumber);
            var shopBill = db.ShopBillDetails.FirstOrDefault(i => i.OrderNumber == OrderNumber);
            var model = new CartListViewModel();
            if (cart != null)
            {
                model.Id = cart.Id;
                model.OrderNumber = OrderNumber;
                model.CustomerId = cart.CustomerId;
                model.CustomerName = cart.CustomerName;
                model.Status = cart.Status;
                model.ShopName = cart.ShopName;
                model.DeliveryAddress = cart.DeliveryAddress;
                model.CustomerPhoneNumber = cart.CustomerPhoneNumber;
                model.DeliveryBoyName = cart.DeliveryBoyName;
                model.DateEncoded = cart.DateEncoded;
                //var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == cart.DeliveryBoyId);
                //if (deliveryBoy != null)
                //{
                //    model.isAssign = deliveryBoy.isAssign;
                //    model.OnWork = deliveryBoy.OnWork;
                //}
            }
            if (shopBill != null)
            {
                model.BillNo = shopBill.BillNo;
                model.BillAmount = shopBill.BillAmount;

            }
            if (payment != null)
            {
                model.RefundAmount = payment.RefundAmount;
                model.RefundRemark = payment.RefundRemark;
            }
            if (cart != null && payment != null && shopBill != null)
            {
                model.DifferenceAmount = Math.Round(shopBill.BillAmount - (cart.TotalPrice - (payment.RefundAmount ?? 0)), 2);
                model.DifferencePercentage = Math.Round(((shopBill.BillAmount - (cart.TotalPrice - (payment.RefundAmount ?? 0))) / shopBill.BillAmount) * 100, 2);
            }
            model.List = db.OrderItems.Where(i => i.OrdeNumber == OrderNumber && i.Status == 0)
            .Select(i => new CartListViewModel.CartList
            {
                Id = i.Id,
                BrandName = i.BrandName,
                CategoryName = i.CategoryName,
                ShopName = cart.ShopName,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Qty = i.Quantity,
                Price = i.Price,
                Status = cart.Status,
                PhoneNumber = cart.CustomerPhoneNumber,
                ImagePath = i.ImagePath == "N/A" ? null : i.ImagePath,
                SinglePrice = i.UnitPrice,
                MRPPrice = i.MRPPrice
            }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCE077")]
        public ActionResult Edit(int OrderNumber, string id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dInt = AdminHelpers.DCodeLong(id);
            var cart = db.Orders.FirstOrDefault(i => i.Id == dInt);
            var payment = db.Payments.FirstOrDefault(i => OrderNumber == cart.OrderNumber);
            var model = new CartListViewModel();
            if (cart != null)
            {
                model.Id = cart.Id;
                model.OrderNumber = OrderNumber;
                model.CustomerId = cart.CustomerId;
                model.CustomerName = cart.CustomerName;
                model.Status = cart.Status;
                model.ShopName = cart.ShopName;
                model.ShopPhoneNumber = cart.ShopPhoneNumber;
                model.DeliveryAddress = cart.DeliveryAddress;
                model.CustomerPhoneNumber = cart.CustomerPhoneNumber;
                model.DeliveryBoyId = cart.DeliveryBoyId;
                model.DeliveryBoyName = cart.DeliveryBoyName;
                model.DeliveryBoyPhoneNumber = cart.DeliveryBoyPhoneNumber;
                model.DateEncoded = cart.DateEncoded;
                model.PenaltyAmount = cart.PenaltyAmount;
                model.WaitingCharge = cart.WaitingCharge;
                model.TotalPrice = cart.TotalPrice - (payment.RefundAmount ?? 0);
                model.NetTotal = cart.NetTotal - (payment.RefundAmount ?? 0);
                model.PaymentMode = cart.PaymentMode;
                model.PrescriptionImagePath = cart.PrescriptionImagePath;
                model.IsPickupDrop = cart.IsPickupDrop;
                model.Remarks = cart.Remarks;
                model.ImagePathLists = db.CustomerPrescriptionImages.Where(i => i.CustomerPrescriptionId == cart.CustomerPrescriptionId)
                       .Select(i => new CartListViewModel.ImagePathList
                       {
                           ImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath
                       }).ToList();
                //var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == cart.DeliveryBoyId);
                //if (deliveryBoy != null)
                //{
                //    model.isAssign = deliveryBoy.isAssign;
                //    model.OnWork = deliveryBoy.OnWork;
                //}
                model.OnWork = cart.DeliveryBoyOnWork;
                model.Latitude = cart.Latitude;
                model.Longitude = cart.Longitude;
                //var refundamount = db.Payments.FirstOrDefault(i => i.RefundAmount == model.RefundAmount);
                //if(refundamount != null)
                //{
                //    model.Amount = refundamount.Amount - (refundamount.RefundAmount ?? 0);
                //    model.RefundRemark = refundamount.RefundRemark ?? "";
                //}
            }
            model.List = db.OrderItems.Where(i => i.OrderId == cart.Id && i.Status == 0).Select(i => new CartListViewModel.CartList
            {
                Id = i.Id,
                BrandName = i.BrandName,
                CategoryName = i.CategoryName,
                ShopName = cart.ShopName,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Qty = i.Quantity,
                Price = i.Price,
                Status = cart.Status,
                PhoneNumber = cart.CustomerPhoneNumber,
                ImagePath = i.ImagePath == "N/A" ? null : i.ImagePath,
                SinglePrice = i.UnitPrice
            }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCAD078")]
        public ActionResult AssignDeliveryBoy(int OrderNumber)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var order = db.Orders.Where(i => i.OrderNumber == OrderNumber).FirstOrDefault();
            var shop = db.Shops.FirstOrDefault(i => i.Id == order.ShopId);
            var model = new CartAssignDeliveryBoyViewModel();
            model.OrderId = order.Id;
            DateTime date = DateTime.Now;

            var amount = (from i in db.Orders
                          where i.OrderNumber == OrderNumber && i.Status == 6 && i.DateUpdated.Year == date.Year && i.DateUpdated.Month == date.Month && i.DateUpdated.Day == date.Day
                          select (Double?)i.DeliveryCharge).Sum() ?? 0;

            model.Lists = db.DeliveryBoys
               .Where(i => i.Active == 1 && i.Status == 0)
                 .AsEnumerable()
                 .Select(i => new CartAssignDeliveryBoyViewModel.CartAssignList
                 {
                     Id = i.Id,
                     Name = i.Name,
                     Status = i.Status,
                     Amount = amount,
                     Meters = (((Math.Acos(Math.Sin((shop.Latitude * Math.PI / 180)) * Math.Sin((i.Latitude * Math.PI / 180)) + Math.Cos((shop.Latitude * Math.PI / 180)) * Math.Cos((i.Latitude * Math.PI / 180))
                 * Math.Cos(((shop.Longitude - i.Longitude) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344)
                 }).Where(i => i.Meters < 8000 && i.Status == 0).ToList();

            return View(model);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SNCCAD078")]
        public ActionResult AssignDeliveryBoy(CartAssignDeliveryBoyViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var cart = db.Orders.FirstOrDefault(i => i.Id == model.OrderId);
            if (cart != null && model.DeliveryBoyId != 0)
            {
                var delivery = db.DeliveryBoys.FirstOrDefault(i => i.Id == model.DeliveryBoyId);

                cart.DeliveryBoyId = delivery.Id;
                cart.DeliveryBoyName = delivery.Name;
                cart.DeliveryBoyPhoneNumber = delivery.PhoneNumber;
                cart.Status = 4;
                cart.DateUpdated = DateTime.Now;
                cart.UpdatedBy = user.Name;
                db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                delivery.isAssign = 1;
                delivery.DateUpdated = DateTime.Now;
                delivery.UpdatedBy = user.Name;
                db.Entry(delivery).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                if (delivery.CustomerId != 0)
                {
                    var fcmToken = (from c in db.Customers
                                    where c.Id == delivery.CustomerId
                                    select c.FcmTocken ?? "").FirstOrDefault().ToString();
                    Helpers.PushNotification.SendbydeviceId("You have received new order. Accept Soon", "ShopNowChat", "a.mp3", fcmToken.ToString());
                }
                //Customer
                if (cart.CustomerId != 0)
                {
                    var fcmTokenCustomer = (from c in db.Customers
                                            where c.Id == cart.CustomerId
                                            select c.FcmTocken ?? "").FirstOrDefault().ToString();
                    Helpers.PushNotification.SendbydeviceId($"Delivery Boy {cart.DeliveryBoyName} is Assigned for your Order.", "ShopNowChat", "../../assets/b.mp3", fcmTokenCustomer.ToString());
                }
                return RedirectToAction("List");
            }
            else
            {
                return View(model);
            }
        }

        [AccessPolicy(PageCode = "SNCCDHR079")]
        // public ActionResult DeliveryBoyCashHandoverReport(DateTime? StartDate, DateTime? EndDate, int deliveryboyId = 0)
        public ActionResult DeliveryBoyCashHandoverReport(CartReportViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            // var model = new CartReportViewModel();

            model.List = db.Payments.Where(i => i.Status == 0 && i.PaymentMode == "Cash On Hand" && ((model.StartDate != null && model.EndDate != null) ? DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.StartDate) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.EndDate) : true))
                .Join(db.Orders.Where(i => i.Status == 6 && (model.DeliveryBoyId != 0 ? i.DeliveryBoyId == model.DeliveryBoyId : true)), p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
                   .Select(i => new CartReportViewModel.CartReportList
                   {
                       Id = i.c.Id,
                       OrderNumber = i.p.OrderNumber,
                       DeliveryBoyPhoneNumber = i.c.DeliveryBoyPhoneNumber,
                       DeliveryBoyId = i.c.DeliveryBoyId,
                       DeliveryBoyName = i.c.DeliveryBoyName,
                       // Amount = i.p.Amount - (i.p.RefundAmount ?? 0),
                       Amount = i.c.IsPickupDrop == true ? (i.p.RefundAmount != null && i.p.RefundAmount != 0) ? i.c.NetTotal - (i.p.RefundAmount ?? 0) : i.c.TotalPrice : i.p.Amount - (i.p.RefundAmount ?? 0),
                       DateEncoded = i.p.DateEncoded,
                       DeliveryOrderPaymentStatus = i.c.DeliveryOrderPaymentStatus
                   }).OrderByDescending(i => i.DateEncoded).ToList();

            //if (deliveryboyId != 0)
            //{
            //    if (StartDate != null && EndDate != null)
            //    {
            //        DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
            //        DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

            //        model.List = db.Payments.Join(db.Orders, p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
            //        .Where(i => i.p.DateEncoded >= startDatetFilter && i.p.DateEncoded <= endDateFilter && i.p.Status == 0
            //        && i.c.DeliveryBoyId == deliveryboyId && i.c.Status == 6 && i.p.PaymentMode == "Cash On Hand")
            //         .Select(i => new CartReportViewModel.CartReportList
            //         {
            //             Id = i.c.Id,
            //             OrderNumber = i.p.OrderNumber,
            //             DeliveryBoyPhoneNumber =i.c.DeliveryBoyPhoneNumber,
            //             DeliveryBoyId = i.c.DeliveryBoyId,
            //             DeliveryBoyName = i.c.DeliveryBoyName,
            //             Amount = i.p.Amount - (i.p.RefundAmount ?? 0),
            //             DateUpdated = i.c.DateUpdated,
            //             DeliveryOrderPaymentStatus = i.c.DeliveryOrderPaymentStatus
            //         }).OrderByDescending(i => i.DateUpdated).ToList();
            //        model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
            //        model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
            //    }
            //    else
            //    {
            //        model.List = db.Payments
            //            .Join(db.Orders, p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
            //          .Where(i => i.p.Status == 0 && i.c.DeliveryBoyId == deliveryboyId && i.c.Status == 6 && i.p.PaymentMode == "Cash On Hand")
            //         .Select(i => new CartReportViewModel.CartReportList
            //         {
            //             Id = i.c.Id,
            //             OrderNumber = i.p.OrderNumber,
            //             DeliveryBoyPhoneNumber = i.c.DeliveryBoyPhoneNumber,
            //             DeliveryBoyId = i.c.DeliveryBoyId,
            //             DeliveryBoyName = i.c.DeliveryBoyName,
            //             Amount = i.p.Amount - (i.p.RefundAmount ?? 0),
            //             DateUpdated = i.c.DateUpdated,
            //             DeliveryOrderPaymentStatus = i.c.DeliveryOrderPaymentStatus
            //         }).OrderByDescending(i => i.DateUpdated).ToList();
            //    }
            //}
            //else
            //{
            //    if (StartDate != null && EndDate != null)
            //    {
            //        DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
            //        DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

            //        model.List = db.Payments
            //            .Join(db.Orders, p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
            //        .Where(i => i.p.DateEncoded >= startDatetFilter && i.p.DateEncoded <= endDateFilter && i.p.Status == 0
            //        && i.c.Status == 6 && i.p.PaymentMode == "Cash On Hand")
            //         .Select(i => new CartReportViewModel.CartReportList
            //         {
            //             Id = i.c.Id,
            //             OrderNumber = i.p.OrderNumber,
            //             DeliveryBoyPhoneNumber = i.c.DeliveryBoyPhoneNumber,
            //             DeliveryBoyId = i.c.DeliveryBoyId,
            //             DeliveryBoyName = i.c.DeliveryBoyName,
            //             Amount = i.p.Amount - (i.p.RefundAmount ?? 0),
            //             DateUpdated = i.c.DateUpdated,
            //             DeliveryOrderPaymentStatus = i.c.DeliveryOrderPaymentStatus
            //         }).OrderByDescending(i => i.DateUpdated).ToList();
            //        model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
            //        model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
            //    }
            //    else
            //    {
            //        model.List = db.Payments.Join(db.Orders, p => p.OrderNumber, c => c.OrderNumber, (p, c) => new { p, c })
            //           .Where(i => i.p.Status == 0 && i.c.Status == 6 && i.p.PaymentMode == "Cash On Hand")
            //         .Select(i => new CartReportViewModel.CartReportList
            //         {
            //             Id = i.c.Id,
            //             OrderNumber = i.p.OrderNumber,
            //             DeliveryBoyPhoneNumber = i.c.DeliveryBoyPhoneNumber,
            //             DeliveryBoyId = i.c.DeliveryBoyId,
            //             DeliveryBoyName = i.c.DeliveryBoyName,
            //             Amount = i.p.Amount - (i.p.RefundAmount ?? 0),
            //             DateUpdated = i.c.DateUpdated,
            //             DeliveryOrderPaymentStatus = i.c.DeliveryOrderPaymentStatus
            //         }).OrderByDescending(i => i.DateUpdated).ToList();
            //    }
            //}
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCDPS080")]
        public ActionResult DeliveryBoyPaymentStatus(CartReportViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            // var model = new CartReportViewModel();

            model.DeliveryBoyPaymentStatusLists = db.Orders
                   .Where(i => i.Status == 6 && ((model.StartDate != null && model.EndDate != null) ? DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.StartDate) && DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.EndDate) : true) && (model.DeliveryBoyId != 0 ? i.DeliveryBoyId == model.DeliveryBoyId : true))
                   .Join(db.DeliveryBoys, o => o.DeliveryBoyId, d => d.Id, (o, d) => new { o, d })
                       .Select(i => new CartReportViewModel.DeliveryBoyPaymentStatusList
                       {
                           Id = i.o.Id,
                           DateEncoded = i.o.DateEncoded,
                           OrderNumber = i.o.OrderNumber,
                           DeliveryBoyId = i.o.DeliveryBoyId,
                           DeliveryBoyName = i.o.DeliveryBoyName,
                           ShopName = i.o.ShopName,
                           DeliveryCharge = i.d.WorkType == 1 ? (i.o.DeliveryCharge == 35 ? 20 + i.o.TipsAmount : 20 + (i.o.DeliveryCharge - 35) + i.o.TipsAmount) : i.o.DeliveryCharge + i.o.TipsAmount,
                           DeliveryBoyPaymentStatus = i.o.DeliveryBoyPaymentStatus,
                           Distance = i.o.Distance
                       }).OrderByDescending(i => i.DateEncoded).ToList();

            //if (DeliveryBoyId != 0)
            //{
            //    if (StartDate != null && EndDate != null)
            //    {
            //        DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
            //        DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

            //        model.DeliveryBoyPaymentStatusLists = db.Orders
            //        .Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.Status == 6 && i.DeliveryBoyId == DeliveryBoyId)
            //            .Select(i => new CartReportViewModel.DeliveryBoyPaymentStatusList
            //            {
            //                Id = i.Id,
            //                DateEncoded = i.DateEncoded,
            //                OrderNumber = i.OrderNumber,
            //                DeliveryBoyId = i.DeliveryBoyId,
            //                DeliveryBoyName = i.DeliveryBoyName,
            //                DeliveryBoyPhoneNumber = i.DeliveryBoyPhoneNumber,
            //                DeliveryCharge = i.DeliveryCharge,
            //                DeliveryBoyPaymentStatus = i.DeliveryBoyPaymentStatus,
            //                Distance = i.Distance
            //            }).OrderBy(i => i.DeliveryBoyName).ToList();
            //    }
            //    else
            //    {
            //        model.DeliveryBoyPaymentStatusLists = db.Orders
            //          .Where(i => i.Status == 6 && i.DeliveryBoyId == DeliveryBoyId)
            //              .Select(i => new CartReportViewModel.DeliveryBoyPaymentStatusList
            //              {
            //                  Id = i.Id,
            //                  DateEncoded = i.DateEncoded,
            //                  OrderNumber = i.OrderNumber,
            //                  DeliveryBoyId = i.DeliveryBoyId,
            //                  DeliveryBoyName = i.DeliveryBoyName,
            //                  DeliveryBoyPhoneNumber = i.DeliveryBoyPhoneNumber,
            //                  DeliveryCharge = i.DeliveryCharge,
            //                  DeliveryBoyPaymentStatus = i.DeliveryBoyPaymentStatus,
            //                  Distance = i.Distance
            //              }).OrderBy(i => i.DeliveryBoyName).ToList();
            //    }
            //}
            //else
            //{
            //    if (StartDate != null && EndDate != null)
            //    {
            //        DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
            //        DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

            //        model.DeliveryBoyPaymentStatusLists = db.Orders
            //        .Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.Status == 6)
            //            .Select(i => new CartReportViewModel.DeliveryBoyPaymentStatusList
            //            {
            //                Id = i.Id,
            //                DateEncoded = i.DateEncoded,
            //                OrderNumber = i.OrderNumber,
            //                DeliveryBoyId = i.DeliveryBoyId,
            //                DeliveryBoyName = i.DeliveryBoyName,
            //                DeliveryBoyPhoneNumber = i.DeliveryBoyPhoneNumber,
            //                DeliveryCharge = i.DeliveryCharge,
            //                DeliveryBoyPaymentStatus = i.DeliveryBoyPaymentStatus,
            //                Distance = i.Distance
            //            }).OrderBy(i => i.DeliveryBoyName).ToList();
            //    }
            //    else
            //    {
            //        model.DeliveryBoyPaymentStatusLists = db.Orders
            //          .Where(i => i.Status == 6)
            //              .Select(i => new CartReportViewModel.DeliveryBoyPaymentStatusList
            //              {
            //                  Id = i.Id,
            //                  DateEncoded = i.DateEncoded,
            //                  OrderNumber = i.OrderNumber,
            //                  DeliveryBoyId = i.DeliveryBoyId,
            //                  DeliveryBoyName = i.DeliveryBoyName,
            //                  DeliveryBoyPhoneNumber = i.DeliveryBoyPhoneNumber,
            //                  DeliveryCharge = i.DeliveryCharge,
            //                  DeliveryBoyPaymentStatus = i.DeliveryBoyPaymentStatus,
            //                  Distance = i.Distance
            //              }).OrderBy(i => i.DeliveryBoyName).ToList();
            //    }
            //}
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCA081")]
        public JsonResult Accept(int OrderNumber)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (OrderNumber != 0)
            {
                var order = db.Orders.FirstOrDefault(i => i.OrderNumber == OrderNumber);
                order.Status = 3;
                order.UpdatedBy = user.Name;
                order.DateUpdated = DateTime.Now;
                order.ShopAcceptedTime = DateTime.Now;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //Customer
                if (order.CustomerId != 0)
                {
                    var fcmToken = (from c in db.Customers
                                    where c.Id == order.CustomerId
                                    select c.FcmTocken ?? "").FirstOrDefault().ToString();
                    Helpers.PushNotification.SendbydeviceId($"Your order has been accepted by shop({order.ShopName}).", "ShopNowChat", "a.mp3", fcmToken.ToString());
                }
                //AddPaymentData
                //if (order.PaymentModeType == 1)
                //{
                //    var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == order.OrderNumber);
                //    if (payment != null)
                //        AddPaymentData(payment.ReferenceCode, order.OrderNumber);
                //}

                return Json(new { message = "Order Confirmed!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "Failed to Confirm the Order!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [AccessPolicy(PageCode = "SNCCC082")]
        public JsonResult Cancel(int OrderNumber, int customerId, int? status, string remark)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (OrderNumber != 0 && status != 0)
            {
                var customer = db.Customers.FirstOrDefault(i => i.Id == customerId);
                var order = db.Orders.FirstOrDefault(i => i.OrderNumber == OrderNumber);
                order.Status = status ?? 0;
                order.CancelledRemark = remark;
                order.UpdatedBy = user.Name;
                order.DateUpdated = DateTime.Now;
                order.ShopAcceptedTime = DateTime.Now;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //Refund
                var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == order.OrderNumber);
                if (status == 7)
                {
                    payment.RefundRemark = "Your order has been cancelled by shop.";
                    payment.RefundAmount = payment.Amount;
                }
                if (status == 10)
                {
                    payment.RefundRemark = "Customer Not Pickedup the Order.";
                    payment.RefundAmount = 0;
                }
                payment.UpdatedBy = user.Name;
                payment.DateUpdated = DateTime.Now;
                db.Entry(payment).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var orderItemList = db.OrderItems.Where(i => i.OrderId == order.Id);
                if (orderItemList != null)
                {
                    foreach (var item in orderItemList)
                    {
                        //Product Stock Update
                        var product = db.Products.FirstOrDefault(i => i.Id == item.ProductId && i.ProductTypeId == 3);
                        if (product != null)
                        {
                            product.HoldOnStok -= Convert.ToInt32(item.Quantity);
                            product.Qty += Convert.ToInt32(item.Quantity);
                            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }

                //Add Wallet Amount to customer
                if (order.WalletAmount != 0 && customer != null)
                {
                    customer.WalletAmount += order.WalletAmount;
                    db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

                if (order.CustomerId != 0)
                {
                    var fcmToken = (from c in db.Customers
                                    where c.Id == order.CustomerId
                                    select c.FcmTocken ?? "").FirstOrDefault().ToString();
                    //order cancel
                    Helpers.PushNotification.SendbydeviceId($"Shop({order.ShopName}) has rejected your order. Kindly contact shop for details or try another order.", "ShopNowChat", "a.mp3", fcmToken.ToString());

                    //Refund notification
                    if (payment.PaymentMode == "Online Payment" && status == 7)
                        Helpers.PushNotification.SendbydeviceId($"Your refund of amount {payment.Amount} for order no {payment.OrderNumber} is for {payment.RefundRemark} initiated and you will get credited with in 7 working days.", "ShopNowChat", "a.mp3", fcmToken.ToString());

                }
                //AddPaymentData
                //if (order.PaymentModeType == 1)
                //{
                //    if (payment != null)
                //        AddPaymentData(payment.ReferenceCode, order.OrderNumber);
                //}
                return Json(new { message = "Order Cancelled!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "Failed to Cancel the Order!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [AccessPolicy(PageCode = "SNCCUAD083")]
        public ActionResult UnAssignDeliveryBoy(int OrderNumber)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var order = db.Orders.FirstOrDefault(i => i.OrderNumber == OrderNumber);
            var deliveryboy = db.DeliveryBoys.FirstOrDefault(i => i.Id == order.DeliveryBoyId);
            //deliveryboy.isAssign = 0;
            //deliveryboy.OnWork = 0;
            deliveryboy.DateUpdated = DateTime.Now;
            db.Entry(deliveryboy).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            order.DeliveryBoyOnWork = 0;
            order.DeliveryBoyId = 0;
            order.DeliveryBoyName = string.Empty;
            order.DeliveryBoyPhoneNumber = string.Empty;
            order.Status = 3;
            order.DateUpdated = DateTime.Now;
            order.UpdatedBy = user.Name;
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("DeliveryAgentAssigned");
        }

        [AccessPolicy(PageCode = "SNCCDA084")]
        public ActionResult DeliveryBoyAccept(int OrderNumber, long id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var order = db.Orders.FirstOrDefault(i => i.Id == id);
            if (order != null)
            {
                var delivaryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == order.DeliveryBoyId && i.Status == 0);
                delivaryBoy.OnWork = 1;
                delivaryBoy.UpdatedBy = user.Name;
                delivaryBoy.DateUpdated = DateTime.Now;
                db.Entry(delivaryBoy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                order.DeliveryBoyOnWork = 1;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Edit", "Cart", new { OrderNumber = OrderNumber, id = AdminHelpers.ECodeLong(id) });
        }

        [AccessPolicy(PageCode = "SNCCDP085")]
        public ActionResult DeliveryBoyPickup(int OrderNumber, int id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var order = db.Orders.FirstOrDefault(i => i.Id == id);
            order.Status = 5;
            order.UpdatedBy = user.Name;
            order.OrderPickupTime = DateTime.Now;
            order.DateUpdated = DateTime.Now;
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            string notificationMessage = $"Your order on shop({order.ShopName}) is on the way.";
            var orderItemList = db.OrderItems.Where(i => i.OrderId == order.Id).ToList();
            foreach (var item in orderItemList)
            {
                //Product Stock Update
                var product = db.Products.FirstOrDefault(i => i.Id == item.ProductId && i.ProductTypeId == 3);
                if (product != null)
                {
                    product.HoldOnStok -= Convert.ToInt32(item.Quantity);
                    db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
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
                notificationMessage = $"Your order on shop({order.ShopName}) is on the way. Please share the delivery code { otpVerification.Otp} with the delivery partner {order.DeliveryBoyName} for verification.";
            }
            if (order.CustomerId != 0)
            {
                var fcmToken = (from c in db.Customers
                                where c.Id == order.CustomerId
                                select c.FcmTocken ?? "").FirstOrDefault().ToString();
                Helpers.PushNotification.SendbydeviceId(notificationMessage, "ShopNowChat", "a.mp3", fcmToken.ToString());
            }
            return RedirectToAction("Edit", "Cart", new { OrderNumber = OrderNumber, id = AdminHelpers.ECodeLong(id) });
        }

        [AccessPolicy(PageCode = "SNCCMD086")]
        public ActionResult MarkAsDelivered(int OrderNumber, int id, string address = "")
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var order = db.Orders.FirstOrDefault(i => i.Id == id);

            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == order.DeliveryBoyId && i.Status == 0);
            if (deliveryBoy != null)
            {
                deliveryBoy.OnWork = 0;
                deliveryBoy.isAssign = 0;
                deliveryBoy.UpdatedBy = user.Name;
                deliveryBoy.DateUpdated = DateTime.Now;
                db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            var otpVerify = db.OtpVerifications.FirstOrDefault(i => i.OrderNo == OrderNumber);
            if (otpVerify != null)
            {
                otpVerify.Verify = true;
                otpVerify.UpdatedBy = user.Name;
                otpVerify.DateUpdated = DateTime.Now;
                db.Entry(otpVerify).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            if (!string.IsNullOrEmpty(address))
            {
                order.DeliveryAddress = address;
            }
            order.DeliveryBoyOnWork = 0;
            order.Status = 6;
            order.UpdatedBy = user.Name;
            order.DateUpdated = DateTime.Now;
            order.DeliveredTime = DateTime.Now;
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            //var fcmToken = (from c in db.Customers
            //                where c.Id == order.CustomerId
            //                select c.FcmTocken ?? "").FirstOrDefault().ToString();

            //Reducing Platformcredits
            var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == OrderNumber);
            var shop = db.Shops.FirstOrDefault(i => i.Id == order.ShopId);
            if (shop.IsTrail == false || order.IsPickupDrop == true) //Only Reduce DeliveryCredits When Shop is not trail and for all Pickupdrop order
            {
                var shopCredits = db.ShopCredits.FirstOrDefault(i => i.CustomerId == shop.CustomerId);
                shopCredits.DeliveryCredit -= order.DeliveryCharge;
                db.Entry(shopCredits).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            var customerDetails = (from c in db.Customers
                                   where c.Id == order.CustomerId
                                   select c).FirstOrDefault();
            if (customerDetails != null)
            {
                if (customerDetails.IsReferred == false && !string.IsNullOrEmpty(customerDetails.ReferralNumber))
                {
                    //customerDetails.Id = customerDetails.Id;
                    customerDetails.IsReferred = true;
                    db.Entry(customerDetails).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    var referralCustomer = db.Customers.FirstOrDefault(c => c.PhoneNumber == customerDetails.ReferralNumber);
                    if (referralCustomer != null)
                    {
                        var referalAmount = db.ReferralSettings.Where(r => r.Status == 0 && r.ShopDistrict == shop.DistrictName).Select(r => r.Amount).FirstOrDefault();
                        referralCustomer.WalletAmount = referralCustomer.WalletAmount + referalAmount;
                        db.Entry(referralCustomer).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        //Wallet History for Referral
                        var walletHistory = new CustomerWalletHistory
                        {
                            Amount = referalAmount,
                            CustomerId = referralCustomer.Id,
                            DateEncoded = DateTime.Now,
                            Description = "Received from referral",
                            Type = 1
                        };
                        db.CustomerWalletHistories.Add(walletHistory);
                        db.SaveChanges();
                    }
                }
            }
            //Update Wallet Amount with offers
            var offer = db.Offers.FirstOrDefault(i => i.Id == order.OfferId);
            if (offer != null)
            {
                if (offer.DiscountType == 2)
                {
                    customerDetails.WalletAmount += order.OfferAmount;
                    db.Entry(customerDetails).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    //Wallet History for Wallet Offer
                    var walletHistory = new CustomerWalletHistory
                    {
                        Amount = order.OfferAmount,
                        CustomerId = customerDetails.Id,
                        DateEncoded = DateTime.Now,
                        Description = $"Received from offer({offer.Name})",
                        Type = 1
                    };
                    db.CustomerWalletHistories.Add(walletHistory);
                    db.SaveChanges();
                }
            }

            if (order.WalletAmount > 0)
            {
                //Wallet History for Wallet Offer
                var walletHistory = new CustomerWalletHistory
                {
                    Amount = order.WalletAmount,
                    CustomerId = customerDetails.Id,
                    DateEncoded = DateTime.Now,
                    Description = $"Payment to Order(#{order.OrderNumber})",
                    Type = 2
                };
                db.CustomerWalletHistories.Add(walletHistory);
                db.SaveChanges();
            }

            //Update Achievement Wallet
            if (order.CustomerId != 0)
            {
                Helpers.AchievementHelpers.UpdateAchievements(order.CustomerId);
            }
            if (customerDetails != null)
            {
                string fcmtocken = customerDetails.FcmTocken ?? "";

                Helpers.PushNotification.SendbydeviceId($"Your order on shop({ order.ShopName}) has been delivered by delivery partner { order.DeliveryBoyName}.", "ShopNowChat", "a.mp3", fcmtocken);
            }
            return RedirectToAction("Edit", "Cart", new { OrderNumber = OrderNumber, id = AdminHelpers.ECodeLong(id) });
        }

        [AccessPolicy(PageCode = "SNCCCNP087")]
        public ActionResult CustomerNotPickUp(int OrderNumber, int id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var order = db.Orders.FirstOrDefault(i => i.Id == id);

            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == order.DeliveryBoyId && i.Status == 0);
            if (deliveryBoy != null)
            {
                deliveryBoy.OnWork = 0;
                deliveryBoy.isAssign = 0;
                deliveryBoy.UpdatedBy = user.Name;
                deliveryBoy.DateUpdated = DateTime.Now;
                db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            order.DeliveryBoyOnWork = 0;
            order.Status = 10;
            order.UpdatedBy = user.Name;
            order.DateUpdated = DateTime.Now;
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Edit", "Cart", new { OrderNumber = OrderNumber, id = AdminHelpers.ECodeLong(id) });
        }

        [AccessPolicy(PageCode = "SNCCAW088")]
        public ActionResult AddWaitingCharge(int orderId, string remark, double amount)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var order = db.Orders.FirstOrDefault(i => i.Id == orderId);
            if (order != null)
            {
                order.WaitingCharge = amount;
                order.WaitingRemark = remark;
                if (order.DeliveryLocationReachTime != null && order.DeliveredTime != null)
                    order.WaitingTime = (order.DeliveryLocationReachTime.Value - order.DeliveredTime.Value).Minutes;
                order.DateUpdated = DateTime.Now;
                order.UpdatedBy = user.Name;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var customer = db.Customers.FirstOrDefault(i => i.Id == order.CustomerId);
                if (customer != null)
                {
                    customer.DeliveryWaitingCharge += amount;
                    customer.DateUpdated = DateTime.Now;
                    customer.UpdatedBy = user.Name;
                    db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Edit", "Cart", new { OrderNumber = order.OrderNumber, id = AdminHelpers.ECodeLong(orderId) });
        }

        [AccessPolicy(PageCode = "SNCCAP089")]
        public ActionResult AddPenaltyCharge(int orderId, string remark, double amount)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var order = db.Orders.FirstOrDefault(i => i.Id == orderId);
            if (order != null)
            {
                order.PenaltyAmount = amount;
                order.PenaltyRemark = remark;
                order.UpdatedBy = user.Name;
                order.DateUpdated = DateTime.Now;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var customer = db.Customers.FirstOrDefault(i => i.Id == order.CustomerId);
                if (customer != null)
                {
                    customer.PenaltyAmount += amount;
                    customer.DateUpdated = DateTime.Now;
                    customer.UpdatedBy = user.Name;
                    db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Edit", "Cart", new { OrderNumber = order.OrderNumber, id = AdminHelpers.ECodeLong(orderId) });
        }

        [AccessPolicy(PageCode = "SNCCOR090")]
        public ActionResult OrderRatios(OrderRatioViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.MonthFilter = model.MonthFilter != 0 ? model.MonthFilter : DateTime.Now.Month;
            model.YearFilter = model.YearFilter != 0 ? model.YearFilter : DateTime.Now.Year;
            //model.ListItems = db.Orders.Where(a => a.DateEncoded.Month == model.MonthFilter && a.DateEncoded.Year == model.YearFilter && (a.Status == 6 || a.Status == 7 || a.Status == 9 || a.Status == 10)).GroupBy(i => DbFunctions.TruncateTime(i.DateEncoded))
            //    .AsEnumerable()
            //    .Select(i => new OrderRatioViewModel.ListItem
            //    {
            //        Date = i.Key.Value.ToString("dd-MMM-yyyy"),
            //        TotalOrder = i.Count(), //status 6,7
            //        CancelOrder = i.Where(a => a.Status == 7).Count(),
            //        NewOrder = GetNewOrderCount(i.Key.Value, 0), //status 6
            //        ResTotal = i.Join(db.Shops.Where(a => a.ShopCategoryId == 1), o => o.ShopId, s => s.Id, (o, s) => new { o, s }).Count(),
            //        VegTotal = i.Join(db.Shops.Where(a => a.ShopCategoryId == 2), o => o.ShopId, s => s.Id, (o, s) => new { o, s }).Count(),
            //        MedicalTotal = i.Join(db.Shops.Where(a => a.ShopCategoryId == 4), o => o.ShopId, s => s.Id, (o, s) => new { o, s }).Count(),
            //        ResNewOrder = GetNewOrderCount(i.Key.Value, 1),
            //        VegNewOrder = GetNewOrderCount(i.Key.Value, 2),
            //        MedicalNewOrder = GetNewOrderCount(i.Key.Value, 4),
            //        ResCancelOrder = i.Where(a => a.Status == 7).Join(db.Shops.Where(a => a.ShopCategoryId == 1), o => o.ShopId, s => s.Id, (o, s) => new { o, s }).Count(),
            //        VegCancelOrder = i.Where(a => a.Status == 7).Join(db.Shops.Where(a => a.ShopCategoryId == 2), o => o.ShopId, s => s.Id, (o, s) => new { o, s }).Count(),
            //        MedicalCancelOrder = i.Where(a => a.Status == 7).Join(db.Shops.Where(a => a.ShopCategoryId == 4), o => o.ShopId, s => s.Id, (o, s) => new { o, s }).Count()
            //    }).ToList();


            model.ListItems = db.Orders.Where(a => a.DateEncoded.Month == model.MonthFilter && a.DateEncoded.Year == model.YearFilter &&
            (a.Status == 6 || a.Status == 7 || a.Status == 9 || a.Status == 10)).Select(i=>new { DateEncoded = i.DateEncoded, ShopId = i.ShopId, Status = i.Status })
                .Join(db.Shops.Select(i=>new { Id = i.Id, ShopCategoryId = i.ShopCategoryId}), o => o.ShopId, s => s.Id, (o, s) => new { o, s })
                .GroupBy(i => DbFunctions.TruncateTime(i.o.DateEncoded))
               .AsEnumerable()
               .Select(i => new OrderRatioViewModel.ListItem
               {
                   Date = i.Key.Value.ToString("dd-MMM-yyyy"),
                   TotalOrder = i.Count(), //status 6,7,9,10
                   DeliveredOrder = i.Where(a => a.o.Status == 6).Count(),
                   CancelOrder = i.Where(a => a.o.Status != 6).Count(),
                   NewOrder = GetNewOrderCount(i.Key.Value, 0), //status 6
                   ResTotal = i.Where(a => a.s.ShopCategoryId == 1).Count(),
                   VegTotal = i.Where(a => a.s.ShopCategoryId == 2).Count(),
                   MedicalTotal = i.Where(a => a.s.ShopCategoryId == 4).Count(),
                   ResNewOrder = GetNewOrderCount(i.Key.Value, 1),
                   VegNewOrder = GetNewOrderCount(i.Key.Value, 2),
                   MedicalNewOrder = GetNewOrderCount(i.Key.Value, 4),
                   ResCancelOrder = i.Where(a => a.o.Status == 7 && a.s.ShopCategoryId == 1).Count(),
                   VegCancelOrder = i.Where(a => a.o.Status == 7 && a.s.ShopCategoryId == 2).Count(),
                   MedicalCancelOrder = i.Where(a => a.o.Status == 7 && a.s.ShopCategoryId == 4).Count(),
                   CustomerCount = GetNewCustomerCount(i.Key.Value)
               }).ToList();
            return View(model);
        }

        public ActionResult PickUpDropReport(CartReportViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.PickUpDropReportLists = db.Orders.Where(i => (i.IsPickupDrop == true) && (model.ShopId != 0 ? i.ShopId == model.ShopId : true) &&
                ((DbFunctions.TruncateTime(i.DateEncoded) >= DbFunctions.TruncateTime(model.StartDate)) &&
                 (DbFunctions.TruncateTime(i.DateEncoded) <= DbFunctions.TruncateTime(model.EndDate))) && i.Status == 6)
                .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                .AsEnumerable()
            .Select(i => new CartReportViewModel.PickUpDropReportList
            {
                Id = i.c.Id,
                ShopName = i.c.ShopName,
                OrderNumber = i.c.OrderNumber.ToString(),
                DeliveryAddress = i.c.DeliveryAddress,
                PhoneNumber = i.c.CustomerPhoneNumber,
                DateEncoded = i.c.DateEncoded,
                PaymentMode = i.c.PaymentMode,
                Amount = (i.p.RefundAmount != null && i.p.RefundAmount != 0) ? i.c.NetTotal - (i.p.RefundAmount ?? 0) : i.c.TotalPrice,
            }).OrderByDescending(i => i.DateEncoded).ToList();
            int counter = 1;
            model.PickUpDropReportLists.ForEach(x => x.No = counter++);
            return View(model);
        }

        public ActionResult AddRefundFromShopOrderProcessing(long id, double amount, string remark, int redirection = 0)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var order = db.Orders.FirstOrDefault(i => i.Id == id);
            //Refund
            var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == order.OrderNumber);
            payment.RefundAmount = amount;
            payment.RefundRemark = remark;
            payment.UpdatedBy = user.Name;
            payment.DateUpdated = DateTime.Now;
            db.Entry(payment).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            if (order.CustomerId != 0)
            {
                var fcmToken = (from c in db.Customers
                                where c.Id == order.CustomerId
                                select c.FcmTocken ?? "").FirstOrDefault().ToString();
                if (payment.PaymentMode == "Online Payment")
                    Helpers.PushNotification.SendbydeviceId($"Your refund of amount {payment.RefundAmount} for order no {payment.OrderNumber} is for {payment.RefundRemark} initiated and you will get credited with in 7 working days.", "ShopNowChat", "a.mp3", fcmToken.ToString());
                else
                    Helpers.PushNotification.SendbydeviceId($"Your order is reduced with {payment.RefundAmount} amount for {payment.RefundRemark}", "ShopNowChat", "a.mp3", fcmToken.ToString());
            }
            //AddPaymentData
            //if (order.PaymentModeType == 1)
            //{
            //    if (payment != null)
            //        AddPaymentData(payment.ReferenceCode, order.OrderNumber);
            //}

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
            else if (redirection == 5)
                return RedirectToAction("Details", "Cart", new { id = AdminHelpers.ECodeLong(id) });
            else
                return RedirectToAction("Delivered");
        }

        public ActionResult UpdateRefundAmount(long id, double amount = 0, string remark = "")
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var order = db.Orders.FirstOrDefault(i => i.Id == id);

            var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == order.OrderNumber);
            if (payment != null)
                payment.RefundAmount = amount;
            payment.RefundRemark = remark;
            payment.UpdatedBy = user.Name;
            payment.DateUpdated = DateTime.Now;
            db.Entry(payment).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            //AddPaymentData
            //if (order.PaymentModeType == 1)
            //{
            //    if (payment != null)
            //        AddPaymentData(payment.ReferenceCode, order.OrderNumber);
            //}


            return RedirectToAction("Details", "Cart", new { id = AdminHelpers.ECodeLong(id) });
        }

        public ActionResult UpdatePaymentMode(int OrderNo, int PaymentType)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var cart = db.Orders.FirstOrDefault(i => i.OrderNumber == OrderNo);
            var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == OrderNo);
            if (PaymentType == 1)
            {
                if (cart != null)
                {
                    cart.PaymentMode = "Online Payment";
                    cart.PaymentModeType = 1;
                }
                if (payment != null)
                {
                    payment.PaymentMode = "Online Payment";
                    payment.PaymentModeType = 1;
                }
            }
            else if (PaymentType == 2)
            {
                if (cart != null)
                {
                    cart.PaymentMode = "Cash On Hand";
                    cart.PaymentModeType = 2;
                }
                if (payment != null)
                {
                    payment.PaymentMode = "Cash On Hand";
                    payment.PaymentModeType = 2;
                }
            }
            cart.UpdatedBy = user.Name;
            cart.DateUpdated = DateTime.Now;
            payment.UpdatedBy = user.Name;
            payment.DateUpdated = DateTime.Now;
            db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            db.Entry(payment).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("List");
        }

        public ActionResult UpdateShopPayment(long id, double amount = 0)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var order = db.Orders.FirstOrDefault(i => i.Id == id);
            if (order != null)
            {
                order.TotalShopPrice = amount;
                order.UpdatedBy = user.Name;
                order.DateUpdated = DateTime.Now;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Details", "Cart", new { id = AdminHelpers.ECodeLong(id) });
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

        public JsonResult ShopPay(int OrderNumber)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var order = db.Orders.FirstOrDefault(i => i.OrderNumber == OrderNumber && i.ShopPaymentStatus == 0);
            if (order != null)
            {
                order.ShopPaymentStatus = 1;
                order.UpdatedBy = user.Name;
                order.DateUpdated = DateTime.Now;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ShopNowChatPay(int OrderNumber)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var order = db.Orders.FirstOrDefault(i => i.OrderNumber == OrderNumber && i.DeliveryOrderPaymentStatus == 0);
            if (order != null)
            {
                order.DeliveryOrderPaymentStatus = 1;
                order.UpdatedBy = user.Name;
                order.DateUpdated = DateTime.Now;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeliveryBoyPay(int OrderNumber)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var order = db.Orders.FirstOrDefault(i => i.OrderNumber == OrderNumber && i.DeliveryBoyPaymentStatus == 0);
            if (order != null)
            {
                order.DeliveryBoyPaymentStatus = 1;
                order.UpdatedBy = user.Name;
                order.DateUpdated = DateTime.Now;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeliveryBoyReject(int OrderNumber)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (OrderNumber != 0)
            {
                var order = db.Orders.FirstOrDefault(i => i.OrderNumber == OrderNumber);
                order.DeliveryBoyId = 0;
                order.DeliveryBoyName = null;
                order.DeliveryBoyPhoneNumber = null;
                order.Status = 3;
                order.DeliveryBoyOnWork = 0;
                order.UpdatedBy = user.Name;
                order.DateUpdated = DateTime.Now;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var dboy = db.DeliveryBoys.FirstOrDefault(i => i.Id == order.DeliveryBoyId);
                dboy.isAssign = 0;
                dboy.OnWork = 0;
                dboy.UpdatedBy = user.Name;
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

        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && (a.Status == 0 || a.Status==1)).Select(i => new
            {
                id = i.Id,
                text = i.Name,
                shopCategoryId = i.ShopCategoryId
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDeliveryBoySelect2(string q = "")
        {
            var model = await db.DeliveryBoys.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public int GetNewOrderCount(DateTime date, int categoryType)
        {
            var orders = db.Orders.Where(i => DbFunctions.TruncateTime(i.DateEncoded) < DbFunctions.TruncateTime(date) && i.Status == 6).Select(i=>new { ShopId=i.ShopId, CustomerId =i.CustomerId})
                .Join(db.Shops.Where(i => categoryType != 0 ? i.ShopCategoryId == categoryType : true), o => o.ShopId, s => s.Id, (o, s) => new { o, s })
                .Select(i => i.o.CustomerId);
            var count = db.Orders.Where(a => !orders.Contains(a.CustomerId) && DbFunctions.TruncateTime(a.DateEncoded) == DbFunctions.TruncateTime(date) && a.Status == 6).Select(i => new { ShopId = i.ShopId})
                .Join(db.Shops.Where(i => categoryType != 0 ? i.ShopCategoryId == categoryType : true), o => o.ShopId, s => s.Id, (o, s) => new { o, s })
                .Count();
            return count;
        }

        public int GetNewCustomerCount(DateTime date)
        {
            var count = db.Customers.Where(i => DbFunctions.TruncateTime(i.DateEncoded) == DbFunctions.TruncateTime(date)).Count();
            return count;
        }

        public ActionResult UpdateItem(long orderid, long id, int quantity, double unitprice, string remarks)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var order = db.Orders.FirstOrDefault(i => i.Id == orderid);
            var orderItem = db.OrderItems.FirstOrDefault(i => i.Id == id);
            orderItem.Quantity = quantity;
            orderItem.UnitPrice = unitprice;
            orderItem.Price = quantity * unitprice;
            orderItem.UpdatedBy = user.Name;
            orderItem.UpdatedTime = DateTime.Now;
            orderItem.UpdateRemarks = remarks;
            db.Entry(orderItem).State = EntityState.Modified;
            db.SaveChanges();

            var orderItemList = db.OrderItems.Where(i => i.OrderId == orderid).ToList();
            order.TotalPrice = orderItemList.Sum(i => i.Price);
            order.TotalQuantity = orderItemList.Sum(i => i.Quantity);
            order.NetTotal = Math.Round(order.TotalPrice + order.TipsAmount + order.Packingcharge + order.Convinenientcharge + order.NetDeliveryCharge - (order.WalletAmount - order.OfferAmount), MidpointRounding.AwayFromZero);
            order.UpdatedBy = user.Name;
            order.DateUpdated = DateTime.Now;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            var payments = db.Payments.FirstOrDefault(i => i.OrderNumber == order.OrderNumber);
            payments.OriginalAmount = order.TotalPrice;
            payments.GSTAmount = order.NetTotal;
            payments.Amount = order.NetTotal;
            payments.UpdatedBy = user.Name;
            payments.DateUpdated = DateTime.Now;
            db.Entry(payments).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", "Cart", new { id = AdminHelpers.ECodeLong(orderid) });
        }

        [AccessPolicy(PageCode = "SNCCBL305")]
        public ActionResult BatchList(BatchOrderListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Orders.Where(i => i.Status == 2 && (model.ShopId != 0 ? i.ShopId == model.ShopId : true) /*&& SqlFunctions.DateDiff("minute", i.DateEncoded, DateTime.Now) <= 10*/)
                 .AsEnumerable()
                            .Join(db.Shops, c => c.ShopId, s => s.Id, (c, s) => new { c, s })
                            .Select(i => new BatchOrderListViewModel.ListItem
                            {
                                Id = i.c.Id,
                                ShopName = i.c.ShopName,
                                OrderNumber = i.c.OrderNumber,
                                DeliveryAddress = i.c.DeliveryAddress,
                                ShopOwnerPhoneNumber = i.c.ShopOwnerPhoneNumber,
                                Status = i.c.Status,
                                DeliveryBoyName = i.c.DeliveryBoyName,
                                DateEncoded = i.c.DateEncoded,
                                Price = i.c.NetTotal,
                                PaymentMode = i.c.PaymentMode,
                                CustomerLatitude = i.c.Latitude,
                                CustomerLongitude = i.c.Longitude,
                                ShopLatitude = i.s.Latitude,
                                ShopLongitude = i.s.Longitude
                            })
                            .Where(i => (double)(GetMeters(i.CustomerLatitude, i.CustomerLongitude, i.ShopLatitude, i.ShopLongitude) / 1000) <= 4)
                            .ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult DeliveryLocationMap()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        public JsonResult GetDeliveryLocations()
        {
            var ordersList = db.Orders.Where(i => i.Status == 2 || i.Status == 3 || i.Status == 4 || i.Status == 5)
                .Join(db.Shops, o => o.ShopId, s => s.Id, (o, s) => new { o, s })
                .AsEnumerable()
                .Select(i => new
                {
                    ShopLatitude = i.s.Latitude,
                    ShopLongitude = i.s.Longitude,
                    CustomerLatitude = i.o.Latitude,
                    CustomerLongitude = i.o.Longitude,
                    CustomerAddress = i.o.DeliveryAddress,
                    ShopAddress = i.s.Address,
                    ShopName = i.s.Name,
                    OrderNumber = i.o.OrderNumber,
                    OrderTime = i.o.DateEncoded.ToString("hh:mm tt"),
                    Distance = Math.Round((double)(GetMeters(i.s.Latitude, i.s.Longitude, i.o.Latitude, i.o.Longitude) / 1000), 2)
                }).ToList();

            return Json(ordersList, JsonRequestBehavior.AllowGet);
        }

        //public void AddPaymentData(string code, int ordernumber)
        //{
        //    if (!string.IsNullOrEmpty(code) && ordernumber != 0)
        //    {
        //        if (!db.PaymentsDatas.Any(i => i.PaymentId == code))
        //        {
        //            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
        //                                                     SecurityProtocolType.Tls11 |
        //                                                     SecurityProtocolType.Tls12;
        //            string key = BaseClass.razorpaykey;// "rzp_live_PNoamKp52vzWvR";
        //            string secret = BaseClass.razorpaySecretkey;//"yychwOUOsYLsSn3XoNYvD1HY";

        //            RazorpayClient client = new RazorpayClient(key, secret);
        //            Razorpay.Api.Payment varpayment = new Razorpay.Api.Payment();
        //            var s = varpayment.Fetch(code);
        //            PaymentsData pay = new PaymentsData();
        //            pay.OrderNumber = ordernumber;
        //            pay.PaymentId = code;

        //            pay.Invoice_Id = s["invoice_id"];
        //            if (s["status"] == "created")
        //                pay.Status = 0;
        //            else if (s["status"] == "authorized")
        //                pay.Status = 1;
        //            else if (s["status"] == "captured")
        //                pay.Status = 2;
        //            else if (s["status"] == "refunded")
        //                pay.Status = 3;
        //            else if (s["status"] == "failed")
        //                pay.Status = 4;
        //            pay.Order_Id = s["order_id"];
        //            if (s["fee"] != null && s["fee"] > 0)
        //                pay.Fee = (decimal)s["fee"] / 100;
        //            else
        //                pay.Fee = s["fee"];
        //            pay.Entity = s["entity"];
        //            pay.Currency = s["currency"];
        //            pay.Method = s["method"];
        //            if (s["tax"] != null && s["tax"] > 0)
        //                pay.Tax = (decimal)s["tax"] / 100;
        //            else
        //                pay.Tax = s["tax"];
        //            if (s["amount"] != null && s["amount"] > 0)
        //                pay.Amount = s["amount"] / 100;
        //            else
        //                pay.Amount = s["amount"];
        //            pay.DateEncoded = DateTime.Now;
        //            db.PaymentsDatas.Add(pay);
        //            db.SaveChanges();
        //        }
        //    }
        //}

        //public JsonResult GetLiveOrderCount()
        //{
        //    try
        //    {
        //        var user = ((Helpers.Sessions.User)Session["USER"]);
        //        ViewBag.Name = user.Name;

        //        string[] shop = db.Shops.Where(i => i.CustomerCode == user.Code && i.Status == 0).Select(S => S.Code).ToArray();
        //        int count = db.Carts.Where(i => shop.Contains(i.ShopCode) && i.CartStatus == 2 && i.Status == 0).GroupBy(i => i.OrderNo).Count();
        //        return Json(new { OrderCount = count }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.Write(ex.Message);
        //        return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult ShopBillUpdate(string BillNo, double BillAmount, int OrderNumber, string OrderId)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var shopBill = db.ShopBillDetails.FirstOrDefault(i => i.OrderNumber == OrderNumber);
            if (shopBill != null)
            {
                shopBill.BillNo = BillNo;
                shopBill.BillAmount = BillAmount;
                shopBill.OrderNumber = OrderNumber;
                shopBill.DateEncoded = DateTime.Now;
                shopBill.UpdatedBy = user.Name;
                db.Entry(shopBill).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else if (!string.IsNullOrEmpty(BillNo))
            {
                ShopBillDetail shopBillDetail = new ShopBillDetail();
                shopBillDetail.BillNo = BillNo;
                shopBillDetail.BillAmount = BillAmount;
                shopBillDetail.OrderNumber = OrderNumber;
                shopBillDetail.DateEncoded = DateTime.Now;
                shopBillDetail.UpdatedBy = user.Name;
                if (!db.ShopBillDetails.Any(i => i.OrderNumber == OrderNumber))
                {
                    db.ShopBillDetails.Add(shopBillDetail);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("PickupSlip", new { OrderNumber = OrderNumber, id = OrderId });
        }

        public ActionResult UpdateDeliveryAddress(long orderId, string address, double distance = 0, double latitude = 0, double longitude = 0, double deliverycharge = 0)
        {
            var order = db.Orders.FirstOrDefault(i => i.Id == orderId);
            order.DeliveryAddress = address;
            order.Latitude = latitude;
            order.Longitude = longitude;
            order.Distance = distance;
            order.DeliveryCharge = deliverycharge;
            order.NetDeliveryCharge = deliverycharge;
            order.NetTotal = order.TotalPrice + deliverycharge;
            order.DateUpdated = DateTime.Now;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == order.OrderNumber);
            payment.GSTAmount = order.NetTotal;
            payment.Amount = order.NetTotal;
            db.Entry(payment).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", "Cart", new { id = AdminHelpers.ECodeLong(orderId) });
        }

        public JsonResult GetDeliveryCharge(double PickupLatitude, double PickupLongitude, double DeliveryLatitude, double DeliveryLongitude)
        {
            double DeliveryCharge = 0;
            var Distance = (((Math.Acos(Math.Sin((PickupLatitude * Math.PI / 180)) * Math.Sin(((DeliveryLatitude) * Math.PI / 180)) + Math.Cos((PickupLatitude * Math.PI / 180)) * Math.Cos((DeliveryLatitude * Math.PI / 180))
                 * Math.Cos(((PickupLongitude - DeliveryLongitude) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344) / 1000;

            if (Distance < 5)
            {
                DeliveryCharge = 50;
            }
            else
            {
                var dist = Distance - 5;
                var amount = dist * 6;
                DeliveryCharge = 50 + amount;
            }
            return Json(new { DeliveryCharge, Distance }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "")]
        public JsonResult MultipleOrderAssignDeliveryBoy(MultipleOrderAssignDeliveryBoyViewModel model)
        {
            try
            {
                var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
                ViewBag.Name = user.Name;
                foreach (var item in model.OrderLists)
                {
                    var cart = db.Orders.FirstOrDefault(i => i.Id == item.OrderId);
                    if (cart != null && model.DeliveryBoyId != 0)
                    {
                        var delivery = db.DeliveryBoys.FirstOrDefault(i => i.Id == model.DeliveryBoyId);

                        cart.DeliveryBoyId = delivery.Id;
                        cart.DeliveryBoyName = delivery.Name;
                        cart.DeliveryBoyPhoneNumber = delivery.PhoneNumber;
                        cart.Status = 4;
                        cart.DateUpdated = DateTime.Now;
                        cart.UpdatedBy = user.Name;
                        db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        delivery.isAssign = 1;
                        delivery.DateUpdated = DateTime.Now;
                        delivery.UpdatedBy = user.Name;
                        db.Entry(delivery).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        if (delivery.CustomerId != 0)
                        {
                            var fcmToken = (from c in db.Customers
                                            where c.Id == delivery.CustomerId
                                            select c.FcmTocken ?? "").FirstOrDefault().ToString();
                            Helpers.PushNotification.SendbydeviceId("You have received new order. Accept Soon", "ShopNowChat", "a.mp3", fcmToken.ToString());
                        }
                        //Customer
                        if (cart.CustomerId != 0)
                        {
                            var fcmTokenCustomer = (from c in db.Customers
                                                    where c.Id == cart.CustomerId
                                                    select c.FcmTocken ?? "").FirstOrDefault().ToString();
                            Helpers.PushNotification.SendbydeviceId($"Delivery Boy {cart.DeliveryBoyName} is Assigned for your Order.", "ShopNowChat", "../../assets/b.mp3", fcmTokenCustomer.ToString());
                        }
                    }
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeliveredCancel(int id)
        {
            var order = db.Orders.FirstOrDefault(i => i.Id == id);
            order.Status = 7;
            order.CancelledRemark = "Test Order";
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Delivered");
        }

        public ActionResult UpdateAmount(CartUpdateAmountViewModel model)
        {
            var cart = db.Orders.FirstOrDefault(i => i.OrderNumber == model.OrderNumber);
            var payment = db.Payments.FirstOrDefault(i => i.OrderNumber == model.OrderNumber);
            if (cart != null && model.OrderNumber != 0)
            {
                cart.NetTotal = model.Amount;
                cart.Convinenientcharge = model.ConvenientCharge;
                cart.DeliveryCharge = model.GrossDeliveryCharge;
                cart.ShopDeliveryDiscount = model.ShopDeliveryDiscount;
                cart.NetDeliveryCharge = model.DeliveryCharge;
                cart.Distance = model.Distance;
                db.Entry(cart).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (payment != null && model.OrderNumber != 0)
            {
                payment.Amount = model.Amount;
                payment.GSTAmount = model.Amount;
                payment.ConvenientCharge = model.ConvenientCharge;
                payment.PackingCharge = model.PackingCharge;
                payment.DeliveryCharge = model.GrossDeliveryCharge;
                db.Entry(payment).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("List");
        }

        public JsonResult GetDeliveryDiscount(int OrderNumber)
        {
            var order = db.Orders.FirstOrDefault(i => i.OrderNumber == OrderNumber);
            var billingCharge = db.BillingCharges.FirstOrDefault(i => i.ShopId == order.ShopId && i.Status == 0);
            double ShopDeliveryDiscount = 0;
            if (billingCharge != null)
            {
                ShopDeliveryDiscount = order.TotalPrice * (billingCharge.DeliveryDiscountPercentage / 100);
            }
            return Json(ShopDeliveryDiscount, JsonRequestBehavior.AllowGet);
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

