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
                config.CreateMap<Offer, OfferEditViewModel>();
                config.CreateMap<OfferEditViewModel,Offer>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult List()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
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
                    Type = i.Type,
                    Id = i.Id
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
            if (offer != null && model.ProductIds != null)
            {
                foreach (var item in model.ProductIds)
                {
                    var offerproduct = new OfferProduct();
                    offerproduct.ProductId = item;
                    offerproduct.OfferId = offer.Id;
                    db.OfferProducts.Add(offerproduct);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult Delete(string id)
        {
            int dId = AdminHelpers.DCodeInt(id);
            var offer = db.Offers.FirstOrDefault(i => i.Id == dId);
            if(offer != null)
            {            
                offer.Status = 2;
                db.Entry(offer).State = System.Data.Entity.EntityState.Modified;
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
            var offer = db.Offers.FirstOrDefault(i => i.Id == dId);
            var model = _mapper.Map<Offer, OfferEditViewModel>(offer);

            if (model.BrandId != 0)
                model.BrandName = db.Brands.FirstOrDefault(i => i.Id == model.BrandId)?.Name;

            var offerShops = db.OfferShops.Where(i => i.OfferId == dId).ToList();
            if (offerShops.Count > 0)
            {
                model.ShopIds = offerShops.Select(i => i.ShopId).ToArray();
                model.ShopIdstring = string.Join(",", offerShops.Select(i => i.ShopId));
                model.ShopNames = string.Join(",", db.Shops.Where(i => model.ShopIds.Contains(i.Id)).Select(i => i.Name).ToList());
            }

            var offerProducts = db.OfferProducts.Where(i => i.OfferId == dId).ToList();
            if (offerProducts.Count > 0)
            {
                model.ProductIds = offerProducts.Select(i => i.ProductId).ToArray();
                model.ProductIdstring = string.Join(",", offerProducts.Select(i => i.ProductId));
                model.ProductNames = string.Join(",", db.Products.Where(i => model.ProductIds.Contains(i.Id)).Select(i => i.Name).ToList());
            }
            return View(model);
        }

        [AccessPolicy(PageCode = "")]
        [HttpPost]
        public ActionResult Edit(OfferEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var offer = _mapper.Map<OfferEditViewModel, Offer>(model);
            offer.DateUpdated = DateTime.Now;
            db.Entry(offer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = AdminHelpers.ECodeInt(model.Id) });
        }
    }
}