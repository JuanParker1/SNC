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
            var agencyExist = db.Agencies.Any(i => i.PhoneNumber == model.PhoneNumber);
            if (!agencyExist)
            {
                agency.Status = 1;
                agency.DateEncoded = DateTime.Now;
                agency.DateUpdated = DateTime.Now;
                agency.CreatedBy = user.Name;
                agency.UpdatedBy = user.Name;
                db.Agencies.Add(agency);
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
            else
            {
                var agencyupdate = db.Agencies.FirstOrDefault(i => i.PhoneNumber == model.PhoneNumber);
                _mapper.Map(model, agencyupdate);

                agencyupdate.Status = 1;
                agencyupdate.DateEncoded = DateTime.Now;
                agencyupdate.DateUpdated = DateTime.Now;
                agencyupdate.CreatedBy = user.Name;
                agencyupdate.UpdatedBy = user.Name;
                db.Entry(agencyupdate).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                try
                {
                    var agencyUpdateImage = db.Agencies.FirstOrDefault(i => i.Id == agencyupdate.Id);
                    // Agency Image
                    if (model.AgencyImage != null)
                    {
                        uc.UploadFiles(model.AgencyImage.InputStream, agency.Id + "_" + model.AgencyImage.FileName, accesskey, secretkey, "image");
                        agencyUpdateImage.ImagePath = agency.Id + "_" + model.AgencyImage.FileName.Replace(" ", "");
                    }

                    // Pan Image
                    if (model.PanImage != null)
                    {
                        uc.UploadFiles(model.PanImage.InputStream, agencyUpdateImage.Id + "_" + model.PanImage.FileName, accesskey, secretkey, "image");
                        agencyUpdateImage.ImagePanPath = agencyUpdateImage.Id + "_" + model.PanImage.FileName.Replace(" ", "");
                    }

                    // BankPassbook Image
                    if (model.BankPassbookImage != null)
                    {
                        uc.UploadFiles(model.BankPassbookImage.InputStream, agencyUpdateImage.Id + "_" + model.BankPassbookImage.FileName, accesskey, secretkey, "image");
                        agencyUpdateImage.BankPassbookPath = agencyUpdateImage.Id + "_" + model.BankPassbookImage.FileName.Replace(" ", "");
                    }

                    //// BankPassbook Pdf
                    //if (model.BankPassbookPdf != null)
                    //{
                    //    uc.UploadFiles(model.BankPassbookPdf.InputStream, agencyImage.Id + "_" + model.BankPassbookPdf.FileName, accesskey, secretkey, "pdf");
                    //    agencyImage.BankPassbookPath = agencyImage.Id + "_" + model.BankPassbookPdf.FileName.Replace(" ", "");
                    //}

                    db.Entry(agencyUpdateImage).State = System.Data.Entity.EntityState.Modified;
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

        public ActionResult Edit(string id)
        {
            var dId = AdminHelpers.DCodeInt(id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var agency = db.Agencies.FirstOrDefault(i => i.Id == dId);
            var model = _mapper.Map<Agency, AgencyEditViewModel>(agency);
            var customer = db.Customers.FirstOrDefault(i=> i.Id == agency.CustomerId);
            if(customer != null)
            {
                model.Password = customer.Password;
            }
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

        // Agency Delete 
        public JsonResult DeleteAgency(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var agency = db.Agencies.FirstOrDefault(i=> i.Id == id);
            var customer = db.Customers.FirstOrDefault(i => i.Id == agency.CustomerId);
            if (agency != null)
            {
                agency.Status = 2;
                agency.DateUpdated = DateTime.Now;
                agency.UpdatedBy = user.Name;
                db.Entry(agency).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            if (customer != null)
            {
                customer.Position = 0;
                customer.Password = null;
                customer.DateUpdated = DateTime.Now;
                customer.UpdatedBy = user.Name;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        // Assign Agency Remove
        public JsonResult Delete(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var shoplist = db.Shops.Where(i => i.AgencyId == id).ToList();
            var deliveryBoylist = db.DeliveryBoys.Where(i => i.AgencyId == id).ToList();
            if (shoplist.Count() > 0)
            {
                shoplist.ForEach(i => i.AgencyId = 0);
                shoplist.ForEach(i => i.AgencyName = null);
                db.SaveChanges();
            }
            if (deliveryBoylist.Count() > 0)
            {
                deliveryBoylist.ForEach(i => i.AgencyId = 0);
                deliveryBoylist.ForEach(i => i.AgencyName = null);
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteShop(int id)
        {
            var shop = db.Shops.FirstOrDefault(i => i.Id == id);
            if (shop != null)
            {
                shop.AgencyId = 0;
                shop.AgencyName = null;
                db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteDeliveryBoy(int id)
        {
            var deliveryBoy = db.DeliveryBoys.FirstOrDefault(i => i.Id == id);

            if (deliveryBoy != null)
            {
                deliveryBoy.AgencyId = 0;
                deliveryBoy.AgencyName = null;
                db.Entry(deliveryBoy).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        // Agency Assign List

        [AccessPolicy(PageCode = "")]
        public ActionResult AssignList(AgencyAssignListViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.Lists = db.Agencies.Where(i => i.Status == 0 && (model.FilterAgencyId != 0 ? i.Id == model.FilterAgencyId : true))
                .GroupJoin(db.Shops.Where(i => i.Status == 0), a => a.Id, s => s.AgencyId, (a, s) => new { a, s })
                .GroupJoin(db.DeliveryBoys.Where(i => i.Status == 0), ss => ss.s.FirstOrDefault().AgencyId, d => d.AgencyId, (ss, d) => new { ss, d })
            .Select(i => new AgencyAssignListViewModel.AgencyList
            {
                AgencyId = i.ss.a.Id,
                AgencyName = i.ss.a.Name,
                ShopListItems = i.ss.s.Select(a=> new AgencyAssignListViewModel.AgencyList.ShopListItem
                {
                    ShopId = a.Id,
                    ShopName = a.Name,
                }).ToList(),
                DeliveryBoyListItems = i.d.Select(a => new AgencyAssignListViewModel.AgencyList.DeliveryBoyListItem
                {
                    DeliveryBoyId = a.Id,
                    DeliveryBoyName = a.Name
                }).ToList()
            }).ToList();
            return View(model);
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
            var customer = db.Customers.FirstOrDefault(i => i.Id == agency.CustomerId);
            if (agency != null)
            {
                agency.Status = 0;
                agency.UpdatedBy = user.Name;
                agency.DateUpdated = DateTime.Now;
                db.Entry(agency).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            if (customer != null)
            {
                customer.Position = 5;   // Agency Login
                customer.DateUpdated = DateTime.Now;
                customer.UpdatedBy = user.Name;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
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

        public JsonResult Add(AgencyAssignViewModel model)
        {
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
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(AgencyAssignListViewModel model)
        {
            // Delete
            var shoplist = db.Shops.Where(i => i.AgencyId == model.editAgencyId).ToList();
            var deliveryBoylist = db.DeliveryBoys.Where(i => i.AgencyId == model.editAgencyId).ToList();
            if (shoplist.Count() > 0)
            {
                shoplist.ForEach(i => i.AgencyId = 0);
                shoplist.ForEach(i => i.AgencyName = null);
                db.SaveChanges();
            }
            if (deliveryBoylist.Count() > 0)
            {
                deliveryBoylist.ForEach(i => i.AgencyId = 0);
                deliveryBoylist.ForEach(i => i.AgencyName = null);
                db.SaveChanges();
            }
            
            // Update
            if (model.editShopIds != null)
            {
                foreach (var item in model.editShopIds)
                {
                    var shop = db.Shops.FirstOrDefault(i => i.Id == item);
                    shop.AgencyId = model.editAgencyId;
                    shop.AgencyName = model.editAgencyName;
                    db.Entry(shop).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            if (model.editDeliveryBoyIds != null)
            {
                foreach (var item in model.editDeliveryBoyIds)
                {
                    var deliveryboy = db.DeliveryBoys.FirstOrDefault(i => i.Id == item);
                    deliveryboy.AgencyId = model.editAgencyId;
                    deliveryboy.AgencyName = model.editAgencyName;
                    db.Entry(deliveryboy).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("AssignList");
        }

        public JsonResult GetAssignAgency(int id)
        {
            int[] shopids, deliveryboyids;
            string[] shopnames, deliveryboynames;
            var agency = db.Agencies.FirstOrDefault(i => i.Id == id);
            var shop = db.Shops.Where(i => i.AgencyId == id).ToList();
            var deliveryboy = db.DeliveryBoys.Where(i => i.AgencyId == id).ToList();
            shopids = shop.Select(x => x.Id).ToArray();
            shopnames = shop.Select(x => x.Name).ToArray();
            deliveryboyids = deliveryboy.Select(x => x.Id).ToArray();
            deliveryboynames = deliveryboy.Select(x => x.Name).ToArray();

            return Json(new { agencyid = agency.Id, agencyname = agency.Name, shopids = shopids, shopnames= shopnames, deliveryboyids = deliveryboyids, deliveryboynames= deliveryboynames }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.AgencyId == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDeliveryBoySelect2(string q = "")
        {
            var model = await db.DeliveryBoys.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.AgencyId == 0).Select(i => new
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

        //public static string RandomString()
        //{
        //    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        //    return new string(Enumerable.Repeat(chars, 6)
        //      .Select(s => s[random.Next(s.Length)]).ToArray());
        //}

        private static Random random = new Random();
        public JsonResult GeneratePassword(int customerid)
        {
            string password = "";
            var customer = db.Customers.FirstOrDefault(i => i.Id == customerid);
            if(customer!= null)
            {
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                password = new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
                customer.Password = password;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(password, JsonRequestBehavior.AllowGet);
            }
            return Json(password, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatePassword(int customerid, string password)
        {
            var customer = db.Customers.FirstOrDefault(i => i.Id == customerid);
            if (customer != null)
            {
                customer.Password = password;
                db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(password, JsonRequestBehavior.AllowGet);
        }

    }
}