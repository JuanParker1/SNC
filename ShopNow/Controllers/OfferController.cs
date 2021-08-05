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
    public class OfferController : Controller
    {
        private sncEntities db = new sncEntities();

        [AccessPolicy(PageCode = "")]
        public ActionResult List()
        {
            var model = new OfferListViewModel();
            model.ListItems = db.Offers.Where(i => i.Status == 0)
                .Select(i => new OfferListViewModel.ListItem
                {
                    AmountLimit = i.AmountLimit,
                    CustomerCountLimit = i.CustomerCountLimit,
                    DiscountType = i.DiscountType,
                    IsForBlackListAbusers = i.IsForBlackListAbusers,
                    IsForFirstOrder = i.IsForFirstOrder,
                    IsForOnlinePayment = i.IsForOnlinePayment,
                    Name = i.Name,
                    OwnerType = i.OwnerType,
                    Percentage = i.Percentage,
                    QuantityLimit = i.QuantityLimit,
                    Type = i.Type
                }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult Create()
        {
            return View();
        }
    }
}