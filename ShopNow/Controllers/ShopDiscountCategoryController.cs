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
using System.Web.Mvc;
namespace ShopNow.Controllers
{
    public class ShopDiscountCategoryController : Controller
    {
        private ShopnowchatEntities db = new ShopnowchatEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private const string _prefix = "DCA";

        private static string _generatedCode
        {
            get
            {
                return ShopNow.Helpers.DRC.Generate(_prefix);
            }
        }
        public ShopDiscountCategoryController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<ShopDiscountCategoryViewModel, Product>();
                config.CreateMap<Product, ShopDiscountCategoryViewModel.DiscountCategoryList>();
                config.CreateMap<DiscountCategory, ShopDiscountCategoryViewModel.CategoryList>();
                config.CreateMap<DiscountCategory, ShopDiscountCategoryEditViewModel>();
                config.CreateMap<ShopDiscountCategoryEditViewModel, DiscountCategory>();
            });
            _mapper = _mapperConfiguration.CreateMapper();

        }

        [AccessPolicy(PageCode = "SHNSDCL001")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new ShopDiscountCategoryViewModel();
            model.CategoryLists = db.DiscountCategories.Where(i=> i.Status == 0)
                   .Select(i => new ShopDiscountCategoryViewModel.CategoryList
                   {
                       Id = i.Id,
                       CategoryName = i.Name,
                       DiscountPercentage = i.Percentage,
                   }).ToList();

            return View(model.CategoryLists);
        }

        [AccessPolicy(PageCode = "SHNSDCC002")]
        public ActionResult Create()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNSDCC002")]
        public ActionResult Create(ShopDiscountCategoryViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _mapper.Map<ShopDiscountCategoryViewModel, Product >(model);
        
            return RedirectToAction("List");

        }

        [AccessPolicy(PageCode = "SHNSDCE003")]
        public ActionResult Edit(string code)
        {
            var dCode = AdminHelpers.DCodeInt(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dc = db.DiscountCategories.FirstOrDefault(i => i.Id == dCode); // DiscountCategory.Get(dCode);
            var model = _mapper.Map<DiscountCategory, ShopDiscountCategoryEditViewModel>(dc);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNSDCE003")]
        public ActionResult Edit(ShopDiscountCategoryEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var discountcategory = db.DiscountCategories.FirstOrDefault(i => i.Id == model.Id); // DiscountCategory.Get(model.Code);
            var product = db.Products.Where(i => i.DiscountCategoryName == discountcategory.Name && i.Status == 0).ToList();
            //var dc = _mapper.Map<ShopDiscountCategoryEditViewModel, DiscountCategory>(model);

            discountcategory.Percentage = model.Percentage;
            discountcategory.UpdatedBy = user.Name;
            discountcategory.DateUpdated = DateTime.Now;
            db.Entry(discountcategory).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
           // DiscountCategory.Edit(discountcategory, out int error1);
            if (product != null)
            {
                foreach (var item in product)
                {
                    var prod = db.Products.FirstOrDefault(i => i.Id == item.Id); // Product.Get(item.Code);
                    if (prod != null)
                    {
                        //prod.DiscountCategoryId = discountcategory.Id;
                        prod.DiscountCategoryName = discountcategory.Name;
                       // prod.DiscountCategoryPercentage = discountcategory.Percentage;
                        //prod.DiscountCategoryType = discountcategory.CategoryType;
                        //prod.DiscountType = discountcategory.Type;
                        prod.UpdatedBy = user.Name;
                        prod.DateUpdated = DateTime.Now;
                        db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        //Product.Edit(prod, out int error);
                    }
                }
            }
            
            return RedirectToAction("List");

        }

        //[AccessPolicy(PageCode = "SHNSDCC002")]
        //public JsonResult GetDiscountCategory(string category, string type)
        //{
        //    var model = new List<ShopDiscountCategoryViewModel.DiscountCategoryList>();
        //    if (category == "1")
        //    {
        //        model = db.Products.Where(i => i.DiscountCategoryType == 1 && i.Status == 0)
        //            .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
        //            {
        //                Id = i.Id,
        //                CategoryNameMain = i.CategoryNameMain,
        //                DiscountCategoryId = i.DiscountCategoryId,
        //                DiscountCategoryName = i.DiscountCategoryName,
        //                DiscountCategoryPercentage = i.DiscountCategoryPercentage,
        //                DiscountCategoryType = i.DiscountCategoryType,
        //                DiscountType = i.DiscountType
        //            }).ToList();
        //    }
        //    if (category == "2")
        //    {
        //        model = db.Products.Where(i => i.DiscountCategoryType == 2 && i.Status == 0)
        //            .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
        //            {
        //                Id = i.Id,
        //                CategoryNameMain = i.CategoryNameMain,
        //                DiscountCategoryId = i.DiscountCategoryId,
        //                DiscountCategoryName = i.DiscountCategoryName,
        //                DiscountCategoryPercentage = i.DiscountCategoryPercentage,
        //                DiscountCategoryType = i.DiscountCategoryType,
        //                DiscountType = i.DiscountType
        //            }).ToList();
        //    }
        //    if (category == "3")
        //    {
        //        model = db.Products.Where(i => i.DiscountCategoryType == 3 && i.Status == 0)
        //           .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
        //           {
        //               Id = i.Id,
        //               CategoryNameMain = i.CategoryNameMain,
        //               DiscountCategoryId = i.DiscountCategoryId,
        //               DiscountCategoryName = i.DiscountCategoryName,
        //               DiscountCategoryPercentage = i.DiscountCategoryPercentage,
        //               DiscountCategoryType = i.DiscountCategoryType,
        //               DiscountType = i.DiscountType
        //           }).ToList();
        //    }
        //    if (category == "4")
        //    {
        //        model = db.Products.Where(i => i.DiscountCategoryType == 4 && i.Status == 0)
        //           .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
        //           {
        //               Id = i.Id,
        //               CategoryNameMain = i.CategoryNameMain,
        //               DiscountCategoryId = i.DiscountCategoryId,
        //               DiscountCategoryName = i.DiscountCategoryName,
        //               DiscountCategoryPercentage = i.DiscountCategoryPercentage,
        //               DiscountCategoryType = i.DiscountCategoryType,
        //               DiscountType = i.DiscountType
        //           }).ToList();
        //    }
        //    if (category == "5")
        //    {
        //        model = db.Products.Where(i => i.DiscountCategoryType == 5 && i.Status == 0)
        //            .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
        //            {
        //                Id = i.Id,
        //                CategoryNameMain = i.CategoryNameMain,
        //                DiscountCategoryId = i.DiscountCategoryId,
        //                DiscountCategoryName = i.DiscountCategoryName,
        //                DiscountCategoryPercentage = i.DiscountCategoryPercentage,
        //                DiscountCategoryType = i.DiscountCategoryType,
        //                DiscountType = i.DiscountType
        //            }).ToList();
        //    }
        //    if (category == "6")
        //    {
        //        model = db.Products.Where(i => i.DiscountCategoryType == 6 && i.Status == 0)
        //            .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
        //            {
        //                Id = i.Id,
        //                CategoryNameMain = i.CategoryNameMain,
        //                DiscountCategoryId = i.DiscountCategoryId,
        //                DiscountCategoryName = i.DiscountCategoryName,
        //                DiscountCategoryPercentage = i.DiscountCategoryPercentage,
        //                DiscountCategoryType = i.DiscountCategoryType,
        //                DiscountType = i.DiscountType
        //            }).ToList();
        //    }
        //    if (category == "7")
        //    {
        //        model = db.Products.Where(i => i.DiscountCategoryType == 7 && i.Status == 0)
        //           .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
        //           {
        //               Id = i.Id,
        //               CategoryNameMain = i.CategoryNameMain,
        //               DiscountCategoryId = i.DiscountCategoryId,
        //               DiscountCategoryName = i.DiscountCategoryName,
        //               DiscountCategoryPercentage = i.DiscountCategoryPercentage,
        //               DiscountCategoryType = i.DiscountCategoryType,
        //               DiscountType = i.DiscountType
        //           }).ToList();
        //    }
        //    if (category == "8")
        //    {
        //        model = db.Products.Where(i => i.DiscountCategoryType == 8 && i.Status == 0)
        //            .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
        //            {
        //                Id = i.Id,
        //                CategoryNameMain = i.CategoryNameMain,
        //                DiscountCategoryId = i.DiscountCategoryId,
        //                DiscountCategoryName = i.DiscountCategoryName,
        //                DiscountCategoryPercentage = i.DiscountCategoryPercentage,
        //                DiscountCategoryType = i.DiscountCategoryType,
        //                DiscountType = i.DiscountType
        //            }).ToList();
        //    }
        //    if (category == "9")
        //    {
        //        model = db.Products.Where(i => i.DiscountCategoryType == 9 && i.Status == 0)
        //           .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
        //           {
        //               Id = i.Id,
        //               CategoryNameMain = i.CategoryNameMain,
        //               DiscountCategoryId = i.DiscountCategoryId,
        //               DiscountCategoryName = i.DiscountCategoryName,
        //               DiscountCategoryPercentage = i.DiscountCategoryPercentage,
        //               DiscountCategoryType = i.DiscountCategoryType,
        //               DiscountType = i.DiscountType
        //           }).ToList();
        //    }
        //    if (category == "10")
        //    {
        //        model = db.Products.Where(i => i.DiscountCategoryType == 10 && i.Status == 0)
        //           .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
        //           {
        //               Id = i.Id,
        //               CategoryNameMain = i.CategoryNameMain,
        //               DiscountCategoryId = i.DiscountCategoryId,
        //               DiscountCategoryName = i.DiscountCategoryName,
        //               DiscountCategoryPercentage = i.DiscountCategoryPercentage,
        //               DiscountCategoryType = i.DiscountCategoryType,
        //               DiscountType = i.DiscountType
        //           }).ToList();
        //    }

        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}

        //[AccessPolicy(PageCode = "SHNSDCC002")]
        //public JsonResult UpdateDiscountCategory(string offercode, string offername, int type, int category, double discount, string shopcode, string shopname)
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    var discountcategory = db.DiscountCategories.FirstOrDefault(i => i.OfferCategoryCode == offercode); // DiscountCategory.GetOfferCategoryCode(offercode);
        //    DiscountCategory dc = new DiscountCategory();
        //    if (type == 1 || type == 2)
        //    {
        //        if (discountcategory == null)
        //        {
        //            dc.Name = offername;
        //            dc.OfferCategoryCode = offercode;
        //            dc.Percentage = discount;
        //            dc.Type = type;
        //            dc.CategoryType = category;
        //            dc.CreatedBy = user.Name;
        //            dc.UpdatedBy = user.Name;
        //            dc.ShopCode = shopcode;
        //            dc.ShopName = shopname;
        //            dc.Code = _generatedCode;
        //            dc.Status = 0;
        //            dc.DateEncoded = DateTime.Now;
        //            dc.DateUpdated = DateTime.Now;
        //            db.DiscountCategories.Add(dc);
        //            db.SaveChanges();
        //            //dc.Code = DiscountCategory.Add(dc, out int error);
        //        }
        //        else
        //        {
        //            discountcategory.Name = offername;
        //            discountcategory.OfferCategoryCode = offercode;
        //            discountcategory.Percentage = discount;
        //            discountcategory.Type = type;
        //            discountcategory.CategoryType = category;
        //            discountcategory.CreatedBy = user.Name;
        //            discountcategory.UpdatedBy = user.Name;
        //            discountcategory.ShopCode = shopcode;
        //            discountcategory.ShopName = shopname;
        //            db.Entry(discountcategory).State = System.Data.Entity.EntityState.Modified;
        //            db.SaveChanges();
        //            // DiscountCategory.Edit(discountcategory, out int error1);
        //        }
        //        var pms = db.Products.Where(i => i. == offercode && i.Status == 0).ToList();
        //        if (pms != null)
        //        {
        //            foreach (var item in pms)
        //            {
        //                var product = db.Products.FirstOrDefault(i => i.Id == item.productid); // Product.Get(item.ProductCode);
        //                if (product != null)
        //                {
        //                    var shop = db.Shops.FirstOrDefault(i => i.Code == product.ShopCode); // Shop.Get(product.ShopCode);
        //                    if (shop != null)
        //                    {
        //                        if (dc.Code != null)
        //                        {
        //                            product.DiscountCategoryCode = dc.Code;
        //                        }
        //                        else
        //                        {
        //                            product.DiscountCategoryCode = discountcategory.Code;
        //                        }
        //                        product.DiscountCategoryName = offername;
        //                        product.DiscountCategoryPercentage = discount;
        //                        product.DiscountCategoryType = category;
        //                        product.DiscountType = type;
        //                        product.UpdatedBy = user.Name;
        //                        product.DateUpdated = DateTime.Now;
        //                        db.Entry(product).State = System.Data.Entity.EntityState.Modified;
        //                        db.SaveChanges();
        //                       // Product.Edit(product, out int errorcode);
        //                    }
        //                    var pms1 = db.ProductMedicalStocks.FirstOrDefault(i => i.productid == product.Id); // ProductMedicalStock.GetProduct(product.Code);
        //                    if (pms1 != null)
        //                    {
        //                        pms1.DiscountPercentage = discountcategory.Percentage;
        //                        pms1.DateUpdated = DateTime.Now;
        //                        db.Entry(pms1).State = System.Data.Entity.EntityState.Modified;
        //                        db.SaveChanges();
        //                       // ProductMedicalStock.Edit(pms1, out int error2);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else if(type == 3)
        //    {
        //        if (discountcategory == null)
        //        {
        //            dc.Name = offername;
        //            dc.OfferCategoryCode = offercode;
        //            dc.Percentage = 0.0;
        //            dc.Type = type;
        //            dc.CategoryType = category;
        //            dc.CreatedBy = user.Name;
        //            dc.UpdatedBy = user.Name;
        //            dc.ShopCode = shopcode;
        //            dc.ShopName = shopname;
        //            dc.Code = _generatedCode;
        //            dc.Status = 0;
        //            dc.DateEncoded = DateTime.Now;
        //            dc.DateUpdated = DateTime.Now;
        //            db.DiscountCategories.Add(dc);
        //            db.SaveChanges();
        //           // dc.Code = DiscountCategory.Add(dc, out int error);
        //        }
        //        else
        //        {
        //            discountcategory.Name = offername;
        //            discountcategory.OfferCategoryCode = offercode;
        //            discountcategory.Percentage = 0.0;
        //            discountcategory.Type = type;
        //            discountcategory.CategoryType = category;
        //            discountcategory.CreatedBy = user.Name;
        //            discountcategory.UpdatedBy = user.Name;
        //            discountcategory.ShopCode = shopcode;
        //            discountcategory.ShopName = shopname;
        //            db.Entry(discountcategory).State = System.Data.Entity.EntityState.Modified;
        //            db.SaveChanges();
        //           // DiscountCategory.Edit(discountcategory, out int error1);
        //        }
        //        var pms = db.ProductMedicalStocks.Where(i => i.OfferCategoryCode == offercode && i.Status == 0).ToList();
        //        if (pms != null)
        //        {
        //            foreach (var item in pms)
        //            {
        //                var product = db.Products.FirstOrDefault(i => i.Id == item.productid); // Product.Get(item.ProductCode);
        //                if (product != null)
        //                {
        //                    var shop = db.Shops.FirstOrDefault(i => i.Code == product.ShopCode); // Shop.Get(product.ShopCode);
        //                    if (shop != null)
        //                    {
        //                        if (dc.Code != null)
        //                        {
        //                            product.DiscountCategoryCode = dc.Code;
        //                        }
        //                        else
        //                        {
        //                            product.DiscountCategoryCode = discountcategory.Code;
        //                        }
        //                        product.DiscountCategoryName = offername;
        //                        product.DiscountCategoryPercentage = 0.0;
        //                        product.DiscountCategoryType = category;
        //                        product.DiscountType = type;
        //                        product.UpdatedBy = user.Name;
        //                        product.DateUpdated = DateTime.Now;
        //                        db.Entry(product).State = System.Data.Entity.EntityState.Modified;
        //                        db.SaveChanges();
        //                        //Product.Edit(product, out int errorcode);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        [AccessPolicy(PageCode = "SHNSDCC002")]
        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
