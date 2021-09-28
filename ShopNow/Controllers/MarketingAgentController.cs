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
        private sncEntities _db = new sncEntities();
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
                config.CreateMap<DeliveryBoyAddViewModel, DeliveryBoy>();
                config.CreateMap<DeliveryBoyEditViewModel, DeliveryBoy>();
                config.CreateMap<DeliveryBoy, DeliveryBoyEditViewModel>();                
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
            marketingAgent.Status = 0;
            marketingAgent.DateEncoded = DateTime.Now;
            marketingAgent.DateUpdated = DateTime.Now;
            _db.MarketingAgents.Add(marketingAgent);
            _db.SaveChanges();

            return RedirectToAction("Login", "MarketingAgent");
        }

        public ActionResult Updation()
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            ViewBag.Name = user.Name;
            var marketingAgent = _db.MarketingAgents.FirstOrDefault(i => i.Id == user.Id);//MarketingAgent.Get(user.Code);
            var model = _mapper.Map<MarketingAgent, MarketingAgentUpdationViewModel>(marketingAgent);
            return View(model);
        }

        [HttpPost]
        public ActionResult Updation(MarketingAgentUpdationViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            ViewBag.Name = user.Name;
            var marketingAgent = _db.MarketingAgents.FirstOrDefault(i => i.Id == model.Id);// MarketingAgent.Get(model.Code);
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
                    uc.UploadImage(model.PanImage, marketingAgent.Id + "_", "/Content/ImageUpload/", Server, _db, "", marketingAgent.Id.ToString(), "");
                    var s3Client = new AmazonS3Client(accesskey, secretkey, bucketRegion);
                    var fileTransferUtility = new TransferUtility(s3Client);

                    if (model.PanImage.ContentLength > 0)
                    {
                        var filePath = Path.Combine(Server.MapPath("/Content/ImageUpload/Original/"),
                        Path.GetFileName(marketingAgent.Id + "_" + model.PanImage.FileName));
                        var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                        {
                            BucketName = bucketName,
                            FilePath = filePath.ToString(),
                            StorageClass = S3StorageClass.StandardInfrequentAccess,
                            PartSize = 6291456, // 6 MB.
                            Key = marketingAgent.Id + "_" + model.PanImage.FileName,
                            ContentType = model.PanImage.ContentType,
                            CannedACL = S3CannedACL.PublicRead
                        };
                        fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                        fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
                        fileTransferUtility.Upload(fileTransferUtilityRequest);
                        fileTransferUtility.Dispose();
                    }
                    var PanImg = _db.MarketingAgents.FirstOrDefault(i => i.Id == model.Id);// MarketingAgent.Get(model.Code);
                    PanImg.ImagePanPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/" + PanImg.Id + "_" + model.PanImage.FileName;
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
            var ExistingDetails = _db.MarketingAgents.FirstOrDefault(i => i.Id == Convert.ToInt32(LoginMemberId) && i.Status == 0);
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
                Id = i.Id,
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
             //model.List = _db.Shops.Where(i => i.Status == 0 && Convert.ToInt32(i.MarketingAgentId) == user.Id).Select(i => new ShopListViewModel.ShopList
             model.List = _db.Shops.Where(i => i.Status == 0).Select(i => new ShopListViewModel.ShopList
            {
                Id = i.Id,
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
            model.List = _db.DeliveryBoys.Where(i => i.Status == 0 && Convert.ToInt32(i.MarketingAgentId) == user.Id).Select(i => new DeliveryBoyListViewModel.DeliveryBoyList
            {
                Id = i.Id,
                Name = i.Name,
                PhoneNumber = i.PhoneNumber,
                //ShopId = i.ShopCode,
                //ShopName = i.ShopName,
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
        public ActionResult DeliveryBoyCreate(DeliveryBoyAddViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);

            var deliveryboy = _mapper.Map<DeliveryBoyAddViewModel, DeliveryBoy>(model);
            var customer = _db.Customers.Where(i => i.PhoneNumber == model.PhoneNumber).FirstOrDefault();
            if (customer != null)
            {
                customer.Position = 3;
                customer.DateUpdated = DateTime.Now;
                _db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                deliveryboy.CustomerId = customer.Id;
                deliveryboy.CustomerName = customer.Name;
            }
            deliveryboy.CreatedBy = user.Name;
            deliveryboy.UpdatedBy = user.Name;
            deliveryboy.DateEncoded = DateTime.Now;
            deliveryboy.DateUpdated = DateTime.Now;
            deliveryboy.Status = 1;
            try
            {
                // DeliveryBoy Image
                if (model.DeliveryBoyImage != null)
                {
                    uc.UploadFiles(model.DeliveryBoyImage.InputStream, deliveryboy.Id + "_" + model.DeliveryBoyImage.FileName, accesskey, secretkey, "image");
                    deliveryboy.ImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + deliveryboy.Id + "_" + model.DeliveryBoyImage.FileName.Replace(" ", "");
                }

                // DrivingLicense Image
                if (model.DrivingLicenseImage != null)
                {
                    uc.UploadFiles(model.DrivingLicenseImage.InputStream, deliveryboy.Id + "_" + model.DrivingLicenseImage.FileName, accesskey, secretkey, "image");
                    deliveryboy.DrivingLicenseImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + deliveryboy.Id + "_" + model.DrivingLicenseImage.FileName.Replace(" ", "");
                }

                // DrivingLicense Pdf
                if (model.DrivingLicensePdf != null)
                {
                    uc.UploadFiles(model.DrivingLicensePdf.InputStream, deliveryboy.Id + "_" + model.DrivingLicensePdf.FileName, accesskey, secretkey, "pdf");
                    deliveryboy.DrivingLicenseImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Uploads/" + deliveryboy.Id + "_" + model.DrivingLicensePdf.FileName.Replace(" ", "");
                }

                // BankPassbook Image
                if (model.BankPassbookImage != null)
                {
                    uc.UploadFiles(model.BankPassbookImage.InputStream, deliveryboy.Id + "_" + model.BankPassbookImage.FileName, accesskey, secretkey, "image");
                    deliveryboy.BankPassbookPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + deliveryboy.Id + "_" + model.BankPassbookImage.FileName.Replace(" ", "");
                }

                // BankPassbook Pdf
                if (model.BankPassbookPdf != null)
                {
                    uc.UploadFiles(model.BankPassbookPdf.InputStream, deliveryboy.Id + "_" + model.BankPassbookPdf.FileName, accesskey, secretkey, "pdf");
                    deliveryboy.BankPassbookPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Uploads/" + deliveryboy.Id + "_" + model.BankPassbookPdf.FileName.Replace(" ", "");
                }

                // CV Pdf
                if (model.CVPdf != null)
                {
                    uc.UploadFiles(model.CVPdf.InputStream, deliveryboy.Id + "_" + model.CVPdf.FileName, accesskey, secretkey, "pdf");
                    deliveryboy.CVPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Uploads/" + deliveryboy.Id + "_" + model.CVPdf.FileName.Replace(" ", "");
                }
                _db.DeliveryBoys.Add(deliveryboy);
                _db.SaveChanges();
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
        public ActionResult DeliveryBoyEdit(int id)
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(id.ToString()))
                return HttpNotFound();
            var deliveryBoy = _db.DeliveryBoys.FirstOrDefault(i => i.Id == id);
            var model = _mapper.Map<DeliveryBoy, DeliveryBoyEditViewModel>(deliveryBoy);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "")]
        public ActionResult DeliveryBoyEdit(DeliveryBoyEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            DeliveryBoy deliveryboy = _db.DeliveryBoys.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, deliveryboy);

            deliveryboy.DateUpdated = DateTime.Now;
            deliveryboy.UpdatedBy = user.Name;
            try
            {
                // DeliveryBoy Image
                if (model.DeliveryBoyImage != null)
                {
                    uc.UploadFiles(model.DeliveryBoyImage.InputStream, deliveryboy.Id + "_" + model.DeliveryBoyImage.FileName, accesskey, secretkey, "image");
                    deliveryboy.ImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + deliveryboy.Id + "_" + model.DeliveryBoyImage.FileName.Replace(" ", "");
                }

                // DrivingLicense Image
                if (model.DrivingLicenseImage != null)
                {
                    uc.UploadFiles(model.DrivingLicenseImage.InputStream, deliveryboy.Id + "_" + model.DrivingLicenseImage.FileName, accesskey, secretkey, "image");
                    deliveryboy.DrivingLicenseImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + deliveryboy.Id + "_" + model.DrivingLicenseImage.FileName.Replace(" ", "");
                }

                // DrivingLicense Pdf
                if (model.DrivingLicensePdf != null)
                {
                    uc.UploadFiles(model.DrivingLicensePdf.InputStream, deliveryboy.Id + "_" + model.DrivingLicensePdf.FileName, accesskey, secretkey, "pdf");
                    deliveryboy.DrivingLicenseImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Uploads/" + deliveryboy.Id + "_" + model.DrivingLicensePdf.FileName.Replace(" ", "");
                }

                // BankPassbook Image
                if (model.BankPassbookImage != null)
                {
                    uc.UploadFiles(model.BankPassbookImage.InputStream, deliveryboy.Id + "_" + model.BankPassbookImage.FileName, accesskey, secretkey, "image");
                    deliveryboy.BankPassbookPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + deliveryboy.Id + "_" + model.BankPassbookImage.FileName.Replace(" ", "");
                }

                // BankPassbook Pdf
                if (model.BankPassbookPdf != null)
                {
                    uc.UploadFiles(model.BankPassbookPdf.InputStream, deliveryboy.Id + "_" + model.BankPassbookPdf.FileName, accesskey, secretkey, "pdf");
                    deliveryboy.BankPassbookPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Uploads/" + deliveryboy.Id + "_" + model.BankPassbookPdf.FileName.Replace(" ", "");
                }

                // CV Pdf
                if (model.CVPdf != null)
                {
                    uc.UploadFiles(model.CVPdf.InputStream, deliveryboy.Id + "_" + model.CVPdf.FileName, accesskey, secretkey, "pdf");
                    deliveryboy.CVPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Uploads/" + deliveryboy.Id + "_" + model.CVPdf.FileName.Replace(" ", "");
                }
                _db.Entry(deliveryboy).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
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
        public ActionResult DeliveryBoyDetails(int id)
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            ViewBag.Name = user.Name;
            DeliveryBoy deliverBoy = _db.DeliveryBoys.FirstOrDefault(i => i.Id == id);
            var model = new DeliveryBoyEditViewModel();
            _mapper.Map(deliverBoy, model);
            return View(model);
        }

        // Franchise Assign
        [AccessPolicy(PageCode = "")]
        public ActionResult AssignedFranchiseList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new FranchiseListViewModel();
            model.Lists = _db.MarketingAgents.Where(i => i.Status == 0).Join(_db.Shops.Where(i => i.Status == 0), m => m.Id, s => s.MarketingAgentId, (m, s) => new { m, s })
                .Join(_db.DeliveryBoys.Where(i => i.Status == 0), p => p.m.Id, d => d.MarketingAgentId, (p, d) => new { p, d })
                .GroupBy(i => i.p.m.Id)
                .AsEnumerable()
                .Select(i => new FranchiseListViewModel.FranchiseList
                {
                    MarketingAgentId = i.FirstOrDefault().p.m.Id,
                    MarketingAgentName = i.FirstOrDefault().p.m.Name,
                    ShopListItems = i.Where(a => a.p.s.Status == 0).Select(a => new FranchiseListViewModel.FranchiseList.ShopListItem
                    {
                        ShopId = a.p.s.Id,
                        ShopName = a.p.s.Name,
                    }).ToList(),
                    DeliveryBoyListItems = i.Where(a => a.d.Status == 0).Select(a => new FranchiseListViewModel.FranchiseList.DeliveryBoyListItem
                    {
                        DeliveryBoyId = a.d.Id,
                        DeliveryBoyName = a.d.Name
                    }).ToList()
                }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult AssignFranchise()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [AccessPolicy(PageCode = "")]
        public ActionResult AssignFranchise(FranchiseAssignViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);          
            if (model.ShopIds != null)
            {
                foreach (var item in model.ShopIds)
                {
                    var shop = _db.Shops.FirstOrDefault(i => i.Id == item);
                    shop.MarketingAgentId = model.MarketingAgentId;
                    shop.MarketingAgentName = model.MarketingAgentName;
                    _db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            if (model.DeliveryBoyIds != null)
            {
                foreach (var item in model.DeliveryBoyIds)
                {
                    var deliveryboy = _db.DeliveryBoys.FirstOrDefault(i => i.Id == item);
                    deliveryboy.MarketingAgentId = model.MarketingAgentId;
                    deliveryboy.MarketingAgentName = model.MarketingAgentName;
                    _db.Entry(deliveryboy).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            return RedirectToAction("AssignedFranchiseList");
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult FranchiseUpdate(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new FranchiseAssignUpdateViewModel();
            model.ShopIds = string.Join(",",_db.Shops.Where(i => i.MarketingAgentId == id && i.Status == 0).Select(i => i.Id).ToList());
            model.DeliveryBoyIds = string.Join(",",_db.DeliveryBoys.Where(i => i.MarketingAgentId == id && i.Status == 0).Select(i => i.Id).ToList());

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "")]
        public ActionResult FranchiseUpdate(FranchiseAssignUpdateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            return RedirectToAction("AssignedFranchiseList");
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult ShopDelete(string id)
        {
            var dId = AdminHelpers.DCodeInt(id);
            var marketAgent = _db.MarketingAgents.FirstOrDefault(i => i.Id == dId);
            if (marketAgent != null)
            {
                marketAgent.Status = 2;
                _db.Entry(marketAgent).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
            }
            return RedirectToAction("AssignedFranchiseList");
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult DeliveryBoyDelete(string id)
        {
            var dId = AdminHelpers.DCodeInt(id);
            var marketingAgent = _db.MarketingAgents.FirstOrDefault(i => i.Id == dId);
            if (marketingAgent != null)
            {
                marketingAgent.Status = 2;
                _db.Entry(marketingAgent).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
            }
            return RedirectToAction("AssignedFranchiseList");
        }

        #endregion

        public JsonResult GetAssignMarketingAgent(int marketingagentId)
        {
            var shop = _db.Shops.Any(i => i.MarketingAgentId == marketingagentId);
            var deliveryboy = _db.DeliveryBoys.Any(i => i.MarketingAgentId == marketingagentId);
            if (shop == true || deliveryboy == true) 
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetDeliveryBoySelect2(string q = "")
        {
            var model = await _db.DeliveryBoys.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetMarketingAgentSelect2(string q = "")
        {
            var model = await _db.MarketingAgents.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetShopCategorySelect2(string q = "")
        {
            var model = await _db.ShopCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q)).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetBrandSelect2(string q = "")
        {
            var model = await _db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q)).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await _db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetStaffSelect2(int shopId)
        {
            var model = await _db.Staffs.OrderBy(i => i.Name).Where(a => a.ShopId == shopId && a.Status == 0).Select(i => new
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
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}