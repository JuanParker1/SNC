using Amazon;
using Amazon.S3;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    //[Authorize]

    public class DeliveryBoyController : Controller
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
        private static string _generatedCode(string _prefix)
        {

            return ShopNow.Helpers.DRC.Generate(_prefix);

        }

        public DeliveryBoyController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<DeliveryBoy, DeliveryBoyListViewModel.DeliveryBoyList>();
                config.CreateMap<DeliveryBoyCreateEditViewModel, DeliveryBoy>();
                config.CreateMap<DeliveryBoy, DeliveryBoyCreateEditViewModel>();
                config.CreateMap<DeliveryBoy, DeliveryBoyPlacesListViewModel>();
                config.CreateMap<DeliveryBoy, FranchiseViewModel>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNDBYL003")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from s in db.DeliveryBoys
                        select s).OrderBy(s => s.Name).Where(i => i.Status == 0).ToList();
            return View(List);
        }
        
        [AccessPolicy(PageCode = "SHNDBYIAL006")]
        public ActionResult InactiveList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DeliveryBoyListViewModel();

            model.List = model.List = db.DeliveryBoys.Where(i => i.Status == 1 || i.Status == 5).Select(i => new DeliveryBoyListViewModel.DeliveryBoyList
            {
                Id = i.Id,
                Name = i.Name,
                PhoneNumber = i.PhoneNumber,
                ImagePath = i.ImagePath,
               // ShopId = i.ShopId,
               // ShopName = i.ShopName
            }).ToList();

            return View(model);
        }

        [AccessPolicy(PageCode = "SHNDBYC001")]
        public ActionResult Create()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNDBYC001")]
        public ActionResult Create(DeliveryBoyCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);

            var deliveryboy = _mapper.Map<DeliveryBoyCreateEditViewModel, DeliveryBoy>(model);
            //if (model.Name == null && model.StaffCode != null)
            //{
            //    deliveryboy.Name = model.StaffName;
            //}
            var dboy = db.Customers.Where(i => i.PhoneNumber == model.PhoneNumber).FirstOrDefault();
            if (dboy != null)
            {
                dboy.Position = 3;
                dboy.DateUpdated = DateTime.Now;
                db.Entry(dboy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                deliveryboy.CustomerId = dboy.Id;
                deliveryboy.CustomerName = dboy.Name;
            }

            deliveryboy.CreatedBy = user.Name;
            deliveryboy.UpdatedBy = user.Name;
            deliveryboy.DateEncoded = DateTime.Now;
            deliveryboy.DateUpdated = DateTime.Now;
            deliveryboy.Status = 1;
            db.DeliveryBoys.Add(deliveryboy);
            db.SaveChanges();
            deliveryboy.Id = deliveryboy.Id;// DeliveryBoy.Add(deliveryboy, out int error);
            try
            {
                var deliveryBoyImage = db.DeliveryBoys.FirstOrDefault(i => i.Id == deliveryboy.Id);//DeliveryBoy.Get(deliveryboy.Code);
                // DeliveryBoy Image
                if (model.DeliveryBoyImage != null)
                {
                    uc.UploadFiles(model.DeliveryBoyImage.InputStream, deliveryboy.Id + "_" + model.DeliveryBoyImage.FileName, accesskey, secretkey, "image");
                    deliveryBoyImage.ImagePath = deliveryboy.Id + "_" + model.DeliveryBoyImage.FileName.Replace(" ", "");
                }

                // DrivingLicense Image
                if (model.DrivingLicenseImage != null)
                {
                    uc.UploadFiles(model.DrivingLicenseImage.InputStream, deliveryboy.Id + "_" + model.DrivingLicenseImage.FileName, accesskey, secretkey, "image");
                    deliveryBoyImage.DrivingLicenseImagePath = deliveryboy.Id + "_" + model.DrivingLicenseImage.FileName.Replace(" ", "");
                }

                // DrivingLicense Pdf
                if (model.DrivingLicensePdf != null)
                {
                    uc.UploadFiles(model.DrivingLicensePdf.InputStream, deliveryboy.Id + "_" + model.DrivingLicensePdf.FileName, accesskey, secretkey, "pdf");
                    deliveryBoyImage.DrivingLicenseImagePath = deliveryboy.Id + "_" + model.DrivingLicensePdf.FileName.Replace(" ", "");
                }

                // BankPassbook Image
                if (model.BankPassbookImage != null)
                {
                    uc.UploadFiles(model.BankPassbookImage.InputStream, deliveryboy.Id + "_" + model.BankPassbookImage.FileName, accesskey, secretkey, "image");
                    deliveryBoyImage.BankPassbookPath = deliveryboy.Id + "_" + model.BankPassbookImage.FileName.Replace(" ", "");
                }

                // BankPassbook Pdf
                if (model.BankPassbookPdf != null)
                {
                    uc.UploadFiles(model.BankPassbookPdf.InputStream, deliveryboy.Id + "_" + model.BankPassbookPdf.FileName, accesskey, secretkey, "pdf");
                    deliveryBoyImage.BankPassbookPath = deliveryboy.Id + "_" + model.BankPassbookPdf.FileName.Replace(" ", "");
                }

                // CV Pdf
                if (model.CVPdf != null)
                {
                    uc.UploadFiles(model.CVPdf.InputStream, deliveryboy.Id + "_" + model.CVPdf.FileName, accesskey, secretkey, "pdf");
                    deliveryBoyImage.CVPath = deliveryboy.Id + "_" + model.CVPdf.FileName.Replace(" ", "");
                }
                deliveryBoyImage.DateUpdated = DateTime.Now;
                db.Entry(deliveryBoyImage).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                // DeliveryBoy.Edit(deliveryBoyImage, out int error1);
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

        [AccessPolicy(PageCode = "SHNDBYE002")]
        public ActionResult Edit(string code)
        {
            var dCode = AdminHelpers.DCodeInt(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (dCode==0)
                return HttpNotFound();
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == dCode);// DeliveryBoy.Get(dCode);
            var model = _mapper.Map<DeliveryBoy, DeliveryBoyCreateEditViewModel>(deliveryBoy);
           // model.DeliveryBoyShopCount = db.DeliveryBoyShops.Where(i => i.DeliveryBoyCode == deliveryBoy.Code && i.Status == 0).Count();
            if (model.Status == 1)
            {
                int count = 0;
                if (model.DrivingLicenseImagePath != null) { count++; }
                if (model.BankPassbookPath != null) { count++; }
                model.Count = count;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNDBYE002")]
        public ActionResult Edit(DeliveryBoyCreateEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);

            DeliveryBoy deliveryboy = db.DeliveryBoys.FirstOrDefault(i => i.Id == model.Id);// DeliveryBoy.Get(model.Code);
            _mapper.Map(model, deliveryboy);

            //if (model.Name == null && model.StaffCode != null)
            //{
            //    deliveryboy.Name = model.StaffName;
            //}

            deliveryboy.DateUpdated = DateTime.Now;
            deliveryboy.UpdatedBy = user.Name;
            deliveryboy.DateUpdated = DateTime.Now;
            db.Entry(deliveryboy).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            // DeliveryBoy.Edit(deliveryboy, out int errorCode);

            try
            {
                var deliveryBoyImage = db.DeliveryBoys.FirstOrDefault(i => i.Id == deliveryboy.Id); //DeliveryBoy.Get(deliveryboy.Code);
                // DeliveryBoy Image
                if (model.DeliveryBoyImage != null)
                {
                    uc.UploadFiles(model.DeliveryBoyImage.InputStream, deliveryboy.Id + "_" + model.DeliveryBoyImage.FileName, accesskey, secretkey, "image");
                    deliveryBoyImage.ImagePath = deliveryboy.Id + "_" + model.DeliveryBoyImage.FileName.Replace(" ", "");
                }

                // DrivingLicense Image
                if (model.DrivingLicenseImage != null)
                {
                    uc.UploadFiles(model.DrivingLicenseImage.InputStream, deliveryboy.Id + "_" + model.DrivingLicenseImage.FileName, accesskey, secretkey, "image");
                    deliveryBoyImage.DrivingLicenseImagePath = deliveryboy.Id + "_" + model.DrivingLicenseImage.FileName.Replace(" ", "");
                }

                // DrivingLicense Pdf
                if (model.DrivingLicensePdf != null)
                {
                    uc.UploadFiles(model.DrivingLicensePdf.InputStream, deliveryboy.Id + "_" + model.DrivingLicensePdf.FileName, accesskey, secretkey, "pdf");
                    deliveryBoyImage.DrivingLicenseImagePath = deliveryboy.Id + "_" + model.DrivingLicensePdf.FileName.Replace(" ", "");
                }

                // BankPassbook Image
                if (model.BankPassbookImage != null)
                {
                    uc.UploadFiles(model.BankPassbookImage.InputStream, deliveryboy.Id + "_" + model.BankPassbookImage.FileName, accesskey, secretkey, "image");
                    deliveryBoyImage.BankPassbookPath = deliveryboy.Id + "_" + model.BankPassbookImage.FileName.Replace(" ", "");
                }

                // BankPassbook Pdf
                if (model.BankPassbookPdf != null)
                {
                    uc.UploadFiles(model.BankPassbookPdf.InputStream, deliveryboy.Id + "_" + model.BankPassbookPdf.FileName, accesskey, secretkey, "pdf");
                    deliveryBoyImage.BankPassbookPath = deliveryboy.Id + "_" + model.BankPassbookPdf.FileName.Replace(" ", "");
                }

                // CV Pdf
                if (model.CVPdf != null)
                {
                    uc.UploadFiles(model.CVPdf.InputStream, deliveryboy.Id + "_" + model.CVPdf.FileName, accesskey, secretkey, "pdf");
                    deliveryBoyImage.CVPath = deliveryboy.Id + "_" + model.CVPdf.FileName.Replace(" ", "");
                }
                deliveryBoyImage.DateUpdated = DateTime.Now;
                db.Entry(deliveryBoyImage).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //DeliveryBoy.Edit(deliveryBoyImage, out int error);
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

        [AccessPolicy(PageCode = "SHNDBYD004")]
        public ActionResult Details(string code)
        {
            var dCode = AdminHelpers.DCodeInt(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            DeliveryBoy deliverBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == dCode); //DeliveryBoy.Get(dCode);
            var model = new DeliveryBoyCreateEditViewModel();
            _mapper.Map(deliverBoy, model);
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNDBYR005")]
        public ActionResult Delete(string code)
        {
            var dCode = AdminHelpers.DCodeInt(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == dCode);
            var customer = db.Customers.FirstOrDefault(i => i.Id == deliveryBoy.CustomerId);
            if (customer != null)
            {
                customer.Position = 0;
                customer.DateUpdated = DateTime.Now;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            if (deliveryBoy != null)
            {
                deliveryBoy.Status = 2;
                deliveryBoy.UpdatedBy = user.Name;
                deliveryBoy.DateUpdated = DateTime.Now;
                deliveryBoy.DateUpdated = DateTime.Now;
                db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNDBYAP007")]
        public ActionResult Approve(int code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == code);//DeliveryBoy.Get(code);
            deliveryBoy.Status = 0;
            deliveryBoy.UpdatedBy = user.Name;
            deliveryBoy.DateUpdated = DateTime.Now;
            deliveryBoy.DateUpdated = DateTime.Now;
            db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            // DeliveryBoy.Edit(deliveryBoy, out int error);

            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNDBYRE008")]
        public ActionResult Reject(int code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == code);// DeliveryBoy.Get(code);
            var customer = db.Customers.FirstOrDefault(i => i.Id == deliveryBoy.CustomerId);// Customer.Get(deliveryBoy.CustomerCode);
            customer.Position = 0;
            customer.DateUpdated = DateTime.Now;
            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

           // Customer.Edit(customer, out int errors);
            deliveryBoy.Status = 3;
            deliveryBoy.UpdatedBy = user.Name;
            deliveryBoy.DateUpdated = DateTime.Now;
            deliveryBoy.DateUpdated = DateTime.Now;
            db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            // DeliveryBoy.Edit(deliveryBoy, out int error);

            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNDBYDA009")]
        public ActionResult DeActivate(int code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == code);// DeliveryBoy.Get(code);
            deliveryBoy.Status = 4;
            deliveryBoy.UpdatedBy = user.Name;
            deliveryBoy.DateUpdated = DateTime.Now;
            deliveryBoy.DateUpdated = DateTime.Now;
            db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            // DeliveryBoy.Edit(deliveryBoy, out int error);

            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNDBYAS010")]
        public ActionResult Assign(int code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            DeliveryBoy deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == code);// DeliveryBoy.Get(code);
            var model = new DeliveryBoyPlacesListViewModel();

            _mapper.Map(deliveryBoy, model);

            model.List = db.Shops.AsEnumerable().Select(i => new DeliveryBoyPlacesListViewModel.Places
            {
                Id = i.Id,
                Name = i.Name,
                PhoneNumber = i.PhoneNumber,
                ImagePath = i.ImagePath,
                Address = i.Address,
                Latitude = i.Latitude,
                Longitude = i.Longitude,
                Status = i.Status,
                Meters = (((Math.Acos(Math.Sin((deliveryBoy.Latitude * Math.PI / 180)) * Math.Sin((i.Latitude * Math.PI / 180)) + Math.Cos((deliveryBoy.Latitude * Math.PI / 180)) * Math.Cos((i.Latitude * Math.PI / 180))
                * Math.Cos(((deliveryBoy.Longitude - i.Longitude) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344)
            }).Where(i => i.Meters < 8000 && i.Status == 0).ToList();

            model.DeliveryBoyShopCount = db.DeliveryBoyShops.Where(i => i.DeliveryBoyId == deliveryBoy.Id && i.Status == 0).Count();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNDBYAS010")]
        public ActionResult Assign(DeliveryBoyPlacesListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            DeliveryBoy deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == model.Id);// DeliveryBoy.Get(model.Code);

            model.List = db.Shops.AsEnumerable().Select(i => new DeliveryBoyPlacesListViewModel.Places
            {
                Id = i.Id,
                Name = i.Name,
                PhoneNumber = i.PhoneNumber,
                ImagePath = i.ImagePath,
                Address = i.Address,
                Latitude = i.Latitude,
                Longitude = i.Longitude,
                Status = i.Status,
                Meters = (((Math.Acos(Math.Sin((deliveryBoy.Latitude * Math.PI / 180)) * Math.Sin((i.Latitude * Math.PI / 180)) + Math.Cos((deliveryBoy.Latitude * Math.PI / 180)) * Math.Cos((i.Latitude * Math.PI / 180))
                * Math.Cos(((deliveryBoy.Longitude - i.Longitude) * Math.PI / 180)))) * 180 / Math.PI) * 60 * 1.1515 * 1609.344)
            }).Where(i => i.Meters < 8000 && i.Status == 0).ToList();

            DeliveryBoyShop dbs = new DeliveryBoyShop();
            foreach (var s in model.List)
            {
                dbs.DeliveryBoyId = deliveryBoy.Id;
                dbs.DeliveryBoyName = deliveryBoy.Name;
                dbs.ShopId = s.Id;
                dbs.ShopName = s.Name;
                dbs.PhoneNumber = s.PhoneNumber;
                dbs.Address = s.Address;
                dbs.Latitude = s.Latitude;
                dbs.Longitude = s.Longitude;
                dbs.CreatedBy = user.Name;
                dbs.UpdatedBy = user.Name;
                dbs.DateEncoded = DateTime.Now;
                dbs.DateUpdated = DateTime.Now;
                db.DeliveryBoyShops.Add(dbs);
                db.SaveChanges();
                //DeliveryBoyShop.Add(dbs, out int error);
            }

            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNDBYAF011")]
        public ActionResult AssignFranchise()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNDBYAF011")]
        public ActionResult AssignFranchise(FranchiseViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == model.DeliveryBoyId);// DeliveryBoy.Get(model.DeliveryBoyCode);
            if (deliveryBoy != null)
            {
                deliveryBoy.MarketingAgentId = model.MarketingAgentId;
                deliveryBoy.MarketingAgentName = model.MarketingAgentName;
                deliveryBoy.DateUpdated = DateTime.Now;
                db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //DeliveryBoy.Edit(deliveryBoy, out int error);
            }

            return RedirectToAction("AssignedFranchiseList");
        }

        [AccessPolicy(PageCode = "SHNDBYFL013")]
        public ActionResult AssignedFranchiseList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new FranchiseViewModel();
            model.Lists = db.DeliveryBoys.Where(i => i.Status == 0 && i.MarketingAgentId != null && i.MarketingAgentName != null)
                .Select(i=> new FranchiseViewModel.FranchiseList
                {
                    Name = i.Name,
                    Id = i.Id,
                    MarketingAgentId = i.MarketingAgentId,
                    MarketingAgentName = i.MarketingAgentName
                }).OrderBy(i=> i.Name).ToList();
            return View(model.Lists);
        }

        [AccessPolicy(PageCode = "SHNDBYFU012")]
        public ActionResult FranchiseUpdate(int code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new FranchiseViewModel();
            var deliveryboy = db.DeliveryBoys.FirstOrDefault(i => i.Id == code);// DeliveryBoy.Get(code);
            _mapper.Map(deliveryboy, model);
            if (deliveryboy != null)
            {
                model.DeliveryBoyId = deliveryboy.Id;
                model.DeliveryBoyName = deliveryboy.Name;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNDBYFU012")]
        public ActionResult FranchiseUpdate(FranchiseViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == model.DeliveryBoyId);// DeliveryBoy.Get(model.DeliveryBoyCode);
            if (deliveryBoy != null)
            {
                deliveryBoy.MarketingAgentId = model.MarketingAgentId;
                deliveryBoy.MarketingAgentName = model.MarketingAgentName;
                deliveryBoy.UpdatedBy = user.Name;
                deliveryBoy.DateUpdated = DateTime.Now;
                deliveryBoy.DateUpdated = DateTime.Now;
                db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //DeliveryBoy.Edit(deliveryBoy, out int error);
            }

            return RedirectToAction("AssignedFranchiseList");
        }

        [AccessPolicy(PageCode = "SHNDBYCA015")]
        public ActionResult CreditAmount()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DeliveryBoyCreditAmountViewModel();
            model.List = db.ShopCharges.Where(i => i.CartStatus == 6)
                .GroupBy(i => i.DeliveryBoyCode)
                .Select(i => new DeliveryBoyCreditAmountViewModel.CreditAmountList
            {
                Code = i.Any() ? i.FirstOrDefault().Code : "",
                DeliveryBoyCode = i.Any() ? i.FirstOrDefault().DeliveryBoyCode : "",
                DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "",
                GrossDeliveryCharge = i.Any() ? i.Sum(j=> j.GrossDeliveryCharge) : 0.0
            }).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNDBYTC016")]
        public ActionResult TodayCreditAmount()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DeliveryBoyCreditAmountViewModel();
            var date = DateTime.Now;
            model.List = db.ShopCharges.Where(i => i.CartStatus == 6 && i.DateUpdated.Year == date.Year && i.DateUpdated.Month == date.Month && i.DateUpdated.Day == date.Day)
                .GroupBy(i => i.DeliveryBoyCode)
                .Select(i => new DeliveryBoyCreditAmountViewModel.CreditAmountList
                {
                    Code = i.Any() ? i.FirstOrDefault().Code : "",
                    DeliveryBoyCode = i.Any() ? i.FirstOrDefault().DeliveryBoyCode : "",
                    DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "",
                    GrossDeliveryCharge = i.Any() ? i.Sum(j => j.GrossDeliveryCharge) : 0.0
                }).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNDBYFR014")]
        public JsonResult FranchiseRemove(string code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Code == code);// DeliveryBoy.Get(code);
            if (deliveryBoy != null)
            {
                deliveryBoy.MarketingAgentId = null;
                deliveryBoy.MarketingAgentName = null;
                deliveryBoy.DateUpdated = DateTime.Now;
                db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                // DeliveryBoy.Edit(deliveryBoy, out int error);
                IsAdded = true;
            }
            return Json(new { IsAdded = IsAdded }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNDBYC001")]
        public JsonResult ExistMobileNumber(string PhoneNumber = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsExist = false;
            string message = "";
            var shop = db.Shops.FirstOrDefault(i => i.PhoneNumber == PhoneNumber);// Shop.GetPhoneNumber(PhoneNumber);
            if (shop == null)
            {
                IsExist = false;
            }
            else
            {
                IsExist = true;
                message = PhoneNumber + " Already Exist!";
            }
            return Json(new { IsExist = IsExist, message = message }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNDBYC001")]
        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNDBYC001")]
        public async Task<JsonResult> GetStaffSelect2(string ShopId)
        {
            var model = await db.Staffs.OrderBy(i => i.Name).Where(a => a.ShopId == ShopId && a.Status == 0).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNDBYAF011")]
        public async Task<JsonResult> GetDeliveryBoySelect2(string q = "")
        {
            var model = await db.DeliveryBoys.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.MarketingAgentId == null && a.MarketingAgentName == null).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNDBYAF011")]
        public async Task<JsonResult> GetMarketingAgentSelect2(string q = "")
        {
            var model = await db.MarketingAgents.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNDBYE002")]
        public JsonResult GetDeliveryBoyShop(string code, double latitude, double longitude)
        {
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Code == code);//DeliveryBoy.Get(code);
            if (deliveryBoy.Latitude == latitude && deliveryBoy.Longitude == longitude)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        [AccessPolicy(PageCode = "SHNDBYE002")]
        public JsonResult DeactivateDeliveryBoyShop(string code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var list = db.DeliveryBoyShops.Where(i => i.DeliveryBoyCode == code && i.Status == 0).ToList();
            if (list != null)
            {
                foreach (var dbs in list)
                {
                    var dbshops = db.DeliveryBoyShops.FirstOrDefault(i => i.Code == dbs.Code);// DeliveryBoyShop.Get(dbs.Code);
                    dbshops.Status = 4;
                    dbshops.DateUpdated = DateTime.Now;
                    dbshops.UpdatedBy = user.Name;
                    dbshops.DateUpdated = DateTime.Now;
                    db.Entry(dbshops).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                  //  DeliveryBoyShop.Edit(dbshops, out int error);
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNDBYE002")]
        public JsonResult VerifyImage(string code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var dboy = db.DeliveryBoys.FirstOrDefault(i => i.Code == code); // DeliveryBoy.Get(code);
            dboy.Status = 5;
            dboy.DateUpdated = DateTime.Now;
            dboy.UpdatedBy = user.Name;
            db.Entry(dboy).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { data = true, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNDBYC001")]
        public JsonResult GetPhoneNumberCheck(string phone)
        {
            var dboy = db.DeliveryBoys.FirstOrDefault(i => i.PhoneNumber == phone && (i.Status == 0 || i.Status == 1));
            var customer = db.Customers.FirstOrDefault(i => i.PhoneNumber == phone);
            int isCheck = 0;
            if (customer != null && dboy == null)
            {
                return Json(isCheck = 1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (customer == null)
                    return Json(isCheck = 2, JsonRequestBehavior.AllowGet);
                else if(dboy != null)
                    return Json(isCheck = 3, JsonRequestBehavior.AllowGet);
            }
            return Json(isCheck = 0, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateDeliveryBoyOnline(string code, int Active)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var dboy = db.DeliveryBoys.Where(i => i.Code == code && i.Status == 0).FirstOrDefault();
            dboy.Active = Active;
            db.Entry(dboy).State = System.Data.Entity.EntityState.Modified;
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
