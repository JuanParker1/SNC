using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using AutoMapper;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class MarketingAgentController : Controller
    {
        private ShopnowchatEntities _db = new ShopnowchatEntities();
        UploadContent uc = new UploadContent();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private const string filePath = null;
        private static readonly string bucketName = ConfigurationManager.AppSettings["BucketName"];
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.APSouth1;
        private static readonly string accesskey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static readonly string secretkey = ConfigurationManager.AppSettings["AWSSecretKey"];

        public MarketingAgentController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {                
                config.CreateMap<MarketingAgentRegisterViewModel, MarketingAgent>();
                config.CreateMap<MarketingAgent, MarketingAgentUpdationViewModel>();
                config.CreateMap<MarketingAgent, MarketingAgentListViewModel>();
                config.CreateMap<Shop, ShopListViewModel>();
            });

            _mapper = _mapperConfiguration.CreateMapper();
        }

        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(MarketingAgentRegisterViewModel model)
        {

            var marketingAgent = _mapper.Map<MarketingAgentRegisterViewModel, MarketingAgent>(model);
            marketingAgent.Code = ShopNow.Helpers.DRC.Generate("MAG");
            marketingAgent.Status = 0;
            marketingAgent.DateEncoded = DateTime.Now;
            marketingAgent.DateUpdated = DateTime.Now;
            _db.MarketingAgents.Add(marketingAgent);
            _db.SaveChanges();
          //  marketingAgent.Code = marketingAgent.Code;// MarketingAgent.Add(marketingAgent, out int error);
            //if (model.PanImage != null)
            //{
            //    uc.UploadImage(model.PanImage, marketingAgent.Code + "_", "/Content/PanImage/", Server, _db, "", marketingAgent.Code, "");
            //    var marketingAgentImage = MarketingAgent.Get(marketingAgent.Code);
            //    marketingAgentImage.ImagePanPath = marketingAgent.Code + "_" + model.PanImage.FileName;
            //    MarketingAgent.Edit(marketingAgentImage, out error);
            //}
            return RedirectToAction("Login", "MarketingAgent");
        }

        public ActionResult Updation()
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            ViewBag.Name = user.Name;
            var marketingAgent = _db.MarketingAgents.FirstOrDefault(i => i.Code == user.Code);//MarketingAgent.Get(user.Code);
            var model = _mapper.Map<MarketingAgent, MarketingAgentUpdationViewModel>(marketingAgent);
            return View(model);
        }

        [HttpPost]
        public ActionResult Updation(MarketingAgentUpdationViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            ViewBag.Name = user.Name;
            var marketingAgent = _db.MarketingAgents.FirstOrDefault(i => i.Code == model.Code);// MarketingAgent.Get(model.Code);
            //var ma = _mapper.Map(model, marketingAgent);
            marketingAgent.Name = model.Name;
            marketingAgent.PhoneNumber = model.PhoneNumber;
            marketingAgent.Email = model.Email;
            marketingAgent.PanNumber = model.PanNumber;
            marketingAgent.ImagePanPath = model.ImagePanPath;
            marketingAgent.DateUpdated = DateTime.Now;
            _db.Entry(marketingAgent).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();

            //MarketingAgent.Edit(marketingAgent, out int error);
            try
            {
                if (model.PanImage != null)
                {
                    uc.UploadImage(model.PanImage, marketingAgent.Code + "_", "/Content/ImageUpload/", Server, _db, "", marketingAgent.Code, "");
                    var s3Client = new AmazonS3Client(accesskey, secretkey, bucketRegion);
                    var fileTransferUtility = new TransferUtility(s3Client);

                    if (model.PanImage.ContentLength > 0)
                    {
                        var filePath = Path.Combine(Server.MapPath("/Content/ImageUpload/Original/"),
                        Path.GetFileName(marketingAgent.Code + "_" + model.PanImage.FileName));
                        var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                        {
                            BucketName = bucketName,
                            FilePath = filePath.ToString(),
                            StorageClass = S3StorageClass.StandardInfrequentAccess,
                            PartSize = 6291456, // 6 MB.
                            Key = marketingAgent.Code + "_" + model.PanImage.FileName,
                            ContentType = model.PanImage.ContentType,
                            CannedACL = S3CannedACL.PublicRead
                        };
                        fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                        fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
                        fileTransferUtility.Upload(fileTransferUtilityRequest);
                        fileTransferUtility.Dispose();
                    }
                    var PanImg = _db.MarketingAgents.FirstOrDefault(i => i.Code == model.Code);// MarketingAgent.Get(model.Code);
                    PanImg.ImagePanPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/" + PanImg.Code + "_" + model.PanImage.FileName;
                    PanImg.DateUpdated = DateTime.Now;
                    _db.Entry(PanImg).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                    //MarketingAgent.Edit(PanImg, out int error1);
                }
                return View(model);
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

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            Session.Clear();
            if (CheckAlreadyLoggedIn())
                return Redirect(returnUrl != null ? returnUrl : "/MarketingAgent/Login");
            else
            {
                MarketingAgentLoginViewModel loginModel = new MarketingAgentLoginViewModel();
                loginModel.PhoneNumber = Request.Cookies["PhoneNumber"] != null ? Request.Cookies["PhoneNumber"].Value : "";
                loginModel.Password = Request.Cookies["Password"] != null ? Request.Cookies["Password"].Value : "";
                ViewBag.ReturnUrl = returnUrl;
                return View(loginModel);
            }
        }

        private bool CheckAlreadyLoggedIn()
        {
            bool alreadyLoggedIn = false;
            if (Session["PhoneNumber"] != null)
            {
                string userId = Convert.ToString(Session["PhoneNumber"]);
                var user = _db.MarketingAgents.FirstOrDefault(i => i.PhoneNumber == userId && i.Status == 0);
                if (user != null)
                {
                    Response.Cookies["Name"].Value = user.Name;
                    alreadyLoggedIn = true;
                }
            }
            return alreadyLoggedIn;
        }


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Login(MarketingAgentLoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = _db.MarketingAgents.FirstOrDefault(i => i.PhoneNumber == model.PhoneNumber && i.Password == model.Password && i.Status == 0);
                if (user != null)
                {
                    Helpers.Sessions.SetMarketingUser(model.PhoneNumber);
                    Session["PhoneNumber"] = user.PhoneNumber;
                    Response.Cookies["Name"].Value = user.Name;
                    if (Request["RememberMe"] == "on")
                    {
                        Response.Cookies["PhoneNumber"].Value = user.PhoneNumber;
                        Response.Cookies["Password"].Value = user.Password;
                    }
                    else
                    {
                        Response.Cookies["PhoneNumber"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["Password"].Expires = DateTime.Now.AddDays(-1);
                    }

                    return RedirectToAction(returnUrl != null ? returnUrl : "ShopList", "MarketingAgent");
                }

                else
                {
                    ModelState.AddModelError("Password", "Invalid PhoneNumber or password");
                }
            }
            return View(model);
        }

        public ActionResult Dashboard()
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            ViewBag.Name = user.Name;
            return View();
        }


        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ChangePassword(ChangePasswordViewModel cpm)
        {
            string LoginMemberId = Convert.ToString(Session["UserCode"]);
            var ExistingDetails = _db.MarketingAgents.FirstOrDefault(i => i.Code == LoginMemberId && i.Status == 0);
            if (cpm.OldPassword == ExistingDetails.Password)
            {
                ExistingDetails.Password = cpm.NewPassword;
                _db.SaveChanges();
                ViewBag.PasswordChangeMsg = "Password changed successfully";
            }
            else
                ModelState.AddModelError("OldPassword", "Current password is wrong");
            return View();
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Login", "MarketingAgent");
        }

        public ActionResult List()
        {
            var model = new MarketingAgentListViewModel();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.List = _db.MarketingAgents.Where(i => i.Status == 0).Select(i => new MarketingAgentListViewModel.MarketingAgentList
            {
                Code = i.Code,
                Email = i.Email,
                Name = i.Name,
                PhoneNumber = i.PhoneNumber
            }).OrderBy(i=>i.Name).ToList();

            return View(model.List);
        }

        #region Shop
        public ActionResult ShopList()
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            ViewBag.Name = user.Name;
            var model = new ShopListViewModel();
            model.List = _db.Shops.Where(i => i.Status == 0 && i.MarketingAgentCode == user.Code).Select(i => new ShopListViewModel.ShopList
            {
                Code = i.Code,
                Name = i.Name,
                PhoneNumber = i.PhoneNumber,
                OwnerPhoneNumber = i.OwnerPhoneNumber,
                ManualPhoneNumber = i.ManualPhoneNumber,
                ShopCategoryName = i.ShopCategoryName,
                Address = i.Address,
                DistrictName = i.DistrictName,
                StateName = i.StateName,
                PinCode = i.PinCode
            }).OrderBy(i => i.Name).ToList();

            return View(model);
        }

        #endregion

        #region Delivery Boy
        public ActionResult DeliveryBoyList()
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            ViewBag.Name = user.Name;
            var model = new DeliveryBoyListViewModel();
            model.List = _db.DeliveryBoys.Where(i => i.Status == 0 && i.MarketingAgentCode == user.Code).Select(i => new DeliveryBoyListViewModel.DeliveryBoyList
            {
                Code = i.Code,
                Name = i.Name,
                PhoneNumber = i.PhoneNumber,
                ShopCode = i.ShopCode,
                ShopName = i.ShopName,
                ImagePath = i.ImagePath
            }).OrderBy(i => i.Name).ToList();

            return View(model);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult DeliveryBoyCreate()
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "")]
        public ActionResult DeliveryBoyCreate(DeliveryBoyCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);

            var deliveryboy = _mapper.Map<DeliveryBoyCreateEditViewModel, DeliveryBoy>(model);
            if (model.Name == null && model.StaffCode != null)
            {
                deliveryboy.Name = model.StaffName;
            }
            deliveryboy.CreatedBy = user.Name;
            deliveryboy.UpdatedBy = user.Name;
            deliveryboy.CustomerCode = user.Code;
            deliveryboy.CustomerName = user.Name;
            deliveryboy.Code = ShopNow.Helpers.DRC.Generate("DBY");
            deliveryboy.DateEncoded = DateTime.Now;
            deliveryboy.DateUpdated = DateTime.Now;
            model.Status = 1;
            _db.DeliveryBoys.Add(deliveryboy);
            _db.SaveChanges();
           // deliveryboy.Code = DeliveryBoy.Add(deliveryboy, out int error);
            try
            {
                // DeliveryBoy Image
                if (model.DeliveryBoyImage != null)
                {
                    uc.UploadImage(model.DeliveryBoyImage, deliveryboy.Code + "_", "/Content/ImageUpload/", Server, _db, "", deliveryboy.Code, "");
                    var s3Client = new AmazonS3Client(accesskey, secretkey, bucketRegion);
                    var fileTransferUtility = new TransferUtility(s3Client);

                    if (model.DeliveryBoyImage.ContentLength > 0)
                    {
                        var filePath = Path.Combine(Server.MapPath("/Content/ImageUpload/Original/"),
                        Path.GetFileName(deliveryboy.Code + "_" + model.DeliveryBoyImage.FileName));
                        var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                        {
                            BucketName = bucketName,
                            FilePath = filePath.ToString(),
                            StorageClass = S3StorageClass.StandardInfrequentAccess,
                            PartSize = 6291456, // 6 MB.
                            Key = deliveryboy.Code + "_" + model.DeliveryBoyImage.FileName,
                            ContentType = model.DeliveryBoyImage.ContentType,
                            CannedACL = S3CannedACL.PublicRead
                        };
                        fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                        fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
                        fileTransferUtility.Upload(fileTransferUtilityRequest);
                        fileTransferUtility.Dispose();
                    }
                    var deliveryBoyImage = _db.DeliveryBoys.FirstOrDefault(i => i.Code == deliveryboy.Code);//DeliveryBoy.Get(deliveryboy.Code);
                    deliveryBoyImage.ImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/" + deliveryboy.Code + "_" + model.DeliveryBoyImage.FileName;
                    deliveryBoyImage.DateUpdated = DateTime.Now;
                    _db.Entry(deliveryBoyImage).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    //  DeliveryBoy.Edit(deliveryBoyImage, out int errorCode);
                }

                // DrivingLicense Image
                if (model.DrivingLicenseImage != null)
                {
                    uc.UploadImage(model.DrivingLicenseImage, deliveryboy.Code + "_", "/Content/ImageUpload/", Server, _db, "", deliveryboy.Code, "");
                    var s3Client = new AmazonS3Client(accesskey, secretkey, bucketRegion);
                    var fileTransferUtility = new TransferUtility(s3Client);

                    if (model.DrivingLicenseImage.ContentLength > 0)
                    {
                        var filePath = Path.Combine(Server.MapPath("/Content/ImageUpload/Original/"),
                        Path.GetFileName(deliveryboy.Code + "_" + model.DrivingLicenseImage.FileName));
                        var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                        {
                            BucketName = bucketName,
                            FilePath = filePath.ToString(),
                            StorageClass = S3StorageClass.StandardInfrequentAccess,
                            PartSize = 6291456, // 6 MB.
                            Key = deliveryboy.Code + "_" + model.DrivingLicenseImage.FileName,
                            ContentType = model.DrivingLicenseImage.ContentType,
                            CannedACL = S3CannedACL.PublicRead
                        };
                        fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                        fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
                        fileTransferUtility.Upload(fileTransferUtilityRequest);
                        fileTransferUtility.Dispose();
                    }
                    var drivingLicenseImage = _db.DeliveryBoys.FirstOrDefault(i => i.Code == deliveryboy.Code); //DeliveryBoy.Get(deliveryboy.Code);
                    drivingLicenseImage.DrivingLicenseImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/" + deliveryboy.Code + "_" + model.DrivingLicenseImage.FileName;
                    drivingLicenseImage.DateUpdated = DateTime.Now;
                    _db.Entry(drivingLicenseImage).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    //DeliveryBoy.Edit(drivingLicenseImage, out int errorCode);
                }

                // DrivingLicense Pdf
                if (model.DrivingLicensePdf != null)
                {
                    if (model.DrivingLicensePdf.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(model.DrivingLicensePdf.FileName);
                        string _path = Path.Combine(Server.MapPath("/Content/PdfUpload"), deliveryboy.Code + "_" + _FileName);
                        model.DrivingLicensePdf.SaveAs(_path);
                        var s3Client1 = new AmazonS3Client(accesskey, secretkey, bucketRegion);
                        var fileTransferUtility1 = new TransferUtility(s3Client1);
                        if (model.DrivingLicensePdf.ContentLength > 0)
                        {
                            var filePath = Path.Combine(Server.MapPath("/Content/PdfUpload"),
                            Path.GetFileName(deliveryboy.Code + "_" + model.DrivingLicensePdf.FileName));
                            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                            {
                                BucketName = bucketName,
                                FilePath = filePath.ToString(),
                                StorageClass = S3StorageClass.StandardInfrequentAccess,
                                PartSize = 6291456, // 6 MB.
                                Key = deliveryboy.Code + "_" + model.DrivingLicensePdf.FileName,
                                ContentType = model.DrivingLicensePdf.ContentType,
                                CannedACL = S3CannedACL.PublicRead
                            };
                            fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                            fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
                            fileTransferUtility1.Upload(fileTransferUtilityRequest);
                            fileTransferUtility1.Dispose();
                        }
                        var drivingLicensePdf = _db.DeliveryBoys.FirstOrDefault(i => i.Code == deliveryboy.Code);// DeliveryBoy.Get(deliveryboy.Code);
                        drivingLicensePdf.DrivingLicenseImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/" + deliveryboy.Code + "_" + model.DrivingLicensePdf.FileName;
                        drivingLicensePdf.DateUpdated = DateTime.Now;
                        _db.Entry(drivingLicensePdf).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                        //DeliveryBoy.Edit(drivingLicensePdf, out int errors);
                    }
                }

                // BankPassbook Image
                if (model.BankPassbookImage != null)
                {
                    uc.UploadImage(model.BankPassbookImage, deliveryboy.Code + "_", "/Content/ImageUpload/", Server, _db, "", deliveryboy.Code, "");
                    var s3Client = new AmazonS3Client(accesskey, secretkey, bucketRegion);
                    var fileTransferUtility = new TransferUtility(s3Client);

                    if (model.BankPassbookImage.ContentLength > 0)
                    {
                        var filePath = Path.Combine(Server.MapPath("/Content/ImageUpload/Original/"),
                        Path.GetFileName(deliveryboy.Code + "_" + model.BankPassbookImage.FileName));
                        var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                        {
                            BucketName = bucketName,
                            FilePath = filePath.ToString(),
                            StorageClass = S3StorageClass.StandardInfrequentAccess,
                            PartSize = 6291456, // 6 MB.
                            Key = deliveryboy.Code + "_" + model.BankPassbookImage.FileName,
                            ContentType = model.BankPassbookImage.ContentType,
                            CannedACL = S3CannedACL.PublicRead
                        };
                        fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                        fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
                        fileTransferUtility.Upload(fileTransferUtilityRequest);
                        fileTransferUtility.Dispose();
                    }
                    var bankPassbookImage = _db.DeliveryBoys.FirstOrDefault(i => i.Code == deliveryboy.Code);// DeliveryBoy.Get(deliveryboy.Code);
                    bankPassbookImage.BankPassbookPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/" + deliveryboy.Code + "_" + model.BankPassbookImage.FileName;
                    bankPassbookImage.DateUpdated = DateTime.Now;
                    _db.Entry(bankPassbookImage).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    //DeliveryBoy.Edit(bankPassbookImage, out int errorCode);
                }

                // BankPassbook Pdf
                if (model.BankPassbookPdf != null)
                {
                    if (model.BankPassbookPdf.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(model.BankPassbookPdf.FileName);
                        string _path = Path.Combine(Server.MapPath("/Content/PdfUpload"), deliveryboy.Code + "_" + _FileName);
                        model.BankPassbookPdf.SaveAs(_path);
                        var s3Client1 = new AmazonS3Client(accesskey, secretkey, bucketRegion);
                        var fileTransferUtility1 = new TransferUtility(s3Client1);
                        if (model.BankPassbookPdf.ContentLength > 0)
                        {
                            var filePath = Path.Combine(Server.MapPath("/Content/PdfUpload"),
                            Path.GetFileName(deliveryboy.Code + "_" + model.BankPassbookPdf.FileName));
                            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                            {
                                BucketName = bucketName,
                                FilePath = filePath.ToString(),
                                StorageClass = S3StorageClass.StandardInfrequentAccess,
                                PartSize = 6291456, // 6 MB.
                                Key = deliveryboy.Code + "_" + model.BankPassbookPdf.FileName,
                                ContentType = model.BankPassbookPdf.ContentType,
                                CannedACL = S3CannedACL.PublicRead
                            };
                            fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                            fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
                            fileTransferUtility1.Upload(fileTransferUtilityRequest);
                            fileTransferUtility1.Dispose();
                        }
                        var bankPassbookPdf = _db.DeliveryBoys.FirstOrDefault(i => i.Code == deliveryboy.Code);// DeliveryBoy.Get(deliveryboy.Code);
                        bankPassbookPdf.BankPassbookPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/" + deliveryboy.Code + "_" + model.BankPassbookPdf.FileName;
                        bankPassbookPdf.DateUpdated = DateTime.Now;
                        _db.Entry(bankPassbookPdf).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                        //  DeliveryBoy.Edit(bankPassbookPdf, out int errors);
                    }
                }

                // CV Pdf
                if (model.CVPdf != null)
                {
                    if (model.CVPdf.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(model.CVPdf.FileName);
                        string _path = Path.Combine(Server.MapPath("/Content/PdfUpload"), deliveryboy.Code + "_" + _FileName);
                        model.CVPdf.SaveAs(_path);
                        var s3Client1 = new AmazonS3Client(accesskey, secretkey, bucketRegion);
                        var fileTransferUtility1 = new TransferUtility(s3Client1);
                        if (model.CVPdf.ContentLength > 0)
                        {
                            var filePath = Path.Combine(Server.MapPath("/Content/PdfUpload"),
                            Path.GetFileName(deliveryboy.Code + "_" + model.CVPdf.FileName));
                            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                            {
                                BucketName = bucketName,
                                FilePath = filePath.ToString(),
                                StorageClass = S3StorageClass.StandardInfrequentAccess,
                                PartSize = 6291456, // 6 MB.
                                Key = deliveryboy.Code + "_" + model.CVPdf.FileName,
                                ContentType = model.CVPdf.ContentType,
                                CannedACL = S3CannedACL.PublicRead
                            };
                            fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                            fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
                            fileTransferUtility1.Upload(fileTransferUtilityRequest);
                            fileTransferUtility1.Dispose();
                        }
                        var cVPdf = _db.DeliveryBoys.FirstOrDefault(i => i.Code == deliveryboy.Code);// DeliveryBoy.Get(deliveryboy.Code);
                        cVPdf.CVPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/" + deliveryboy.Code + "_" + model.CVPdf.FileName;
                        cVPdf.DateUpdated = DateTime.Now;
                        _db.Entry(cVPdf).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                        //   DeliveryBoy.Edit(cVPdf, out int errors);
                    }
                }

                return RedirectToAction("List");
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    return ViewBag.Message = "Check the provided AWS Credentials.";
                }
                else
                {
                    return ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                }
            }

        }

        [AccessPolicy(PageCode = "")]
        public ActionResult DeliveryBoyEdit(string code)
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(code))
                return HttpNotFound();
            var deliveryBoy = _db.DeliveryBoys.FirstOrDefault(i => i.Code == code);// DeliveryBoy.Get(code);
            var model = _mapper.Map<DeliveryBoy, DeliveryBoyCreateEditViewModel>(deliveryBoy);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "")]
        public ActionResult DeliveryBoyEdit(DeliveryBoyCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);

            DeliveryBoy deliveryboy = _db.DeliveryBoys.FirstOrDefault(i => i.Code == model.Code);// DeliveryBoy.Get(model.Code);
            _mapper.Map(model, deliveryboy);

            if (model.Name == null && model.StaffCode != null)
            {
                deliveryboy.Name = model.StaffName;
            }

            deliveryboy.DateUpdated = DateTime.Now;
            deliveryboy.UpdatedBy = user.Name;
            deliveryboy.DateUpdated = DateTime.Now;
            _db.Entry(deliveryboy).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            // DeliveryBoy.Edit(deliveryboy, out int errorCode);

            try
            {
                // DeliveryBoy Image
                if (model.DeliveryBoyImage != null)
                {
                    uc.UploadImage(model.DeliveryBoyImage, deliveryboy.Code + "_", "/Content/ImageUpload/", Server, _db, "", deliveryboy.Code, "");
                    var s3Client = new AmazonS3Client(accesskey, secretkey, bucketRegion);
                    var fileTransferUtility = new TransferUtility(s3Client);

                    if (model.DeliveryBoyImage.ContentLength > 0)
                    {
                        var filePath = Path.Combine(Server.MapPath("/Content/ImageUpload/Original/"),
                        Path.GetFileName(deliveryboy.Code + "_" + model.DeliveryBoyImage.FileName));
                        var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                        {
                            BucketName = bucketName,
                            FilePath = filePath.ToString(),
                            StorageClass = S3StorageClass.StandardInfrequentAccess,
                            PartSize = 6291456, // 6 MB.
                            Key = deliveryboy.Code + "_" + model.DeliveryBoyImage.FileName,
                            ContentType = model.DeliveryBoyImage.ContentType,
                            CannedACL = S3CannedACL.PublicRead
                        };
                        fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                        fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
                        fileTransferUtility.Upload(fileTransferUtilityRequest);
                        fileTransferUtility.Dispose();
                    }
                    var deliveryBoyImage = _db.DeliveryBoys.FirstOrDefault(i => i.Code == deliveryboy.Code);// DeliveryBoy.Get(deliveryboy.Code);
                    deliveryBoyImage.ImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/" + deliveryboy.Code + "_" + model.DeliveryBoyImage.FileName;
                    deliveryBoyImage.DateUpdated = DateTime.Now;
                    _db.Entry(deliveryBoyImage).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    //DeliveryBoy.Edit(deliveryBoyImage, out int errorCode1);
                }

                // DrivingLicense Image
                if (model.DrivingLicenseImage != null)
                {
                    uc.UploadImage(model.DrivingLicenseImage, deliveryboy.Code + "_", "/Content/ImageUpload/", Server, _db, "", deliveryboy.Code, "");
                    var s3Client = new AmazonS3Client(accesskey, secretkey, bucketRegion);
                    var fileTransferUtility = new TransferUtility(s3Client);

                    if (model.DrivingLicenseImage.ContentLength > 0)
                    {
                        var filePath = Path.Combine(Server.MapPath("/Content/ImageUpload/Original/"),
                        Path.GetFileName(deliveryboy.Code + "_" + model.DrivingLicenseImage.FileName));
                        var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                        {
                            BucketName = bucketName,
                            FilePath = filePath.ToString(),
                            StorageClass = S3StorageClass.StandardInfrequentAccess,
                            PartSize = 6291456, // 6 MB.
                            Key = deliveryboy.Code + "_" + model.DrivingLicenseImage.FileName,
                            ContentType = model.DrivingLicenseImage.ContentType,
                            CannedACL = S3CannedACL.PublicRead
                        };
                        fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                        fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
                        fileTransferUtility.Upload(fileTransferUtilityRequest);
                        fileTransferUtility.Dispose();
                    }
                    var drivingLicenseImage = _db.DeliveryBoys.FirstOrDefault(i => i.Code == deliveryboy.Code);// DeliveryBoy.Get(deliveryboy.Code);
                    drivingLicenseImage.DrivingLicenseImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/" + deliveryboy.Code + "_" + model.DrivingLicenseImage.FileName;
                    drivingLicenseImage.DateUpdated = DateTime.Now;
                    _db.Entry(drivingLicenseImage).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    // DeliveryBoy.Edit(drivingLicenseImage, out int errorCode1);
                }

                // DrivingLicense Pdf
                if (model.DrivingLicensePdf != null)
                {
                    if (model.DrivingLicensePdf.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(model.DrivingLicensePdf.FileName);
                        string _path = Path.Combine(Server.MapPath("/Content/PdfUpload"), deliveryboy.Code + "_" + _FileName);
                        model.DrivingLicensePdf.SaveAs(_path);
                        var s3Client1 = new AmazonS3Client(accesskey, secretkey, bucketRegion);
                        var fileTransferUtility1 = new TransferUtility(s3Client1);
                        if (model.DrivingLicensePdf.ContentLength > 0)
                        {
                            var filePath = Path.Combine(Server.MapPath("/Content/PdfUpload"),
                            Path.GetFileName(deliveryboy.Code + "_" + model.DrivingLicensePdf.FileName));
                            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                            {
                                BucketName = bucketName,
                                FilePath = filePath.ToString(),
                                StorageClass = S3StorageClass.StandardInfrequentAccess,
                                PartSize = 6291456, // 6 MB.
                                Key = deliveryboy.Code + "_" + model.DrivingLicensePdf.FileName,
                                ContentType = model.DrivingLicensePdf.ContentType,
                                CannedACL = S3CannedACL.PublicRead
                            };
                            fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                            fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
                            fileTransferUtility1.Upload(fileTransferUtilityRequest);
                            fileTransferUtility1.Dispose();
                        }
                        var drivingLicensePdf = _db.DeliveryBoys.FirstOrDefault(i => i.Code == deliveryboy.Code);// DeliveryBoy.Get(deliveryboy.Code);
                        drivingLicensePdf.DrivingLicenseImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/" + deliveryboy.Code + "_" + model.DrivingLicensePdf.FileName;
                        drivingLicensePdf.DateUpdated = DateTime.Now;
                        _db.Entry(drivingLicensePdf).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                        // DeliveryBoy.Edit(drivingLicensePdf, out int errors);
                    }
                }

                // BankPassbook Image
                if (model.BankPassbookImage != null)
                {
                    uc.UploadImage(model.BankPassbookImage, deliveryboy.Code + "_", "/Content/ImageUpload/", Server, _db, "", deliveryboy.Code, "");
                    var s3Client = new AmazonS3Client(accesskey, secretkey, bucketRegion);
                    var fileTransferUtility = new TransferUtility(s3Client);

                    if (model.BankPassbookImage.ContentLength > 0)
                    {
                        var filePath = Path.Combine(Server.MapPath("/Content/ImageUpload/Original/"),
                        Path.GetFileName(deliveryboy.Code + "_" + model.BankPassbookImage.FileName));
                        var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                        {
                            BucketName = bucketName,
                            FilePath = filePath.ToString(),
                            StorageClass = S3StorageClass.StandardInfrequentAccess,
                            PartSize = 6291456, // 6 MB.
                            Key = deliveryboy.Code + "_" + model.BankPassbookImage.FileName,
                            ContentType = model.BankPassbookImage.ContentType,
                            CannedACL = S3CannedACL.PublicRead
                        };
                        fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                        fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
                        fileTransferUtility.Upload(fileTransferUtilityRequest);
                        fileTransferUtility.Dispose();
                    }
                    var bankPassbookImage = _db.DeliveryBoys.FirstOrDefault(i => i.Code == deliveryboy.Code);// DeliveryBoy.Get(deliveryboy.Code);
                    bankPassbookImage.BankPassbookPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/" + deliveryboy.Code + "_" + model.BankPassbookImage.FileName;
                    bankPassbookImage.DateUpdated = DateTime.Now;
                    _db.Entry(bankPassbookImage).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    //DeliveryBoy.Edit(bankPassbookImage, out int errorCode1);
                }

                // BankPassbook Pdf
                if (model.BankPassbookPdf != null)
                {
                    if (model.BankPassbookPdf.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(model.BankPassbookPdf.FileName);
                        string _path = Path.Combine(Server.MapPath("/Content/PdfUpload"), deliveryboy.Code + "_" + _FileName);
                        model.BankPassbookPdf.SaveAs(_path);
                        var s3Client1 = new AmazonS3Client(accesskey, secretkey, bucketRegion);
                        var fileTransferUtility1 = new TransferUtility(s3Client1);
                        if (model.BankPassbookPdf.ContentLength > 0)
                        {
                            var filePath = Path.Combine(Server.MapPath("/Content/PdfUpload"),
                            Path.GetFileName(deliveryboy.Code + "_" + model.BankPassbookPdf.FileName));
                            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                            {
                                BucketName = bucketName,
                                FilePath = filePath.ToString(),
                                StorageClass = S3StorageClass.StandardInfrequentAccess,
                                PartSize = 6291456, // 6 MB.
                                Key = deliveryboy.Code + "_" + model.BankPassbookPdf.FileName,
                                ContentType = model.BankPassbookPdf.ContentType,
                                CannedACL = S3CannedACL.PublicRead
                            };
                            fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                            fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
                            fileTransferUtility1.Upload(fileTransferUtilityRequest);
                            fileTransferUtility1.Dispose();
                        }
                        var bankPassbookPdf = _db.DeliveryBoys.FirstOrDefault(i => i.Code == deliveryboy.Code);// DeliveryBoy.Get(deliveryboy.Code);
                        bankPassbookPdf.BankPassbookPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/" + deliveryboy.Code + "_" + model.BankPassbookPdf.FileName;
                        bankPassbookPdf.DateUpdated = DateTime.Now;
                        _db.Entry(bankPassbookPdf).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                       // DeliveryBoy.Edit(bankPassbookPdf, out int errors);
                    }
                }

                // CV Pdf
                if (model.CVPdf != null)
                {
                    if (model.CVPdf.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(model.CVPdf.FileName);
                        string _path = Path.Combine(Server.MapPath("/Content/PdfUpload"), deliveryboy.Code + "_" + _FileName);
                        model.CVPdf.SaveAs(_path);
                        var s3Client1 = new AmazonS3Client(accesskey, secretkey, bucketRegion);
                        var fileTransferUtility1 = new TransferUtility(s3Client1);
                        if (model.CVPdf.ContentLength > 0)
                        {
                            var filePath = Path.Combine(Server.MapPath("/Content/PdfUpload"),
                            Path.GetFileName(deliveryboy.Code + "_" + model.CVPdf.FileName));
                            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                            {
                                BucketName = bucketName,
                                FilePath = filePath.ToString(),
                                StorageClass = S3StorageClass.StandardInfrequentAccess,
                                PartSize = 6291456, // 6 MB.
                                Key = deliveryboy.Code + "_" + model.CVPdf.FileName,
                                ContentType = model.CVPdf.ContentType,
                                CannedACL = S3CannedACL.PublicRead
                            };
                            fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                            fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
                            fileTransferUtility1.Upload(fileTransferUtilityRequest);
                            fileTransferUtility1.Dispose();
                        }
                        var cVPdf = _db.DeliveryBoys.FirstOrDefault(i => i.Code == deliveryboy.Code);// DeliveryBoy.Get(deliveryboy.Code);
                        cVPdf.CVPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/" + deliveryboy.Code + "_" + model.CVPdf.FileName;
                        cVPdf.DateUpdated = DateTime.Now;
                        _db.Entry(cVPdf).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();

                        //DeliveryBoy.Edit(cVPdf, out int errors);
                    }
                }

                return RedirectToAction("DeliveryBoyList");
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    return ViewBag.Message = "Check the provided AWS Credentials.";
                }
                else
                {
                    return ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                }
            }
        }
        
        [AccessPolicy(PageCode = "")]
        public ActionResult DeliveryBoyDetails(string code)
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            ViewBag.Name = user.Name;
            DeliveryBoy deliverBoy = _db.DeliveryBoys.FirstOrDefault(i => i.Code == code);// DeliveryBoy.Get(code);
            var model = new DeliveryBoyCreateEditViewModel();
            _mapper.Map(deliverBoy, model);
            return View(model);
        }

        #endregion

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetShopCategorySelect2(string q = "")
        {
            var model = await _db.ShopCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q)).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetBrandSelect2(string q = "")
        {
            var model = await _db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q)).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            var marketingAgent = _db.MarketingAgents.FirstOrDefault(i => i.Code == user.Code);// MarketingAgent.Get(user.Code);

            var model = await _db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.MarketingAgentCode == marketingAgent.Code).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetStaffSelect2(string shopcode)
        {
            var model = await _db.Staffs.OrderBy(i => i.Name).Where(a => a.ShopCode == shopcode && a.Status == 0).Select(i => new
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
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}