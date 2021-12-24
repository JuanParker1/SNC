using Amazon;
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

        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        UploadContent uc = new UploadContent();
        private const string _prefix = "REF";
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
                config.CreateMap<ShopEditViewModel, Shop>();
                config.CreateMap<Shop, ShopEditViewModel>();
                config.CreateMap<OtpViewModel, OtpVerification>();
                config.CreateMap<Shop, ShopFranchiseViewModel>();
                config.CreateMap<Bill, Bill>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SNCSL253")]
        public ActionResult List(string district = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from s in db.Shops
                        where (district != "" ? s.DistrictName == district : true) && (s.Status == 0 || s.Status == 6)
                        select s).OrderBy(s => s.Name).ToList();
            ViewBag.District = district;
            return View(List);
        }

        [AccessPolicy(PageCode = "SNCSIL254")]
        public ActionResult InactiveList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from s in db.Shops
                        select s).OrderBy(s => s.Name).Where(i => i.Status == 1).ToList();
            return View(List);
        }

        [AccessPolicy(PageCode = "SNCSC255")]
        public ActionResult Create()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var customer = db.Customers.Any(i => i.Id == user.Id);
            if(customer != false)
            {
                ViewBag.Name = user.Name;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCSC255")]
        public ActionResult Create(ShopRegisterViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            //var shopduplicateCount = db.Shops.Where(m => m.GooglePlaceId == model.GooglePlaceId ).Count();
            //var shopbeforeApprovalCount = db.Shops.Where(m => m.GooglePlaceId == model.GooglePlaceId && m.Status == 0).Count();
            //if (shopduplicateCount != 0)
            //{
            //    ViewBag.shopduplicateCount = shopduplicateCount;
            //    return View(model);
            //}
            //if (shopbeforeApprovalCount > 0)
            //{
            //    ViewBag.shopbeforeApprovalCount = shopbeforeApprovalCount;
            //    var shopownerphonenumber = db.Shops.Where(m => m.GooglePlaceId == model.GooglePlaceId && m.Status == 0).Select(i => i.OwnerPhoneNumber).ToList();
            //    var shopname = db.Shops.Where(m => m.GooglePlaceId == model.GooglePlaceId && m.Status == 0).Select(i => i.Name).ToList();
            //    ViewBag.shopownerphonenumber = shopownerphonenumber[0].ToString();
            //    ViewBag.shopname = shopname[0].ToString();
            //    return View(model);
            //}

            var customer = db.Customers.FirstOrDefault(i => i.Id == user.Id);
            var shop = _mapper.Map<ShopRegisterViewModel, Shop>(model);
            shop.Status = 1;
            shop.CreatedBy = user.Name;
            shop.UpdatedBy = user.Name;
            shop.DateEncoded = DateTime.Now;
            shop.DateUpdated = DateTime.Now;
            try
            {
                // Shop Image
                if (model.ShopImage != null)
                {
                    uc.UploadFiles(model.ShopImage.InputStream, shop.Id + "_" + model.ShopImage.FileName, accesskey, secretkey, "image");
                    shop.ImagePath = shop.Id + "_" + model.ShopImage.FileName.Replace(" ", "");
                }

                // Logo Image
                if (model.LogoImage != null)
                {
                    uc.UploadFiles(model.LogoImage.InputStream, shop.Id + "_" + model.LogoImage.FileName, accesskey, secretkey, "image");
                    shop.ImageLogoPath = shop.Id + "_" + model.LogoImage.FileName.Replace(" ", "");
                }

                // Pan Image
                if (model.PanImage != null)
                {
                    uc.UploadFiles(model.PanImage.InputStream, shop.Id + "_" + model.PanImage.FileName, accesskey, secretkey, "image");
                    shop.ImagePanPath = shop.Id + "_" + model.PanImage.FileName.Replace(" ", "");
                }

                if (model.PanPdf != null)
                {
                    uc.UploadFiles(model.PanPdf.InputStream, shop.Id + "_" + model.PanPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImagePanPath = shop.Id + "_" + model.PanPdf.FileName.Replace(" ", "");
                }

                // GSTIN Image
                if (model.GSTINImage != null)
                {
                    uc.UploadFiles(model.GSTINImage.InputStream, shop.Id + "_" + model.GSTINImage.FileName, accesskey, secretkey, "image");
                    shop.ImageGSTINPath = shop.Id + "_" + model.GSTINImage.FileName.Replace(" ", "");
                }

                if (model.GSTINPdf != null)
                {
                    uc.UploadFiles(model.GSTINPdf.InputStream, shop.Id + "_" + model.GSTINPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageGSTINPath = shop.Id + "_" + model.GSTINPdf.FileName.Replace(" ", "");
                }

                // Account Image
                if (model.AccountImage != null)
                {
                    uc.UploadFiles(model.AccountImage.InputStream, shop.Id + "_" + model.AccountImage.FileName, accesskey, secretkey, "image");
                    shop.ImageAccountPath = shop.Id + "_" + model.AccountImage.FileName.Replace(" ", "");
                }

                if (model.AccountPdf != null)
                {
                    uc.UploadFiles(model.AccountPdf.InputStream, shop.Id + "_" + model.AccountPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageAccountPath = shop.Id + "_" + model.AccountPdf.FileName.Replace(" ", "");
                }

                // FSSAI Image
                if (model.FSSAIImage != null)
                {
                    uc.UploadFiles(model.FSSAIImage.InputStream, shop.Id + "_" + model.FSSAIImage.FileName, accesskey, secretkey, "image");
                    shop.ImageFSSAIPath = shop.Id + "_" + model.FSSAIImage.FileName.Replace(" ", "");
                }

                if (model.FSSAIPdf != null)
                {
                    uc.UploadFiles(model.FSSAIPdf.InputStream, shop.Id + "_" + model.FSSAIPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageFSSAIPath = shop.Id + "_" + model.FSSAIPdf.FileName.Replace(" ", "");
                }

                // Drug Image
                if (model.DrugImage != null)
                {
                    uc.UploadFiles(model.DrugImage.InputStream, shop.Id + "_" + model.DrugImage.FileName, accesskey, secretkey, "image");
                    shop.ImageDrugPath = shop.Id + "_" + model.DrugImage.FileName.Replace(" ", "");
                }
                if (model.DrugPdf != null)
                {
                    uc.UploadFiles(model.DrugPdf.InputStream, shop.Id + "_" + model.DrugPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageDrugPath = shop.Id + "_" + model.DrugPdf.FileName.Replace(" ", "");
                }

                // Establish Image
                if (model.EstablishImage != null)
                {
                    uc.UploadFiles(model.EstablishImage.InputStream, shop.Id + "_" + model.EstablishImage.FileName, accesskey, secretkey, "image");
                    shop.ImageEstablishPath = shop.Id + "_" + model.EstablishImage.FileName.Replace(" ", "");
                }

                if (model.EstablishPdf != null)
                {
                    uc.UploadFiles(model.EstablishPdf.InputStream, shop.Id + "_" + model.EstablishPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageEstablishPath = shop.Id + "_" + model.EstablishPdf.FileName.Replace(" ", "");
                }

                // Other License Image
                if (model.OtherLicenseImage != null)
                {
                    uc.UploadFiles(model.OtherLicenseImage.InputStream, shop.Id + "_" + model.OtherLicenseImage.FileName, accesskey, secretkey, "image");
                    shop.ImageOtherLicensePath = shop.Id + "_" + model.OtherLicenseImage.FileName.Replace(" ", "");
                }
                if (model.OtherLicensePdf != null)
                {
                    uc.UploadFiles(model.OtherLicensePdf.InputStream, shop.Id + "_" + model.OtherLicensePdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageOtherLicensePath = shop.Id + "_" + model.OtherLicensePdf.FileName.Replace(" ", "");
                }

                // Authorised Distributor Image
                if (model.AuthorisedDistributorImage != null)
                {
                    uc.UploadFiles(model.AuthorisedDistributorImage.InputStream, shop.Id + "_" + model.AuthorisedDistributorImage.FileName, accesskey, secretkey, "image");
                    shop.ImageAuthoriseBrandPath = shop.Id + "_" + model.AuthorisedDistributorImage.FileName.Replace(" ", "");
                }
                if (model.AuthorisedDistributorPdf != null)
                {
                    uc.UploadFiles(model.AuthorisedDistributorPdf.InputStream, shop.Id + "_" + model.AuthorisedDistributorPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageAuthoriseBrandPath = shop.Id + "_" + model.AuthorisedDistributorPdf.FileName.Replace(" ", "");
                }

                // Aadhar Image
                if (model.AadharImage != null)
                {
                    uc.UploadFiles(model.AadharImage.InputStream, shop.Id + "_" + model.AadharImage.FileName, accesskey, secretkey, "image");
                    shop.ImageAadharPath = shop.Id + "_" + model.AadharImage.FileName.Replace(" ", "");
                }

                if (model.AadharPdf != null)
                {
                    uc.UploadFiles(model.AadharPdf.InputStream, shop.Id + "_" + model.AadharPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageAadharPath = shop.Id + "_" + model.AadharPdf.FileName.Replace(" ", "");
                }
                db.Shops.Add(shop);
                db.SaveChanges();

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

        [AccessPolicy(PageCode = "SNCSE256")]
        public ActionResult Edit(string id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dId = AdminHelpers.DCodeInt(id.Trim());
            if (dId == 0)
                return HttpNotFound();
            var shop = db.Shops.FirstOrDefault(i => i.Id == dId);
            var customer = db.Customers.FirstOrDefault(i => i.Id == shop.CustomerId);
            var shopcredit = db.ShopCredits.FirstOrDefault(i => i.CustomerId == shop.CustomerId);
            ViewBag.count = db.Products.Where(i => i.ShopId == dId).Count();
            var model = _mapper.Map<Shop, ShopEditViewModel>(shop);
            if (model.Password == null)
            {
                model.Password = customer.Password;
            }
            var otp = new OtpVerification();
            if (model.PhoneNumber != null)
            {
                otp = db.OtpVerifications.FirstOrDefault(i => i.PhoneNumber == model.PhoneNumber && i.Verify == true) ?? (OtpVerification)null;
            }
            else
            {
                otp = db.OtpVerifications.FirstOrDefault(i => i.PhoneNumber == model.OwnerPhoneNumber && i.Verify == false) ?? (OtpVerification)null;
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
            var verifyPhoneNumber = db.OtpVerifications.FirstOrDefault(i => i.PhoneNumber == model.PhoneNumber && i.Verify == true);
            if (verifyPhoneNumber != null)
            {
                model.PhoneVerify = true;
            }
            else
            {
                model.PhoneVerify = false;
            }
            if (shopcredit != null)
            {
                model.PlatformCredit = shopcredit.PlatformCredit;
                model.DeliveryCredit = shopcredit.DeliveryCredit;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCSE256")]
        public ActionResult Edit(ShopEditViewModel model)
        {
            var shop = db.Shops.FirstOrDefault(i => i.Id == model.Id);
            shop.DateUpdated = DateTime.Now;
            _mapper.Map(model, shop);

            try
            {
                // Shop Image
                if (model.ShopImage != null)
                {
                    uc.UploadFiles(model.ShopImage.InputStream, shop.Id + "_" + model.ShopImage.FileName, accesskey, secretkey, "image");
                    shop.ImagePath = shop.Id + "_" + model.ShopImage.FileName.Replace(" ", "");
                }

                // Logo Image
                if (model.LogoImage != null)
                {
                    uc.UploadFiles(model.LogoImage.InputStream, shop.Id + "_" + model.LogoImage.FileName, accesskey, secretkey, "image");
                    shop.ImageLogoPath = shop.Id + "_" + model.LogoImage.FileName.Replace(" ", "");
                }

                // Pan Image
                if (model.PanImage != null)
                {
                    uc.UploadFiles(model.PanImage.InputStream, shop.Id + "_" + model.PanImage.FileName, accesskey, secretkey, "image");
                    shop.ImagePanPath = shop.Id + "_" + model.PanImage.FileName.Replace(" ", "");
                }

                if (model.PanPdf != null)
                {
                    uc.UploadFiles(model.PanPdf.InputStream, shop.Id + "_" + model.PanPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImagePanPath = shop.Id + "_" + model.PanPdf.FileName.Replace(" ", "");
                }

                // GSTIN Image
                if (model.GSTINImage != null)
                {
                    uc.UploadFiles(model.GSTINImage.InputStream, shop.Id + "_" + model.GSTINImage.FileName, accesskey, secretkey, "image");
                    shop.ImageGSTINPath = shop.Id + "_" + model.GSTINImage.FileName.Replace(" ", "");
                }

                if (model.GSTINPdf != null)
                {
                    uc.UploadFiles(model.GSTINPdf.InputStream, shop.Id + "_" + model.GSTINPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageGSTINPath = shop.Id + "_" + model.GSTINPdf.FileName.Replace(" ", "");
                }

                // Account Image
                if (model.AccountImage != null)
                {
                    uc.UploadFiles(model.AccountImage.InputStream, shop.Id + "_" + model.AccountImage.FileName, accesskey, secretkey, "image");
                    shop.ImageAccountPath = shop.Id + "_" + model.AccountImage.FileName.Replace(" ", "");
                }

                if (model.AccountPdf != null)
                {
                    uc.UploadFiles(model.AccountPdf.InputStream, shop.Id + "_" + model.AccountPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageAccountPath = shop.Id + "_" + model.AccountPdf.FileName.Replace(" ", "");
                }

                // FSSAI Image
                if (model.FSSAIImage != null)
                {
                    uc.UploadFiles(model.FSSAIImage.InputStream, shop.Id + "_" + model.FSSAIImage.FileName, accesskey, secretkey, "image");
                    shop.ImageFSSAIPath = shop.Id + "_" + model.FSSAIImage.FileName.Replace(" ", "");
                }

                if (model.FSSAIPdf != null)
                {
                    uc.UploadFiles(model.FSSAIPdf.InputStream, shop.Id + "_" + model.FSSAIPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageFSSAIPath = shop.Id + "_" + model.FSSAIPdf.FileName.Replace(" ", "");
                }

                // Drug Image
                if (model.DrugImage != null)
                {
                    uc.UploadFiles(model.DrugImage.InputStream, shop.Id + "_" + model.DrugImage.FileName, accesskey, secretkey, "image");
                    shop.ImageDrugPath = shop.Id + "_" + model.DrugImage.FileName.Replace(" ", "");
                }
                if (model.DrugPdf != null)
                {
                    uc.UploadFiles(model.DrugPdf.InputStream, shop.Id + "_" + model.DrugPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageDrugPath = shop.Id + "_" + model.DrugPdf.FileName.Replace(" ", "");
                }

                // Establish Image
                if (model.EstablishImage != null)
                {
                    uc.UploadFiles(model.EstablishImage.InputStream, shop.Id + "_" + model.EstablishImage.FileName, accesskey, secretkey, "image");
                    shop.ImageEstablishPath = shop.Id + "_" + model.EstablishImage.FileName.Replace(" ", "");
                }

                if (model.EstablishPdf != null)
                {
                    uc.UploadFiles(model.EstablishPdf.InputStream, shop.Id + "_" + model.EstablishPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageEstablishPath = shop.Id + "_" + model.EstablishPdf.FileName.Replace(" ", "");
                }

                // Other License Image
                if (model.OtherLicenseImage != null)
                {
                    uc.UploadFiles(model.OtherLicenseImage.InputStream, shop.Id + "_" + model.OtherLicenseImage.FileName, accesskey, secretkey, "image");
                    shop.ImageOtherLicensePath = shop.Id + "_" + model.OtherLicenseImage.FileName.Replace(" ", "");
                }
                if (model.OtherLicensePdf != null)
                {
                    uc.UploadFiles(model.OtherLicensePdf.InputStream, shop.Id + "_" + model.OtherLicensePdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageOtherLicensePath = shop.Id + "_" + model.OtherLicensePdf.FileName.Replace(" ", "");
                }

                // Authorised Distributor Image
                if (model.AuthorisedDistributorImage != null)
                {
                    uc.UploadFiles(model.AuthorisedDistributorImage.InputStream, shop.Id + "_" + model.AuthorisedDistributorImage.FileName, accesskey, secretkey, "image");
                    shop.ImageAuthoriseBrandPath = shop.Id + "_" + model.AuthorisedDistributorImage.FileName.Replace(" ", "");
                }
                if (model.AuthorisedDistributorPdf != null)
                {
                    uc.UploadFiles(model.AuthorisedDistributorPdf.InputStream, shop.Id + "_" + model.AuthorisedDistributorPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageAuthoriseBrandPath = shop.Id + "_" + model.AuthorisedDistributorPdf.FileName.Replace(" ", "");
                }

                // Aadhar Image
                if (model.AadharImage != null)
                {
                    uc.UploadFiles(model.AadharImage.InputStream, shop.Id + "_" + model.AadharImage.FileName, accesskey, secretkey, "image");
                    shop.ImageAadharPath = shop.Id + "_" + model.AadharImage.FileName.Replace(" ", "");
                }

                if (model.AadharPdf != null)
                {
                    uc.UploadFiles(model.AadharPdf.InputStream, shop.Id + "_" + model.AadharPdf.FileName, accesskey, secretkey, "pdf");
                    shop.ImageAadharPath = shop.Id + "_" + model.AadharPdf.FileName.Replace(" ", "");
                }
                if (model.Count == 0)
                {
                    shop.Verify = true;
                }
                else
                {
                    shop.Verify = false;
                }

                //Delivery charge Assign
                shop.DeliveryType = model.DeliveryType;
                shop.DeliveryTierType = model.DeliveryTierType;

                db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

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

        [AccessPolicy(PageCode = "SNCSDT257")]
        public ActionResult Details(string Id)
        {
            var dId = AdminHelpers.DCodeInt(Id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            Shop sh = db.Shops.FirstOrDefault(i => i.Id == dId);
            var model = new ShopEditViewModel();
            _mapper.Map(sh, model);
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCSD258")]
        public JsonResult Delete(string id)
        {
            var dId = AdminHelpers.DCodeInt(id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.FirstOrDefault(i => i.Id == dId);
            if (shop != null)
            {
                shop.Status = 2;
                shop.DateUpdated = DateTime.Now;
                shop.UpdatedBy = user.Name;
                db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SNCSCL259")]
        public ActionResult CreditList()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new ShopCreditViewModel();
            model.ListItems = db.ShopCredits.Where(i => i.PlatformCredit <= 100 && i.DeliveryCredit <= 100) //Low credits
                .Join(db.Shops.Where(i => i.IsTrail == false), sc => sc.CustomerId, s => s.CustomerId, (sc, s) => new { sc, s })
                .Select(i => new ShopCreditViewModel.ListItem
                {
                    DeliveryCredit = i.sc.DeliveryCredit,
                    Id = i.sc.Id,
                    PlatformCredit = i.sc.PlatformCredit,
                    ShopName = i.s.Name,
                    ShopOwnerName = i.s.CustomerName,
                    ShopOwnerPhoneNumber = i.s.OwnerPhoneNumber,
                    DeliveryCreditCssColor = i.sc.DeliveryCredit <= 150 ? "text-danger" : (i.sc.DeliveryCredit <= 250 && i.sc.DeliveryCredit > 150) ? "text-warning" : "text-success",
                    PlatformCreditCssColor = i.sc.PlatformCredit <= 100 ? "text-danger" : (i.sc.PlatformCredit <= 200 && i.sc.PlatformCredit > 100) ? "text-warning" : "text-success"
                }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCSCH260")]
        public ActionResult CreditsHistory()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new ShopCreditViewModel();
            model.Lists = db.Payments.Where(i => i.CreditType == 0 || i.CreditType == 1 && i.Amount != -20)
                 .Join(db.Shops, p => p.ShopId, s => s.Id, (p, s) => new { p, s })
                .Select(i => new ShopCreditViewModel.List
                {
                    Id = i.p.Id,
                    ShopName = i.s.Name,
                    ShopOwnerName = i.s.CustomerName != null ? i.s.CustomerName : "N/A",
                    ShopOwnerPhoneNumber = i.s.OwnerPhoneNumber,
                    Amount = i.p.Amount,
                    CreditType = i.p.CreditType
                }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCSIA261")]
        public ActionResult InActive(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.FirstOrDefault(i => i.Id == Id);
            shop.Status = 1;
            shop.DateUpdated = DateTime.Now;
            shop.UpdatedBy = user.Name;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List", "Shop");
        }

        [AccessPolicy(PageCode = "SNCSILB262")]
        public ActionResult InActiveLowBalance(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.FirstOrDefault(i => i.Id == Id);
            shop.Status = 6;
            shop.DateUpdated = DateTime.Now;
            shop.UpdatedBy = user.Name;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List", "Shop");
        }

        [AccessPolicy(PageCode = "SNCSA263")]
        public ActionResult Active(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.FirstOrDefault(i => i.Id == id);
            shop.Status = 0;
            shop.DateUpdated = DateTime.Now;
            shop.UpdatedBy = user.Name;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List", "Shop");
        }

        [AccessPolicy(PageCode = "SNCSA263")]
        public ActionResult Activate(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.FirstOrDefault(i => i.Id == id);
            shop.Status = 0;
            shop.DateUpdated = DateTime.Now;
            shop.UpdatedBy = user.Name;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List", "Shop");
        }

        public ActionResult UpdateShopOnline(int Id, bool isOnline)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.Where(i => i.Id == Id && i.Status == 0).FirstOrDefault();
            shop.IsOnline = isOnline;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List");
        }

        public ActionResult UpdateShopTrail(int Id, bool isTrail)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.Where(i => i.Id == Id).FirstOrDefault();
            shop.IsTrail = isTrail;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List");
        }

        // Json Result
        public JsonResult GenerateOTP(string MobileNo, int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var model = new OtpViewModel();
            var customer = db.Customers.FirstOrDefault(i => i.PhoneNumber == MobileNo);
            var shop = db.Shops.FirstOrDefault(i => i.Id == id);
            var models = _mapper.Map<OtpViewModel, OtpVerification>(model);
            if (customer != null)
            {
                models.CustomerId = customer.Id;
                models.CustomerName = customer.Name;
                models.PhoneNumber = shop.PhoneNumber;
                models.ShopId = shop.Id;
                models.Otp = _generatedCode;
                models.ReferenceCode = _referenceCode;
                models.CreatedBy = user.Name;
                models.UpdatedBy = user.Name;
                models.Verify = false;
                models.Status = 0;
                models.DateEncoded = DateTime.Now;
                models.DateUpdated = DateTime.Now;
                db.OtpVerifications.Add(models);
                db.SaveChanges();
            }
            else
            {
                model.ErrorMessage = "Invalid Mobile Number! You are not a Customer. Please Register in APP";
            }

            return Json(new { data = models.Otp, models.Verify, model.ErrorMessage, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult OTPGenerate(string MobileNo, int Customerid)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var model = new OtpViewModel();
            var customer = db.Customers.FirstOrDefault(i => i.Id == Customerid);
            var models = _mapper.Map<OtpViewModel, OtpVerification>(model);
            if (customer != null)
            {
                models.CustomerId = customer.Id;
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

                string result = SendSMS.execute(joyra, MobileNo, Msg);
                models.Status = 0;
                models.DateEncoded = DateTime.Now;
                models.DateUpdated = DateTime.Now;
                db.OtpVerifications.Add(models);
                db.SaveChanges();

            }
            return Json(new { data = models.Otp, models.Verify, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

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
            }

            return Json(new { data, otp = Otp, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GeneratePassword(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            SmtpClient client = new SmtpClient();
            var customer = db.Customers.FirstOrDefault(i => i.Id == id);
            customer.Password = _generatedPassword;
            customer.Position = 1;
            customer.DateUpdated = DateTime.Now;
            customer.UpdatedBy = user.Name;
            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { data = customer.Password, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Activation(int id)
        {
            var count = db.Products.Where(i => i.ShopId == id).Count();
            return Json(new { count, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult VerifyOTP(int Id, string phoneNumber)
        {
            bool Verify = false;
            var otp = "";
            int otpcode;
            var otpverification = db.OtpVerifications.Where(i => i.ShopId == Id && i.PhoneNumber == phoneNumber).OrderByDescending(i => i.DateEncoded).ToList();
            if (otpverification.Count != 0)
            {
                otpcode = otpverification[0].Id;
                var otpverify = (from o in db.OtpVerifications
                                 where o.Id == otpcode
                                 select o).FirstOrDefault();
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

        public JsonResult VerifyImage(int code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shop = db.Shops.FirstOrDefault(i => i.Id == code);
            shop.Verify = true;
            shop.DateUpdated = DateTime.Now;
            shop.UpdatedBy = user.Name;
            db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { data = shop.Verify, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CustomerCheck(string phoneNumber)
        {
            bool code = false;
            var PhoneNumber = "";
            var CustomerName = "";
            var AadharName = "";
            var AadharNumber = "";
            var Email = "";
            var AadharImage = "";
            int CustomerId = 0;
            var customer = db.Customers.FirstOrDefault(i => i.PhoneNumber == phoneNumber);
            var isDeliveryBoyExist = db.Customers.Any(i => i.PhoneNumber == phoneNumber && i.Position == 3);
            if (isDeliveryBoyExist)
            {
                return Json(new { ErrorMsg = isDeliveryBoyExist, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
            }
            else if (customer != null)
            {
                code = true;
                CustomerId = customer.Id;
                CustomerName = customer.Name;
                PhoneNumber = customer.PhoneNumber;
                AadharName = customer.AadharName;
                AadharNumber = customer.AadharNumber;
                Email = customer.Email;
                AadharImage = customer.ImagePath;
            }

            return Json(new { data = code, CustomerId, CustomerName, PhoneNumber, AadharName, AadharNumber, Email, AadharImage, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
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

        // Select2
        public async Task<JsonResult> GetListSelect2(string q = "")
        {
            var model = await db.Shops.Where(a => a.Name.Contains(q)).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetActiveListSelect2(string q = "")
        {
            var model = await db.Shops.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetShopCategorySelect2(string q = "")
        {
            var model = await db.ShopCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q)).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetBrandSelect2(string q = "")
        {
            var model = await db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q)).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.AgencyName == null).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDistrictSelect2(string q = "")
        {
            var model = await db.Shops
                .Where(a => a.DistrictName.Contains(q) && a.Status == 0)
                .GroupBy(i => i.DistrictName)
                .Select(i => new
                {
                    id = i.Key,
                    text = i.Key
                }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetShopByDistrictSelect2(string district, string q = "")
        {
            var model = await db.Shops.Where(a => a.Name.Contains(q) && a.Status == 0 && a.DistrictName == district).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

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