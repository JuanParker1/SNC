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
                config.CreateMap<Order, SalesReportViewModel.SalesReportList>();
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

                model.List = db.Orders
                    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                 .Where(i => i.c.DateEncoded >= startDatetFilter && i.c.DateEncoded <= endDateFilter && i.c.Status == 6)
                 .Select(i => new SalesReportViewModel.SalesReportList
                 {
                     Id = i.c.Id,
                     ShopId = i.c.ShopId,
                     ShopName = i.c.ShopName,
                     DeliveryAddress = i.c.DeliveryAddress,
                     DateEncoded = i.c.DateEncoded,
                     Amount = i.p.Amount,
                     OrderNo = i.p.OrderNumber
                 }).OrderByDescending(i => i.DateEncoded).ToList();
            }
            else
            {
                model.List = db.Orders
                    .Join(db.Payments, c => c.OrderNumber, p => p.OrderNumber, (c, p) => new { c, p })
                  .Where(i => i.c.Status == 6)
                  .Select(i => new SalesReportViewModel.SalesReportList
                  {
                      Id = i.c.Id,
                      ShopId = i.c.ShopId,
                      ShopName = i.c.ShopName,
                      DeliveryAddress = i.c.DeliveryAddress,
                      DateEncoded = i.c.DateEncoded,
                      Amount = i.p.Amount,
                      OrderNo = i.p.OrderNumber
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