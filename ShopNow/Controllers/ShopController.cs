﻿using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Newtonsoft.Json;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class ShopController : Controller
    {

        private ShopnowchatEntities db = new ShopnowchatEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        UploadContent uc = new UploadContent();
        private const string _prefix = "REF";
       // private static readonly string bucketName = "shopnowchat.com/";
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.APSouth1;
        private static readonly string accesskey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static readonly string secretkey = ConfigurationManager.AppSettings["AWSSecretKey"];
        private static string _genCode(string _prefix)
        {
                return ShopNow.Helpers.DRC.Generate(_prefix);
        }
        private static string _referenceCode
        {
            get
            {
                return ShopNow.Helpers.DRC.Generate(_prefix);
            }
        }

        private static string _generatedCode
        {
            get
            {
                return ShopNow.Helpers.DRC.GenerateOTP();
            }
        }
        private static string _generatedPassword
        {
            get
            {
                return ShopNow.Helpers.DRC.GeneratePassword();
            }
        }

        public ShopController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<Shop, ShopListViewModel.ShopList>();
                config.CreateMap<ShopRegisterViewModel, Shop>();
                config.CreateMap<ShopPostEditViewModel, Shop>();
                config.CreateMap<ShopEditViewModel, Shop>();
                config.CreateMap<Shop, ShopEditViewModel>();
                config.CreateMap<OtpViewModel, OtpVerification>();
                config.CreateMap<Shop, ShopFranchiseViewModel>();
                config.CreateMap<Bill, Bill>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNSHPL005")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            //var model = new ShopListViewModel();
            var List = (from s in db.Shops
                        select s).OrderBy(s => s.Name).Where(i=> i.Status == 0).ToList();
            //model.List = (from mp in db.Shops
            //            select new ShopListViewModel.ShopList
            //            {
            //              Code = mp.Code,
            //              Name = mp.Name,
            //              OwnerPhoneNumber = mp.OwnerPhoneNumber,
            //              PhoneNumber = mp.PhoneNumber,
            //              ShopCategoryName = mp.ShopCategoryName,
            //              Address = mp.Address,
            //              DistrictName = mp.DistrictName,
            //              StateName = mp.StateName,
            //              PinCode = mp.PinCode
            //            }).OrderBy(mp => mp.Name).Where(i=> i.Status == 0).ToList();

            return View(List);
        }

        [AccessPolicy(PageCode = "SHNSHPIL004")]
        public ActionResult InactiveList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from s in db.Shops
                        select s).OrderBy(s => s.Name).Where(i => i.Status == 1) .ToList();
            return View(List);

            //var user = ((Helpers.Sessions.User)Session["USER"]);
            //ViewBag.Name = user.Name;
            //var model = new ShopListViewModel();
            //model.List = Shop.GetInActiveList().AsQueryable().ProjectTo<ShopListViewModel.ShopList>(_mapperConfiguration).OrderBy(i => i.Name).ToList();

            //return View(model);
        }

        [AccessPolicy(PageCode = "SHNSHPC001")]
        public ActionResult Create()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNSHPC001")]
        public ActionResult Create(ShopRegisterViewModel model)
        {
            //var shopduplicateCount = db.Shops.Where(m => m.Latitude == model.Latitude && m.Longitude == model.Longitude).Count();
            //var shopbeforeApprovalCount = db.Shops.Where(m => m.Latitude == model.Latitude && m.Longitude == model.Longitude && m.Status == 0).Count();
            var shopduplicateCount = db.Shops.Where(m => m.GooglePlaceId == model.GooglePlaceId).Count();
            var shopbeforeApprovalCount = db.Shops.Where(m => m.GooglePlaceId == model.GooglePlaceId && m.Status == 0).Count();
            if (shopduplicateCount != 0)
            {
                ViewBag.shopduplicateCount = shopduplicateCount;
                return View(model);
            }
            if (shopbeforeApprovalCount > 0)
            {
                ViewBag.shopbeforeApprovalCount = shopbeforeApprovalCount;
                var shopownerphonenumber = db.Shops.Where(m => m.GooglePlaceId == model.GooglePlaceId && m.Status == 0).Select(i => i.OwnerPhoneNumber).ToList();
                var shopname = db.Shops.Where(m => m.GooglePlaceId == model.GooglePlaceId && m.Status == 0).Select(i => i.Name).ToList();
                ViewBag.shopownerphonenumber = shopownerphonenumber[0].ToString();
                ViewBag.shopname = shopname[0].ToString();
                return View(model);
            }

            var user = ((Helpers.Sessions.User)Session["USER"]);
            var customer = db.Customers.FirstOrDefault(i => i.Code == user.Code); //Customer.Get(user.Code);
            var shop = _mapper.Map<ShopRegisterViewModel, Shop>(model);
            shop.Status = 1;
            var custom = db.Customers.FirstOrDefault(i => i.Code == shop.CustomerCode); // Customer.Get(shop.CustomerCode);
            if (custom != null)
            {
                shop.CustomerCode = custom.Code;
                shop.CustomerName = custom.Name;
                custom.Position = custom.Position;
                custom.DateUpdated = DateTime.Now;
                db.Entry(custom).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //Customer.Edit(custom, out errorCode);
            }
            shop.CreatedBy = user.Name;
            shop.UpdatedBy = user.Name;

            shop.Code = _genCode("SHP");
           // shop.Status = 0;
            shop.DateEncoded = DateTime.Now;
            shop.DateUpdated = DateTime.Now;
            db.Shops.Add(shop);
            db.SaveChanges();
           // shop.Code = Shop.Add(shop, out errorCode);

            Admin admin = new Admin();
            admin.AnonymisedID = shop.Code;
            admin.Code = _genCode("ADM");
            admin.Status = 0;
            admin.DateEncoded = DateTime.Now;
            admin.DateUpdated = DateTime.Now;
            db.Admins.Add(admin);
            db.SaveChanges();
            //Admin.Add(admin, out errorCode);
            //Image

            try
            {
                var ShopImg = db.Shops.FirstOrDefault(i => i.Code == shop.Code); // Shop.Get(shop.Code);
                // Shop Image
                if (model.ShopImage != null)
                {
                    uc.UploadFiles(model.ShopImage.InputStream, shop.Code + "_" + model.ShopImage.FileName, accesskey, secretkey, "image");
                    ShopImg.ImagePath = shop.Code + "_" + model.ShopImage.FileName.Replace(" ", "");
                }

                // Logo Image
                if (model.LogoImage != null)
                {
                    uc.UploadFiles(model.LogoImage.InputStream, shop.Code + "_" + model.LogoImage.FileName, accesskey, secretkey, "image");
                    ShopImg.ImageLogoPath = shop.Code + "_" + model.LogoImage.FileName.Replace(" ", "");
                }

                // Banner Image
                if (model.BannerImage != null)
                {
                    uc.UploadFiles(model.BannerImage.InputStream, shop.Code + "_" + model.BannerImage.FileName, accesskey, secretkey, "image");
                    ShopImg.ImageBannerPath = shop.Code + "_" + model.BannerImage.FileName.Replace(" ", "");
                }

                // Pan Image
               
                if (model.PanImage != null)
                {
                    uc.UploadFiles(model.PanImage.InputStream, shop.Code + "_" + model.PanImage.FileName, accesskey, secretkey, "image");
                    ShopImg.ImagePanPath = shop.Code + "_" + model.PanImage.FileName.Replace(" ", "");
                }

                if (model.PanPdf != null)
                {
                    uc.UploadFiles(model.PanPdf.InputStream, shop.Code + "_" + model.PanPdf.FileName, accesskey, secretkey, "pdf");
                    ShopImg.ImagePanPath = shop.Code + "_" + model.PanPdf.FileName.Replace(" ", "");
                }

                // GSTIN Image

                if (model.GSTINImage != null)
                {
                    uc.UploadFiles(model.GSTINImage.InputStream, shop.Code + "_" + model.GSTINImage.FileName, accesskey, secretkey, "image");
                    ShopImg.ImageGSTINPath = shop.Code + "_" + model.GSTINImage.FileName.Replace(" ", "");
                }

                if (model.GSTINPdf != null)
                {
                    uc.UploadFiles(model.GSTINPdf.InputStream, shop.Code + "_" + model.GSTINPdf.FileName, accesskey, secretkey, "pdf");
                    ShopImg.ImageGSTINPath = shop.Code + "_" + model.GSTINPdf.FileName.Replace(" ", "");
                }

               
                // Account Image

                if (model.AccountImage != null)
                {
                    uc.UploadFiles(model.AccountImage.InputStream, shop.Code + "_" + model.AccountImage.FileName, accesskey, secretkey, "image");
                    ShopImg.ImageAccountPath = shop.Code + "_" + model.AccountImage.FileName.Replace(" ", "");
                }

                if (model.AccountPdf != null)
                {
                    uc.UploadFiles(model.AccountPdf.InputStream, shop.Code + "_" + model.AccountPdf.FileName, accesskey, secretkey, "pdf");
                    ShopImg.ImageAccountPath = shop.Code + "_" + model.AccountPdf.FileName.Replace(" ", "");
                }

                // FSSAI Image

                if (model.FSSAIImage != null)
                {
                    uc.UploadFiles(model.FSSAIImage.InputStream, shop.Code + "_" + model.FSSAIImage.FileName, accesskey, secretkey, "image");
                    ShopImg.ImageFSSAIPath = shop.Code + "_" + model.FSSAIImage.FileName.Replace(" ", "");
                }

                if (model.FSSAIPdf != null)
                {
                    uc.UploadFiles(model.FSSAIPdf.InputStream, shop.Code + "_" + model.FSSAIPdf.FileName, accesskey, secretkey, "pdf");
                    ShopImg.ImageFSSAIPath = shop.Code + "_" + model.FSSAIPdf.FileName.Replace(" ", "");
                }

                // Drug Image

                if (model.DrugImage != null)
                {
                    uc.UploadFiles(model.DrugImage.InputStream, shop.Code + "_" + model.DrugImage.FileName, accesskey, secretkey, "image");
                    ShopImg.ImageDrugPath = shop.Code + "_" + model.DrugImage.FileName.Replace(" ", "");
                }
                if (model.DrugPdf != null)
                {
                    uc.UploadFiles(model.DrugPdf.InputStream, shop.Code + "_" + model.DrugPdf.FileName, accesskey, secretkey, "pdf");
                    ShopImg.ImageDrugPath = shop.Code + "_" + model.DrugPdf.FileName.Replace(" ", "");
                }
                
                // Establish Image

                if (model.EstablishImage != null)
                {
                    uc.UploadFiles(model.EstablishImage.InputStream, shop.Code + "_" + model.EstablishImage.FileName, accesskey, secretkey, "image");
                    ShopImg.ImageEstablishPath = shop.Code + "_" + model.EstablishImage.FileName.Replace(" ", "");
                }

                if (model.EstablishPdf != null)
                {
                    uc.UploadFiles(model.EstablishPdf.InputStream, shop.Code + "_" + model.EstablishPdf.FileName, accesskey, secretkey, "pdf");
                    ShopImg.ImageEstablishPath = shop.Code + "_" + model.EstablishPdf.FileName.Replace(" ", "");
                }

                // Other License Image

                if (model.OtherLicenseImage != null)
                {
                    uc.UploadFiles(model.OtherLicenseImage.InputStream, shop.Code + "_" + model.OtherLicenseImage.FileName, accesskey, secretkey, "image");
                    ShopImg.ImageOtherLicensePath = shop.Code + "_" + model.OtherLicenseImage.FileName.Replace(" ", "");
                }
                if (model.OtherLicensePdf != null)
                {
                    uc.UploadFiles(model.OtherLicensePdf.InputStream, shop.Code + "_" + model.OtherLicensePdf.FileName, accesskey, secretkey, "pdf");
                    ShopImg.ImageOtherLicensePath = shop.Code + "_" + model.OtherLicensePdf.FileName.Replace(" ", "");
                }
                
                // Authorised Distributor Image
                if (model.AuthorisedDistributorImage != null)
                {
                    uc.UploadFiles(model.AuthorisedDistributorImage.InputStream, shop.Code + "_" + model.AuthorisedDistributorImage.FileName, accesskey, secretkey, "image");
                    ShopImg.ImageAuthoriseBrandPath = shop.Code + "_" + model.AuthorisedDistributorImage.FileName.Replace(" ", "");
                }
                if (model.AuthorisedDistributorPdf != null)
                {
                    uc.UploadFiles(model.AuthorisedDistributorPdf.InputStream, shop.Code + "_" + model.AuthorisedDistributorPdf.FileName, accesskey, secretkey, "pdf");
                    ShopImg.ImageAuthoriseBrandPath = shop.Code + "_" + model.AuthorisedDistributorPdf.FileName.Replace(" ", "");
                }
                
                // Aadhar Image
                if (model.AadharImage != null)
                {
                    uc.UploadFiles(model.AadharImage.InputStream, shop.Code + "_" + model.AadharImage.FileName, accesskey, secretkey, "image");
                    ShopImg.ImageAadharPath = shop.Code + "_" + model.AadharImage.FileName.Replace(" ", "");
                }

                if (model.AadharPdf != null)
                {
                    uc.UploadFiles(model.AadharPdf.InputStream, shop.Code + "_" + model.AadharPdf.FileName, accesskey, secretkey, "pdf");
                    ShopImg.ImageAadharPath = shop.Code + "_" + model.AadharPdf.FileName.Replace(" ", "");
                }
                db.Entry(ShopImg).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
               // Shop.Edit(ShopImg, out int error);
                return RedirectToAction("InActiveList", "Shop");
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    ViewBag.Message = "Check the provided AWS Credentials.";
                    return ViewBag.Message;
                }
                else
                {
                    ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                    return ViewBag.Message;
                }
            }
        }

        [AccessPolicy(PageCode = "SHNSHPE003")]
        public ActionResult Edit(string code)
        {
            var dCode = AdminHelpers.DCode(code.Trim());
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dCode))
                return HttpNotFound();
            var shop = db.Shops.FirstOrDefault(i => i.Code == dCode); // Shop.Get(dCode);
            var customer = db.Customers.FirstOrDefault(i => i.Code == shop.CustomerCode); // Customer.Get(shop.CustomerCode);
            ViewBag.count = db.Products.Where(i => i.ShopCode == dCode).Count();
            var model = _mapper.Map<Shop, ShopEditViewModel>(shop);
            if (model.Password == null)
            {
                model.Password = customer.Password;
            }
            var otp = new OtpVerification();
            if (model.ManualPhoneNumber != null)
            {
                otp = db.OtpVerifications.FirstOrDefault(i => i.PhoneNumber == model.ManualPhoneNumber & i.Verify == true) ?? (OtpVerification)null;
            }
            else
            {
                otp = db.OtpVerifications.FirstOrDefault(i => i.PhoneNumber == model.OwnerPhoneNumber & i.Verify == false) ?? (OtpVerification)null;
            }
            if (otp != null)
            {
                model.Otp = otp.ToString();
            }
            if (model.Verify == false)
            {
                int count = 0;
                if (model.ImageAuthoriseBrandPath != null) { count++; }
                if (model.ImageDrugPath != null) { count++; }
                if (model.ImageEstablishPath != null) { count++; }
                if (model.ImageFSSAIPath != null) { count++; }
                if (model.ImageGSTINPath != null) { count++; }
                if (model.ImageOtherLicensePath != null) { count++; }
                if (model.ImagePanPath != null) { count++; }
                if (model.ImageAccountPath != null) { count++; }
                model.Count = count;
            }
            var bill = db.Bills.Where(i => i.ShopCode == shop.Code && i.NameOfBill == 0 && i.Status == 0).FirstOrDefault();
            if (bill != null)
                model.Type = bill.Type;
            //if (model.OwnerPhoneNumber != null)
            //{
            //    model.AadharName = customer.AadharName;
            //    model.AadharNumber = customer.AadharNumber;
            //    model.ImageAadharPath = customer.ImageAadharPath;
            //    model.Email = customer.Email;
            //}

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNSHPE003")]
        public ActionResult Edit(ShopEditViewModel model)
        {
            var shop = db.Shops.FirstOrDefault(i => i.Code == model.Code); 
            model.DateUpdated = DateTime.Now;
            _mapper.Map(model, shop);

            try
            {
                // Shop Image
                if (model.ShopImage != null)
                {
                    uc.UploadFiles(model.ShopImage.InputStream, shop.Code + "_" + model.ShopImage.FileName, accesskey, secretkey, "image");
                    shop.ImagePath = shop.Code + "_" + model.ShopImage.FileName.Replace(" ", "");
                }

                // Logo Image
                if (model.LogoImage != null)
                {
                    uc.UploadFiles(model.LogoImage.InputStream, shop.Code + "_" + model.LogoImage.FileName, accesskey, secretkey, "image");
                    shop.ImageLogoPath = shop.Code + "_" + model.LogoImage.FileName.Replace(" ", "");
                }

                // Banner Image
                if (model.BannerImage != null)
                {
                    uc.UploadFiles(model.BannerImage.InputStream, shop.Code + "_" + model.BannerImage.FileName, accesskey, secretkey, "image");
                    shop.ImageBannerPath = shop.Code + "_" + model.BannerImage.FileName.Replace(" ", "");
                }

                // Pan Image

                if (model.PanImage != null)
                {
                    uc.UploadFiles(model.PanImage.InputStream, shop.Code + "_" + model.PanImage.FileName, accesskey, secretkey, "image");
                    shop.ImagePanPath = shop.Code + "_" + model.PanImage.FileName.Replace(" ", "");
                }

                if (model.PanPdf != null)
                {
                    uc.UploadFiles(model.PanPdf.InputStream, shop.Code + "_" + model.PanPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImagePanPath = shop.Code + "_" + model.PanPdf.FileName.Replace(" ", "");
                }

                // GSTIN Image

                if (model.GSTINImage != null)
                {
                    uc.UploadFiles(model.GSTINImage.InputStream, shop.Code + "_" + model.GSTINImage.FileName, accesskey, secretkey, "image");
                    shop.ImageGSTINPath = shop.Code + "_" + model.GSTINImage.FileName.Replace(" ", "");
                }

                if (model.GSTINPdf != null)
                {
                    uc.UploadFiles(model.GSTINPdf.InputStream, shop.Code + "_" + model.GSTINPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageGSTINPath = shop.Code + "_" + model.GSTINPdf.FileName.Replace(" ", "");
                }


                // Account Image

                if (model.AccountImage != null)
                {
                    uc.UploadFiles(model.AccountImage.InputStream, shop.Code + "_" + model.AccountImage.FileName, accesskey, secretkey, "image");
                    shop.ImageAccountPath = shop.Code + "_" + model.AccountImage.FileName.Replace(" ", "");
                }

                if (model.AccountPdf != null)
                {
                    uc.UploadFiles(model.AccountPdf.InputStream, shop.Code + "_" + model.AccountPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageAccountPath = shop.Code + "_" + model.AccountPdf.FileName.Replace(" ", "");
                }

                // FSSAI Image

                if (model.FSSAIImage != null)
                {
                    uc.UploadFiles(model.FSSAIImage.InputStream, shop.Code + "_" + model.FSSAIImage.FileName, accesskey, secretkey, "image");
                    shop.ImageFSSAIPath = shop.Code + "_" + model.FSSAIImage.FileName.Replace(" ", "");
                }

                if (model.FSSAIPdf != null)
                {
                    uc.UploadFiles(model.FSSAIPdf.InputStream, shop.Code + "_" + model.FSSAIPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageFSSAIPath = shop.Code + "_" + model.FSSAIPdf.FileName.Replace(" ", "");
                }

                // Drug Image

                if (model.DrugImage != null)
                {
                    uc.UploadFiles(model.DrugImage.InputStream, shop.Code + "_" + model.DrugImage.FileName, accesskey, secretkey, "image");
                    shop.ImageDrugPath = shop.Code + "_" + model.DrugImage.FileName.Replace(" ", "");
                }
                if (model.DrugPdf != null)
                {
                    uc.UploadFiles(model.DrugPdf.InputStream, shop.Code + "_" + model.DrugPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageDrugPath = shop.Code + "_" + model.DrugPdf.FileName.Replace(" ", "");
                }

                // Establish Image

                if (model.EstablishImage != null)
                {
                    uc.UploadFiles(model.EstablishImage.InputStream, shop.Code + "_" + model.EstablishImage.FileName, accesskey, secretkey, "image");
                    shop.ImageEstablishPath = shop.Code + "_" + model.EstablishImage.FileName.Replace(" ", "");
                }

                if (model.EstablishPdf != null)
                {
                    uc.UploadFiles(model.EstablishPdf.InputStream, shop.Code + "_" + model.EstablishPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageEstablishPath = shop.Code + "_" + model.EstablishPdf.FileName.Replace(" ", "");
                }

                // Other License Image

                if (model.OtherLicenseImage != null)
                {
                    uc.UploadFiles(model.OtherLicenseImage.InputStream, shop.Code + "_" + model.OtherLicenseImage.FileName, accesskey, secretkey, "image");
                    shop.ImageOtherLicensePath = shop.Code + "_" + model.OtherLicenseImage.FileName.Replace(" ", "");
                }
                if (model.OtherLicensePdf != null)
                {
                    uc.UploadFiles(model.OtherLicensePdf.InputStream, shop.Code + "_" + model.OtherLicensePdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageOtherLicensePath = shop.Code + "_" + model.OtherLicensePdf.FileName.Replace(" ", "");
                }

                // Authorised Distributor Image
                if (model.AuthorisedDistributorImage != null)
                {
                    uc.UploadFiles(model.AuthorisedDistributorImage.InputStream, shop.Code + "_" + model.AuthorisedDistributorImage.FileName, accesskey, secretkey, "image");
                    shop.ImageAuthoriseBrandPath = shop.Code + "_" + model.AuthorisedDistributorImage.FileName.Replace(" ", "");
                }
                if (model.AuthorisedDistributorPdf != null)
                {
                    uc.UploadFiles(model.AuthorisedDistributorPdf.InputStream, shop.Code + "_" + model.AuthorisedDistributorPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageAuthoriseBrandPath = shop.Code + "_" + model.AuthorisedDistributorPdf.FileName.Replace(" ", "");
                }

                // Aadhar Image
                if (model.AadharImage != null)
                {
                    uc.UploadFiles(model.AadharImage.InputStream, shop.Code + "_" + model.AadharImage.FileName, accesskey, secretkey, "image");
                    shop.ImageAadharPath = shop.Code + "_" + model.AadharImage.FileName.Replace(" ", "");
                }

                if (model.AadharPdf != null)
                {
                    uc.UploadFiles(model.AadharPdf.InputStream, shop.Code + "_" + model.AadharPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageAadharPath = shop.Code + "_" + model.AadharPdf.FileName.Replace(" ", "");
                }
                if (model.Count == 0)
                {
                    shop.Verify = true;
                }
                else
                {
                    shop.Verify = false;
                }
                db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //Delivery charge Assign
                var deliveryChargeList = db.Bills.Where(i => i.Type == model.Type && i.Status == 0 && i.ShopCode == "Admin").ToList();
                var deliveryChargeShopList = db.Bills.Where(i => i.ShopCode == shop.Code && i.NameOfBill == 0 && i.Status == 0).ToList();
                var general = db.Bills.FirstOrDefault(i => i.Type == model.Type && i.DeliveryRateSet == 0 && i.Status == 0 && i.ShopCode == "Admin");
                var special = db.Bills.FirstOrDefault(i => i.Type == model.Type && i.DeliveryRateSet == 1 && i.Status == 0 && i.ShopCode == "Admin");
                if (deliveryChargeShopList.Count() > 0)
                {
                    foreach (var dc in deliveryChargeShopList)
                    {
                        var dcbill = db.Bills.FirstOrDefault(i => i.Code == dc.Code);
                        if (dcbill.DeliveryRateSet == 0)
                        {
                            dcbill.DeliveryChargeKM = general.DeliveryChargeKM;
                            dcbill.DeliveryChargeOneKM = general.DeliveryChargeOneKM;
                        }
                        else
                        {
                            dcbill.DeliveryChargeKM = special.DeliveryChargeKM;
                            dcbill.DeliveryChargeOneKM = special.DeliveryChargeOneKM;
                        }
                        dcbill.Type = model.Type;
                        dcbill.DateUpdated = DateTime.Now;
                        db.Entry(dcbill).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    foreach (var item in deliveryChargeList)
                    {
                        var bill = _mapper.Map<Bill, Bill>(item);
                        bill.Code = _genCode("BIL");
                        bill.Status = 0;
                        bill.DateEncoded = DateTime.Now;
                        bill.DateUpdated = DateTime.Now;
                        bill.ShopCode = shop.Code;
                        bill.ShopName = shop.Name;
                        db.Bills.Add(bill);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("List", "Shop");
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    ViewBag.Message = "Check the provided AWS Credentials.";
                    return ViewBag.Message;
                }
                else
                {
                    ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                    return ViewBag.Message;
                }
            }
        }

        [AccessPolicy(PageCode = "SHNSHPD002")]
        public ActionResult Details(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            Shop sh = db.Shops.FirstOrDefault(i => i.Code == dCode); // Shop.Get(dCode);
            var model = new ShopEditViewModel();
            _mapper.Map(sh, model);
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNSHPR006")]
        public ActionResult Delete(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.FirstOrDefault(i => i.Code == dCode); // Shop.Get(dCode);
            shop.Status = 2;
            shop.DateUpdated = DateTime.Now;
            shop.UpdatedBy = user.Name;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List", "Shop");
        }

        [AccessPolicy(PageCode = "SHNSHPIA007")]
        public ActionResult InActive(string code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.FirstOrDefault(i => i.Code == code); // Shop.Get(code);
            shop.Status = 1;
            shop.DateUpdated = DateTime.Now;
            shop.UpdatedBy = user.Name;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List", "Shop");
        }

        [AccessPolicy(PageCode = "SHNSHPA008")]
        public ActionResult Active(string code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.FirstOrDefault(i => i.Code == code);  //Shop.Get(code);
            shop.Status = 0;
            shop.DateUpdated = DateTime.Now;
            shop.UpdatedBy = user.Name;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List", "Shop");
        }

        [AccessPolicy(PageCode = "SHNSHPA008")]
        public ActionResult Activate(string code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.FirstOrDefault(i => i.Code == code); // Shop.Get(code);
            shop.Status = 0;
            shop.DateUpdated = DateTime.Now;
            shop.UpdatedBy = user.Name;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List", "Shop");
        }

        [AccessPolicy(PageCode = "SHNSHPAF015")]
        public ActionResult AssignFranchise()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNSHPAF015")]
        public ActionResult AssignFranchise(ShopFranchiseViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.FirstOrDefault(i => i.Code == model.ShopCode); // Shop.Get(model.ShopCode);
            if (shop != null)
            {
                shop.MarketingAgentCode = model.MarketingAgentCode;
                shop.MarketingAgentName = model.MarketingAgentName;
                db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //Shop.Edit(shop, out int error);
            }

            return RedirectToAction("AssignedFranchiseList");
        }

        [AccessPolicy(PageCode = "SHNSHPFL017")]
        public ActionResult AssignedFranchiseList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new ShopFranchiseViewModel();
            model.List = db.Shops.Where(i => i.Status == 0 && i.MarketingAgentCode != null && i.MarketingAgentName != null)
                .Select(i => new ShopFranchiseViewModel.FranchiseList
                {
                    Name = i.Name,
                    Code = i.Code,
                    MarketingAgentCode = i.MarketingAgentCode,
                    MarketingAgentName = i.MarketingAgentName
                }).OrderBy(i => i.Name).ToList();
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNSHPFU016")]
        public ActionResult FranchiseUpdate(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new ShopFranchiseViewModel();
            var shop = db.Shops.FirstOrDefault(i => i.Code == dCode); // Shop.Get(dCode);
            _mapper.Map(shop, model);
            if (shop != null)
            {
                model.ShopCode = shop.Code;
                model.ShopName = shop.Name;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNSHPFU016")]
        public ActionResult FranchiseUpdate(ShopFranchiseViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.FirstOrDefault(i => i.Code == model.ShopCode); // Shop.Get(model.ShopCode);
            if (shop != null)
            {
                shop.MarketingAgentCode = model.MarketingAgentCode;
                shop.MarketingAgentName = model.MarketingAgentName;
                shop.UpdatedBy = user.Name;
                shop.DateUpdated = DateTime.Now;
                db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
               // Shop.Edit(shop, out int error);
            }

            return RedirectToAction("AssignedFranchiseList");
        }

        [AccessPolicy(PageCode = "SHNSHPFR018")]
        public JsonResult FranchiseRemove(string code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            var shop = db.Shops.FirstOrDefault(i => i.Code == code); // Shop.Get(code);
            if (shop != null)
            {
                shop.MarketingAgentCode = null;
                shop.MarketingAgentName = null;
                db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //Shop.Edit(shop, out int error);
                IsAdded = true;
            }
            return Json(new { IsAdded = IsAdded }, JsonRequestBehavior.AllowGet);
        }

        // Json Result

        [AccessPolicy(PageCode = "SHNSHPGO009")]
        public JsonResult GenerateOTP(string MobileNo, string Code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            //int errorCode = 0;
            var model = new OtpViewModel();
            var customer = db.Customers.FirstOrDefault(i => i.PhoneNumber == MobileNo); // Customer.GetPhoneNumber(MobileNo);
            var shop = db.Shops.FirstOrDefault(i => i.Code == Code); // Shop.Get(Code);
            var models = _mapper.Map<OtpViewModel, OtpVerification>(model);
            if (customer != null)
            {
                models.CustomerCode = customer.Code;
                models.CustomerName = customer.Name;
                models.PhoneNumber = shop.PhoneNumber;
                models.ShopCode = shop.Code;
                models.Otp = _generatedCode;
                models.ReferenceCode = _referenceCode;
                models.CreatedBy = user.Name;
                models.UpdatedBy = user.Name;
                models.Verify = false;
                models.Code = _genCode("SMS");
                models.Status = 0;
                models.DateEncoded = DateTime.Now;
                models.DateUpdated = DateTime.Now;
                db.OtpVerifications.Add(models);
                db.SaveChanges();
               // OtpVerification.Add(models, out errorCode);
            }
            else
            {
                model.ErrorMessage = "Invalid Mobile Number! You are not a Customer. Please Register in APP";
            }

            return Json(new { data = models.Otp, models.Verify, model.ErrorMessage, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSHPOG010")]
        public JsonResult OTPGenerate(string MobileNo, string CustomerCode)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            //int errorCode = 0;
            var model = new OtpViewModel();
            var customer = db.Customers.FirstOrDefault(i => i.Code == CustomerCode); // Customer.Get(CustomerCode);
            var models = _mapper.Map<OtpViewModel, OtpVerification>(model);
            if (customer != null)
            {
                models.CustomerCode = customer.Code;
                models.CustomerName = customer.Name;
                models.PhoneNumber = MobileNo;
                models.Otp = _generatedCode;
                models.ReferenceCode = _referenceCode;
                models.Verify = false;
                models.CreatedBy = user.Name;
                models.UpdatedBy = user.Name;
                var dateAndTime = DateTime.Now;
                var date = dateAndTime.ToString("d");
                var time = dateAndTime.ToString("HH:mm");

                string joyra = "04448134440";
                string Msg = "Hi, " + models.Otp + " is the OTP for (Shop Now Chat) Verification at " + time + " with " + models.ReferenceCode + " reference - Joyra";
                //string Msg = "ShopNowChat[#] " + models.Otp + " is the verification OTP " + time + " time " + date + " date " + models.ReferenceCode + " reference";

                string result = SendSMS.execute(joyra, MobileNo, Msg);
                models.Code = _genCode("SMS");
                models.Status = 0;
                models.DateEncoded = DateTime.Now;
                models.DateUpdated = DateTime.Now;
                db.OtpVerifications.Add(models);
                db.SaveChanges();
              //  OtpVerification.Add(models, out errorCode);

            }
            return Json(new { data = models.Otp, models.Verify, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSHPOV011")]
        public JsonResult OTPVerify(string Otp)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var otpverification = db.OtpVerifications.FirstOrDefault(i => i.Otp == Otp);
            var data = false;
            if (otpverification != null)
            {
                otpverification.Verify = true;
                data = true;
                otpverification.DateUpdated = DateTime.Now;
                otpverification.UpdatedBy = user.Name;
                db.Entry(otpverification).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                // OtpVerification.Edit(otpverification, out int error);
            }

            return Json(new { data, otp = Otp, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSHPGP012")]
        public JsonResult GeneratePassword(string code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            SmtpClient client = new SmtpClient();
            var customer = db.Customers.FirstOrDefault(i => i.Code == code); // Customer.Get(code);
            customer.Password = _generatedPassword;
            customer.Position = 1;
            customer.DateUpdated = DateTime.Now;
            customer.UpdatedBy = user.Name;
            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
          //  Customer.Edit(customer, out int error);
            //if (shop.Password != null)
            //{
            //    MailAddress from = new MailAddress("brunojose88@gmail.com", "Bruno Jose");
            //    MailAddress to = new MailAddress(shop.OfficialEmail, shop.Name);
            //    MailMessage message = new MailMessage(from, to);
            //    message.Subject = "SHOP NOW ACCOUNT PASSWORD";
            //    message.Body = "<p>Hi " + shop.Name + ",</p><p>Your Shop Now Account Password is " + shop.Password + ".</p><br/>";

            //    MailAddress copy = new MailAddress("brunojose88@gmail.com");
            //    message.CC.Add(copy);

            //    client.Send(message);
            //}
            return Json(new { data = customer.Password, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSHPA008")]
        public JsonResult Activation(string code)
        {
            var count = db.Products.Where(i => i.ShopCode == code).Count();
            return Json(new { count, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSHPOV011")]
        public JsonResult VerifyOTP(string code, string phoneNumber)
        {
            bool Verify = false;
            var otp = "";
            var otpcode = "";
            var otpverification = db.OtpVerifications.Where(i => i.ShopCode == code && i.PhoneNumber == phoneNumber).OrderByDescending(i => i.DateEncoded).ToList();
            if (otpverification.Count != 0)
            {
                otpcode = otpverification[0].Code;
                var otpverify = (from o in db.OtpVerifications
                                 where o.Code == otpcode
                                 select o).FirstOrDefault(); //db.OtpVerifications.Where(i => i.Code == otpverification.FirstOrDefault().Code).FirstOrDefault(); // OtpVerification.Get(otpverification.FirstOrDefault().Code);
                if (otpverify != null && otpverify.Verify == true)
                {
                    Verify = true;
                    otp = otpverify.Otp;
                }
                else
                {
                    Verify = false;
                    if (otpverify == null)
                    {
                        otp = "";
                    }
                    else
                    {
                        otp = otpverify.Otp;
                    }
                }
            }

            return Json(new { Verify, otp, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSHPVI013")]
        public JsonResult VerifyImage(string code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.FirstOrDefault(i => i.Code == code); // Shop.Get(code);
            shop.Verify = true;
            shop.DateUpdated = DateTime.Now;
            shop.UpdatedBy = user.Name;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { data = shop.Verify, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSHPCC014")]
        public JsonResult CustomerCheck(string phoneNumber)
        {
            bool code = false;
            var PhoneNumber = "";
            var CustomerName = "";
            var CustomerCode = "";
            var AadharName = "";
            var AadharNumber = "";
            var Email = "";
            var customer = db.Customers.FirstOrDefault(i => i.PhoneNumber == phoneNumber); // Customer.GetPhoneNumber(customerCode);
            if (customer != null)
            {
                code = true;
                CustomerCode = customer.Code;
                CustomerName = customer.Name;
                PhoneNumber = customer.PhoneNumber;
                AadharName = customer.AadharName;
                AadharNumber = customer.AadharNumber;
                Email = customer.Email;
            }

            return Json(new { data = code, CustomerCode, CustomerName, PhoneNumber, AadharName, AadharNumber, Email, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        // Select2
        [AccessPolicy(PageCode = "SHNSHPC001")]
        public async Task<JsonResult> GetListSelect2(string q = "")
        {
            var model = await db.Shops.Where(a => a.Name.Contains(q)).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetActiveListSelect2(string q = "")
        {
            var model = await db.Shops.Where(a => a.Name.Contains(q) && a.Status ==0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSHPC001")]
        public async Task<JsonResult> GetShopCategorySelect2(string q = "")
        {
            var model = await db.ShopCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q)).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSHPC001")]
        public async Task<JsonResult> GetBrandSelect2(string q = "")
        {
            var model = await db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q)).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSHPAF015")]
        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.MarketingAgentCode == null && a.MarketingAgentName == null).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSHPAF015")]
        public async Task<JsonResult> GetMarketingAgentSelect2(string q = "")
        {
            var model = await db.MarketingAgents.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShop(string placeid)
        {
            using (WebClient myData = new WebClient())
            {
                string getDetails = myData.DownloadString("https://maps.googleapis.com/maps/api/place/details/json?place_id=" + placeid + "&key=AIzaSyCRsR3Wpkj_Vofy5FSU0otOx-6k-YFiNBk");
                var result = JsonConvert.DeserializeObject<Results>(getDetails);
                return Json(new { result, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult UpdateShopOnline(string code, bool isOnline)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.Where(i => i.Code == code && i.Status == 0).FirstOrDefault();
            shop.isOnline = isOnline;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List");
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