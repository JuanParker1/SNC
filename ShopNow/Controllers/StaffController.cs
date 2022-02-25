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
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    //[Authorize]

    public class StaffController : Controller
    {
        private sncEntities db = new sncEntities();
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

        public StaffController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<Staff, StaffListViewModel.StaffList> ();
                config.CreateMap<StaffCreateEditViewModel, Staff>();
                config.CreateMap<Staff, StaffCreateEditViewModel>();
                config.CreateMap<Shop, StaffCreateEditViewModel.ShopList>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SNCSFL279")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new StaffListViewModel();

            int[] shop = db.Shops.Where( i=> i.Status == 0).Select(s => s.Id).ToArray();
            int[] staffshop = db.Staffs.Join(db.ShopStaffs, s => s.Id, ss => ss.StaffId, (s, ss) => new { s, ss }).Select(i => i.ss.ShopId).ToArray();
           
            if (shop != null && shop.Length > 0)
            {
                model.List = db.Staffs.Where(i => i.Status == 0)
                 .Join(db.ShopStaffs.Where(i => shop.Contains(i.ShopId)), st => st.Id, ss => ss.StaffId, (st, ss) => new { st, ss })
                 .Join(db.Shops, st => st.ss.ShopId, sh => sh.Id, (st, sh) => new { st, sh })
                 .GroupBy(i => i.st.ss.StaffId)
                 .AsEnumerable()
                 .Select(i => new StaffListViewModel.StaffList
                 {
                     Id = i.FirstOrDefault().st.st.Id,
                     ImagePath = i.FirstOrDefault().st.st.ImagePath,
                     Name = i.FirstOrDefault().st.st.Name,
                     PhoneNumber = i.FirstOrDefault().st.st.PhoneNumber,
                     ShopName = string.Join(", ", i.Select(a => a.sh.Name))
                 }).ToList();
                return View(model.List);
            }

            if (staffshop != null)
            {
                model.List = db.Staffs.Where(i => i.Status == 0)
                 .Join(db.ShopStaffs.Where(i => staffshop.Contains(i.ShopId)), st => st.Id, ss => ss.StaffId, (st, ss) => new { st, ss })
                 .Join(db.Shops, st => st.ss.ShopId, sh => sh.Id, (st, sh) => new { st, sh })
                 .GroupBy(i => i.st.ss.StaffId)
                 .AsEnumerable()
                 .Select(i => new StaffListViewModel.StaffList
                 {
                     Id = i.FirstOrDefault().st.st.Id,
                     ImagePath = i.FirstOrDefault().st.st.ImagePath,
                     Name = i.FirstOrDefault().st.st.Name,
                     PhoneNumber = i.FirstOrDefault().st.st.PhoneNumber,
                     ShopName = string.Join(", ", i.Select(a => a.sh.Name))
                 }).ToList();
                return View(model.List);
            }
            return View();
        }

        [AccessPolicy(PageCode = "SNCSFC280")]
        public ActionResult Create()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCSFC280")]
        public ActionResult Create(StaffCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var staff = _mapper.Map<StaffCreateEditViewModel, Staff>(model);
            staff.CreatedBy = user.Name;
            staff.UpdatedBy = user.Name;
            staff.Status = 0;
            staff.DateEncoded = DateTime.Now;
            staff.DateUpdated = DateTime.Now;
            try
            {
                // Staff Image
                if (model.StaffImage != null)
                {
                    uc.UploadFiles(model.StaffImage.InputStream, staff.Id + "_" + model.StaffImage.FileName, accesskey, secretkey, "image");
                    staff.ImagePath = staff.Id + "_" + model.StaffImage.FileName.Replace(" ", "");
                }
                db.Staffs.Add(staff);
                db.SaveChanges();

                // ShopStaff
                if (staff != null && model.ShopIds != null)
                {
                    var customer = new Customer
                    {
                        Name = model.Name,
                        PhoneNumber = model.PhoneNumber,
                        Position = 2,
                        CreatedBy = user.Name,
                        UpdatedBy = user.Name,
                        DateEncoded = DateTime.Now,
                        DateUpdated = DateTime.Now
                    };
                    db.Customers.Add(customer);
                    db.SaveChanges();

                    foreach (var item in model.ShopIds)
                    {
                        ShopStaff ss = new ShopStaff();
                        ss.ShopId = item;
                        ss.StaffId = staff.Id;
                        ss.StaffCustomerId = customer.Id;
                        db.ShopStaffs.Add(ss);
                        db.SaveChanges();
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

        [AccessPolicy(PageCode = "SNCSFE281")]
        public ActionResult Edit(string Id)
        {
            var dCode = AdminHelpers.DCodeInt(Id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (dCode==0)
                return HttpNotFound();
            var staff = db.Staffs.FirstOrDefault(i => i.Id == dCode);
            var model = _mapper.Map<Staff, StaffCreateEditViewModel>(staff);
            if (staff != null)
            {
                model.ShopIds = db.ShopStaffs.Where(i => i.StaffId == staff.Id).Select(i => i.ShopId).ToArray();
                model.ShopId = String.Join(",", model.ShopIds);
                if (model.ShopIds != null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var s in model.ShopIds)
                    {
                        
                        var sid = Convert.ToInt32(s);
                        var sh = db.Shops.FirstOrDefault(i => i.Id == sid && i.Status == 0);
                        sb.Append(sh.Name);
                        sb.Append(",");
                    }
                    if (sb.Length >= 1)
                    {
                        model.ShopNames = sb.ToString().Remove(sb.Length - 1);
                    }
                    else
                    {
                        model.ShopNames = sb.ToString();
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCSFE281")]
        public ActionResult Edit(StaffCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            Staff staff = db.Staffs.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, staff);

            staff.DateUpdated = DateTime.Now;
            staff.UpdatedBy = user.Name;

            try
            {
                // Staff Image
                if (model.StaffImage != null)
                {
                    uc.UploadFiles(model.StaffImage.InputStream, staff.Id + "_" + model.StaffImage.FileName, accesskey, secretkey, "image");
                    staff.ImagePath = staff.Id + "_" + model.StaffImage.FileName.Replace(" ", "");
                }
                db.Entry(staff).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                // ShopStaff
                if (staff != null && model.ShopIds != null)
                {
                    var shopStafflist = db.ShopStaffs.Where(i => i.StaffId == staff.Id).ToList();
                    foreach (var sid in model.ShopIds)
                    {
                        var shopStaff = db.ShopStaffs.Where(i=> i.ShopId == sid && i.StaffId == staff.Id).FirstOrDefault();
                        if (shopStaff == null)
                        {
                            ShopStaff ss = new ShopStaff();
                            ss.ShopId = sid;
                            ss.StaffId = staff.Id;
                            db.ShopStaffs.Add(ss);
                            db.SaveChanges();
                        }
                    }

                    var removeShop = shopStafflist.Where(i => !model.ShopIds.Contains(i.ShopId)).ToList();
                    if (removeShop != null)
                    {
                        db.ShopStaffs.RemoveRange(removeShop);
                        db.SaveChanges();
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

        [AccessPolicy(PageCode = "SNCSFDT282")]
        public ActionResult Details(string Id)
        {
            var dCode = AdminHelpers.DCodeInt(Id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            Staff staff = db.Staffs.FirstOrDefault(i => i.Id == dCode);
            var model = new StaffCreateEditViewModel();
            _mapper.Map(staff, model);
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCSFD283")]
        public JsonResult Delete(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var staff = db.Staffs.FirstOrDefault(i => i.Id == Id);
            if (staff != null)
            {
                staff.Status = 2;
                staff.DateUpdated = DateTime.Now;
                staff.UpdatedBy = user.Name;
                db.Entry(staff).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GeneratePassword(int Id)
        {
            var staff = db.Staffs.FirstOrDefault(i => i.Id == Id);
            if (staff != null)
            {
                staff.Password = _generatedPassword;
                staff.DateUpdated = DateTime.Now;
                db.Entry(staff).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new { data = staff.Password, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
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
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
