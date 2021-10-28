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
    public class ApiSettingController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;

        public ApiSettingController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<ApiSettingCreateViewModel, ApiSetting>();
                config.CreateMap<ApiSetting, ApiSettingEditViewModel>();
                config.CreateMap<ApiSettingEditViewModel, ApiSetting>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SNCAPISL030")]
        public ActionResult List()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new ApiSettingListViewModel();
            model.ListItems = db.ApiSettings.Where(i => i.Status == 0)
                .Select(i => new ApiSettingListViewModel.ListItem
                {
                  
                    Category = i.Category,
                    Id = i.Id,
                    OutletId = i.OutletId,
                    ProviderName = i.ProviderName,
                    ShopId = i.ShopId,
                    ShopName = i.ShopName,
                    Url = i.Url,
                    Version = i.Version
                }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCAPISC031")]
        public ActionResult Create()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [AccessPolicy(PageCode = "SNCAPISC031")]
        [HttpPost]
        public ActionResult Create(ApiSettingCreateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var apiSetting = _mapper.Map<ApiSettingCreateViewModel, ApiSetting>(model);
            apiSetting.CreatedBy = user.Name;
            apiSetting.UpdatedBy = user.Name;
            apiSetting.DateEncoded = DateTime.Now;
            apiSetting.DateUpdated = DateTime.Now;
            apiSetting.Status = 0;
            db.ApiSettings.Add(apiSetting);
            db.SaveChanges();
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SNCAPISE032")]
        public ActionResult Edit(string id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            int dId = AdminHelpers.DCodeInt(id);
            var apiSetting = db.ApiSettings.FirstOrDefault(i => i.Id == dId);
            var model = _mapper.Map<ApiSetting, ApiSettingEditViewModel>(apiSetting);
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCAPISE032")]
        [HttpPost]
        public ActionResult Edit(ApiSettingEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var apiSetting = db.ApiSettings.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, apiSetting);
            apiSetting.DateUpdated = DateTime.Now;
            apiSetting.UpdatedBy = user.Name;
            db.Entry(apiSetting).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = AdminHelpers.ECodeInt(model.Id) });
        }

        [AccessPolicy(PageCode = "SNCAPISD033")]
        public JsonResult Delete(string id)
        {
            int dId = AdminHelpers.DCodeInt(id);
            var apiSetting = db.ApiSettings.FirstOrDefault(i => i.Id == dId);
            if (apiSetting != null)
            {
                apiSetting.Status = 2;
                db.Entry(apiSetting).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}