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
    public class OfferController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;

        public OfferController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<OfferCreateViewModel, Offer>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

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

        [AccessPolicy(PageCode = "")]
        [HttpPost]
        public ActionResult Create(OfferCreateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var offer = _mapper.Map<OfferCreateViewModel, Offer>(model);
            offer.DateEncoded = DateTime.Now;
            offer.DateUpdated = DateTime.Now;
            offer.Status = 0;
            db.Offers.Add(offer);
            db.SaveChanges();
            if (offer != null && model.ShopIds != null)
            {
                foreach (var item in model.ShopIds)
                {
                    var offershop = new OfferShop();
                    offershop.ShopId = item;
                    offershop.OfferId = offer.Id;
                    db.OfferShops.Add(offershop);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("List");
        }
    }
}