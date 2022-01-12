using Amazon;
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
                config.CreateMap<Product, ElectronicCreateEditViewModel>();
                config.CreateMap<ElectronicCreateEditViewModel, Product>();
            });

            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SNCPRL203")]
        public ActionResult List(ProductItemListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Products.Where(i => i.Status == 0 && (model.ShopId != 0 ? i.ShopId == model.ShopId : false))
                .Join(db.MasterProducts, p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
                .Join(db.Categories, p => p.p.CategoryId, c => c.Id, (p, c) => new { p, c })
                .Select(i => new ProductItemListViewModel.ListItem
                {
                    Id = i.p.p.Id,
                    BrandName = i.p.m.BrandName ?? "N/A",
                    CategoryName = i.c.Name,
                    ProductTypeId = i.p.m.ProductTypeId,
                    ProductTypeName = i.p.m.ProductTypeName,
                    Name = i.p.m.Name,
                    ShopId = i.p.p.ShopId,
                    ShopName = i.p.p.ShopName,
                    IsOnline = i.p.p.IsOnline
                }).ToList();

            return View(model);
        }

        [AccessPolicy(PageCode = "SNCPRD204")]
        public ActionResult Details(string id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dId = AdminHelpers.DCodeLong(id);
            Product pd = db.Products.FirstOrDefault(i => i.Id == dId);
            var model = new ProductDetailsViewModel();
            _mapper.Map(pd, model);
            if (pd.MasterProductId != 0)
            {
                var master = db.MasterProducts.FirstOrDefault(i => i.Id == pd.MasterProductId);
                model.ImagePath = master.ImagePath1;
                model.MasterProductName = master.Name;
            }
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCPRMC205")]
        public ActionResult MedicalCreate()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
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
        [AccessPolicy(PageCode = "SNCPRMC205")]
        public ActionResult MedicalCreate(MedicalCreateViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var isExist = db.Products.Any(i => i.MasterProductId == model.MasterProductId && i.Status == 0 && i.ProductTypeId == 3 && i.ShopId == model.ShopId);
            if (isExist)
            {
                ViewBag.ErrorMessage = model.MasterProductName + " Already Exist";
                return View();
            }
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
            prod.IsOnline = true;
            prod.MappedDate = DateTime.Now;
            db.Products.Add(prod);
            db.SaveChanges();
            ViewBag.Message = model.MasterProductName + " Saved Successfully!";

            if (model.ShopId != 0)
            {
                var sh = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
                var productcount = db.Products.Where(i => i.ShopId == model.ShopId && i.Status == 0).Count();
                if (productcount >= 10 && sh.Status == 1)
                {
                    //Payment payment = new Payment();
                    //payment.CustomerId = sh.CustomerId;
                    //payment.CustomerName = sh.CustomerName;
                    //payment.ShopId = sh.Id;
                    //payment.ShopName = sh.Name;
                    //payment.CountryName = sh.CountryName;
                    //payment.CreatedBy = sh.CustomerName;
                    //payment.UpdatedBy = sh.CustomerName;
                    //payment.GSTINNumber = sh.GSTINNumber;
                    //payment.Credits = "PlatForm Credits";
                    //payment.OriginalAmount = 1000;
                    //payment.Amount = 1000;
                    //payment.PaymentResult = "success";
                    //payment.DateEncoded = DateTime.Now;
                    //payment.DateUpdated = DateTime.Now;
                    //payment.Status = 0;
                    //payment.PlatformCreditType = 2;
                    //db.Payments.Add(payment);
                    //db.SaveChanges();

                    //// ShopCredit
                    //ShopCredit shopCredit = new ShopCredit
                    //{
                    //    CustomerId = sh.CustomerId,
                    //    DateUpdated = DateTime.Now,
                    //    DeliveryCredit = 0,
                    //    PlatformCredit = 1000
                    //};
                    //db.ShopCredits.Add(shopCredit);
                    //db.SaveChanges();

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

        [AccessPolicy(PageCode = "SNCPRME206")]
        public ActionResult MedicalEdit(string id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dCode = AdminHelpers.DCodeLong(id);
            if (string.IsNullOrEmpty(dCode.ToString()))
                return HttpNotFound();
            var product = db.Products.FirstOrDefault(i => i.Id == dCode);
            var model = _mapper.Map<Product, MedicalEditViewModel>(product);
            var masterProduct = db.MasterProducts.FirstOrDefault(i => i.Id == product.MasterProductId);
            var shop = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
            if (shop != null)
                model.ShopName = shop.Name;
            if (masterProduct != null)
            {
                model.MasterProductName = masterProduct.Name;
                model.BrandId = masterProduct.BrandId;
                model.BrandName = masterProduct.BrandName;
                model.CategoryId = product.CategoryId;
                var categoryName = db.Categories.FirstOrDefault(i => i.Id == product.CategoryId);
                if (categoryName != null)
                    model.CategoryName = categoryName.Name;
                else
                    model.CategoryName = "N/A";
                model.GoogleTaxonomyCode = masterProduct.GoogleTaxonomyCode;
                model.PriscriptionCategory = masterProduct.PriscriptionCategory;
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
                model.SizeLBH = masterProduct.SizeLWH;
                model.IBarU = masterProduct.IBarU;
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
        [AccessPolicy(PageCode = "SNCPRME206")]
        public ActionResult MedicalEdit(MedicalEditViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var prod = db.Products.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, prod);
            if (model.IsPreorder == false)
            {
                model.PreorderHour = 0;
            }
            prod.Name = model.Name;
            prod.UpdatedBy = user.Name;
            prod.DateUpdated = DateTime.Now;
            db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("MedicalEdit", new { id = AdminHelpers.ECodeLong(prod.Id) });
        }

        [AccessPolicy(PageCode = "SNCPRML207")]
        public ActionResult MedicalList(MedicalListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Products.Where(i => i.Status == 0 && i.ProductTypeId == 3 && (model.ShopId != 0 ? i.ShopId == model.ShopId : false))
              .Join(db.MasterProducts, p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
              .Join(db.Categories, p => p.p.CategoryId, c => c.Id, (p, c) => new { p, c })
            .Select(i => new MedicalListViewModel.ListItem
            {
                CategoryName = i.c.Name,
                Id = i.p.p.Id,
                Name = i.p.m.Name,
                Percentage = i.p.p.Percentage,
                ShopId = i.p.p.ShopId,
                ShopName = i.p.p.ShopName,
                IsOnline = i.p.p.IsOnline
            }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCPRDL208")]
        public ActionResult FoodList(FoodListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Products.Where(i => i.Status == 0 && i.ProductTypeId == 1 && (model.ShopId != 0 ? i.ShopId == model.ShopId : false))
             .Join(db.MasterProducts, p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
            .Select(i => new FoodListViewModel.ListItem
            {
                CategoryName = db.Categories.FirstOrDefault(j => j.Id == i.p.CategoryId).Name,
                Id = i.p.Id,
                Name = i.m.Name,
                Percentage = i.p.Percentage,
                ShopId = i.p.ShopId,
                ShopName = i.p.ShopName,
                IsOnline = i.p.IsOnline
            }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCPRDC209")]
        public ActionResult FoodCreate()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
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
        [AccessPolicy(PageCode = "SNCPRDC209")]
        public ActionResult FoodCreate(FoodCreateViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var isExist = db.Products.Any(i => i.MasterProductId == model.MasterProductId && i.Status == 0 && i.ProductTypeId == 1 && i.ShopId == model.ShopId);
            if (isExist)
            {
                ViewBag.ErrorMessage = model.Name + " Already Exist";
                return View();
            }
            var prod = _mapper.Map<FoodCreateViewModel, Product>(model);
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
            prod.IsOnline = true;
            prod.MappedDate = DateTime.Now;
            prod.DateEncoded = DateTime.Now;
            prod.DateUpdated = DateTime.Now;
            prod.CreatedBy = user.Name;
            prod.UpdatedBy = user.Name;
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
                    //Payment payment = new Payment();
                    //payment.CustomerId = sh.CustomerId;
                    //payment.CustomerName = sh.CustomerName;
                    //payment.ShopId = sh.Id;
                    //payment.ShopName = sh.Name;
                    //payment.CountryName = sh.CountryName;
                    //payment.CreatedBy = sh.CustomerName;
                    //payment.UpdatedBy = sh.CustomerName;
                    //payment.GSTINNumber = sh.GSTINNumber;
                    //payment.Credits = "PlatForm Credits";
                    //payment.OriginalAmount = 1000;
                    //payment.Amount = 1000;
                    //payment.GSTAmount = 0;
                    //payment.CreditType = 0;
                    //payment.PaymentResult = "success";
                    //payment.DateEncoded = DateTime.Now;
                    //payment.DateUpdated = DateTime.Now;
                    //payment.Status = 0;
                    //payment.PlatformCreditType = 2;
                    //db.Payments.Add(payment);
                    //db.SaveChanges();

                    //ShopCredit shopCredit = new ShopCredit
                    //{
                    //    CustomerId = sh.CustomerId,
                    //    DateUpdated = DateTime.Now,
                    //    DeliveryCredit = 0,
                    //    PlatformCredit = 1000
                    //};
                    //db.ShopCredits.Add(shopCredit);
                    //db.SaveChanges();

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

        [AccessPolicy(PageCode = "SNCPRDE210")]
        public ActionResult FoodEdit(string id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dCode = AdminHelpers.DCodeLong(id);
            if (dCode == 0)
                return HttpNotFound();
            var product = db.Products.FirstOrDefault(i => i.Id == dCode);
            var model = _mapper.Map<Product, FoodEditViewModel>(product);
            var masterProduct = db.MasterProducts.FirstOrDefault(i => i.Id == model.MasterProductId);
            if (masterProduct != null)
            {
                model.MasterProductName = masterProduct.Name;
                model.CategoryId = product.CategoryId;
                model.GoogleTaxonomyCode = masterProduct.GoogleTaxonomyCode;
                model.ColorCode = masterProduct.ColorCode;
                var categoryName = db.Categories.FirstOrDefault(i => i.Id == product.CategoryId);
                if (categoryName != null)
                    model.CategoryName = categoryName.Name;
                else
                    model.CategoryName = "N/A";
                model.ImagePath1 = masterProduct.ImagePath1;
            }
            Session["ShopAddOnsEdit"] = new List<ShopAddOnSessionEditViewModel>();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SNCPRDE210")]
        public ActionResult FoodEdit(FoodEditViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            // var shop = db.Shops.Any(i => i.Id == user.Id);
            var prod = db.Products.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, prod);
            if (model.IsPreorder == false)
            {
                model.PreorderHour = 0;
            }
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

        [AccessPolicy(PageCode = "SNCPRFL211")]
        public ActionResult FMCGList(FMCGListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Products.Where(i => i.Status == 0 && i.ProductTypeId == 2 && (model.ShopId != 0 ? i.ShopId == model.ShopId : false))
                .Join(db.MasterProducts, p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
            .Select(i => new FMCGListViewModel.ListItem
            {
                CategoryName = db.Categories.FirstOrDefault(j => j.Id == i.p.CategoryId).Name,
                Id = i.p.Id,
                Name = i.m.Name,
                Percentage = i.p.Percentage,
                ShopId = i.p.ShopId,
                ShopName = i.p.ShopName,
                IsOnline = i.p.IsOnline
            }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCPRFC212")]
        public ActionResult FMCGCreate()
        {
            // Session["DefaultFMCG"] = new List<FMCGCreateEditViewModel>();
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
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
        [AccessPolicy(PageCode = "SNCPRFC212")]
        public ActionResult FMCGCreate(FMCGCreateViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            //Return to View if product already exist
            var isExist = db.Products.Any(i => i.MasterProductId == model.MasterProductId && i.Status == 0 && i.ProductTypeId == 2 && i.ShopId == model.ShopId);
            if (isExist)
            {
                ViewBag.ErrorMessage = model.Name + " Already Exist";
                return View();
            }
            var product = _mapper.Map<FMCGCreateViewModel, Product>(model);
            product.ProductTypeId = 2;
            product.ProductTypeName = "FMCG";
            if (model.ShopId != 0)
            {
                var shop = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
                product.ShopId = shop.Id;
                product.ShopName = shop.Name;
                product.ShopCategoryId = shop.ShopCategoryId;
                product.ShopCategoryName = shop.ShopCategoryName;
            }
            product.IsOnline = true;
            product.MappedDate = DateTime.Now;
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
                    //Payment payment = new Payment();
                    //payment.CustomerId = sh.CustomerId;
                    //payment.CustomerName = sh.CustomerName;
                    //payment.ShopId = sh.Id;
                    //payment.ShopName = sh.Name;
                    //payment.CountryName = sh.CountryName;
                    //payment.CreatedBy = sh.CustomerName;
                    //payment.UpdatedBy = sh.CustomerName;
                    //payment.GSTINNumber = sh.GSTINNumber;
                    //payment.Credits = "PlatForm Credits";
                    //payment.OriginalAmount = 1000;
                    //payment.Amount = 1000;
                    //payment.GSTAmount = 0;
                    //payment.CreditType = 0;
                    //payment.PaymentResult = "success";
                    //payment.DateEncoded = DateTime.Now;
                    //payment.DateUpdated = DateTime.Now;
                    //payment.Status = 0;
                    //db.Payments.Add(payment);
                    //payment.PlatformCreditType = 2;
                    //db.SaveChanges();

                    //ShopCredit shopCredit = new ShopCredit
                    //{
                    //    CustomerId = sh.CustomerId,
                    //    DateUpdated = DateTime.Now,
                    //    DeliveryCredit = 0,
                    //    PlatformCredit = 1000
                    //};
                    //db.ShopCredits.Add(shopCredit);
                    //db.SaveChanges();

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

        [AccessPolicy(PageCode = "SNCPRFE213")]
        public ActionResult FMCGEdit(string id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dCode = AdminHelpers.DCodeLong(id);
            if (string.IsNullOrEmpty(dCode.ToString()))
                return HttpNotFound();
            var product = db.Products.FirstOrDefault(i => i.Id == dCode);
            var model = _mapper.Map<Product, FMCGEditViewModel>(product);
            product.ProductTypeId = 2;
            product.ProductTypeName = "FMCG";
            var masterProduct = db.MasterProducts.FirstOrDefault(i => i.Id == product.MasterProductId);
            if (masterProduct != null)
            {
                model.MasterProductName = masterProduct.Name;
                model.BrandId = masterProduct.BrandId;
                model.BrandName = masterProduct.BrandName;
                model.CategoryId = product.CategoryId;
                var categoryName = db.Categories.FirstOrDefault(i => i.Id == product.CategoryId);
                if (categoryName != null)
                    model.CategoryName = categoryName.Name;
                else
                    model.CategoryName = "N/A";
                model.GoogleTaxonomyCode = masterProduct.GoogleTaxonomyCode;
                model.ImagePath1 = masterProduct.ImagePath1;
                model.ImagePath2 = masterProduct.ImagePath2;
                model.ImagePath3 = masterProduct.ImagePath3;
                model.ImagePath4 = masterProduct.ImagePath4;
                model.ImagePath5 = masterProduct.ImagePath5;
                model.LongDescription = masterProduct.LongDescription;
                model.ShortDescription = masterProduct.ShortDescription;
                model.SubCategoryId = masterProduct.SubCategoryId;
                var subcategoryName = db.SubCategories.FirstOrDefault(i => i.Id == masterProduct.SubCategoryId);
                if (subcategoryName != null)
                    model.SubCategoryName = subcategoryName.Name;
                else
                    model.SubCategoryName = "N/A";
                model.NextSubCategoryId = masterProduct.NextSubCategoryId;
                var nextsubcategoryName = db.NextSubCategories.FirstOrDefault(i => i.Id == masterProduct.NextSubCategoryId);
                if (nextsubcategoryName != null)
                    model.NextSubCategoryName = nextsubcategoryName.Name;
                else
                    model.NextSubCategoryName = "N/A";
                model.MeasurementUnitId = masterProduct.MeasurementUnitId;
                model.MeasurementUnitName = masterProduct.MeasurementUnitName;
                model.PackageId = masterProduct.PackageId;
                model.PackageName = masterProduct.PackageName;
                model.Weight = masterProduct.Weight;
                model.SizeLBH = masterProduct.SizeLWH;
                model.ASIN = masterProduct.ASIN;
            }
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCPRFE213")]
        public ActionResult FMCGEdit(FMCGEditViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var prod = db.Products.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, prod);
            if (model.IsPreorder == false)
            {
                model.PreorderHour = 0;
            }
            prod.ProductTypeId = 2;
            prod.ProductTypeName = "FMCG";
            prod.Name = model.Name;
            prod.UpdatedBy = user.Name;
            prod.DateUpdated = DateTime.Now;
            db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            //return RedirectToAction("FMCGList", new { ShopId = prod.ShopId, shopName = prod.ShopName });
            return RedirectToAction("FMCGEdit", new { id = AdminHelpers.ECodeLong(model.Id) });
        }

        [AccessPolicy(PageCode = "SNCPREC214")]
        public ActionResult ElectronicCreate()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
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
        [AccessPolicy(PageCode = "SNCPREC214")]
        public ActionResult ElectronicCreate(ElectronicCreateEditViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            //Return to View if product already exist
            var name = db.Products.Any(i => i.MasterProductId == model.MasterProductId && i.Status == 0 && i.ProductTypeId == 4 && i.ShopId == model.ShopId);
            if (name)
            {
                ViewBag.ErrorMessage = model.Name + " Already Exist";
                return View();
            }
            var product = _mapper.Map<ElectronicCreateEditViewModel, Product>(model);
            product.CreatedBy = user.Name;
            product.UpdatedBy = user.Name;
            product.ProductTypeId = 4;
            product.ProductTypeName = "Electronic";
            if (model.ShopId != 0)
            {
                var shop = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);
                product.ShopId = shop.Id;
                product.ShopName = shop.Name;
                product.ShopCategoryId = shop.ShopCategoryId;
                product.ShopCategoryName = shop.ShopCategoryName;
            }
            product.IsOnline = true;
            product.MappedDate = DateTime.Now;
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
                    //Payment payment = new Payment();
                    //payment.CustomerId = sh.CustomerId;
                    //payment.CustomerName = sh.CustomerName;
                    //payment.ShopId = sh.Id;
                    //payment.ShopName = sh.Name;
                    //payment.CountryName = sh.CountryName;
                    //payment.CreatedBy = sh.CustomerName;
                    //payment.UpdatedBy = sh.CustomerName;
                    //payment.GSTINNumber = sh.GSTINNumber;
                    //payment.Credits = "PlatForm Credits";
                    //payment.OriginalAmount = 1000;
                    //payment.Amount = 1000;
                    //payment.GSTAmount = 0;
                    //payment.CreditType = 0;
                    //payment.PaymentResult = "success";
                    //payment.DateEncoded = DateTime.Now;
                    //payment.DateUpdated = DateTime.Now;
                    //payment.Status = 0;
                    //payment.PlatformCreditType = 2;
                    //db.Payments.Add(payment);
                    //db.SaveChanges();

                    //ShopCredit shopCredit = new ShopCredit
                    //{
                    //    CustomerId = sh.CustomerId,
                    //    DateUpdated = DateTime.Now,
                    //    DeliveryCredit = 0,
                    //    PlatformCredit = 1000
                    //};
                    //db.ShopCredits.Add(shopCredit);
                    //db.SaveChanges();

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

        [AccessPolicy(PageCode = "SNCPREE215")]
        public ActionResult ElectronicEdit(string id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dId = AdminHelpers.DCodeLong(id);
            if (string.IsNullOrEmpty(dId.ToString()))
                return HttpNotFound();
            var product = db.Products.FirstOrDefault(i => i.Id == dId);
            var model = _mapper.Map<Product, ElectronicCreateEditViewModel>(product);
            var master = db.MasterProducts.FirstOrDefault(i => i.Id == product.MasterProductId);
            if (master != null)
            {
                model.MasterProductName = master.Name;
                model.BrandId = master.BrandId;
                model.BrandName = master.BrandName;
                model.GoogleTaxonomyCode = master.GoogleTaxonomyCode;
                model.ImagePath1 = master.ImagePath1;
                model.ImagePath2 = master.ImagePath2;
                model.ImagePath3 = master.ImagePath3;
                model.ImagePath4 = master.ImagePath4;
                model.ImagePath5 = master.ImagePath5;
                model.CategoryId = product.CategoryId;
                var categoryName = db.Categories.FirstOrDefault(i => i.Id == product.CategoryId);
                if (categoryName != null)
                    model.CategoryName = categoryName.Name;
                else
                    model.CategoryName = "N/A";
                model.ShortDescription = master.ShortDescription;
                model.LongDescription = master.LongDescription;
                model.SizeLWH = master.SizeLWH;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCPREE215")]
        public ActionResult ElectronicEdit(ElectronicCreateEditViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var prod = db.Products.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, prod);
            if (model.IsPreorder == false)
            {
                model.PreorderHour = 0;
            }
            prod.Name = model.Name;
            prod.UpdatedBy = user.Name;
            prod.DateUpdated = DateTime.Now;
            db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("ElectronicEdit", new { id = AdminHelpers.ECodeLong(model.Id) });
        }

        [AccessPolicy(PageCode = "SNCPREL216")]
        public ActionResult ElectronicList(ElectronicListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Products.Where(i => i.Status == 0 && i.ProductTypeId == 4 && (model.ShopId != 0 ? i.ShopId == model.ShopId : false))
                .Join(db.MasterProducts, p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
            .Select(i => new ElectronicListViewModel.ListItem
            {
                CategoryName = db.Categories.FirstOrDefault(j => j.Id == i.p.CategoryId).Name,
                Id = i.p.Id,
                Name = i.m.Name,
                Percentage = i.p.Percentage,
                ShopId = i.p.ShopId,
                ShopName = i.p.ShopName,
                IsOnline = i.p.IsOnline
            }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCPRD217")]
        public JsonResult Delete(string id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
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
        [AccessPolicy(PageCode = "SNCPRSM218")]
        public ActionResult ShopItemMapping(int originalShopId, int newShopId, string newShopName)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
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

        [HttpGet]
        [AccessPolicy(PageCode = "SNCPRUS219")]
        public ActionResult UpdateStock(ProductUpdateStockViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (model.ItemId != 0 && model.ShopId != 0)
            {
                var product = db.Products.FirstOrDefault(i => i.ItemId == model.ItemId && i.ShopId == model.ShopId);
                if (product != null)
                {
                    model.ProductName = product.Name;
                    model.ItemId = product.ItemId;
                    model.Qty = product.Qty;
                }
            }
            return View(model);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SNCPRUSV220")]
        public ActionResult UpdateStockValue(ProductUpdateStockViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (model.ItemId != 0 && model.ShopId != 0)
            {
                var product = db.Products.FirstOrDefault(i => i.ItemId == model.ItemId && i.ShopId == model.ShopId);
                product.Qty = model.Qty;
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("UpdateStock");
        }

        [HttpGet]
        [AccessPolicy(PageCode = "SNCPRAD221")]
        public ActionResult ApplyDiscount()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SNCPRAD221")]
        public ActionResult ApplyDiscount(ApplyDiscountViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var productsList = db.Products.Where(i => i.ShopId == model.ShopId && i.Status == 0).ToList();
            if (productsList != null)
            {
                productsList.ForEach(i => { i.Percentage = model.Percentage; i.Price = Math.Round(i.MenuPrice - (i.MenuPrice * model.Percentage) / 100, 0); });
                db.SaveChanges();
            }
            ViewBag.Message = "Successfully Updated!";
            return View();
        }

        [AccessPolicy(PageCode = "SNCPRZL222")]
        public ActionResult ZeroPriceList(ProductZeroPriceList model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (model.ShopId != 0)
                model.ShopCategoryId = db.Shops.FirstOrDefault(i => i.Id == model.ShopId).ShopCategoryId;

            model.ListItems = db.Products.Where(i => i.MasterProductId != 0 && (i.MenuPrice == 0 || i.Price == 0) && i.Status == 0 && (model.ShopId != 0 ? i.ShopId == model.ShopId : false))
                .Join(db.MasterProducts, p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
                .Select(i => new ProductZeroPriceList.ListItem
                {
                    Id = i.p.Id,
                    MenuPrice = i.p.MenuPrice,
                    Name = i.m.Name,
                    Quantity = i.p.Qty,
                    SellingPrice = i.p.Price,
                    ItemId = i.p.ItemId
                }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCPRIL223")]
        public ActionResult InactiveList(ProductInactiveList model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Products.Where(i => i.MasterProductId != 0 && i.Status == 1 && (model.ShopId != 0 ? i.ShopId == model.ShopId : false))
                .Join(db.MasterProducts, p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
                .Select(i => new ProductInactiveList.ListItem
                {
                    Id = i.p.Id,
                    MenuPrice = i.p.MenuPrice,
                    Name = i.m.Name,
                    Quantity = i.p.Qty,
                    SellingPrice = i.p.Price,
                    ItemId = i.p.ItemId
                }).ToList();
            return View(model);
        }

        public ActionResult UpdateProductOnline(int Id, bool isOnline)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var product = db.Products.Where(i => i.Id == Id).FirstOrDefault();
            if (product != null)
            {
                product.IsOnline = isOnline;
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("List");
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

        public ActionResult GetList()
        {
            var products = db.Products.Where(i => i.Status == 0).ToList();
            return View(products);
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
                        where (p.MasterProductId == masterProductId && p.Status == 0) && !db.ShopDishAddOns.Any(i => i.ProductDishAddonId == p.Id)
                        select p).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShopDishAddOns(int productId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var list = db.ShopDishAddOns.Where(i => i.ProductId == productId && i.Status == 0).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && (a.Status == 0 || a.Status == 1)).Select(i => new
            {
                id = i.Id,
                text = i.Name + " -- " + i.DistrictName,
                textSave = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDishShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && (a.Status == 0 || a.Status == 1) && (a.ShopCategoryId == 1 || a.ShopCategoryId == 3)).Select(i => new
            {
                id = i.Id,
                text = i.Name + " -- " + i.DistrictName,
                textSave = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetFMCGShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && (a.Status == 0 || a.Status == 1) && (a.ShopCategoryId == 1 || a.ShopCategoryId == 2 || a.ShopCategoryId == 3 || a.ShopCategoryId == 4)).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetMedicalShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && (a.Status == 0 || a.Status == 1) && (a.ShopCategoryId == 3 || a.ShopCategoryId == 4)).Select(i => new
            {
                id = i.Id,
                text = i.Name + " (" + i.PhoneNumber + ")" + " -- " + i.DistrictName,
                shopname = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetElectronicShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && (a.Status == 0 || a.Status == 1) && (a.ShopCategoryId == 3 || a.ShopCategoryId == 4 || a.ShopCategoryId == 5)).Select(i => new
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
                text = i.Name + " -- " + i.DistrictName
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
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

            return Json(new
            {
                PortionId = model.PortionId,
                PortionName = model.PortionName,
                PortionPrice = model.PortionPrice,
                AddOnCategoryId = model.AddOnCategoryId,
                AddOnCategoryName = model.AddOnCategoryName,
                AddOnsPrice = model.AddOnsPrice,
                MinSelectionLimit = model.MinSelectionLimit,
                MaxSelectionLimit = model.MaxSelectionLimit,
                Name = model.Name,
                CrustName = model.CrustName,
                CrustPrice = model.CrustPrice,
                Id = id
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
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
        public async Task<JsonResult> GetPortionSelect2(string q = "")
        {
            var model = await db.Portions.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDishCategorySelect2(string q = "")
        {
            var model = await db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 1).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetAddonCategorySelect2(string q = "")
        {
            var model = await db.AddOnCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDishSelect2(string q = "")
        {
            var model = await db.MasterProducts.Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 1)
                .Join(db.Categories, m => m.CategoryId, c => c.Id, (m, c) => new { m, c })
                .Select(i => new
                {
                    id = i.m.Id,
                    text = i.m.Name,
                    CategoryId = i.m.CategoryId,
                    CategoryName = i.c.Name,
                    BrandId = i.m.BrandId,
                    BrandName = i.m.BrandName,
                    ShortDescription = i.m.ShortDescription,
                    LongDescription = i.m.LongDescription,
                    Customisation = i.m.Customisation,
                    ColorCode = i.m.ColorCode,
                    Price = i.m.Price,
                    ProductTypeId = i.m.ProductTypeId,
                    ProductTypeName = i.m.ProductTypeName,
                    ImagePath1 = i.m.ImagePath1,
                    GoogleTaxonomyCode = i.m.GoogleTaxonomyCode,
                    MasterId = i.m.Id
                }).Take(500).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // Product Select2
        public async Task<JsonResult> GetBrandSelect2(string q = "")
        {
            var model = await db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetMedicalBrandSelect2(string q = "")
        {
            var model = await db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetCategorySelect2(string q = "")
        {
            var model = await db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetMedicalCategorySelect2(string q = "")
        {
            var model = await db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 3).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetProductCategorySelect2(string q = "")
        {
            var model = await db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 4).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetMasterProductSelect2(string q = "")
        {
            var model = await db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 4).Select(i => new
            {
                id = i.Id,
                text = i.Name,
                CategoryId = i.CategoryId,
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

        public async Task<JsonResult> GetMedicalSelect2(string q = "")
        {
            var model = await db.MasterProducts.Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 3)
                .Join(db.Categories, m => m.CategoryId, c => c.Id, (m, c) => new { m, c })
                .Select(i => new
                {
                    id = i.m.Id,
                    text = i.m.Name,
                    CategoryId = i.m.CategoryId,
                    CategoryName = i.c.Name,
                    BrandId = i.m.BrandId,
                    BrandName = i.m.BrandName,
                    MeasurementUnitId = i.m.MeasurementUnitId,
                    MeasurementUnitName = i.m.MeasurementUnitName,
                    PriscriptionCategory = i.m.PriscriptionCategory,
                    DrugCompoundDetailIds = i.m.DrugCompoundDetailIds,
                    DrugCompoundDetailName = i.m.DrugCompoundDetailName,
                    PackageId = i.m.PackageId,
                    PackageName = i.m.PackageName,
                    Manufacturer = i.m.Manufacturer,
                    OriginCountry = i.m.OriginCountry,
                    iBarU = i.m.IBarU,
                    weight = i.m.Weight,
                    SizeLB = i.m.SizeLWH,
                    Price = i.m.Price,
                    ImagePath1 = i.m.ImagePath1,
                    ImagePath2 = i.m.ImagePath2,
                    ImagePath3 = i.m.ImagePath3,
                    ImagePath4 = i.m.ImagePath4,
                    ImagePath5 = i.m.ImagePath5,
                    ProductTypeId = i.m.ProductTypeId,
                    GoogleTaxonomyCode = i.m.GoogleTaxonomyCode
                }).Take(500).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

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

        public async Task<JsonResult> GetSpecificationSelect2(string q = "")
        {
            var model = await db.Specifications.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDrugUnitSelect2(string q = "")
        {
            var model = await db.MeasurementUnits.Where(a => a.UnitName.Contains(q) && a.Status == 0).OrderBy(i => i.UnitName).Select(i => new
            {
                id = i.Id,
                text = i.UnitName
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDrugCompoundDetailSelect2(string q = "")
        {
            var model = await db.DrugCompoundDetails.Where(a => a.AliasName.Contains(q) && a.Status == 0).OrderBy(i => i.AliasName).Select(i => new
            {
                id = i.Id,
                text = i.AliasName
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

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

        public async Task<JsonResult> GetFMCGSelect2(string q = "")
        {
            var model = await db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 2)
                .Join(db.Categories, m => m.CategoryId, c => c.Id, (m, c) => new { m, c })
                .Join(db.SubCategories, m => m.m.SubCategoryId, sc => sc.Id, (m, sc) => new { m, sc })
                .Join(db.NextSubCategories, m => m.m.m.NextSubCategoryId, nsc => nsc.Id, (m, nsc) => new { m, nsc })
                .Select(i => new
                {
                    id = i.m.m.m.Id,
                    text = i.m.m.m.Name,
                    CategoryId = i.m.m.m.CategoryId,
                    CategoryName = i.m.m.c.Name,
                    SubCategoryId = i.m.m.m.SubCategoryId,
                    SubCategoryName = i.m.sc.Name,
                    NextSubCategoryId = i.m.m.m.NextSubCategoryId,
                    NextSubCategoryName = i.nsc.Name,
                    BrandId = i.m.m.m.BrandId,
                    BrandName = i.m.m.m.BrandName,
                    ShortDescription = i.m.m.m.ShortDescription,
                    LongDescription = i.m.m.m.LongDescription,
                    ImagePath1 = i.m.m.m.ImagePath1,
                    ImagePath2 = i.m.m.m.ImagePath2,
                    ImagePath3 = i.m.m.m.ImagePath3,
                    ImagePath4 = i.m.m.m.ImagePath4,
                    ImagePath5 = i.m.m.m.ImagePath5,
                    Price = i.m.m.m.Price,
                    ProductTypeId = i.m.m.m.ProductTypeId,
                    ASIN = i.m.m.m.ASIN,
                    GoogleTaxonomyCode = i.m.m.m.GoogleTaxonomyCode,
                    Weight = i.m.m.m.Weight,
                    SizeLBH = i.m.m.m.SizeLWH,
                    MeasurementUnitId = i.m.m.m.MeasurementUnitId,
                    MeasurementUnitName = i.m.m.m.MeasurementUnitName,
                    PackageId = i.m.m.m.PackageId,
                    PackageName = i.m.m.m.PackageName
                }).Take(500).ToListAsync();
            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSelectCategoryNames(int categoryId, int subcategoryId, int nextsubcategoryId)
        {
            var categoryName = db.Categories.FirstOrDefault(i => i.Id == categoryId).Name;
            var subcategoryname = db.SubCategories.FirstOrDefault(i => i.Id == subcategoryId).Name;
            var nextsubcategoryname = db.NextSubCategories.FirstOrDefault(i => i.Id == nextsubcategoryId).Name;
            return Json(new { categoryName = categoryName, subcategoryname = subcategoryname, nextsubcategoryname = nextsubcategoryname }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategoryName(int categoryId)
        {
            var categoryName = db.Categories.FirstOrDefault(i => i.Id == categoryId).Name;
            return Json(new { categoryName = categoryName }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetFMCGCategorySelect2(string q = "")
        {
            var model = await db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 2).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetFMCGSubCategorySelect2(string q = "")
        {
            var model = await db.SubCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 2).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // NextSub Category Select2
        public async Task<JsonResult> GetFMCGNextSubCategorySelect2(string q = "")
        {
            var model = await db.NextSubCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 2).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
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
            var model = await db.MasterProducts.Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 4)
              .Join(db.Categories, m => m.CategoryId, c => c.Id, (m, c) => new { m, c })
              .Select(i => new
              {
                  id = i.m.Id,
                  text = i.m.Name,
                  CategoryId = i.m.CategoryId,
                  CategoryName = i.c.Name,
                  BrandId = i.m.BrandId,
                  BrandName = i.m.BrandName,
                  ShortDescription = i.m.ShortDescription,
                  LongDescription = i.m.LongDescription,
                  Customisation = i.m.Customisation,
                  ColorCode = i.m.ColorCode,
                  Price = i.m.Price,
                  SizeLWH = i.m.SizeLWH,
                  ProductTypeId = i.m.ProductTypeId,
                  ProductTypeName = i.m.ProductTypeName,
                  ImagePath1 = i.m.ImagePath1,
                  ImagePath2 = i.m.ImagePath2,
                  ImagePath3 = i.m.ImagePath3,
                  ImagePath4 = i.m.ImagePath4,
                  ImagePath5 = i.m.ImagePath5,
                  GoogleTaxonomyCode = i.m.GoogleTaxonomyCode,
                  MasterId = i.m.Id
              }).Take(500).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetProductByShopSelect2(string shopIds, string q = "")
        {
            string[] shops = shopIds.Split(',');
            var model = await db.Products.Where(a => a.Name.Contains(q) && a.Status == 0 && shopIds.Contains(a.ShopId.ToString())).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetAllProductSelect2(string q = "")
        {
            var model = await db.Products.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatePriceAndQuantityAndDiscount(long id, double mrp, double price, int qty, int discatid = 0, string discatname = "")
        {
            var product = db.Products.FirstOrDefault(i => i.Id == id);
            product.MenuPrice = mrp;
            product.Price = price;
            product.Qty = qty;
            if (discatid != 0 && !(string.IsNullOrEmpty(discatname)))
            {
                product.DiscountCategoryId = discatid;
                product.DiscountCategoryName = discatname;
            }
            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateActive(long id)
        {
            var product = db.Products.FirstOrDefault(i => i.Id == id);
            product.Status = 0;
            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdatePrice()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SNCPRAD221")]
        public ActionResult UpdatePrice(UpdatePriceViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var productsList = db.Products.Where(i => i.ShopId == model.ShopId && i.Status == 0).ToList();
            if (model.Percentage != 0)
            {
                if (productsList != null)
                {
                    productsList.ForEach(i => { i.Price = Math.Round(i.Price + (i.ShopPrice * model.Percentage) / 100, MidpointRounding.AwayFromZero); i.MenuPrice = Math.Round(i.MenuPrice + (i.ShopPrice * model.Percentage) / 100, MidpointRounding.AwayFromZero); });
                    db.SaveChanges();
                }
            }
            else
            {
                if (productsList != null)
                {
                    productsList.ForEach(i => { i.Price = Math.Round(i.ShopPrice - (i.ShopPrice * i.Percentage) / 100, MidpointRounding.AwayFromZero); i.MenuPrice = i.ShopPrice; });
                    db.SaveChanges();
                }
            }
            ViewBag.Message = "Successfully Updated!";
            return View();
        }

        public async Task<JsonResult> GetMedicalDiscountCategorySelect2(string q = "", int shopid = 0)
        {
            var model = await db.DiscountCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.ShopId == shopid)
                .Select(i => new
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