using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class BrandOwnerController : Controller
    {
        private sncEntities _db = new sncEntities();
        UploadContent uc = new UploadContent();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        public BrandOwnerController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {;
                config.CreateMap<BrandOwnerRegisterEditViewModel, BrandOwner>();
                config.CreateMap<BrandOwner, BrandOwnerRegisterEditViewModel>();
                config.CreateMap<Brand, BrandListViewModel.BrandList>();
            });

            _mapper = _mapperConfiguration.CreateMapper();
        }

        public ActionResult Register()
        {
            return View();
        }
        private const string _prefix = "BOW";

        private static string _generatedCode
        {
            get
            {
                return ShopNow.Helpers.DRC.Generate(_prefix);
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(BrandOwnerRegisterEditViewModel model)
        {

            var brandOwner = _mapper.Map<BrandOwnerRegisterEditViewModel, BrandOwner>(model);
           // brandOwner.Code = _generatedCode;
            brandOwner.Status = 0;
            brandOwner.DateEncoded = DateTime.Now;
            brandOwner.DateUpdated = DateTime.Now;
            _db.BrandOwners.Add(brandOwner);
            _db.SaveChanges();
         //   brandOwner.Code = brandOwner.Code;// BrandOwner.Add(brandOwner, out int error);
            if (model.BrandOwnerImage != null)
            {
                uc.UploadImage(model.BrandOwnerImage, brandOwner.Id + "_", "/Content/BrandImage/", Server, _db, "", brandOwner.Id.ToString(), "");
                var brandOwnerImage = _db.BrandOwners.FirstOrDefault(i => i.Id == brandOwner.Id); //BrandOwner.Get(brandOwner.Code);
                brandOwnerImage.ImageAuthoriseBrandPath = brandOwner.Id + "_" + model.BrandOwnerImage.FileName;
                brandOwnerImage.DateUpdated = DateTime.Now;
                _db.Entry(brandOwnerImage).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                //BrandOwner.Edit(brandOwnerImage, out error);
            }
            return RedirectToAction("Login", "BrandOwner");
        }

        public ActionResult Updation()
        {
            var user = ((Helpers.Sessions.User)Session["BRANDUSER"]);
            ViewBag.Name = user.Name;
            var brandOwner = _db.BrandOwners.FirstOrDefault(i => i.Id == user.Id);//BrandOwner.Get(user.Code);
            var model = _mapper.Map<BrandOwner, BrandOwnerRegisterEditViewModel>(brandOwner);
            return View(model);
        }

        [HttpPost]
        public ActionResult Updation(BrandOwnerRegisterEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["BRANDUSER"]);
            ViewBag.Name = user.Name;

            return View(model);
        }

        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["BRANDUSER"]);
            ViewBag.Name = user.Name;
            var model = new BrandListViewModel();

            model.List = _db.Brands.Where(i => i.Status == 0).ToList().AsQueryable().ProjectTo<BrandListViewModel.BrandList>(_mapperConfiguration).OrderBy(i => i.Name).ToList();

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (CheckAlreadyLoggedIn())
                return Redirect(returnUrl != null ? returnUrl : "/BrandOwner/Login");
            else
            {
                BrandOwnerLoginViewModel loginModel = new BrandOwnerLoginViewModel();
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
                var user = _db.BrandOwners.FirstOrDefault(i => i.PhoneNumber == userId && i.Status == 0);
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
        public ActionResult Login(BrandOwnerLoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = _db.BrandOwners.FirstOrDefault(i => i.PhoneNumber == model.PhoneNumber && i.Password == model.Password && i.Status == 0);
                if (user != null)
                {
                    Helpers.Sessions.SetBrandOwnerUser(model.PhoneNumber);
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

                    return RedirectToAction(returnUrl != null ? returnUrl : "Dashboard", "BrandOwner");
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
            var user = ((Helpers.Sessions.User)Session["BRANDUSER"]);
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
            var ExistingDetails = _db.BrandOwners.FirstOrDefault(i => i.Id == Convert.ToInt32(LoginMemberId) && i.Status == 0);
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

        //  [AccessPolicy(Page = "ACTC.LogOut")]
        public ActionResult LogOut()
        {
            Session.Abandon(); 
            return RedirectToAction("Login");
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