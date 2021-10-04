using Amazon;
using Amazon.S3;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Resources;
using ShopNow.Base;

namespace ShopNow.Controllers
{
    //[Authorize]

    public class DeliveryBoyController : Controller
    {
        private sncEntities db = new sncEntities();
        UploadContent uc = new UploadContent();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private static readonly string bucketName = ConfigurationManager.AppSettings["BucketName"];
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.APSouth1;
        private static readonly string accesskey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static readonly string secretkey = ConfigurationManager.AppSettings["AWSSecretKey"];
       
        public DeliveryBoyController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<DeliveryBoy, DeliveryBoyListViewModel.DeliveryBoyList>();
                config.CreateMap<DeliveryBoyAddViewModel, DeliveryBoy>();
                config.CreateMap<DeliveryBoyEditViewModel, DeliveryBoy>();
                config.CreateMap<DeliveryBoy, DeliveryBoyEditViewModel>();
                config.CreateMap<DeliveryBoy, DeliveryBoyPlacesListViewModel>();
                 //config.CreateMap<DeliveryBoy, AgencyAssignViewModel>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNDBYL003")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            List<GetDeliveryBoy> list = new List<GetDeliveryBoy>();
            list = db.DeliveryBoys.Where(D => D.Status == 0)
                        .Select(m => new GetDeliveryBoy
                        {
                            Id = m.Id,
                            Name = m.Name,
                            PhoneNumber = m.PhoneNumber,
                            Active = m.Active,
                            ImagePath = m.ImagePath,
                            Email = m.Email
                        }).ToList();
            return View(list);
        }
        
        [AccessPolicy(PageCode = "SHNDBYIAL006")]
        public ActionResult InactiveList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DeliveryBoyListViewModel();
            model.List = model.List = db.DeliveryBoys.Where(i => i.Status == 1).Select(i => new DeliveryBoyListViewModel.DeliveryBoyList
            {
                Id = i.Id,
                Name = i.Name,
                PhoneNumber = i.PhoneNumber,
                ImagePath = i.ImagePath,
                Email = i.Email
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
        public ActionResult Create(DeliveryBoyAddViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var isExist = db.DeliveryBoys.Any(i => i.PhoneNumber == model.PhoneNumber && i.Status == 3);
            var customer = db.Customers.Where(i => i.PhoneNumber == model.PhoneNumber).FirstOrDefault();
            if (!isExist)
            {
                var deliveryboy = _mapper.Map<DeliveryBoyAddViewModel, DeliveryBoy>(model);
                if (customer != null)
                {
                    customer.Position = 3;
                    customer.DateUpdated = DateTime.Now;
                    db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    deliveryboy.CustomerId = customer.Id;
                    deliveryboy.CustomerName = customer.Name;
                }
                deliveryboy.CreatedBy = user.Name;
                deliveryboy.UpdatedBy = user.Name;
                deliveryboy.DateEncoded = DateTime.Now;
                deliveryboy.DateUpdated = DateTime.Now;
                deliveryboy.Status = 1;
                db.DeliveryBoys.Add(deliveryboy);
                db.SaveChanges();
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
                    db.Entry(deliveryboy).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
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
            else
            {
                var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.PhoneNumber == model.PhoneNumber);
                _mapper.Map(model, deliveryBoy);
                if (customer != null)
                {
                    customer.Position = 3;
                    customer.DateUpdated = DateTime.Now;
                    db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    deliveryBoy.CustomerId = customer.Id;
                    deliveryBoy.CustomerName = customer.Name;
                }
                deliveryBoy.Status = 1;
                deliveryBoy.DateEncoded = DateTime.Now;
                deliveryBoy.DateUpdated = DateTime.Now;
                try
                {
                    // DeliveryBoy Image
                    if (model.DeliveryBoyImage != null)
                    {
                        uc.UploadFiles(model.DeliveryBoyImage.InputStream, deliveryBoy.Id + "_" + model.DeliveryBoyImage.FileName, accesskey, secretkey, "image");
                        deliveryBoy.ImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + deliveryBoy.Id + "_" + model.DeliveryBoyImage.FileName.Replace(" ", "");
                    }

                    // DrivingLicense Image
                    if (model.DrivingLicenseImage != null)
                    {
                        uc.UploadFiles(model.DrivingLicenseImage.InputStream, deliveryBoy.Id + "_" + model.DrivingLicenseImage.FileName, accesskey, secretkey, "image");
                        deliveryBoy.DrivingLicenseImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + deliveryBoy.Id + "_" + model.DrivingLicenseImage.FileName.Replace(" ", "");
                    }

                    // DrivingLicense Pdf
                    if (model.DrivingLicensePdf != null)
                    {
                        uc.UploadFiles(model.DrivingLicensePdf.InputStream, deliveryBoy.Id + "_" + model.DrivingLicensePdf.FileName, accesskey, secretkey, "pdf");
                        deliveryBoy.DrivingLicenseImagePath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Uploads/" + deliveryBoy.Id + "_" + model.DrivingLicensePdf.FileName.Replace(" ", "");
                    }

                    // BankPassbook Image
                    if (model.BankPassbookImage != null)
                    {
                        uc.UploadFiles(model.BankPassbookImage.InputStream, deliveryBoy.Id + "_" + model.BankPassbookImage.FileName, accesskey, secretkey, "image");
                        deliveryBoy.BankPassbookPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + deliveryBoy.Id + "_" + model.BankPassbookImage.FileName.Replace(" ", "");
                    }

                    // BankPassbook Pdf
                    if (model.BankPassbookPdf != null)
                    {
                        uc.UploadFiles(model.BankPassbookPdf.InputStream, deliveryBoy.Id + "_" + model.BankPassbookPdf.FileName, accesskey, secretkey, "pdf");
                        deliveryBoy.BankPassbookPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Uploads/" + deliveryBoy.Id + "_" + model.BankPassbookPdf.FileName.Replace(" ", "");
                    }

                    // CV Pdf
                    if (model.CVPdf != null)
                    {
                        uc.UploadFiles(model.CVPdf.InputStream, deliveryBoy.Id + "_" + model.CVPdf.FileName, accesskey, secretkey, "pdf");
                        deliveryBoy.CVPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Uploads/" + deliveryBoy.Id + "_" + model.CVPdf.FileName.Replace(" ", "");
                    }
                    db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
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
        }

        [AccessPolicy(PageCode = "SHNDBYE002")]
        public ActionResult Edit(string Id)
        {
            var dCode = AdminHelpers.DCodeInt(Id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (dCode==0)
                return HttpNotFound();
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == dCode);
            var model = _mapper.Map<DeliveryBoy, DeliveryBoyEditViewModel>(deliveryBoy);
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
        public ActionResult Edit(DeliveryBoyEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            DeliveryBoy deliveryboy = db.DeliveryBoys.FirstOrDefault(i => i.Id == model.Id);
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
                db.Entry(deliveryboy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
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
        public ActionResult Details(string id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dId = AdminHelpers.DCodeInt(id);
            DeliveryBoy deliverBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == dId);
            var model = new DeliveryBoyEditViewModel();
            _mapper.Map(deliverBoy, model);
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNDBYR005")]
        public JsonResult Delete(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == Id);
            var customer = db.Customers.FirstOrDefault(i => i.Id == deliveryBoy.CustomerId);
            if (deliveryBoy != null)
            {
                deliveryBoy.Status = 2;
                deliveryBoy.UpdatedBy = user.Name;
                deliveryBoy.DateUpdated = DateTime.Now;
                deliveryBoy.DateUpdated = DateTime.Now;
                db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            if (customer != null)
            {
                customer.Position = 0;
                customer.DateUpdated = DateTime.Now;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult Clear(string Id)
        {
            var dCode = AdminHelpers.DCodeInt(Id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == dCode);
            if (deliveryBoy != null)
            {
                deliveryBoy.OnWork = 0;
                deliveryBoy.isAssign = 0;
                deliveryBoy.UpdatedBy = user.Name;
                deliveryBoy.DateUpdated = DateTime.Now;
                db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNDBYAP007")]
        public ActionResult Approve(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == Id);
            deliveryBoy.Status = 0;
            deliveryBoy.UpdatedBy = user.Name;
            deliveryBoy.DateUpdated = DateTime.Now;
            deliveryBoy.DateUpdated = DateTime.Now;
            db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            var fcmToken = (from c in db.Customers
                            where c.Id == deliveryBoy.CustomerId
                            select c.FcmTocken ?? "").FirstOrDefault().ToString();
            Helpers.PushNotification.SendbydeviceId("Your registration has been accepted. Go online to receive order.", "ShopNowChat", "a.mp3", fcmToken.ToString());

            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNDBYRE008")]
        public ActionResult Reject(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == Id);
            var customer = db.Customers.FirstOrDefault(i => i.Id == deliveryBoy.CustomerId);
            customer.Position = 0;
            customer.DateUpdated = DateTime.Now;
            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            
            deliveryBoy.Status = 3;
            deliveryBoy.UpdatedBy = user.Name;
            deliveryBoy.DateUpdated = DateTime.Now;
            deliveryBoy.DateUpdated = DateTime.Now;
            db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            var fcmToken = (from c in db.Customers
                            where c.Id == deliveryBoy.CustomerId
                            select c.FcmTocken ?? "").FirstOrDefault().ToString();
            Helpers.PushNotification.SendbydeviceId("Your registration has been rejected.", "ShopNowChat", "a.mp3", fcmToken.ToString());
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNDBYDA009")]
        public ActionResult DeActivate(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == Id);
            deliveryBoy.Status = 4;
            deliveryBoy.UpdatedBy = user.Name;
            deliveryBoy.DateUpdated = DateTime.Now;
            deliveryBoy.DateUpdated = DateTime.Now;
            db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNDBYAS010")]
        public ActionResult Assign(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            DeliveryBoy deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == Id);
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
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNDBYAS010")]
        public ActionResult Assign(DeliveryBoyPlacesListViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            DeliveryBoy deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == model.Id);
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
            }

            return RedirectToAction("List");
        }

        //[AccessPolicy(PageCode = "SHNDBYAF011")]
        //public ActionResult AssignFranchise()
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;            
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessPolicy(PageCode = "SHNDBYAF011")]
        //public ActionResult AssignFranchise(FranchiseViewModel model)
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == model.DeliveryBoyId);
        //    if (deliveryBoy != null)
        //    {
        //        deliveryBoy.MarketingAgentId = model.MarketingAgentId;
        //        deliveryBoy.MarketingAgentName = model.MarketingAgentName;
        //        deliveryBoy.DateUpdated = DateTime.Now;
        //        db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    return RedirectToAction("AssignedFranchiseList");
        //}

        //[AccessPolicy(PageCode = "SHNDBYFL013")]
        //public ActionResult AssignedFranchiseList()
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var model = new FranchiseViewModel();
        //    model.Lists = db.DeliveryBoys.Where(i => i.Status == 0 && i.MarketingAgentName != null)
        //        .Select(i=> new FranchiseListViewModel.FranchiseList
        //        {
        //            Name = i.Name,
        //            Id = i.Id,
        //            MarketingAgentId = i.MarketingAgentId,
        //            MarketingAgentName = i.MarketingAgentName
        //        }).OrderBy(i=> i.Name).ToList();
        //    return View(model.Lists);
        //}

        //[AccessPolicy(PageCode = "SHNDBYFU012")]
        //public ActionResult FranchiseUpdate(int id)
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    ViewBag.Name = user.Name;
        //    var model = new FranchiseViewModel();
        //    var deliveryboy = db.DeliveryBoys.FirstOrDefault(i => i.Id == id);
        //    _mapper.Map(deliveryboy, model);
        //    if (deliveryboy != null)
        //    {
        //        model.DeliveryBoyId = deliveryboy.Id;
        //        model.DeliveryBoyName = deliveryboy.Name;
        //    }
        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AccessPolicy(PageCode = "SHNDBYFU012")]
        //public ActionResult FranchiseUpdate(FranchiseViewModel model)
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == model.DeliveryBoyId);
        //    if (deliveryBoy != null)
        //    {
        //        deliveryBoy.MarketingAgentId = model.MarketingAgentId;
        //        deliveryBoy.MarketingAgentName = model.MarketingAgentName;
        //        deliveryBoy.UpdatedBy = user.Name;
        //        deliveryBoy.DateUpdated = DateTime.Now;
        //        deliveryBoy.DateUpdated = DateTime.Now;
        //        db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //    }

        //    return RedirectToAction("AssignedFranchiseList");
        //}

        [AccessPolicy(PageCode = "SHNDBYCA015")]
        public ActionResult CreditAmount()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DeliveryBoyCreditAmountViewModel();
            model.List = db.Orders.Where(i => i.Status == 6)
                .GroupBy(i => i.DeliveryBoyId)
                .Select(i => new DeliveryBoyCreditAmountViewModel.CreditAmountList
            {
                Id = i.Any() ? i.FirstOrDefault().Id : 0,
                DeliveryBoyId = i.Any() ? i.FirstOrDefault().DeliveryBoyId :0,
                DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "",
                GrossDeliveryCharge = i.Any() ? i.Sum(j=> j.DeliveryCharge) : 0.0
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

            model.List = db.Orders.Where(i => i.Status == 6 && i.DateUpdated.Year == date.Year && i.DateUpdated.Month == date.Month && i.DateUpdated.Day == date.Day)
                .GroupBy(i => i.DeliveryBoyId)
                .Select(i => new DeliveryBoyCreditAmountViewModel.CreditAmountList
                {
                    Id = i.Any() ? i.FirstOrDefault().Id : 0,
                    DeliveryBoyId = i.Any() ? i.FirstOrDefault().DeliveryBoyId : 0,
                    DeliveryBoyName = i.Any() ? i.FirstOrDefault().DeliveryBoyName : "",
                    GrossDeliveryCharge = i.Any() ? i.Sum(j => j.DeliveryCharge) : 0.0
                }).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNDBYFR014")]
        public JsonResult FranchiseRemove(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == id);

            if (deliveryBoy != null)
            {
                deliveryBoy.MarketingAgentId = 0;
                deliveryBoy.MarketingAgentName = null;
                deliveryBoy.DateUpdated = DateTime.Now;
                db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
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
            var shop = db.Shops.FirstOrDefault(i => i.PhoneNumber == PhoneNumber);
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
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNDBYC001")]
        public async Task<JsonResult> GetStaffSelect2(int ShopId)
        {
            var model = await db.Staffs.OrderBy(i => i.Name).Where(a => a.ShopId == ShopId && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNDBYAF011")]
        public async Task<JsonResult> GetDeliveryBoySelect2(string q = "")
        {
            var model = await db.DeliveryBoys.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.MarketingAgentName == null).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNDBYAF011")]
        public async Task<JsonResult> GetMarketingAgentSelect2(string q = "")
        {
            var model = await db.MarketingAgents.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        //[AccessPolicy(PageCode = "SHNDBYE002")]
        //public JsonResult GetDeliveryBoyShop(int code, double latitude, double longitude)
        //{
        //    var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == code);
        //    if (deliveryBoy.Latitude == latitude && deliveryBoy.Longitude == longitude)
        //    {
        //        return Json(true, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(false, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[AccessPolicy(PageCode = "SHNDBYE002")]
        //public JsonResult DeactivateDeliveryBoyShop(int code)
        //{
        //    var user = ((Helpers.Sessions.User)Session["USER"]);
        //    var list = db.DeliveryBoyShops.Where(i => i.DeliveryBoyId == code && i.Status == 0).ToList();
        //    if (list != null)
        //    {
        //        foreach (var dbs in list)
        //        {
        //            var dbshops = db.DeliveryBoyShops.FirstOrDefault(i => i.Id == dbs.Id);// DeliveryBoyShop.Get(dbs.Code);
        //            dbshops.Status = 4;
        //            dbshops.DateUpdated = DateTime.Now;
        //            dbshops.UpdatedBy = user.Name;
        //            dbshops.DateUpdated = DateTime.Now;
        //            db.Entry(dbshops).State = System.Data.Entity.EntityState.Modified;
        //            db.SaveChanges();
        //        }
        //    }
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        [AccessPolicy(PageCode = "SHNDBYE002")]
        public JsonResult VerifyImage(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var dboy = db.DeliveryBoys.FirstOrDefault(i => i.Id == Id);
            dboy.DateUpdated = DateTime.Now;
            dboy.UpdatedBy = user.Name;
            db.Entry(dboy).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { data = true, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNDBYC001")]
        public JsonResult GetPhoneNumberCheck(string phone)
        {
            var customer = db.Customers.FirstOrDefault(i => i.PhoneNumber == phone);
            int msg;
            var customerExist = db.Customers.Any(i => i.PhoneNumber == phone);
            if (customerExist)
            {
                var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.PhoneNumber == phone);
                if (deliveryBoy != null)
                {
                    if (deliveryBoy.Status == 0)    // DeliveryBoy already Exist
                    {
                        msg = 1;
                        return Json(msg, JsonRequestBehavior.AllowGet);
                    }
                    else if (deliveryBoy.Status == 1)   // DeliveryBoy in approval Pending status
                    {
                        msg = 2;
                        return Json(msg, JsonRequestBehavior.AllowGet);
                    }
                    else if (deliveryBoy.Status == 3 || deliveryBoy.Status == 2) // DeliveryBoy Update
                    {
                        msg = 3;
                        return Json(new { msg, phone = customer.PhoneNumber, name = customer.Name, email = customer.Email }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {     // DeliveryBoy Create
                    msg = 4;
                    return Json(new { msg, phone = customer.PhoneNumber, name = customer.Name, email = customer.Email }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {   // Not a Customer
                return Json(msg = 0, JsonRequestBehavior.AllowGet);
            }
            return Json(msg = 0, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateDeliveryBoyOnline(int Id, int Active)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var dboy = db.DeliveryBoys.Where(i => i.Id == Id && i.Status == 0).FirstOrDefault();
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
