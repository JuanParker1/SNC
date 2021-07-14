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
            var marketingAgent = _db.MarketingAgents.FirstOrDefault(i => i.Id == user.Id);
            var model = _mapper.Map<MarketingAgent, MarketingAgentUpdationViewModel>(marketingAgent);
            return View(model);
        }

        [HttpPost]
        public ActionResult Updation(MarketingAgentUpdationViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            ViewBag.Name = user.Name;
            var marketingAgent = _db.MarketingAgents.FirstOrDefault(i => i.Id == model.Id);
            //var ma = _mapper.Map(model, marketingAgent);
            marketingAgent.Name = model.Name;
            marketingAgent.PhoneNumber = model.PhoneNumber;
            marketingAgent.Email = model.Email;
            marketingAgent.PanNumber = model.PanNumber;
            marketingAgent.ImagePanPath = model.ImagePanPath;
            marketingAgent.DateUpdated = DateTime.Now;
            _db.Entry(marketingAgent).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();

            try
            {
                if (model.PanImage != null)
                {
                    uc.UploadFiles(model.PanImage.InputStream, marketingAgent.Id + "_" + model.PanImage.FileName, accesskey, secretkey, "image");
                    marketingAgent.ImagePath = marketingAgent.Id + "_" + model.PanImage.FileName.Replace(" ", "");

                    marketingAgent.DateUpdated = DateTime.Now;
                    _db.Entry(marketingAgent).State = System.Data.Entity.EntityState.Modified;
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
            //string LoginMemberId = Convert.ToString(Session["UserCode"]);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var ExistingDetails = _db.MarketingAgents.FirstOrDefault(i => i.Id == user.Id && i.Status == 0);
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
            model.List = _db.Shops.Where(i => i.Status == 0 && i.MarketingAgentId == user.Id).Select(i => new ShopListViewModel.ShopList
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
            model.List = _db.DeliveryBoys.Where(i => i.Status == 0 && i.MarketingAgentId == user.Id).Select(i => new DeliveryBoyListViewModel.DeliveryBoyList
            {
                Id = i.Id,
                Name = i.Name,
                PhoneNumber = i.PhoneNumber,
                ShopId = i.ShopId,
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
            deliveryboy.CustomerId = user.Id;
            deliveryboy.CustomerName = user.Name;
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
                    uc.UploadFiles(model.DeliveryBoyImage.InputStream, deliveryboy.Id + "_" + model.DeliveryBoyImage.FileName, accesskey, secretkey, "image");
                    deliveryboy.ImagePath = deliveryboy.Id + "_" + model.DeliveryBoyImage.FileName.Replace(" ", "");
                }

                // DrivingLicense Image
                if (model.DrivingLicenseImage != null)
                {
                    uc.UploadFiles(model.DrivingLicenseImage.InputStream, deliveryboy.Id + "_" + model.DrivingLicenseImage.FileName, accesskey, secretkey, "image");
                    deliveryboy.ImagePath = deliveryboy.Id + "_" + model.DrivingLicenseImage.FileName.Replace(" ", "");
                }

                // DrivingLicense Pdf
                if (model.DrivingLicensePdf != null)
                {
                    uc.UploadFiles(model.DrivingLicensePdf.InputStream, deliveryboy.Id + "_" + model.DrivingLicensePdf.FileName, accesskey, secretkey, "pdf");
                    deliveryboy.DrivingLicenseImagePath = deliveryboy.Id + "_" + model.DrivingLicensePdf.FileName.Replace(" ", "");
                }

                // BankPassbook Image
                if (model.BankPassbookImage != null)
                {
                    uc.UploadFiles(model.BankPassbookImage.InputStream, deliveryboy.Id + "_" + model.BankPassbookImage.FileName, accesskey, secretkey, "image");
                    deliveryboy.BankPassbookPath = deliveryboy.Id + "_" + model.BankPassbookImage.FileName.Replace(" ", "");
                }

                // BankPassbook Pdf
                if (model.BankPassbookPdf != null)
                {
                    uc.UploadFiles(model.BankPassbookPdf.InputStream, deliveryboy.Id + "_" + model.BankPassbookPdf.FileName, accesskey, secretkey, "pdf");
                    deliveryboy.BankPassbookPath = deliveryboy.Id + "_" + model.BankPassbookPdf.FileName.Replace(" ", "");
                }

                // CV Pdf
                if (model.CVPdf != null)
                {
                    uc.UploadFiles(model.CVPdf.InputStream, deliveryboy.Id + "_" + model.CVPdf.FileName, accesskey, secretkey, "pdf");
                    deliveryboy.CVPath = deliveryboy.Id + "_" + model.CVPdf.FileName.Replace(" ", "");
                }
                deliveryboy.DateUpdated = DateTime.Now;
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
        public ActionResult DeliveryBoyEdit(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            ViewBag.Name = user.Name;
            var deliveryBoy = _db.DeliveryBoys.FirstOrDefault(i => i.Id == Id);
            var model = _mapper.Map<DeliveryBoy, DeliveryBoyCreateEditViewModel>(deliveryBoy);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "")]
        public ActionResult DeliveryBoyEdit(DeliveryBoyCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);

            DeliveryBoy deliveryboy = _db.DeliveryBoys.FirstOrDefault(i => i.Id == model.Id);// DeliveryBoy.Get(model.Code);
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
                    uc.UploadFiles(model.DeliveryBoyImage.InputStream, deliveryboy.Id + "_" + model.DeliveryBoyImage.FileName, accesskey, secretkey, "image");
                    deliveryboy.ImagePath = deliveryboy.Id + "_" + model.DeliveryBoyImage.FileName.Replace(" ", "");
                }

                // DrivingLicense Image
                if (model.DrivingLicenseImage != null)
                {
                    uc.UploadFiles(model.DrivingLicenseImage.InputStream, deliveryboy.Id + "_" + model.DrivingLicenseImage.FileName, accesskey, secretkey, "image");
                    deliveryboy.ImagePath = deliveryboy.Id + "_" + model.DrivingLicenseImage.FileName.Replace(" ", "");
                }

                // DrivingLicense Pdf
                if (model.DrivingLicensePdf != null)
                {
                    uc.UploadFiles(model.DrivingLicensePdf.InputStream, deliveryboy.Id + "_" + model.DrivingLicensePdf.FileName, accesskey, secretkey, "pdf");
                    deliveryboy.DrivingLicenseImagePath = deliveryboy.Id + "_" + model.DrivingLicensePdf.FileName.Replace(" ", "");
                }

                // BankPassbook Image
                if (model.BankPassbookImage != null)
                {
                    uc.UploadFiles(model.BankPassbookImage.InputStream, deliveryboy.Id + "_" + model.BankPassbookImage.FileName, accesskey, secretkey, "image");
                    deliveryboy.BankPassbookPath = deliveryboy.Id + "_" + model.BankPassbookImage.FileName.Replace(" ", "");
                }

                // BankPassbook Pdf
                if (model.BankPassbookPdf != null)
                {
                    uc.UploadFiles(model.BankPassbookPdf.InputStream, deliveryboy.Id + "_" + model.BankPassbookPdf.FileName, accesskey, secretkey, "pdf");
                    deliveryboy.BankPassbookPath = deliveryboy.Id + "_" + model.BankPassbookPdf.FileName.Replace(" ", "");
                }

                // CV Pdf
                if (model.CVPdf != null)
                {
                    uc.UploadFiles(model.CVPdf.InputStream, deliveryboy.Id + "_" + model.CVPdf.FileName, accesskey, secretkey, "pdf");
                    deliveryboy.CVPath = deliveryboy.Id + "_" + model.CVPdf.FileName.Replace(" ", "");
                }
                deliveryboy.DateUpdated = DateTime.Now;
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
        public ActionResult DeliveryBoyDetails(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            ViewBag.Name = user.Name;
            DeliveryBoy deliverBoy = _db.DeliveryBoys.FirstOrDefault(i => i.Id == Id);// DeliveryBoy.Get(code);
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
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            var marketingAgent = _db.MarketingAgents.FirstOrDefault(i => i.Id == user.Id);

            var model = await _db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.MarketingAgentId == marketingAgent.Id).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetStaffSelect2(int shopid)
        {
            var model = await _db.Staffs.OrderBy(i => i.Name).Where(a => a.ShopId == shopid && a.Status == 0).Select(i => new
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