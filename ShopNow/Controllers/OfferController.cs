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

        [AccessPolicy(PageCode = "SNCOL167")]
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
                    Id = i.Id,
                    OfferCode = i.OfferCode
                }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCOC168")]
        public ActionResult Create()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [AccessPolicy(PageCode = "SNCOC168")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(OfferCreateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            int isShopOfferCount = 0;
            bool isOfferExist = db.Offers.Any(i => i.OfferCode.Trim() == model.OfferCode.Trim() && i.Status == 0);
            if (model.OwnerType == 1 || model.OwnerType == 2)
                isShopOfferCount = db.Offers.Where(i => i.Status == 0 && (i.Type == 1 || i.Type == 3))
                    .Join(db.OfferShops.Where(i => model.ShopIds.Contains(i.ShopId)), o => o.Id, os => os.OfferId, (o, os) => new { o, os })
                    .Count();
            else
                isShopOfferCount = db.Offers.Where(i => i.Status == 0 )//&& (i.Type == 1 || i.Type == 3) && i.OwnerType == 3)
                       //.Join(db.OfferShops.Where(i => model.ShopIds.Contains(i.ShopId)), o => o.Id, os => os.OfferId, (o, os) => new { o, os })
                       .Count();
            if (!isOfferExist)
            {
                if (isShopOfferCount == 0)
                {
                    var offer = _mapper.Map<OfferCreateViewModel, Offer>(model);
                    offer.DateEncoded = DateTime.Now;
                    offer.DateUpdated = DateTime.Now;
                    offer.CreatedBy = user.Name;
                    offer.UpdatedBy = user.Name;
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
                }
                else
                {
                    ViewBag.ErrorMessage = "Offer already Exist!";
                    return View();
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Offer Code already Exist!";
                return View();
            }
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SNCOE169")]
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

                if (offer.Type == 3)
                {
                    model.SingleProductId = offerProducts.FirstOrDefault().ProductId;
                    model.SingleProductName = db.Products.FirstOrDefault(i => i.Id == model.SingleProductId).Name;
                }
            }
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCOE169")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(OfferEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var offer = db.Offers.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, offer);
            if (offer != null && model.ShopIds != null)
            {
                foreach (var item in model.ShopIds)
                {
                    var os = db.OfferShops.Where(i => i.ShopId == item).FirstOrDefault();
                    if (os == null)
                    {
                        var offershop = new OfferShop();
                        offershop.ShopId = item;
                        offershop.OfferId = offer.Id;
                        db.OfferShops.Add(offershop);
                        db.SaveChanges();
                    }
                }
            }
            offer.DateUpdated = DateTime.Now;
            offer.UpdatedBy = user.Name;
            db.Entry(offer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = AdminHelpers.ECodeInt(model.Id) });
        }

        [AccessPolicy(PageCode = "SNCOD170")]
        public JsonResult Delete(string id)
        {
            int dId = AdminHelpers.DCodeInt(id);
            var offer = db.Offers.FirstOrDefault(i => i.Id == dId);
            if (offer != null)
            {
                offer.Status = 2;
                db.Entry(offer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDistrictSelect2(string q = "")
        {
            var model = await db.Shops.Where(a => a.DistrictName.Contains(q) && a.Status == 0).GroupBy(i => i.DistrictName).Select(i => new
            {
                id = i.Key,
                text = i.Key
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetShopByDistrictSelect2(string district, string q = "")
        {
            int[] shopidarray = db.Shops.Where(i => i.Status == 0 && i.DistrictName == district).Select(i => i.Id).ToArray();
            var model = await db.Shops.Where(a => a.Name.Contains(q) && a.Status == 0 && a.DistrictName == district).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name,
                shopid = shopidarray
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, result = shopidarray, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

    }
}