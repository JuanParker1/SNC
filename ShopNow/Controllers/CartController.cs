using AutoMapper;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class CartController : Controller
    {
        private ShopnowchatEntities db = new ShopnowchatEntities();
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
                config.CreateMap<Cart, CartListViewModel.CartList>();
                config.CreateMap<Cart, CartDetailsViewModel>();

            });

            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNCARL001")]
        public ActionResult List(string shopcode = "")
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dt = DateTime.Now;
            var model = new CartListViewModel();
            if (shopcode != "")
            {
                model.List = db.Carts.Where(i => i.Status == 0 && i.ShopCode == shopcode && i.CartStatus != 6 && i.CartStatus != 7 && i.CartStatus !=0)
               //&& i.DateEncoded.Year == dt.Year && i.DateEncoded.Month == dt.Month && i.DateEncoded.Day == dt.Day
                .AsEnumerable().GroupBy(i => i.OrderNo).Select(i => new CartListViewModel.CartList
                {
                    Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                    ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                    OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                    DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                    PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                    CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                    DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                    DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now,
                    Date = i.Any() ? i.FirstOrDefault().DateEncoded.ToString("dd/MMM/yyyy hh:mm tt") : "N/A"
                }).OrderBy(i => i.CartStatus).OrderByDescending(i => i.DateEncoded).ToList();
            }
            else
            {
                model.List = db.Carts.Where(i => i.Status == 0 && i.CartStatus != 6 && i.CartStatus != 7 && i.CartStatus != 0)
               //&& i.DateEncoded.Year == dt.Year && i.DateEncoded.Month == dt.Month && i.DateEncoded.Day == dt.Day)
                .AsEnumerable().GroupBy(i => i.OrderNo).Select(i => new CartListViewModel.CartList
                {
                    Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                    ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                    OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                    DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                    PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                    CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                    DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                    DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now,
                    Date = i.Any() ? i.FirstOrDefault().DateEncoded.ToString("dd/MMM/yyyy hh:mm tt") : "N/A"
                }).OrderBy(i => i.CartStatus).OrderByDescending(i => i.DateEncoded).ToList();
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCARPE016")]
        public ActionResult Pending(string shopcode = "")
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartListViewModel();
            if (shopcode != "")
            {
                model.List = db.Carts.Where(i => i.ShopCode == shopcode && i.CartStatus == 2 && i.Status == 0)
                  .Join(db.Shops, c => c.ShopCode, s => s.Code, (c, s) => new { c, s })
                  .Join(db.Payments, c => c.c.OrderNo, p => p.OrderNo, (c, p) => new { c, p })
                    .AsEnumerable().GroupBy(i => i.c.c.OrderNo).Select(i => new CartListViewModel.CartList
                    {
                        Code = i.Any() ? i.FirstOrDefault().c.c.Code : "N/A",
                        ShopName = i.Any() ? i.FirstOrDefault().c.c.ShopName : "N/A",
                        OrderNo = i.Any() ? i.FirstOrDefault().c.c.OrderNo : "N/A",
                        DeliveryAddress = i.Any() ? i.FirstOrDefault().c.c.DeliveryAddress : "N/A",
                        PhoneNumber = i.Any() ? (i.FirstOrDefault().c.s.PhoneNumber != null ? i.FirstOrDefault().c.s.PhoneNumber : i.FirstOrDefault().c.s.ManualPhoneNumber) : "N/A",
                        CartStatus = i.Any() ? i.FirstOrDefault().c.c.CartStatus : 2,
                        DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.c.DeliveryBoyName : "N/A",
                        DateEncoded = i.Any() ? i.FirstOrDefault().c.c.DateEncoded : DateTime.Now,
                        Date = i.Any() ? i.FirstOrDefault().c.c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt") : "N/A",
                        Price = i.Any() ? i.Sum(a => a.c.c.Price) : 0,
                        RefundAmount = i.FirstOrDefault().p.refundAmount ?? 0,
                        RefundRemark = i.FirstOrDefault().p.refundRemark ?? "",
                        PaymentMode = i.FirstOrDefault().p.PaymentMode,
                    }).OrderByDescending(i => i.DateEncoded).ToList();
            }
            else
            {
                model.List = db.Carts.Where(i => i.CartStatus == 2 && i.Status == 0)
                    .Join(db.Shops, c=> c.ShopCode, s=> s.Code, (c,s)=> new {c,s})
                    .Join(db.Payments, c => c.c.OrderNo, p => p.OrderNo, (c, p) => new { c, p })
                    .AsEnumerable().GroupBy(i => i.c.c.OrderNo).Select(i => new CartListViewModel.CartList
                    {
                        Code = i.Any() ? i.FirstOrDefault().c.c.Code : "N/A",
                        ShopName = i.Any() ? i.FirstOrDefault().c.c.ShopName : "N/A",
                        OrderNo = i.Any() ? i.FirstOrDefault().c.c.OrderNo : "N/A",
                        DeliveryAddress = i.Any() ? i.FirstOrDefault().c.c.DeliveryAddress : "N/A",
                        PhoneNumber = i.Any() ? (i.FirstOrDefault().c.s.PhoneNumber != null ? i.FirstOrDefault().c.s.PhoneNumber : i.FirstOrDefault().c.s.ManualPhoneNumber) : "N/A",
                        CartStatus = i.Any() ? i.FirstOrDefault().c.c.CartStatus : 2,
                        DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.c.DeliveryBoyName : "N/A",
                        DateEncoded = i.Any() ? i.FirstOrDefault().c.c.DateEncoded : DateTime.Now,
                        Date = i.Any() ? i.FirstOrDefault().c.c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt") : "N/A",
                        Price = i.Any()? i.Sum(a=> a.c.c.Price) : 0,
                        RefundAmount = i.FirstOrDefault().p.refundAmount ?? 0,
                        RefundRemark = i.FirstOrDefault().p.refundRemark ?? "",
                        PaymentMode = i.FirstOrDefault().p.PaymentMode,
                    }).OrderByDescending(i => i.DateEncoded).ToList();
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCAROP017")]
        public ActionResult OrderPrepared(string shopcode = "") //Order Processing
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartListViewModel();
            if (shopcode != "")
            {
                model.List = db.Carts.Where(i => i.ShopCode == shopcode && (i.CartStatus == 3 || i.CartStatus == 4) && i.Status == 0)
                    .Join(db.Payments, c => c.OrderNo, p => p.OrderNo, (c, p) => new { c, p })
                    .AsEnumerable().GroupBy(i => i.c.OrderNo).Select(i => new CartListViewModel.CartList
                    {
                        Code = i.FirstOrDefault().c.Code,
                        ShopName = i.FirstOrDefault().c.ShopName,
                        OrderNo = i.FirstOrDefault().c.OrderNo,
                        DeliveryAddress = i.FirstOrDefault().c.DeliveryAddress,
                        PhoneNumber = i.FirstOrDefault().c.PhoneNumber,
                        CartStatus = i.FirstOrDefault().c.CartStatus,
                        DeliveryBoyName = i.FirstOrDefault().c.DeliveryBoyName,
                        DateEncoded = i.FirstOrDefault().c.DateEncoded,
                        Date = i.FirstOrDefault().c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt"),
                        RefundAmount = i.FirstOrDefault().p.refundAmount ?? 0,
                        RefundRemark = i.FirstOrDefault().p.refundRemark ?? "",
                        PaymentMode = i.FirstOrDefault().p.PaymentMode,
                        DeliveryPhoneNumber = i.FirstOrDefault().c.DeliveryBoyPhoneNumber ?? "Not Assigned",
                        Price = i.FirstOrDefault().p.Amount - (i.FirstOrDefault().p.refundAmount ?? 0)
                    }).OrderByDescending(i => i.DateEncoded).ToList();
            }
            else
            {
                model.List = db.Carts.Where(i => (i.CartStatus == 3 || i.CartStatus == 4) && i.Status == 0)
                    .Join(db.Payments, c => c.OrderNo, p => p.OrderNo, (c, p) => new { c, p })
                    .AsEnumerable().GroupBy(i => i.c.OrderNo).Select(i => new CartListViewModel.CartList
                    {
                        Code = i.FirstOrDefault().c.Code,
                        ShopName = i.FirstOrDefault().c.ShopName,
                        OrderNo = i.FirstOrDefault().c.OrderNo,
                        DeliveryAddress = i.FirstOrDefault().c.DeliveryAddress,
                        PhoneNumber = i.FirstOrDefault().c.PhoneNumber,
                        CartStatus = i.FirstOrDefault().c.CartStatus,
                        DeliveryBoyName = i.FirstOrDefault().c.DeliveryBoyName,
                        DateEncoded = i.FirstOrDefault().c.DateEncoded,
                        Date = i.FirstOrDefault().c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt"),
                        RefundAmount = i.FirstOrDefault().p.refundAmount ?? 0,
                        RefundRemark = i.FirstOrDefault().p.refundRemark ?? "",
                        PaymentMode = i.FirstOrDefault().p.PaymentMode,
                        DeliveryPhoneNumber = i.FirstOrDefault().c.DeliveryBoyPhoneNumber ?? "Not Assigned",
                        Price = i.FirstOrDefault().p.Amount - (i.FirstOrDefault().p.refundAmount ?? 0)
                    }).OrderByDescending(i => i.DateEncoded).ToList();
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCARDA018")]
        public ActionResult DeliveryAgentAssigned(string shopcode = "")
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartListViewModel();
            if (shopcode != "")
            {
                model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
                    .Join(db.Payments, c => c.c.OrderNo, p => p.OrderNo, (c, p) => new { c, p })
                    .Where(i => i.c.c.ShopCode == shopcode && i.c.c.CartStatus == 4 && i.c.c.Status == 0 && i.c.d.isAssign == 1 && i.c.d.OnWork == 0)
                    .AsEnumerable().GroupBy(i => i.c.c.OrderNo).Select(i => new CartListViewModel.CartList
                    {
                        Code = i.Any() ? i.FirstOrDefault().c.c.Code : "N/A",
                        ShopName = i.Any() ? i.FirstOrDefault().c.c.ShopName : "N/A",
                        OrderNo = i.Any() ? i.FirstOrDefault().c.c.OrderNo : "N/A",
                        DeliveryAddress = i.Any() ? i.FirstOrDefault().c.c.DeliveryAddress : "N/A",
                        PhoneNumber = i.Any() ? i.FirstOrDefault().c.d.PhoneNumber : "N/A",
                        CartStatus = i.Any() ? i.FirstOrDefault().c.c.CartStatus : 2,
                        DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.c.DeliveryBoyName : "N/A",
                        DeliveryBoyCode = i.Any() ? i.FirstOrDefault().c.c.DeliveryBoyCode : "N/A",
                        DateEncoded = i.Any() ? i.FirstOrDefault().c.c.DateEncoded : DateTime.Now,
                        Date = i.Any() ? i.FirstOrDefault().c.c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt") : "N/A",
                        RefundAmount = i.FirstOrDefault().p.refundAmount ?? 0,
                        RefundRemark = i.FirstOrDefault().p.refundRemark ?? "",
                        PaymentMode = i.FirstOrDefault().p.PaymentMode,
                    }).OrderByDescending(i => i.DateEncoded).ToList();
            }
            else
            {
                model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
                    .Join(db.Payments, c => c.c.OrderNo, p => p.OrderNo, (c, p) => new { c, p })
                    .Where(i => i.c.c.CartStatus == 4 && i.c.c.Status == 0 && i.c.d.isAssign == 1 && i.c.d.OnWork == 0)
                    .AsEnumerable().GroupBy(i => i.c.c.OrderNo).Select(i => new CartListViewModel.CartList
                    {
                        Code = i.Any() ? i.FirstOrDefault().c.c.Code : "N/A",
                        ShopName = i.Any() ? i.FirstOrDefault().c.c.ShopName : "N/A",
                        OrderNo = i.Any() ? i.FirstOrDefault().c.c.OrderNo : "N/A",
                        DeliveryAddress = i.Any() ? i.FirstOrDefault().c.c.DeliveryAddress : "N/A",
                        PhoneNumber = i.Any() ? i.FirstOrDefault().c.d.PhoneNumber : "N/A",
                        CartStatus = i.Any() ? i.FirstOrDefault().c.c.CartStatus : 2,
                        DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.c.DeliveryBoyName : "N/A",
                        DateEncoded = i.Any() ? i.FirstOrDefault().c.c.DateEncoded : DateTime.Now,
                        Date = i.Any() ? i.FirstOrDefault().c.c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt") : "N/A",
                        RefundAmount = i.FirstOrDefault().p.refundAmount ?? 0,
                        RefundRemark = i.FirstOrDefault().p.refundRemark ?? "",
                        PaymentMode = i.FirstOrDefault().p.PaymentMode,
                    }).OrderByDescending(i => i.DateEncoded).ToList();
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCARWP021")]
        public ActionResult WaitingForPickup(string shopcode = "")
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartListViewModel();
            //if (shopcode != "")
            //{
            //    model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
            //        .Where(i => i.c.ShopCode == shopcode && i.c.CartStatus == 4 && i.c.Status == 0 && i.d.isAssign == 1 && i.d.OnWork == 1)
            //        .AsEnumerable().GroupBy(i => i.c.OrderNo).Select(i => new CartListViewModel.CartList
            //        {
            //            Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
            //            ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
            //            OrderNo = i.Any() ? i.FirstOrDefault().c.OrderNo : "N/A",
            //            DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
            //            PhoneNumber = i.Any() ? i.FirstOrDefault().d.PhoneNumber : "N/A",
            //            CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
            //            DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
            //            DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now,
            //            Date = i.Any() ? i.FirstOrDefault().c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt") : "N/A"
            //        }).OrderByDescending(i => i.DateEncoded).ToList();
            //}
            //else
            //{
            //    model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
            //        .Where(i => i.c.CartStatus == 4 && i.c.Status == 0 && i.d.isAssign == 1 && i.d.OnWork == 1)
            //        .AsEnumerable().GroupBy(i => i.c.OrderNo).Select(i => new CartListViewModel.CartList
            //        {
            //            Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
            //            ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
            //            OrderNo = i.Any() ? i.FirstOrDefault().c.OrderNo : "N/A",
            //            DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
            //            PhoneNumber = i.Any() ? i.FirstOrDefault().d.PhoneNumber : "N/A",
            //            CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
            //            DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
            //            DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now,
            //            Date = i.Any() ? i.FirstOrDefault().c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt") : "N/A"
            //        }).OrderByDescending(i => i.DateEncoded).ToList();
            //}
            model.List = db.Carts.Where(i=>i.CartStatus == 4 && i.Status ==0 && (shopcode != "" ? i.ShopCode == shopcode : true))
                .Join(db.DeliveryBoys.Where(i=>i.isAssign ==1 && i.OnWork == 1), c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
                .Join(db.Payments,c=>c.c.OrderNo,p=>p.OrderNo,(c,p)=>new { c,p})
                .AsEnumerable()
                    .GroupBy(i => i.c.c.OrderNo).Select(i => new CartListViewModel.CartList
                    {
                        Code = i.FirstOrDefault().c.c.Code,
                        ShopName = i.FirstOrDefault().c.c.ShopName,
                        OrderNo = i.FirstOrDefault().c.c.OrderNo,
                        DeliveryAddress = i.FirstOrDefault().c.c.DeliveryAddress,
                        PhoneNumber = i.FirstOrDefault().c.d.PhoneNumber,
                        CartStatus = i.FirstOrDefault().c.c.CartStatus,
                        DeliveryBoyName = i.FirstOrDefault().c.c.DeliveryBoyName,
                        DateEncoded = i.FirstOrDefault().c.c.DateEncoded,
                        Date = i.FirstOrDefault().c.c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt"),
                        RefundAmount = i.FirstOrDefault().p.refundAmount ?? 0,
                        RefundRemark = i.FirstOrDefault().p.refundRemark ?? "",
                        PaymentMode = i.FirstOrDefault().p.PaymentMode,
                        Amount = i.Sum(a=> a.p.Amount - (a.p.refundAmount ?? 0)),
                    }).OrderByDescending(i => i.DateEncoded).ToList();
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCAROT007")]
        public ActionResult OnTheWay(string shopcode = "")
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartListViewModel();
            if (shopcode != "")
            {
                model.List = db.Carts.Where(i => i.ShopCode == shopcode && i.CartStatus == 5 && i.Status == 0)
                    .Join(db.Payments, c => c.OrderNo, p => p.OrderNo, (c, p) => new { c, p })
                    .AsEnumerable().GroupBy(i => i.c.OrderNo).Select(i => new CartListViewModel.CartList
                    {
                        Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
                        ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
                        OrderNo = i.Any() ? i.FirstOrDefault().c.OrderNo : "N/A",
                        DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
                        PhoneNumber = i.Any() ? i.FirstOrDefault().c.DeliveryBoyPhoneNumber : "N/A",
                        CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
                        DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
                        DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now,
                        Date = i.Any() ? i.FirstOrDefault().c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt") : "N/A",
                        RefundAmount = i.FirstOrDefault().p.refundAmount ?? 0,
                        RefundRemark = i.FirstOrDefault().p.refundRemark ?? "",
                        PaymentMode = i.FirstOrDefault().p.PaymentMode,
                    }).OrderByDescending(i => i.DateEncoded).ToList();
            }
            else
            {
                model.List = db.Carts.Where(i => i.CartStatus == 5 && i.Status == 0)
                    .Join(db.Payments, c => c.OrderNo, p => p.OrderNo, (c, p) => new { c, p })
                    .AsEnumerable().GroupBy(i => i.c.OrderNo).Select(i => new CartListViewModel.CartList
                    {
                        Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
                        ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
                        OrderNo = i.Any() ? i.FirstOrDefault().c.OrderNo : "N/A",
                        DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
                        PhoneNumber = i.Any() ? i.FirstOrDefault().c.DeliveryBoyPhoneNumber : "N/A",
                        CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
                        DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
                        DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now,
                        Date = i.Any() ? i.FirstOrDefault().c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt") : "N/A",
                        RefundAmount = i.FirstOrDefault().p.refundAmount ?? 0,
                        RefundRemark = i.FirstOrDefault().p.refundRemark ?? "",
                        PaymentMode = i.FirstOrDefault().p.PaymentMode,
                    }).OrderByDescending(i => i.DateEncoded).ToList();
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCARDL006")]
        public ActionResult Delivered(string shopcode = "")
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartListViewModel();
            if (shopcode != "")
            {
                model.List = db.Carts.Where(i => i.ShopCode == shopcode && i.CartStatus == 6 && i.Status == 0)
                    .Join(db.Payments, c => c.OrderNo, p => p.OrderNo, (c, p) => new { c, p })
                    .AsEnumerable().GroupBy(i => i.c.OrderNo).Select(i => new CartListViewModel.CartList
                    {
                        Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
                        ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
                        OrderNo = i.Any() ? i.FirstOrDefault().c.OrderNo : "N/A",
                        DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
                        PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "N/A",
                        CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
                        DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
                        DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now,
                        Date = i.Any() ? i.FirstOrDefault().c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt") : "N/A",
                        RefundAmount = i.FirstOrDefault().p.refundAmount ?? 0,
                        RefundRemark = i.FirstOrDefault().p.refundRemark ?? "",
                        PaymentMode = i.FirstOrDefault().p.PaymentMode,
                    }).OrderByDescending(i => i.DateEncoded).ToList();
            }
            else
            {
                model.List = db.Carts.Where(i => i.CartStatus == 6 && i.Status == 0)
                    .Join(db.Payments, c => c.OrderNo, p => p.OrderNo, (c, p) => new { c, p })
                    .AsEnumerable().GroupBy(i => i.c.OrderNo).Select(i => new CartListViewModel.CartList
                    {
                        Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
                        ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
                        OrderNo = i.Any() ? i.FirstOrDefault().c.OrderNo : "N/A",
                        DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
                        PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "N/A",
                        CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
                        DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
                        DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now,
                        Date = i.Any() ? i.FirstOrDefault().c.DateEncoded.ToString("dd/MMM/yyyy hh:mm tt") : "N/A",
                        RefundAmount = i.FirstOrDefault().p.refundAmount ?? 0,
                        RefundRemark = i.FirstOrDefault().p.refundRemark ?? "",
                        PaymentMode = i.FirstOrDefault().p.PaymentMode,
                    }).OrderByDescending(i => i.DateEncoded).ToList();
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCARCA019")]
        public ActionResult Cancelled(string shopcode = "")
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartListViewModel();
            if (shopcode != "")
            {
                model.List = db.Carts.Where(i => i.ShopCode == shopcode && i.CartStatus == 7 && i.Status == 0)
                    .AsEnumerable().GroupBy(i => i.OrderNo).Select(i => new CartListViewModel.CartList
                    {
                        Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                        ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                        OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                        DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                        PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                        CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                        DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                        DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now,
                        Date = i.Any() ? i.FirstOrDefault().DateEncoded.ToString("dd/MMM/yyyy hh:mm tt") : "N/A"
                    }).OrderByDescending(i => i.DateEncoded).ToList();
            }
            else
            {
                model.List = db.Carts.Where(i => i.CartStatus == 7 && i.Status == 0)
                    .AsEnumerable().GroupBy(i => i.OrderNo).Select(i => new CartListViewModel.CartList
                    {
                        Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                        ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                        OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                        DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                        PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                        CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                        DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                        DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now,
                        Date = i.Any() ? i.FirstOrDefault().DateEncoded.ToString("dd/MMM/yyyy hh:mm tt") : "N/A"
                    }).OrderByDescending(i => i.DateEncoded).ToList();
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCARD005")]
        public ActionResult Details(string code)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            Cart cart = db.Carts.FirstOrDefault(i => i.Code == code);
            var model = new CartDetailsViewModel();
            _mapper.Map(cart, model);
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNCARPS025")]
        public ActionResult PickupSlip(string orderno, string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dCode))
                return HttpNotFound();
            var cart = db.Carts.FirstOrDefault(i => i.Code == dCode);
            var payment = db.Payments.FirstOrDefault(i => i.OrderNo == orderno);//Payment.GetOrderNo(orderno);
            var model = new CartListViewModel();
            if (cart != null)
            {
                model.Code = code;
                model.OrderNo = orderno;
                model.CustomerCode = cart.CustomerCode;
                model.CustomerName = cart.CustomerName;
                model.CartStatus = cart.CartStatus;
                model.ShopName = cart.ShopName;
                model.DeliveryAddress = cart.DeliveryAddress;
                model.PhoneNumber = cart.PhoneNumber;
                model.DeliveryBoyName = cart.DeliveryBoyName;
                model.DateEncoded = cart.DateEncoded;
                var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Code == cart.DeliveryBoyCode); //DeliveryBoy.Get(cart.DeliveryBoyCode);
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
                model.List = db.Carts.Where(i => i.OrderNo == orderno && i.Status == 0).Select(i => new CartListViewModel.CartList
            {
                Code = i.Code,
                BrandName = i.BrandName,
                CategoryName = i.CategoryName,
                ShopName = i.ShopName,
                ProductCode = i.ProductCode,
                ProductName = i.ProductName,
                Qty = i.Qty,
                Price = i.Price,
                CartStatus = i.CartStatus,
                PhoneNumber = i.PhoneNumber,
                ImagePath = i.ImagePath == "N/A" ? null : i.ImagePath,
                SinglePrice = i.SinglePrice
            }).ToList();
            return View(model);            
        }
         
        [AccessPolicy(PageCode = "SHNCARE004")]
        public ActionResult Edit(string orderno, string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var cart = db.Carts.FirstOrDefault(i => i.Code == dCode);
            var model = new CartListViewModel();
            if (cart != null)
            {
                model.Code = code;
                model.OrderNo = orderno;
                model.CustomerCode = cart.CustomerCode;
                model.CartStatus = cart.CartStatus;
                model.ShopName = cart.ShopName;
                model.DeliveryAddress = cart.DeliveryAddress;
                model.PhoneNumber = cart.PhoneNumber;
                model.DeliveryBoyName = cart.DeliveryBoyName;
                var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Code == cart.DeliveryBoyCode); //DeliveryBoy.Get(cart.DeliveryBoyCode);
                if (deliveryBoy != null)
                {
                    model.isAssign = deliveryBoy.isAssign;
                    model.OnWork = deliveryBoy.OnWork;
                }
            }
            model.List = db.Carts.Where(i => i.OrderNo == orderno && i.Status == 0).Select(i => new CartListViewModel.CartList
            {
                Code = i.Code,
                BrandName = i.BrandName,
                CategoryName = i.CategoryName,
                ShopName = i.ShopName,
                ProductCode = i.ProductCode,
                ProductName = i.ProductName,
                Qty = i.Qty,
                Price = i.Price,
                CartStatus = i.CartStatus,
                PhoneNumber = i.PhoneNumber,
                ImagePath = i.ImagePath == "N/A" ? null : i.ImagePath
            }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNCARPR008")]
        public ActionResult PendingReport(DateTime? StartDate, DateTime? EndDate, string ShopCode = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartReportViewModel();

            if (ShopCode != "")
            {
                var shop = db.Shops.FirstOrDefault(i => i.Code == ShopCode);//Shop.Get(ShopCode);
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
                    model.List = db.Carts.Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.ShopCode == ShopCode && i.CartStatus == 2 && i.Status == 0)
                 .AsEnumerable().GroupBy(i => i.OrderNo)
                  .Select(i => new CartReportViewModel.CartReportList
                  {
                      Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                      ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                      OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                      DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                      PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                      CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                      DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                      DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
                  }).OrderByDescending(i => i.DateEncoded).ToList();
                    ViewBag.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    ViewBag.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                    ViewBag.ShopCode = ShopCode;
                    ViewBag.ShopName = shop.Name;
                }
                else
                {
                    model.List = db.Carts.Where(i => i.ShopCode == ShopCode && i.CartStatus == 2 && i.Status == 0)
                 .AsEnumerable().GroupBy(i => i.OrderNo)
                  .Select(i => new CartReportViewModel.CartReportList
                  {
                      Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                      ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                      OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                      DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                      PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                      CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                      DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                      DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
                  }).OrderByDescending(i => i.DateEncoded).ToList();
                    ViewBag.ShopCode = ShopCode;
                    ViewBag.ShopName = shop.Name;
                }

            }
            else
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
                    model.List = db.Carts.Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.CartStatus == 2 && i.Status == 0)
                  .AsEnumerable().GroupBy(i => i.OrderNo)
                   .Select(i => new CartReportViewModel.CartReportList
                   {
                       Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                       ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                       OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                       DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                       PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                       CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                       DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                       DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
                   }).OrderByDescending(i => i.DateEncoded).ToList();
                    ViewBag.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    ViewBag.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    model.List = db.Carts.Where(i => i.CartStatus == 2 && i.Status == 0)
                .AsEnumerable().GroupBy(i => i.OrderNo)
                 .Select(i => new CartReportViewModel.CartReportList
                 {
                     Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                     ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                     OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                     DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                     PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                     CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                     DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                     DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
                 }).OrderByDescending(i => i.DateEncoded).ToList();
                }
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCARDAR022")]
        public ActionResult DeliveryAgentAssignedReport(DateTime? StartDate, DateTime? EndDate, string ShopCode = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartReportViewModel();

            if (ShopCode != "")
            {
                var shop = db.Shops.FirstOrDefault(i => i.Code == ShopCode);//Shop.Get(ShopCode);
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
                    .Where(i => i.c.DateEncoded >= startDatetFilter && i.c.DateEncoded <= endDateFilter && i.c.Status == 0 && i.c.ShopCode == ShopCode && i.c.CartStatus == 4 && i.d.isAssign == 1 && i.d.OnWork == 0)
                    .AsEnumerable().GroupBy(i => i.c.OrderNo).Select(i => new CartReportViewModel.CartReportList
                    {
                        Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
                        ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
                        OrderNo = i.Any() ? i.FirstOrDefault().c.OrderNo : "N/A",
                        DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
                        PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "N/A",
                        CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
                        DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
                        DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now
                    }).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                    model.ShopCode = ShopCode;
                    model.ShopName = shop.Name;
                }
                else
                {
                    model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
                   .Where(i => i.c.Status == 0 && i.c.ShopCode == ShopCode && i.c.CartStatus == 4 && i.d.isAssign == 1 && i.d.OnWork == 0)
                   .AsEnumerable().GroupBy(i => i.c.OrderNo).Select(i => new CartReportViewModel.CartReportList
                   {
                       Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
                       ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
                       OrderNo = i.Any() ? i.FirstOrDefault().c.OrderNo : "N/A",
                       DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
                       PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "N/A",
                       CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
                       DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
                       DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now
                   }).ToList();
                    model.ShopCode = ShopCode;
                    model.ShopName = shop.Name;
                }
            }
            else
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
                    model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
                    .Where(i => i.c.DateEncoded >= startDatetFilter && i.c.DateEncoded <= endDateFilter && i.c.Status == 0 && i.c.CartStatus == 4 && i.d.isAssign == 1 && i.d.OnWork == 0)
                    .AsEnumerable().GroupBy(i => i.c.OrderNo).Select(i => new CartReportViewModel.CartReportList
                    {
                        Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
                        ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
                        OrderNo = i.Any() ? i.FirstOrDefault().c.OrderNo : "N/A",
                        DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
                        PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "N/A",
                        CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
                        DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
                        DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now
                    }).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
                   .Where(i => i.c.Status == 0 && i.c.CartStatus == 4 && i.d.isAssign == 1 && i.d.OnWork == 0)
                   .AsEnumerable().GroupBy(i => i.c.OrderNo).Select(i => new CartReportViewModel.CartReportList
                   {
                       Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
                       ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
                       OrderNo = i.Any() ? i.FirstOrDefault().c.OrderNo : "N/A",
                       DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
                       PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "N/A",
                       CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
                       DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
                       DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now
                   }).ToList();
                }
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCARWPR023")]
        public ActionResult WaitingForPickupReport(DateTime? StartDate, DateTime? EndDate, string ShopCode = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartReportViewModel();

            if (ShopCode != "")
            {
                var shop = db.Shops.FirstOrDefault(i => i.Code == ShopCode);//Shop.Get(ShopCode);
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
                    .Where(i => i.c.DateEncoded >= startDatetFilter && i.c.DateEncoded <= endDateFilter && i.c.Status == 0 && i.c.ShopCode == ShopCode && i.c.CartStatus == 4 && i.d.isAssign == 1 && i.d.OnWork == 1)
                    .AsEnumerable().GroupBy(i => i.c.OrderNo).Select(i => new CartReportViewModel.CartReportList
                    {
                        Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
                        ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
                        OrderNo = i.Any() ? i.FirstOrDefault().c.OrderNo : "N/A",
                        DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
                        PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "N/A",
                        CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
                        DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
                        DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now
                    }).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                    model.ShopCode = ShopCode;
                    model.ShopName = shop.Name;
                }
                else
                {
                    model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
                   .Where(i => i.c.Status == 0 && i.c.ShopCode == ShopCode && i.c.CartStatus == 4 && i.d.isAssign == 1 && i.d.OnWork == 1)
                   .AsEnumerable().GroupBy(i => i.c.OrderNo).Select(i => new CartReportViewModel.CartReportList
                   {
                       Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
                       ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
                       OrderNo = i.Any() ? i.FirstOrDefault().c.OrderNo : "N/A",
                       DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
                       PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "N/A",
                       CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
                       DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
                       DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now
                   }).ToList();
                    model.ShopCode = ShopCode;
                    model.ShopName = shop.Name;
                }

            }
            else
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
                    model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
                    .Where(i => i.c.DateEncoded >= startDatetFilter && i.c.DateEncoded <= endDateFilter && i.c.Status == 0 && i.c.CartStatus == 4 && i.d.isAssign == 1 && i.d.OnWork == 1)
                    .AsEnumerable().GroupBy(i => i.c.OrderNo).Select(i => new CartReportViewModel.CartReportList
                    {
                        Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
                        ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
                        OrderNo = i.Any() ? i.FirstOrDefault().c.OrderNo : "N/A",
                        DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
                        PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "N/A",
                        CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
                        DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
                        DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now
                    }).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    model.List = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
                   .Where(i => i.c.Status == 0 && i.c.CartStatus == 4 && i.d.isAssign == 1 && i.d.OnWork == 1)
                   .AsEnumerable().GroupBy(i => i.c.OrderNo).Select(i => new CartReportViewModel.CartReportList
                   {
                       Code = i.Any() ? i.FirstOrDefault().c.Code : "N/A",
                       ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "N/A",
                       OrderNo = i.Any() ? i.FirstOrDefault().c.OrderNo : "N/A",
                       DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "N/A",
                       PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "N/A",
                       CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 2,
                       DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "N/A",
                       DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now
                   }).ToList();
                }
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCAROR009")]
        public ActionResult OntheWayReport(DateTime? StartDate, DateTime? EndDate, string ShopCode = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartReportViewModel();

            if (ShopCode != "")
            {
                var shop = db.Shops.FirstOrDefault(i => i.Code == ShopCode);//Shop.Get(ShopCode);
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
                    
                    model.List = db.Carts.Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.ShopCode == ShopCode && i.CartStatus == 5 && i.Status == 0)
                   .AsEnumerable().GroupBy(i => i.OrderNo).Select(i => new CartReportViewModel.CartReportList
                   {
                       Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                       ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                       OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                       DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                       PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                       CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                       DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                       DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
                   }).OrderByDescending(i => i.DateEncoded).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                    model.ShopCode = ShopCode;
                    model.ShopName = shop.Name;
                }
                else
                {
                    model.List = db.Carts.Where(i => i.ShopCode == ShopCode && i.CartStatus == 5 && i.Status == 0)
                   .AsEnumerable().GroupBy(i => i.OrderNo).Select(i => new CartReportViewModel.CartReportList
                   {
                       Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                       ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                       OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                       DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                       PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                       CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                       DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                       DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
                   }).OrderByDescending(i => i.DateEncoded).ToList();
                    model.ShopCode = ShopCode;
                    model.ShopName = shop.Name;
                }

            }
            else
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
                    model.List = db.Carts.Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.CartStatus == 5 && i.Status == 0)
                   .AsEnumerable().GroupBy(i => i.OrderNo).Select(i => new CartReportViewModel.CartReportList
                   {
                       Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                       ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                       OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                       DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                       PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                       CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                       DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                       DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
                   }).OrderByDescending(i => i.DateEncoded).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    model.List = db.Carts.Where(i => i.CartStatus == 5 && i.Status == 0)
                  .AsEnumerable().GroupBy(i => i.OrderNo).Select(i => new CartReportViewModel.CartReportList
                  {
                      Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                      ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                      OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                      DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                      PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                      CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                      DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                      DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
                  }).OrderByDescending(i => i.DateEncoded).ToList();
                }
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCARDR010")]
        public ActionResult DeliveredReport(DateTime? StartDate, DateTime? EndDate, string ShopCode = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartReportViewModel();

            if (ShopCode != "")
            {
                var shop = db.Shops.FirstOrDefault(i => i.Code == ShopCode);//Shop.Get(ShopCode);
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
                    model.List = db.Carts.Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.ShopCode == ShopCode && i.CartStatus == 6 && i.Status == 0)
                   .AsEnumerable().GroupBy(i => i.OrderNo).Select(i => new CartReportViewModel.CartReportList
                   {
                       Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                       ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                       OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                       DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                       PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                       CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                       DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                       DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
                   }).OrderByDescending(i => i.DateEncoded).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                    model.ShopCode = ShopCode;
                    model.ShopName = shop.Name;
                }
                else
                {
                    model.List = db.Carts.Where(i => i.ShopCode == ShopCode && i.CartStatus == 6 && i.Status == 0)
                  .AsEnumerable().GroupBy(i => i.OrderNo).Select(i => new CartReportViewModel.CartReportList
                  {
                      Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                      ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                      OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                      DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                      PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                      CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                      DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                      DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
                  }).OrderByDescending(i => i.DateEncoded).ToList();
                    model.ShopCode = ShopCode;
                    model.ShopName = shop.Name;
                }

            }
            else
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
                    model.List = db.Carts.Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.CartStatus == 6 && i.Status == 0)
                  .AsEnumerable().GroupBy(i => i.OrderNo).Select(i => new CartReportViewModel.CartReportList
                  {
                      Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                      ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                      OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                      DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                      PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                      CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                      DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                      DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
                  }).OrderByDescending(i => i.DateEncoded).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    model.List = db.Carts.Where(i => i.CartStatus == 6 && i.Status == 0)
                  .AsEnumerable().GroupBy(i => i.OrderNo).Select(i => new CartReportViewModel.CartReportList
                  {
                      Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                      ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                      OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                      DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                      PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                      CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                      DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                      DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
                  }).OrderByDescending(i => i.DateEncoded).ToList();
                }
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCARCR020")]
        public ActionResult CancelledReport(DateTime? StartDate, DateTime? EndDate, string ShopCode = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartReportViewModel();

            if (ShopCode != "")
            {
                var shop = db.Shops.FirstOrDefault(i => i.Code == ShopCode);//Shop.Get(ShopCode);
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
                    
                    model.List = db.Carts.Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.ShopCode == ShopCode && i.CartStatus == 7 && i.Status == 0)
                    .AsEnumerable().GroupBy(i => i.OrderNo).Select(i => new CartReportViewModel.CartReportList
                    {
                        Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                        ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                        OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                        DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                        PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                        CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                        DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                        DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
                    }).OrderByDescending(i => i.DateEncoded).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                    model.ShopCode = ShopCode;
                    model.ShopName = shop.Name;
                }
                else
                {
                    model.List = db.Carts.Where(i =>i.ShopCode == ShopCode && i.CartStatus == 7 && i.Status == 0)
                   .AsEnumerable().GroupBy(i => i.OrderNo).Select(i => new CartReportViewModel.CartReportList
                   {
                       Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                       ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                       OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                       DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                       PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                       CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                       DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                       DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
                   }).OrderByDescending(i => i.DateEncoded).ToList();
                    model.ShopCode = ShopCode;
                    model.ShopName = shop.Name;
                }

            }
            else
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);
                    model.List = db.Carts.Where(i => i.DateEncoded >= startDatetFilter && i.DateEncoded <= endDateFilter && i.CartStatus == 7 && i.Status == 0)
                   .AsEnumerable().GroupBy(i => i.OrderNo).Select(i => new CartReportViewModel.CartReportList
                   {
                       Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                       ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                       OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                       DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                       PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                       CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                       DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                       DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
                   }).OrderByDescending(i => i.DateEncoded).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    model.List = db.Carts.Where(i => i.CartStatus == 7 && i.Status == 0)
                   .AsEnumerable().GroupBy(i => i.OrderNo).Select(i => new CartReportViewModel.CartReportList
                   {
                       Code = i.Any() ? i.FirstOrDefault().Code : "N/A",
                       ShopName = i.Any() ? i.FirstOrDefault().ShopName : "N/A",
                       OrderNo = i.Any() ? i.FirstOrDefault().OrderNo : "N/A",
                       DeliveryAddress = i.Any() ? i.FirstOrDefault().DeliveryAddress : "N/A",
                       PhoneNumber = i.Any() ? i.FirstOrDefault().PhoneNumber : "N/A",
                       CartStatus = i.Any() ? i.FirstOrDefault().CartStatus : 2,
                       DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "N/A",
                       DateEncoded = i.Any() ? i.FirstOrDefault().DateEncoded : DateTime.Now
                   }).OrderByDescending(i => i.DateEncoded).ToList();
                }
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNCARAD015")]
        public ActionResult AssignDeliveryBoy(string code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var cart = db.Carts.Where(i => i.OrderNo == code).FirstOrDefault();
            var shop = db.Shops.FirstOrDefault(i => i.Code == cart.ShopCode);
            var model = new CartAssignDeliveryBoyViewModel();
            model.CartCode = cart.Code;
            DateTime date = DateTime.Now;
            
            var amount = (from i in db.ShopCharges
                                   where i.OrderNo == code && i.Status == 0 && i.CartStatus == 6 && i.DateUpdated.Year == date.Year && i.DateUpdated.Month == date.Month && i.DateUpdated.Day == date.Day
                                   select (Double?)i.GrossDeliveryCharge).Sum() ?? 0;

            model.Lists = db.DeliveryBoys
               .Where(i => i.OnWork == 0 && i.isAssign == 0 && i.Active == 1 && i.Status == 0)
                 .AsEnumerable()
                 .Select(i => new CartAssignDeliveryBoyViewModel.CartAssignList
                 {
                     Code = i.Code,
                     //  Name = "D" + _generatedDelivaryId,
                     Name = i.Name,
                     Status = i.Status,
                     Amount = amount,
                     Meters = (((Math.Acos(Math.Sin((shop.Latitude * Math.PI / 180)) * Math.Sin((i.Latitude * Math.PI / 180)) + Math.Cos((shop.Latitude * Math.PI / 180)) * Math.Cos((i.Latitude * Math.PI / 180))
                 * Math.Cos(((shop.Longitude - i.Longitude) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344)
                 }).Where(i => i.Meters < 8000 && i.Status == 0).ToList();

            //model.Lists = db.DeliveryBoys.Join(db.ShopCharges, d => d.CustomerCode, sc => sc.CustomerCode, (d, sc) => new { d, sc })
            //    .Where(i => i.d.OnWork == 0 && i.d.isAssign == 0 && i.d.Active == 1)
            //      .AsEnumerable()
            //      .GroupBy(i => i.d.CustomerCode)
            //      .Select(i => new CartAssignDeliveryBoyViewModel.CartAssignList
            //      {
            //          Code = i.Any() ? i.FirstOrDefault().d.Code : "N/A",
            //          Name = "D" + _generatedDelivaryId,
            //          DeliveryBoyName = i.Any() ? i.FirstOrDefault().d.Name : "N/A",
            //          Status = i.Any() ? i.FirstOrDefault().d.Status : 0,
            //          Amount = i.Any() ? amount : 0.0,
            //          Meters = (((Math.Acos(Math.Sin((shop.Latitude * Math.PI / 180)) * Math.Sin((i.FirstOrDefault().d.Latitude * Math.PI / 180)) + Math.Cos((shop.Latitude * Math.PI / 180)) * Math.Cos((i.FirstOrDefault().d.Latitude * Math.PI / 180))
            //      * Math.Cos(((shop.Longitude - i.FirstOrDefault().d.Longitude) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344)
            //      }).Where(i => i.Meters < 8000 && i.Status == 0).ToList();

            return View(model);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SHNCARAD015")]
        public ActionResult AssignDeliveryBoy(CartAssignDeliveryBoyViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var cart = db.Carts.FirstOrDefault(i => i.Code == model.CartCode); //Cart.Get(model.CartCode);
            if (cart != null && model.DeliveryBoyCode != null)
            {
                var cartList = db.Carts.Where(i => i.OrderNo == cart.OrderNo).ToList();
                var delivary = db.DeliveryBoys.FirstOrDefault(i => i.Code == model.DeliveryBoyCode);//DeliveryBoy.Get(model.DeliveryBoyCode);
                foreach (var c in cartList)
                {
                    var carts = GetCart(c.Code);// db.Carts.FirstOrDefault(i => i.Code == c.Code);//Cart.Get(c.Code);
                    carts.DeliveryBoyCode = delivary.Code;
                    carts.DeliveryBoyName = delivary.Name;
                    carts.DeliveryBoyPhoneNumber = delivary.PhoneNumber;
                    carts.CartStatus = 4;
                    carts.DateUpdated = DateTime.Now;
                    db.Entry(carts).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    //Cart.Edit(carts, out int err);
                }

                delivary.isAssign = 1;
                //DeliveryBoy.Edit(delivary, out int errors);
                delivary.DateUpdated = DateTime.Now;
                db.Entry(delivary).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();


                var detail = db.ShopCharges.FirstOrDefault(i => i.OrderNo == cart.OrderNo); //ShopCharge.GetOrderNo(cart.OrderNo);
                detail.CustomerCode = delivary.CustomerCode;
                detail.CustomerName = delivary.CustomerName;
                detail.DeliveryBoyCode = delivary.Code;
                detail.DeliveryBoyName = delivary.Name;
                detail.CartStatus = 4;
                detail.UpdatedBy = user.Name;
                detail.DateUpdated = DateTime.Now;
                db.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //ShopCharge.Edit(detail, out int errorss);
                return RedirectToAction("List");
            }
            else
            {
                return View(model);
            }

        }

        [AccessPolicy(PageCode = "SHNCARDAR024")]
        public ActionResult DeliveredAmountReport(DateTime? StartDate, DateTime? EndDate, string ShopCode = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartReportViewModel();

            if (ShopCode != "")
            {
                var shop = db.Shops.FirstOrDefault(i => i.Code == ShopCode);//Shop.Get(ShopCode);
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.List = db.Payments.Join(db.Carts, p => p.OrderNo, c => c.OrderNo, (p, c) => new { p, c })
                    .Where(i => i.p.DateEncoded >= startDatetFilter && i.p.DateEncoded <= endDateFilter && i.p.Status == 0 && i.c.CartStatus == 6 && i.c.ShopCode == ShopCode && i.p.PaymentMode == "Cash On Hand")
                     .GroupBy(i => i.p.OrderNo)
                     .Select(i => new CartReportViewModel.CartReportList
                     {
                         Code = i.Any() ? i.FirstOrDefault().p.Code : "",
                         ShopName = i.Any() ? i.FirstOrDefault().p.ShopName : "",
                         OrderNo = i.Any() ? i.FirstOrDefault().p.OrderNo : "",
                         DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "",
                         PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "",
                         CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 0,
                         DateEncoded = i.Any() ? i.FirstOrDefault().p.DateEncoded : DateTime.Now,
                         Amount = i.Any() ? i.FirstOrDefault().p.OriginalAmount : 0.0,
                     }).OrderByDescending(i => i.DateEncoded).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                    model.ShopCode = ShopCode;
                    model.ShopName = shop.Name;
                }
                else
                {
                    model.List = db.Payments.Join(db.Carts, p => p.OrderNo, c => c.OrderNo, (p, c) => new { p, c })
                      .Where(i => i.p.Status == 0 && i.c.CartStatus == 6 && i.c.ShopCode == ShopCode && i.p.PaymentMode == "Cash On Hand")
                      .GroupBy(i => i.p.OrderNo)
                      .Select(i => new CartReportViewModel.CartReportList
                      {
                          Code = i.Any() ? i.FirstOrDefault().p.Code : "",
                          ShopName = i.Any() ? i.FirstOrDefault().p.ShopName : "",
                          OrderNo = i.Any() ? i.FirstOrDefault().p.OrderNo : "",
                          DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "",
                          PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "",
                          CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 0,
                          DateEncoded = i.Any() ? i.FirstOrDefault().p.DateEncoded : DateTime.Now,
                          Amount = i.Any() ? i.FirstOrDefault().p.OriginalAmount : 0.0,
                      }).OrderByDescending(i => i.DateEncoded).ToList();
                    model.ShopCode = ShopCode;
                    model.ShopName = shop.Name;
                }

            }
            else
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.List = db.Payments.Join(db.Carts, p => p.OrderNo, c => c.OrderNo, (p, c) => new { p, c })
                    .Where(i => i.p.DateEncoded >= startDatetFilter && i.p.DateEncoded <= endDateFilter && i.p.Status == 0 && i.c.CartStatus == 6 && i.p.PaymentMode == "Cash On Hand")
                     .GroupBy(i => i.p.OrderNo)
                     .Select(i => new CartReportViewModel.CartReportList
                     {
                         Code = i.Any() ? i.FirstOrDefault().p.Code : "",
                         ShopName = i.Any() ? i.FirstOrDefault().p.ShopName : "",
                         OrderNo = i.Any() ? i.FirstOrDefault().p.OrderNo : "",
                         DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "",
                         PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "",
                         CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 0,
                         DateEncoded = i.Any() ? i.FirstOrDefault().p.DateEncoded : DateTime.Now,
                         Amount = i.Any() ? i.FirstOrDefault().p.OriginalAmount : 0.0,
                     }).OrderByDescending(i => i.DateEncoded).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    model.List = db.Payments.Join(db.Carts, p => p.OrderNo, c => c.OrderNo, (p, c) => new { p, c })
                       .Where(i => i.p.Status == 0 && i.c.CartStatus == 6 && i.p.PaymentMode == "Cash On Hand")
                       .GroupBy(i => i.p.OrderNo)
                       .Select(i => new CartReportViewModel.CartReportList
                       {
                           Code = i.Any() ? i.FirstOrDefault().p.Code : "",
                           ShopName = i.Any() ? i.FirstOrDefault().p.ShopName : "",
                           OrderNo = i.Any() ? i.FirstOrDefault().p.OrderNo : "",
                           DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "",
                           PhoneNumber = i.Any() ? i.FirstOrDefault().c.PhoneNumber : "",
                           CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 0,
                           DateEncoded = i.Any() ? i.FirstOrDefault().p.DateEncoded : DateTime.Now,
                           Amount = i.Any() ? i.FirstOrDefault().p.OriginalAmount : 0.0,
                       }).OrderByDescending(i => i.DateEncoded).ToList();
                }
            }
            return View(model.List);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult ShopNowChat_ShopReport(DateTime? StartDate, DateTime? EndDate, string shopcode = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartReportViewModel();

            if (shopcode != "")
            {
                var shop = db.Shops.FirstOrDefault(i => i.Code == shopcode);//Shop.Get(shopcode);
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.List = db.Payments.Join(db.Carts, p => p.OrderNo, c => c.OrderNo, (p, c) => new { p, c })
                    .Where(i => i.p.DateEncoded >= startDatetFilter && i.p.DateEncoded <= endDateFilter 
                    && i.p.Status == 0 && i.c.CartStatus == 6 && i.c.ShopCode == shopcode)
                     .GroupBy(i => i.p.OrderNo)
                     .Select(i => new CartReportViewModel.CartReportList
                     {
                         Code = i.Any() ? i.FirstOrDefault().c.Code : "",
                         ShopName = i.Any() ? i.FirstOrDefault().p.ShopName : "",
                         OrderNo = i.Any() ? i.FirstOrDefault().p.OrderNo : "",
                         CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 0,
                         DateEncoded = i.Any() ? i.FirstOrDefault().p.DateEncoded : DateTime.Now,
                         OriginalAmount = i.Any() ? i.FirstOrDefault().p.OriginalAmount : 0.0,
                         ShopPaymentStatus = i.Any() ? i.FirstOrDefault().c.ShopPaymentStatus : 0
                     }).OrderByDescending(i => i.DateEncoded).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                    model.ShopCode = shopcode;
                    model.ShopName = shop.Name;
                }
                else
                {
                    model.List = db.Payments.Join(db.Carts, p => p.OrderNo, c => c.OrderNo, (p, c) => new { p, c })
                      .Where(i => i.p.Status == 0 && i.c.CartStatus == 6 && i.c.ShopCode == shopcode)
                      .GroupBy(i => i.p.OrderNo)
                      .Select(i => new CartReportViewModel.CartReportList
                      {
                          Code = i.Any() ? i.FirstOrDefault().c.Code : "",
                          ShopName = i.Any() ? i.FirstOrDefault().p.ShopName : "",
                          OrderNo = i.Any() ? i.FirstOrDefault().p.OrderNo : "",
                          CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 0,
                          DateEncoded = i.Any() ? i.FirstOrDefault().p.DateEncoded : DateTime.Now,
                          OriginalAmount = i.Any() ? i.FirstOrDefault().p.OriginalAmount : 0.0,
                          ShopPaymentStatus = i.Any() ? i.FirstOrDefault().c.ShopPaymentStatus : 0
                      }).OrderByDescending(i => i.DateEncoded).ToList();
                    model.ShopCode = shopcode;
                    model.ShopName = shop.Name;
                }
            }
            else
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.List = db.Payments.Join(db.Carts, p => p.OrderNo, c => c.OrderNo, (p, c) => new { p, c })
                    .Where(i => i.p.DateEncoded >= startDatetFilter && i.p.DateEncoded <= endDateFilter && i.p.Status == 0 && i.c.CartStatus == 6)
                     .GroupBy(i => i.p.OrderNo)
                     .Select(i => new CartReportViewModel.CartReportList
                     {
                         Code = i.Any() ? i.FirstOrDefault().c.Code : "",
                         ShopName = i.Any() ? i.FirstOrDefault().p.ShopName : "",
                         OrderNo = i.Any() ? i.FirstOrDefault().p.OrderNo : "",
                         CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 0,
                         DateEncoded = i.Any() ? i.FirstOrDefault().p.DateEncoded : DateTime.Now,
                         OriginalAmount = i.Any() ? i.FirstOrDefault().p.OriginalAmount : 0.0,
                         ShopPaymentStatus = i.Any() ? i.FirstOrDefault().c.ShopPaymentStatus : 0
                     }).OrderByDescending(i => i.DateEncoded).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    model.List = db.Payments.Join(db.Carts, p => p.OrderNo, c => c.OrderNo, (p, c) => new { p, c })
                       .Where(i => i.p.Status == 0 && i.c.CartStatus == 6)
                       .GroupBy(i => i.p.OrderNo)
                       .Select(i => new CartReportViewModel.CartReportList
                       {
                           Code = i.Any() ? i.FirstOrDefault().c.Code : "",
                           ShopName = i.Any() ? i.FirstOrDefault().p.ShopName : "",
                           OrderNo = i.Any() ? i.FirstOrDefault().p.OrderNo : "",
                           CartStatus = i.Any() ? i.FirstOrDefault().c.CartStatus : 0,
                           DateEncoded = i.Any() ? i.FirstOrDefault().p.DateEncoded : DateTime.Now,
                           OriginalAmount = i.Any() ? i.FirstOrDefault().p.OriginalAmount : 0.0,
                           ShopPaymentStatus = i.Any() ? i.FirstOrDefault().c.ShopPaymentStatus : 0
                       }).OrderByDescending(i => i.DateEncoded).ToList();
                }
            }
            return View(model);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult DeliveryBoy_ShopNowChatReport(DateTime? StartDate, DateTime? EndDate, string deliveryboycode = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartReportViewModel();

            if (deliveryboycode != "")
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.List = db.Payments.Join(db.Carts, p => p.OrderNo, c => c.OrderNo, (p, c) => new { p, c })
                    .Where(i => i.p.DateEncoded >= startDatetFilter && i.p.DateEncoded <= endDateFilter && i.p.Status == 0
                    && i.c.DeliveryBoyCode == deliveryboycode && i.c.CartStatus == 6 && i.p.PaymentMode == "Cash On Hand")
                     .GroupBy(i => i.c.OrderNo)
                     .Select(i => new CartReportViewModel.CartReportList
                     {
                         Code = i.Any() ? i.FirstOrDefault().c.Code : "",
                         OrderNo = i.Any() ? i.FirstOrDefault().p.OrderNo : "",
                         DeliveryBoyPhoneNumber =i.Any()? i.FirstOrDefault().c.DeliveryBoyPhoneNumber : "",
                         DeliveryBoyCode = i.Any() ? i.FirstOrDefault().c.DeliveryBoyCode : "",
                         DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "",
                         Amount = i.Any() ? ((i.FirstOrDefault().p.Amount) - ((i.FirstOrDefault().p.refundAmount) ?? 0)) : 0.0,
                         //Amount = i.Any() ? (i.FirstOrDefault().p.Amount - (i.FirstOrDefault().p.refundAmount)?? 0) : 0.0,
                         DateUpdated = i.Any() ? i.FirstOrDefault().c.DateUpdated : DateTime.Now,
                         DeliveryOrderPaymentStatus = i.Any() ? i.FirstOrDefault().c.DeliveryOrderPaymentStatus : 0
                     }).OrderByDescending(i => i.DateUpdated).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    model.List = db.Payments.Join(db.Carts, p => p.OrderNo, c => c.OrderNo, (p, c) => new { p, c })
                      .Where(i => i.p.Status == 0 && i.c.DeliveryBoyCode == deliveryboycode && i.c.CartStatus == 6 && i.p.PaymentMode == "Cash On Hand")
                      .GroupBy(i => i.c.OrderNo)
                     .Select(i => new CartReportViewModel.CartReportList
                     {
                         Code = i.Any() ? i.FirstOrDefault().c.Code : "",
                         OrderNo = i.Any() ? i.FirstOrDefault().p.OrderNo : "",
                         DeliveryBoyPhoneNumber = i.Any() ? i.FirstOrDefault().c.DeliveryBoyPhoneNumber : "",
                         DeliveryBoyCode = i.Any() ? i.FirstOrDefault().c.DeliveryBoyCode : "",
                         DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "",
                         Amount = i.Any() ? ((i.FirstOrDefault().p.Amount) - ((i.FirstOrDefault().p.refundAmount) ?? 0)) : 0.0,
                         DateUpdated = i.Any() ? i.FirstOrDefault().c.DateUpdated : DateTime.Now,
                         DeliveryOrderPaymentStatus = i.Any() ? i.FirstOrDefault().c.DeliveryOrderPaymentStatus : 0
                     }).OrderByDescending(i => i.DateUpdated).ToList();
                }
            }
            else
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.List = db.Payments.Join(db.Carts, p => p.OrderNo, c => c.OrderNo, (p, c) => new { p, c })
                    .Where(i => i.p.DateEncoded >= startDatetFilter && i.p.DateEncoded <= endDateFilter && i.p.Status == 0
                    && i.c.CartStatus == 6 && i.p.PaymentMode == "Cash On Hand")
                    .GroupBy(i => i.c.OrderNo)
                     .Select(i => new CartReportViewModel.CartReportList
                     {
                         Code = i.Any() ? i.FirstOrDefault().c.Code : "",
                         OrderNo = i.Any() ? i.FirstOrDefault().p.OrderNo : "",
                         DeliveryBoyPhoneNumber = i.Any() ? i.FirstOrDefault().c.DeliveryBoyPhoneNumber : "",
                         DeliveryBoyCode = i.Any() ? i.FirstOrDefault().c.DeliveryBoyCode : "",
                         DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "",
                         Amount = i.Any() ? ((i.FirstOrDefault().p.Amount) - ((i.FirstOrDefault().p.refundAmount) ?? 0)) : 0.0,
                         DateUpdated = i.Any() ? i.FirstOrDefault().c.DateUpdated : DateTime.Now,
                         DeliveryOrderPaymentStatus = i.Any() ? i.FirstOrDefault().c.DeliveryOrderPaymentStatus : 0
                     }).OrderByDescending(i => i.DateUpdated).ToList();
                    model.StartingDate = StartDate.Value.ToString("yyyy/MM/dd");
                    model.EndingDate = EndDate.Value.ToString("yyyy/MM/dd");
                }
                else
                {
                    model.List = db.Payments.Join(db.Carts, p => p.OrderNo, c => c.OrderNo, (p, c) => new { p, c })
                       .Where(i => i.p.Status == 0 && i.c.CartStatus == 6 && i.p.PaymentMode == "Cash On Hand")
                      .GroupBy(i => i.c.OrderNo)
                     .Select(i => new CartReportViewModel.CartReportList
                     {
                         Code = i.Any() ? i.FirstOrDefault().c.Code : "",
                         OrderNo = i.Any() ? i.FirstOrDefault().p.OrderNo : "",
                         DeliveryBoyPhoneNumber = i.Any() ? i.FirstOrDefault().c.DeliveryBoyPhoneNumber : "",
                         DeliveryBoyCode = i.Any() ? i.FirstOrDefault().c.DeliveryBoyCode : "",
                         DeliveryBoyName = i.Any() ? i.FirstOrDefault().c.DeliveryBoyName : "",
                         Amount = i.Any() ? ((i.FirstOrDefault().p.Amount) - ((i.FirstOrDefault().p.refundAmount) ?? 0)) : 0.0,
                         DateUpdated = i.Any() ? i.FirstOrDefault().c.DateUpdated : DateTime.Now,
                         DeliveryOrderPaymentStatus = i.Any() ? i.FirstOrDefault().c.DeliveryOrderPaymentStatus : 0
                     }).OrderByDescending(i => i.DateUpdated).ToList();
                }
            }
            return View(model);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult ShopNowChat_DeliveryBoyReport(DateTime? StartDate, DateTime? EndDate, string deliveryboycode = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CartReportViewModel();

            if (deliveryboycode != "")
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.List = db.Carts.Join(db.ShopCharges, c => c.OrderNo, sc => sc.OrderNo, (c, sc) => new { c, sc })
                         .Join(db.Payments, cp => cp.c.OrderNo, p => p.OrderNo, (cp, p) => new { cp, p })
                  .Where(i => i.cp.c.DateEncoded >= startDatetFilter && i.cp.c.DateEncoded <= endDateFilter && i.cp.c.CartStatus == 6 && i.cp.c.Status == 0 && i.cp.sc.DeliveryBoyCode == deliveryboycode)
                        .GroupBy(i => i.cp.c.OrderNo).AsEnumerable()
                        .Select(i => new CartReportViewModel.CartReportList
                        {
                            Code = i.Any() ? i.FirstOrDefault().cp.sc.Code : "",
                            DateEncoded = i.Any() ? i.FirstOrDefault().cp.c.DateEncoded : DateTime.Now,
                            OrderNo = i.Any() ? i.FirstOrDefault().cp.c.OrderNo : "",
                            DeliveryBoyCode = i.Any() ? i.FirstOrDefault().cp.sc.DeliveryBoyCode : "",
                            DeliveryBoyName = i.Any() ? i.FirstOrDefault().cp.sc.DeliveryBoyName : "",
                            DeliveryBoyPhoneNumber = i.Any() ? i.FirstOrDefault().cp.c.DeliveryBoyPhoneNumber : "",
                            GrossDeliveryCharge = i.Any() ? i.Sum(j => j.cp.sc.GrossDeliveryCharge) : 0.0,
                            DeliveryBoyPaymentStatus = i.Any() ? i.FirstOrDefault().cp.c.DeliveryBoyPaymentStatus : 0,
                            DeliveryRateSet = i.Any() ? i.FirstOrDefault().p.CreditType : 0,
                            Kilometer = getDeliveryBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode) != null ? (((Math.Acos(Math.Sin((i.FirstOrDefault().cp.c.Latitude * Math.PI / 180))
                            * Math.Sin((getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Latitude * Math.PI / 180))
                               + Math.Cos((i.FirstOrDefault().cp.c.Latitude * Math.PI / 180)) * Math.Cos((getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Latitude * Math.PI / 180))
                              * Math.Cos(((i.FirstOrDefault().cp.c.Longitude - getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Longitude)
                              * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344) : 0
                        }).OrderBy(i => i.DeliveryBoyName).ToList();
                }
                else
                {
                    model.List = db.Carts.Join(db.ShopCharges, c => c.OrderNo, sc => sc.OrderNo, (c, sc) => new { c, sc })
                         .Join(db.Payments, cp => cp.c.OrderNo, p => p.OrderNo, (cp, p) => new { cp, p })
                  .Where(i => i.cp.c.CartStatus == 6 && i.cp.c.Status == 0 && i.cp.sc.DeliveryBoyCode == deliveryboycode)
                        .GroupBy(i => i.cp.c.OrderNo).AsEnumerable()
                        .Select(i => new CartReportViewModel.CartReportList
                        {
                            Code = i.Any() ? i.FirstOrDefault().cp.sc.Code : "",
                            DateEncoded = i.Any() ? i.FirstOrDefault().cp.c.DateEncoded : DateTime.Now,
                            OrderNo = i.Any() ? i.FirstOrDefault().cp.c.OrderNo : "",
                            DeliveryBoyCode = i.Any() ? i.FirstOrDefault().cp.sc.DeliveryBoyCode : "",
                            DeliveryBoyName = i.Any() ? i.FirstOrDefault().cp.sc.DeliveryBoyName : "",
                            DeliveryBoyPhoneNumber = i.Any() ? i.FirstOrDefault().cp.c.DeliveryBoyPhoneNumber : "",
                            GrossDeliveryCharge = i.Any() ? i.Sum(j => j.cp.sc.GrossDeliveryCharge) : 0.0,
                            DeliveryBoyPaymentStatus = i.Any() ? i.FirstOrDefault().cp.c.DeliveryBoyPaymentStatus : 0,
                            DeliveryRateSet = i.Any() ? i.FirstOrDefault().p.CreditType : 0,
                            Kilometer = getDeliveryBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode) != null ? (((Math.Acos(Math.Sin((i.FirstOrDefault().cp.c.Latitude * Math.PI / 180))
                            * Math.Sin((getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Latitude * Math.PI / 180))
                               + Math.Cos((i.FirstOrDefault().cp.c.Latitude * Math.PI / 180)) * Math.Cos((getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Latitude * Math.PI / 180))
                              * Math.Cos(((i.FirstOrDefault().cp.c.Longitude - getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Longitude)
                              * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344) : 0
                        }).OrderBy(i => i.DeliveryBoyName).ToList();
                }
            }
            else
            {
                if (StartDate != null && EndDate != null)
                {
                    DateTime startDatetFilter = new DateTime(StartDate.Value.Year, StartDate.Value.Month, StartDate.Value.Day);
                    DateTime endDateFilter = new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day).AddDays(1);

                    model.List = db.Carts.Join(db.ShopCharges, c => c.OrderNo, sc => sc.OrderNo, (c, sc) => new { c, sc })
                        .Join(db.Payments, cp => cp.c.OrderNo, p => p.OrderNo, (cp, p) => new { cp, p })
                 .Where(i => i.cp.c.DateEncoded >= startDatetFilter && i.cp.c.DateEncoded <= endDateFilter && i.cp.c.CartStatus == 6 && i.cp.c.Status == 0)
                       .GroupBy(i => i.cp.c.OrderNo).AsEnumerable()
                       .Select(i => new CartReportViewModel.CartReportList
                       {
                           Code = i.Any() ? i.FirstOrDefault().cp.sc.Code : "",
                           DateEncoded = i.Any() ? i.FirstOrDefault().cp.c.DateEncoded : DateTime.Now,
                           OrderNo = i.Any() ? i.FirstOrDefault().cp.c.OrderNo : "",
                           DeliveryBoyCode = i.Any() ? i.FirstOrDefault().cp.sc.DeliveryBoyCode : "",
                           DeliveryBoyName = i.Any() ? i.FirstOrDefault().cp.sc.DeliveryBoyName : "",
                           DeliveryBoyPhoneNumber = i.Any() ? i.FirstOrDefault().cp.c.DeliveryBoyPhoneNumber : "",
                           GrossDeliveryCharge = i.Any() ? i.Sum(j => j.cp.sc.GrossDeliveryCharge) : 0.0,
                           DeliveryBoyPaymentStatus = i.Any() ? i.FirstOrDefault().cp.c.DeliveryBoyPaymentStatus : 0,
                           DeliveryRateSet = i.Any() ? i.FirstOrDefault().p.CreditType : 0,
                           Kilometer = getDeliveryBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode) != null ? (((Math.Acos(Math.Sin((i.FirstOrDefault().cp.c.Latitude * Math.PI / 180))
                            * Math.Sin((getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Latitude * Math.PI / 180))
                               + Math.Cos((i.FirstOrDefault().cp.c.Latitude * Math.PI / 180)) * Math.Cos((getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Latitude * Math.PI / 180))
                              * Math.Cos(((i.FirstOrDefault().cp.c.Longitude - getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Longitude)
                              * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344) : 0
                       }).OrderBy(i => i.DeliveryBoyName).ToList();
                }
                else
                {
                    model.List = db.Carts.Join(db.ShopCharges, c => c.OrderNo, sc => sc.OrderNo, (c, sc) => new { c, sc })
                        .Join(db.Payments, cp=> cp.c.OrderNo, p=> p.OrderNo, (cp,p)=> new { cp,p})
                  .Where(i => i.cp.c.CartStatus == 6 && i.cp.c.Status == 0)
                        .GroupBy(i => i.cp.c.OrderNo)
                         .AsEnumerable()
                        .Select(i => new CartReportViewModel.CartReportList
                        {
                            Code = i.Any() ? i.FirstOrDefault().cp.sc.Code : "",
                            DateEncoded = i.Any() ? i.FirstOrDefault().cp.c.DateEncoded : DateTime.Now,
                            OrderNo = i.Any() ? i.FirstOrDefault().cp.c.OrderNo : "",
                            DeliveryBoyCode = i.Any() ? i.FirstOrDefault().cp.sc.DeliveryBoyCode : "",
                            DeliveryBoyName = i.Any() ? i.FirstOrDefault().cp.sc.DeliveryBoyName : "",
                            DeliveryBoyPhoneNumber = i.Any() ? i.FirstOrDefault().cp.c.DeliveryBoyPhoneNumber : "",
                            GrossDeliveryCharge = i.Any() ? i.Sum(j => j.cp.sc.GrossDeliveryCharge) : 0.0,
                            DeliveryBoyPaymentStatus = i.Any() ? i.FirstOrDefault().cp.c.DeliveryBoyPaymentStatus : 0,
                            DeliveryRateSet = i.Any() ? i.FirstOrDefault().p.CreditType : 0,
                            Kilometer = getDeliveryBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode) != null?(((Math.Acos(Math.Sin((i.FirstOrDefault().cp.c.Latitude * Math.PI / 180)) 
                            * Math.Sin((getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Latitude * Math.PI / 180))
                               + Math.Cos((i.FirstOrDefault().cp.c.Latitude * Math.PI / 180)) * Math.Cos((getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Latitude * Math.PI / 180))
                              * Math.Cos(((i.FirstOrDefault().cp.c.Longitude - getDBoy(i.FirstOrDefault().cp.c.DeliveryBoyCode).Longitude)
                              * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344): 0
                        }).OrderBy(i => i.DeliveryBoyName).ToList();
                }
            }
            return View(model);
        }
       DeliveryBoy getDBoy(string dbcode)
        {
            var deliveryBoy = db.DeliveryBoys.Where(d => d.Code == dbcode).FirstOrDefault();
            return deliveryBoy;
        }
        public string getDeliveryBoy(string dbcode)
        {
            var deliveryBoy = db.DeliveryBoys.Where(d => d.Code == dbcode).FirstOrDefault();
            return deliveryBoy.Code;
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
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetDeliveryBoySelect2(string q = "")
        {
            var model = await db.DeliveryBoys.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNCARAC011")]
        public JsonResult Accept(string orderNo, string customerCode)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            if (orderNo != null || orderNo != "" && customerCode != null || customerCode != "")
            {
                var cartList = db.Carts.Where(i => i.OrderNo == orderNo).ToList();

                var topup = db.TopUps.OrderByDescending(q => q.Id).FirstOrDefault(i => i.CustomerCode == customerCode && i.CreditType == 0 && i.Status == 0);//TopUp.GetCustomerPlatform(customerCode);
                if (topup != null)
                {
                    var list = db.PlatFormCreditRates.Where(i => i.Status == 0).ToList();//PlatFormCreditRate.GetList();
                    topup.CreditAmount = topup.CreditAmount - list.FirstOrDefault().RatePerOrder;
                    topup.DateUpdated = DateTime.Now;
                    db.Entry(topup).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    //TopUp.Edit(topup, out int errors);
                }

                var detail = db.ShopCharges.FirstOrDefault(i => i.OrderNo == orderNo); //ShopCharge.GetOrderNo(orderNo);
                detail.CartStatus = 3;
                // ShopCharge.Edit(detail, out int error);
                detail.DateUpdated = DateTime.Now;
                db.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                var customer = db.Customers.FirstOrDefault(i => i.Code == customerCode);//Customer.Get(customerCode);
                foreach (var c in cartList)
                {
                    var cart = GetCart(c.Code); // db.Carts.FirstOrDefault(i => i.Code == c.Code);
                    cart.CartStatus = 3;
                    cart.UpdatedBy = user.Name;
                    cart.DateUpdated = DateTime.Now;
                    db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
                return Json(new { message = "Order Confirmed!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "Failed to Confirm the Order!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [AccessPolicy(PageCode = "SHNCARR012")]
        public JsonResult Cancel(string orderNo, string customerCode, int? status)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            if (orderNo != null || orderNo != "" && customerCode != null || customerCode != "" && status != 0)
            {
                var customer = db.Customers.FirstOrDefault(i => i.Code == customerCode);
                var cartList = db.Carts.Where(i => i.OrderNo == orderNo).ToList();
                foreach (var c in cartList)
                {
                    var cart = GetCart(c.Code);// db.Carts.FirstOrDefault(i => i.Code == c.Code);
                    cart.CartStatus = 7;
                    cart.UpdatedBy = user.Name;
                    cart.DateUpdated = DateTime.Now;
                    db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    //Product Stock Update
                    var product = db.Products.FirstOrDefault(i => i.Code == c.ProductCode);
                    //product.Qty += Convert.ToInt32(c.Qty);
                    product.HoldOnStok -= Convert.ToInt32(c.Qty);
                    db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    //Refund
                    var payment = db.Payments.FirstOrDefault(i => i.OrderNo == c.OrderNo);
                    //if (payment.PaymentMode == "Online Payment")
                    //{
                        payment.refundAmount = payment.Amount;
                        payment.refundRemark = "Your order has been cancelled by shop.";
                        payment.UpdatedBy = customer.Name;
                        payment.DateUpdated = DateTime.Now;
                        db.Entry(payment).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    //}

                    // update Shopcharge cartstatus 7
                    var sc = db.ShopCharges.FirstOrDefault(i => i.OrderNo == orderNo); //ShopCharge.GetOrderNo(orderNo);
                    sc.CartStatus = 7;
                    sc.DateUpdated = DateTime.Now;
                    db.Entry(sc).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return Json(new { message = "Order Cancelled!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { message = "Failed to Cancel the Order!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult ShopPay(string orderNo)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var cartList = db.Carts.Where(i => i.OrderNo == orderNo && i.Status == 0 && i.ShopPaymentStatus == 0).ToList();
            foreach (var c in cartList)
            {
                var cart = GetCart(c.Code);// db.Carts.FirstOrDefault(i => i.Code == c.Code);
                cart.ShopPaymentStatus = 1;
                db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult ShopNowChatPay(string orderno)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var cartList = db.Carts.Where(i => i.OrderNo == orderno && i.Status == 0 && i.DeliveryOrderPaymentStatus == 0).ToList();
            foreach(var c in cartList)
            {
                var cart = GetCart(c.Code);// db.Carts.FirstOrDefault(i=> i.Code == c.Code);
                cart.DeliveryOrderPaymentStatus = 1;
                db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult DeliveryBoyPay(string orderno)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var cartList = db.Carts.Where(i => i.OrderNo == orderno && i.Status == 0 && i.DeliveryBoyPaymentStatus == 0).ToList();
            foreach (var c in cartList)
            {
                var cart = GetCart(c.Code);// db.Carts.FirstOrDefault(i => i.Code == c.Code);
                cart.DeliveryBoyPaymentStatus = 1;
                db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult DeliveryBoyReject(string orderNo)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            if (orderNo != null || orderNo != "")
            {
                var cartList = db.Carts.Where(i => i.OrderNo == orderNo).ToList();
                foreach (var c in cartList)
                {
                    var cart = GetCart(c.Code);// db.Carts.FirstOrDefault(i => i.Code == c.Code);
                    cart.DeliveryBoyCode = null;
                    cart.DeliveryBoyName = null;
                    cart.DeliveryBoyPhoneNumber = null;
                    cart.CartStatus = 3;
                    db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    var dboy = db.DeliveryBoys.FirstOrDefault(i => i.Code == cart.DeliveryBoyCode);// DeliveryBoy.Get(cart.DeliveryBoyCode);
                    dboy.isAssign = 0;
                    dboy.OnWork = 0;
                    dboy.DateUpdated = DateTime.Now;
                    db.Entry(dboy).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    // DeliveryBoy.Edit(dboy, out int error);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

         Cart GetCart(string code)
        {
            try
            {

                return db.Carts.Where(i => i.Code == code).FirstOrDefault();
            }
            catch
            {
                return (Cart)null;
            }
        }

        public ActionResult UnAssignDeliveryBoy(string OrderNo)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
           
            var cartList = db.Carts.Where(i => i.OrderNo == OrderNo).ToList();
            var getDeliveryBoyFromCart = cartList.FirstOrDefault();
            var deliveryboy = db.DeliveryBoys.FirstOrDefault(i => i.Code == getDeliveryBoyFromCart.DeliveryBoyCode);
            deliveryboy.isAssign = 0;
            deliveryboy.DateUpdated = DateTime.Now;
            db.Entry(deliveryboy).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            if (cartList != null)
            {
                foreach (var c in cartList)
                {
                    var carts = GetCart(c.Code);
                    carts.DeliveryBoyCode = string.Empty;
                    carts.DeliveryBoyName = string.Empty;
                    carts.DeliveryBoyPhoneNumber = string.Empty;
                    carts.CartStatus = 3;
                    carts.DateUpdated = DateTime.Now;
                    db.Entry(carts).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                   
                }
            }
            return RedirectToAction("DeliveryAgentAssigned");
        }

        public ActionResult AddRefundFromShopOrderProcessing(string code,double amount,string remark)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var cart = db.Carts.FirstOrDefault(i => i.Code == code);
            //Refund
            var payment = db.Payments.FirstOrDefault(i => i.OrderNo == cart.OrderNo);
            //if (payment.PaymentMode == "Online Payment")
            //{
                payment.refundAmount = amount;
                payment.refundRemark = remark;
                payment.UpdatedBy = user.Name;
                payment.DateUpdated = DateTime.Now;
                db.Entry(payment).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            //}

            return RedirectToAction("OrderPrepared");
        }

        public ActionResult DeliveryBoyAccept(string orderNo, string code)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var cart = db.Carts.FirstOrDefault(i => i.Code == code);
            var delivaryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Code == cart.DeliveryBoyCode && i.Status == 0);
            delivaryBoy.OnWork = 1;
            delivaryBoy.UpdatedBy = user.Name;
            delivaryBoy.DateUpdated = DateTime.Now;
            db.Entry(delivaryBoy).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            var shopCharge = db.ShopCharges.FirstOrDefault(i => i.OrderNo == orderNo);
            var shop = db.Shops.FirstOrDefault(i => i.Code == shopCharge.ShopCode);
            var topup = db.TopUps.OrderByDescending(q => q.Id).FirstOrDefault(i => i.CustomerCode == shop.CustomerCode && i.CreditType == 1 && i.Status == 0);
            if (topup != null)
            {
                topup.CreditAmount = topup.CreditAmount - shopCharge.GrossDeliveryCharge;
                topup.DateUpdated = DateTime.Now;
                db.Entry(topup).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Edit", "Cart", new { orderno = orderNo, code = code });
        }

        public ActionResult DeliveryBoyPickup(string orderNo, string code)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var cartList = db.Carts.Where(i => i.OrderNo == orderNo).ToList();
            foreach (var c in cartList)
            {
                var cart = db.Carts.FirstOrDefault(i => i.Code == c.Code);
                cart.CartStatus = 5;
                cart.UpdatedBy = user.Name;
                cart.DateUpdated = DateTime.Now;
                db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //Product Stock Update
                var product = db.Products.FirstOrDefault(i => i.Code == c.ProductCode);
                product.HoldOnStok -= Convert.ToInt32(c.Qty);
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            var detail = db.ShopCharges.FirstOrDefault(i => i.OrderNo == orderNo);
            detail.CartStatus = 5;
            detail.DateUpdated = DateTime.Now;
            db.Entry(detail).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            var payment = db.Payments.FirstOrDefault(i => i.OrderNo == orderNo);
            if (payment.PaymentMode == "Online Payment" && payment.Amount > 1000)
            {
                //var otpmodel = new OtpViewModel();
                //var models = _mapper.Map<OtpViewModel, OtpVerification>(otpmodel);
                var otpVerification = new OtpVerification();
                otpVerification.Code = Helpers.DRC.Generate("SMS");
                otpVerification.ShopCode = cartList[0].ShopCode;
                otpVerification.CustomerCode = user.Code;
                otpVerification.CustomerName = user.Name;
                otpVerification.PhoneNumber = cartList[0].DeliveryBoyPhoneNumber;
                otpVerification.Otp = Helpers.DRC.GenerateOTP();
                otpVerification.ReferenceCode = Helpers.DRC.Generate("");
                otpVerification.Verify = false;
                otpVerification.OrderNo = orderNo;
                otpVerification.CreatedBy = user.Name;
                otpVerification.UpdatedBy = user.Name;
                otpVerification.DateUpdated = DateTime.Now;
                db.OtpVerifications.Add(otpVerification);
                db.SaveChanges();
            }
            return RedirectToAction("Edit", "Cart", new { orderno = orderNo, code = code });
        }

        public ActionResult MarkAsDelivered(string orderNo, string code)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);

            var otpVerify = db.OtpVerifications.FirstOrDefault(i => i.OrderNo == orderNo);
            var cart = db.Carts.FirstOrDefault(i => i.Code == code);

            var delivaryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Code == cart.DeliveryBoyCode && i.Status == 0);// DeliveryBoy.GetCustomer(customerCode);
            delivaryBoy.OnWork = 0;
            delivaryBoy.isAssign = 0;
            delivaryBoy.DateUpdated = DateTime.Now;
            db.Entry(delivaryBoy).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            otpVerify.Verify = true;
            otpVerify.UpdatedBy = user.Name;
            otpVerify.DateUpdated = DateTime.Now;
            db.Entry(otpVerify).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            var cartList = db.Carts.Where(i => i.OrderNo == orderNo).ToList();
            foreach (var c in cartList)
            {
                var cartItem = db.Carts.FirstOrDefault(i => i.Code == c.Code);
                cartItem.CartStatus = 6;
                cartItem.UpdatedBy = delivaryBoy.CustomerName;
                cartItem.DateUpdated = DateTime.Now;
                db.Entry(cartItem).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            var detail = db.ShopCharges.FirstOrDefault(i => i.OrderNo == orderNo);
            detail.CartStatus = 6;
            detail.DateUpdated = DateTime.Now;
            db.Entry(detail).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Edit", "Cart", new { orderno = orderNo, code = code });
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

