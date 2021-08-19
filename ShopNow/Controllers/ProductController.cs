﻿using Amazon;
using Amazon.S3;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    // [Authorize]
    public class ProductController : Controller
    {
        private sncEntities db = new sncEntities();
        UploadContent uc = new UploadContent();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.APSouth1;
        private static readonly string accesskey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static readonly string secretkey = ConfigurationManager.AppSettings["AWSSecretKey"];
       // private const string _prefix = "PRO";

        private static string _generatedCode(string _prefix)
        {
                return ShopNow.Helpers.DRC.Generate(_prefix);
        }

        public ProductController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<Product, ProductListViewModel.ProductList>();
                config.CreateMap<Product, ProductDetailsViewModel>();
                config.CreateMap<FoodCreateViewModel, Product>();
                config.CreateMap<Product, FoodEditViewModel>();
                config.CreateMap<ServiceCreateEditViewModel, Product>();
                config.CreateMap<Product, ServiceCreateEditViewModel>();
                config.CreateMap<Product, FoodEditViewModel>();
                config.CreateMap<FoodEditViewModel, Product>();
                config.CreateMap<ProductMappingViewModel, Product>();
                config.CreateMap<Product, ProductMappingViewModel.List>();
                config.CreateMap<FMCGCreateViewModel, Product>();
                config.CreateMap<Product, FMCGEditViewModel>();
                config.CreateMap<FMCGEditViewModel, Product>();
                config.CreateMap<MedicalCreateViewModel, Product>();
                config.CreateMap<Product, MedicalEditViewModel>();
                config.CreateMap<MedicalEditViewModel, Product>();
                config.CreateMap<ProductDishAddOn, ShopDishAddOn>();
                //config.CreateMap<MedicalEditViewModel.MedicalStockList, DefaultMedicalStockViewModel>();
                config.CreateMap<Product, ElectronicCreateEditViewModel>();
                config.CreateMap<ElectronicCreateEditViewModel, Product>();
            });

            _mapper = _mapperConfiguration.CreateMapper();
        }


        [AccessPolicy(PageCode = "SHNPROL006")]
        public ActionResult List(ProductItemListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var shid = db.Shops.Where(s => s.Id == model.ShopId).FirstOrDefault();
            if (shid !=null)
            {
                model.ListItems =     (from i in db.Products
                                       join m in db.MasterProducts on i.MasterProductId equals m.Id
                                       where i.Status == 0 && i.ShopId == shid.Id
                 select new ProductItemListViewModel.ListItem
                 {
                     Id = i.Id,
                     ProductTypeId = m.ProductTypeId,
                     ProductTypeName = m.ProductTypeName,
                     CategoryName = db.Categories.FirstOrDefault(i=> i.Id == m.CategoryId).Name,
                     BrandName = m.BrandName ?? "N/A",
                     Name = m.Name,
                     ShopId = i.ShopId,
                     ShopName = i.ShopName
                 }).ToList();
            }
            else
            {
                model.ListItems = new List<ProductItemListViewModel.ListItem>();
            }
           
            return View(model);
        }

        string GetMasterProductName(long Id)
        {
            var masterProduct = db.MasterProducts.FirstOrDefault(i => i.Id == Id);
            var name = "";
            if (masterProduct != null)
            {
                name = masterProduct.Name != null ? masterProduct.Name : "N/A";
            }
            return name;
        }

        [AccessPolicy(PageCode = "SHNPROL006")]
        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && (a.Status == 0 || a.Status==1)).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetActiveShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }


        [AccessPolicy(PageCode = "SHNPROGL009")]
        public ActionResult GetList()
        {
            var products = db.Products.Where(i => i.Status == 0).ToList();
            return View(products);
        }

        [AccessPolicy(PageCode = "SHNPROD002")]
        public ActionResult Details(string id)
        {
            var dId = AdminHelpers.DCodeLong(id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            Product pd = db.Products.FirstOrDefault(i => i.Id == dId);
            var master = db.MasterProducts.FirstOrDefault(i => i.Id == pd.MasterProductId);
            var model = new ProductDetailsViewModel();
            _mapper.Map(pd, model);
            model.ImagePath = master.ImagePath1;
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public ActionResult MedicalCreate()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var shop = db.Shops.Any(i => i.Id == user.Id);
            if (shop != false)
            {
                ViewBag.user = user.Name;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNPROC001")]
        public ActionResult MedicalCreate(MedicalCreateViewModel model)
        {
            var isExist = db.Products.Any(i => i.MasterProductId == model.MasterProductId && i.Status == 0 && i.ProductTypeId == 3 && i.ShopId == model.ShopId);
            if (isExist)
            {
                ViewBag.ErrorMessage = model.MasterProductName + " Already Exist";
                return View();
            }

            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _mapper.Map<MedicalCreateViewModel, Product>(model);
            var master = db.MasterProducts.FirstOrDefault(i => i.Id == model.MasterProductId && i.Status == 0);
            if (master != null)
                prod.MasterProductId = master.Id;
            prod.Name = model.MasterProductName;
            if (model.ShopId != 0)
            {
                var sh = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
                prod.ShopCategoryId = sh.ShopCategoryId;
                prod.ShopCategoryName = sh.ShopCategoryName;
                prod.Id = sh.Id;
            }
            prod.ProductTypeId = 3;
            prod.ProductTypeName = "Medical";
            prod.CreatedBy = user.Name;
            prod.UpdatedBy = user.Name;

            prod.DateEncoded = DateTime.Now;
            prod.DateUpdated = DateTime.Now;
            prod.Status = 0;
            db.Products.Add(prod);
            db.SaveChanges();
            ViewBag.Message = model.MasterProductName + " Saved Successfully!";


            if (model.ShopId != 0)
            {
                var sh = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
                var productcount = db.Products.Where(i => i.ShopId == model.ShopId && i.Status == 0).Count();
                if (productcount >= 10 && sh.Status == 1)
                {
                    Payment payment = new Payment();
                    payment.CustomerId = sh.CustomerId;
                    payment.CustomerName = sh.CustomerName;
                    payment.ShopId = sh.Id;
                    payment.ShopName = sh.Name;
                    payment.CountryName = sh.CountryName;
                    payment.CreatedBy = sh.CustomerName;
                    payment.UpdatedBy = sh.CustomerName;
                    payment.GSTINNumber = sh.GSTINNumber;
                    payment.Credits = "PlatForm Credits";
                    payment.OriginalAmount = 1000;
                    payment.Amount = 1000;
                    payment.PaymentResult = "success";

                    payment.DateEncoded = DateTime.Now;
                    payment.DateUpdated = DateTime.Now;
                    payment.Status = 0;
                    payment.PlatformCreditType = 2;
                    db.Payments.Add(payment);
                    db.SaveChanges();

                    // ShopCredit
                    ShopCredit shopCredit = new ShopCredit
                    {
                        CustomerId = sh.CustomerId,
                        DateUpdated = DateTime.Now,
                        DeliveryCredit = 0,
                        PlatformCredit = 1000
                    };
                    db.ShopCredits.Add(shopCredit);
                    db.SaveChanges();

                    sh.Status = 0;
                    sh.UpdatedBy = user.Name;
                    sh.DateUpdated = DateTime.Now;
                    sh.DateUpdated = DateTime.Now;
                    db.Entry(sh).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
            }
            return View();
        }

        [AccessPolicy(PageCode = "SHNPROME008")]
        public ActionResult MedicalEdit(string id)
        {
            var dCode = AdminHelpers.DCodeInt(id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var product = db.Products.FirstOrDefault(i => i.Id == dCode);
            var model = _mapper.Map<Product, MedicalEditViewModel>(product);
            var masterProduct = db.MasterProducts.FirstOrDefault(i => i.Id == product.MasterProductId);
            if (masterProduct != null)
            {
                model.MasterProductName = masterProduct.Name;
                model.BrandId = masterProduct.BrandId;
                model.BrandName = masterProduct.BrandName;
                model.CategoryId = masterProduct.CategoryId;
               // model.CategoryName = masterProduct.CategoryName;
                model.GoogleTaxonomyCode = masterProduct.GoogleTaxonomyCode;
                model.ImagePath1 = masterProduct.ImagePath1;
                model.ImagePath2 = masterProduct.ImagePath2;
                model.ImagePath3 = masterProduct.ImagePath3;
                model.ImagePath4 = masterProduct.ImagePath4;
                model.ImagePath5 = masterProduct.ImagePath5;
                model.DrugMeasurementUnitId = masterProduct.MeasurementUnitId;
                model.DrugMeasurementUnitName = masterProduct.MeasurementUnitName;
                model.PackageId = masterProduct.PackageId;
                model.PackageName = masterProduct.PackageName;
                model.Weight = masterProduct.Weight;
                model.SizeLB = masterProduct.SizeLB;
                model.DrugCompoundDetailIds = masterProduct.DrugCompoundDetailIds;
                model.DrugCompoundDetailName = masterProduct.DrugCompoundDetailName;
                model.OriginCountry = masterProduct.OriginCountry;
                model.Manufacturer = masterProduct.Manufacturer;
            }
            model.DiscountCategoryPercentage = db.DiscountCategories.FirstOrDefault(i => i.Id == product.DiscountCategoryId)?.Percentage;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNPROME008")]
        public ActionResult MedicalEdit(MedicalEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            Product prod = db.Products.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, prod);
            
            prod.DateUpdated = DateTime.Now;
            prod.UpdatedBy = user.Name;
            db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("MedicalEdit", new { id = AdminHelpers.ECodeLong(prod.Id) });
        }

        public ActionResult FMCGList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new FMCGListViewModel();
            model.ListItems = db.Products.Where(i => i.Status == 0 && i.ProductTypeId == 2 && (model.ShopId != 0 ? i.ShopId == model.ShopId : false))
                .Join(db.MasterProducts,p=>p.MasterProductId,m=>m.Id,(p,m)=>new { p,m})
            .Select(i => new FMCGListViewModel.ListItem
            {
                CategoryName = db.Categories.FirstOrDefault(j => j.Id == i.m.CategoryId).Name,
                Id = i.p.Id,
                Name = i.m.Name,
                Percentage = i.p.Percentage,
                ShopId = i.p.ShopId,
                ShopName = i.p.ShopName
            }).ToList();
            return View(model);
        }

        public ActionResult MedicalList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new MedicalListViewModel();
            model.ListItems = db.Products.Where(i => i.Status == 0 && i.ProductTypeId == 3 && (model.ShopId != 0 ? i.ShopId == model.ShopId : false))
              .Join(db.MasterProducts, p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
            .Select(i => new MedicalListViewModel.ListItem
            {
                CategoryName = db.Categories.FirstOrDefault(j => j.Id == i.m.CategoryId).Name,
                Id = i.p.Id,
                Name = i.m.Name,
                Percentage = i.p.Percentage,
                ShopId = i.p.ShopId,
                ShopName = i.p.ShopName
            }).ToList();
            return View(model);
        }

        public ActionResult FoodList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new FoodListViewModel();
            model.ListItems = db.Products.Where(i => i.Status == 0 &&
            i.ProductTypeId == 1 &&
            (model.ShopId != 0 ? i.ShopId == model.ShopId : false))
             .Join(db.MasterProducts, p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
            .Select(i => new FoodListViewModel.ListItem
            {
                CategoryName = db.Categories.FirstOrDefault(j => j.Id == i.m.CategoryId).Name,
                Id = i.p.Id,
                Name = i.m.Name,
                Percentage = i.p.Percentage,
                ShopId = i.p.ShopId,
                ShopName = i.p.ShopName
            }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNPROFC004")]
        public ActionResult FoodCreate()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var shop = db.Shops.Any(i => i.Id == user.Id);
            if (shop != false)
            {
                ViewBag.user = user.Name;
            }
            Session["ShopAddOns"] = new List<ShopAddOnSessionAddViewModel>();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNPROFC004")]
        public ActionResult FoodCreate(FoodCreateViewModel model)
        {
            var isExist = db.Products.Any(i => i.MasterProductId == model.MasterProductId && i.Status == 0 && i.ProductTypeId == 1 && i.ShopId == model.ShopId);
            if (isExist)
            {
                ViewBag.ErrorMessage = model.Name + " Already Exist";
                return View();
            }

            var prod = _mapper.Map<FoodCreateViewModel, Product>(model);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            if (model.ShopId != 0)
            {
                var sh = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
                prod.ShopCategoryId = sh.ShopCategoryId;
                prod.ShopCategoryName = sh.ShopCategoryName;
                prod.ShopId = sh.Id;
            }
            prod.ProductTypeId = 1;
            prod.ProductTypeName = "Dish";
            prod.Status = 0;
            prod.CreatedBy = user.Name;
            prod.UpdatedBy = user.Name;

            prod.DateEncoded = DateTime.Now;
            prod.DateUpdated = DateTime.Now;
            prod.Status = 0;
            db.Products.Add(prod);
            db.SaveChanges();

            //AddOns
            if (prod.Customisation == true)
            {
                List<ShopAddOnSessionAddViewModel> shopAddOns = Session["ShopAddOns"] as List<ShopAddOnSessionAddViewModel>;
                foreach (var item in shopAddOns)
                {
                    var productDishAddon = db.ProductDishAddOns.FirstOrDefault(i => i.Id == item.Id);
                    var shopdishAddOn = _mapper.Map<ProductDishAddOn, ShopDishAddOn>(productDishAddon);
                    shopdishAddOn.PortionPrice = item.PortionPrice;
                    shopdishAddOn.AddOnsPrice = item.AddOnsPrice;
                    shopdishAddOn.CrustPrice = item.CrustPrice;
                    shopdishAddOn.IsActive = true;
                    shopdishAddOn.ProductId = prod.Id;
                    shopdishAddOn.ProductName = prod.Name;
                    shopdishAddOn.ShopId = prod.ShopId;
                    shopdishAddOn.ShopName = prod.ShopName;
                    shopdishAddOn.ProductDishAddonId = productDishAddon.Id;
                    db.ShopDishAddOns.Add(shopdishAddOn);
                    db.SaveChanges();
                }
            }

            if (model.ShopId != 0)
            {
                var sh = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);// Shop.Get(model.ShopId);
                var productcount = db.Products.Where(i => i.ShopId == model.ShopId && i.Status == 0).Count();
                if (productcount >= 10 && sh.Status == 1)
                {
                    Payment payment = new Payment();
                    payment.CustomerId = sh.CustomerId;
                    payment.CustomerName = sh.CustomerName;
                    payment.ShopId = sh.Id;
                    payment.ShopName = sh.Name;

                    payment.CountryName = sh.CountryName;
                    payment.CreatedBy = sh.CustomerName;
                    payment.UpdatedBy = sh.CustomerName;
                    payment.GSTINNumber = sh.GSTINNumber;
                    payment.Credits = "PlatForm Credits";
                    payment.OriginalAmount = 1000;
                    payment.Amount = 1000;
                    payment.GSTAmount = 0;
                    payment.CreditType = 0;
                    payment.PaymentResult = "success";

                    payment.DateEncoded = DateTime.Now;
                    payment.DateUpdated = DateTime.Now;
                    payment.Status = 0;
                    payment.PlatformCreditType = 2;
                    db.Payments.Add(payment);
                    db.SaveChanges();

                    ShopCredit shopCredit = new ShopCredit
                    {
                        CustomerId = sh.CustomerId,
                        DateUpdated = DateTime.Now,
                        DeliveryCredit = 0,
                        PlatformCredit = 1000
                    };
                    db.ShopCredits.Add(shopCredit);
                    db.SaveChanges();

                    sh.Status = 0;
                    sh.UpdatedBy = user.Name;
                    sh.DateUpdated = DateTime.Now;
                    sh.DateUpdated = DateTime.Now;
                    db.Entry(sh).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
            }
            ViewBag.Message = model.Name + " Saved Successfully!";

            Session["ShopAddOns"] = null;
            return View();
        }

        [AccessPolicy(PageCode = "SHNPROFE005")]
        public ActionResult FoodEdit(string id)
        {
            var dCode = AdminHelpers.DCodeInt(id);
            if (dCode==0)
                return HttpNotFound();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var product = db.Products.FirstOrDefault(i => i.Id == dCode);
            var model = _mapper.Map<Product, FoodEditViewModel>(product);
            var masterProduct = db.MasterProducts.FirstOrDefault(i => i.Id == model.MasterProductId);
            if (masterProduct != null)
            {
                model.MasterProductName = masterProduct.Name;
                model.CategoryId = masterProduct.CategoryId;
              //  model.CategoryName = masterProduct.CategoryName;
            }
            Session["ShopAddOnsEdit"] = new List<ShopAddOnSessionEditViewModel>();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNPROFE005")]
        public ActionResult FoodEdit(FoodEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
           // var shop = db.Shops.Any(i => i.Id == user.Id);
            var prod = db.Products.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, prod);
            prod.DateUpdated = DateTime.Now;
            prod.UpdatedBy = user.Name;
            db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            //Addons
            if (prod.Customisation == true)
            {
                List<ShopAddOnSessionEditViewModel> shopAddOns = Session["ShopAddOnsEdit"] as List<ShopAddOnSessionEditViewModel> ?? new List<ShopAddOnSessionEditViewModel>();
                if (shopAddOns.Count() > 0)
                {
                    foreach (var item in shopAddOns)
                    {
                        var shopDishAddon = db.ShopDishAddOns.FirstOrDefault(i => i.Id == item.Id);
                        if (shopDishAddon != null)
                        {
                            shopDishAddon.AddOnsPrice = item.AddOnsPrice;
                            shopDishAddon.PortionPrice = item.PortionPrice;
                            shopDishAddon.CrustPrice = item.CrustPrice;
                            shopDishAddon.IsActive = item.IsActive;
                            db.Entry(shopDishAddon).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            var productDishAddon = db.ProductDishAddOns.FirstOrDefault(i => i.Id == item.Id);
                            var shopdishAddOn = _mapper.Map<ProductDishAddOn, ShopDishAddOn>(productDishAddon);
                            shopdishAddOn.PortionPrice = item.PortionPrice;
                            shopdishAddOn.AddOnsPrice = item.AddOnsPrice;
                            shopdishAddOn.CrustPrice = item.CrustPrice;
                            shopdishAddOn.IsActive = true;
                            shopdishAddOn.ProductId = prod.Id;
                            shopdishAddOn.ProductName = prod.Name;
                            shopdishAddOn.ShopId = prod.ShopId;
                            shopdishAddOn.ShopName = prod.ShopName;
                            shopdishAddOn.ProductDishAddonId = productDishAddon.Id;
                            db.ShopDishAddOns.Add(shopdishAddOn);
                            db.SaveChanges();
                        }
                    }
                }
            }
            else
            {
                var shopDishAddonList = db.ShopDishAddOns.Where(i => i.ProductId == prod.Id).ToList();
                foreach (var item in shopDishAddonList)
                {
                    var shopDishAddon = db.ShopDishAddOns.FirstOrDefault(i => i.Id == item.Id);
                    shopDishAddon.IsActive = false;
                    db.Entry(shopDishAddon).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            //check addons Inactive and make customisation false
            var hasShopDishAddons = db.ShopDishAddOns.Any(i => i.ProductId == prod.Id && i.IsActive == true);
            if (!hasShopDishAddons)
            {
                prod.Customisation = false;
                db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            Session["ShopAddOnsEdit"] = null;
            //return RedirectToAction("FoodList", "Product",new { ShopId = prod.ShopId,ShopName = prod.ShopName });
            return RedirectToAction("FoodEdit", new { id = AdminHelpers.ECodeLong(model.Id) });
        }

        public JsonResult GetProductDishAddOns(int masterProductId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var list = db.ProductDishAddOns.Where(i => i.MasterProductId == masterProductId && i.Status == 0).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //Food Create Sessions
        public JsonResult AddShopAddOns(ShopAddOnSessionAddViewModel model)
        {
            List<ShopAddOnSessionAddViewModel> shopAddOns = Session["ShopAddOns"] as List<ShopAddOnSessionAddViewModel> ?? new List<ShopAddOnSessionAddViewModel>();
            if (model.Id != 0)
            {
                var addOn = new ShopAddOnSessionAddViewModel
                {
                    AddOnsPrice = model.AddOnsPrice,
                    Id = model.Id,
                    CrustPrice = model.CrustPrice,
                    PortionPrice = model.PortionPrice
                };
                shopAddOns.Add(addOn);
            }
            Session["ShopAddOns"] = shopAddOns;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveFromShopAddOns(int id)
        {
            List<ShopAddOnSessionAddViewModel> shopAddOns = Session["ShopAddOns"] as List<ShopAddOnSessionAddViewModel> ?? new List<ShopAddOnSessionAddViewModel>();

            if (shopAddOns.Remove(shopAddOns.SingleOrDefault(i => i.Id == id)))
                Session["ShopAddOns"] = shopAddOns;

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //Food Edit Sessions
        public JsonResult EditShopAddOns(ShopAddOnSessionEditViewModel model)
        {
            List<ShopAddOnSessionEditViewModel> shopAddOns = Session["ShopAddOnsEdit"] as List<ShopAddOnSessionEditViewModel> ?? new List<ShopAddOnSessionEditViewModel>();
            if (model.Id != 0)
            {
                var addOn = new ShopAddOnSessionEditViewModel
                {
                    AddOnsPrice = model.AddOnsPrice,
                    Id = model.Id,
                    CrustPrice = model.CrustPrice,
                    PortionPrice = model.PortionPrice,
                    IsActive = model.IsActive
                };
                shopAddOns.Add(addOn);
            }
            Session["ShopAddOnsEdit"] = shopAddOns;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveFromEditShopAddOns(int id)
        {
            List<ShopAddOnSessionEditViewModel> shopAddOns = Session["ShopAddOnsEdit"] as List<ShopAddOnSessionEditViewModel> ?? new List<ShopAddOnSessionEditViewModel>();

            if (shopAddOns.Remove(shopAddOns.SingleOrDefault(i => i.Id == id)))
                Session["ShopAddOnsEdit"] = shopAddOns;

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //get product Addons which are not added in Shop Addons
        public JsonResult GetProductDishAddOnsFoodEdit(int masterProductId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var list = (from p in db.ProductDishAddOns
                        where (p.MasterProductId == masterProductId && p.Status==0) && !db.ShopDishAddOns.Any(i => i.ProductDishAddonId == p.Id)
                        select p).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShopDishAddOns(int productId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var list = db.ShopDishAddOns.Where(i => i.ProductId == productId && i.Status == 0).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult FMCGCreate()
        {
           // Session["DefaultFMCG"] = new List<FMCGCreateEditViewModel>();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var shop = db.Shops.Any(i => i.Id == user.Id);
            if (shop != false)
            {
                ViewBag.user = user.Name;
            }

            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "")]
        public ActionResult FMCGCreate(FMCGCreateViewModel model)
        {
            //Return to View if product already exist
            var isExist = db.Products.Any(i => i.MasterProductId == model.MasterProductId && i.Status == 0 && i.ProductTypeId == 2 && i.ShopId == model.ShopId);
            if (isExist)
            {
                ViewBag.ErrorMessage = model.Name + " Already Exist";
                return View();
            }

            var user = ((Helpers.Sessions.User)Session["USER"]);
            var product = _mapper.Map<FMCGCreateViewModel, Product>(model);
            product.ProductTypeId = 2;
            product.ProductTypeName = "FMCG";
            if (model.ShopId != 0)
            {
                var shop = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
                product.ShopId = shop.Id;
                product.ShopCategoryId = shop.ShopCategoryId;
                product.ShopCategoryName = shop.ShopCategoryName;
            }
            product.DateEncoded = DateTime.Now;
            product.DateUpdated = DateTime.Now;
            product.Status = 0;
            product.CreatedBy = user.Name;
            product.UpdatedBy = user.Name;
            db.Products.Add(product);
            db.SaveChanges();
            ViewBag.Message = model.Name + " Saved Successfully!";
            if (model.ShopId != 0)
            {
                var sh = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
                var productcount = db.Products.Where(i => i.ShopId == model.ShopId && i.Status == 0).Count();
                if (productcount >= 10 && sh.Status == 1)
                {
                    Payment payment = new Payment();
                    payment.CustomerId = sh.CustomerId;
                    payment.CustomerName = sh.CustomerName;
                    payment.ShopId = sh.Id;
                    payment.ShopName = sh.Name;
                    payment.CountryName = sh.CountryName;
                    payment.CreatedBy = sh.CustomerName;
                    payment.UpdatedBy = sh.CustomerName;
                    payment.GSTINNumber = sh.GSTINNumber;
                    payment.Credits = "PlatForm Credits";
                    payment.OriginalAmount = 1000;
                    payment.Amount = 1000;
                    payment.GSTAmount = 0;
                    payment.CreditType = 0;
                    payment.PaymentResult = "success";

                    payment.DateEncoded = DateTime.Now;
                    payment.DateUpdated = DateTime.Now;
                    payment.Status = 0;
                    db.Payments.Add(payment);
                    payment.PlatformCreditType = 2;
                    db.SaveChanges();

                    ShopCredit shopCredit = new ShopCredit
                    {
                        CustomerId = sh.CustomerId,
                        DateUpdated = DateTime.Now,
                        DeliveryCredit = 0,
                        PlatformCredit = 1000
                    };
                    db.ShopCredits.Add(shopCredit);
                    db.SaveChanges();

                    sh.Status = 0;
                    sh.UpdatedBy = user.Name;
                    sh.DateUpdated = DateTime.Now;
                    sh.DateUpdated = DateTime.Now;
                    db.Entry(sh).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
            }
            return View();
        }

        public ActionResult FMCGEdit(string id)
        {
            var dCode = AdminHelpers.DCodeInt(id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dCode.ToString()))
                return HttpNotFound();
            var product = db.Products.FirstOrDefault(i => i.Id == dCode);
            var model = _mapper.Map<Product, FMCGEditViewModel>(product);
            var masterProduct = db.MasterProducts.FirstOrDefault(i => i.Id == product.MasterProductId);
            if (masterProduct != null)
            {
                model.MasterProductName = masterProduct.Name;
                model.BrandId = masterProduct.BrandId;
                model.BrandName = masterProduct.BrandName;
                model.CategoryId = masterProduct.CategoryId;
                model.CategoryName = db.Categories.FirstOrDefault(i=> i.Id == masterProduct.CategoryId).Name;
                model.GoogleTaxonomyCode = masterProduct.GoogleTaxonomyCode;
                model.ImagePath1 = masterProduct.ImagePath1;
                model.ImagePath2 = masterProduct.ImagePath2;
                model.ImagePath3 = masterProduct.ImagePath3;
                model.ImagePath4 = masterProduct.ImagePath4;
                model.ImagePath5 = masterProduct.ImagePath5;
                model.LongDescription = masterProduct.LongDescription;
                model.ShortDescription = masterProduct.ShortDescription;
                model.SubCategoryId = masterProduct.SubCategoryId;
                model.SubCategoryName = db.SubCategories.FirstOrDefault(i=> i.Id == masterProduct.SubCategoryId).Name;
                model.NextSubCategoryId = masterProduct.NextSubCategoryId;
                model.NextSubCategoryName = db.NextSubCategories.FirstOrDefault(i=> i.Id == masterProduct.NextSubCategoryId).Name;
                model.MeasurementUnitId = masterProduct.MeasurementUnitId;
                model.MeasurementUnitName = masterProduct.MeasurementUnitName;
                model.PackageId = masterProduct.PackageId;
                model.PackageName = masterProduct.PackageName;
                model.Weight = masterProduct.Weight;
                model.SizeLB = masterProduct.SizeLB;
            }
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "")]
        public ActionResult FMCGEdit(FMCGEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = db.Products.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, prod);
            prod.Name = model.Name;
            prod.UpdatedBy = user.Name;
            prod.DateUpdated = DateTime.Now;
            db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            //return RedirectToAction("FMCGList", new { ShopId = prod.ShopId, shopName = prod.ShopName });
            return RedirectToAction("FMCGEdit", new { id = AdminHelpers.ECodeLong(model.Id) });
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult ElectronicCreate()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var shop = db.Shops.Any(i => i.Id == user.Id);
            if (shop != false)
            {
                ViewBag.user = user.Name;
            }
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "")]
        public ActionResult ElectronicCreate(ElectronicCreateEditViewModel model)
        {
            //Return to View if product already exist
            var name = db.Products.FirstOrDefault(i => i.MasterProductId == model.MasterProductId && i.Status == 0 && i.ProductTypeId == 4 && i.ShopId == model.ShopId);
            if (name != null)
            {
                ViewBag.ErrorMessage = model.Name + " Already Exist";
                return View();
            }

            var user = ((Helpers.Sessions.User)Session["USER"]);
            var product = _mapper.Map<ElectronicCreateEditViewModel, Product>(model);
            product.CreatedBy = user.Name;
            product.UpdatedBy = user.Name;
            product.ProductTypeName = "Electronic";
            if (model.ShopId != 0)
            {
                var shop = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
                product.ShopId = shop.Id;
                product.ShopCategoryId = shop.ShopCategoryId;
                product.ShopCategoryName = shop.ShopCategoryName;
            }
            product.DateEncoded = DateTime.Now;
            product.DateUpdated = DateTime.Now;
            product.Status = 0;
            db.Products.Add(product);
            db.SaveChanges();
            ViewBag.Message = model.Name + " Saved Successfully!";
            if (model.ShopId != 0)
            {
                var sh = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);// Shop.Get(model.ShopId);
                var productcount = db.Products.Where(i => i.ShopId == model.ShopId && i.Status == 0).Count();
                if (productcount >= 10 && sh.Status == 1)
                {
                    Payment payment = new Payment();
                    payment.CustomerId = sh.CustomerId;
                    payment.CustomerName = sh.CustomerName;
                    payment.ShopId = sh.Id;
                    payment.ShopName = sh.Name;
                     
                    payment.CountryName = sh.CountryName;
                    payment.CreatedBy = sh.CustomerName;
                    payment.UpdatedBy = sh.CustomerName;
                    payment.GSTINNumber = sh.GSTINNumber;
                    payment.Credits = "PlatForm Credits";
                    payment.OriginalAmount = 1000;
                    payment.Amount = 1000;
                    payment.GSTAmount = 0;
                    payment.CreditType = 0;
                    payment.PaymentResult = "success"; 
                    payment.DateEncoded = DateTime.Now;
                    payment.DateUpdated = DateTime.Now;
                    payment.Status = 0;
                    payment.PlatformCreditType = 2;
                    db.Payments.Add(payment);
                    db.SaveChanges();

                    ShopCredit shopCredit = new ShopCredit
                    {
                        CustomerId = sh.CustomerId,
                        DateUpdated = DateTime.Now,
                        DeliveryCredit = 0,
                        PlatformCredit = 1000
                    };
                    db.ShopCredits.Add(shopCredit);
                    db.SaveChanges();

                    sh.Status = 0;
                    sh.UpdatedBy = user.Name;
                    sh.DateUpdated = DateTime.Now;
                    sh.DateUpdated = DateTime.Now;
                    db.Entry(sh).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return View();
        }

        public ActionResult ElectronicEdit(int id)
        {
            var dId = AdminHelpers.DCodeLong(id.ToString());
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dId.ToString()))
                return HttpNotFound();
            var product = db.Products.FirstOrDefault(i => i.Id == dId);
            //if (product != null)
            //{
            //    if (product.ImagePath1 != null)
            //        product.ImagePath1 = product.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            //    if (product.ImagePath2 != null)
            //        product.ImagePath2 = product.ImagePath2.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            //    if (product.ImagePath3 != null)
            //        product.ImagePath3 = product.ImagePath3.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            //    if (product.ImagePath4 != null)
            //        product.ImagePath4 = product.ImagePath4.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            //    if (product.ImagePath5 != null)
            //        product.ImagePath5 = product.ImagePath5.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            //}
            var model = _mapper.Map<Product, ElectronicCreateEditViewModel>(product);
            model.MasterProductName = GetMasterProductName(model.MasterProductId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "")]
        public ActionResult ElectronicEdit(ElectronicCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = db.Products.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, prod);
            prod.Name = model.Name;
            prod.ProductTypeName = model.ProductType;
            prod.UpdatedBy = user.Name;
            prod.DateUpdated = DateTime.Now;
            prod.DateUpdated = DateTime.Now;
            db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("ElectronicList", new { ShopId = prod.ShopId, shopName = prod.ShopName });
        }

        public ActionResult ElectronicList(ElectronicListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Products.Where(i => i.Status == 0 && i.ProductTypeId == 4 && (model.ShopId != 0 ? i.ShopId == model.ShopId : false))
           .Join(db.MasterProducts, p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
            .Select(i => new ElectronicListViewModel.ListItem
            {
                CategoryName = db.Categories.FirstOrDefault(j=> j.Id == i.m.CategoryId).Name,
                Id = i.p.Id,
                Name = i.m.Name,
                Percentage = i.p.Percentage,
                ShopId = i.p.ShopId,
                ShopName = i.p.ShopName
            }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNPROSC007")]
        public ActionResult ServiceCreate()
        {
            Session["ServiceAddOns"] = new List<AddOnsCreateViewModel>();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNPROSC007")]
        public ActionResult ServiceCreate(ServiceCreateEditViewModel pd, HttpPostedFileBase _ProductImage)
        {
            var prod = _mapper.Map<ServiceCreateEditViewModel, Product>(pd);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.Any(i => i.Id == user.Id);
            if (shop != false)
            {
                prod.ShopId = user.Id;
                prod.ShopName = user.Name;
                ViewBag.user = user.Id;
            }
            if (prod.Id == 0)
            {
                prod.Status = 0;
                prod.DateEncoded = DateTime.Now;
                prod.DateUpdated = DateTime.Now;
                prod.Status = 0;
                db.Products.Add(prod);
                db.SaveChanges();
                return RedirectToAction("List", "Product");
            }
            else
            {
                //Product.Edit(prod, out error);
                prod.DateUpdated = DateTime.Now;
                db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("List", "Product");
            }
        }

        [AccessPolicy(PageCode = "SHNPROR010")]
        public JsonResult Delete(string id)
        {
            var dId = AdminHelpers.DCodeInt(id);
            var product = db.Products.FirstOrDefault(i => i.Id == dId);
            if (product != null)
            {
                product.Status = 2;
                product.DateUpdated = DateTime.Now;
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SHNPROATA015")]
        public JsonResult AddToAddOns(AddOnsCreateViewModel model)
        {
            List<AddOnsCreateViewModel> addOns = Session["AddOns"] as List<AddOnsCreateViewModel>;
            if (addOns == null)
            {
                addOns = new List<AddOnsCreateViewModel>();
            }
            var id = addOns.Count() + 1;
            model.Id = id;
            addOns.Add(model);
            Session["AddOns"] = addOns;

            return Json(new {PortionCode = model.PortionCode, PortionName = model.PortionName, PortionPrice = model.PortionPrice, AddOnCategoryCode = model.AddOnCategoryCode,
                AddOnCategoryName = model.AddOnCategoryName, AddOnsPrice = model.AddOnsPrice, MinSelectionLimit = model.MinSelectionLimit, MaxSelectionLimit = model.MaxSelectionLimit,
                Name = model.Name, CrustName = model.CrustName, CrustPrice = model.CrustPrice, Id = id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SHNPRORAO016")]
        public JsonResult RemoveFromAddOns(int id)
        {
            List<AddOnsCreateViewModel> addOns = Session["AddOns"] as List<AddOnsCreateViewModel>;
            if (addOns.Remove(addOns.SingleOrDefault(i => i.Id == id)))
            {
                this.Session["AddOns"] = addOns;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SHNPROERA017")]
        public JsonResult EditRemoveAddOns(int id, string code)
        {
            List<AddOnsCreateViewModel> addOns = Session["AddOns"] as List<AddOnsCreateViewModel>;
            if (addOns == null)
            {
                addOns = new List<AddOnsCreateViewModel>();
            }
            if (addOns.Remove(addOns.SingleOrDefault(i => i.Id == id)))
            {
                this.Session["AddOns"] = addOns;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            if (id != 0)
            {
                var addon = db.ProductDishAddOns.FirstOrDefault(i => i.Id == id);
                addon.Status = 2;
                //ProductDishAddOn.Edit(addon, out int error);
                addon.DateUpdated = DateTime.Now;
                db.Entry(addon).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                if (addOns.Remove(addOns.SingleOrDefault(i => i.Id == id)))
                {
                    this.Session["AddOns"] = addOns;
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        
        // Dish Select2
        [AccessPolicy(PageCode = "SHNPROFC004")]
        public async Task<JsonResult> GetPortionSelect2(string q = "")
        {
            var model = await db.Portions.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROFC004")]
        public async Task<JsonResult> GetDishCategorySelect2(string q = "")
        {
            var model = await db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 1).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROFC004")]
        public async Task<JsonResult> GetDishShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && (a.ShopCategoryId == 1 || a.ShopCategoryId == 2) && (a.Status == 0 || a.Status == 1)).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROFC004")]
        public async Task<JsonResult> GetAddonCategorySelect2(string q = "")
        {
            var model = await db.AddOnCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROFC004")]
        public async Task<JsonResult> GetDishMasterProductSelect2(string q = "")
        {
            var model = await db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 1).Select(i => new
            {
                id = i.Id,
                text = i.Name,
                CategoryIds = i.CategoryId,
             //   CategoryName = i.CategoryName,
                BrandId = i.BrandId,
                BrandName = i.BrandName,
                ShortDescription = i.ShortDescription,
                LongDescription = i.LongDescription,
                Customisation = i.Customisation,
                ColorCode = i.ColorCode,
                //ImagePath = i.ImagePath,
                Price = i.Price,
                ProductTypeId = i.ProductTypeId,
                ProductTypeName = i.ProductTypeName,
                ImagePath1 = i.ImagePath1,
                GoogleTaxonomyCode = i.GoogleTaxonomyCode,
                MasterId = i.Id
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // Product Select2

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetBrandSelect2(string q = "")
        {
            var model = await db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetMedicalBrandSelect2(string q = "")
        {
            var model = await db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetProductShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && (a.ShopCategoryId == 2 || a.ShopCategoryId == 3 || a.ShopCategoryId == 4 || a.ShopCategoryId == 1) && (a.Status == 0 || a.Status == 1)).Select(i => new
            {
                id = i.Id,
                text = i.Name + " (" + i.PhoneNumber + ")"
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetCategorySelect2(string q = "")
        {
            var model = await db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetMedicalCategorySelect2(string q = "")
        {
            var model = await db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 3).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetProductCategorySelect2(string q = "")
        {
            var model = await db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId ==4).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetMasterProductSelect2(string q = "")
        {
            var model = await db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 4).Select(i => new
            {
                id = i.Id,
                text = i.Name,
                CategoryIds = i.CategoryId,
             //   CategoryName = i.CategoryName,
                BrandId = i.BrandId,
                BrandName = i.BrandName,
                ShortDescription = i.ShortDescription,
                LongDescription = i.LongDescription,
                ImagePath = i.ImagePath1,
                Price = i.Price,
                ProductTypeName = i.ProductTypeName,
                ProductTypeId = i.ProductTypeId
            }).Take(500).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetMedicalMasterProductSelect2(string q = "")
        {
            var model = await db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 3).Select(i => new
            {
                id = i.Id,
                text = i.Name,
                CategoryIds = i.CategoryId,
             //  CategoryName = i.CategoryName,
                BrandId = i.BrandId,
                BrandName = i.BrandName,
                MeasurementUnitId = i.MeasurementUnitId,
                MeasurementUnitName = i.MeasurementUnitName,
                PriscriptionCategory = i.PriscriptionCategory,
                DrugCompoundDetailIds = i.DrugCompoundDetailIds,
                DrugCompoundDetailName = i.DrugCompoundDetailName,
                PackageId = i.PackageId,
                PackageName = i.PackageName,
                Manufacturer = i.Manufacturer,
                OriginCountry = i.OriginCountry,
                iBarU = i.IBarU,
                weight = i.Weight,
                SizeLB = i.SizeLB,
                Price = i.Price,
                ImagePath1 = i.ImagePath1,
                ImagePath2 = i.ImagePath2,
                ImagePath3 = i.ImagePath3,
                ImagePath4 = i.ImagePath4,
                ImagePath5 = i.ImagePath5,
                ProductTypeId = i.ProductTypeId,
                GoogleTaxonomyCode = i.GoogleTaxonomyCode,
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetManualMedicalMasterProductSelect2(string q = "")
        {
            var model = await db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 3).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).Take(500).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        //[AccessPolicy(PageCode = "SHNPROC001")]
        //public JsonResult GetMasterProductSpecification(int id)
        //{
        //    var model =new  List<ProductEditViewModel.SpecificationList>();
        //    model = db.ProductSpecifications.Where(i => i.MasterProductId == id && i.Status == 0).Select(i => new ProductEditViewModel.SpecificationList
        //    {
        //        Id = i.Id,
        //        SpecificationName = i.SpecificationName,
        //        SpecificationId = i.SpecificationId,
        //        ProductName = i.MasterProductName,
        //        ProductId = i.MasterProductId,
        //        Value = i.Value
        //    }).ToList();

        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetSpecificationSelect2(string q = "")
        {
            var model = await db.Specifications.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetDrugUnitSelect2(string q = "")
        {
            var model = await db.MeasurementUnits.Where(a => a.UnitName.Contains(q) && a.Status == 0).OrderBy(i => i.UnitName).Select(i => new
            { 
                id = i.Id,
                text = i.UnitName
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetDrugCompoundDetailSelect2(string q = "")
        {
            var model = await db.DrugCompoundDetails.Where(a => a.AliasName.Contains(q) && a.Status == 0).OrderBy(i => i.AliasName).Select(i => new
            {
                id = i.Id,
                text = i.AliasName
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetDiscountCategorySelect2(string q = "")
        {
            var model = await db.DiscountCategories.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name,
                percentage = i.Percentage
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public ActionResult ShopItemMapping()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new ProductMappingViewModel();
            return View(model);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SHNPROC001")]
        public ActionResult ShopItemMapping(int originalShopId, int newShopId, string newShopName)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new ProductMappingViewModel();
            model.Lists = db.Products.Where(i => i.ShopId == originalShopId && i.Status == 0).ToList().AsQueryable().ProjectTo<ProductMappingViewModel.List>(_mapperConfiguration).ToList();// Product.GetListItem(originalShopId).AsQueryable().ProjectTo<ProductMappingViewModel.List>(_mapperConfiguration).ToList();

            foreach (var pro in model.Lists)
            {
                Product product = new Product();
                var prod = _mapper.Map<ProductMappingViewModel, Product>(model);
                prod.ShopId = newShopId;
                prod.ShopName = newShopName;
                //prod.MainSNCode = ShopNow.Helpers.DRC.GenerateSNIN();
                prod.DateEncoded = DateTime.Now;
                prod.DateUpdated = DateTime.Now;
                prod.Status = 0;
                db.Products.Add(prod);
                db.SaveChanges();
                // prod.Code = prod.MainSNCode;
            }                      
            return View(model);
        }

        public async Task<JsonResult> GetFMCGSelect2(string q = "")
        {
            var model = await db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 2)
                .Select(i => new
                {
                    id = i.Id,
                    text = i.Name,
                    CategoryId = i.CategoryId,
                    SubCategoryId = i.SubCategoryId,
                    NextSubCategoryId = i.NextSubCategoryId,
                    BrandId = i.BrandId,
                    BrandName = i.BrandName,
                    ShortDescription = i.ShortDescription,
                    LongDescription = i.LongDescription,
                    ImagePath1 = i.ImagePath1,
                    ImagePath2 = i.ImagePath2,
                    ImagePath3 = i.ImagePath3,
                    ImagePath4 = i.ImagePath4,
                    ImagePath5 = i.ImagePath5,
                    Price = i.Price,
                    ProductTypeId = i.ProductTypeId,
                    ASIN = i.ASIN,
                    GoogleTaxonomyCode = i.GoogleTaxonomyCode,
                    Weight = i.Weight,
                    SizeLB = i.SizeLB,
                    MeasurementUnitId = i.MeasurementUnitId,
                    MeasurementUnitName = i.MeasurementUnitName,
                    PackageId = i.PackageId,
                    PackageName = i.PackageName
                }).ToListAsync();
            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSelectCategoryNames(int categoryId, int subcategoryId, int nextsubcategoryId)
        {
            var categoryName = db.Categories.FirstOrDefault(i => i.Id == categoryId).Name;
            var subcategoryname = db.SubCategories.FirstOrDefault(i => i.Id == subcategoryId).Name;
            var nextsubcategoryname = db.NextSubCategories.FirstOrDefault(i => i.Id == nextsubcategoryId).Name;
            return Json(new { categoryName = categoryName, subcategoryname = subcategoryname, nextsubcategoryname=nextsubcategoryname }, JsonRequestBehavior.AllowGet);
        }
        
        public async Task<JsonResult> GetFMCGPackageSelect2(string q = "")
        {
            var model = await db.Packages.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetFMCGMeasurementUnitSelect2(string q = "")
        {
            var model = await db.MeasurementUnits.OrderBy(i => i.UnitName).Where(a => a.UnitName.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.UnitName
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetElectronicSelect2(string q = "")
        {
            var model = await db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 4).Select(i => new
            {
                id = i.Id,
                text = i.Name,
                CategoryId = i.CategoryId,
                SubCategoryId = i.SubCategoryId,
                NextSubCategoryId = i.NextSubCategoryId,
                BrandId = i.BrandId,
                BrandName = i.BrandName,
                ShortDescription = i.ShortDescription,
                LongDescription = i.LongDescription,
                ImagePath1 = i.ImagePath1,
                ImagePath2 = i.ImagePath2,
                ImagePath3 = i.ImagePath3,
                ImagePath4 = i.ImagePath4,
                ImagePath5 = i.ImagePath5,
                Price = i.Price,
                ProductTypeId = i.ProductTypeId,
                ASIN = i.ASIN,
                GoogleTaxonomyCode = i.GoogleTaxonomyCode,
                weight = i.Weight,
                SizeLB = i.SizeLB,
                MeasurementUnitId = i.MeasurementUnitId,
                MeasurementUnitName = i.MeasurementUnitName,
                PackageId = i.PackageId,
                PackageName = i.PackageName,
                MasterId = i.Id
            }).ToListAsync();
            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        [AccessPolicy(PageCode = "")]
        [HttpGet]
        public ActionResult UpdateStock(ProductUpdateStockViewModel model)
        {
            if (model.ItemId != 0 && model.ShopId !=0)
            {
                var product = db.Products.FirstOrDefault(i => i.ItemId == model.ItemId && i.ShopId == model.ShopId);
                model.ProductName = product.Name;
                model.ItemId = product.ItemId;
                model.Qty = product.Qty;
            }
            return View(model);
        }

        [AccessPolicy(PageCode = "")]
        [HttpPost]
        public ActionResult UpdateStockValue(ProductUpdateStockViewModel model)
        {
            if (model.ItemId != 0 && model.ShopId != 0)
            {
                var product = db.Products.FirstOrDefault(i => i.ItemId == model.ItemId && i.ShopId == model.ShopId);
                product.Qty = model.Qty;
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("UpdateStock");
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetProductByShopSelect2(string shopIds,string q = "")
        {
            string[] shops = shopIds.Split(',');
            var model = await db.Products.Where(a => a.Name.Contains(q) && a.Status == 0 && shopIds.Contains(a.ShopId.ToString())).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetAllProductSelect2(string q = "")
        {
            var model = await db.Products.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }
    }
}