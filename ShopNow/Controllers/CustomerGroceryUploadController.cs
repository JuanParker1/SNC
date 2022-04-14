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
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class CustomerGroceryUploadController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        public CustomerGroceryUploadController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<CustomerGroceryUpload, GroceryAddToCartViewModel>();
                config.CreateMap<GroceryAddToCartViewModel, Order>();
                config.CreateMap<GroceryAddToCartViewModel.ListItem, OrderItem>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SNCCGUL317")]
        public ActionResult List()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var list = db.CustomerGroceryUploads.OrderByDescending(i => i.Id).ToList();
            var model = new CustomerGroceryUploadListViewModel();
            model.ListItems = db.CustomerGroceryUploads.Where(i => i.Status == 0).OrderByDescending(i => i.Id)
                .GroupJoin(db.CustomerGroceryUploadImages, cg => cg.Id, cgi => cgi.CustomerGroceryUploadId, (cg, cgi) => new { cg, cgi })
                .Join(db.Shops, c => c.cg.ShopId, s => s.Id, (c, s) => new { c, s })
                .Select(i => new CustomerGroceryUploadListViewModel.ListItem
                {
                    Id = i.c.cg.Id,
                    AudioPath = (!string.IsNullOrEmpty(i.c.cg.AudioPath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Audio/" + i.c.cg.AudioPath : "",
                    CustomerId = i.c.cg.CustomerId,
                    CustomerName = i.c.cg.CustomerName,
                    CustomerPhoneNumber = i.c.cg.CustomerPhoneNumber,
                    ImagePath = i.c.cg.ImagePath,
                    Remarks = i.c.cg.Remarks,
                    ShopId = i.c.cg.ShopId,
                    ShopName = i.s.Name,
                    DateEncoded = i.c.cg.DateEncoded,
                    Status = i.c.cg.Status,
                    ImagePathLists = i.c.cgi.Select(a => new CustomerGroceryUploadListViewModel.ListItem.ImagePathList
                    {
                        ImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + a.ImagePath
                    }).ToList()
                }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCGUCL318")]
        public ActionResult CancelList()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var list = db.CustomerGroceryUploads.OrderByDescending(i => i.Id).ToList();
            var model = new CustomerGroceryUploadListViewModel();
            model.ListItems = db.CustomerGroceryUploads.Where(i => i.Status == 2).OrderByDescending(i => i.Id)
                .GroupJoin(db.CustomerGroceryUploadImages, cg => cg.Id, cgi => cgi.CustomerGroceryUploadId, (cg, cgi) => new { cg, cgi })
                .Select(i => new CustomerGroceryUploadListViewModel.ListItem
                {
                    Id = i.cg.Id,
                    AudioPath = (!string.IsNullOrEmpty(i.cg.AudioPath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Audio/" + i.cg.AudioPath : "",
                    CustomerId = i.cg.CustomerId,
                    CustomerName = i.cg.CustomerName,
                    CustomerPhoneNumber = i.cg.CustomerPhoneNumber,
                    ImagePath = i.cg.ImagePath,
                    Remarks = i.cg.Remarks,
                    ShopId = i.cg.ShopId,
                    DateEncoded = i.cg.DateEncoded,
                    Status = i.cg.Status,
                    ImagePathLists = i.cgi.Select(a => new CustomerGroceryUploadListViewModel.ListItem.ImagePathList
                    {
                        ImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + a.ImagePath
                    }).ToList()
                }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCGUGOL319")]
        public ActionResult GroceryOrderList()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new GroceryUploadOrderListViewModel();
            model.GroceryOrderLists = db.Orders.OrderByDescending(i => i.DateEncoded).Where(i => (i.UploadType == 2) && i.Status == 6)
                .AsEnumerable()
               .Select((i, index) => new GroceryUploadOrderListViewModel.GroceryOrderList
               {
                   No = index + 1,
                   Id = i.Id,
                   ShopName = i.ShopName,
                   OrderNumber = i.OrderNumber.ToString(),
                   CustomerPhoneNumber = i.CustomerPhoneNumber,
                   Status = i.Status,
                   DateEncoded = i.DateEncoded,
                   DateUpdated = i.DateUpdated,
                   Amount = i.NetTotal,
                   PaymentMode = i.PaymentMode
               }).ToList();
            return View(model.GroceryOrderLists);
        }

        [AccessPolicy(PageCode = "SNCCGUAC320")]
        public ActionResult AddToCart(int id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new GroceryAddToCartViewModel();
            var cg = db.CustomerGroceryUploads.FirstOrDefault(i => i.Id == id);
            if (cg != null)
            {
                _mapper.Map(cg, model);
                model.GroceryId = cg.Id;
                model.DeliveryAddress = cg.DeliveryAddress;
                model.Latitude = cg.Latitude;
                model.Longitude = cg.Longitude;
                model.ImagePathLists = db.CustomerGroceryUploadImages.Where(i => i.CustomerGroceryUploadId == cg.Id)
                        .Select(i => new GroceryAddToCartViewModel.ImagePathList
                        {
                            ImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath
                        }).ToList();
                model.AudioPath = (!string.IsNullOrEmpty(cg.AudioPath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Audio/" + cg.AudioPath : "";
                var shop = db.Shops.FirstOrDefault(i => i.Id == cg.ShopId);
                if (shop != null)
                {
                    model.ShopName = shop.Name;
                    model.ShopPhoneNumber = shop.PhoneNumber;
                    model.ShopImagePath = shop.ImagePath;
                    model.ShopAddress = shop.Address;
                    model.ShopLatitude = shop.Latitude;
                    model.ShopLongitude = shop.Longitude;
                }
                var customer = db.Customers.FirstOrDefault(i => i.Id == cg.CustomerId);
                if (customer != null)
                {
                    model.CustomerId = customer.Id;
                    model.CustomerName = customer.Name; 
                    model.CustomerPhoneNumber = customer.PhoneNumber;
                }
            }
            return View(model);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SNCCGUAC320")]
        public ActionResult AddToCart(GroceryAddToCartViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            try
            {
                if (model.ToPay > 0)
                {
                    var shop = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
                    var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);
                    var shopCredits = db.ShopCredits.FirstOrDefault(i => i.CustomerId == shop.CustomerId);
                    if ((shopCredits.PlatformCredit < 26 && shopCredits.DeliveryCredit < 67) && shop.IsTrail == false)
                    {
                        //Shop DeActivate
                        shop.Status = 6;
                        db.Entry(shop).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        //Order 
                        var order = _mapper.Map<GroceryAddToCartViewModel, Models.Order>(model);
                        if (model.CustomerId != 0)
                        {
                            order.CustomerId = customer.Id;
                            order.CustomerName = customer.Name;
                            order.CustomerPhoneNumber = customer.PhoneNumber;
                            order.CreatedBy = customer.Name;
                            order.UpdatedBy = customer.Name;
                        }
                        order.OrderNumber = model.OrderNumber;
                        order.ShopId = shop.Id;
                        order.ShopName = shop.Name;
                        order.DeliveryAddress = model.DeliveryAddress;
                        order.ShopPhoneNumber = shop.PhoneNumber ?? shop.ManualPhoneNumber;
                        order.ShopOwnerPhoneNumber = shop.OwnerPhoneNumber;
                        order.TotalPrice = model.ListItems.Sum(i => i.Price);
                        order.TotalProduct = model.ListItems.Count();
                        order.TotalQuantity = model.ListItems.Sum(i => Convert.ToInt32(i.Quantity));
                        order.NetTotal = model.ToPay;
                        order.DeliveryCharge = model.GrossDeliveryCharge;
                        order.ShopDeliveryDiscount = model.ShopDeliveryDiscount;
                        order.NetDeliveryCharge = model.NetDeliveryCharge;
                        order.Convinenientcharge = model.ConvenientCharge;
                        order.Packingcharge = model.PackingCharge;
                        order.Latitude = model.Latitude ?? 0;
                        order.Longitude = model.Longitude ?? 0;
                        order.Distance = model.Distance;
                        order.RatePerOrder = db.PlatFormCreditRates.FirstOrDefault(i => i.Status == 0).RatePerOrder;
                        order.PaymentMode = "Cash On Hand";
                        order.PaymentModeType = 2;
                        order.DateEncoded = DateTime.Now;
                        order.DateUpdated = DateTime.Now;
                        order.Status = 2;
                        order.UploadType = 2;               // For Grocery Upload type is 2
                        order.UploadId = model.GroceryId;
                        var custAddress = db.CustomerAddresses.FirstOrDefault(i => i.Address == order.DeliveryAddress);
                        if (custAddress != null)
                        {
                            order.CustomerAddressId = custAddress.Id;
                            customer.DistrictName = custAddress.DistrictName;
                            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        var deliveryRatePercentage = db.DeliveryRatePercentages.OrderByDescending(i => i.Id).FirstOrDefault(i => i.Status == 0);
                        if (deliveryRatePercentage != null)
                        {
                            order.DeliveryRatePercentage = deliveryRatePercentage.Percentage;
                            order.DeliveryRatePercentageId = deliveryRatePercentage.Id;
                        }
                        //var deliveryCharge = db.DeliveryCharges.FirstOrDefault(i => i.Type == shop.DeliveryType && i.TireType == shop.DeliveryTierType && i.VehicleType == 1 && i.Status == 0);
                        //if (deliveryCharge != null)
                        //{
                            order.DeliveryChargeUpto5Km = (shop.DeliveryTierType == 1 && shop.DeliveryType == 0) ? 35 : 40;
                            order.DeliveryChargePerKm = (shop.DeliveryTierType == 1 && shop.DeliveryType == 0) ? 6 : 8;
                        order.DeliveryChargeRemarks = (shop.DeliveryTierType == 1 && shop.DeliveryType == 0) ? "" : db.PincodeRates.FirstOrDefault(i => i.Id == shop.PincodeRateId && i.Status == 0)?.Remarks;
                        //}
                        db.Orders.Add(order);
                        db.SaveChanges();
                        //OrderItems
                        foreach (var item in model.ListItems)
                        {
                            if (item.ItemId != 0)
                            {
                                //var product = db.Products.FirstOrDefault(i => i.ItemId == item.ItemId && i.Status == 0);
                                var product = db.Products.FirstOrDefault(i => i.Id == item.ProductId && i.Status == 0);
                                product.HoldOnStok = Convert.ToInt32(item.Quantity);
                                product.Qty = product.Qty - Convert.ToInt32(item.Quantity);
                                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                            var orderItem = _mapper.Map<GroceryAddToCartViewModel.ListItem, OrderItem>(item);
                            orderItem.Status = 0;
                            orderItem.OrderId = order.Id;
                            orderItem.OrdeNumber = order.OrderNumber;
                            orderItem.ProductId = item.ProductId;
                            orderItem.ProductName = item.ProductName;
                            orderItem.BrandId = item.BrandId;
                            orderItem.BrandName = item.BrandName;
                            orderItem.CategoryId = item.CategoryId;
                            orderItem.CategoryName = item.CategoryName;
                            orderItem.ImagePath = item.ImagePath;
                            orderItem.Quantity = item.Quantity;
                            orderItem.UnitPrice = item.UnitPrice;
                            orderItem.Price = item.Price;
                            orderItem.MRPPrice = item.MRPPrice;
                            db.OrderItems.Add(orderItem);
                            db.SaveChanges();
                        }
                        // Payment
                        var payment = new Payment();
                        payment.Amount = model.ToPay;
                        payment.PaymentMode = "Cash On Hand";
                        payment.PaymentModeType = 2;
                        payment.CustomerId = customer.Id;
                        payment.CustomerName = customer.Name;
                        payment.ShopId = shop.Id;
                        payment.ShopName = shop.Name;
                        payment.OriginalAmount = order.TotalPrice;
                        payment.GSTAmount = model.ToPay;
                        payment.Currency = "Rupees";
                        payment.CountryName = null;
                        payment.PaymentResult = "pending";
                        payment.Credits = "N/A";
                        payment.OrderNumber = order.OrderNumber;
                        payment.PaymentCategoryType = 0;
                        payment.Credits = "N/A";
                        payment.CreditType = 2;
                        payment.ConvenientCharge = model.ConvenientCharge;
                        payment.PackingCharge = model.PackingCharge;
                        payment.DeliveryCharge = model.GrossDeliveryCharge;
                        payment.RatePerOrder = order.RatePerOrder;
                        payment.RefundStatus = 1;
                        payment.Status = 0;
                        payment.CreatedBy = customer.Name;
                        payment.UpdatedBy = customer.Name;
                        payment.DateEncoded = DateTime.Now;
                        payment.DateUpdated = DateTime.Now;
                        db.Payments.Add(payment);
                        db.SaveChanges();
                        // Prescription 
                        var grocery = db.CustomerGroceryUploads.FirstOrDefault(i => i.Id == model.GroceryId);
                        if (grocery != null)
                        {
                            grocery.Status = 1;
                            db.Entry(grocery).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }

                        // Shop Credits Balance
                        shopCredits.PlatformCredit -= payment.RatePerOrder.Value;
                        db.Entry(shopCredits).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        if (order != null)
                        {
                            var fcmToken = (from c in db.Customers
                                            join s in db.Shops on c.Id equals s.CustomerId
                                            where s.Id == model.ShopId
                                            select c.FcmTocken ?? "").FirstOrDefault().ToString();
                            Helpers.PushNotification.SendbydeviceId("You have received new order.Accept Soon", "Snowch", "OwnerNewOrder","", fcmToken.ToString(), "tune4.caf");

                            return Json(new { status = true, orderId = order.Id }, JsonRequestBehavior.AllowGet);
                        }
                        else
                            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public async Task<JsonResult> GetShopProductSelect2(int shopid, string q = "")
        {
            var model = await db.Products.Where(a => a.ShopId == shopid && a.Status == 0)
                .Join(db.MasterProducts.Where(a => a.Name.Contains(q)), p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
                .Join(db.Categories, p => p.p.CategoryId, c => c.Id, (p, c) => new { p, c })
                .Take(500)
                .Select(i => new
                {
                    id = i.p.p.Id,
                    text = i.p.m.Name,
                    price = i.p.p.Price,
                    weight = i.p.m.Weight,
                    size = i.p.m.SizeLWH,
                    brandid = i.p.m.BrandId,
                    brandname = i.p.m.BrandName,
                    categoryid = i.p.p.CategoryId,
                    categoryname = i.c.Name,
                    imagepath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + i.p.m.ImagePath1,
                    itemid = i.p.p.ItemId,
                    quantity = i.p.p.Qty,
                    mrpprice = i.p.p.MenuPrice,
                    percentage = i.p.p.Percentage
                }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShopCharge(int shopid, double itemTotal, int customerid, double totalSize, double totalWeight, double distance)
        {
            var model = new BillingDeliveryChargeViewModel();
            model = CommonHelpers.GetDeliveryCharge(shopid, totalSize, totalWeight);

            var shop = db.Shops.Where(i => i.Id == shopid && i.Status == 0).FirstOrDefault();
            var ConvenientCharge = 0.0;
            var GrossDeliveryCharge = 0.0;
            var ShopDeliveryDiscount = 0.0;
            var NetDeliveryCharge = 0.0;
            var PackingCharge = model.PackingCharge;
            if (itemTotal < model.ConvenientChargeRange)
                ConvenientCharge = model.ConvenientCharge;
            if (distance < 5)
                GrossDeliveryCharge = model.DeliveryChargeKM;
            else
            {
                var dist = distance - 5;
                var amount = dist * model.DeliveryChargeOneKM;
                GrossDeliveryCharge = model.DeliveryChargeKM + amount;
            }
            ShopDeliveryDiscount = itemTotal * (model.DeliveryDiscountPercentage / 100);
            if (ShopDeliveryDiscount >= GrossDeliveryCharge)
            {
                ShopDeliveryDiscount = GrossDeliveryCharge;
                NetDeliveryCharge = 0;
            }
            else
                NetDeliveryCharge = GrossDeliveryCharge - ShopDeliveryDiscount;
            return Json(new { PackingCharge, ConvenientCharge, GrossDeliveryCharge, ShopDeliveryDiscount, NetDeliveryCharge, distance }, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult Reject(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var cg = db.CustomerGroceryUploads.Where(b => b.Id == Id).FirstOrDefault();
            if (cg != null)
            {
                cg.Status = 2;
                db.Entry(cg).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Cancel(CustomerGroceryCancelViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var cg = db.CustomerGroceryUploads.Where(b => b.Id == model.Id).FirstOrDefault();
            if (cg != null)
            {
                cg.Status = 2;
                cg.CancelRemarks = model.CancelRemarks;
                db.Entry(cg).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("List");
        }

    }
}