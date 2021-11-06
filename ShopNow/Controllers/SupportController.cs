using AutoMapper;
using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
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