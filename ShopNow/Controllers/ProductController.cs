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
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    // [Authorize]
    public class ProductController : Controller
    {
        private ShopnowchatEntities db = new ShopnowchatEntities();
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
                config.CreateMap<ProductCreateViewModel, Product>();
                config.CreateMap<Product, ProductEditViewModel>();
                config.CreateMap<ProductEditViewModel, Product>();
                config.CreateMap<FoodCreateViewModel, Product>();
                config.CreateMap<Product, FoodEditViewModel>();
                config.CreateMap<ServiceCreateEditViewModel, Product>();
                config.CreateMap<Product, ServiceCreateEditViewModel>();
                config.CreateMap<Product, FoodEditViewModel>();
                //config.CreateMap<Product, FoodEditViewModel.AddonList>();
                config.CreateMap<FoodEditViewModel, Product>();
                config.CreateMap<ProductMappingViewModel, Product>();
                config.CreateMap<Product, ProductMappingViewModel.List>();
                config.CreateMap<FMCGCreateEditViewModel, Product>();
                config.CreateMap<Product, FMCGCreateEditViewModel>();
                config.CreateMap<MedicalCreateViewModel, Product>();
                config.CreateMap<Product, MedicalEditViewModel>();
                config.CreateMap<MedicalEditViewModel, Product>();
                config.CreateMap<ProductDishAddOn, ShopDishAddOn>();
                config.CreateMap<MedicalEditViewModel.MedicalStockList, DefaultMedicalStockViewModel>();
                config.CreateMap<Product, ElectronicCreateEditViewModel>();
                config.CreateMap<ElectronicCreateEditViewModel, Product>();
            });

            _mapper = _mapperConfiguration.CreateMapper();
        }

        //[AccessPolicy(PageCode = "SHNPROL006")]
        //public ActionResult List(int? ColumnName, int? AdvanceFilter, string keyword = "")
        //{
        //    var model = new ProductListViewModel();
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    if (ColumnName == 0)
        //    {
        //        model.List = db.Products.Where(i => (AdvanceFilter == 0 ? i.ShopCategoryName.Contains(keyword) : true) && (AdvanceFilter == 1 ? i.ShopCategoryName != keyword : true)
        //        && (AdvanceFilter == 2 ? i.ShopCategoryName != null : true) && (AdvanceFilter == 3 ? i.ShopCategoryName == null : true) && (AdvanceFilter == 4 ? i.ShopCategoryName.EndsWith(keyword) : true)
        //        && (AdvanceFilter == 5 ? i.ShopCategoryName.StartsWith(keyword) : true) && (AdvanceFilter == 6 ? !i.ShopCategoryName.StartsWith(keyword) : true))
        //         .Select(i => new ProductListViewModel.ProductList
        //         {
        //             Code = i.Code,
        //             Name = i.Name,
        //             MasterProductName = i.MasterProductName,
        //             ShopCategoryName = i.ShopCategoryName,
        //             BrandName = i.BrandName,
        //             CategoryName = i.CategoryName,
        //             ImagePath = i.ImagePath,
        //             ProductType = i.ProductType,
        //             ShopName = i.ShopName
        //         }).ToList();
        //    }
        //    else if (ColumnName == 1)
        //    {
        //        model.List = db.Products.Where(i => (AdvanceFilter == 0 ? i.ProductType.Contains(keyword) : true) && (AdvanceFilter == 1 ? i.ProductType != keyword : true)
        //       && (AdvanceFilter == 2 ? i.ProductType != null : true) && (AdvanceFilter == 3 ? i.ProductType == null : true) && (AdvanceFilter == 4 ? i.ProductType.EndsWith(keyword) : true)
        //       && (AdvanceFilter == 5 ? i.ProductType.StartsWith(keyword) : true) && (AdvanceFilter == 6 ? !i.ProductType.StartsWith(keyword) : true))
        //        .Select(i => new ProductListViewModel.ProductList
        //        {
        //            Code = i.Code,
        //            Name = i.Name,
        //            MasterProductName = i.MasterProductName,
        //            ShopCategoryName = i.ShopCategoryName,
        //            BrandName = i.BrandName,
        //            CategoryName = i.CategoryName,
        //            ImagePath = i.ImagePath,
        //            ProductType = i.ProductType,
        //            ShopName = i.ShopName
        //        }).ToList();
        //    }
        //    else if (ColumnName == 2)
        //    {
        //        model.List = db.Products.Where(i => (AdvanceFilter == 0 ? i.Name.Contains(keyword) : true) && (AdvanceFilter == 1 ? i.Name != keyword : true)
        //        && (AdvanceFilter == 2 ? i.Name != null : true) && (AdvanceFilter == 3 ? i.Name == null : true) && (AdvanceFilter == 4 ? i.Name.EndsWith(keyword) : true)
        //        && (AdvanceFilter == 5 ? i.Name.StartsWith(keyword) : true) && (AdvanceFilter == 6 ? !i.Name.StartsWith(keyword) : true))
        //         .Select(i => new ProductListViewModel.ProductList
        //         {
        //             Code = i.Code,
        //             Name = i.Name,
        //             MasterProductName = i.MasterProductName,
        //             ShopCategoryName = i.ShopCategoryName,
        //             BrandName = i.BrandName,
        //             CategoryName = i.CategoryName,
        //             ImagePath = i.ImagePath,
        //             ProductType = i.ProductType,
        //             ShopName = i.ShopName
        //         }).ToList();
        //    }
        //    else if (ColumnName == 3)
        //    {
        //        model.List = db.Products.Where(i => (AdvanceFilter == 0 ? i.CategoryName.Contains(keyword) : true) && (AdvanceFilter == 1 ? i.CategoryName != keyword : true)
        //       && (AdvanceFilter == 2 ? i.CategoryName != null : true) && (AdvanceFilter == 3 ? i.CategoryName == null : true) && (AdvanceFilter == 4 ? i.CategoryName.EndsWith(keyword) : true)
        //       && (AdvanceFilter == 5 ? i.CategoryName.StartsWith(keyword) : true) && (AdvanceFilter == 6 ? !i.CategoryName.StartsWith(keyword) : true))
        //        .Select(i => new ProductListViewModel.ProductList
        //        {
        //            Code = i.Code,
        //            Name = i.Name,
        //            MasterProductName = i.MasterProductName,
        //            ShopCategoryName = i.ShopCategoryName,
        //            BrandName = i.BrandName,
        //            CategoryName = i.CategoryName,
        //            ImagePath = i.ImagePath,
        //            ProductType = i.ProductType,
        //            ShopName = i.ShopName
        //        }).ToList();
        //    }
        //    else if (ColumnName == 4)
        //    {
        //        model.List = db.Products.Where(i => (AdvanceFilter == 0 ? i.BrandName.Contains(keyword) : true) && (AdvanceFilter == 1 ? i.BrandName != keyword : true)
        //        && (AdvanceFilter == 2 ? i.BrandName != null : true) && (AdvanceFilter == 3 ? i.BrandName == null : true) && (AdvanceFilter == 4 ? i.BrandName.EndsWith(keyword) : true)
        //        && (AdvanceFilter == 5 ? i.BrandName.StartsWith(keyword) : true) && (AdvanceFilter == 6 ? !i.BrandName.StartsWith(keyword) : true))
        //         .Select(i => new ProductListViewModel.ProductList
        //         {
        //             Code = i.Code,
        //             Name = i.Name,
        //             MasterProductName = i.MasterProductName,
        //             ShopCategoryName = i.ShopCategoryName,
        //             BrandName = i.BrandName,
        //             CategoryName = i.CategoryName,
        //             ImagePath = i.ImagePath,
        //             ProductType = i.ProductType,
        //             ShopName = i.ShopName
        //         }).ToList();
        //    }
        //    else if (ColumnName == 5)
        //    {
        //        model.List = db.Products.Where(i => (AdvanceFilter == 0 ? i.ShopName.Contains(keyword) : true) && (AdvanceFilter == 1 ? i.ShopName != keyword : true)
        //        && (AdvanceFilter == 2 ? i.ShopName != null : true) && (AdvanceFilter == 3 ? i.ShopName == null : true) && (AdvanceFilter == 4 ? i.ShopName.EndsWith(keyword) : true)
        //        && (AdvanceFilter == 5 ? i.ShopName.StartsWith(keyword) : true) && (AdvanceFilter == 6 ? !i.ShopName.StartsWith(keyword) : true))
        //         .Select(i => new ProductListViewModel.ProductList
        //         {
        //             Code = i.Code,
        //             Name = i.Name,
        //             MasterProductName = i.MasterProductName,
        //             ShopCategoryName = i.ShopCategoryName,
        //             BrandName = i.BrandName,
        //             CategoryName = i.CategoryName,
        //             ImagePath = i.ImagePath,
        //             ProductType = i.ProductType,
        //             ShopName = i.ShopName
        //         }).ToList();
        //    }
        //    else
        //    {
        //        model.List = Product.GetList().AsQueryable().ProjectTo<ProductListViewModel.ProductList>(_mapperConfiguration).OrderBy(i => i.Name).ToList();
        //    }

        //    return View(model);
        //}

        [AccessPolicy(PageCode = "SHNPROL006")]
        public ActionResult List(ProductItemListViewModel model)
        {
            //var user = ((Helpers.Sessions.User)Session["USER"]);
            //ViewBag.Name = user.Name;
            //var shop = db.Shops.FirstOrDefault(i => i.Code == shopcode);// Shop.Get(shopcode);
            //var List = db.Products.Where(i => i.Status == 0 && i.ShopCode == shopcode).OrderBy(i => i.Name).ToList();
            //if (shop != null)
            //{
            //    ViewBag.ShopCode = shopcode;
            //    ViewBag.ShopName = shop.Name;
            //}
            //return View(List);
            var shid = db.Shops.Where(s => s.Code == model.ShopCode).FirstOrDefault();
            if (shid !=null)
            {
                var user = ((Helpers.Sessions.User)Session["USER"]);
                ViewBag.Name = user.Name;

                model.ListItems =     (from i in db.Products
                                       join m in db.MasterProducts on i.MasterProductCode equals m.Code
                                       where i.Status == 0 && i.shopid == shid.Id
                 select new ProductItemListViewModel.ListItem
                 {
                     Id = i.Id,
                     ProductType = i.ProductType,
                     CategoryName = i.CategoryName,
                     BrandName = i.BrandName,
                     Code = i.Code,
                     Name = m.Name,
                     DiscountCategoryPercentage = i.DiscountCategoryPercentage,
                     ShopCode = i.ShopCode,
                     ShopName = i.ShopName
                 }).ToList();

                //    var s=(from p in db.Products where p.Status == 0 && p.shopid == shid.Id)
            }
            else
            {
                model.ListItems = new List<ProductItemListViewModel.ListItem>();
            }
           

            return View(model);
        }
        string GetMasterProductName(string code)
        {
            var masterProduct = db.MasterProducts.FirstOrDefault(i => i.Code == code);
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
                id = i.Code,
                //text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetActiveShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }


        [AccessPolicy(PageCode = "SHNPROGL009")]
        public ActionResult GetList()
        {
            var products = db.Products.Where(i => i.Status == 0).ToList();// Product.GetList();
            return View(products);
        }

        [AccessPolicy(PageCode = "SHNPROD002")]
        public ActionResult Details(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            Product pd = db.Products.FirstOrDefault(i => i.Code == dCode);// Product.Get(dCode);
            var model = new ProductEditViewModel();
            _mapper.Map(pd, model);
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public ActionResult Create()
        {
            Session["DefaultMedicalStock"] = new List<DefaultMedicalStockViewModel>();
            Session["ManualMedicalStock"] = new List<ManualMedicalStockViewModel>();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var shop = db.Shops.Any(i => i.Code == user.Code);
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
        public ActionResult Create(ProductCreateViewModel model)
        {
            var prod = _mapper.Map<ProductCreateViewModel, Product>(model);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.Any(i => i.Code == user.Code);
            if (shop != false)
            {
                prod.ShopCode = user.Code;
                prod.ShopName = user.Name;
                ViewBag.user = user.Code;
            }
            if (model.ShopCode != null)
            {
                var sh = db.Shops.FirstOrDefault(i => i.Code == model.ShopCode);// Shop.Get(model.ShopCode);
                prod.ShopCategoryCode = sh.ShopCategoryCode;
                prod.ShopCategoryName = sh.ShopCategoryName;
            }
            if (prod.ShopCode != null)
            {
                var sh = db.Shops.FirstOrDefault(i => i.Code == prod.ShopCode);// Shop.Get(prod.ShopCode);
                prod.ShopCategoryCode = sh.ShopCategoryCode;
                prod.ShopCategoryName = sh.ShopCategoryName;
            }

            prod.CategoryCode = model.CategoryCode;
            var cat = db.Categories.FirstOrDefault(i => i.Code == model.CategoryCode);//  Category.Get(model.CategoryCode);
            if (cat != null)
            {
                prod.CategoryName = cat.Name;
            }
            prod.BrandCode = model.BrandCode;
            var brand = db.Brands.FirstOrDefault(i => i.Code == model.BrandCode && i.Status == 0);//  Brand.Get(model.BrandCode);
            if (brand != null)
            {
                prod.BrandName = brand.Name;
            }
            if (model.Name == null)
            {
                prod.Name = model.MasterProductName;
            }
            prod.ProductType = "Product";
            prod.CreatedBy = user.Name;
            prod.UpdatedBy = user.Name;
            var name = db.Products.FirstOrDefault(i => i.Name == model.Name && i.Status == 0 && i.ProductType == "Product");// Product.GetElectronicName(model.Name);
            if (name == null)
            {
                //Product.Add(prod);
                prod.MainSNCode = ShopNow.Helpers.DRC.Generate("PRO");
                prod.DateEncoded = DateTime.Now;
                prod.DateUpdated = DateTime.Now;
                prod.Status = 0;
                db.Products.Add(prod);
                db.SaveChanges();
                ViewBag.Message = model.Name + " Saved Successfully!";
            }
            else
            {
                ViewBag.ErrorMessage = model.Name + " Already Exist";
            }

            var model1 = new SpecificationCreateViewModel();
            model1.Lists = db.ProductSpecifications.Where(i => i.MasterProductCode == prod.MasterProductCode && i.Status == 0).Select(i => new SpecificationCreateViewModel.List
            {
                MasterProductCode = i.MasterProductCode,
                MasterProductName = i.MasterProductName,
                SpecificationCode = i.SpecificationCode,
                SpecificationName = i.SpecificationName,
                Value = i.Value
            }).ToList();
            var psi = new ProductSpecificationItem();
            foreach (var s in model1.Lists)
            {
                psi.ProductCode = prod.Code;
                psi.ProductName = prod.Name;
                psi.ShopCode = prod.ShopCode;
                psi.ShopName = prod.ShopName;
                psi.MasterProductCode = s.MasterProductCode;
                psi.MasterProductName = s.MasterProductName;
                psi.SpecificationCode = s.SpecificationCode;
                psi.SpecificationName = s.SpecificationName;
                psi.Value = s.Value;
                psi.CreatedBy = user.Name;
                psi.UpdatedBy = user.Name;
                // ProductSpecificationItem.Add(psi, out int errorCode);
                psi.Code = ShopNow.Helpers.DRC.Generate("PSI");
                psi.Status = 0;
                psi.DateEncoded = DateTime.Now;
                psi.DateUpdated = DateTime.Now;
                db.ProductSpecificationItems.Add(psi);
                db.SaveChanges();
            }

            if (model.ShopCode != null)
            {
                var sh = db.Shops.FirstOrDefault(i => i.Code == model.ShopCode);// Shop.Get(model.ShopCode);
                var productcount = db.Products.Where(i => i.ShopCode == model.ShopCode && i.Status == 0).Count();
                if (productcount >= 10 && sh.Status == 1)
                {
                    Payment payment = new Payment();
                    payment.CustomerCode = sh.CustomerCode;
                    payment.CustomerName = sh.CustomerName;
                    payment.ShopCode = sh.Code;
                    payment.ShopName = sh.Name;
                    payment.Address = sh.Address;
                    payment.CountryName = sh.CountryName;
                    payment.CreatedBy = sh.CustomerName;
                    payment.UpdatedBy = sh.CustomerName;
                    payment.GSTINNumber = sh.GSTINNumber;
                    payment.Credits = "PlatForm Credits";
                    payment.OriginalAmount = 1000;
                    payment.Amount = 1000;
                    payment.PaymentResult = "success";
                    // Payment.Add(payment, out int error);
                    payment.Code = ShopNow.Helpers.DRC.Generate("PAY");
                    payment.DateEncoded = DateTime.Now;
                    payment.DateUpdated = DateTime.Now;
                    payment.Status = 0;
                    db.Payments.Add(payment);
                    db.SaveChanges();
                    sh.Status = 0;
                    sh.UpdatedBy = user.Name;
                    sh.DateUpdated = DateTime.Now;
                    //Shop.Edit(sh, out int errorcode);
                    sh.DateUpdated = DateTime.Now;
                    db.Entry(sh).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            try
            {
                var productImage = db.Products.FirstOrDefault(i => i.Code == prod.Code);// Product.Get(prod.Code);
                //ProductImage1
                if (model.ProductImage1 != null)
                {
                    uc.UploadFiles(model.ProductImage1.InputStream, prod.Code + "_" + model.ProductImage1.FileName, accesskey, secretkey, "image");
                    productImage.ImagePathLarge1 = prod.Code + "_" + model.ProductImage1.FileName.Replace(" ", "");
                }

                //ProductImage2
                if (model.ProductImage2 != null)
                {
                    uc.UploadFiles(model.ProductImage2.InputStream, prod.Code + "_" + model.ProductImage2.FileName, accesskey, secretkey, "image");
                    productImage.ImagePathLarge2 = prod.Code + "_" + model.ProductImage2.FileName.Replace(" ", "");
                }

                //ProductImage3
                if (model.ProductImage3 != null)
                {
                    uc.UploadFiles(model.ProductImage3.InputStream, prod.Code + "_" + model.ProductImage3.FileName, accesskey, secretkey, "image");
                    productImage.ImagePathLarge3 = prod.Code + "_" + model.ProductImage3.FileName.Replace(" ", "");
                }

                //ProductImage4
                if (model.ProductImage4 != null)
                {
                    uc.UploadFiles(model.ProductImage4.InputStream, prod.Code + "_" + model.ProductImage4.FileName, accesskey, secretkey, "image");
                    productImage.ImagePathLarge4 = prod.Code + "_" + model.ProductImage4.FileName.Replace(" ", "");
                }

                //ProductImage5
                if (model.ProductImage5 != null)
                {
                    uc.UploadFiles(model.ProductImage5.InputStream, prod.Code + "_" + model.ProductImage5.FileName, accesskey, secretkey, "image");
                    productImage.ImagePathLarge5 = prod.Code + "_" + model.ProductImage5.FileName.Replace(" ", "");
                }
                if (model.ProductImage1 != null || model.ProductImage2 != null || model.ProductImage3 != null || model.ProductImage4 != null || model.ProductImage5 != null)
                {
                    // Product.Edit(productImage, out int error);
                    productImage.DateUpdated = DateTime.Now;
                    db.Entry(productImage).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    ViewBag.Message = "Check the provided AWS Credentials.";
                }
                else
                {
                    ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                }
            }
            return View();
        }

        [AccessPolicy(PageCode = "SHNPROE003")]
        public ActionResult Edit(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            if (string.IsNullOrEmpty(dCode))
                return HttpNotFound();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var product = db.Products.FirstOrDefault(i => i.Code == dCode);// Product.Get(dCode);
            var model = _mapper.Map<Product, ProductEditViewModel>(product);
            model.SpecificationLists = db.ProductSpecificationItems.Where(i => i.ProductCode == dCode && i.Status == 0).Select(i => new ProductEditViewModel.SpecificationList
            {
                MasterProductCode = i.MasterProductCode,
                MasterProductName = i.MasterProductName,
                SpecificationCode = i.SpecificationCode,
                SpecificationName = i.SpecificationName,
                Value = i.Value
            }).ToList();
            model.MasterProductName = CommonHelpers.GetMasterProductName(model.MasterProductCode);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNPROE003")]
        public ActionResult Edit(ProductEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            Product prod = db.Products.FirstOrDefault(i => i.Code == model.Code);// Product.Get(model.Code);
            _mapper.Map(model, prod);
            var cat = db.Categories.FirstOrDefault(i => i.Code == model.CategoryCode);// Category.Get(model.CategoryCode);
            if (cat != null)
            {
                prod.CategoryName = cat.Name;
            }

            var brand = db.Brands.FirstOrDefault(i => i.Code == model.BrandCode && i.Status == 0);//  Brand.Get(model.BrandCode);
            if (brand != null)
            {
                prod.BrandName = brand.Name;
            }
            prod.DateUpdated = DateTime.Now;
            prod.UpdatedBy = user.Name;

            try
            {
                //ProductImage1
                if (model.ProductImage1 != null)
                {
                    uc.UploadFiles(model.ProductImage1.InputStream, prod.Code + "_" + model.ProductImage1.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge1 = prod.Code + "_" + model.ProductImage1.FileName.Replace(" ", "");
                }

                //ProductImage2
                if (model.ProductImage2 != null)
                {
                    uc.UploadFiles(model.ProductImage2.InputStream, prod.Code + "_" + model.ProductImage2.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge2 = prod.Code + "_" + model.ProductImage2.FileName.Replace(" ", "");
                }

                //ProductImage3
                if (model.ProductImage3 != null)
                {
                    uc.UploadFiles(model.ProductImage3.InputStream, prod.Code + "_" + model.ProductImage3.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge3 = prod.Code + "_" + model.ProductImage3.FileName.Replace(" ", "");
                }

                //ProductImage4
                if (model.ProductImage4 != null)
                {
                    uc.UploadFiles(model.ProductImage4.InputStream, prod.Code + "_" + model.ProductImage4.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge4 = prod.Code + "_" + model.ProductImage4.FileName.Replace(" ", "");
                }

                //ProductImage5
                if (model.ProductImage5 != null)
                {
                    uc.UploadFiles(model.ProductImage5.InputStream, prod.Code + "_" + model.ProductImage5.FileName, accesskey, secretkey, "image");
                    prod.ImagePathLarge5 = prod.Code + "_" + model.ProductImage5.FileName.Replace(" ", "");
                }
                //  Product.Edit(prod, out int error);
                prod.DateUpdated = DateTime.Now;
                db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    ViewBag.Message = "Check the provided AWS Credentials.";
                }
                else
                {
                    ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                }
            }
            return RedirectToAction("ElectronicList", "Product");
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public ActionResult MedicalCreate()
        {
            Session["MedicalStock"] = new List<DefaultMedicalStockViewModel>();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var shop = db.Shops.Any(i => i.Code == user.Code);
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
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _mapper.Map<MedicalCreateViewModel, Product>(model);
            prod.Code = _generatedCode("PRO");
            var name = db.Products.FirstOrDefault(i => i.Name == model.MasterProductName && i.Status == 0 && i.ProductType == "Medical" && i.ShopCode == model.ShopCode);
            var master = db.MasterProducts.FirstOrDefault(i => i.Code == model.MasterProductCode && i.Status == 0);
            if (master != null)
                prod.MasterProductId = master.Id;
            prod.Name = model.MasterProductName;
            if (model.ShopCode != null)
            {
                var sh = db.Shops.FirstOrDefault(i => i.Code == model.ShopCode);// Shop.Get(model.ShopCode);
                prod.ShopCategoryCode = sh.ShopCategoryCode;
                prod.ShopCategoryName = sh.ShopCategoryName;
                prod.shopid = sh.Id;
            }
            prod.ProductType = "Medical";
            prod.CreatedBy = user.Name;
            prod.UpdatedBy = user.Name;
            if (name == null)
            {
                prod.MainSNCode = ShopNow.Helpers.DRC.Generate("PRO");
                prod.DateEncoded = DateTime.Now;
                prod.DateUpdated = DateTime.Now;
                prod.Status = 0;
                db.Products.Add(prod);
                db.SaveChanges();
                ViewBag.Message = model.MasterProductName + " Saved Successfully!";
            }
            else
            {
                ViewBag.ErrorMessage = model.MasterProductName + " Already Exist";
            }

            List<DefaultMedicalStockViewModel> dms = Session["MedicalStock"] as List<DefaultMedicalStockViewModel>;
            var pms = new ProductMedicalStock();
            foreach (var s in dms)
            {
                pms.ProductCode = prod.Code;
                pms.productid = prod.Id;
                pms.ProductName = prod.Name;
                pms.Stock = s.Stock;
                pms.SupplierName = s.SupplierName;
                pms.MRP = s.MRP;
                pms.SalePrice = s.SalePrice;
                pms.TaxPercentage = s.TaxPercentage;
                pms.DiscountPercentage = s.DiscountPercentage;
                pms.LoyaltyPointsper100Value = s.LoyaltyPointsper100Value;
                pms.MinimumLoyaltyReducationPercentage = s.MinimumLoyaltyReducationPercentage;
                pms.SpecialCostOfDelivery = s.SpecialCostOfDelivery;
                pms.OutLetId = s.OutLetId;
                pms.SpecialPrice = s.SpecialPrice;
                pms.MinSaleQty = s.MinSaleQty;
                pms.CreatedBy = user.Name;
                pms.UpdatedBy = user.Name;
                pms.Code = ShopNow.Helpers.DRC.Generate("PMS");
                pms.Status = 0;
                pms.DateEncoded = DateTime.Now;
                pms.DateUpdated = DateTime.Now;
                db.ProductMedicalStocks.Add(pms);
                db.SaveChanges();
            }
            if (model.ShopCode != null)
            {
                var sh = db.Shops.FirstOrDefault(i => i.Code == model.ShopCode);// Shop.Get(model.ShopCode);
                var productcount = db.Products.Where(i => i.ShopCode == model.ShopCode && i.Status == 0).Count();
                if (productcount >= 10 && sh.Status == 1)
                {
                    Payment payment = new Payment();
                    payment.CustomerCode = sh.CustomerCode;
                    payment.CustomerName = sh.CustomerName;
                    payment.ShopCode = sh.Code;
                    payment.ShopName = sh.Name;
                    payment.Address = sh.Address;
                    payment.CountryName = sh.CountryName;
                    payment.CreatedBy = sh.CustomerName;
                    payment.UpdatedBy = sh.CustomerName;
                    payment.GSTINNumber = sh.GSTINNumber;
                    payment.Credits = "PlatForm Credits";
                    payment.OriginalAmount = 1000;
                    payment.Amount = 1000;
                    payment.PaymentResult = "success";
                    //Payment.Add(payment, out int error);
                    payment.Code = ShopNow.Helpers.DRC.Generate("PAY");
                    payment.DateEncoded = DateTime.Now;
                    payment.DateUpdated = DateTime.Now;
                    payment.Status = 0;
                    db.Payments.Add(payment);
                    db.SaveChanges();
                    sh.Status = 0;
                    sh.UpdatedBy = user.Name;
                    sh.DateUpdated = DateTime.Now;
                    //Shop.Edit(sh, out int errorcode);
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
            Session["MedicalStock"] = new List<DefaultMedicalStockViewModel>();
            var Medicalstocks = new List<DefaultMedicalStockViewModel>();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var product = db.Products.FirstOrDefault(i => i.Id == dCode);
            var model = _mapper.Map<Product, MedicalEditViewModel>(product);
            model.MedicalStockLists = db.ProductMedicalStocks.Where(i => i.productid == dCode && i.Status == 0).Select(i => new MedicalEditViewModel.MedicalStockList
            {
               ProductCode = i.ProductCode,
               ProductName = i.ProductName,
               Code = i.Code,
               Stock = i.Stock,
               SupplierName = i.SupplierName,
               LoyaltyPointsper100Value = i.LoyaltyPointsper100Value,
               MinimumLoyaltyReducationPercentage = i.MinimumLoyaltyReducationPercentage,
               TaxPercentage = i.TaxPercentage,
               DiscountPercentage = i.DiscountPercentage,
               MRP = i.MRP,
               SalePrice = i.SalePrice,
               SpecialPrice = i.SpecialPrice,
               MinSaleQty = i.MinSaleQty,
               OutLetId = i.OutLetId,
               SpecialCostOfDelivery = i.SpecialCostOfDelivery,
               productid = i.productid
            }).ToList();
            foreach (var s in model.MedicalStockLists)
            {
                var stock = _mapper.Map<MedicalEditViewModel.MedicalStockList, DefaultMedicalStockViewModel>(s);
                Medicalstocks.Add(stock);
            }
            Session["MedicalStock"] = Medicalstocks;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNPROME008")]
        public ActionResult MedicalEdit(MedicalEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            Product prod = db.Products.FirstOrDefault(i => i.Code == model.Code);
            _mapper.Map(model, prod);
            
            prod.DateUpdated = DateTime.Now;
            prod.UpdatedBy = user.Name;
            db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            List<DefaultMedicalStockViewModel> dms = Session["MedicalStock"] as List<DefaultMedicalStockViewModel>;
            var pms = new ProductMedicalStock();
            foreach (var s in dms)
            {
                if (s.Code == null)
                {
                    pms.Code = ShopNow.Helpers.DRC.Generate("PMS");
                    pms.ProductCode = prod.Code;
                    pms.productid = prod.Id;
                    pms.ProductName = prod.Name;
                    pms.Stock = s.Stock;
                    pms.SupplierName = s.SupplierName;
                    pms.MRP = s.MRP;
                    pms.SalePrice = s.SalePrice;
                    pms.TaxPercentage = s.TaxPercentage;
                    pms.DiscountPercentage = s.DiscountPercentage;
                    pms.LoyaltyPointsper100Value = s.LoyaltyPointsper100Value;
                    pms.MinimumLoyaltyReducationPercentage = s.MinimumLoyaltyReducationPercentage;
                    pms.SpecialCostOfDelivery = s.SpecialCostOfDelivery;
                    pms.OutLetId = s.OutLetId;
                    pms.SpecialPrice = s.SpecialPrice;
                    pms.MinSaleQty = s.MinSaleQty;
                    pms.CreatedBy = user.Name;
                    pms.UpdatedBy = user.Name;
                    pms.Status = 0;
                    pms.DateEncoded = DateTime.Now;
                    pms.DateUpdated = DateTime.Now;
                    db.ProductMedicalStocks.Add(pms);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("MedicalList", "Product");
        }

        public ActionResult FMCGList(FMCGListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Products.Where(i => i.Status == 0 && i.ProductType == "FMCG" && (model.ShopCode != null ? i.ShopCode == model.ShopCode : false))
                .AsEnumerable()
            .Select(i => new FMCGListViewModel.ListItem
            {
                CategoryName = i.CategoryName,
                Code = i.Code,
                Name = CommonHelpers.GetMasterProductName(i.MasterProductCode),
                Percentage = i.Percentage,
                ShopCode = i.ShopCode,
                ShopName = i.ShopName
            }).ToList();
            return View(model);
        }

        public ActionResult MedicalList(MedicalListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Products.Where(i => i.Status == 0 &&
            i.ProductType == "Medical" &&
            (model.ShopCode != null ? i.ShopCode == model.ShopCode : false))
            .AsEnumerable()
            .Select(i => new MedicalListViewModel.ListItem
            {
                Id = i.Id,
                CategoryName = i.CategoryName,
                Code = i.Code,
                Name = CommonHelpers.GetMasterProductName(i.MasterProductCode),
                Percentage = i.Percentage,
                ShopCode = i.ShopCode,
                ShopName = i.ShopName
            }).ToList();
            return View(model);
        }

        public ActionResult FoodList(FoodListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Products.Where(i => i.Status == 0 &&
            i.ProductType == "Dish" &&
            (model.ShopCode != null ? i.ShopCode == model.ShopCode : false))
            .AsEnumerable()
            .Select(i => new FoodListViewModel.ListItem
            {
                CategoryName = i.CategoryName,
                Code = i.Code,
                Name = CommonHelpers.GetMasterProductName(i.MasterProductCode),
                Percentage = i.Percentage,
                ShopCode = i.ShopCode,
                ShopName = i.ShopName
            }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNPROFC004")]
        public ActionResult FoodCreate()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var shop = db.Shops.Any(i => i.Code == user.Code);
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
            var name = db.Products.FirstOrDefault(i => i.Name == model.Name && i.Status == 0 && i.ProductType == "Dish" && i.shopid == model.ShopId);
            if (name != null)
            {
                ViewBag.ErrorMessage = model.Name + " Already Exist";
                return View();
            }

            var prod = _mapper.Map<FoodCreateViewModel, Product>(model);
            prod.Code = _generatedCode("PRO");
            var user = ((Helpers.Sessions.User)Session["USER"]);
            if (model.ShopId != 0)
            {
                var sh = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);// Shop.Get(model.ShopCode);
                prod.ShopCategoryCode = sh.ShopCategoryCode;
                prod.ShopCategoryName = sh.ShopCategoryName;
                prod.shopid = sh.Id;
            }
            prod.ProductType = "Dish";
            prod.Status = 0;
            prod.CreatedBy = user.Name;
            prod.UpdatedBy = user.Name;
            if (name == null)
            {
                prod.MainSNCode = ShopNow.Helpers.DRC.Generate("SNC");
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
                        var productDishAddon = db.ProductDishAddOns.FirstOrDefault(i => i.Code == item.Code);
                        var shopdishAddOn = _mapper.Map<ProductDishAddOn, ShopDishAddOn>(productDishAddon);
                        shopdishAddOn.PortionPrice = item.PortionPrice;
                        shopdishAddOn.AddOnsPrice = item.AddOnsPrice;
                        shopdishAddOn.CrustPrice = item.CrustPrice;
                        shopdishAddOn.IsActive = true;
                        shopdishAddOn.ProductId = prod.Id;
                        shopdishAddOn.ProductName = prod.Name;
                        shopdishAddOn.Shopid = prod.shopid;
                        shopdishAddOn.ShopName = prod.ShopName;
                        shopdishAddOn.ProductDishAddonId = productDishAddon.Id;
                        db.ShopDishAddOns.Add(shopdishAddOn);
                        db.SaveChanges();
                    }
                }

                if (model.ShopId != 0)
                {
                    var sh = db.Shops.FirstOrDefault(i => i.Id == model.ShopId);// Shop.Get(model.ShopCode);
                    var productcount = db.Products.Where(i => i.shopid == model.ShopId && i.Status == 0).Count();
                    if (productcount >= 10 && sh.Status == 1)
                    {
                        Payment payment = new Payment();
                        payment.CustomerCode = sh.CustomerCode;
                        payment.CustomerName = sh.CustomerName;
                        payment.ShopCode = sh.Code;
                        payment.ShopName = sh.Name;
                        payment.Address = sh.Address;
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
                        payment.Code = ShopNow.Helpers.DRC.Generate("PAY");
                        payment.DateEncoded = DateTime.Now;
                        payment.DateUpdated = DateTime.Now;
                        payment.Status = 0;
                        db.Payments.Add(payment);
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
            }
            return View();
        }

        [AccessPolicy(PageCode = "SHNPROFE005")]
        public ActionResult FoodEdit(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            if (string.IsNullOrEmpty(dCode))
                return HttpNotFound();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var product = db.Products.FirstOrDefault(i => i.Code == dCode);
            var model = _mapper.Map<Product, FoodEditViewModel>(product);
            if (model.MasterProductCode != null)
            {
                model.MasterProductName = CommonHelpers.GetMasterProductName(model.MasterProductCode);
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
            var shop = db.Shops.Any(i => i.Code == user.Code);
            var prod = db.Products.FirstOrDefault(i => i.Code == model.Code);
            _mapper.Map(model, prod);
            prod.DateUpdated = DateTime.Now;
            prod.UpdatedBy = user.Name;
            db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            //Addons
            if (prod.Customisation == true)
            {
                List<ShopAddOnSessionEditViewModel> shopAddOns = Session["ShopAddOnsEdit"] as List<ShopAddOnSessionEditViewModel> ?? new List<ShopAddOnSessionEditViewModel>();
                foreach (var item in shopAddOns)
                {
                    var shopDishAddon = db.ShopDishAddOns.FirstOrDefault(i => i.Code == item.Code);
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
                        var productDishAddon = db.ProductDishAddOns.FirstOrDefault(i => i.Code == item.Code);
                        var shopdishAddOn = _mapper.Map<ProductDishAddOn, ShopDishAddOn>(productDishAddon);
                        shopdishAddOn.PortionPrice = item.PortionPrice;
                        shopdishAddOn.AddOnsPrice = item.AddOnsPrice;
                        shopdishAddOn.CrustPrice = item.CrustPrice;
                        shopdishAddOn.IsActive = true;
                        shopdishAddOn.ProductId = prod.Id;
                        shopdishAddOn.ProductName = prod.Name;
                        shopdishAddOn.Shopid = prod.shopid;
                        shopdishAddOn.ShopName = prod.ShopName;
                        shopdishAddOn.ProductDishAddonId = productDishAddon.Id;
                        db.ShopDishAddOns.Add(shopdishAddOn);
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                var shopDishAddonList = db.ShopDishAddOns.Where(i => i.ProductId == prod.Id);
                foreach (var item in shopDishAddonList)
                {
                    var shopDishAddon = db.ShopDishAddOns.FirstOrDefault(i => i.Code == item.Code);
                    shopDishAddon.IsActive = false;
                    db.Entry(shopDishAddon).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("FoodList", "Product",new { ShopCode = prod.ShopCode,ShopName = prod.ShopName });
        }

        public JsonResult GetProductDishAddOns(int masterProductId)
        {
            var list = db.ProductDishAddOns.Where(i => i.MasterProductId == masterProductId && i.Status == 0).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //Food Create Sessions
        public JsonResult AddShopAddOns(ShopAddOnSessionAddViewModel model)
        {
            List<ShopAddOnSessionAddViewModel> shopAddOns = Session["ShopAddOns"] as List<ShopAddOnSessionAddViewModel> ?? new List<ShopAddOnSessionAddViewModel>();
            if (model.Code != null)
            {
                var addOn = new ShopAddOnSessionAddViewModel
                {
                    AddOnsPrice = model.AddOnsPrice,
                    Code = model.Code,
                    CrustPrice = model.CrustPrice,
                    PortionPrice = model.PortionPrice
                };
                shopAddOns.Add(addOn);
            }
            Session["ShopAddOns"] = shopAddOns;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveFromShopAddOns(string code)
        {
            List<ShopAddOnSessionAddViewModel> shopAddOns = Session["ShopAddOns"] as List<ShopAddOnSessionAddViewModel> ?? new List<ShopAddOnSessionAddViewModel>();

            if (shopAddOns.Remove(shopAddOns.SingleOrDefault(i => i.Code == code)))
                Session["ShopAddOns"] = shopAddOns;

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //Food Edit Sessions
        public JsonResult EditShopAddOns(ShopAddOnSessionEditViewModel model)
        {
            List<ShopAddOnSessionEditViewModel> shopAddOns = Session["ShopAddOnsEdit"] as List<ShopAddOnSessionEditViewModel> ?? new List<ShopAddOnSessionEditViewModel>();
            if (model.Code != null)
            {
                var addOn = new ShopAddOnSessionEditViewModel
                {
                    AddOnsPrice = model.AddOnsPrice,
                    Code = model.Code,
                    CrustPrice = model.CrustPrice,
                    PortionPrice = model.PortionPrice,
                    IsActive = model.IsActive
                };
                shopAddOns.Add(addOn);
            }
            Session["ShopAddOnsEdit"] = shopAddOns;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveFromEditShopAddOns(string code)
        {
            List<ShopAddOnSessionEditViewModel> shopAddOns = Session["ShopAddOnsEdit"] as List<ShopAddOnSessionEditViewModel> ?? new List<ShopAddOnSessionEditViewModel>();

            if (shopAddOns.Remove(shopAddOns.SingleOrDefault(i => i.Code == code)))
                Session["ShopAddOnsEdit"] = shopAddOns;

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //get product Addons which are not added in Shop Addons
        public JsonResult GetProductDishAddOnsFoodEdit(int masterProductId)
        {
            var list = (from p in db.ProductDishAddOns
                        where (p.MasterProductId == masterProductId && p.Status==0) && !db.ShopDishAddOns.Any(i => i.ProductDishAddonId == p.Id)
                        select p).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShopDishAddOns(int productId)
        {
            var list = db.ShopDishAddOns.Where(i => i.ProductId == productId && i.Status == 0).ToList();

            //Initial add to session
            //List<ShopAddOnSessionEditViewModel> shopAddOns = Session["ShopAddOnsEdit"] as List<ShopAddOnSessionEditViewModel> ?? new List<ShopAddOnSessionEditViewModel>();
            //foreach (var item in list)
            //{
            //    var addon = new ShopAddOnSessionEditViewModel
            //    {
            //        AddOnsPrice = item.AddOnsPrice,
            //        Code = item.Code,
            //        CrustPrice = item.CrustPrice,
            //        PortionPrice = item.PortionPrice,
            //        IsActive = item.IsActive.Value
            //    };
            //    shopAddOns.Add(addon);
            //}

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult FMCGCreate()
        {
           // Session["DefaultFMCG"] = new List<FMCGCreateEditViewModel>();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var shop = db.Shops.Any(i => i.Code == user.Code);
            if (shop != false)
            {
                ViewBag.user = user.Name;
            }

            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "")]
        public ActionResult FMCGCreate(FMCGCreateEditViewModel model)
        {
            //Return to View if product already exist
            var name = db.Products.FirstOrDefault(i => i.Name == model.Name && i.Status == 0 && i.ProductType == "FMCG" && i.ShopCode == model.ShopCode);
            if (name != null)
            {
                ViewBag.ErrorMessage = model.Name + " Already Exist";
                return View();
            }

            var user = ((Helpers.Sessions.User)Session["USER"]);
            var product = _mapper.Map<FMCGCreateEditViewModel, Product>(model);
            product.CreatedBy = user.Name;
            product.UpdatedBy = user.Name;
            product.ProductType = "FMCG";
            product.Code = _generatedCode("PRO");
            if (model.ShopCode != null)
            {
                var shop = db.Shops.FirstOrDefault(i => i.Code == model.ShopCode);
                product.shopid = shop.Id;
                product.ShopCategoryCode = shop.ShopCategoryCode;
                product.ShopCategoryName = shop.ShopCategoryName;
            }
            product.DateEncoded = DateTime.Now;
            product.DateUpdated = DateTime.Now;
            product.Status = 0;
            db.Products.Add(product);
            db.SaveChanges();
            ViewBag.Message = model.Name + " Saved Successfully!";
            if (model.ShopCode != null)
            {
                var sh = db.Shops.FirstOrDefault(i => i.Code == model.ShopCode);// Shop.Get(model.ShopCode);
                var productcount = db.Products.Where(i => i.ShopCode == model.ShopCode && i.Status == 0).Count();
                if (productcount >= 10 && sh.Status == 1)
                {
                    Payment payment = new Payment();
                    payment.CustomerCode = sh.CustomerCode;
                    payment.CustomerName = sh.CustomerName;
                    payment.ShopCode = sh.Code;
                    payment.ShopName = sh.Name;
                    payment.Address = sh.Address;
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
                    payment.Code = ShopNow.Helpers.DRC.Generate("PAY");
                    payment.DateEncoded = DateTime.Now;
                    payment.DateUpdated = DateTime.Now;
                    payment.Status = 0;
                    db.Payments.Add(payment);
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

        public ActionResult FMCGEdit(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dCode))
                return HttpNotFound();
            var product = db.Products.FirstOrDefault(i => i.Code == dCode);
            if (product != null)
            {
                if (product.ImagePathLarge1 != null)
                    product.ImagePathLarge1 = product.ImagePathLarge1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
                if (product.ImagePathLarge2 != null)
                    product.ImagePathLarge2 = product.ImagePathLarge2.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
                if (product.ImagePathLarge3 != null)
                    product.ImagePathLarge3 = product.ImagePathLarge3.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
                if (product.ImagePathLarge4 != null)
                    product.ImagePathLarge4 = product.ImagePathLarge4.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
                if (product.ImagePathLarge5 != null)
                    product.ImagePathLarge5 = product.ImagePathLarge5.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            }
            var model = _mapper.Map<Product, FMCGCreateEditViewModel>(product);
            model.MasterProductName = CommonHelpers.GetMasterProductName(model.MasterProductCode);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "")]
        public ActionResult FMCGEdit(FMCGCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = db.Products.FirstOrDefault(i => i.Code == model.Code);
            _mapper.Map(model, prod);
            prod.Name = model.Name;
            prod.ProductType = model.ProductType;
            prod.UpdatedBy = user.Name;
            prod.DateUpdated = DateTime.Now;
            prod.DateUpdated = DateTime.Now;
            db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("FMCGList", new { shopCode = prod.ShopCode, shopName = prod.ShopName });
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult ElectronicCreate()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var shop = db.Shops.Any(i => i.Code == user.Code);
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
            var name = db.Products.FirstOrDefault(i => i.Name == model.Name && i.Status == 0 && i.ProductType == "Product" && i.ShopCode == model.ShopCode);
            if (name != null)
            {
                ViewBag.ErrorMessage = model.Name + " Already Exist";
                return View();
            }

            var user = ((Helpers.Sessions.User)Session["USER"]);
            var product = _mapper.Map<ElectronicCreateEditViewModel, Product>(model);
            product.CreatedBy = user.Name;
            product.UpdatedBy = user.Name;
            product.ProductType = "Product";
            product.Code = _generatedCode("PRO");
            if (model.ShopCode != null)
            {
                var shop = db.Shops.FirstOrDefault(i => i.Code == model.ShopCode);
                product.shopid = shop.Id;
                product.ShopCategoryCode = shop.ShopCategoryCode;
                product.ShopCategoryName = shop.ShopCategoryName;
            }
            product.DateEncoded = DateTime.Now;
            product.DateUpdated = DateTime.Now;
            product.Status = 0;
            db.Products.Add(product);
            db.SaveChanges();
            ViewBag.Message = model.Name + " Saved Successfully!";
            if (model.ShopCode != null)
            {
                var sh = db.Shops.FirstOrDefault(i => i.Code == model.ShopCode);// Shop.Get(model.ShopCode);
                var productcount = db.Products.Where(i => i.ShopCode == model.ShopCode && i.Status == 0).Count();
                if (productcount >= 10 && sh.Status == 1)
                {
                    Payment payment = new Payment();
                    payment.CustomerCode = sh.CustomerCode;
                    payment.CustomerName = sh.CustomerName;
                    payment.ShopCode = sh.Code;
                    payment.ShopName = sh.Name;
                    payment.Address = sh.Address;
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
                    payment.Code = ShopNow.Helpers.DRC.Generate("PAY");
                    payment.DateEncoded = DateTime.Now;
                    payment.DateUpdated = DateTime.Now;
                    payment.Status = 0;
                    db.Payments.Add(payment);
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

        public ActionResult ElectronicEdit(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dCode))
                return HttpNotFound();
            var product = db.Products.FirstOrDefault(i => i.Code == dCode);
            if (product != null)
            {
                if (product.ImagePathLarge1 != null)
                    product.ImagePathLarge1 = product.ImagePathLarge1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
                if (product.ImagePathLarge2 != null)
                    product.ImagePathLarge2 = product.ImagePathLarge2.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
                if (product.ImagePathLarge3 != null)
                    product.ImagePathLarge3 = product.ImagePathLarge3.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
                if (product.ImagePathLarge4 != null)
                    product.ImagePathLarge4 = product.ImagePathLarge4.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
                if (product.ImagePathLarge5 != null)
                    product.ImagePathLarge5 = product.ImagePathLarge5.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            }
            var model = _mapper.Map<Product, ElectronicCreateEditViewModel>(product);
            model.MasterProductName = CommonHelpers.GetMasterProductName(model.MasterProductCode);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "")]
        public ActionResult ElectronicEdit(ElectronicCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = db.Products.FirstOrDefault(i => i.Code == model.Code);
            _mapper.Map(model, prod);
            prod.Name = model.Name;
            prod.ProductType = model.ProductType;
            prod.UpdatedBy = user.Name;
            prod.DateUpdated = DateTime.Now;
            prod.DateUpdated = DateTime.Now;
            db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("ElectronicList", new { shopCode = prod.ShopCode, shopName = prod.ShopName });
        }

        public ActionResult ElectronicList(ElectronicListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.Products.Where(i => i.Status == 0 && i.ProductType == "Product" && (model.ShopCode != null ? i.ShopCode == model.ShopCode : false))
                .AsEnumerable()
            .Select(i => new ElectronicListViewModel.ListItem
            {
                CategoryName = i.CategoryName,
                Code = i.Code,
                Name = CommonHelpers.GetMasterProductName(i.MasterProductCode),
                Percentage = i.Percentage,
                ShopCode = i.ShopCode,
                ShopName = i.ShopName
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
           // int error = 0;

            var prod = _mapper.Map<ServiceCreateEditViewModel, Product>(pd);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.Any(i => i.Code == user.Code);
            if (shop != false)
            {
                prod.ShopCode = user.Code;
                prod.ShopName = user.Name;
                ViewBag.user = user.Code;
            }
            prod.CategoryCode = pd.CategoryCode;
            var cat = db.Categories.FirstOrDefault(i => i.Code == pd.CategoryCode);//  Category.Get(pd.CategoryCode);
            if (cat != null)
            {
                prod.CategoryName = cat.Name;
            }

            //prod.ImagePath = _ProductImage != null ? _ProductImage.FileName : prod.ImagePath;

            prod.ProductType = "Service";

            if (prod.Code == "" || prod.Code == null)
            {
                prod.Status = 0;
                // prod.Code = Product.Add(prod);
                prod.MainSNCode = ShopNow.Helpers.DRC.Generate("PRO");
                prod.Code = prod.MainSNCode;
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
        public ActionResult Delete(string code, int redirectPage=0) //redirectPage : 0-Product List, 1-Food List, 2-Medical List, 3-FMCG List , 4-Electronic List
        {
            var dCode = AdminHelpers.DCode(code);
            var product = db.Products.FirstOrDefault(i => i.Code == dCode);// Product.Get(dCode);
            product.Status = 2;
            product.DateUpdated = DateTime.Now;
            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            switch (redirectPage)
            {
                case 1:
                    return RedirectToAction("FoodList", "Product");
                case 2:
                    return RedirectToAction("MedicalList", "Product");
                case 3:
                    return RedirectToAction("FMCGList", "Product");
                case 4:
                    return RedirectToAction("ElectronicList", "Product");
                default:
                    return RedirectToAction("List", "Product");

            }
        }

        // Food Create(Dish) Json Result

        [AccessPolicy(PageCode = "SHNPROAO014")]
        public JsonResult AddOns(string code)
        {
            var model = new AddOnsCreateViewModel();
            model.DishLists = db.DishAddOns.Where(i => i.MasterProductCode == code && i.Status == 0).Select(i => new AddOnsCreateViewModel.DishList
            {
                Name = i.Name,
                Code = i.Code,
                MasterProductCode = i.MasterProductCode,
                MasterProductName = i.MasterProductName,
                AddOnCategoryCode = i.AddOnCategoryCode,
                AddOnCategoryName = i.AddOnCategoryName,
                PortionCode = i.PortionCode,
                PortionName = i.PortionName,
                CrustName = i.CrustName,
                Price = i.Price,
                Qty = i.Qty
            }).ToList();
            return Json(model.DishLists, JsonRequestBehavior.AllowGet);
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
            if (code != null)
            {
                var addon = db.ProductDishAddOns.FirstOrDefault(i => i.Code == code);// ProductDishAddOn.Get(code);
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
                id = i.Code,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROFC004")]
        public async Task<JsonResult> GetDishCategorySelect2(string q = "")
        {
            var model = await db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "Dish").Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROFC004")]
        public async Task<JsonResult> GetDishShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && (a.ShopCategoryCode == "0" || a.ShopCategoryCode == "2") && (a.Status == 0 || a.Status == 1)).Select(i => new
            {
                id = i.Code,
                text = i.Name,
                shopid = i.Id
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROFC004")]
        public async Task<JsonResult> GetAddonCategorySelect2(string q = "")
        {
            var model = await db.AddOnCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROFC004")]
        public async Task<JsonResult> GetDishMasterProductSelect2(string q = "")
        {
            var model = await db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "Dish").Select(i => new
            {
                id = i.Code,
                text = i.Name,
                CategoryCode = i.CategoryCode,
                CategoryName = i.CategoryName,
                BrandCode = i.BrandCode,
                BrandName = i.BrandName,
                ShortDescription = i.ShortDescription,
                LongDescription = i.LongDescription,
                Customisation = i.Customisation,
                ColorCode = i.ColorCode,
                ImagePath = i.ImagePath,
                Price = i.Price,
                ProductType = i.ProductType,
                ImagePathLarge1 = i.ImagePathLarge1,
                GoogleTaxonomyCode = i.GoogleTaxonomyCode,
                MasterId = i.Id

            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // Medical Product Json Result

        [HttpPost]
        [AccessPolicy(PageCode = "SHNPROADC011")]
        public JsonResult AddToDefaultMedicalStock(DefaultMedicalStockViewModel model)
        {
            List<DefaultMedicalStockViewModel> medicalStock = Session["MedicalStock"] as List<DefaultMedicalStockViewModel>;
            if (medicalStock == null)
            {
                medicalStock = new List<DefaultMedicalStockViewModel>();
            }
            var id = medicalStock.Count() + 1;
            model.Id = id;
            medicalStock.Add(model);
            Session["MedicalStock"] = medicalStock;

            return Json(new
            {
                Id = id,
                Stock = model.Stock,
                SupplierName = model.SupplierName,
                MRP = model.MRP,
                SalePrice= model.SalePrice,
                TaxPercentage = model.TaxPercentage,
                DiscountPercentage = model.DiscountPercentage,
                LoyaltyPointsper100Value = model.LoyaltyPointsper100Value,
                MinimumLoyaltyReducationPercentage = model.MinimumLoyaltyReducationPercentage,
                SpecialCostOfDelivery = model.SpecialCostOfDelivery,
                OutLetId = model.OutLetId,
                SpecialPrice = model.SpecialPrice,
                MinSaleQty = model.MinSaleQty
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SHNPRORDC012")]
        public JsonResult RemoveFromDefaultMedicalStock(int id)
        {
            List<DefaultMedicalStockViewModel> medicalStock = Session["MedicalStock"] as List<DefaultMedicalStockViewModel>;

            if (medicalStock.Remove(medicalStock.SingleOrDefault(i => i.Id == id)))
            {
                this.Session["MedicalStock"] = medicalStock;
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SHNPROADC011")]
        public JsonResult AddToManualMedicalStock(ManualMedicalStockViewModel model)
        {
            List<ManualMedicalStockViewModel> medicalStock = Session["ManualMedicalStock"] as List<ManualMedicalStockViewModel>;
            if (medicalStock == null)
            {
                medicalStock = new List<ManualMedicalStockViewModel>();
            }
            var id = medicalStock.Count() + 1;
            model.Id = id;
            medicalStock.Add(model);
            Session["ManualMedicalStock"] = medicalStock;

            return Json(new
            {
                Id = id,
                Stock = model.Stock1,
                SupplierName = model.SupplierName1,
                MRP = model.MRP1,
                SalePrice = model.SalePrice1,
                TaxPercentage = model.TaxPercentage1,
                DiscountPercentage = model.DiscountPercentage1,
                LoyaltyPointsper100Value = model.LoyaltyPointsper100Value1,
                MinimumLoyaltyReducationPercentage = model.MinimumLoyaltyReducationPercentage1,
                SpecialCostOfDelivery = model.SpecialCostOfDelivery1,
                OutLetId = model.OutLetId1,
                SpecialPrice = model.SpecialPrice1,
                MinSaleQty = model.MinSaleQty1
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SHNPRORDC012")]
        public JsonResult RemoveFromManualMedicalStock(int id)
        {
            List<ManualMedicalStockViewModel> medicalStock = Session["ManualMedicalStock"] as List<ManualMedicalStockViewModel>;

            if (medicalStock.Remove(medicalStock.SingleOrDefault(i => i.Id == id)))
            {
                this.Session["ManualMedicalStock"] = medicalStock;
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "SHNPROERDC013")]
        public JsonResult EditRemoveMedicalStock(int id, string code)
        {
            List<DefaultMedicalStockViewModel> medicalStock = Session["MedicalStock"] as List<DefaultMedicalStockViewModel>;
            if (medicalStock == null)
            {
                medicalStock = new List<DefaultMedicalStockViewModel>();
            }
            if (medicalStock.Remove(medicalStock.SingleOrDefault(i => i.Id == id)))
            {
                this.Session["MedicalStock"] = medicalStock;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            if (code != null)
            {
                var pms = db.ProductMedicalStocks.FirstOrDefault(i => i.Code == code && i.Status == 0);// ProductMedicalStock.Get(code);
                pms.Status = 2;
                // ProductMedicalStock.Edit(pms, out int error);
                pms.DateUpdated = DateTime.Now;
                db.Entry(pms).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                if (medicalStock.Remove(medicalStock.SingleOrDefault(i => i.Code == code)))
                {
                    this.Session["MedicalStock"] = medicalStock;
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        // Product Select2

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetBrandSelect2(string q = "")
        {
            var model = await db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetMedicalBrandSelect2(string q = "")
        {
            var model = await db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "Medical").Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetProductShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && (a.ShopCategoryCode == "2" || a.ShopCategoryCode == "3" || a.ShopCategoryCode == "4" || a.ShopCategoryCode == "1") && (a.Status == 0 || a.Status == 1)).Select(i => new
            {
                id = i.Code,
                text = i.Name + " (" + i.PhoneNumber + ")"
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetCategorySelect2(string q = "")
        {
            var model = await db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetMedicalCategorySelect2(string q = "")
        {
            var model = await db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "Medical").Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetProductCategorySelect2(string q = "")
        {
            var model = await db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "Product").Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetMasterProductSelect2(string q = "")
        {
            var model = await db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "Product").Select(i => new
            {
                id = i.Code,
                text = i.Name,
                CategoryCode = i.CategoryCode,
                CategoryName = i.CategoryName,
                BrandCode = i.BrandCode,
                BrandName = i.BrandName,
                ShortDescription = i.ShortDescription,
                LongDescription = i.LongDescription,
                ImagePath = i.ImagePath,
                Price = i.Price,
                ProductType = i.ProductType
            }).Take(500).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetMedicalMasterProductSelect2(string q = "")
        {
            var model = await db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "Medical").Select(i => new
            {
                id = i.Code,
                text = i.Name,
                CategoryCode = i.CategoryCode,
                CategoryName = i.CategoryName,
                BrandCode = i.BrandCode,
                BrandName = i.BrandName,
                DrugMeasurementUnitCode = i.MeasurementUnitCode,
                DrugMeasurementUnitName = i.MeasurementUnitName,
                PriscriptionCategory = i.PriscriptionCategory,
                DrugCompoundDetailCode = i.DrugCompoundDetailCode,
                CombinationDrugCompound = i.CombinationDrugCompound,
                PackageCode = i.PackageCode,
                PackageName = i.PackageName,
                Manufacturer = i.Manufacturer,
                OriginCountry = i.OriginCountry,
                iBarU = i.iBarU,
                weight = i.weight,
                SizeLB = i.SizeLB,
                Price = i.Price,
                ImagePathLarge1 = i.ImagePathLarge1,
                ImagePathLarge2 = i.ImagePathLarge2,
                ImagePathLarge3 = i.ImagePathLarge3,
                ImagePathLarge4 = i.ImagePathLarge4,
                ImagePathLarge5 = i.ImagePathLarge5,
                ProductType = i.ProductType,
                GoogleTaxonomyCode = i.GoogleTaxonomyCode
            }).Take(500).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetManualMedicalMasterProductSelect2(string q = "")
        {
            var model = await db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "Medical").Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).Take(500).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public JsonResult GetMasterProductSpecification(string code)
        {
            var model =new  List<ProductEditViewModel.SpecificationList>();
            model = db.ProductSpecifications.Where(i => i.MasterProductCode == code && i.Status == 0).Select(i => new ProductEditViewModel.SpecificationList
            {
                Code = i.Code,
                SpecificationName = i.SpecificationName,
                SpecificationCode = i.SpecificationCode,
                ProductName = i.MasterProductName,
                ProductCode = i.MasterProductCode,
                Value = i.Value
            }).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetSpecificationSelect2(string q = "")
        {
            var model = await db.Specifications.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetDrugUnitSelect2(string q = "")
        {
            var model = await db.MeasurementUnits.Where(a => a.UnitName.Contains(q) && a.Status == 0).OrderBy(i => i.UnitName).Select(i => new
            { 
                id = i.Code,
                text = i.UnitName
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetDrugCompoundDetailSelect2(string q = "")
        {
            var model = await db.DrugCompoundDetails.Where(a => a.AliasName.Contains(q) && a.Status == 0).OrderBy(i => i.AliasName).Select(i => new
            {
                id = i.Code,
                text = i.AliasName
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPROC001")]
        public async Task<JsonResult> GetDiscountCategorySelect2(string q = "")
        {
            var model = await db.DiscountCategories.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Code,
                text = i.Name,
                percentage = i.Percentage,
                type = i.Type,
                categorytype = i.CategoryType
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
        public ActionResult ShopItemMapping(string originalShopCode, string newShopCode, string newShopName)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new ProductMappingViewModel();
            model.Lists = db.Products.Where(i => i.ShopCode == originalShopCode && i.Status == 0).ToList().AsQueryable().ProjectTo<ProductMappingViewModel.List>(_mapperConfiguration).ToList();// Product.GetListItem(originalShopCode).AsQueryable().ProjectTo<ProductMappingViewModel.List>(_mapperConfiguration).ToList();
            
                foreach (var pro in model.Lists)
                {
                    Product product = new Product();
                    var prod = _mapper.Map<ProductMappingViewModel, Product>(model);
                    prod.ShopCode = newShopCode;
                    prod.ShopName = newShopName;
                prod.MainSNCode = ShopNow.Helpers.DRC.GenerateSNIN();
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
            var model = await db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "FMCG").Select(i => new
            {
                id = i.Code,
                text = i.Name,
                CategoryCode = i.CategoryCode,
                CategoryName = i.CategoryName,
                SubCategoryCode = i.SubCategoryCode,
                SubCategoryName = i.SubCategoryName,
                NextSubCategoryCode = i.NextSubCategoryCode,
                NextSubCategoryName = i.NextSubCategoryName,
                BrandCode = i.BrandCode,
                BrandName = i.BrandName,
                ShortDescription = i.ShortDescription,
                LongDescription = i.LongDescription,
                ImagePathLarge1 = i.ImagePathLarge1,
                ImagePathLarge2 = i.ImagePathLarge2,
                ImagePathLarge3 = i.ImagePathLarge3,
                ImagePathLarge4 = i.ImagePathLarge4,
                ImagePathLarge5 = i.ImagePathLarge5,
                Price = i.Price,
                ProductType = i.ProductType,
                ASIN = i.ASIN,
                GoogleTaxonomyCode = i.GoogleTaxonomyCode,
                weight = i.weight,
                SizeLB = i.SizeLB,
                MeasurementUnitCode = i.MeasurementUnitCode,
                MeasurementUnitName = i.MeasurementUnitName,
                PackageCode = i.PackageCode,
                PackageName = i.PackageName,
                MasterId = i.Id
            }).ToListAsync();
            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetFMCGPackageSelect2(string q = "")
        {
            var model = await db.Packages.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.Type == 2).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetFMCGMeasurementUnitSelect2(string q = "")
        {
            var model = await db.MeasurementUnits.OrderBy(i => i.UnitName).Where(a => a.UnitName.Contains(q) && a.Status == 0 && a.Type == 2).Select(i => new
            {
                id = i.Code,
                text = i.UnitName
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetElectronicSelect2(string q = "")
        {
            var model = await db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "FMCG").Select(i => new
            {
                id = i.Code,
                text = i.Name,
                CategoryCode = i.CategoryCode,
                CategoryName = i.CategoryName,
                SubCategoryCode = i.SubCategoryCode,
                SubCategoryName = i.SubCategoryName,
                NextSubCategoryCode = i.NextSubCategoryCode,
                NextSubCategoryName = i.NextSubCategoryName,
                BrandCode = i.BrandCode,
                BrandName = i.BrandName,
                ShortDescription = i.ShortDescription,
                LongDescription = i.LongDescription,
                ImagePathLarge1 = i.ImagePathLarge1,
                ImagePathLarge2 = i.ImagePathLarge2,
                ImagePathLarge3 = i.ImagePathLarge3,
                ImagePathLarge4 = i.ImagePathLarge4,
                ImagePathLarge5 = i.ImagePathLarge5,
                Price = i.Price,
                ProductType = i.ProductType,
                ASIN = i.ASIN,
                GoogleTaxonomyCode = i.GoogleTaxonomyCode,
                weight = i.weight,
                SizeLB = i.SizeLB,
                MeasurementUnitCode = i.MeasurementUnitCode,
                MeasurementUnitName = i.MeasurementUnitName,
                PackageCode = i.PackageCode,
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

    }

}