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
        private ShopnowchatEntities db = new ShopnowchatEntities();
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

        [AccessPolicy(PageCode = "SHNPRTL001")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new PincodeRateListViewModel();
            model.List = db.Shops.Where(i=>i.Status == 0).GroupBy(i => new { PinCode = i.PinCode, PincodeRateDeliveryRateSet = i.PincodeRateDeliveryRateSet })
                .Select(i => new PincodeRateListViewModel.PincodeRateList
            {
                PinCode = i.Key.PinCode,
                PincodeRateDeliveryRateSet = i.Key.PincodeRateDeliveryRateSet
            }).ToList();
             
            return View(model);
        }
        private const string _prefix = "PRT";

        private static string _generatedCode
        {
            get
            {
                return ShopNow.Helpers.DRC.Generate(_prefix);
            }
        }
        [AccessPolicy(PageCode = "SHNPRTU002")]
        public JsonResult Update(string pincode, int deliveryRateSet, int remarks)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            string message = "";
            var pincodeRate = new PincodeRate();
            pincodeRate.PinCode = pincode;
            pincodeRate.DeliveryRateSet = deliveryRateSet;
            pincodeRate.Remarks = remarks;
            pincodeRate.CreatedBy = user.Name;
            pincodeRate.UpdatedBy = user.Name;
            // string code = PincodeRate.Add(pincodeRate, out int error);
            pincodeRate.Code = _generatedCode;
            pincodeRate.Status = 0;
            pincodeRate.DateEncoded = DateTime.Now;
            pincodeRate.DateUpdated = DateTime.Now;
            db.PincodeRates.Add(pincodeRate);
            db.SaveChanges();
            string code = pincodeRate.Code;
            message = pincode + " Successfully Updated";

            var shopList = db.Shops.Where(i => i.Status == 0 && i.PinCode == pincode).ToList();
            if (shopList != null)
            {
                foreach(var s in shopList)
                {
                    var shop = db.Shops.FirstOrDefault(i => i.Code == s.Code);// Shop.Get(s.Code);
                    if (shop != null)
                    {
                        shop.PincodeRateCode = code;
                        shop.PincodeRateDeliveryRateSet = deliveryRateSet;
                        shop.DateUpdated = DateTime.Now;

                        shop.DateUpdated = DateTime.Now;
                        db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        // Shop.Edit(shop, out int errorCode);
                    }
                }
            }

            return Json(new {message = message }, JsonRequestBehavior.AllowGet);
        }

    }
}