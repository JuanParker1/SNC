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
        private sncEntities db = new sncEntities();
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
                config.CreateMap<AgencyCreateViewModel, MarketingAgent>();
                config.CreateMap<MarketingAgent, AgencyEditViewModel>();
                config.CreateMap<AgencyEditViewModel, MarketingAgent>();
                config.CreateMap<MarketingAgent, AgencyListViewModel>();
            });

            _mapper = _mapperConfiguration.CreateMapper();
        }

        public ActionResult List()
        {
            var model = new AgencyListViewModel();
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.List = db.MarketingAgents.Where(i => i.Status == 0).Select(i => new AgencyListViewModel.AgencyList
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
            model.List = db.MarketingAgents.Where(i => i.Status == 1).Select(i => new AgencyListViewModel.AgencyList
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
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Create(AgencyCreateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var agency = _mapper.Map<AgencyCreateViewModel, MarketingAgent>(model);
            agency.Status = 1;
            agency.DateEncoded = DateTime.Now;
            agency.DateUpdated = DateTime.Now;
            db.MarketingAgents.Add(agency);
            db.SaveChanges();

            try
            {
                var agencyImage = db.MarketingAgents.FirstOrDefault(i=> i.Id == agency.Id);
                // Agency Image
                if (model.AgencyImage != null)
                {
                    uc.UploadFiles(model.AgencyImage.InputStream, agency.Id + "_" + model.AgencyImage.FileName, accesskey, secretkey, "image");
                    agencyImage.ImagePath = agency.Id + "_" + model.AgencyImage.FileName.Replace(" ", "");
                }

                //// DrivingLicense Image
                //if (model.PanImage != null)
                //{
                //    uc.UploadFiles(model.DrivingLicenseImage.InputStream, deliveryboy.Id + "_" + model.DrivingLicenseImage.FileName, accesskey, secretkey, "image");
                //    agencyImage.ImagePanPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + deliveryboy.Id + "_" + model.DrivingLicenseImage.FileName.Replace(" ", "");
                //}

                //// BankPassbook Image
                //if (model.BankPassbookImage != null)
                //{
                //    uc.UploadFiles(model.BankPassbookImage.InputStream, deliveryboy.Id + "_" + model.BankPassbookImage.FileName, accesskey, secretkey, "image");
                //    agencyImage.BankPassbookPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/" + deliveryboy.Id + "_" + model.BankPassbookImage.FileName.Replace(" ", "");
                //}

                //// BankPassbook Pdf
                //if (model.BankPassbookPdf != null)
                //{
                //    uc.UploadFiles(model.BankPassbookPdf.InputStream, deliveryboy.Id + "_" + model.BankPassbookPdf.FileName, accesskey, secretkey, "pdf");
                //    agencyImage. = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Uploads/" + deliveryboy.Id + "_" + model.BankPassbookPdf.FileName.Replace(" ", "");
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
            return RedirectToAction("List", "MarketingAgent");
        }

        public ActionResult Edit()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var agency = db.MarketingAgents.FirstOrDefault(i => i.Id == user.Id);
            var model = _mapper.Map<MarketingAgent, AgencyEditViewModel>(agency);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AgencyEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            ViewBag.Name = user.Name;
            var marketingAgent = db.MarketingAgents.FirstOrDefault(i => i.Id == model.Id);
            var agency = _mapper.Map(model, marketingAgent);

            agency.DateUpdated = DateTime.Now;
            db.Entry(agency).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();


            return View(model);
        }

        
        // Franchise Assign
        [AccessPolicy(PageCode = "")]
        public ActionResult AssignedFranchiseList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new AgencyAssignListViewModel();
            model.Lists = db.MarketingAgents.Where(i => i.Status == 0).Join(db.Shops.Where(i => i.Status == 0), m => m.Id, s => s.AgencyId, (m, s) => new { m, s })
                .Join(db.DeliveryBoys.Where(i => i.Status == 0), p => p.m.Id, d => d.AgencyId, (p, d) => new { p, d })
                .GroupBy(i => i.p.m.Id)
                .AsEnumerable()
                .Select(i => new AgencyAssignListViewModel.AgencyList
                {
                    MarketingAgentId = i.FirstOrDefault().p.m.Id,
                    MarketingAgentName = i.FirstOrDefault().p.m.Name,
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
        public ActionResult AssignFranchise()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [AccessPolicy(PageCode = "")]
        public ActionResult AssignFranchise(AgencyAssignViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);          
            if (model.ShopIds != null)
            {
                foreach (var item in model.ShopIds)
                {
                    var shop = db.Shops.FirstOrDefault(i => i.Id == item);
                    shop.AgencyId = model.MarketingAgentId;
                    shop.AgencyName = model.MarketingAgentName;
                    db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            if (model.DeliveryBoyIds != null)
            {
                foreach (var item in model.DeliveryBoyIds)
                {
                    var deliveryboy = db.DeliveryBoys.FirstOrDefault(i => i.Id == item);
                    deliveryboy.AgencyId = model.MarketingAgentId;
                    deliveryboy.AgencyName = model.MarketingAgentName;
                    db.Entry(deliveryboy).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("AssignedFranchiseList");
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult FranchiseUpdate(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new AgencyAssignUpdateViewModel();
            model.ShopIds = string.Join(",",db.Shops.Where(i => i.AgencyId == id && i.Status == 0).Select(i => i.Id).ToList());
            model.DeliveryBoyIds = string.Join(",",db.DeliveryBoys.Where(i => i.AgencyId == id && i.Status == 0).Select(i => i.Id).ToList());

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "")]
        public ActionResult FranchiseUpdate(AgencyAssignUpdateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            return RedirectToAction("AssignedFranchiseList");
        }

        public JsonResult GetAssignMarketingAgent(int marketingagentId)
        {
            var shop = db.Shops.Any(i => i.AgencyId == marketingagentId);
            var deliveryboy = db.DeliveryBoys.Any(i => i.AgencyId == marketingagentId);
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
                var agent = db.MarketingAgents.FirstOrDefault(i => i.PhoneNumber == phone);
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
                        return Json(new { msg, phone = customer.PhoneNumber, name = customer.Name, email = customer.Email }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {     // Agency Create
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