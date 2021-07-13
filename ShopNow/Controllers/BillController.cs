﻿using AutoMapper;
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
        private ShopnowchatEntities db = new ShopnowchatEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private const string _prefix = "BIL";
        private static string _generatedCode
        {
            get
            {
                return ShopNow.Helpers.DRC.Generate(_prefix);
            }
        }

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
                Code = i.Code,
                ConvenientCharge = i.ConvenientCharge,
                ItemType = i.ItemType,
                PackingCharge = i.PackingCharge,
                ShopCode = i.ShopCode,
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
            model.List = db.Bills.Where(i => i.NameOfBill == 0 && i.Status == 0 && i.ShopCode == "Admin").Select(i => new DeliveryListViewModel.DeliveryList
            {
                Code = i.Code,
                DeliveryChargeKM = i.DeliveryChargeKM,
                DeliveryChargeOneKM = i.DeliveryChargeOneKM,
                DeliveryRateSet = i.DeliveryRateSet,
                Type = i.Type
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
            var ratecode = db.PlatFormCreditRates.Where(i => i.Status == 0).Select(i => i.Code).FirstOrDefault();
            ViewBag.PlatformCreditRateCode = ratecode;
            var platFormCreaditRate = db.PlatFormCreditRates.Where(i => i.Code == ratecode).FirstOrDefault();//PlatFormCreditRate.Get(model.PlatformCreditRateCode);
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
            bool isCheck = db.Bills.Any(i => i.ShopCode == model.ShopCode && i.NameOfBill == 1 && i.Status == 0);
            if (isCheck)
            {
                ViewBag.Message = "Billing Charge Already Exist";
                return View();
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var bill = _mapper.Map<BillCreateEditViewModel, Bill>(model);
            var shop = db.Shops.FirstOrDefault(i => i.Code == model.ShopCode);
            if (shop != null) {
                bill.CustomerCode = shop.CustomerCode;
                bill.CustomerName = shop.CustomerName;
            }
            bill.CreatedBy = user.Name;
            bill.UpdatedBy = user.Name;
            bill.NameOfBill = 1;                // 1 - BillingCharge
            bill.DeliveryRateSet = 2;           // 2 - N/A
            if(model.ItemType == 1)
            {
                bill.PackingCharge = model.PackingCharge1;
            }

            // Bill.Add(bill, out int error);
            bill.Code = _generatedCode;
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
            var isExist = db.Bills.Any(i => i.NameOfBill == 0 && i.DeliveryRateSet == model.DeliveryRateSet && i.Type == model.Type && i.Status == 0);
            if (isExist)
            {
                ViewBag.Message = "Already Exist";
                return View();
            }
            var bill = _mapper.Map<BillCreateEditViewModel, Bill>(model);
            model.GeneralCount = db.Bills.Where(i => i.NameOfBill == 0 && i.DeliveryRateSet == 0 && i.Status == 0).Count();
            model.SpecialCount = db.Bills.Where(i => i.NameOfBill == 0 && i.DeliveryRateSet == 1 && i.Status == 0).Count();
            bill.CustomerCode = user.Code;
            bill.CustomerName = user.Name;
            bill.ShopCode = "Admin";
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
            // Bill.Add(bill, out int error);
            bill.Code = _generatedCode;
            bill.Status = 0;
            bill.DateEncoded = DateTime.Now;
            bill.DateUpdated = DateTime.Now;
            db.Bills.Add(bill);
            db.SaveChanges();
            return RedirectToAction("DeliveryChargeList");
        }

        [AccessPolicy(PageCode = "SHNBILBU005")]
        public ActionResult BillingUpdate(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dCode))
                return HttpNotFound();
            var bill = db.Bills.Where(b => b.Code == dCode && b.Status==0).FirstOrDefault(); //Bill.Get(dCode);
            var model = _mapper.Map<Bill, BillCreateEditViewModel>(bill);
            if(model.ItemType == 1)
            {
                model.PackingCharge1 = model.PackingCharge;
            }
            model.PlatformCreditRateCode = db.PlatFormCreditRates.Where(i => i.Status == 0).Select(i => i.Code).FirstOrDefault();
            var platFormCreaditRate = db.PlatFormCreditRates.Where(p => p.Code == model.PlatformCreditRateCode).FirstOrDefault(); //PlatFormCreditRate.Get(model.PlatformCreditRateCode);
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
            Bill bill = db.Bills.Where(b => b.Code == model.Code && b.Status == 0).FirstOrDefault(); //Bill.Get(model.Code);
            _mapper.Map(model, bill);
            var shop = db.Shops.FirstOrDefault(i => i.Code == bill.ShopCode);
            bill.CustomerCode = shop.CustomerCode;
            bill.CustomerName = shop.CustomerName;
            if (bill != null)
            {
                bill.DateUpdated = DateTime.Now;
                bill.UpdatedBy = user.Name;
                if (model.ItemType == 1)
                {
                    bill.PackingCharge = model.PackingCharge1;
                }
                //Bill.Edit(bill, out int errorCode);
                bill.DateUpdated = DateTime.Now;
                db.Entry(bill).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("BillingChargeList");
        }

        [AccessPolicy(PageCode = "SHNBILDU006")]
        public ActionResult DeliveryUpdate(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dCode))
                return HttpNotFound();
            var bill = db.Bills.Where(b => b.Code ==dCode && b.Status == 0).FirstOrDefault();// Bill.Get(dCode);
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
            Bill bill = db.Bills.Where(b => b.Code == model.Code && b.Status == 0).FirstOrDefault(); //Bill.Get(model.Code);
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
                // Bill.Edit(bill, out int errorCode);
                bill.DateUpdated = DateTime.Now;
                db.Entry(bill).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("DeliveryChargeList");
        }

        [AccessPolicy(PageCode = "SHNBILBD007")]
        public ActionResult BillingDelete(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var bill = db.Bills.Where(b => b.Code == dCode && b.Status == 0).FirstOrDefault();//Bill.Get(dCode);
            if (bill != null)
            {
                bill.Status = 2;
                bill.DateUpdated = DateTime.Now;
                bill.UpdatedBy = user.Name;
                db.Entry(bill).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("BillingChargeList");
        }

        [AccessPolicy(PageCode = "SHNBILDD008")]
        public ActionResult DeliveryDelete(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var bill = db.Bills.Where(b => b.Code == dCode && b.Status == 0).FirstOrDefault(); //Bill.Get(dCode);
            if (bill != null)
            {
                bill.Status = 2;
                bill.DateUpdated = DateTime.Now;
                bill.UpdatedBy = user.Name;
                db.Entry(bill).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("DeliveryChargeList");
        }

        [AccessPolicy(PageCode = "SHNBILDD008")]
        public ActionResult DeliveryChargeDelete(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var bill = db.Bills.Where(b => b.Code == dCode && b.Status == 0).FirstOrDefault();
            if (bill != null)
            {
                bill.Status = 2;
                bill.DateUpdated = DateTime.Now;
                bill.UpdatedBy = user.Name;
                db.Entry(bill).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("DeliveryChargeAssignList");
        }

        [AccessPolicy(PageCode = "SHNBILDCAL09")]
        public ActionResult DeliveryChargeAssignList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DeliveryListViewModel();
            model.List = db.Bills.Where(i => i.NameOfBill == 0 && i.Status == 0 && i.ShopCode != null && i.ShopCode != "Admin").Select(i => new DeliveryListViewModel.DeliveryList
            {
                Code = i.Code,
                ShopCode = i.ShopCode,
                ShopName = i.ShopName,
                DeliveryChargeKM = i.DeliveryChargeKM,
                DeliveryChargeOneKM = i.DeliveryChargeOneKM,
                DeliveryRateSet = i.DeliveryRateSet,
                Type = i.Type
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
            bool isCheck = db.Bills.Any(i => i.ShopCode == model.ShopCode && i.NameOfBill == 0 && i.Status == 0);
            if (isCheck)
            {
                ViewBag.Message = "Delivery Charge Already Exist";
                return View();
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var bill = _mapper.Map<BillCreateEditViewModel, Bill>(model);
            bill.CustomerCode = user.Code;
            bill.CustomerName = user.Name;
            bill.CreatedBy = user.Name;
            bill.UpdatedBy = user.Name;
            bill.NameOfBill = 0;
            if (model.DeliveryRateSet == 0)
            {
                bill.DeliveryRateSet = model.DeliveryRateSet;
                // Bill.Add(bill, out int error);
                bill.Code = _generatedCode;
                bill.Status = 0;
                bill.DateEncoded = DateTime.Now;
                bill.DateUpdated = DateTime.Now;
                db.Bills.Add(bill);
                db.SaveChanges();
            }
            if (model.DeliveryRateSet1 == 1)
            {
                bill.DeliveryChargeKM = model.DeliveryChargeKM1;
                bill.DeliveryChargeOneKM = model.DeliveryChargeOneKM1;
                bill.DeliveryRateSet = model.DeliveryRateSet1;
                //Bill.Add(bill, out int error);
                bill.Code = _generatedCode;
                bill.Status = 0;
                bill.DateEncoded = DateTime.Now;
                bill.DateUpdated = DateTime.Now;
                db.Bills.Add(bill);
                db.SaveChanges();
            }
            
            return RedirectToAction("DeliveryChargeAssignList");
        }

        [AccessPolicy(PageCode = "SHNBILBC003")]
        public async Task<JsonResult> GetBillShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNBILDC004")]
        public JsonResult DeliveryChargeExist(int DeliveryRateSet, int type)
        {
            var GeneralCount = db.Bills.Where(i => i.NameOfBill == 0 && i.DeliveryRateSet == 0 && i.Status == 0 && i.Type == type).Count();
            var SpecialCount = db.Bills.Where(i => i.NameOfBill == 0 && i.DeliveryRateSet == 1 && i.Status == 0 && i.Type == type).Count();
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
        public async Task<JsonResult> GetDeliveryChargeType(int type)
        {
            var model = new DeliveryListViewModel();
            model.List = await db.Bills.Where(a => a.Type == type && a.Status == 0).Select(i => new DeliveryListViewModel.DeliveryList
            {
               Code = i.Code,
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