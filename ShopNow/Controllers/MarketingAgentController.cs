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

        public ActionResult Create()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Create(AgencyCreateViewModel model)
        {

            var marketingAgent = _mapper.Map<AgencyCreateViewModel, MarketingAgent>(model);
            marketingAgent.Status = 0;
            marketingAgent.DateEncoded = DateTime.Now;
            marketingAgent.DateUpdated = DateTime.Now;
            db.MarketingAgents.Add(marketingAgent);
            db.SaveChanges();

            return RedirectToAction("Login", "MarketingAgent");
        }

        public ActionResult Edit()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var marketingAgent = db.MarketingAgents.FirstOrDefault(i => i.Id == user.Id);
            var model = _mapper.Map<MarketingAgent, AgencyEditViewModel>(marketingAgent);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AgencyEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            ViewBag.Name = user.Name;
            var marketingAgent = db.MarketingAgents.FirstOrDefault(i => i.Id == model.Id);// MarketingAgent.Get(model.Code);
            //var ma = _mapper.Map(model, marketingAgent);
            marketingAgent.Name = model.Name;
            marketingAgent.PhoneNumber = model.PhoneNumber;
            marketingAgent.Email = model.Email;
            marketingAgent.PanNumber = model.PanNumber;
            marketingAgent.ImagePanPath = model.ImagePanPath;
            marketingAgent.DateUpdated = DateTime.Now;
            db.Entry(marketingAgent).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            //MarketingAgent.Edit(marketingAgent, out int error);
            try
            {
                if (model.PanImage != null)
                {
                    uc.UploadImage(model.PanImage, marketingAgent.Id + "_", "/Content/ImageUpload/", Server, db, "", marketingAgent.Id.ToString(), "");
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
                    var PanImg = db.MarketingAgents.FirstOrDefault(i => i.Id == model.Id);// MarketingAgent.Get(model.Code);
                    PanImg.ImagePanPath = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/" + PanImg.Id + "_" + model.PanImage.FileName;
                    PanImg.DateUpdated = DateTime.Now;
                    db.Entry(PanImg).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

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

        
        public ActionResult Dashboard()
        {
            var user = ((Helpers.Sessions.User)Session["MARKETINGUSER"]);
            ViewBag.Name = user.Name;
            return View();
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
            }).OrderBy(i=>i.Name).ToList();

            return View(model.List);
        }

      
        // Franchise Assign
        [AccessPolicy(PageCode = "")]
        public ActionResult AssignedFranchiseList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new AgencyAssignListViewModel();
            model.Lists = db.MarketingAgents.Where(i => i.Status == 0).Join(db.Shops.Where(i => i.Status == 0), m => m.Id, s => s.MarketingAgentId, (m, s) => new { m, s })
                .Join(db.DeliveryBoys.Where(i => i.Status == 0), p => p.m.Id, d => d.MarketingAgentId, (p, d) => new { p, d })
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
                    shop.MarketingAgentId = model.MarketingAgentId;
                    shop.MarketingAgentName = model.MarketingAgentName;
                    db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            if (model.DeliveryBoyIds != null)
            {
                foreach (var item in model.DeliveryBoyIds)
                {
                    var deliveryboy = db.DeliveryBoys.FirstOrDefault(i => i.Id == item);
                    deliveryboy.MarketingAgentId = model.MarketingAgentId;
                    deliveryboy.MarketingAgentName = model.MarketingAgentName;
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
            model.ShopIds = string.Join(",",db.Shops.Where(i => i.MarketingAgentId == id && i.Status == 0).Select(i => i.Id).ToList());
            model.DeliveryBoyIds = string.Join(",",db.DeliveryBoys.Where(i => i.MarketingAgentId == id && i.Status == 0).Select(i => i.Id).ToList());

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
            var shop = db.Shops.Any(i => i.MarketingAgentId == marketingagentId);
            var deliveryboy = db.DeliveryBoys.Any(i => i.MarketingAgentId == marketingagentId);
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
            var model = await db.DeliveryBoys.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetMarketingAgentSelect2(string q = "")
        {
            var model = await db.MarketingAgents.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetShopCategorySelect2(string q = "")
        {
            var model = await db.ShopCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q)).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetBrandSelect2(string q = "")
        {
            var model = await db.Brands.OrderBy(i => i.Name).Where(a => a.Name.Contains(q)).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public async Task<JsonResult> GetStaffSelect2(int shopId)
        {
            var model = await db.Staffs.OrderBy(i => i.Name).Where(a => a.ShopId == shopId && a.Status == 0).Select(i => new
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