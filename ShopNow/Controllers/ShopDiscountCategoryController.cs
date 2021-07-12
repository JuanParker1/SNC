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
                config.CreateMap<ShopDiscountCategoryViewModel, ProductMedicalStock>();
                config.CreateMap<ProductMedicalStock, ShopDiscountCategoryViewModel.DiscountCategoryList>();
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
            model.CategoryLists = db.DiscountCategories.Where(i=> i.Status == 0 && i.Type != 0)
                   .Select(i => new ShopDiscountCategoryViewModel.CategoryList
                   {
                       Code = i.Code,
                       CategoryCode = i.OfferCategoryCode,
                       CategoryName = i.Name,
                       DiscountPercentage = i.Percentage,
                       Type = i.Type,
                       CategoryType = i.CategoryType
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
            var prod = _mapper.Map<ShopDiscountCategoryViewModel, ProductMedicalStock >(model);
        
            return RedirectToAction("List");

        }

        [AccessPolicy(PageCode = "SHNSDCE003")]
        public ActionResult Edit(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dc = db.DiscountCategories.FirstOrDefault(i => i.Code == dCode); // DiscountCategory.Get(dCode);
            var model = _mapper.Map<DiscountCategory, ShopDiscountCategoryEditViewModel>(dc);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNSDCE003")]
        public ActionResult Edit(ShopDiscountCategoryEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var discountcategory = db.DiscountCategories.FirstOrDefault(i => i.Code == model.Code); // DiscountCategory.Get(model.Code);
            var product = db.Products.Where(i => i.DiscountCategoryCode == discountcategory.Code && i.Status == 0).ToList();
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
                    var prod = db.Products.FirstOrDefault(i => i.Code == item.Code); // Product.Get(item.Code);
                    if (prod != null)
                    {
                        prod.DiscountCategoryCode = discountcategory.Code;
                        prod.DiscountCategoryName = discountcategory.Name;
                        prod.DiscountCategoryPercentage = discountcategory.Percentage;
                        prod.DiscountCategoryType = discountcategory.CategoryType;
                        prod.DiscountType = discountcategory.Type;
                        prod.UpdatedBy = user.Name;
                        prod.DateUpdated = DateTime.Now;
                        db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        //Product.Edit(prod, out int error);

                        var pms = db.ProductMedicalStocks.FirstOrDefault(i => i.Id == prod.Id); //ProductMedicalStock.GetProduct(prod.Code);
                        if (pms != null)
                        {
                            pms.DiscountPercentage = discountcategory.Percentage;
                            pms.DateUpdated = DateTime.Now;
                            db.Entry(pms).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            //ProductMedicalStock.Edit(pms, out int error2);
                        }
                    }
                }
            }
            
            return RedirectToAction("List");

        }

        [AccessPolicy(PageCode = "SHNSDCC002")]
        public JsonResult GetDiscountCategory(string category, string type)
        {
            var model = new List<ShopDiscountCategoryViewModel.DiscountCategoryList>();
            if (category == "1")
            {
                model = db.ProductMedicalStocks.Join(db.Products, pm => pm.productid, p => p.Id, (pm, p) => new { pm, p })
                    .Where(i => i.pm.OfferCategoryType == 1 && i.pm.Status == 0).GroupBy(i => i.pm.OfferCategoryCode)
                    .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
                    {
                        Code = i.FirstOrDefault().pm.Code,
                        OfferCategoryCode = i.FirstOrDefault().pm.OfferCategoryCode,
                        OfferCategoryName = i.FirstOrDefault().pm.OfferCategoryName,
                        OfferCategoryType = i.FirstOrDefault().pm.OfferCategoryType,
                        ProductCode = i.FirstOrDefault().pm.ProductCode,
                        ProductName = i.FirstOrDefault().pm.ProductName,
                        DiscountPercentage = i.FirstOrDefault().pm.DiscountPercentage
                    }).ToList();
            }
            if (category == "2")
            {
                model = db.ProductMedicalStocks.Join(db.Products, pm => pm.productid, p => p.Id, (pm, p) => new { pm, p })
                     .Where(i => i.pm.OfferCategoryType == 2 && i.pm.Status == 0).GroupBy(i => i.pm.OfferCategoryCode)
                     .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
                     {
                         Code = i.FirstOrDefault().pm.Code,
                         OfferCategoryCode = i.FirstOrDefault().pm.OfferCategoryCode,
                         OfferCategoryName = i.FirstOrDefault().pm.OfferCategoryName,
                         OfferCategoryType = i.FirstOrDefault().pm.OfferCategoryType,
                         ProductCode = i.FirstOrDefault().pm.ProductCode,
                         ProductName = i.FirstOrDefault().pm.ProductName,
                         DiscountPercentage = i.FirstOrDefault().pm.DiscountPercentage
                     }).ToList();
            }
            if (category == "3")
            {
                model = db.ProductMedicalStocks.Join(db.Products, pm => pm.productid, p => p.Id, (pm, p) => new { pm, p })
                     .Where(i => i.pm.OfferCategoryType == 3 && i.pm.Status == 0).GroupBy(i => i.pm.OfferCategoryCode)
                     .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
                     {
                         Code = i.FirstOrDefault().pm.Code,
                         OfferCategoryCode = i.FirstOrDefault().pm.OfferCategoryCode,
                         OfferCategoryName = i.FirstOrDefault().pm.OfferCategoryName,
                         OfferCategoryType = i.FirstOrDefault().pm.OfferCategoryType,
                         ProductCode = i.FirstOrDefault().pm.ProductCode,
                         ProductName = i.FirstOrDefault().pm.ProductName,
                         DiscountPercentage = i.FirstOrDefault().pm.DiscountPercentage
                     }).ToList();
            }
            if (category == "4")
            {
                model = db.ProductMedicalStocks.Join(db.Products, pm => pm.productid, p => p.Id, (pm, p) => new { pm, p })
                     .Where(i => i.pm.OfferCategoryType == 4 && i.pm.Status == 0).GroupBy(i => i.pm.OfferCategoryCode)
                     .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
                     {
                         Code = i.FirstOrDefault().pm.Code,
                         OfferCategoryCode = i.FirstOrDefault().pm.OfferCategoryCode,
                         OfferCategoryName = i.FirstOrDefault().pm.OfferCategoryName,
                         OfferCategoryType = i.FirstOrDefault().pm.OfferCategoryType,
                         ProductCode = i.FirstOrDefault().pm.ProductCode,
                         ProductName = i.FirstOrDefault().pm.ProductName,
                         DiscountPercentage = i.FirstOrDefault().pm.DiscountPercentage
                     }).ToList();
            }
            if (category == "5")
            {
                model = db.ProductMedicalStocks.Join(db.Products, pm => pm.productid, p => p.Id, (pm, p) => new { pm, p })
                    .Where(i => i.pm.OfferCategoryType == 5 && i.pm.Status == 0).GroupBy(i => i.pm.OfferCategoryCode)
                    .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
                    {
                        Code = i.FirstOrDefault().pm.Code,
                        OfferCategoryCode = i.FirstOrDefault().pm.OfferCategoryCode,
                        OfferCategoryName = i.FirstOrDefault().pm.OfferCategoryName,
                        OfferCategoryType = i.FirstOrDefault().pm.OfferCategoryType,
                        ProductCode = i.FirstOrDefault().pm.ProductCode,
                        ProductName = i.FirstOrDefault().pm.ProductName,
                        DiscountPercentage = i.FirstOrDefault().pm.DiscountPercentage
                    }).ToList();
            }
            if (category == "6")
            {
                model = db.ProductMedicalStocks.Join(db.Products, pm => pm.productid, p => p.Id, (pm, p) => new { pm, p })
                     .Where(i => i.pm.OfferCategoryType == 6 && i.pm.Status == 0).GroupBy(i => i.pm.OfferCategoryCode)
                     .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
                     {
                         Code = i.FirstOrDefault().pm.Code,
                         OfferCategoryCode = i.FirstOrDefault().pm.OfferCategoryCode,
                         OfferCategoryName = i.FirstOrDefault().pm.OfferCategoryName,
                         OfferCategoryType = i.FirstOrDefault().pm.OfferCategoryType,
                         ProductCode = i.FirstOrDefault().pm.ProductCode,
                         ProductName = i.FirstOrDefault().pm.ProductName,
                         DiscountPercentage = i.FirstOrDefault().pm.DiscountPercentage
                     }).ToList();
            }
            if (category == "7")
            {
                model = db.ProductMedicalStocks.Join(db.Products, pm => pm.productid, p => p.Id, (pm, p) => new { pm, p })
                    .Where(i => i.pm.OfferCategoryType == 7 && i.pm.Status == 0).GroupBy(i => i.pm.OfferCategoryCode)
                    .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
                    {
                        Code = i.FirstOrDefault().pm.Code,
                        OfferCategoryCode = i.FirstOrDefault().pm.OfferCategoryCode,
                        OfferCategoryName = i.FirstOrDefault().pm.OfferCategoryName,
                        OfferCategoryType = i.FirstOrDefault().pm.OfferCategoryType,
                        ProductCode = i.FirstOrDefault().pm.ProductCode,
                        ProductName = i.FirstOrDefault().pm.ProductName,
                        DiscountPercentage = i.FirstOrDefault().pm.DiscountPercentage
                    }).ToList();
            }
            if (category == "8")
            {
                model = db.ProductMedicalStocks.Join(db.Products, pm => pm.productid, p => p.Id, (pm, p) => new { pm, p })
                    .Where(i => i.pm.OfferCategoryType == 8 && i.pm.Status == 0).GroupBy(i => i.pm.OfferCategoryCode)
                    .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
                    {
                        Code = i.FirstOrDefault().pm.Code,
                        OfferCategoryCode = i.FirstOrDefault().pm.OfferCategoryCode,
                        OfferCategoryName = i.FirstOrDefault().pm.OfferCategoryName,
                        OfferCategoryType = i.FirstOrDefault().pm.OfferCategoryType,
                        ProductCode = i.FirstOrDefault().pm.ProductCode,
                        ProductName = i.FirstOrDefault().pm.ProductName,
                        DiscountPercentage = i.FirstOrDefault().pm.DiscountPercentage
                    }).ToList();
            }
            if (category == "9")
            {
                model = db.ProductMedicalStocks.Join(db.Products, pm => pm.productid, p => p.Id, (pm, p) => new { pm, p })
                    .Where(i => i.pm.OfferCategoryType == 9 && i.pm.Status == 0).GroupBy(i => i.pm.OfferCategoryCode)
                    .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
                    {
                        Code = i.FirstOrDefault().pm.Code,
                        OfferCategoryCode = i.FirstOrDefault().pm.OfferCategoryCode,
                        OfferCategoryName = i.FirstOrDefault().pm.OfferCategoryName,
                        OfferCategoryType = i.FirstOrDefault().pm.OfferCategoryType,
                        ProductCode = i.FirstOrDefault().pm.ProductCode,
                        ProductName = i.FirstOrDefault().pm.ProductName,
                        DiscountPercentage = i.FirstOrDefault().pm.DiscountPercentage
                    }).ToList();
            }
            if (category == "10")
            {
                model = db.ProductMedicalStocks.Join(db.Products, pm => pm.productid, p => p.Id, (pm, p) => new { pm, p })
                     .Where(i => i.pm.OfferCategoryType == 10 && i.pm.Status == 0).GroupBy(i => i.pm.OfferCategoryCode)
                     .Select(i => new ShopDiscountCategoryViewModel.DiscountCategoryList
                     {
                         Code = i.FirstOrDefault().pm.Code,
                         OfferCategoryCode = i.FirstOrDefault().pm.OfferCategoryCode,
                         OfferCategoryName = i.FirstOrDefault().pm.OfferCategoryName,
                         OfferCategoryType = i.FirstOrDefault().pm.OfferCategoryType,
                         ProductCode = i.FirstOrDefault().pm.ProductCode,
                         ProductName = i.FirstOrDefault().pm.ProductName,
                         DiscountPercentage = i.FirstOrDefault().pm.DiscountPercentage
                     }).ToList();
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSDCC002")]
        public JsonResult UpdateDiscountCategory(string offercode, string offername, int type, int category, double discount, string shopcode, string shopname)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var discountcategory = db.DiscountCategories.FirstOrDefault(i => i.OfferCategoryCode == offercode); // DiscountCategory.GetOfferCategoryCode(offercode);
            DiscountCategory dc = new DiscountCategory();
            if (type == 1 || type == 2)
            {
                if (discountcategory == null)
                {
                    dc.Name = offername;
                    dc.OfferCategoryCode = offercode;
                    dc.Percentage = discount;
                    dc.Type = type;
                    dc.CategoryType = category;
                    dc.CreatedBy = user.Name;
                    dc.UpdatedBy = user.Name;
                    dc.ShopCode = shopcode;
                    dc.ShopName = shopname;
                    dc.Code = _generatedCode;
                    dc.Status = 0;
                    dc.DateEncoded = DateTime.Now;
                    dc.DateUpdated = DateTime.Now;
                    db.DiscountCategories.Add(dc);
                    db.SaveChanges();
                    //dc.Code = DiscountCategory.Add(dc, out int error);
                }
                else
                {
                    discountcategory.Name = offername;
                    discountcategory.OfferCategoryCode = offercode;
                    discountcategory.Percentage = discount;
                    discountcategory.Type = type;
                    discountcategory.CategoryType = category;
                    discountcategory.CreatedBy = user.Name;
                    discountcategory.UpdatedBy = user.Name;
                    discountcategory.ShopCode = shopcode;
                    discountcategory.ShopName = shopname;
                    db.Entry(discountcategory).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    // DiscountCategory.Edit(discountcategory, out int error1);
                }
                var pms = db.ProductMedicalStocks.Where(i => i.OfferCategoryCode == offercode && i.Status == 0).ToList();
                if (pms != null)
                {
                    foreach (var item in pms)
                    {
                        var product = db.Products.FirstOrDefault(i => i.Id == item.productid); // Product.Get(item.ProductCode);
                        if (product != null)
                        {
                            var shop = db.Shops.FirstOrDefault(i => i.Code == product.ShopCode); // Shop.Get(product.ShopCode);
                            if (shop != null)
                            {
                                if (dc.Code != null)
                                {
                                    product.DiscountCategoryCode = dc.Code;
                                }
                                else
                                {
                                    product.DiscountCategoryCode = discountcategory.Code;
                                }
                                product.DiscountCategoryName = offername;
                                product.DiscountCategoryPercentage = discount;
                                product.DiscountCategoryType = category;
                                product.DiscountType = type;
                                product.UpdatedBy = user.Name;
                                product.DateUpdated = DateTime.Now;
                                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                               // Product.Edit(product, out int errorcode);
                            }
                            var pms1 = db.ProductMedicalStocks.FirstOrDefault(i => i.productid == product.Id); // ProductMedicalStock.GetProduct(product.Code);
                            if (pms1 != null)
                            {
                                pms1.DiscountPercentage = discountcategory.Percentage;
                                pms1.DateUpdated = DateTime.Now;
                                db.Entry(pms1).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                               // ProductMedicalStock.Edit(pms1, out int error2);
                            }
                        }
                    }
                }
            }
            else if(type == 3)
            {
                if (discountcategory == null)
                {
                    dc.Name = offername;
                    dc.OfferCategoryCode = offercode;
                    dc.Percentage = 0.0;
                    dc.Type = type;
                    dc.CategoryType = category;
                    dc.CreatedBy = user.Name;
                    dc.UpdatedBy = user.Name;
                    dc.ShopCode = shopcode;
                    dc.ShopName = shopname;
                    dc.Code = _generatedCode;
                    dc.Status = 0;
                    dc.DateEncoded = DateTime.Now;
                    dc.DateUpdated = DateTime.Now;
                    db.DiscountCategories.Add(dc);
                    db.SaveChanges();
                   // dc.Code = DiscountCategory.Add(dc, out int error);
                }
                else
                {
                    discountcategory.Name = offername;
                    discountcategory.OfferCategoryCode = offercode;
                    discountcategory.Percentage = 0.0;
                    discountcategory.Type = type;
                    discountcategory.CategoryType = category;
                    discountcategory.CreatedBy = user.Name;
                    discountcategory.UpdatedBy = user.Name;
                    discountcategory.ShopCode = shopcode;
                    discountcategory.ShopName = shopname;
                    db.Entry(discountcategory).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                   // DiscountCategory.Edit(discountcategory, out int error1);
                }
                var pms = db.ProductMedicalStocks.Where(i => i.OfferCategoryCode == offercode && i.Status == 0).ToList();
                if (pms != null)
                {
                    foreach (var item in pms)
                    {
                        var product = db.Products.FirstOrDefault(i => i.Id == item.productid); // Product.Get(item.ProductCode);
                        if (product != null)
                        {
                            var shop = db.Shops.FirstOrDefault(i => i.Code == product.ShopCode); // Shop.Get(product.ShopCode);
                            if (shop != null)
                            {
                                if (dc.Code != null)
                                {
                                    product.DiscountCategoryCode = dc.Code;
                                }
                                else
                                {
                                    product.DiscountCategoryCode = discountcategory.Code;
                                }
                                product.DiscountCategoryName = offername;
                                product.DiscountCategoryPercentage = 0.0;
                                product.DiscountCategoryType = category;
                                product.DiscountType = type;
                                product.UpdatedBy = user.Name;
                                product.DateUpdated = DateTime.Now;
                                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                                //Product.Edit(product, out int errorcode);
                            }
                        }
                    }
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSDCC002")]
        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Code,
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
