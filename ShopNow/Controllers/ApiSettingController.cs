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
    public class ApiSettingController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;

        public ApiSettingController()
        {

        }

        [AccessPolicy(PageCode = "")]
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

        [AccessPolicy(PageCode = "")]
        public ActionResult Create()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }
    }
}