using AutoMapper;
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
            model.ListItems = db.CustomerPrescriptions.OrderByDescending(i => i.Id)
                .GroupJoin(db.CustomerPrescriptionImages, cp => cp.Id, cpi => cpi.CustomerPrescriptionId, (cp, cpi) => new { cp, cpi })
                .Select(i => new CustomerPrescriptionWebListViewModel.ListItem
                {
                    Id = i.cp.Id,
                    //AudioPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Audio/" + i.cp.AudioPath,
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
                        ImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + a.ImagePath
                    }).ToList()
                }).ToList();
            return View(model);
        }

        public ActionResult AddToCart(int id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new AddToCartViewModel();
            var cp = db.CustomerPrescriptions.FirstOrDefault(i => i.Id == id);
            if (cp != null)
            {
                _mapper.Map(cp, model);
                var shop = db.Shops.FirstOrDefault(i => i.Id == cp.ShopId);
                if (shop != null)
                {
                    model.ShopName = shop.Name;
                    model.ShopPhoneNumber = shop.PhoneNumber;
                    model.ShopImagePath = shop.ImagePath;
                    model.ShopAddress = shop.Address;
                }
                var customer = db.Customers.FirstOrDefault(i => i.Id == cp.CustomerId);
                if(customer != null)
                {
                    model.CustomerId = customer.Id;
                    model.CustomerName = customer.Name;
                    model.DeliveryAddress = customer.Address;
                    model.CustomerPhoneNumber = customer.PhoneNumber;
                    model.Latitude = customer.Latitude;
                    model.Longitude = customer.Longitude;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(AddToCartViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            try
            {
                var shop = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
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
                    var order = _mapper.Map<AddToCartViewModel, Models.Order>(model);
                    if (model.CustomerId != 0)
                    {
                        var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);
                        order.CustomerId = customer.Id;
                        order.CreatedBy = customer.Name;
                        order.UpdatedBy = customer.Name;
                        order.CustomerName = customer.Name;
                        order.CustomerPhoneNumber = customer.PhoneNumber;

                        //Store Referral Number
                        //customer.ReferralNumber = model.ReferralNumber;
                        //db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                        //db.SaveChanges();
                    }
                    //order.OrderNumber = Math.Floor(Random *100);
                    order.ShopId = shop.Id;
                    order.ShopName = shop.Name;
                    order.ShopPhoneNumber = shop.PhoneNumber ?? shop.ManualPhoneNumber;
                    order.ShopOwnerPhoneNumber = shop.OwnerPhoneNumber;
                    order.TotalPrice = model.ListItems.Sum(i => i.Price);
                    order.TotalProduct = model.ListItems.Count();
                    order.TotalQuantity = model.ListItems.Sum(i => Convert.ToInt32(i.Quantity));
                    order.DateEncoded = DateTime.Now;
                    order.DateUpdated = DateTime.Now;
                    order.Status = 0;
                    db.Orders.Add(order);
                    db.SaveChanges();
                    foreach (var item in model.ListItems)
                    {
                        if (item.ItemId != 0)
                        {
                            var productMedicalStock = db.Products.FirstOrDefault(i => i.ItemId == item.ItemId && i.Status == 0);
                            productMedicalStock.HoldOnStok = Convert.ToInt32(item.Quantity);
                            productMedicalStock.Qty = productMedicalStock.Qty - Convert.ToInt32(item.Quantity);
                            db.Entry(productMedicalStock).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        var orderItem = _mapper.Map<AddToCartViewModel.ListItem, OrderItem>(item);
                        orderItem.Status = 0;
                        orderItem.OrderId = order.Id;
                        orderItem.OrdeNumber = order.OrderNumber;
                        db.OrderItems.Add(orderItem);
                        db.SaveChanges();

                        //if (item.AddOnListItems != null)
                        //{
                        //    foreach (var addon in item.AddOnListItems)
                        //    {
                        //        if (addon.Index == item.AddOnIndex)
                        //        {
                        //            var addonItem = _mapper.Map<OrderCreateViewModel.ListItem.AddOnListItem, OrderItemAddon>(addon);
                        //            addonItem.Status = 0;
                        //            addonItem.OrderItemId = orderItem.Id;
                        //            db.OrderItemAddons.Add(addonItem);
                        //            db.SaveChanges();
                        //        }
                        //    }
                        //}
                    }

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
                .Join(db.MasterProducts.Where(a => a.Name.Contains(q)), p => p.MasterProductId, m => m.Id, (p, m) => new { p, m }).Take(500)
                .Select(i => new
                {
                    id = i.p.Id,
                    text = i.m.Name,
                    price = i.p.Price
                }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShopCharge(int shopid, double itemTotal,int customerid)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            var customer = db.Customers.FirstOrDefault(i => i.Id == customerid);
            var shopbill = db.BillingCharges.Where(i => i.ShopId == shopid).FirstOrDefault();
            var shop = db.Shops.Where(i => i.Id == shopid && i.Status == 0).FirstOrDefault();
            var ConvenientCharge = 0.0;
            var GrossDeliveryCharge = 0.0;
            var ShopDeliveryDiscount = 0.0;
            var NetDeliveryCharge = 0.0;
            var PackingCharge = shopbill.PackingCharge;
            if(itemTotal < shopbill.ConvenientCharge)
            {
                ConvenientCharge = db.PlatFormCreditRates.FirstOrDefault(i=> i.Status == 0).RatePerOrder;
            }
            // Gross Delivery Charge
            var Distance = (((Math.Acos(Math.Sin((shop.Latitude * Math.PI / 180)) * Math.Sin((customer.Latitude * Math.PI / 180)) + Math.Cos((shop.Latitude * Math.PI / 180)) * Math.Cos((customer.Latitude * Math.PI / 180))
                 * Math.Cos(((shop.Longitude - customer.Longitude) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344) / 1000;
            var shopdelivery = db.DeliveryCharges.Where(i => i.Type == shop.DeliveryType && i.TireType == shop.DeliveryTierType ).FirstOrDefault();
            //var deliverybill = db.Bills.Where(i => i.ShopId == cart.ShopId && i.NameOfBill == 0 && i.DeliveryRateSet == 0 && i.Status == 0).FirstOrDefault();
            if (Distance < 5)
            {
                GrossDeliveryCharge = shopdelivery.ChargeUpto5Km;
            }
            else
            {
                var dist = Distance - 5;
                var amount = dist * shopdelivery.ChargePerKm;
                GrossDeliveryCharge = shopdelivery.ChargePerKm + amount;
            }
            ShopDeliveryDiscount = itemTotal * (shopbill.DeliveryDiscountPercentage / 100);
            if(ShopDeliveryDiscount >= GrossDeliveryCharge)
            {
                ShopDeliveryDiscount = GrossDeliveryCharge;
                NetDeliveryCharge = 0;
            }
            else
            {
                NetDeliveryCharge = GrossDeliveryCharge - ShopDeliveryDiscount;
            }
           // var Distance = Distance.ToString("0.##");
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



        }
    }