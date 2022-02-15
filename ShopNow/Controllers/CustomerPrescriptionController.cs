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
    public class CustomerPrescriptionController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        public CustomerPrescriptionController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<CustomerPrescription, AddToCartViewModel>();
                config.CreateMap<AddToCartViewModel, Order>();
                config.CreateMap<AddToCartViewModel.ListItem, OrderItem>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SNCCPL111")]
        public ActionResult List()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var list = db.CustomerPrescriptions.OrderByDescending(i => i.Id).ToList();
            var model = new CustomerPrescriptionWebListViewModel();
            model.ListItems = db.CustomerPrescriptions.Where(i => i.Status == 0).OrderByDescending(i => i.Id)
                .GroupJoin(db.CustomerPrescriptionImages, cp => cp.Id, cpi => cpi.CustomerPrescriptionId, (cp, cpi) => new { cp, cpi })
                .Join(db.Shops, cp => cp.cp.ShopId, s => s.Id, (cp, s) => new { cp, s })
                .Select(i => new CustomerPrescriptionWebListViewModel.ListItem
                {
                    Id = i.cp.cp.Id,
                    AudioPath = (!string.IsNullOrEmpty(i.cp.cp.AudioPath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Audio/" + i.cp.cp.AudioPath : "",
                    CustomerId = i.cp.cp.CustomerId,
                    CustomerName = i.cp.cp.CustomerName,
                    CustomerPhoneNumber = i.cp.cp.CustomerPhoneNumber,
                    ImagePath = i.cp.cp.ImagePath,
                    Remarks = i.cp.cp.Remarks,
                    ShopId = i.cp.cp.ShopId,
                    ShopName = i.s.Name,
                    DateEncoded = i.cp.cp.DateEncoded,
                    Status = i.cp.cp.Status,
                    ImagePathLists = i.cp.cpi.Select(a => new CustomerPrescriptionWebListViewModel.ListItem.ImagePathList
                    {
                        ImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + a.ImagePath
                    }).ToList()
                }).ToList();
            return View(model);
        }

        public ActionResult CancelList()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var list = db.CustomerPrescriptions.OrderByDescending(i => i.Id).ToList();
            var model = new CustomerPrescriptionWebListViewModel();
            model.ListItems = db.CustomerPrescriptions.Where(i => i.Status == 2).OrderByDescending(i => i.Id)
                .GroupJoin(db.CustomerPrescriptionImages, cp => cp.Id, cpi => cpi.CustomerPrescriptionId, (cp, cpi) => new { cp, cpi })
                .Select(i => new CustomerPrescriptionWebListViewModel.ListItem
                {
                    Id = i.cp.Id,
                    AudioPath = (!string.IsNullOrEmpty(i.cp.AudioPath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Audio/" + i.cp.AudioPath : "",
                    CustomerId = i.cp.CustomerId,
                    CustomerName = i.cp.CustomerName,
                    CustomerPhoneNumber = i.cp.CustomerPhoneNumber,
                    ImagePath = i.cp.ImagePath,
                    Remarks = i.cp.Remarks,
                    ShopId = i.cp.ShopId,
                    DateEncoded = i.cp.DateEncoded,
                    Status = i.cp.Status,
                    ImagePathLists = i.cpi.Select(a => new CustomerPrescriptionWebListViewModel.ListItem.ImagePathList
                    {
                        ImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + a.ImagePath
                    }).ToList()
                }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCPAC298")]
        public ActionResult AddToCart(int id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new AddToCartViewModel();
            var cp = db.CustomerPrescriptions.FirstOrDefault(i => i.Id == id);
            if (cp != null)
            {
                _mapper.Map(cp, model);
                model.PrescriptionId = cp.Id;
                model.DeliveryAddress = cp.DeliveryAddress;
                model.Latitude = cp.Latitude;
                model.Longitude = cp.Longitude;
                model.ImagePathLists = db.CustomerPrescriptionImages.Where(i => i.CustomerPrescriptionId == cp.Id)
                        .Select(i => new AddToCartViewModel.ImagePathList
                        {
                            ImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/" + i.ImagePath
                        }).ToList();
                model.AudioPath = (!string.IsNullOrEmpty(cp.AudioPath)) ? "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Audio/" + cp.AudioPath : "";
                var shop = db.Shops.FirstOrDefault(i => i.Id == cp.ShopId);
                if (shop != null)
                {
                    model.ShopName = shop.Name;
                    model.ShopPhoneNumber = shop.PhoneNumber;
                    model.ShopImagePath = shop.ImagePath;
                    model.ShopAddress = shop.Address;
                }
                var customer = db.Customers.FirstOrDefault(i => i.Id == cp.CustomerId);
                if (customer != null)
                {
                    model.CustomerId = customer.Id;
                    model.CustomerName = customer.Name;
                    model.CustomerPhoneNumber = customer.PhoneNumber;
                }
            }
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCCPPO310")]
        public ActionResult PrescriptionOrderList()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new PrescriptionOrderListViewModel();
            model.PrescriptionOrderLists = db.Orders.OrderByDescending(i => i.DateEncoded).Where(i => (i.UploadType == 1) && i.Status == 6)
                .AsEnumerable()
               .Select((i, index) => new PrescriptionOrderListViewModel.PrescriptionOrderList
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
            return View(model.PrescriptionOrderLists);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SNCCPAC298")]
        public ActionResult AddToCart(AddToCartViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            try
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
                    var order = _mapper.Map<AddToCartViewModel, Models.Order>(model);
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
                    order.IsPrescriptionOrder = true;
                    order.CustomerPrescriptionId = model.PrescriptionId;
                    order.UploadType = 1;               // For Prescription Upload type is 1
                    order.UploadId = model.PrescriptionId;
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
                        var orderItem = _mapper.Map<AddToCartViewModel.ListItem, OrderItem>(item);
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
                    var prescription = db.CustomerPrescriptions.FirstOrDefault(i => i.Id == model.PrescriptionId);
                    if (prescription != null)
                    {
                        prescription.Status = 1;
                        db.Entry(prescription).State = System.Data.Entity.EntityState.Modified;
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
                        Helpers.PushNotification.SendbydeviceId("You have received new order.Accept Soon", "ShopNowChat", "a.mp3", fcmToken.ToString());

                        return Json(new { status = true, orderId = order.Id }, JsonRequestBehavior.AllowGet);
                    }
                    else
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
                .Join(db.DiscountCategories, p => p.p.p.DiscountCategoryId, dc => dc.Id, (p, dc) => new { p, dc })
                .Take(500)
                .Select(i => new
                {
                    id = i.p.p.p.Id,
                    text = i.p.p.m.Name,
                    price = i.p.p.p.Price,
                    weight = i.p.p.m.Weight,
                    size = i.p.p.m.SizeLWH,
                    brandid = i.p.p.m.BrandId,
                    brandname = i.p.p.m.BrandName,
                    categoryid = i.p.p.p.CategoryId,
                    categoryname = i.p.c.Name,
                    imagepath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + i.p.p.m.ImagePath1,
                    itemid = i.p.p.p.ItemId,
                    quantity = i.p.p.p.Qty,
                    mrpprice = i.p.p.p.MenuPrice,
                    percentage = i.dc.Percentage
                }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShopCharge(int shopid, double itemTotal, int customerid, double totalSize, double totalWeight)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var model = new BillingDeliveryChargeViewModel();
            model = CommonHelpers.GetDeliveryCharge(shopid, totalSize, totalWeight);
            var customerPrescription = db.CustomerPrescriptions.FirstOrDefault(i => i.CustomerId == customerid);

            var shop = db.Shops.Where(i => i.Id == shopid && i.Status == 0).FirstOrDefault();
            var ConvenientCharge = 0.0;
            var GrossDeliveryCharge = 0.0;
            var ShopDeliveryDiscount = 0.0;
            var NetDeliveryCharge = 0.0;
            var PackingCharge = model.PackingCharge;
            if (itemTotal < model.ConvenientChargeRange)
            {
                ConvenientCharge = model.ConvenientCharge;
            }
            // Gross Delivery Charge
            var Distance = (((Math.Acos(Math.Sin((shop.Latitude * Math.PI / 180)) * Math.Sin(((customerPrescription.Latitude ??0)* Math.PI / 180)) + Math.Cos((shop.Latitude * Math.PI / 180)) * Math.Cos(((customerPrescription.Latitude??0) * Math.PI / 180))
                 * Math.Cos(((shop.Longitude - (customerPrescription.Longitude??0)) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344) / 1000;
            if (Distance < 5)
            {
                GrossDeliveryCharge = model.DeliveryChargeKM;
            }
            else
            {
                var dist = Distance - 5;
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
            {
                NetDeliveryCharge = GrossDeliveryCharge - ShopDeliveryDiscount;
            }
            return Json(new { PackingCharge, ConvenientCharge, GrossDeliveryCharge, ShopDeliveryDiscount, NetDeliveryCharge, Distance }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult AddPrescriptionItem(PrescriptionItemAddViewModel model)
        //{
        //    var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
        //    foreach (var item in model.ListItems)
        //    {
        //        var prescriptionItem = new CustomerPrescriptionItem
        //        {
        //            CreatedBy = user.Name,
        //            CustomerPrescriptionId = model.PrescriptionId,
        //            DateEncoded = DateTime.Now,
        //            ProductId = item.ProductId,
        //            Quantity = item.Quantity,
        //            Status = 0
        //        };
        //        db.CustomerPrescriptionItems.Add(prescriptionItem);
        //        db.SaveChanges();
        //    }

        //    var prescription = db.CustomerPrescriptions.FirstOrDefault(i => i.Id == model.PrescriptionId);
        //    if (prescription != null)
        //    {
        //        prescription.Status = 1;
        //        db.Entry(prescription).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public JsonResult GetItemList(int id)
        //{
        //    var list = db.CustomerPrescriptionItems.Where(i => i.CustomerPrescriptionId == id)
        //        .Join(db.Products, cp => cp.ProductId, p => p.Id, (cp, p) => new { cp, p })
        //        .Join(db.MasterProducts, cp => cp.p.MasterProductId, m => m.Id, (cp, m) => new { cp, m })
        //        .Select(i => new
        //        {
        //            ProductName = i.m.Name,
        //            Quantity = i.cp.cp.Quantity
        //        }).ToList();
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult Reject(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var cp = db.CustomerPrescriptions.Where(b => b.Id == Id).FirstOrDefault();
            if (cp != null)
            {
                cp.Status = 2;
                db.Entry(cp).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Cancel(CustomerPrescriptionCancelViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var cp = db.CustomerPrescriptions.Where(b => b.Id == model.Id).FirstOrDefault();
            if (cp != null)
            {
                cp.Status = 2;
                cp.CancelRemarks = model.CancelRemarks;
                db.Entry(cp).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("List");
        }

    }
}