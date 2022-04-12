using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class ServiceController : Controller
    {
        private sncEntities db = new sncEntities();

        [AccessPolicy(PageCode = "SNCSVL334")]
        public ActionResult List(ServiceListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Orders.Where(i => i.IsPickupDrop == true && (model.Status != 0 ? i.Status == model.Status : true) && (model.ShopId != 0 ? i.ShopId == model.ShopId : true))
                .Select(i => new ServiceListViewModel.ListItem
                {
                    Amount = i.TotalPrice,
                    DateEncoded = i.DateEncoded,
                    DeliveryAddress = i.DeliveryAddress,
                    DeliveryCharge = i.DeliveryCharge,
                    Distance = i.Distance + "Kms",
                    Name = i.CustomerName,
                    PhoneNumber = i.CustomerPhoneNumber,
                    PickupAddress = i.PickupAddress,
                    Remarks = i.Remarks,
                    Status = i.Status,
                    OrderNumber = i.OrderNumber,
                    ShopName = i.ShopName
                }).OrderByDescending(i=> i.DateEncoded).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCSVC335")]
        public ActionResult Create()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SNCSVC335")]
        public ActionResult Create(ServiceCreateViewModel model)
        {
            if (!db.Orders.Any(i => i.OrderNumber == model.OrderNumber))
            {
                var user = ((Helpers.Sessions.User)Session["USER"]);
                
                var shop = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
                var order = new Order
                {
                    OrderNumber = model.OrderNumber,
                    CustomerId = 0,
                    CustomerName = model.Name,
                    CustomerPhoneNumber = model.PhoneNumber,
                    ShopId = model.ShopId,
                    ShopName = shop.Name,
                    DeliveryAddress = model.DeliveryAddress,
                    ShopPhoneNumber = shop.PhoneNumber,
                    ShopOwnerPhoneNumber = shop.OwnerPhoneNumber,
                    TotalProduct = 0,
                    TotalQuantity = 0,
                    TotalPrice = model.Amount,
                    WalletAmount = 0,
                    NetTotal = model.Amount + model.DeliveryCharge,
                    DeliveryCharge = model.DeliveryCharge,
                    ShopDeliveryDiscount = 0,
                    NetDeliveryCharge = model.DeliveryCharge,
                    Latitude = model.DeliveryLatitude,
                    Longitude = model.DeliveryLongitude,
                    Distance = model.Distance,
                    Remarks = model.Remarks,
                    PaymentMode = "Cash On Hand",
                    PaymentModeType = 2,
                    TipsAmount = 0,
                    IsPickupDrop = true,
                    Status = 2,
                    DateEncoded = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    CreatedBy = user.Name,
                    UpdatedBy = user.Name,
                    PickupAddress = shop.Address,
                    PickupLatitude = shop.Latitude,
                    PickupLongitude = shop.Longitude,
                    PickupDateTime = model.PickupDateTime,
                    DeliveryDate = model.DeliveryDate,
                    DeliverySlotType = model.DeliverySlotType
                };
                var deliveryRatePercentage = db.DeliveryRatePercentages.OrderByDescending(i => i.Id).FirstOrDefault(i => i.Status == 0);
                if (deliveryRatePercentage != null)
                {
                    order.DeliveryRatePercentage = deliveryRatePercentage.Percentage;
                    order.DeliveryRatePercentageId = deliveryRatePercentage.Id;
                }
                //order.DeliveryChargeUpto5Km = (shop.DeliveryTierType == 1 && shop.DeliveryType == 0) ? 50 : 40;
                //order.DeliveryChargePerKm = (shop.DeliveryTierType == 1 && shop.DeliveryType == 0) ? 6 : 8;
                //order.DeliveryChargeRemarks = db.PincodeRates.FirstOrDefault(i => i.Id == shop.PincodeRateId && i.Status == 0)?.Remarks;
                db.Orders.Add(order);
                db.SaveChanges();

                //Create one OrderItem to get the OrderId in App for UpdateTimings
                var orderItem = new OrderItem
                {
                    AddOnType = 0,
                    BrandId = 0,
                    CategoryId = 0,
                    HasAddon = false,
                    OrdeNumber = order.OrderNumber,
                    OrderId = order.Id,
                    ProductId = 0,
                    ProductName = "Parcel Drop Service",
                    UpdatedTime = DateTime.Now,
                    Price = order.TotalPrice,
                    UnitPrice = order.TotalPrice,
                    Quantity = 1
                };
                db.OrderItems.Add(orderItem);
                db.SaveChanges();

                // Payment
                var payment = new Payment();
                payment.Amount = order.NetTotal;
                payment.PaymentMode = "Cash On Hand";
                payment.PaymentModeType = 2;
                payment.CustomerId = order.CustomerId;
                payment.CustomerName = order.CustomerName;
                payment.ShopId = shop.Id;
                payment.ShopName = shop.Name;
                payment.OriginalAmount = order.TotalPrice;
                payment.GSTAmount = order.NetTotal;
                payment.Currency = "Rupees";
                payment.CountryName = null;
                payment.PaymentResult = "pending";
                payment.Credits = "N/A";
                payment.OrderNumber = order.OrderNumber;
                payment.PaymentCategoryType = 0;
                payment.Credits = "N/A";
                payment.CreditType = 2;
                payment.ConvenientCharge = 0;
                payment.PackingCharge = 0;
                payment.DeliveryCharge = order.NetDeliveryCharge;
                payment.RatePerOrder = order.RatePerOrder;
                payment.RefundStatus = 1;
                payment.Status = 0;
                payment.CreatedBy = user.Name;
                payment.UpdatedBy = user.Name;
                payment.DateEncoded = DateTime.Now;
                payment.DateUpdated = DateTime.Now;
                db.Payments.Add(payment);
                db.SaveChanges();

                //To Add Shop credit
                var shopCredits = db.ShopCredits.FirstOrDefault(i => i.CustomerId == shop.CustomerId);
                if (shopCredits == null)
                {
                    var shopcredit = new ShopCredit
                    {
                        CustomerId = model.CustomerId,
                        DateUpdated = DateTime.Now,
                        DeliveryCredit = 0,
                        PlatformCredit = 0
                    };
                    db.ShopCredits.Add(shopcredit);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("List");
        }

        public JsonResult GetDeliveryCharge(int ShopId, double PickupLatitude, double PickupLongitude, double DeliveryLatitude, double DeliveryLongitude,double distance)
        {
            var shop = db.Shops.FirstOrDefault(i => i.Id == ShopId);
            var DeliveryCredit = db.ShopCredits.FirstOrDefault(i => i.CustomerId == shop.CustomerId).DeliveryCredit;
            double DeliveryCharge = 0;
            //var Distance = (((Math.Acos(Math.Sin((PickupLatitude * Math.PI / 180)) * Math.Sin(((DeliveryLatitude) * Math.PI / 180)) + Math.Cos((PickupLatitude * Math.PI / 180)) * Math.Cos((DeliveryLatitude * Math.PI / 180))
            //     * Math.Cos(((PickupLongitude - DeliveryLongitude) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344) / 1000;

            if (shop.ShopCategoryId == 1)
            {
                if (distance <= 5)
                {
                    DeliveryCharge = 50;
                }
                else if (distance > 5 && distance <= 15)
                {
                    double amount = (distance - 5) * 6;
                    DeliveryCharge = 50 + amount;
                }
                else
                { // above 15kms -12Rs
                    double amount = (10 * 6) + ((distance - 15) * 12);
                    DeliveryCharge = 50 + amount;
                }
            }
            else
            {
                if (distance <= 5)
                {
                    DeliveryCharge = 50;
                }
                else
                {
                    double amount = (distance - 5) * 6;
                    DeliveryCharge = 50 + amount;
                }
            }
            return Json(new { DeliveryCharge, distance, DeliveryCredit }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAddressCount(string phoneNumber = "", int shopId = 0)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                count = db.Orders.Where(a => a.CustomerPhoneNumber == phoneNumber && a.IsPickupDrop == true && a.ShopId == shopId).Distinct().Count();
            }
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && (a.Status == 0 || a.Status == 1)).Select(i => new
            {
                id = i.Id,
                text = i.Name,
                shopid = i.Id,
                address = i.Address,
                latitude = i.Latitude,
                longitude = i.Longitude,
                customerid = i.CustomerId,
                shopCategoryId = i.ShopCategoryId
            }).ToListAsync();
            
            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDeliveryAddressSelect2(string customerPhoneNumber = "", int shopId = 0, string q = "")
        {
            var model = await db.Orders
                .Where(a => a.DeliveryAddress.Contains(q) && a.CustomerPhoneNumber == customerPhoneNumber && a.IsPickupDrop == true && a.ShopId == shopId)
                .Select(i => new
                {
                    id = i.DeliveryAddress,
                    text = i.DeliveryAddress,
                    latitude = i.Latitude,
                    longitude = i.Longitude
                }).Distinct().OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocationDetails(double sourceLatitude, double sourceLongitude, double destinationLatitude, double destinationLongitude)
        {
            bool isAvailable = db.LocationDetails.Any(i => i.SourceLatitude == sourceLatitude && i.SourceLongitude == sourceLongitude && i.DestinationLatitude == destinationLatitude && i.DestinationLongitude == destinationLongitude);
            if (isAvailable)
            {
                var location = db.LocationDetails.FirstOrDefault(i => i.SourceLatitude == sourceLatitude && i.SourceLongitude == sourceLongitude && i.DestinationLatitude == destinationLatitude && i.DestinationLongitude == destinationLongitude);
                return Json(location, JsonRequestBehavior.AllowGet);
            }
            return Json(isAvailable, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveLocationDetails(LocationDetailsCreateViewModel model)
        {
            if (model.Distance > 0)
            {
                var locationDetails = new LocationDetail
                {
                    DateEncoded = DateTime.Now,
                    DestinationLatitude = model.DestinationLatitude,
                    DestinationLongitude = model.DestinationLontitude,
                    Distance = model.Distance,
                    Duration = model.Duration,
                    SourceLatitude = model.SourceLatitude,
                    SourceLongitude = model.SourceLontitude
                };
                db.LocationDetails.Add(locationDetails);
                db.SaveChanges();
            }
            return Json(new { message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
        }
    }
}