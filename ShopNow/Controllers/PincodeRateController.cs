using AutoMapper;
using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class PincodeRateController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;

        public PincodeRateController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<PincodeRate, PincodeRateListViewModel.PincodeRateList>();

            });
            _mapper = _mapperConfiguration.CreateMapper();

        }

        [AccessPolicy(PageCode = "SNCPCL192")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new PincodeRateListViewModel();
            model.List = db.Shops.Where(i => i.Status == 0 && !string.IsNullOrEmpty(i.PinCode)).GroupBy(i => i.PinCode)
                .GroupJoin(db.PincodeRates, s => s.FirstOrDefault().PincodeRateId, p => p.Id, (s, p) => new { s, p })
                .Select(i => new PincodeRateListViewModel.PincodeRateList
                {
                    Pincode = i.s.FirstOrDefault().PinCode,
                    Id = i.p.Any() ? i.p.FirstOrDefault().Id : 0,
                    Remarks = i.p.Any() ? i.p.FirstOrDefault().Remarks : "",
                    Tier = i.s.FirstOrDefault().DeliveryTierType,
                    Type = i.s.FirstOrDefault().DeliveryType,
                }).ToList();
            return View(model);
        }

        public JsonResult AddUpdate(int id, int type, int tier, string remarks, string pincode)
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        
        //[AccessPolicy(PageCode = "SNCPCU193")]
        //public JsonResult Update(string pincode, int deliveryRateSet, string remarks)
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    string message = "";
        //    var pincodeRate = new PincodeRate();
        //    pincodeRate.Pincode = pincode;
        //    pincodeRate.Remarks = remarks;
        //    pincodeRate.CreatedBy = user.Name;
        //    pincodeRate.UpdatedBy = user.Name;
        //    pincodeRate.Status = 0;
        //    pincodeRate.DateEncoded = DateTime.Now;
        //    pincodeRate.DateUpdated = DateTime.Now;
        //    db.PincodeRates.Add(pincodeRate);
        //    db.SaveChanges();
        //    message = pincode + " Successfully Updated";

        //    var shopList = db.Shops.Where(i => i.Status == 0 && i.PinCode == pincode).ToList();
        //    if (shopList != null)
        //    {
        //        foreach(var s in shopList)
        //        {
        //            var shop = db.Shops.FirstOrDefault(i => i.Id == s.Id);// Shop.Get(s.Code);
        //            if (shop != null)
        //            {
        //                shop.PincodeRateId = pincodeRate.Id;
        //                //shop.PincodeRateDeliveryRateSet = deliveryRateSet;
        //                shop.DateUpdated = DateTime.Now;

        //                shop.DateUpdated = DateTime.Now;
        //                db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
        //                db.SaveChanges();
        //                // Shop.Edit(shop, out int errorCode);
        //            }
        //        }
        //    }

        //    return Json(new {message = message }, JsonRequestBehavior.AllowGet);
        //}

    }
}