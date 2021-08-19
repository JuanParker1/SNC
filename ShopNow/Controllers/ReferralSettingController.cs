using AutoMapper;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class ReferralSettingController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;

        public ReferralSettingController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<ReferralSettingCreateViewModel, ReferralSetting>();
                config.CreateMap<ReferralSetting, ReferralSettingEditViewModel>();
                config.CreateMap<ReferralSettingEditViewModel, ReferralSetting>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult Index()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new ReferralSettingIndexViewModel();
            model.ListItems = db.ReferralSettings.Where(i => i.Status == 0)
            .Select(i => new ReferralSettingIndexViewModel.ListItem
            {
                Amount = i.Amount,
                Id = i.Id,
                PaymentMode = i.PaymentMode,
                ShopDistrict = i.ShopDistrict
            }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "")]
        [HttpPost]
        public ActionResult Create(ReferralSettingCreateViewModel model)
        {
            var isExist = db.ReferralSettings.Any(i => i.ShopDistrict == model.ShopDistrict && i.PaymentMode == model.PaymentMode);
            if (!isExist)
            {
                var referralSetting = _mapper.Map<ReferralSettingCreateViewModel, ReferralSetting>(model);
                referralSetting.DateEncoded = DateTime.Now;
                referralSetting.DateUpdated = DateTime.Now;
                referralSetting.Status = 0;
                db.ReferralSettings.Add(referralSetting);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult Delete(string id)
        {
            int dId = AdminHelpers.DCodeInt(id);
            var referralSetting = db.ReferralSettings.FirstOrDefault(i => i.Id == dId);
            if (referralSetting != null)
            {
                referralSetting.Status = 2;
                db.Entry(referralSetting).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        [HttpPost]
        public ActionResult Edit(ReferralSettingEditViewModel model)
        {
            var isExist = db.ReferralSettings.Any(i => i.ShopDistrict == model.ShopDistrict && i.PaymentMode == model.PaymentMode);
            if (!isExist)
            {
                var referralSetting = db.ReferralSettings.FirstOrDefault(i => i.Id == model.Id);
                _mapper.Map(model, referralSetting);
                referralSetting.DateUpdated = DateTime.Now;
                db.Entry(referralSetting).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }

}