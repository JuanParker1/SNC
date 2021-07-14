using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    //[Authorize]

    public class StaffController : Controller
    {
        private ShopnowchatEntities db = new ShopnowchatEntities();
        UploadContent uc = new UploadContent();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private const string filePath = null;
        private static readonly string bucketName = ConfigurationManager.AppSettings["BucketName"];
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.APSouth1;
        private static readonly string accesskey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static readonly string secretkey = ConfigurationManager.AppSettings["AWSSecretKey"];
        private static string _generatedPassword
        {
            get
            {
                return ShopNow.Helpers.DRC.GeneratePassword();
            }
        }
        private const string _prefix = "STF";

        private static string _generatedCode
        {
            get
            {
                return ShopNow.Helpers.DRC.Generate(_prefix);
            }
        }
        public StaffController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<Staff, StaffListViewModel.StaffList > ();
                config.CreateMap<StaffCreateEditViewModel, Staff>();
                config.CreateMap<Staff, StaffCreateEditViewModel>();
                config.CreateMap<Shop, StaffCreateEditViewModel.ShopList>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNSTFL001")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from s in db.Staffs
                        select s).OrderBy(s => s.Name).Where(i => i.Status == 0).ToList();
            return View(List);
        }
        [AccessPolicy(PageCode = "SHNSTFC002")]
        public ActionResult Create()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNSTFC002")]
        public ActionResult Create(StaffCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);

            var staff = _mapper.Map<StaffCreateEditViewModel, Staff>(model);
            if(model.ShopId != 0)
            {
                var shop = db.Shops.FirstOrDefault(i => i.Id == model.ShopId); //Get(model.ShopCode);
                staff.CustomerId = shop.CustomerId;
                staff.CustomerName = shop.CustomerName;
            }
            staff.CreatedBy = user.Name;
            staff.UpdatedBy = user.Name;
            staff.Status = 0;
            staff.DateEncoded = DateTime.Now;
            staff.DateUpdated = DateTime.Now;
            db.Staffs.Add(staff);
            db.SaveChanges();
            //staff.Code = Staff.Add(staff, out int error);
            try
            {
                //var staffImg = Staff.Get(staff.Code);
                // Staff Image
                if (model.StaffImage != null)
                {
                    uc.UploadFiles(model.StaffImage.InputStream, staff.Id + "_" + model.StaffImage.FileName, accesskey, secretkey, "image");
                    staff.ImagePath = staff.Id + "_" + model.StaffImage.FileName.Replace(" ", "");
                }
                db.Staffs.Add(staff);
                db.SaveChanges();
               //Staff.Edit(staffImg, out int error1);
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

        [AccessPolicy(PageCode = "SHNSTFE003")]
        public ActionResult Edit(string code)
        {
            var dCode = AdminHelpers.DCodeInt(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (dCode==0)
                return HttpNotFound();
            var staff = db.Staffs.FirstOrDefault(i => i.Id == dCode); //Staff.Get(dCode);
            var model = _mapper.Map<Staff, StaffCreateEditViewModel>(staff);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNSTFE003")]
        public ActionResult Edit(StaffCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            Staff staff = db.Staffs.FirstOrDefault(i => i.Id == model.Id); // Staff.Get(model.Code);
            _mapper.Map(model, staff);

            staff.DateUpdated = DateTime.Now;
            staff.UpdatedBy = user.Name;
           // Staff.Edit(staff, out int errorCode);

            try
            {
               // var staffImg = Staff.Get(staff.Code);
                // Staff Image
                if (model.StaffImage != null)
                {
                    uc.UploadFiles(model.StaffImage.InputStream, staff.Id + "_" + model.StaffImage.FileName, accesskey, secretkey, "image");
                    staff.ImagePath = staff.Id + "_" + model.StaffImage.FileName.Replace(" ", "");
                }
                db.Entry(staff).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //  Staff.Edit(staffImg, out int error1);
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

        [AccessPolicy(PageCode = "SHNSTFD004")]
        public ActionResult Details(string code)
        {
            var dCode = AdminHelpers.DCodeInt(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            Staff staff = db.Staffs.FirstOrDefault(i => i.Id == dCode); //Staff.Get(dCode);
            var model = new StaffCreateEditViewModel();
            _mapper.Map(staff, model);
            model.List = db.Shops.Where(i => i.Id == staff.ShopId).Select(i => new StaffCreateEditViewModel.ShopList
            {
                Address = i.Address,
                CountryName = i.CountryName,
                Id = i.Id,
                DistrictName = i.DistrictName,
                Name=i.Name,
                PinCode=i.PinCode,
                StateName=i.StateName,
                StreetName=i.StreetName
            }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNSTFR005")]
        public ActionResult Delete(string code)
        {
            var dCode = AdminHelpers.DCodeInt(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var staff = db.Staffs.FirstOrDefault(i => i.Id == dCode); // Staff.Get(dCode);
            staff.Status = 2;
            staff.DateUpdated = DateTime.Now;
            staff.UpdatedBy = user.Name;
            db.Entry(staff).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNSTFGP006")]
        public JsonResult GeneratePassword(string code)
        {
            var dCode = AdminHelpers.DCodeInt(code);
            var staff = db.Staffs.FirstOrDefault(i => i.Id == dCode); // Staff.Get(code);
            staff.Password = _generatedPassword;
            staff.DateUpdated = DateTime.Now;
            db.Entry(staff).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

           // Staff.Edit(staff, out int error);

            return Json(new { data = staff.Password, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSTFC002")]
        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
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
