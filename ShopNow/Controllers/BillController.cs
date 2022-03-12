using AutoMapper;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class BillController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        public BillController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                // Delivery Charge
                config.CreateMap<DeliveryChargeCreateViewModel, DeliveryCharge>();
                config.CreateMap<DeliveryChargeEditViewModel, DeliveryCharge>();
                config.CreateMap<DeliveryCharge, DeliveryChargeEditViewModel>();
                config.CreateMap<DeliveryCharge, DeliveryChargeListViewModel.DeliveryList>();

                // Delivery Charge Assign
                //config.CreateMap<DeliveryChargeAssignCreateViewModel, DeliveryCharge>();
                config.CreateMap<DeliveryChargeEditViewModel, DeliveryCharge>();
                config.CreateMap<DeliveryCharge, DeliveryChargeEditViewModel>();

                // Billing Charge
                config.CreateMap<BillingChargeCreateViewModel, BillingCharge>();
                config.CreateMap<BillingChargeEditViewModel, BillingCharge>();
                config.CreateMap<BillingCharge, BillingChargeEditViewModel>();
                config.CreateMap<BillingCharge, BillingChargeListViewModel.BillList>();

            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SNCBBL043")]
        public ActionResult BillingChargeList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new BillingChargeListViewModel();
            model.List = db.Shops.Join(db.BillingCharges.Where(i => i.Status == 0), s=> s.Id, b=> b.ShopId, (s,b)=> new { s,b}).Select(i => new BillingChargeListViewModel.BillList
            {
                Id = i.b.Id,
                ConvenientCharge = i.b.ConvenientCharge,
                ItemType = i.b.ItemType,
                PackingCharge = i.b.PackingCharge,
                ShopId = i.b.ShopId,
                ShopName = i.s.Name
            }).ToList();
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SNCBBEL044")]
        public ActionResult BillingChargeEmptyList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new BillingChargeEmptyListViewModel();
            model.EmptyList = db.Shops.Where(i=>i.Status==0)
                .GroupJoin(db.BillingCharges.Where(i => i.Status == 0), s => s.Id, b => b.ShopId, (s, b) => new { s, b }).Select(i => new BillingChargeEmptyListViewModel.EmptyBillList
            {
                ShopId = i.s.Id,
                ShopName = i.s.Name,
                //ConvenientCharge = i.b.FirstOrDefault().ConvenientCharge,
                //PackingCharge = i.b.FirstOrDefault().PackingCharge,
                HasBilling = i.b.Any()
            }).Where(i=> i.HasBilling == false).ToList();
            return View(model.EmptyList);
        }

        [AccessPolicy(PageCode = "SNCBB045")]
        public ActionResult BillingCharge(string shopname, int shopid=0)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var model = new BillingChargeCreateViewModel();
            ViewBag.Name = user.Name;
            ViewBag.ShopId = shopid;
            ViewBag.ShopName = shopname;
            var ratecode = db.PlatFormCreditRates.Where(i => i.Status == 0).Select(i => i.Id).FirstOrDefault();
            ViewBag.PlatformCreditRateId = ratecode;
            var platFormCreaditRate = db.PlatFormCreditRates.Where(i => i.Id == ratecode).FirstOrDefault();
            if (platFormCreaditRate != null)
            {
                ViewBag.PlatformCreditRate = platFormCreaditRate.RatePerOrder;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCBB045")]
        public ActionResult BillingCharge(BillingChargeCreateViewModel model)
        {
            bool isCheck = db.BillingCharges.Any(i => i.ShopId == model.ShopId && i.Status == 0);
            if (isCheck)
            {
                ViewBag.Message = "Billing Charge Already Exist";
                return View();
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var billingcharge = _mapper.Map<BillingChargeCreateViewModel, BillingCharge>(model);

            billingcharge.Status = 0;
            billingcharge.DateEncoded = DateTime.Now;
            billingcharge.DateUpdated = DateTime.Now;
            billingcharge.CreatedBy = user.Name;
            billingcharge.UpdatedBy = user.Name;
            db.BillingCharges.Add(billingcharge);
            db.SaveChanges();
            UpdateShopDeliveryDiscountPercentage(billingcharge.ShopId, billingcharge.DeliveryDiscountPercentage);
            return RedirectToAction("BillingChargeList");
        }

        [AccessPolicy(PageCode = "SNCBBU046")]
        public ActionResult BillingUpdate(string Id)
        {
            var dCode = AdminHelpers.DCodeInt(Id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (dCode == 0)
                return HttpNotFound();
            var bill = db.BillingCharges.Where(b => b.Id == dCode && b.Status == 0).FirstOrDefault();
            var model = _mapper.Map<BillingCharge, BillingChargeEditViewModel>(bill);
            var ratecode = db.PlatFormCreditRates.Where(i => i.Status == 0).Select(i => i.Id).FirstOrDefault();
            ViewBag.PlatformCreditRateId = ratecode;
            var platFormCreaditRate = db.PlatFormCreditRates.Where(i => i.Id == ratecode).FirstOrDefault();
            if (platFormCreaditRate != null)
            {
                ViewBag.PlatformCreditRate = platFormCreaditRate.RatePerOrder;
            }
            model.ShopName = db.Shops.FirstOrDefault(i => i.Id == model.ShopId).Name;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCBBU046")]
        public ActionResult BillingUpdate(BillingChargeEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            BillingCharge bill = db.BillingCharges.Where(b => b.Id == model.Id && b.Status == 0).FirstOrDefault();
            _mapper.Map(model, bill);
            if (bill != null)
            {
                bill.DateUpdated = DateTime.Now;
                bill.UpdatedBy = user.Name;
                db.Entry(bill).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                UpdateShopDeliveryDiscountPercentage(bill.ShopId, bill.DeliveryDiscountPercentage);
            }
            return RedirectToAction("BillingChargeList");
        }

        [AccessPolicy(PageCode = "SNCBBD047")]
        public JsonResult BillingDelete(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var bill = db.BillingCharges.Where(b => b.Id == Id && b.Status == 0).FirstOrDefault();
            if (bill != null)
            {
                bill.Status = 2;
                bill.DateUpdated = DateTime.Now;
                bill.UpdatedBy = user.Name;
                db.Entry(bill).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        // Delivery Charge Entry

        [AccessPolicy(PageCode = "SNCBDL048")]
        public ActionResult DeliveryChargeList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DeliveryChargeListViewModel();
            model.List = db.DeliveryCharges.Where(i => i.Status == 0).Select(i => new DeliveryChargeListViewModel.DeliveryList
            {
                Id = i.Id,
                ChargeUpto5Km = i.ChargeUpto5Km,
                ChargePerKm = i.ChargePerKm,
                Type = i.Type,
                TireType = i.TireType,
                VehicleType = i.VehicleType
            }).OrderBy(i => i.Type).ToList();
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SNCBD049")]
        public ActionResult DeliveryCharge()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCBD049")]
        public ActionResult DeliveryCharge(DeliveryChargeCreateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var isExist = db.DeliveryCharges.Any(i => i.Type == model.Type && i.TireType == model.TireType && i.VehicleType == model.VehicleType && i.Status == 0);
            if (isExist)
            {
                ViewBag.Message = "Already Exist";
                return View();
            }
            var deliveycharge = _mapper.Map<DeliveryChargeCreateViewModel, DeliveryCharge>(model);

            deliveycharge.Status = 0;
            deliveycharge.DateEncoded = DateTime.Now;
            deliveycharge.DateUpdated = DateTime.Now;
            deliveycharge.CreatedBy = user.Name;
            deliveycharge.UpdatedBy = user.Name;
            db.DeliveryCharges.Add(deliveycharge);
            db.SaveChanges();
            return RedirectToAction("DeliveryChargeList");
        }

        [AccessPolicy(PageCode = "SNCBDU050")]
        public ActionResult DeliveryUpdate(string id)
        {
            var dCode = AdminHelpers.DCodeInt(id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (dCode == 0)
                return HttpNotFound();
            var deliveryCharge = db.DeliveryCharges.Where(b => b.Id == dCode && b.Status == 0).FirstOrDefault();
            var model = _mapper.Map<DeliveryCharge, DeliveryChargeEditViewModel>(deliveryCharge);
            if (deliveryCharge.Type == 1)
            {
                model.ChargeUpto5Km1 = deliveryCharge.ChargeUpto5Km;
                model.ChargePerKm1 = deliveryCharge.ChargePerKm;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCBDU050")]
        public ActionResult DeliveryUpdate(DeliveryChargeEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            DeliveryCharge deliverycharge = db.DeliveryCharges.Where(b => b.Id == model.Id && b.Status == 0).FirstOrDefault();
            _mapper.Map(model, deliverycharge);
            if (deliverycharge != null)
            {
                deliverycharge.DateUpdated = DateTime.Now;
                deliverycharge.UpdatedBy = user.Name;
                if (model.Type == 1)
                {
                    deliverycharge.ChargeUpto5Km = model.ChargeUpto5Km1;
                    deliverycharge.ChargePerKm = model.ChargePerKm1;
                }
                deliverycharge.DateUpdated = DateTime.Now;
                db.Entry(deliverycharge).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("DeliveryChargeList");
        }

        [AccessPolicy(PageCode = "SNCBDD051")]
        public JsonResult DeliveryDelete(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var deliverycharge = db.DeliveryCharges.Where(b => b.Id == id && b.Status == 0).FirstOrDefault();
            if (deliverycharge != null)
            {
                deliverycharge.Status = 2;
                deliverycharge.DateUpdated = DateTime.Now;
                deliverycharge.UpdatedBy = user.Name;
                db.Entry(deliverycharge).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        // Delivery Charge Assign
        [AccessPolicy(PageCode = "SNCBDAL052")]
        public ActionResult DeliveryChargeAssignList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DeliveryChargeAssignListViewModel();
            model.List = db.Shops.Where(i => i.Status == 0).Select(i => new DeliveryChargeAssignListViewModel.DeliveryAssignList
            {
                ShopId = i.Id,
                ShopName = i.Name,
                Type = i.DeliveryType,
                TireType = i.DeliveryTierType
            }).ToList();
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SNCBDA053")]
        public ActionResult DeliveryChargeAssign()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCBDA053")]
        public ActionResult DeliveryChargeAssign(DeliveryChargeAssignCreateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.Where(i => i.Id == model.ShopId).FirstOrDefault();

            shop.DeliveryType = model.Type;
            shop.DeliveryTierType = model.TireType;
            shop.UpdatedBy = user.Name;
            shop.DateUpdated = DateTime.Now;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("DeliveryChargeAssignList");
        }

        //[AccessPolicy(PageCode = "SHNBILDD008")]
        //public JsonResult DeliveryChargeDelete(int id)
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    var deliverycharge = db.DeliveryCharges.Where(b => b.Id == id && b.Status == 0).FirstOrDefault();
        //    if (deliverycharge != null)
        //    {
        //        deliverycharge.Status = 2;
        //        deliverycharge.DateUpdated = DateTime.Now;
        //        deliverycharge.UpdatedBy = user.Name;
        //        db.Entry(deliverycharge).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        public async Task<JsonResult> GetBillShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name + " -- " + i.DistrictName,
                textSave = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeliveryChargeExist(int tiretype, int vehicletype)
        {
            var GeneralCount = db.DeliveryCharges.Where(i => i.Type == 0 && i.TireType == tiretype && i.VehicleType == vehicletype && i.Status == 0).Count();
            var SpecialCount = db.DeliveryCharges.Where(i => i.Type == 1 && i.TireType == tiretype && i.VehicleType == vehicletype && i.Status == 0).Count();
            bool IsAdded = false;
            string message = "";
            if (GeneralCount == 0)
            {
                IsAdded = true;
            }
            else
            {
                message = "General Entry Already Exist";
            }
            if (SpecialCount == 0)
            {
                IsAdded = true;
            }
            else
            {
                IsAdded = false;
                message = "Special Entry Already Exist";
            }
            return Json(new { IsAdded = IsAdded, message = message }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDeliveryChargeType(int type, int tiretype)
        {
            var model = new DeliveryChargeListViewModel();
            model.List = await db.DeliveryCharges.Where(a => a.Type == type && a.TireType == tiretype && a.Status == 0).Select(i => new DeliveryChargeListViewModel.DeliveryList
            {
                Id = i.Id,
                ChargeUpto5Km = i.ChargeUpto5Km,
                ChargePerKm = i.ChargePerKm,
                TireType = i.TireType,
                Type = i.Type,
                VehicleType = i.VehicleType
            }).ToListAsync();

            return Json(new { model.List, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public void UpdateShopDeliveryDiscountPercentage(int shopid,double percentage)
        {
            var shop = db.Shops.FirstOrDefault(i => i.Id == shopid);
            shop.DeliveryDiscountPercentage = percentage;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
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