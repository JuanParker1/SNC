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

    public class AgencyController : Controller
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

        public AgencyController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<AgencyCreateViewModel, Agency>();
                config.CreateMap<Agency, AgencyEditViewModel>();
                config.CreateMap<AgencyEditViewModel, Agency>();
                config.CreateMap<Agency, AgencyListViewModel>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        public ActionResult List()
        {
            var model = new AgencyListViewModel();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.List = db.Agencies.Where(i => i.Status == 0).Select(i => new AgencyListViewModel.AgencyList
            {
                Id = i.Id,
                Email = i.Email,
                Name = i.Name,
                PhoneNumber = i.PhoneNumber
            }).OrderBy(i => i.Name).ToList();

            return View(model.List);
        }

        public ActionResult InActiveList()
        {
            var model = new AgencyListViewModel();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.List = db.Agencies.Where(i => i.Status == 1).Select(i => new AgencyListViewModel.AgencyList
            {
                Id = i.Id,
                Email = i.Email,
                Name = i.Name,
                PhoneNumber = i.PhoneNumber
            }).OrderBy(i => i.Name).ToList();

            return View(model.List);
        }

        public ActionResult Create()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "")]
        public ActionResult Create(AgencyCreateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var agency = _mapper.Map<AgencyCreateViewModel, Agency>(model);
            agency.Status = 1;
            agency.DateEncoded = DateTime.Now;
            agency.DateUpdated = DateTime.Now;
            agency.CreatedBy = user.Name;
            agency.UpdatedBy = user.Name;
            db.Agencies.Add(agency);
            db.SaveChanges();

            var customer = db.Customers.FirstOrDefault(i => i.Id == model.CustomerId);
            if(customer != null)
            {
                customer.Position = 5;   // Agency
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            try
            {
                var agencyImage = db.Agencies.FirstOrDefault(i => i.Id == agency.Id);
                // Agency Image
                if (model.AgencyImage != null)
                {
                    uc.UploadFiles(model.AgencyImage.InputStream, agency.Id + "_" + model.AgencyImage.FileName, accesskey, secretkey, "image");
                    agencyImage.ImagePath = agency.Id + "_" + model.AgencyImage.FileName.Replace(" ", "");
                }

                // Pan Image
                if (model.PanImage != null)
                {
                    uc.UploadFiles(model.PanImage.InputStream, agencyImage.Id + "_" + model.PanImage.FileName, accesskey, secretkey, "image");
                    agencyImage.ImagePanPath = agencyImage.Id + "_" + model.PanImage.FileName.Replace(" ", "");
                }

                // BankPassbook Image
                if (model.BankPassbookImage != null)
                {
                    uc.UploadFiles(model.BankPassbookImage.InputStream, agencyImage.Id + "_" + model.BankPassbookImage.FileName, accesskey, secretkey, "image");
                    agencyImage.BankPassbookPath = agencyImage.Id + "_" + model.BankPassbookImage.FileName.Replace(" ", "");
                }

                //// BankPassbook Pdf
                //if (model.BankPassbookPdf != null)
                //{
                //    uc.UploadFiles(model.BankPassbookPdf.InputStream, agencyImage.Id + "_" + model.BankPassbookPdf.FileName, accesskey, secretkey, "pdf");
                //    agencyImage.BankPassbookPath = agencyImage.Id + "_" + model.BankPassbookPdf.FileName.Replace(" ", "");
                //}

                db.Entry(agencyImage).State = System.Data.Entity.EntityState.Modified;
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

        public ActionResult Edit(string id)
        {
            var dId = AdminHelpers.DCodeInt(id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var agency = db.Agencies.FirstOrDefault(i => i.Id == dId);
            var model = _mapper.Map<Agency, AgencyEditViewModel>(agency);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AccessPolicy(PageCode = "")]
        public ActionResult Edit(AgencyEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var agent = db.Agencies.FirstOrDefault(i => i.Id == model.Id);
            var agency = _mapper.Map(model, agent);
            agency.DateUpdated = DateTime.Now;
            db.Entry(agency).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            try
            {
                var agencyImage = db.Agencies.FirstOrDefault(i => i.Id == agency.Id);
                // Agency Image
                if (model.AgencyImage != null)
                {
                    uc.UploadFiles(model.AgencyImage.InputStream, agency.Id + "_" + model.AgencyImage.FileName, accesskey, secretkey, "image");
                    agencyImage.ImagePath = agency.Id + "_" + model.AgencyImage.FileName.Replace(" ", "");
                }

                // Pan Image
                if (model.PanImage != null)
                {
                    uc.UploadFiles(model.PanImage.InputStream, agencyImage.Id + "_" + model.PanImage.FileName, accesskey, secretkey, "image");
                    agencyImage.ImagePanPath = agencyImage.Id + "_" + model.PanImage.FileName.Replace(" ", "");
                }

                // BankPassbook Image
                if (model.BankPassbookImage != null)
                {
                    uc.UploadFiles(model.BankPassbookImage.InputStream, agencyImage.Id + "_" + model.BankPassbookImage.FileName, accesskey, secretkey, "image");
                    agencyImage.BankPassbookPath = agencyImage.Id + "_" + model.BankPassbookImage.FileName.Replace(" ", "");
                }

                //// BankPassbook Pdf
                //if (model.BankPassbookPdf != null)
                //{
                //    uc.UploadFiles(model.BankPassbookPdf.InputStream, agencyImage.Id + "_" + model.BankPassbookPdf.FileName, accesskey, secretkey, "pdf");
                //    agencyImage.BankPassbookPath = agencyImage.Id + "_" + model.BankPassbookPdf.FileName.Replace(" ", "");
                //}


                db.Entry(agencyImage).State = System.Data.Entity.EntityState.Modified;
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

        public JsonResult Delete(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var agency = db.Agencies.FirstOrDefault(i => i.Id == id);
            if (agency != null)
            {
                agency.Status = 2;
                agency.DateUpdated = DateTime.Now;
                agency.UpdatedBy = user.Name;
                db.Entry(agency).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        // Agency Assign
        [AccessPolicy(PageCode = "")]
        public ActionResult AgencyAssignList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new AgencyAssignListViewModel();
            model.Lists = db.Agencies.Where(i => i.Status == 0).Join(db.Shops.Where(i => i.Status == 0), m => m.Id, s => s.AgencyId, (m, s) => new { m, s })
                .Join(db.DeliveryBoys.Where(i => i.Status == 0), p => p.m.Id, d => d.AgencyId, (p, d) => new { p, d })
                .GroupBy(i => i.p.m.Id)
                .AsEnumerable()
                .Select(i => new AgencyAssignListViewModel.AgencyList
                {
                    AgencyId = i.FirstOrDefault().p.m.Id,
                    AgencyName = i.FirstOrDefault().p.m.Name,
                    ShopListItems = i.Where(a => a.p.s.Status == 0).Select(a => new AgencyAssignListViewModel.AgencyList.ShopListItem
                    {
                        ShopId = a.p.s.Id,
                        ShopName = a.p.s.Name,
                    }).ToList(),
                    DeliveryBoyListItems = i.Where(a => a.d.Status == 0).Select(a => new AgencyAssignListViewModel.AgencyList.DeliveryBoyListItem
                    {
                        DeliveryBoyId = a.d.Id,
                        DeliveryBoyName = a.d.Name
                    }).ToList()
                }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult AgencyAssign()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [AccessPolicy(PageCode = "")]
        public ActionResult AgencyAssign(AgencyAssignViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            if (model.ShopIds != null)
            {
                foreach (var item in model.ShopIds)
                {
                    var shop = db.Shops.FirstOrDefault(i => i.Id == item);
                    shop.AgencyId = model.AgencyId;
                    shop.AgencyName = model.AgencyName;
                    db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            if (model.DeliveryBoyIds != null)
            {
                foreach (var item in model.DeliveryBoyIds)
                {
                    var deliveryboy = db.DeliveryBoys.FirstOrDefault(i => i.Id == item);
                    deliveryboy.AgencyId = model.AgencyId;
                    deliveryboy.AgencyName = model.AgencyName;
                    db.Entry(deliveryboy).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("AgencyAssignList");
        }

        public JsonResult GetAgencyAssign(int agencyId)
        {
            var shop = db.Shops.Any(i => i.AgencyId == agencyId);
            var deliveryboy = db.DeliveryBoys.Any(i => i.AgencyId == agencyId);
            if (shop == true || deliveryboy == true)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetPhoneNumberCheck(string phone)
        {
            var customer = db.Customers.FirstOrDefault(i => i.PhoneNumber == phone);
            int msg;
            var customerExist = db.Customers.Any(i => i.PhoneNumber == phone);
            if (customerExist)
            {
                var agent = db.Agencies.FirstOrDefault(i => i.PhoneNumber == phone);
                if (agent != null)
                {
                    if (agent.Status == 0)    // Agency already Exist
                    {
                        msg = 1;
                        return Json(msg, JsonRequestBehavior.AllowGet);
                    }
                    else if (agent.Status == 1)   // Agency in approval Pending status
                    {
                        msg = 2;
                        return Json(msg, JsonRequestBehavior.AllowGet);
                    }
                    else if (agent.Status == 3 || agent.Status == 2) // Agency Update
                    {
                        msg = 3;
                        return Json(new { msg, phone = customer.PhoneNumber, name = customer.Name, email = customer.Email, customerid = customer.Id }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {     // Agency Create
                    msg = 4;
                    return Json(new { msg, phone = customer.PhoneNumber, name = customer.Name, email = customer.Email, customerid = customer.Id }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {   // Not a Customer
                return Json(msg = 0, JsonRequestBehavior.AllowGet);
            }
            return Json(msg = 0, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Approve(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var agency = db.Agencies.FirstOrDefault(i => i.Id == Id);
            agency.Status = 0;
            agency.UpdatedBy = user.Name;
            agency.DateUpdated = DateTime.Now;
            db.Entry(agency).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult Reject(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var agency = db.Agencies.FirstOrDefault(i => i.Id == Id);
            agency.Status = 3;
            agency.UpdatedBy = user.Name;
            agency.DateUpdated = DateTime.Now;
            db.Entry(agency).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List");
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

        public async Task<JsonResult> GetDeliveryBoySelect2(string q = "")
        {
            var model = await db.DeliveryBoys.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetAgencySelect2(string q = "")
        {
            var model = await db.Agencies.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
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