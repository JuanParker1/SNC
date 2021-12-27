using Amazon.S3;
using AutoMapper;
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
    public class BannerController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private static readonly string accesskey = ConfigurationManager.AppSettings["AWSAccessKey"];
        private static readonly string secretkey = ConfigurationManager.AppSettings["AWSSecretKey"];
        UploadContent uc = new UploadContent();
        
        public BannerController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<BannerCreateViewModel, Banner>();
                config.CreateMap<Banner, BannerEditViewModel>();
                config.CreateMap<BannerEditViewModel, Banner>();
                config.CreateMap<Banner, BannerListViewModel.BannerList>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SNCBL038")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new BannerListViewModel();
            model.List = db.Banners.Where(i => i.Status == 0 || i.Status == 2)
                .GroupJoin(db.MasterProducts, b => b.MasterProductId, m => m.Id, (b, m) => new { b, m })
                .GroupJoin(db.Shops, b => b.b.ShopId, s => s.Id, (b, s) => new { b, s })
                .Select(i => new BannerListViewModel.BannerList
                {
                    BannerName = i.b.b.BannerName,
                    Bannerpath = i.b.b.BannerPath,
                    Id = i.b.b.Id,
                    Days = i.b.b.Days,
                    FromDate = i.b.b.FromDate,
                    Position = i.b.b.Position,
                    ProductId = i.b.b.ProductId,
                    ProductName = i.b.m.Any() ? i.b.m.FirstOrDefault().Name : "N/A",
                    ShopId = i.b.b.ShopId,
                    ShopName = i.s.Any() ? i.s.FirstOrDefault().Name : "N/A",
                    ToDate = i.b.b.Todate,
                    CreditType = i.b.b.CreditType,
                    Status = i.b.b.Status
                }).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SNCBPL039")]
        public ActionResult PendingList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new BannerListViewModel();

            model.List = db.Banners.Where(i => i.Status == 1)
                .GroupJoin(db.MasterProducts, b => b.MasterProductId, m => m.Id, (b, m) => new { b, m })
                .GroupJoin(db.Shops, b => b.b.ShopId, s => s.Id, (b, s) => new { b, s })
                .Select(i => new BannerListViewModel.BannerList
                {
                    BannerName = i.b.b.BannerName,
                    Bannerpath = i.b.b.BannerPath,
                    Id = i.b.b.Id,
                    Days = i.b.b.Days,
                    FromDate = i.b.b.FromDate,
                    Position = i.b.b.Position,
                    ProductId = i.b.b.ProductId,
                    ProductName = i.b.m.Any()? i.b.m.FirstOrDefault().Name : "N/A",
                    ShopId = i.b.b.ShopId,
                    ShopName = i.s.Any() ? i.s.FirstOrDefault().Name:"N/A",
                    ToDate = i.b.b.Todate,
                    CreditType = i.b.b.CreditType
                }).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SNCBE040")]
        public ActionResult Edit(string id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dcode = AdminHelpers.DCodeInt(id);
            var banner = db.Banners.FirstOrDefault(i => i.Id == dcode);
            var model = _mapper.Map<Banner, BannerEditViewModel>(banner);
            model.ShopName = db.Shops.FirstOrDefault(i => i.Id == banner.ShopId)?.Name;
            model.ProductName = db.MasterProducts.FirstOrDefault(i => i.Id == banner.MasterProductId)?.Name;
            if (banner.ProductId != 0)
                model.OfferQuantityLimit = db.Products.FirstOrDefault(i => i.Id == banner.ProductId).OfferQuantityLimit;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCBE040")]
        public ActionResult Edit(BannerEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var banner = db.Banners.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, banner);
            banner.UpdatedBy = user.Name;
            banner.DateUpdated = DateTime.Now;
            if (banner.ProductId != 0)
            {
                var product = db.Products.FirstOrDefault(i => i.Id == banner.ProductId && i.Status == 0);
                if (product != null)
                {
                    banner.MasterProductId = product.MasterProductId;
                    product.OfferQuantityLimit = model.OfferQuantityLimit;
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            try
            {
                //Banner Image
                if (model.BannerImage != null)
                {
                    uc.UploadMediumFile(model.BannerImage.InputStream, banner.Id + "_" + model.BannerImage.FileName, accesskey, secretkey, "image");  // Upload Medium Image
                    banner.BannerPath = banner.Id + "_" + model.BannerImage.FileName;
                }
                db.Entry(banner).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    ViewBag.Message = "Check the provided AWS Credentials.";
                }
                else
                {
                    ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                }
            }

            return RedirectToAction("List", "Banner");
        }

        [AccessPolicy(PageCode = "SNCBD041")]
        public ActionResult Delete(int id)
        {
            var banner = db.Banners.FirstOrDefault(i => i.Id == id);
            banner.Status = 2;
            db.Entry(banner).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            var product = db.Products.FirstOrDefault(i => i.Id == banner.ProductId);
            product.OfferQuantityLimit = 0;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("List", "Banner");
        }

        //[AccessPolicy(PageCode = "")]
        [HttpPost]
        [AccessPolicy(PageCode = "SNCBUA042")]
        public ActionResult UpdateActive(int Id, int status)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var banner = db.Banners.FirstOrDefault(i => i.Id == Id);
            banner.Status = status;
            banner.UpdatedBy = user.Name;
            banner.DateUpdated = DateTime.Now;
            db.Entry(banner).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            if (status == 2)
            {
                var product = db.Products.FirstOrDefault(i => i.Id == banner.ProductId);
                product.OfferQuantityLimit = 0;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
            }
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

        public async Task<JsonResult> GetShopProductSelect2(int shopid, string q = "")
        {
            var model = await db.Products.Where(a => a.ShopId == shopid && a.Status == 0)
                .Join(db.MasterProducts.Where(a => a.Name.Contains(q)), p => p.MasterProductId, m => m.Id, (p, m) => new { p, m }).Take(500)
                .Select(i => new
                {
                    id = i.p.Id,
                    text = i.m.Name
                }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SNCBC297")]
        public ActionResult Create()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCBC297")]
        public ActionResult Create(BannerCreateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var banner = _mapper.Map<BannerCreateViewModel, Banner>(model);
            banner.PaymentId = 123456;
            banner.Status = 0;
            banner.DateEncoded = DateTime.Now;
            banner.DateUpdated = DateTime.Now;
            banner.CreatedBy = user.Name;
            banner.UpdatedBy = user.Name;
            if (model.ProductId == 0)
            {
                banner.ProductId = 0;
                // banner.ProductName = "N/A";
            }
            else
            {
                var product = db.Products.FirstOrDefault(i => i.Id == model.ProductId && i.Status == 0);
                banner.MasterProductId = product.MasterProductId;
                product.OfferQuantityLimit = model.OfferQuantityLimit;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
            }
            try
            {
                //Banner Image
                if (model.BannerImage != null)
                {
                    uc.UploadMediumFile(model.BannerImage.InputStream, banner.Id + "_" + model.BannerImage.FileName, accesskey, secretkey, "image");  // Upload Medium Image
                    banner.BannerPath = banner.Id + "_" + model.BannerImage.FileName;
                }
                db.Banners.Add(banner);
                db.SaveChanges();
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    ViewBag.Message = "Check the provided AWS Credentials.";
                }
                else
                {
                    ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                }
            }
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