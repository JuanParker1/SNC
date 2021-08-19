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
    public class AchievementSettingController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;

        public AchievementSettingController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<AchievementSettingCreateViewModel, AchievementSetting>();
                config.CreateMap<AchievementSetting, AchievementSettingEditViewModel>();
                config.CreateMap<AchievementSettingEditViewModel, AchievementSetting>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult List()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new AchievementSettingListViewModel();
            model.ListItems = db.AchievementSettings.Where(i => i.Status == 0)
                .Select(i => new AchievementSettingListViewModel.ListItem
                {
                    Amount = i.Amount,
                    CountType = i.CountType,
                    CountValue = i.CountValue,
                    Id = i.Id,
                    Name = i.Name,
                    ShopDistrict = i.ShopDistrict
                }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult Create()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [AccessPolicy(PageCode = "")]
        [HttpPost]
        public ActionResult Create(AchievementSettingCreateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var achievementSetting = _mapper.Map<AchievementSettingCreateViewModel, AchievementSetting>(model);
            achievementSetting.DateEncoded = DateTime.Now;
            achievementSetting.DateUpdated = DateTime.Now;
            achievementSetting.Status = 0;
            db.AchievementSettings.Add(achievementSetting);
            db.SaveChanges();
            if (achievementSetting != null && model.ShopIds != null)
            {
                foreach (var item in model.ShopIds)
                {
                    var achievementShop = new AchievementShop();
                    achievementShop.ShopId = item;
                    achievementShop.AchievementId = achievementSetting.Id;
                    db.AchievementShops.Add(achievementShop);
                    db.SaveChanges();
                }
            }
            if (achievementSetting != null && model.ProductIds != null)
            {
                foreach (var item in model.ProductIds)
                {
                    var achievementProduct = new AchievementProduct();
                    achievementProduct.ProductId = item;
                    achievementProduct.AchievementId = achievementSetting.Id;
                    db.AchievementProducts.Add(achievementProduct);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult Delete(string id)
        {
            int dId = AdminHelpers.DCodeInt(id);
            var achievementSetting = db.AchievementSettings.FirstOrDefault(i => i.Id == dId);
            if (achievementSetting != null)
            {
                achievementSetting.Status = 2;
                db.Entry(achievementSetting).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult Edit(string id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            int dId = AdminHelpers.DCodeInt(id);
            var achievementSetting = db.AchievementSettings.FirstOrDefault(i => i.Id == dId);
            var model = _mapper.Map<AchievementSetting, AchievementSettingEditViewModel>(achievementSetting);

            if (model.ActivateAfterId != 0)
                model.ActivateAfterName = db.AchievementSettings.FirstOrDefault(i => i.Id == model.ActivateAfterId)?.Name;

            var achievementShops = db.AchievementShops.Where(i => i.AchievementId == dId).ToList();
            if (achievementShops.Count > 0)
            {
                model.ShopIds = achievementShops.Select(i => i.ShopId).ToArray();
                model.ShopIdstring = string.Join(",", achievementShops.Select(i => i.ShopId));
                model.ShopNames = string.Join(",", db.Shops.Where(i => model.ShopIds.Contains(i.Id)).Select(i => i.Name).ToList());
            }

            var achievementProducts = db.AchievementProducts.Where(i => i.AchievementId == dId).ToList();
            if (achievementProducts.Count > 0)
            {
                model.ProductIds = achievementProducts.Select(i => i.ProductId).ToArray();
                model.ProductIdstring = string.Join(",", achievementProducts.Select(i => i.ProductId));
                model.ProductNames = string.Join(",", db.Products.Where(i => model.ProductIds.Contains(i.Id)).Select(i => i.Name).ToList());
            }
            return View(model);
        }

        [AccessPolicy(PageCode = "")]
        [HttpPost]
        public ActionResult Edit(AchievementSettingEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var achievementSetting = db.AchievementSettings.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, achievementSetting);
            achievementSetting.DateUpdated = DateTime.Now;
            db.Entry(achievementSetting).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = AdminHelpers.ECodeInt(model.Id) });
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetListSelect2(string q = "")
        {
            var model = await db.AchievementSettings.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }
    }
}