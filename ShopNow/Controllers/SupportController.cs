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
        private ShopnowchatEntities db = new ShopnowchatEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        public SupportController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {

            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNSUPL001")]
        public ActionResult LivePending()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new SupportViewModel();

            model.ShopAcceptanceCount = db.Carts.Where(i => i.CartStatus == 2 && i.Status == 0 && SqlFunctions.DateDiff("minute", i.DateUpdated, DateTime.Now) >= 5)
                   .AsEnumerable().GroupBy(i => i.OrderNo).Count();
            model.DeliveryAcceptanceCount = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
                      .Where(i => i.c.CartStatus == 4 && i.c.Status == 0 && i.d.isAssign == 1 && i.d.OnWork == 0 && SqlFunctions.DateDiff("minute", i.c.DateUpdated, DateTime.Now) >= 5)
                      .AsEnumerable().GroupBy(i => i.c.OrderNo).Count();
            model.ShopPickupCount = db.Carts.Join(db.DeliveryBoys, c => c.DeliveryBoyCode, d => d.Code, (c, d) => new { c, d })
                    .Where(i => i.c.CartStatus == 4 && i.c.Status == 0 && i.d.isAssign == 1 && i.d.OnWork == 1 && SqlFunctions.DateDiff("minute", i.c.DateUpdated, DateTime.Now) >= 15)
                    .AsEnumerable().GroupBy(i => i.c.OrderNo).Count();
            model.CustomerDeliveryCount = db.Carts.Where(i => i.CartStatus == 5 && i.Status == 0 && SqlFunctions.DateDiff("minute", i.DateUpdated, DateTime.Now) >= 15)
                    .AsEnumerable().GroupBy(i => i.OrderNo).Count();
            model.OrderswithoutDeliveryboyCount = db.Carts.Where(i => i.CartStatus == 3 && i.Status == 0)
                    .AsEnumerable().GroupBy(i => i.OrderNo).Count();

            model.CustomerAadhaarVerifyCount = db.Customers.Where(i => i.AadharVerify == false && i.Status == 0 && i.ImageAadharPath != null && i.ImageAadharPath != "Rejected").Count();
            model.ShopOnBoardingVerifyCount = db.Shops.Where(i => i.Status == 1).Count();
            model.DeliveryBoyVerifyCount = db.DeliveryBoys.Where(i => i.Status == 1).Count();
            model.BannerPendingCount = db.Banners.Where(i => i.Status == 1).Count();
            model.ShopCount = db.Shops.Where(i => i.Status == 0).Count();
            model.CustomerCount = db.Customers.Where(i => i.Status == 0).Count();
            model.OrderCount = db.Carts.Where(i => i.Status == 0 && i.OrderNo !=null && i.CartStatus != 7 && i.CartStatus != 6 && i.CartStatus != 0).GroupBy(i=>i.OrderNo).Count();
            model.DeliveryBoyLiveCount = db.DeliveryBoys.Where(i => i.Status == 0 && i.isAssign == 0 && i.OnWork == 0 && i.Active == 1).Count();
            model.RefundCount = db.Payments.Where(i => i.refundAmount != 0 && i.refundStatus == 1 && i.refundAmount != null && i.PaymentMode == "Online Payment").Count();
            return View(model);
        }
 
        [AccessPolicy(PageCode = "SHNSUPLD002")]
        public ActionResult LiveDeliveryboyAssignment(string shopcode="")
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

            if (shopcode != "")
            {
                model.List = db.Carts.Where(i => i.CartStatus == 3 && i.Status == 0 && i.ShopCode == shopcode)
                   .GroupBy(i => i.OrderNo).Select(i => new DeliveryBoyAssignViewModel.AssignList
                   {
                       Code = i.FirstOrDefault().Code,
                       ShopCode = i.FirstOrDefault().ShopCode,
                       ShopName = i.FirstOrDefault().ShopName,
                       OrderNo = i.FirstOrDefault().OrderNo,
                       CartStatus = i.FirstOrDefault().CartStatus,
                       DateEncoded = i.FirstOrDefault().DateEncoded
                   }).ToList();
            }
            else
            {
                model.List = db.Carts.Where(i => i.CartStatus == 3 && i.Status == 0)
                   .GroupBy(i => i.OrderNo).Select(i => new DeliveryBoyAssignViewModel.AssignList
                   {
                       Code = i.FirstOrDefault().Code,
                       ShopCode = i.FirstOrDefault().ShopCode,
                       ShopName = i.FirstOrDefault().ShopName,
                       OrderNo = i.FirstOrDefault().OrderNo,
                       CartStatus = i.FirstOrDefault().CartStatus,
                       DateEncoded = i.FirstOrDefault().DateEncoded
                   }).ToList();
            }
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