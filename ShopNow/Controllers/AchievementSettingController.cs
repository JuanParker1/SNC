using AutoMapper;
using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}