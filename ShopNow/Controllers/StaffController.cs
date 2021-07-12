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
        //public ActionResult List(int? ColumnName, int? AdvanceFilter, string keyword = "", string shopcode = "")
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var model = new StaffListViewModel();

        //    if (shopcode != "")
        //    {
        //        model.List = db.Staffs.Where(i => i.Status == 0 && i.ShopCode == shopcode).Select(i => new StaffListViewModel.StaffList
        //        {
        //            Code = i.Code,
        //            Name = i.Name,
        //            PhoneNumber = i.PhoneNumber,
        //            ImagePath = i.ImagePath,
        //            ShopCode = i.ShopCode,
        //            ShopName = i.ShopName
        //        }).ToList();
        //    }
        //    else if (ColumnName == 0)
        //    {
        //        model.List = db.Staffs.Where(i => (AdvanceFilter == 0 ? i.Name.Contains(keyword) : true) && (AdvanceFilter == 1 ? i.Name != keyword : true)
        //        && (AdvanceFilter == 2 ? i.Name != null : true) && (AdvanceFilter == 3 ? i.Name == null : true) && (AdvanceFilter == 4 ? i.Name.EndsWith(keyword) : true)
        //        && (AdvanceFilter == 5 ? i.Name.StartsWith(keyword) : true) && (AdvanceFilter == 6 ? !i.Name.StartsWith(keyword) : true))
        //         .Select(i => new StaffListViewModel.StaffList
        //         {
        //             Code = i.Code,
        //             Name = i.Name,
        //             PhoneNumber = i.PhoneNumber,
        //             ImagePath = i.ImagePath,
        //             ShopCode = i.ShopCode,
        //             ShopName = i.ShopName
        //         }).ToList();
        //    }
        //    else if (ColumnName == 1)
        //    {
        //        model.List = db.Staffs.Where(i => (AdvanceFilter == 0 ? i.PhoneNumber.Contains(keyword) : true) && (AdvanceFilter == 1 ? i.PhoneNumber != keyword : true)
        //       && (AdvanceFilter == 2 ? i.PhoneNumber != null : true) && (AdvanceFilter == 3 ? i.PhoneNumber == null : true) && (AdvanceFilter == 4 ? i.PhoneNumber.EndsWith(keyword) : true)
        //       && (AdvanceFilter == 5 ? i.PhoneNumber.StartsWith(keyword) : true) && (AdvanceFilter == 6 ? !i.PhoneNumber.StartsWith(keyword) : true))
        //        .Select(i => new StaffListViewModel.StaffList
        //        {
        //            Code = i.Code,
        //            Name = i.Name,
        //            PhoneNumber = i.PhoneNumber,
        //            ImagePath = i.ImagePath,
        //            ShopCode = i.ShopCode,
        //            ShopName = i.ShopName
        //        }).ToList();
        //    }
        //    else if (ColumnName == 2)
        //    {
        //        model.List = db.Staffs.Where(i => (AdvanceFilter == 0 ? i.ShopName.Contains(keyword) : true) && (AdvanceFilter == 1 ? i.ShopName != keyword : true)
        //       && (AdvanceFilter == 2 ? i.ShopName != null : true) && (AdvanceFilter == 3 ? i.ShopName == null : true) && (AdvanceFilter == 4 ? i.ShopName.EndsWith(keyword) : true)
        //       && (AdvanceFilter == 5 ? i.ShopName.StartsWith(keyword) : true) && (AdvanceFilter == 6 ? !i.ShopName.StartsWith(keyword) : true))
        //        .Select(i => new StaffListViewModel.StaffList
        //        {
        //            Code = i.Code,
        //            Name = i.Name,
        //            PhoneNumber = i.PhoneNumber,
        //            ImagePath = i.ImagePath,
        //            ShopCode = i.ShopCode,
        //            ShopName = i.ShopName
        //        }).ToList();
        //    }
        //    else
        //    {
        //        model.List = Staff.GetList().AsQueryable().ProjectTo<StaffListViewModel.StaffList>(_mapperConfiguration).OrderBy(i => i.Name).ToList();
        //    }

        //    return View(model);
        //}

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
            if(model.ShopCode != null)
            {
                var shop = db.Shops.FirstOrDefault(i => i.Code == model.ShopCode); //Get(model.ShopCode);
                staff.CustomerCode = shop.CustomerCode;
                staff.CustomerName = shop.CustomerName;
                staff.IpAddress = shop.IpAddress;
            }
            staff.CreatedBy = user.Name;
            staff.UpdatedBy = user.Name;
            staff.Code = _generatedCode;
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
                    uc.UploadFiles(model.StaffImage.InputStream, staff.Code + "_" + model.StaffImage.FileName, accesskey, secretkey, "image");
                    staff.ImagePath = staff.Code + "_" + model.StaffImage.FileName.Replace(" ", "");
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
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (string.IsNullOrEmpty(dCode))
                return HttpNotFound();
            var staff = db.Staffs.FirstOrDefault(i => i.Code == dCode); //Staff.Get(dCode);
            var model = _mapper.Map<Staff, StaffCreateEditViewModel>(staff);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNSTFE003")]
        public ActionResult Edit(StaffCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            Staff staff = db.Staffs.FirstOrDefault(i => i.Code == model.Code); // Staff.Get(model.Code);
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
                    uc.UploadFiles(model.StaffImage.InputStream, staff.Code + "_" + model.StaffImage.FileName, accesskey, secretkey, "image");
                    staff.ImagePath = staff.Code + "_" + model.StaffImage.FileName.Replace(" ", "");
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
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            Staff staff = db.Staffs.FirstOrDefault(i => i.Code == dCode); //Staff.Get(dCode);
            var model = new StaffCreateEditViewModel();
            _mapper.Map(staff, model);
            model.List = db.Shops.Where(i => i.Code == staff.ShopCode).Select(i => new StaffCreateEditViewModel.ShopList
            {
                Address = i.Address,
                CountryName = i.CountryName,
                Code = i.Code,
                DistrictName = i.DistrictName,
                FridayCloseTime=i.FridayCloseTime,
                FridayOpenTime=i.FridayOpenTime,
                MondayCloseTime=i.MondayCloseTime,
                MondayOpenTime=i.MondayOpenTime,
                Name=i.Name,
                PinCode=i.PinCode,
                SaturdayCloseTime=i.SaturdayCloseTime,
                SaturdayOpenTime=i.SaturdayOpenTime,
                StateName=i.StateName,
                StreetName=i.StreetName,
                SundayCloseTime=i.SundayCloseTime,
                SundayOpenTime=i.SundayOpenTime,
                ThursdayCloseTime=i.ThursdayCloseTime,
                ThursdayOpenTime=i.ThursdayOpenTime,
                TuesdayCloseTime=i.TuesdayCloseTime,
                TuesdayOpenTime=i.TuesdayOpenTime,
                WednesdayCloseTime=i.WednesdayCloseTime,
               WednesdayOpenTime=i.WednesdayOpenTime 
            }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNSTFR005")]
        public ActionResult Delete(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var staff = db.Staffs.FirstOrDefault(i => i.Code == dCode); // Staff.Get(dCode);
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
            var staff = db.Staffs.FirstOrDefault(i => i.Code == code); // Staff.Get(code);
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
                id = i.Code,
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
