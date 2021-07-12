using AutoMapper;
using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ShopNowPay.Controllers
{
    public class SalesController : Controller
    {
        private ShopnowchatEntities db = new ShopnowchatEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;

        public SalesController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<Cart, SalesReportViewModel.SalesReportList>();
                config.CreateMap<Payment,SalesReportViewModel.SalesReportList>();

            });

            _mapper = _mapperConfiguration.CreateMapper();
        }
       
        [AccessPolicy(PageCode = "SHNSALR001")]
        public ActionResult Report(DateTime? startDate, DateTime? endDate)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new SalesReportViewModel();

            if (startDate != null && endDate != null)
            {
                DateTime startDatetFilter = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day);
                DateTime endDateFilter = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day).AddDays(1);

                model.List = db.Carts.Join(db.Payments, c => c.OrderNo, p => p.OrderNo, (c, p) => new { c, p })
                 .Where(i => i.c.DateEncoded >= startDatetFilter && i.c.DateEncoded <= endDateFilter && i.c.Status == 0 && i.c.CartStatus == 6)
                 .GroupBy(i => i.c.OrderNo)
                 .Select(i => new SalesReportViewModel.SalesReportList
                 {
                     Code = i.Any() ? i.FirstOrDefault().c.Code : "",
                     ShopCode = i.Any() ? i.FirstOrDefault().c.ShopCode : "",
                     ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "",
                     DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "",
                     DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now,
                     Amount = i.Any() ? i.FirstOrDefault().p.Amount : 0.0,
                     OrderNo = i.Any() ? i.FirstOrDefault().p.OrderNo : ""
                 }).OrderByDescending(i => i.DateEncoded).ToList();
            }
            else
            {
                model.List = db.Carts.Join(db.Payments, c => c.OrderNo, p => p.OrderNo, (c, p) => new { c, p })
                  .Where(i => i.c.Status == 0 && i.c.CartStatus == 6)
                  .GroupBy(i=> i.c.OrderNo)
                  .Select(i => new SalesReportViewModel.SalesReportList
                  {
                      Code = i.Any()?i.FirstOrDefault().c.Code:"",
                      ShopCode = i.Any() ? i.FirstOrDefault().c.ShopCode : "",
                      ShopName = i.Any() ? i.FirstOrDefault().c.ShopName : "",
                      DeliveryAddress = i.Any() ? i.FirstOrDefault().c.DeliveryAddress : "",
                      DateEncoded = i.Any() ? i.FirstOrDefault().c.DateEncoded : DateTime.Now,
                      Amount = i.Any() ? i.FirstOrDefault().p.Amount : 0.0,
                      OrderNo = i.Any() ? i.FirstOrDefault().p.OrderNo : ""
                  }).OrderByDescending(i => i.DateEncoded).ToList();
            }
            return View(model.List);
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