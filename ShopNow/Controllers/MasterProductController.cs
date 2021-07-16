﻿using Amazon;
using Amazon.S3;
using AutoMapper;
using ExcelDataReader;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class MasterProductController : Controller
    {
        private ShopnowchatEntities _db = new ShopnowchatEntities();
        
        UploadContent uc = new UploadContent();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private const string _prefix = "MPR";
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.APSouth1;
        private static readonly string accesskey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static readonly string secretkey = ConfigurationManager.AppSettings["AWSSecretKey"];
        private static string _generatedCode(string _prefix)
        {
            
                return ShopNow.Helpers.DRC.Generate(_prefix);
           
        }
        public MasterProductController()
        {
            _db.Database.ExecuteSqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
            
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<MasterProduct, MasterProductListViewModel.MasterProductList>();
                config.CreateMap<MasterProductCreateEditViewModel, MasterProduct>();
                config.CreateMap<MasterProduct, MasterProductCreateEditViewModel>();
                config.CreateMap<MasterProduct, MedicalDrugListViewModel.MedicalDrugList>();
                config.CreateMap<MedicalDrugCreateEditViewModel, MasterProduct>();
                config.CreateMap<MasterProduct, MedicalDrugCreateEditViewModel>();
                config.CreateMap<Product, ItemMappingViewModel>();
                config.CreateMap<MasterFMCGCreateEditViewModel, MasterProduct>();
                config.CreateMap<MasterProduct, MasterFMCGCreateEditViewModel>();
                config.CreateMap<MasterFoodCreateViewModel, MasterProduct>();
                config.CreateMap<MasterProduct, MasterFoodCreateViewModel>();
                config.CreateMap<MasterFoodEditViewModel, MasterProduct>();
                config.CreateMap<MasterProduct, MasterFoodEditViewModel>();
                config.CreateMap<MasterFoodEditViewModel.AddonList, MasterAddOnsCreateViewModel>();
            });

            _mapper = _mapperConfiguration.CreateMapper();
        }

        // All MasterItem List
        [AccessPolicy(PageCode = "SHNMPRL001")]
        public ActionResult List()
        {
            if (Session["USER"] == null)
            {

                return RedirectToAction("LogOut", "Home");
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from mp in _db.MasterProducts select mp).OrderBy(mp => mp.Name).Where(mp => mp.Status == 0).ToList();
            return View(List);
        }

        // All MasterItem Delete
        [AccessPolicy(PageCode = "SHNMPRD004")]
        public ActionResult Delete(string Id)
        {
            var dCode = AdminHelpers.DCodeInt(Id);
            var master = _db.MasterProducts.FirstOrDefault(i => i.Id == dCode);
            master.Status = 2;
            _db.Entry(master).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("List");
        }

        // Dish Create
        [AccessPolicy(PageCode = "SHNMPRFC015")]
        public ActionResult FoodCreate()
        {
            Session["AddOns"] = new List<MasterAddOnsCreateViewModel>();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNMPRFC015")]
        public ActionResult FoodCreate(MasterFoodCreateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _mapper.Map<MasterFoodCreateViewModel, MasterProduct>(model);
            prod.CreatedBy = user.Name;
            prod.UpdatedBy = user.Name;
            prod.ProductTypeName = "Dish";
            prod.ProductTypeId = 1;
            prod.Status = 0;
            var name = _db.MasterProducts.FirstOrDefault(i => i.Name == model.Name && i.Status == 0 && i.ProductTypeId == 1 && i.CategoryId == model.CategoryId);
            prod.Name = model.Name;
            if(model.NickName == null)
            {
                prod.NickName = model.Name;
            }
            if (name == null)
            {
                prod.DateEncoded = DateTime.Now;
                prod.DateUpdated = DateTime.Now;
                prod.Status = 0;
                try
                {
                    if (model.ProductImage1 != null)
                    {
                        uc.UploadFiles(model.ProductImage1.InputStream, prod.Id + "_" + model.ProductImage1.FileName, accesskey, secretkey, "image");
                        prod.ImagePath1 = prod.Id + "_" + model.ProductImage1.FileName.Replace(" ", "");
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
                _db.MasterProducts.Add(prod);
                _db.SaveChanges();
                ViewBag.Message = model.Name + " Saved Successfully!";
            }
            else
            {
                ViewBag.ErrorMessage = model.Name + " Already Exist";
            }

            List<MasterAddOnsCreateViewModel> addOns = Session["AddOns"] as List<MasterAddOnsCreateViewModel>;
            var productDishaddOn = new ProductDishAddOn();
            foreach (var s in addOns)
            {
                productDishaddOn.AddOnItemName = s.AddOnItemName;
                productDishaddOn.MasterProductId = prod.Id;
                productDishaddOn.MasterProductName = prod.Name;
                productDishaddOn.AddOnCategoryId = s.AddOnCategoryId;
                productDishaddOn.AddOnCategoryName = s.AddOnCategoryName;
                productDishaddOn.PortionId = s.PortionId;
                productDishaddOn.PortionName = s.PortionName;
                productDishaddOn.MinSelectionLimit = s.MinSelectionLimit;
                productDishaddOn.MaxSelectionLimit = s.MaxSelectionLimit;
                productDishaddOn.CrustName = s.CrustName;
                productDishaddOn.AddOnsPrice = s.AddOnsPrice;
                productDishaddOn.PortionPrice = s.PortionPrice;
                productDishaddOn.CrustPrice = s.CrustPrice;
                productDishaddOn.CreatedBy = user.Name;
                productDishaddOn.UpdatedBy = user.Name;
                productDishaddOn.DateEncoded = DateTime.Now;
                productDishaddOn.DateUpdated = DateTime.Now;
                productDishaddOn.Status = 0;
                productDishaddOn.AddOnType = s.AddOnType;
                productDishaddOn.MasterProductId = prod.Id;
                _db.ProductDishAddOns.Add(productDishaddOn);
                _db.SaveChanges();
            }

            return View();
        }

        // Dish Update
        [AccessPolicy(PageCode = "SHNMPRFE016")]
        public ActionResult FoodEdit(string Id)
        {
            var dCode = AdminHelpers.DCodeInt(Id);
            Session["AddOns"] = new List<MasterAddOnsCreateViewModel>();
            var addOns = new List<MasterAddOnsCreateViewModel>();
            if (string.IsNullOrEmpty(dCode.ToString()))
                return HttpNotFound();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var product = _db.MasterProducts.FirstOrDefault(i => i.Id == dCode); 
            var model = _mapper.Map<MasterProduct, MasterFoodEditViewModel>(product);
            model.AddonLists = _db.ProductDishAddOns.Where(i => i.MasterProductId == product.Id && i.Status == 0).Select(i => new MasterFoodEditViewModel.AddonList
            {
                Id = i.Id,
                AddOnItemName = i.AddOnItemName,
                AddOnCategoryId = i.AddOnCategoryId,
                MasterProductName = i.MasterProductName,
                PortionId = i.PortionId,
                PortionName = i.PortionName,
                PortionPrice = i.PortionPrice,
                AddOnCategoryName = i.AddOnCategoryName,
                AddOnsPrice = i.AddOnsPrice,
                CrustName = i.CrustName,
                CrustPrice = i.CrustPrice,
                MinSelectionLimit = i.MinSelectionLimit,
                MaxSelectionLimit = i.MaxSelectionLimit,
                AddOnType = i.AddOnType,
                MasterProductId =i.MasterProductId.Value
            }).ToList();
            foreach(var s in model.AddonLists)
            {
                var addOn = _mapper.Map<MasterFoodEditViewModel.AddonList, MasterAddOnsCreateViewModel>(s);
                addOns.Add(addOn);
            }
            Session["AddOns"] = addOns;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNMPRFE016")]
        public ActionResult FoodEdit(MasterFoodEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _db.MasterProducts.FirstOrDefault(i=>i.Id == model.Id); 
            _mapper.Map(model, prod);
            prod.DateUpdated = DateTime.Now;
            prod.UpdatedBy = user.Name;
            try
            {
                if (model.ProductImage1 != null)
                {
                    uc.UploadFiles(model.ProductImage1.InputStream, prod.Id + "_" + model.ProductImage1.FileName, accesskey, secretkey, "image");
                    prod.ImagePath1 = prod.Id + "_" + model.ProductImage1.FileName.Replace(" ", "");
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
            _db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();

            List<MasterAddOnsCreateViewModel> addOns = Session["AddOns"] as List<MasterAddOnsCreateViewModel>;
            var productDishaddOn = new ProductDishAddOn();
            foreach (var s in addOns)
            {
                if (s.Id == 0)
                {
                    productDishaddOn.AddOnItemName = s.AddOnItemName;
                    productDishaddOn.MasterProductId = prod.Id;
                    productDishaddOn.MasterProductName = prod.Name;
                    productDishaddOn.AddOnCategoryId = s.AddOnCategoryId;
                    productDishaddOn.AddOnCategoryName = s.AddOnCategoryName;
                    productDishaddOn.PortionId = s.PortionId;
                    productDishaddOn.PortionName = s.PortionName;
                    productDishaddOn.MinSelectionLimit = s.MinSelectionLimit;
                    productDishaddOn.MaxSelectionLimit = s.MaxSelectionLimit;
                    productDishaddOn.CrustName = s.CrustName;
                    productDishaddOn.AddOnsPrice = s.AddOnsPrice;
                    productDishaddOn.PortionPrice = s.PortionPrice;
                    productDishaddOn.CrustPrice = s.CrustPrice;
                    productDishaddOn.CreatedBy = user.Name;
                    productDishaddOn.UpdatedBy = user.Name;
                    productDishaddOn.DateEncoded = DateTime.Now;
                    productDishaddOn.DateUpdated = DateTime.Now;
                    productDishaddOn.Status = 0;
                    productDishaddOn.AddOnType = s.AddOnType;
                    productDishaddOn.MasterProductId = prod.Id;
                    _db.ProductDishAddOns.Add(productDishaddOn);
                    _db.SaveChanges();
                }
            }

            return RedirectToAction("FoodList", "MasterProduct");
        }

        // Dish List
        [AccessPolicy(PageCode = "SHNMPRFL017")]
        public ActionResult FoodList()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from mp in _db.MasterProducts select mp).OrderBy(mp => mp.Name).Where(mp => mp.Status == 0 && mp.ProductTypeId == 1).ToList();
            return View(List);
        }

        // Dish Delete  -- return Dish List page
        [AccessPolicy(PageCode = "")]
        public ActionResult FoodDelete(string Id)
        {
            var dCode = AdminHelpers.DCodeInt(Id);
            var master = _db.MasterProducts.FirstOrDefault(i => i.Id == dCode);
            master.Status = 2;
            _db.Entry(master).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("FoodList");
        }

        // Electronic List
        [AccessPolicy(PageCode = "SHNMPREL018")]
        public ActionResult ElectronicList()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from mp in _db.MasterProducts select mp).OrderBy(mp => mp.Name).Where(mp => mp.Status == 0 && mp.ProductTypeId == 4).ToList();
            return View(List);
        }

        // Electronic Create
        [AccessPolicy(PageCode = "SHNMPREC019")]
        public ActionResult ElectronicCreate()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNMPREC019")]
        public ActionResult ElectronicCreate(MasterProductCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _mapper.Map<MasterProductCreateEditViewModel, MasterProduct>(model);
            prod.CreatedBy = user.Name;
            prod.UpdatedBy = user.Name;
            //prod.Code = _generatedCode("MPR");
            if (model.NickName == null)
            {
                prod.NickName = model.Name;
            }
            var name = _db.MasterProducts.FirstOrDefault(i => i.Name == model.Name && i.Status == 0 && i.ProductTypeId == 4);// MasterProduct.GetElectronicName(model.Name);
            try
            {
                //if (model.CategoryCode != null)
                //{
                //    prod.CategoryCode = String.Join(",", model.CategoryCode);
                //    StringBuilder sb = new StringBuilder();
                //    foreach (var s in model.CategoryCode)
                //    {
                //        var cat = _db.Categories.FirstOrDefault(i => i.Code == s);// Category.Get(s);
                //        sb.Append(cat.Name);
                //        sb.Append(",");
                //    }
                //    if (sb.Length >= 1)
                //    {
                //        model.CategoryName = sb.ToString().Remove(sb.Length - 1);
                //        prod.CategoryName = model.CategoryName;
                //    }
                //    else
                //    {
                //        model.CategoryName = sb.ToString();
                //        prod.CategoryName = model.CategoryName;
                //    }
                //}
                //ProductImage1
                if (model.ProductImage1 != null)
                {
                    uc.UploadFiles(model.ProductImage1.InputStream, prod.Id + "_" + model.ProductImage1.FileName, accesskey, secretkey, "image");
                    prod.ImagePath1 = prod.Id + "_" + model.ProductImage1.FileName.Replace(" ", "");
                }

                //ProductImage2
                if (model.ProductImage2 != null)
                {
                    uc.UploadFiles(model.ProductImage2.InputStream, prod.Id + "_" + model.ProductImage2.FileName, accesskey, secretkey, "image");
                    prod.ImagePath2 = prod.Id + "_" + model.ProductImage2.FileName.Replace(" ", "");
                }

                //ProductImage3
                if (model.ProductImage3 != null)
                {
                    uc.UploadFiles(model.ProductImage3.InputStream, prod.Id + "_" + model.ProductImage3.FileName, accesskey, secretkey, "image");
                    prod.ImagePath3 = prod.Id + "_" + model.ProductImage3.FileName.Replace(" ", "");
                }

                //ProductImage4
                if (model.ProductImage4 != null)
                {
                    uc.UploadFiles(model.ProductImage4.InputStream, prod.Id + "_" + model.ProductImage4.FileName, accesskey, secretkey, "image");
                    prod.ImagePath4 = prod.Id + "_" + model.ProductImage4.FileName.Replace(" ", "");
                }

                //ProductImage5
                if (model.ProductImage5 != null)
                {
                    uc.UploadFiles(model.ProductImage5.InputStream, prod.Id + "_" + model.ProductImage5.FileName, accesskey, secretkey, "image");
                    prod.ImagePath5 = prod.Id + "_" + model.ProductImage5.FileName.Replace(" ", "");
                }
                if (name == null)
                {
                    prod.DateEncoded = DateTime.Now;
                    prod.DateUpdated = DateTime.Now;
                    prod.Status = 0;
                    _db.MasterProducts.Add(prod);
                    _db.SaveChanges();
                    ViewBag.Message = model.Name + " Saved Successfully!";
                }
                else
                {
                    ViewBag.ErrorMessage = model.Name + " Already Exist";
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

        // Electronic Update
        [AccessPolicy(PageCode = "SHNMPREE020")]
        public ActionResult ElectronicEdit(string Id)
        {
            var dCode = AdminHelpers.DCodeInt(Id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dCode.ToString()))
                return HttpNotFound();
            var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Id == dCode);
            var model = _mapper.Map<MasterProduct, MasterProductCreateEditViewModel>(masterProduct);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNMPREE020")]
        public ActionResult ElectronicEdit(MasterProductCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _db.MasterProducts.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, prod);
            prod.Name = model.Name;
            prod.ProductTypeId = model.ProductTypeId;
            prod.UpdatedBy = user.Name;
            prod.DateUpdated = DateTime.Now;
            try
            {
                //if (model.CategoryCode != null)
                //{
                //    prod.CategoryCode = String.Join(",", model.CategoryCode);
                //    StringBuilder sb = new StringBuilder();
                //    foreach (var s in model.CategoryCode)
                //    {
                //        var cat = _db.Categories.FirstOrDefault(i => i.Code == s);// Category.Get(s);
                //        if (cat != null)
                //        {
                //            sb.Append(cat.Name);
                //            sb.Append(",");
                //        }
                //    }
                //    if (sb.Length >= 1)
                //    {
                //        model.CategoryName = sb.ToString().Remove(sb.Length - 1);
                //        prod.CategoryName = model.CategoryName;
                //    }
                //    else
                //    {
                //        model.CategoryName = sb.ToString();
                //        prod.CategoryName = model.CategoryName;
                //    }
                //}

                //ProductImage1
                if (model.ProductImage1 != null)
                {
                    uc.UploadFiles(model.ProductImage1.InputStream, prod.Id + "_" + model.ProductImage1.FileName, accesskey, secretkey, "image");
                    prod.ImagePath1 = prod.Id + "_" + model.ProductImage1.FileName.Replace(" ", "");
                }

                //ProductImage2
                if (model.ProductImage2 != null)
                {
                    uc.UploadFiles(model.ProductImage2.InputStream, prod.Id + "_" + model.ProductImage2.FileName, accesskey, secretkey, "image");
                    prod.ImagePath2 = prod.Id + "_" + model.ProductImage2.FileName.Replace(" ", "");
                }

                //ProductImage3
                if (model.ProductImage3 != null)
                {
                    uc.UploadFiles(model.ProductImage3.InputStream, prod.Id + "_" + model.ProductImage3.FileName, accesskey, secretkey, "image");
                    prod.ImagePath3 = prod.Id + "_" + model.ProductImage3.FileName.Replace(" ", "");
                }

                //ProductImage4
                if (model.ProductImage4 != null)
                {
                    uc.UploadFiles(model.ProductImage4.InputStream, prod.Id + "_" + model.ProductImage4.FileName, accesskey, secretkey, "image");
                    prod.ImagePath4 = prod.Id + "_" + model.ProductImage4.FileName.Replace(" ", "");
                }

                //ProductImage5
                if (model.ProductImage5 != null)
                {
                    uc.UploadFiles(model.ProductImage5.InputStream, prod.Id + "_" + model.ProductImage5.FileName, accesskey, secretkey, "image");
                    prod.ImagePath5 = prod.Id + "_" + model.ProductImage5.FileName.Replace(" ", "");
                }
                prod.DateUpdated = DateTime.Now;
                _db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                //  MasterProduct.Edit(prod, out errorCode);
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
            return RedirectToAction("List");
        }

        // Electronic Delete
        [AccessPolicy(PageCode = "SHNMPRED021")]
        public ActionResult ElectronicDelete(string Id)
        {
            var dCode = AdminHelpers.DCodeInt(Id);
            var master = _db.MasterProducts.FirstOrDefault(i => i.Id == dCode);// MasterProduct.Get(dCode);
            master.Status = 2;
            _db.Entry(master).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("List");
        }

        // Medical List
        [AccessPolicy(PageCode = "SHNMPRML006")]
        public ActionResult MedicalList()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from mp in _db.MasterProducts select mp).OrderBy(mp => mp.Name).Where(mp => mp.Status == 0 && mp.ProductTypeId == 3).ToList();
            return View(List);
        }

        // Medical Create
        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public ActionResult MedicalCreate()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public ActionResult MedicalCreate(MedicalDrugCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _mapper.Map<MedicalDrugCreateEditViewModel, MasterProduct>(model);
            prod.CreatedBy = user.Name;
            prod.UpdatedBy = user.Name;
            prod.ProductTypeName = "Medical";
            prod.ProductTypeId = 3;
          //  prod.Code = _generatedCode("MPR");
            if (model.NickName == null)
            {
                prod.NickName = model.Name;
            }
            var name = _db.MasterProducts.FirstOrDefault(i => i.Name == model.Name && i.Status == 0 && i.ProductTypeId == 3);
            try
            {
                //if (model.CategoryCode != null)
                //{
                //    prod.CategoryCode = String.Join(",", model.CategoryCode);
                //    StringBuilder sb = new StringBuilder();
                //    foreach (var s in model.CategoryCode)
                //    {
                //        var cat = _db.Categories.FirstOrDefault(i => i.Code == s);// Category.Get(s);
                //        sb.Append(cat.Name);
                //        sb.Append(",");
                //    }
                //    if (sb.Length >= 1)
                //    {
                //        model.CategoryName = sb.ToString().Remove(sb.Length - 1);
                //        prod.CategoryName = model.CategoryName;
                //    }
                //    else
                //    {
                //        model.CategoryName = sb.ToString();
                //        prod.CategoryName = model.CategoryName;
                //    }
                //}
                //if (model.DrugCompoundDetailCode != null)
                //{
                //    prod.DrugCompoundDetailCode = String.Join(",", model.DrugCompoundDetailCode);
                //    StringBuilder sb = new StringBuilder();
                //    foreach (var s in model.DrugCompoundDetailCode)
                //    {
                //        var dcd = _db.DrugCompoundDetails.FirstOrDefault(i => i.Code == s && i.Status == 0); // DrugCompoundDetail.Get(s);
                //        sb.Append(dcd.AliasName);
                //        sb.Append(",");
                //    }
                //    if (sb.Length >= 1)
                //    {
                //        model.CombinationDrugCompound = sb.ToString().Remove(sb.Length - 1);
                //        prod.CombinationDrugCompound = model.CombinationDrugCompound;
                //    }
                //    else
                //    {
                //        model.CombinationDrugCompound = sb.ToString();
                //        prod.CombinationDrugCompound = model.CombinationDrugCompound;
                //    }
                //}
               
                //ProductImage1
                if (model.ProductImage1 != null)
                {
                    uc.UploadFiles(model.ProductImage1.InputStream, prod.Id + "_" + model.ProductImage1.FileName, accesskey, secretkey, "image");
                    prod.ImagePath1 = prod.Id + "_" + model.ProductImage1.FileName.Replace(" ", "");
                }

                //ProductImage2
                if (model.ProductImage2 != null)
                {
                    uc.UploadFiles(model.ProductImage2.InputStream, prod.Id + "_" + model.ProductImage2.FileName, accesskey, secretkey, "image");
                    prod.ImagePath2 = prod.Id + "_" + model.ProductImage2.FileName.Replace(" ", "");
                }

                //ProductImage3
                if (model.ProductImage3 != null)
                {
                    uc.UploadFiles(model.ProductImage3.InputStream, prod.Id + "_" + model.ProductImage3.FileName, accesskey, secretkey, "image");
                    prod.ImagePath3 = prod.Id + "_" + model.ProductImage3.FileName.Replace(" ", "");
                }

                //ProductImage4
                if (model.ProductImage4 != null)
                {
                    uc.UploadFiles(model.ProductImage4.InputStream, prod.Id + "_" + model.ProductImage4.FileName, accesskey, secretkey, "image");
                    prod.ImagePath4 = prod.Id + "_" + model.ProductImage4.FileName.Replace(" ", "");
                }

                //ProductImage5
                if (model.ProductImage5 != null)
                {
                    uc.UploadFiles(model.ProductImage5.InputStream, prod.Id + "_" + model.ProductImage5.FileName, accesskey, secretkey, "image");
                    prod.ImagePath5 = prod.Id + "_" + model.ProductImage5.FileName.Replace(" ", "");
                }
                if (name == null)
                {
                    prod.DateEncoded = DateTime.Now;
                    prod.DateUpdated = DateTime.Now;
                    prod.Status = 0;
                    _db.MasterProducts.Add(prod);
                    _db.SaveChanges();
                    ViewBag.Message = model.Name + " Saved Successfully!";
                }
                else
                {
                    ViewBag.ErrorMessage = model.Name + " Already Exist";
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

        // Medical Update
        [AccessPolicy(PageCode = "SHNMPRME008")]
        public ActionResult MedicalEdit(string Id)
        {
            var dCode = AdminHelpers.DCodeInt(Id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dCode.ToString()))
                return HttpNotFound();
            var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Id == dCode);
            var model = _mapper.Map<MasterProduct, MedicalDrugCreateEditViewModel>(masterProduct);
            if(model.ImagePath1 !=null)
            model.ImagePath1 = model.ImagePath1.Replace("%", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (model.ImagePath2 != null)
                model.ImagePath2 = model.ImagePath2.Replace("%", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (model.ImagePath3 != null)
                model.ImagePath3 = model.ImagePath3.Replace("%", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (model.ImagePath4 != null)
                model.ImagePath4 = model.ImagePath4.Replace("%", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (model.ImagePath5 != null)
                model.ImagePath5 = model.ImagePath5.Replace("%", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "SHNMPRME008")]
        public ActionResult MedicalEdit(MedicalDrugCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _db.MasterProducts.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, prod);
            prod.UpdatedBy = user.Name;
            prod.DateUpdated = DateTime.Now;
            try
            {
                //if (model.CategoryCode != null)
                //{
                //    prod.CategoryCode = String.Join(",", model.CategoryCode);
                //    StringBuilder sb = new StringBuilder();
                //    foreach (var s in model.CategoryCode)
                //    {
                //        var cat = _db.Categories.FirstOrDefault(i => i.Id == s);// Category.Get(s);
                //        if (cat != null)
                //        {
                //            sb.Append(cat.Name);
                //            sb.Append(",");
                //        }
                //    }
                //    if (sb.Length >= 1)
                //    {
                //        model.CategoryName = sb.ToString().Remove(sb.Length - 1);
                //        prod.CategoryName = model.CategoryName;
                //    }
                //    else
                //    {
                //        model.CategoryName = sb.ToString();
                //        prod.CategoryName = model.CategoryName;
                //    }
                //}

                //if (model.DrugCompoundDetailCode != null)
                //{
                //    prod.DrugCompoundDetailCode = String.Join(",", model.DrugCompoundDetailCode);
                //    StringBuilder sb = new StringBuilder();
                //    foreach (var s in model.DrugCompoundDetailCode)
                //    {
                //        var dcd = _db.DrugCompoundDetails.FirstOrDefault(i => i.Code == s.Replace(" ","") && i.Status == 0);// DrugCompoundDetail.Get(s);
                //        if (dcd != null)
                //        {
                //            sb.Append(dcd.AliasName);
                //            sb.Append(",");
                //        }
                //    }
                //    if (sb.Length >= 1)
                //    {
                //        model.CombinationDrugCompound = sb.ToString().Remove(sb.Length - 1);
                //        prod.CombinationDrugCompound = model.CombinationDrugCompound;
                //    }
                //    else
                //    {
                //        model.CombinationDrugCompound = sb.ToString();
                //        prod.CombinationDrugCompound = model.CombinationDrugCompound;
                //    }
                //}

                //ProductImage1
                if (model.ProductImage1 != null)
                {
                    uc.UploadFiles(model.ProductImage1.InputStream, prod.Id + "_" + model.ProductImage1.FileName, accesskey, secretkey, "image");
                    prod.ImagePath1 = prod.Id + "_" + model.ProductImage1.FileName.Replace(" ", "");
                    //if (product != null)
                    //product.ImagePath1 = prod.Code + "_" + model.ProductImage1.FileName.Replace(" ", "");
                }

                //ProductImage2
                if (model.ProductImage2 != null)
                {
                    uc.UploadFiles(model.ProductImage2.InputStream, prod.Id + "_" + model.ProductImage2.FileName, accesskey, secretkey, "image");
                    prod.ImagePath2 = prod.Id + "_" + model.ProductImage2.FileName.Replace(" ", "");
                    //if (product != null)
                    //    product.ImagePath2 = prod.Code + "_" + model.ProductImage2.FileName.Replace(" ", "");
                }

                //ProductImage3
                if (model.ProductImage3 != null)
                {
                    uc.UploadFiles(model.ProductImage3.InputStream, prod.Id + "_" + model.ProductImage3.FileName, accesskey, secretkey, "image");
                    prod.ImagePath3 = prod.Id + "_" + model.ProductImage3.FileName.Replace(" ", "");
                    //if (product != null)
                    //    product.ImagePath3 = prod.Code + "_" + model.ProductImage3.FileName.Replace(" ", "");
                }

                //ProductImage4
                if (model.ProductImage4 != null)
                {
                    uc.UploadFiles(model.ProductImage4.InputStream, prod.Id + "_" + model.ProductImage4.FileName, accesskey, secretkey, "image");
                    prod.ImagePath4 = prod.Id + "_" + model.ProductImage4.FileName.Replace(" ", "");
                    //if (product != null)
                    //    product.ImagePath4 = prod.Code + "_" + model.ProductImage4.FileName.Replace(" ", "");
                }

                //ProductImage5
                if (model.ProductImage5 != null)
                {
                    uc.UploadFiles(model.ProductImage5.InputStream, prod.Id + "_" + model.ProductImage5.FileName, accesskey, secretkey, "image");
                    prod.ImagePath5 = prod.Id + "_" + model.ProductImage5.FileName.Replace(" ", "");
                    //if (product != null)
                    //    product.ImagePath5 = prod.Code + "_" + model.ProductImage5.FileName.Replace(" ", "");
                }
                _db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                //  MasterProduct.Edit(prod, out int error);
                //if (product != null)
                //{
                //    product.MasterProductCode = prod.Code;
                //    product.MasterProductCode = prod.Name;
                //    _db.Entry(product).State = EntityState.Modified;
                //    _db.SaveChanges();
                //}
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
            return RedirectToAction("MedicalList");

        }

        // Medical Update
        [AccessPolicy(PageCode = "SHNMPRMD009")]
        public ActionResult MedicalDelete(string Id)
        {
            var dCode = AdminHelpers.DCodeInt(Id);
            var master = _db.MasterProducts.FirstOrDefault(i => i.Id == dCode);
            master.Status = 2;
            _db.Entry(master).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("MedicalList");
        }

        // Upload Medical MasterItems
        [AccessPolicy(PageCode = "SHNMPRMI010")]
        public ActionResult MedicalIndex()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNMPRMI010")]
        public ActionResult MedicalIndex(HttpPostedFileBase upload, MedicalDrugViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            DataTable dt = new DataTable();

            if (model.button == "upload")
            {
                string path = Server.MapPath("~/Content/ExcelUpload/" + upload.FileName);
                upload.SaveAs(path);
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        Stream stream = upload.InputStream;
                        IExcelDataReader reader = null;

                        if (upload.FileName.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (upload.FileName.EndsWith(".xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "This file format is not supported");
                            return View();
                        }
                        DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });

                        // reader.IsFirstRowAsColumnNames = true;

                        reader.Close();

                        model.DataTable = result.Tables[0];
                        model.Filename = upload.FileName;
                        return View(model);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Please Upload Your file");
                    }

                }
            }
            else
            {
                string path = Server.MapPath("~/Content/ExcelUpload/" + model.Filename);
                string excelConnectionString = @"Provider='Microsoft.ACE.OLEDB.12.0';Data Source='" + path + "';Extended Properties='Excel 12.0 Xml;IMEX=1'";

                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                using (OleDbConnection connExcel = new OleDbConnection(excelConnectionString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                // Insert records to database table.
                // MainPageModel entities = new MainPageModel();
                foreach (DataRow row in dt.Rows)
                {
                    var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Name == row[model.Name].ToString() && i.Status == 0);// MasterProduct.GetName(row[model.Name].ToString());
                    if (masterProduct == null)
                    {
                        _db.MasterProducts.Add(new MasterProduct
                        {
                            Name = row[model.Name].ToString(),
                           // Code = _generatedCode("MPR"),
                            BrandId = MedicalCheckBrand(row[model.BrandName].ToString(), model.ProductTypeId),
                            BrandName = row[model.BrandName].ToString(),
                            CategoryId = MedicalCheckCategory(row[model.CategoryName].ToString(), model.ProductTypeId),
                            CategoryName = row[model.CategoryName].ToString(),
                            MeasurementUnitId = Convert.ToInt32(row[model.DrugMeasurementUnitId]),
                            MeasurementUnitName = row[model.DrugMeasurementUnitName].ToString(),
                            PriscriptionCategory = Convert.ToBoolean(row[model.PriscriptionCategory]),
                            DrugCompoundDetailId = Convert.ToInt32(row[model.DrugCompoundDetailId]),
                            DrugCompoundDetailName = row[model.CombinationDrugCompound].ToString(),
                            Price = Convert.ToDouble(row[model.Price]),
                            ImagePath1 = row[model.ImagePath1].ToString(),
                            ImagePath2 = row[model.ImagePath2].ToString(),
                            ImagePath3 = row[model.ImagePath3].ToString(),
                            ImagePath4 = row[model.ImagePath4].ToString(),
                            ImagePath5 = row[model.ImagePath5].ToString(),
                            OriginCountry = row[model.OriginCountry].ToString(),
                            Manufacturer = row[model.Manufacturer].ToString(),
                            IBarU = row[model.iBarU].ToString(),
                            SizeLB = row[model.SizeLB].ToString(),
                            Weight = Convert.ToDouble(row[model.weight]),
                            PackageId = CheckMedicalPackage(row[model.PackageName].ToString()),
                            PackageName = row[model.PackageName].ToString(),
                            ProductTypeId = model.ProductTypeId,
                            ColorCode = "N/A",
                            LongDescription = "N/A",
                            ShortDescription = "N/A",
                            Status = 0,
                            CreatedBy = user.Name,
                            UpdatedBy = user.Name,
                            DateEncoded = DateTime.Now,
                            DateUpdated = DateTime.Now
                        });
                        _db.SaveChanges();
                    }
                }
            }

            return View();
        }

        public int MedicalCheckBrand(string BrandName, int ProductType)
        {
            var brand = _db.Brands.FirstOrDefault(i => i.Name == BrandName && i.Status == 0);// Brand.GetName(BrandName);
            if (brand != null)
            {
                return brand.Id;
            }
            else
            {
                Brand br = new Brand();
                br.Name = BrandName;
                br.ProductTypeId = ProductType;
               // br.Code = _generatedCode("BRA");
                br.Status = 0;
                br.DateEncoded = DateTime.Now;
                br.DateUpdated = DateTime.Now;
                _db.Brands.Add(br);
                _db.SaveChanges();
                return br.Id;
            }

        }

        public int MedicalCheckCategory(string CategoryName, int ProductType)
        {
            var category = _db.Categories.FirstOrDefault(i => i.Name == CategoryName && i.Status == 0);// Category.GetName(CategoryName);
            if (category != null)
            {

                return category.Id;
            }
            else
            {
                Category cat = new Category();
                cat.Name = CategoryName;
                cat.ProductTypeId = ProductType;
               // cat.Code = _generatedCode("CAT");
                cat.Status = 0;
                cat.DateEncoded = DateTime.Now;
                cat.DateUpdated = DateTime.Now;
                _db.Categories.Add(cat);
                _db.SaveChanges();
                return cat.Id;
            }

        }

        public int CheckMedicalPackage(string PackageName)
        {
            var package = _db.Packages.FirstOrDefault(i => i.Name == PackageName && i.Status == 0);// Package.GetName(PackageName);
            if (package != null)
            {
                return package.Id;
            }
            else
            {
                Package mp = new Package();
                mp.Name = PackageName;
                mp.Type = 1;
               // mp.Code = _generatedCode("PKG");
                mp.Status = 0;
                mp.DateEncoded = DateTime.Now;
                mp.DateUpdated = DateTime.Now;
                _db.Packages.Add(mp);
                _db.SaveChanges();
                // mp.Code = Package.Add(mp, out int error);
                return mp.Id;
            }

        }

        public int CheckPackage(string PackageName)
        {
            var package = _db.Packages.FirstOrDefault(i => i.Name == PackageName && i.Status == 0);// Package.GetName(PackageName);
            if (package != null)
            {
                return package.Id;
            }
            else
            {
                Package mp = new Package();
                mp.Name = PackageName;
                mp.Type = 2;
               // mp.Code = _generatedCode("PKG");
                mp.Status = 0;
                mp.DateEncoded = DateTime.Now;
                mp.DateUpdated = DateTime.Now;
                _db.Packages.Add(mp);
                _db.SaveChanges();
                // mp.Code = Package.Add(mp, out int error);
                return mp.Id;
            }

        }

        public int CheckFMCGMeasurementUnit(string MeasurementUnitName)
        {
            var mu = _db.MeasurementUnits.FirstOrDefault(i => i.UnitName == MeasurementUnitName && i.Status == 0);// MeasurementUnit.GetName(MeasurementUnitName);
            if (mu != null)
            {
                return mu.Id;
            }
            else
            {
                MeasurementUnit mp = new MeasurementUnit();
                mp.UnitName = MeasurementUnitName;
                mp.Type = 2;
               // mp.Code = _generatedCode("MSU");
                mp.Status = 0;
                mp.DateEncoded = DateTime.Now;
                mp.DateUpdated = DateTime.Now;
                _db.MeasurementUnits.Add(mp);
                _db.SaveChanges();
                return mp.Id;
            }

        }

        // Item Mapping
        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public ActionResult ItemMapping()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public ActionResult SingleItemMapping(int shopId)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new ItemMappingViewModel();
            if (shopId != 0)
            {
                var shop = _db.Shops.FirstOrDefault(i => i.Id == shopId);// Shop.Get(shopcode);
                model.ShopId = shopId;
                if (shop != null)
                {
                    model.ShopName = shop.Name;
                }
            }
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNMPRPL013")]
        public ActionResult PendingList(int shopId)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new MasterProductListViewModel();
           
            model.Lists = _db.Products
                 .Where(a => a.ShopId == shopId && a.Status == 0 && a.MasterProductId == 0)
                 .OrderBy(i => i.Name).Select(i => new MasterProductListViewModel.PendingList
                 {
                     Id = i.Id,
                     Name = i.Name,
                     ItemId = i.ItemId,
                     TypeName = i.TypeName
                 }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNMPRML012")]
        public ActionResult MappedList(int shopId)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;

            var List = (from p in _db.Products
                        where (p.ShopId == shopId && p.Status == 0 && p.MasterProductId != 0)
                        select p).ToList();
            return View(List);
        }

        [AccessPolicy(PageCode = "SHNMPRMU014")]
        public ActionResult ItemMappingUpdate(int id)
        {
            var dCode = AdminHelpers.DCodeInt(id.ToString());
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var product = _db.Products.FirstOrDefault(i => i.Id == dCode);
            var model = _mapper.Map<Product, ItemMappingViewModel>(product);
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public JsonResult MappingProduct(int masterproductId, int itemId, int shopId)
        {
            try
            {
                var user = ((Helpers.Sessions.User)Session["USER"]);
                ViewBag.Name = user.Name;
                var masterproduct = _db.MasterProducts.FirstOrDefault(i => i.Id == masterproductId);// MasterProduct.Get(masterproductcode);

                var product = _db.Products.FirstOrDefault(i => i.Id == itemId);// Product.Get(itemcode);
                if (product != null)
                {
                    product.MasterProductId = masterproductId;
                    product.MasterProductId = masterproduct.Id;
                   
                    if (product.Customisation == false && masterproduct.Customisation != false)
                    {
                        product.Customisation = masterproduct.Customisation;
                    }
                    if (product.Price == 0 && masterproduct.Price != 0)
                    {
                        product.Price = masterproduct.Price;
                    }
                
                    if (product.IBarU == 0 && masterproduct.IBarU != null)
                    {
                        product.IBarU = Convert.ToInt32(masterproduct.IBarU);
                    }
                   
                    if (product.TypeId == 0 && masterproduct.ProductTypeId != 0)
                    {
                        product.TypeId = masterproduct.ProductTypeId;
                    }
                    if (shopId == 0)
                    {
                        product.ShopId = 0;
                        product.ShopName = "Admin";
                    }
                    if (shopId != 0)
                    {
                        var shop = _db.Shops.FirstOrDefault(i => i.Id == shopId);
                        if (shop != null)
                        {
                            product.ShopId = shop.Id;
                            product.ShopName = shop.Name;
                            product.ShopCategoryId = shop.ShopCategoryId;
                            product.ShopCategoryName = shop.ShopCategoryName;
                        }
                    }
                    product.UpdatedBy = user.Name;
                    product.DateUpdated = DateTime.Now;

                    _db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                _db.Dispose();
               
            }

            
        }

        [AccessPolicy(PageCode = "SHNMPRMU014")]
        public JsonResult UpdateMappingProduct(int masterproductId, int id, int shopId, bool isCheck)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var masterproduct = _db.MasterProducts.FirstOrDefault(i => i.Id == masterproductId);
            var product = _db.Products.FirstOrDefault(i => i.Id == id);
            if (isCheck == true)
            {
                product.MasterProductId = 0;
                product.IBarU = 0;
                product.UpdatedBy = user.Name;
                product.DateUpdated = DateTime.Now;
                _db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
            }
            else
            {
                if (product != null && masterproduct != null)
                {
                    product.MasterProductId = masterproductId;
                    product.MasterProductId = masterproduct.Id;

                    if (product.Customisation == false && masterproduct.Customisation != false)
                    {
                        product.Customisation = masterproduct.Customisation;
                    }
                    if (product.Price == 0 && masterproduct.Price != 0)
                    {
                        product.Price = masterproduct.Price;
                    }

                    if (product.IBarU == 0 && masterproduct.IBarU != null)
                    {
                        product.IBarU = Convert.ToInt32(masterproduct.IBarU);
                    }

                    if (product.TypeId == 0 && masterproduct.ProductTypeId != 0)
                    {
                        product.TypeId = masterproduct.ProductTypeId;
                    }
                    if (masterproduct.Price != 0)
                    {
                        product.Price = masterproduct.Price;
                    }

                    if (masterproduct.IBarU != null)
                    {
                        product.IBarU = Convert.ToInt32(masterproduct.IBarU);
                    }

                    if (masterproduct.ProductTypeId != 0)
                    {
                        product.TypeId = masterproduct.ProductTypeId;
                    }
                    if (shopId == 0)
                    {
                        product.ShopId = 0;
                        product.ShopName = "Admin";
                    }
                    if (shopId != 0)
                    {
                        var shop = _db.Shops.FirstOrDefault(i => i.Id == shopId);
                        if (shop != null)
                        {
                            product.ShopId = shop.Id;
                            product.ShopName = shop.Name;
                            product.ShopCategoryId = shop.ShopCategoryId;
                            product.ShopCategoryName = shop.ShopCategoryName;
                        }
                    }
                    product.UpdatedBy = user.Name;
                    product.DateUpdated = DateTime.Now;
                    _db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            bool results = true;
            return Json(new { results, shopId }, JsonRequestBehavior.AllowGet);
        }

        // Upload FMCG MasterItems
        [AccessPolicy(PageCode = "SHNMPRI005")]
        public ActionResult GroceryEntry()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNMPRI005")]
        public ActionResult GroceryEntry(HttpPostedFileBase upload, GroceryPageModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            DataTable dt = new DataTable();

            if (model.button == "upload")
            {
                string path = Server.MapPath("~/Content/ExcelUpload/" + upload.FileName);
                upload.SaveAs(path);
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        Stream stream = upload.InputStream;
                        IExcelDataReader reader = null;

                        if (upload.FileName.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (upload.FileName.EndsWith(".xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "This file format is not supported");
                            return View();
                        }
                        DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });

                        // reader.IsFirstRowAsColumnNames = true;

                        reader.Close();

                        model.DataTable = result.Tables[0];
                        model.Filename = upload.FileName;
                        return View(model);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Please Upload Your file");
                    }

                }
            }
            else
            {
                string path = Server.MapPath("~/Content/ExcelUpload/" + model.Filename);
                string excelConnectionString = @"Provider='Microsoft.ACE.OLEDB.12.0';Data Source='" + path + "';Extended Properties='Excel 12.0 Xml;IMEX=1'";

                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                using (OleDbConnection connExcel = new OleDbConnection(excelConnectionString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                // Insert records to database table.
                // MainPageModel entities = new MainPageModel();
                foreach (DataRow row in dt.Rows)
                {
                    var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Name == row[model.Name].ToString() && i.Status == 0);// MasterProduct.GetName(row[model.Name].ToString());
                    if (masterProduct == null)
                    {
                        model.CategoryId = CheckCategory(row[model.CategoryName].ToString(), model.ProductTypeId);
                        model.SubCategoryCode1 = CheckSubCategory(model.CategoryId, row[model.CategoryName].ToString(), row[model.SubCategoryName1].ToString(), model.ProductTypeId);
                        _db.MasterProducts.Add(new MasterProduct
                        {
                            Name = row[model.Name].ToString(),
                           // Code = _generatedCode("MPR"),
                            BrandId = CheckBrand(row[model.BrandName].ToString(), model.ProductTypeId),
                            BrandName = row[model.BrandName].ToString(),
                            SizeLB = row[model.SizeLB].ToString(),
                            Weight = Convert.ToDouble(row[model.weight]),
                            GoogleTaxonomyCode = row[model.GoogleTaxonomyCode].ToString(),
                            ASIN = row[model.ASIN].ToString(),
                            CategoryId = model.CategoryId,
                            CategoryName = row[model.CategoryName].ToString(),
                            ShortDescription = row[model.ShortDescription].ToString(),
                            LongDescription = row[model.LongDescription].ToString(),
                            Price = Convert.ToDouble(row[model.Price]),
                            ImagePath1 = row[model.ImagePath1].ToString(),
                            ImagePath2 = row[model.ImagePath2].ToString(),
                            ImagePath3 = row[model.ImagePath3].ToString(),
                            ImagePath4 = row[model.ImagePath4].ToString(),
                            ImagePath5 = row[model.ImagePath5].ToString(),
                            SubCategoryId = model.SubCategoryCode1,
                            SubCategoryName = row[model.SubCategoryName1].ToString(),
                            NextSubCategoryId = CheckNextSubCategory(model.SubCategoryCode1, row[model.SubCategoryName1].ToString(), row[model.SubCategoryName2].ToString(), model.ProductTypeId),
                            NextSubCategoryName = row[model.SubCategoryName2].ToString(),
                            ProductTypeId = model.ProductTypeId,
                            PackageId = CheckPackage(row[model.PackageName].ToString()),
                            PackageName = row[model.PackageName].ToString(),
                            MeasurementUnitId = CheckFMCGMeasurementUnit(row[model.MeasurementUnitName].ToString()),
                            MeasurementUnitName = row[model.MeasurementUnitName].ToString(),
                            Status = 0,
                            CreatedBy = user.Name,
                            UpdatedBy = user.Name,
                            DateEncoded = DateTime.Now,
                            DateUpdated = DateTime.Now,
                        });
                        _db.SaveChanges();
                    }
                }
            }

            return View();
        }

        // FMCG List
        [AccessPolicy(PageCode = "SHNMPRFL022")]
        public ActionResult FMCGList()
        {
            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from mp in _db.MasterProducts select mp).OrderBy(mp => mp.Name).Where(mp => mp.Status == 0 && mp.ProductTypeId == 2).ToList();
            return View(List);
        }

        // FMCG Create
        [AccessPolicy(PageCode = "SHNMPRFC023")]
        public ActionResult FMCGCreate()
        {

            if (Session["USER"] == null)
            {
                return RedirectToAction("LogOut", "Home");
            }

            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNMPRFC023")]
        public ActionResult FMCGCreate(MasterFMCGCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _mapper.Map<MasterFMCGCreateEditViewModel, MasterProduct>(model);
            var product = _db.Products.Where(s => s.MasterProductId == model.Id).FirstOrDefault();
            prod.CreatedBy = user.Name;
            prod.UpdatedBy = user.Name;
            prod.ProductTypeName = "FMCG";
            prod.ProductTypeId = 2;
            //prod.Code = _generatedCode("MPR");
            if(model.NickName == null)
            {
                prod.NickName = model.Name;
            }
            var name = _db.MasterProducts.FirstOrDefault(i => i.Name == model.Name && i.Status == 0 && i.ProductTypeId == 2);
            try
            {
                //if (model.CategoryCode != null)
                //{
                //    prod.CategoryCode = String.Join(",", model.CategoryCode);
                //    StringBuilder sb = new StringBuilder();
                //    foreach (var s in model.CategoryCode)
                //    {
                //        var cat = _db.Categories.FirstOrDefault(i => i.Code == s);
                //        sb.Append(cat.Name);
                //        sb.Append(",");
                //    }
                //    if (sb.Length >= 1)
                //    {
                //        model.CategoryName = sb.ToString().Remove(sb.Length - 1);
                //        prod.CategoryName = model.CategoryName;
                //    }
                //    else
                //    {
                //        model.CategoryName = sb.ToString();
                //        prod.CategoryName = model.CategoryName;
                //    }
                //}
                //if (model.SubCategoryCode != null)
                //{
                //    prod.SubCategoryCode = String.Join(",", model.SubCategoryCode);
                //    StringBuilder sb = new StringBuilder();
                //    foreach (var s in model.SubCategoryCode)
                //    {
                //        var scat = _db.SubCategories.FirstOrDefault(i => i.Code == s);// SubCategory.Get(s);
                //        sb.Append(scat.Name);
                //        sb.Append(",");
                //    }
                //    if (sb.Length >= 1)
                //    {
                //        model.SubCategoryName = sb.ToString().Remove(sb.Length - 1);
                //        prod.SubCategoryName = model.SubCategoryName;
                //    }
                //    else
                //    {
                //        model.SubCategoryName = sb.ToString();
                //        prod.SubCategoryName = model.SubCategoryName;
                //    }
                //}
                //if (model.NextSubCategoryCode != null)
                //{
                //    prod.NextSubCategoryCode = String.Join(",", model.NextSubCategoryCode);
                //    StringBuilder sb = new StringBuilder();
                //    foreach (var s in model.NextSubCategoryCode)
                //    {
                //        var nscat = _db.NextSubCategories.FirstOrDefault(i => i.Code == s);// NextSubCategory.Get(s);
                //        sb.Append(nscat.Name);
                //        sb.Append(",");
                //    }
                //    if (sb.Length >= 1)
                //    {
                //        model.NextSubCategoryName = sb.ToString().Remove(sb.Length - 1);
                //        prod.NextSubCategoryName = model.NextSubCategoryName;
                //    }
                //    else
                //    {
                //        model.NextSubCategoryName = sb.ToString();
                //        prod.NextSubCategoryName = model.NextSubCategoryName;
                //    }
                //}
                //ProductImage1
                if (model.FMCGImage1 != null)
                {
                    uc.UploadFiles(model.FMCGImage1.InputStream, prod.Id + "_" + model.FMCGImage1.FileName, accesskey, secretkey, "image");
                    prod.ImagePath1 = prod.Id + "_" + model.FMCGImage1.FileName.Replace(" ","");
                    
                }

                //ProductImage2
                if (model.FMCGImage2 != null)
                {
                    uc.UploadFiles(model.FMCGImage2.InputStream, prod.Id + "_" + model.FMCGImage2.FileName, accesskey, secretkey, "image");
                    prod.ImagePath2 = prod.Id + "_" + model.FMCGImage2.FileName.Replace(" ", "");
                  
                }

                //ProductImage3
                if (model.FMCGImage3 != null)
                {
                    uc.UploadFiles(model.FMCGImage3.InputStream, prod.Id + "_" + model.FMCGImage3.FileName, accesskey, secretkey, "image");
                    prod.ImagePath3 = prod.Id + "_" + model.FMCGImage3.FileName.Replace(" ", "");
                  
                }

                //ProductImage4
                if (model.FMCGImage4 != null)
                {
                    uc.UploadFiles(model.FMCGImage4.InputStream, prod.Id + "_" + model.FMCGImage4.FileName, accesskey, secretkey, "image");
                    prod.ImagePath4 = prod.Id + "_" + model.FMCGImage4.FileName.Replace(" ", "");
                    
                }

                //ProductImage5
                if (model.FMCGImage5 != null)
                {
                    uc.UploadFiles(model.FMCGImage5.InputStream, prod.Id + "_" + model.FMCGImage5.FileName, accesskey, secretkey, "image");
                    prod.ImagePath5 = prod.Id + "_" + model.FMCGImage5.FileName.Replace(" ", "");
                    
                }
                if (name == null)
                {
                    prod.DateEncoded = DateTime.Now;
                    prod.DateUpdated = DateTime.Now;
                    prod.Status = 0;
                    _db.MasterProducts.Add(prod);
                    _db.SaveChanges();
                    product.DateUpdated = DateTime.Now;
                    _db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    // MasterProduct.Add(prod);
                    ViewBag.Message = model.Name + " Saved Successfully!";
                }
                else
                {
                    ViewBag.ErrorMessage = model.Name + " Already Exist";
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

        // FMCG Update
        [AccessPolicy(PageCode = "SHNMPRFE024")]
        public ActionResult FMCGEdit(string Id)
        {
            var dCode = AdminHelpers.DCodeInt(Id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dCode.ToString()))
                return HttpNotFound();
            var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Id == dCode);// MasterProduct.Get(dCode);
            if (masterProduct.ImagePath1 != null)
                masterProduct.ImagePath1 = masterProduct.ImagePath1.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (masterProduct.ImagePath2 != null)
                masterProduct.ImagePath2 = masterProduct.ImagePath2.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (masterProduct.ImagePath3 != null)
                masterProduct.ImagePath3 = masterProduct.ImagePath3.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (masterProduct.ImagePath4 != null)
                masterProduct.ImagePath4 = masterProduct.ImagePath4.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            if (masterProduct.ImagePath5 != null)
                masterProduct.ImagePath5 = masterProduct.ImagePath5.Replace("%", "%25").Replace("% ", "%25").Replace("+", "%2B").Replace(" + ", "+%2B+").Replace("+ ", "%2B+").Replace(" ", "+").Replace("#", "%23");
            var model = _mapper.Map<MasterProduct, MasterFMCGCreateEditViewModel>(masterProduct);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNMPRFE024")]
        public ActionResult FMCGEdit(MasterFMCGCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var prod = _db.MasterProducts.FirstOrDefault(i => i.Id == model.Id);// MasterProduct.Get(model.Code);
            var product = _db.Products.Where(s => s.MasterProductId == model.Id).FirstOrDefault();
            _mapper.Map(model, prod);
            prod.Name = model.Name;
            prod.ProductTypeId = model.ProductTypeId;
            prod.UpdatedBy = user.Name;
            prod.DateUpdated = DateTime.Now;
            if (model.NickName == null)
            {
                prod.NickName = model.Name;
            }
            try
            {
                //if (model.CategoryCode != null)
                //{
                //    prod.CategoryCode = String.Join(",", model.CategoryCode);
                //    StringBuilder sb = new StringBuilder();
                //    foreach (var s in model.CategoryCode)
                //    {
                //        var cat = _db.Categories.FirstOrDefault(i => i.Code == s);// Category.Get(s);
                //        sb.Append(cat.Name);
                //        sb.Append(",");
                //    }
                //    if (sb.Length >= 1)
                //    {
                //        model.CategoryName = sb.ToString().Remove(sb.Length - 1);
                //        prod.CategoryName = model.CategoryName;
                //    }
                //    else
                //    {
                //        model.CategoryName = sb.ToString();
                //        prod.CategoryName = model.CategoryName;
                //    }
                //}
                //if (model.SubCategoryCode != null)
                //{
                //    prod.SubCategoryCode = String.Join(",", model.SubCategoryCode);
                //    StringBuilder sb = new StringBuilder();
                //    foreach (var s in model.SubCategoryCode)
                //    {
                //        var scat = _db.SubCategories.FirstOrDefault(i => i.Code == s);// SubCategory.Get(s);
                //        sb.Append(scat.Name);
                //        sb.Append(",");
                //    }
                //    if (sb.Length >= 1)
                //    {
                //        model.SubCategoryName = sb.ToString().Remove(sb.Length - 1);
                //        prod.SubCategoryName = model.SubCategoryName;
                //    }
                //    else
                //    {
                //        model.SubCategoryName = sb.ToString();
                //        prod.SubCategoryName = model.SubCategoryName;
                //    }
                //}
                //if (model.NextSubCategoryCode != null)
                //{
                //    prod.NextSubCategoryCode = String.Join(",", model.NextSubCategoryCode);
                //    StringBuilder sb = new StringBuilder();
                //    foreach (var s in model.NextSubCategoryCode)
                //    {
                //        var nscat = _db.NextSubCategories.FirstOrDefault(i => i.Code == s);// NextSubCategory.Get(s);
                //        sb.Append(nscat.Name);
                //        sb.Append(",");
                //    }
                //    if (sb.Length >= 1)
                //    {
                //        model.NextSubCategoryName = sb.ToString().Remove(sb.Length - 1);
                //        prod.NextSubCategoryName = model.NextSubCategoryName;
                //    }
                //    else
                //    {
                //        model.NextSubCategoryName = sb.ToString();
                //        prod.NextSubCategoryName = model.NextSubCategoryName;
                //    }
                //}
                //ProductImage1
                if (model.FMCGImage1 != null)
                {
                    uc.UploadFiles(model.FMCGImage1.InputStream, prod.Id + "_" + model.FMCGImage1.FileName, accesskey, secretkey, "image");
                    prod.ImagePath1 = prod.Id + "_" + model.FMCGImage1.FileName.Replace(" ", "");
                }

                //ProductImage2
                if (model.FMCGImage2 != null)
                {
                    uc.UploadFiles(model.FMCGImage2.InputStream, prod.Id + "_" + model.FMCGImage2.FileName, accesskey, secretkey, "image");
                    prod.ImagePath2 = prod.Id + "_" + model.FMCGImage2.FileName.Replace(" ", "");
                }

                //ProductImage3
                if (model.FMCGImage3 != null)
                {
                    uc.UploadFiles(model.FMCGImage3.InputStream, prod.Id + "_" + model.FMCGImage3.FileName, accesskey, secretkey, "image");
                    prod.ImagePath3 = prod.Id + "_" + model.FMCGImage3.FileName.Replace(" ", "");
                }

                //ProductImage4
                if (model.FMCGImage4 != null)
                {
                    uc.UploadFiles(model.FMCGImage4.InputStream, prod.Id + "_" + model.FMCGImage4.FileName, accesskey, secretkey, "image");
                    prod.ImagePath4 = prod.Id + "_" + model.FMCGImage4.FileName.Replace(" ", "");
                }

                //ProductImage5
                if (model.FMCGImage5 != null)
                {
                    uc.UploadFiles(model.FMCGImage5.InputStream, prod.Id + "_" + model.FMCGImage5.FileName, accesskey, secretkey, "image");
                    prod.ImagePath5 = prod.Id + "_" + model.FMCGImage5.FileName.Replace(" ", "");
                }
                prod.DateUpdated = DateTime.Now;
                _db.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                if (product != null)
                {
                    product.MasterProductId = prod.Id;
                   // product.MasterProductName = prod.Name;
                    product.DateUpdated = DateTime.Now;
                    _db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }

                //  MasterProduct.Edit(prod, out int error);
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

            return RedirectToAction("FMCGList");
        }

        // FMCG Delete
        [AccessPolicy(PageCode = "SHNMPRFD025")]
        public ActionResult FMCGDelete(string Id)
        {
            var dCode = AdminHelpers.DCodeInt(Id);
            var master = _db.MasterProducts.FirstOrDefault(i => i.Id == dCode);
            master.Status = 2;
            _db.Entry(master).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("FMCGList");
        }

        // Json Results
        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public JsonResult GetMasterItemSelect2(string q = "")
        {
            if (q != "")
            {
                var model = _db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId != 1).Select(i => new
                {
                    id = i.Id,
                    text = i.Name,
                    image = i.ImagePath1,
                    description = i.LongDescription,
                    brandname = i.BrandName,
                    categoryname = i.CategoryName,
                    price = i.Price,
                    type = i.ProductTypeName
                }).Take(50).ToList();
                return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }



        }

        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public JsonResult GetMasterProductList(string itemName)
        {
            string result = itemName.Substring(0, 3);
            var model = new List<ItemMappingViewModel.ItemMappingList>();
            model = _db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(result) && a.Status == 0).Select(i => new ItemMappingViewModel.ItemMappingList
            {
                Id = i.Id,
                Name = i.Name,
                ImagePath = i.ImagePath1,
                LongDescription = i.LongDescription
            }).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public async Task<JsonResult> GetProductItemSelect2(int shopId, string q = "")
        {
            var model = await _db.Products
                .Where(a => a.Name.Contains(q) && a.ShopId == shopId && a.Status == 0 && a.MasterProductId == 0)
                .OrderBy(i => i.Name).Select(i => new
                {
                    id = i.Id,
                    text = i.Name,
                    type = i.TypeName
                }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public async Task<JsonResult> GetPendingProductItemSelect2(int shopId)
        {
            var model = await _db.Products
                .Join(_db.MasterProducts, p => p.MasterProductId, m => m.Id, (p, m) => new { p, m })
                .Where(a => a.p.ShopId == shopId && a.p.Status == 0 && a.p.MasterProductId == 0)
                .OrderBy(i => i.p.Name).Select(i => new
                {
                    Id = i.p.Id,
                    Name = i.m.Name,
                    BrandName = i.m.BrandName,
                    CategoryName = i.m.CategoryName,
                    ProductType = i.p.TypeName,
                    Price = i.p.Price
                }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDishMasterSelect2(string q = "")
        {
            var model = await _db.MasterProducts.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 1).Select(i => new
            {
                id = i.Id,
                text = i.Name,
                CategoryId = i.CategoryId,
                CategoryName = i.CategoryName,
                BrandId = i.BrandId,
                BrandName = i.BrandName,
                ShortDescription = i.ShortDescription,
                LongDescription = i.LongDescription,
                Customisation = i.Customisation,
                ColorCode = i.ColorCode,
                ImagePath = i.ImagePath1,
                Price = i.Price,
                ProductType = i.ProductTypeName,
                ImagePath1 = i.ImagePath1
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // Brand Select2
        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public async Task<JsonResult> GetMedicalBrandSelect2(string q = "")
        {
            var model = await _db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 3).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetFMCGBrandSelect2(string q = "")
        {
            var model = await _db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 2).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // Category Select2
        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetDishCategorySelect2(string q = "")
        {
            var model = await _db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 1).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetCategorySelect2(string q = "")
        {
            var model = await _db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 2).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public async Task<JsonResult> GetMedicalCategorySelect2(string q = "")
        {
            var model = await _db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 3).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // Measurement Unit Select2
        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public async Task<JsonResult> GetDrugUnitSelect2(string q = "")
        {
            var model = await _db.MeasurementUnits.Where(a => a.UnitName.Contains(q) && a.Status == 0 && a.Type == 1).OrderBy(i => i.UnitName).Select(i => new
            {
                id = i.Id,
                text = i.UnitName
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetFMCGMeasurementUnitSelect2(string q = "")
        {
            var model = await _db.MeasurementUnits.OrderBy(i => i.UnitName).Where(a => a.UnitName.Contains(q) && a.Status == 0 && a.Type == 2).Select(i => new
            {
                id = i.Id,
                text = i.UnitName
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // Drug Compound Detail Select2
        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public async Task<JsonResult> GetDrugCompoundDetailSelect2(string q = "")
        {
            var model = await _db.DrugCompoundDetails.Where(a => a.AliasName.Contains(q) && a.Status == 0).OrderBy(i => i.AliasName).Select(i => new
            {
                id = i.Id,
                text = i.AliasName
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        //Package Select2
        [AccessPolicy(PageCode = "SHNMPRMC007")]
        public async Task<JsonResult> GetMedicalPackageSelect2(string q = "")
        {
            var model = await _db.Packages.Where(a => a.Name.Contains(q) && a.Status == 0 && a.Type == 1).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetFMCGPackageSelect2(string q = "")
        {
            var model = await _db.Packages.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.Type == 2).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // Shop Select2
        [AccessPolicy(PageCode = "SHNMPRIM011")]
        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await _db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // Sub Category Select2
        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetSubCategorySelect2(string q = "")
        {
            var model = await _db.SubCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 2).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync(); 

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // NextSub Category Select2
        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetNextSubCategorySelect2(string q = "")
        {
            var model = await _db.NextSubCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 2).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // AddonCategory Select2
        public async Task<JsonResult> GetAddonCategorySelect2(string q = "")
        {
            var model = await _db.AddOnCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // PortionSelect2
        public async Task<JsonResult> GetPortionSelect2(string q = "")
        {
            var model = await _db.Portions.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // AddOns
        [HttpPost]
        [AccessPolicy(PageCode = "")]
        public JsonResult AddToAddOns(MasterAddOnsCreateViewModel model)
        {
            List<MasterAddOnsCreateViewModel> addOns = Session["AddOns"] as List<MasterAddOnsCreateViewModel>;
            if (addOns == null)
            {
                addOns = new List<MasterAddOnsCreateViewModel>();
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
                AddOnItemName = model.AddOnItemName,
                CrustName = model.CrustName,
                CrustPrice = model.CrustPrice,
                AddOnType = model.AddOnType,
                MasterProductId = model.MasterProductId,
                Id = model.Id
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "")]
        public JsonResult RemoveFromAddOns(int id)
        {
            List<MasterAddOnsCreateViewModel> addOns = Session["AddOns"] as List<MasterAddOnsCreateViewModel>;

            if (addOns.Remove(addOns.SingleOrDefault(i => i.Id == id)))
            {
                this.Session["AddOns"] = addOns;
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AccessPolicy(PageCode = "")]
        public JsonResult EditRemoveAddOns(int id, string code)
        {
            List<MasterAddOnsCreateViewModel> addOns = Session["AddOns"] as List<MasterAddOnsCreateViewModel>;
            if (addOns == null)
            {
                addOns = new List<MasterAddOnsCreateViewModel>();
            }
            if (addOns.Remove(addOns.SingleOrDefault(i => i.Id == id)))
            {
                this.Session["AddOns"] = addOns;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            if (code != "")
            {
                var addon = _db.ProductDishAddOns.FirstOrDefault(i => i.Id == id);
                addon.Status = 2;
                addon.DateUpdated = DateTime.Now;
                _db.Entry(addon).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();

                if (addOns.Remove(addOns.SingleOrDefault(i => i.Id == id)))
                {
                    this.Session["AddOns"] = addOns;
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        // Upload Master Items
        [AccessPolicy(PageCode = "SHNMPRI005")]
        public ActionResult Index()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNMPRI005")]
        public ActionResult Index(HttpPostedFileBase upload, MainPageModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            DataTable dt = new DataTable();

            if (model.button == "upload")
            {
                string path = Server.MapPath("~/Content/ExcelUpload/" + upload.FileName);
                upload.SaveAs(path);
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        Stream stream = upload.InputStream;
                        IExcelDataReader reader = null;

                        if (upload.FileName.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (upload.FileName.EndsWith(".xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "This file format is not supported");
                            return View();
                        }
                        DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });

                        // reader.IsFirstRowAsColumnNames = true;

                        reader.Close();

                        model.DataTable = result.Tables[0];
                        model.Filename = upload.FileName;
                        return View(model);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Please Upload Your file");
                    }

                }
            }
            else
            {
                string path = Server.MapPath("~/Content/ExcelUpload/" + model.Filename);
                string excelConnectionString = @"Provider='Microsoft.ACE.OLEDB.12.0';Data Source='" + path + "';Extended Properties='Excel 12.0 Xml;IMEX=1'";

                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                using (OleDbConnection connExcel = new OleDbConnection(excelConnectionString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                // Insert records to database table.
                // MainPageModel entities = new MainPageModel();
                foreach (DataRow row in dt.Rows)
                {
                    var masterProduct = _db.MasterProducts.FirstOrDefault(i => i.Name == row[model.Name].ToString() && i.Status == 0);// MasterProduct.GetName(row[model.Name].ToString());
                    if (masterProduct == null)
                    {
                        _db.MasterProducts.Add(new MasterProduct
                        {
                            Name = row[model.Name].ToString(),
                            // Code = _generatedCode("MPR"),
                            BrandId = CheckBrand(row[model.BrandName].ToString(), model.ProductTypeId),
                            BrandName = row[model.BrandName].ToString(),
                            CategoryId = CheckCategory(row[model.CategoryName].ToString(), model.ProductTypeId),
                            CategoryName = row[model.CategoryName].ToString(),
                            ShortDescription = row[model.ShortDescription].ToString(),
                            LongDescription = row[model.LongDescription].ToString(),
                            Customisation = Convert.ToBoolean(row[model.Customisation]),
                            ColorCode = row[model.ColorCode].ToString(),
                            Price = Convert.ToDouble(row[model.Price]),
                            ImagePath1 = row[model.ImagePath1].ToString(),
                            ImagePath2 = row[model.ImagePath2].ToString(),
                            ImagePath3 = row[model.ImagePath3].ToString(),
                            ImagePath4 = row[model.ImagePath4].ToString(),
                            ImagePath5 = row[model.ImagePath5].ToString(),
                            ProductTypeId = model.ProductTypeId,
                            Status = 0,
                            CreatedBy = user.Name,
                            UpdatedBy = user.Name,
                            DateEncoded = DateTime.Now,
                            DateUpdated = DateTime.Now,
                        });
                        _db.SaveChanges();
                    }
                }
            }

            return View();
        }

        public int CheckBrand(string BrandName, int ProductTypeId)
        {
            var brand = _db.Brands.FirstOrDefault(i => i.Name == BrandName && i.Status == 0);// Brand.GetName(BrandName);
            if (brand != null)
            {
                return brand.Id;
            }
            else
            {
                Brand br = new Brand();
                br.Name = BrandName;
                br.ProductTypeId = ProductTypeId;
                //br.Code = _generatedCode("BRA");
                br.Status = 0;
                br.DateEncoded = DateTime.Now;
                br.DateUpdated = DateTime.Now;
                _db.Brands.Add(br);
                _db.SaveChanges();
                //br.Code = Brand.Add(br, out int error);
                return br.Id;
            }

        }

        public int CheckSubCategory(int CategoryId, string CategoryName, string SubCategoryName1, int ProductTypeId)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var subCategory = _db.SubCategories.FirstOrDefault(i => i.Name == SubCategoryName1 && i.Status == 0);// SubCategory.GetName(SubCategoryName1);
            if (subCategory != null)
            {

                return subCategory.Id;
            }
            else
            {
                SubCategory sub = new SubCategory();
                sub.Name = SubCategoryName1;
                sub.CategoryId = CategoryId;
                sub.CategoryName = CategoryName;
                sub.ProductTypeId = ProductTypeId;
                sub.CreatedBy = user.Name;
                sub.UpdatedBy = user.Name;
                //sub.Code = _generatedCode("SCT");
                sub.Status = 0;
                sub.DateEncoded = DateTime.Now;
                sub.DateUpdated = DateTime.Now;
                _db.SubCategories.Add(sub);
                _db.SaveChanges();
                //sub.Code = SubCategory.Add(sub, out int error);
                return sub.Id;
            }

        }

        public int CheckNextSubCategory(int subCategoryCode1, string SubCategoryName1, string SubCategoryName2, int ProductTypeId)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);

            var nextSubCategory = _db.NextSubCategories.FirstOrDefault(i => i.Name == SubCategoryName2 && i.Status == 0);// NextSubCategory.GetName(SubCategoryName2);
            if (nextSubCategory != null)
            {

                return nextSubCategory.Id;
            }
            else
            {
                NextSubCategory sub = new NextSubCategory();
                sub.Name = SubCategoryName2;
                sub.SubCategoryId = subCategoryCode1;
                sub.SubCategoryName = SubCategoryName1;
                sub.ProductTypeId = ProductTypeId;
                sub.CreatedBy = user.Name;
                sub.UpdatedBy = user.Name;
                // sub.Code = _generatedCode("NSC");
                sub.DateEncoded = DateTime.Now;
                sub.DateUpdated = DateTime.Now;
                sub.Status = 0;
                _db.NextSubCategories.Add(sub);
                _db.SaveChanges();
                //sub.Code = NextSubCategory.Add(sub, out int error);
                return sub.Id;
            }

        }

        public int CheckCategory(string CategoryName, int ProductTypeId)
        {
            var category = _db.Categories.FirstOrDefault(i => i.Name == CategoryName && i.Status == 0);// Category.GetName(CategoryName);
            if (category != null)
            {

                return category.Id;
            }
            else
            {
                Category cat = new Category();
                cat.Name = CategoryName;
                cat.ProductTypeId = ProductTypeId;
                //cat.Code = _generatedCode("CAT");
                cat.Status = 0;
                cat.DateEncoded = DateTime.Now;
                cat.DateUpdated = DateTime.Now;
                _db.Categories.Add(cat);
                _db.SaveChanges();
                return cat.Id;
            }

        }
    }
}