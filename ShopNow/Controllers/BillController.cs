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
                config.CreateMap<Bill, BillListViewModel.BillList>();
                config.CreateMap<Bill, DeliveryListViewModel.DeliveryList>();
                config.CreateMap<BillCreateEditViewModel, Bill>();
                config.CreateMap<Bill, BillCreateEditViewModel>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNBILBL001")]
        public ActionResult BillingChargeList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new BillListViewModel();
            model.List = db.Bills.Where(i => i.NameOfBill == 1 && i.Status == 0).Select(i => new BillListViewModel.BillList
            {
                Id = i.Id,
                ConvenientCharge = i.ConvenientCharge,
                ItemType = i.ItemType,
                PackingCharge = i.PackingCharge,
                ShopId = i.ShopId,
                ShopName = i.ShopName
            }).ToList();
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNBILDL002")]
        public ActionResult DeliveryChargeList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DeliveryListViewModel();
            model.List = db.Bills.Where(i => i.NameOfBill == 0 && i.Status == 0 && i.ShopId == 0).Select(i => new DeliveryListViewModel.DeliveryList
            {
                Id = i.Id,
                DeliveryChargeKM = i.DeliveryChargeKM,
                DeliveryChargeOneKM = i.DeliveryChargeOneKM,
                DeliveryRateSet = i.DeliveryRateSet,
                Type = i.Type,
                VehicleType = i.VehicleType
            }).OrderBy(i=> i.DeliveryRateSet).ToList();
            model.GeneralCount1 = db.Bills.Where(i => i.NameOfBill == 0 && i.DeliveryRateSet == 0 && i.Status == 0 && i.Type == 1).Count();
            model.GeneralCount2 = db.Bills.Where(i => i.NameOfBill == 0 && i.DeliveryRateSet == 0 && i.Status == 0 && i.Type == 2).Count();
            model.GeneralCount3 = db.Bills.Where(i => i.NameOfBill == 0 && i.DeliveryRateSet == 0 && i.Status == 0 && i.Type == 3).Count();
            model.SpecialCount1 = db.Bills.Where(i => i.NameOfBill == 0 && i.DeliveryRateSet == 1 && i.Status == 0 && i.Type == 1).Count();
            model.SpecialCount2 = db.Bills.Where(i => i.NameOfBill == 0 && i.DeliveryRateSet == 1 && i.Status == 0 && i.Type == 2).Count();
            model.SpecialCount3 = db.Bills.Where(i => i.NameOfBill == 0 && i.DeliveryRateSet == 1 && i.Status == 0 && i.Type == 3).Count();
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNBILBC003")]
        public ActionResult BillingCharge()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var model = new BillCreateEditViewModel();
            ViewBag.Name = user.Name;
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
        [AccessPolicy(PageCode = "SHNBILBC003")]
        public ActionResult BillingCharge(BillCreateEditViewModel model)
        {
            bool isCheck = db.Bills.Any(i => i.ShopId == model.ShopId && i.NameOfBill == 1 && i.Status == 0);
            if (isCheck)
            {
                ViewBag.Message = "Billing Charge Already Exist";
                return View();
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var bill = _mapper.Map<BillCreateEditViewModel, Bill>(model);
            var shop = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
            if (shop != null)
            {
                bill.CustomerId = shop.CustomerId;
                bill.CustomerName = shop.CustomerName;
            }
            bill.CreatedBy = user.Name;
            bill.UpdatedBy = user.Name;
            bill.NameOfBill = 1;                // 1 - BillingCharge
            bill.DeliveryRateSet = 2;           // 2 - N/A
            bill.Status = 0;
            bill.DateEncoded = DateTime.Now;
            bill.DateUpdated = DateTime.Now;
            db.Bills.Add(bill);
            db.SaveChanges();
            return RedirectToAction("BillingChargeList");
        }
      
        [AccessPolicy(PageCode = "SHNBILDC004")]
        public ActionResult DeliveryCharge()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNBILDC004")]
        public ActionResult DeliveryCharge(BillCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var isExist = db.Bills.Any(i => i.NameOfBill == 0 && i.DeliveryRateSet == model.DeliveryRateSet && i.Type == model.Type && i.VehicleType == model.VehicleType && i.Status == 0);
            if (isExist)
            {
                ViewBag.Message = "Already Exist";
                return View();
            }
            var bill = _mapper.Map<BillCreateEditViewModel, Bill>(model);
            model.GeneralCount = db.Bills.Where(i => i.NameOfBill == 0 && i.DeliveryRateSet == 0 && i.Status == 0).Count();
            model.SpecialCount = db.Bills.Where(i => i.NameOfBill == 0 && i.DeliveryRateSet == 1 && i.Status == 0).Count();
            bill.CustomerId = user.Id;
            bill.CustomerName = user.Name;
            bill.ShopId = 0;
            bill.ShopName = "Admin";
            bill.CreatedBy = user.Name;
            bill.UpdatedBy = user.Name;
            bill.NameOfBill = 0;            // 0 - DeliveryCharge 
            bill.ItemType = 2;              // 2 - N/A
            bill.Distance = 5;
            if (model.DeliveryRateSet == 1)
            {
                bill.DeliveryChargeKM = model.DeliveryChargeKM1;
                bill.DeliveryChargeOneKM = model.DeliveryChargeOneKM1;
            }
            bill.Status = 0;
            bill.DateEncoded = DateTime.Now;
            bill.DateUpdated = DateTime.Now;
            db.Bills.Add(bill);
            db.SaveChanges();
            return RedirectToAction("DeliveryChargeList");
        }

        [AccessPolicy(PageCode = "SHNBILBU005")]
        public ActionResult BillingUpdate(string Id)
        {
            var dCode = AdminHelpers.DCodeInt(Id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (dCode==0)
                return HttpNotFound();
            var bill = db.Bills.Where(b => b.Id == dCode && b.Status==0).FirstOrDefault();
            var model = _mapper.Map<Bill, BillCreateEditViewModel>(bill);
            model.PlatformCreditRateId = db.PlatFormCreditRates.Where(i => i.Status == 0).Select(i => i.Id).FirstOrDefault();
            var platFormCreaditRate = db.PlatFormCreditRates.Where(p => p.Id == model.PlatformCreditRateId).FirstOrDefault();
            if (platFormCreaditRate != null)
            {
                model.PlatformCreditRate = platFormCreaditRate.RatePerOrder;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNBILBU005")]
        public ActionResult BillingUpdate(BillCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            Bill bill = db.Bills.Where(b => b.Id == model.Id && b.Status == 0).FirstOrDefault();
            _mapper.Map(model, bill);
            var shop = db.Shops.FirstOrDefault(i => i.Id == bill.ShopId);
            bill.CustomerId = shop.CustomerId;
            bill.CustomerName = shop.CustomerName;
            if (bill != null)
            {
                bill.DateUpdated = DateTime.Now;
                bill.UpdatedBy = user.Name;
                db.Entry(bill).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("BillingChargeList");
        }

        [AccessPolicy(PageCode = "SHNBILDU006")]
        public ActionResult DeliveryUpdate(string id)
        {
            var dCode = AdminHelpers.DCodeInt(id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (dCode==0)
                return HttpNotFound();
            var bill = db.Bills.Where(b => b.Id ==dCode && b.Status == 0).FirstOrDefault();
            var model = _mapper.Map<Bill, BillCreateEditViewModel>(bill);
            if(bill.DeliveryRateSet == 1)
            {
                model.DeliveryChargeKM1 = bill.DeliveryChargeKM;
                model.DeliveryChargeOneKM1 = bill.DeliveryChargeOneKM;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNBILDU006")]
        public ActionResult DeliveryUpdate(BillCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            Bill bill = db.Bills.Where(b => b.Id == model.Id && b.Status == 0).FirstOrDefault();
            _mapper.Map(model, bill);
            if (bill != null)
            {
                bill.DateUpdated = DateTime.Now;
                bill.UpdatedBy = user.Name;
                if (model.DeliveryRateSet == 1)
                {
                    bill.DeliveryChargeKM = model.DeliveryChargeKM1;
                    bill.DeliveryChargeOneKM = model.DeliveryChargeOneKM1;
                }
                bill.DateUpdated = DateTime.Now;
                db.Entry(bill).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("DeliveryChargeList");
        }

        [AccessPolicy(PageCode = "SHNBILBD007")]
        public JsonResult BillingDelete(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var bill = db.Bills.Where(b => b.Id == Id && b.Status == 0).FirstOrDefault();
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

        [AccessPolicy(PageCode = "SHNBILDD008")]
        public JsonResult DeliveryDelete(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var bill = db.Bills.Where(b => b.Id == id && b.Status == 0).FirstOrDefault();
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

        [AccessPolicy(PageCode = "SHNBILDD008")]
        public JsonResult DeliveryChargeDelete(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var bill = db.Bills.Where(b => b.Id == id && b.Status == 0).FirstOrDefault();
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

        [AccessPolicy(PageCode = "SHNBILDCAL09")]
        public ActionResult DeliveryChargeAssignList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DeliveryListViewModel();
            model.List = db.Bills.Where(i => i.NameOfBill == 0 && i.Status == 0 && i.ShopId != 0 ).Select(i => new DeliveryListViewModel.DeliveryList
            {
                Id = i.Id,
                ShopId = i.ShopId,
                ShopName = i.ShopName,
                DeliveryChargeKM = i.DeliveryChargeKM,
                DeliveryChargeOneKM = i.DeliveryChargeOneKM,
                DeliveryRateSet = i.DeliveryRateSet,
                Type = i.Type,
                VehicleType = i.VehicleType
            }).ToList();
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNBILDCA010")]
        public ActionResult DeliveryChargeAssign()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNBILDCA010")]
        public ActionResult DeliveryChargeAssign(BillCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var bill = _mapper.Map<BillCreateEditViewModel, Bill>(model);
            bill.CustomerId = user.Id;
            bill.CustomerName = user.Name;
            bill.CreatedBy = user.Name;
            bill.UpdatedBy = user.Name;
            bill.NameOfBill = 0;

            bool isGeneralCheck = db.Bills.Any(i => i.ShopId == model.ShopId && i.NameOfBill == 0 && i.DeliveryRateSet == 0 && i.Type == model.Type && i.VehicleType == model.VehicleType && i.Status == 0);
            bool isSpecialCheck = db.Bills.Any(i => i.ShopId == model.ShopId && i.NameOfBill == 0 && i.DeliveryRateSet == 1 && i.Type == model.Type && i.VehicleType == model.VehicleType && i.Status == 0);
            if (isGeneralCheck && isSpecialCheck)
            {
                ViewBag.Message = "Delivery Charge Already Exist";
                return View();
            }
             if(!isGeneralCheck)
            {
                bill.DeliveryChargeKM = model.DeliveryChargeKM;
                bill.DeliveryRateSet = model.DeliveryRateSet;
                bill.DeliveryChargeOneKM = model.DeliveryChargeOneKM;
                bill.Status = 0;
                bill.DateEncoded = DateTime.Now;
                bill.DateUpdated = DateTime.Now;
                db.Bills.Add(bill);
                db.SaveChanges();
            }
             if (!isSpecialCheck)
            {
                bill.DeliveryChargeKM = model.DeliveryChargeKM1;
                bill.DeliveryChargeOneKM = model.DeliveryChargeOneKM1;
                bill.DeliveryRateSet = model.DeliveryRateSet1;
                bill.Status = 0;
                bill.DateEncoded = DateTime.Now;
                bill.DateUpdated = DateTime.Now;
                db.Bills.Add(bill);
                db.SaveChanges();
            }
          
            
            //if (model.DeliveryRateSet == 0)
            //{
            //    bill.DeliveryRateSet = model.DeliveryRateSet;
            //    bill.DeliveryChargeOneKM = model.DeliveryChargeOneKM;
            //    bill.Status = 0;
            //    bill.DateEncoded = DateTime.Now;
            //    bill.DateUpdated = DateTime.Now;
            //    db.Bills.Add(bill);
            //    db.SaveChanges();
            //}
            //if (model.DeliveryRateSet1 == 1)
            //{
            //    bill.DeliveryChargeKM = model.DeliveryChargeKM1;
            //    bill.DeliveryChargeOneKM = model.DeliveryChargeOneKM1;
            //    bill.DeliveryRateSet = model.DeliveryRateSet1;
            //    bill.Status = 0;
            //    bill.DateEncoded = DateTime.Now;
            //    bill.DateUpdated = DateTime.Now;
            //    db.Bills.Add(bill);
            //    db.SaveChanges();
            //}
            
            return RedirectToAction("DeliveryChargeAssignList");
        }

        [AccessPolicy(PageCode = "SHNBILBC003")]
        public async Task<JsonResult> GetBillShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNBILDC004")]
        public JsonResult DeliveryChargeExist(int DeliveryRateSet, int type, int vehicletype)
        {
            var GeneralCount = db.Bills.Where(i => i.NameOfBill == 0 && i.DeliveryRateSet == 0 && i.Status == 0 && i.Type == type && i.VehicleType == vehicletype).Count();
            var SpecialCount = db.Bills.Where(i => i.NameOfBill == 0 && i.DeliveryRateSet == 1 && i.Status == 0 && i.Type == type && i.VehicleType == vehicletype).Count();
            bool IsAdded = false;
            string message = "";
            if(DeliveryRateSet == 0)
            {
                if (GeneralCount == 0)
                {
                    IsAdded = true;
                }
                else {
                    message = "General Entry Already Exist";
                }
            }
            else
            {
                if (SpecialCount == 0){
                    IsAdded = true;
                }
                else
                {
                    IsAdded = false;
                    message = "Special Entry Already Exist";
                }
            }
                return Json(new { IsAdded = IsAdded, message = message }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNBILDCA010")]
        public async Task<JsonResult> GetDeliveryChargeType(int type, int vehicletype)
        {
            var model = new DeliveryListViewModel();
            model.List = await db.Bills.Where(a => a.Type == type && a.VehicleType == vehicletype && a.Status == 0).Select(i => new DeliveryListViewModel.DeliveryList
            {
               Id = i.Id,
               DeliveryChargeKM = i.DeliveryChargeKM,
               DeliveryChargeOneKM = i.DeliveryChargeOneKM,
               DeliveryRateSet = i.DeliveryRateSet
            }).ToListAsync();

            return Json(new { model.List, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
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