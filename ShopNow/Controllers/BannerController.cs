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
                config.CreateMap<Banner, BannerEditViewModel>();
                config.CreateMap<BannerEditViewModel, Banner>();
                config.CreateMap<Banner, BannerListViewModel.BannerList>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNBANL001")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new BannerListViewModel();
            model.List = db.Banners.Where(i => i.Status == 0 || i.Status == 2).Select(i => new BannerListViewModel.BannerList
            {
                BannerName = i.BannerName,
                Bannerpath = i.BannerPath,
                Id = i.Id,
                Days = i.Days,
                FromDate = i.FromDate,
                Position = i.Position,
                ProductId = i.ProductId,
                // ProductName = i.ProductName,
                ShopId = i.ShopId,
                // ShopName = i.ShopName,
                ToDate = i.Todate,
                CreditType = i.CreditType,
                Status = i.Status
            }).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNBANPL002")]
        public ActionResult PendingList()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new BannerListViewModel();

            model.List = db.Banners.Where(i => i.Status == 1).Select(i => new BannerListViewModel.BannerList
            {
                BannerName = i.BannerName,
                Bannerpath = i.BannerPath,
                Id= i.Id,
                Days = i.Days,
                FromDate = i.FromDate,
                Position = i.Position,
                ProductId = i.ProductId,
               // ProductName = i.ProductName,
                ShopId = i.ShopId,
               // ShopName = i.ShopName,
                ToDate = i.Todate,
                CreditType = i.CreditType
            }).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNBANE003")]
        public ActionResult Edit(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dcode = AdminHelpers.DCodeInt(id.ToString());
            var banner = db.Banners.FirstOrDefault(i => i.Id == dcode);
            var model = _mapper.Map<Banner, BannerEditViewModel>(banner);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNBANE003")]
        public ActionResult Edit(BannerEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var banner = _mapper.Map<BannerEditViewModel, Banner>(model);
            banner.UpdatedBy = user.Name;
            banner.DateUpdated = DateTime.Now;
            if (banner.ProductId != 0)
            {
                var product = db.Products.FirstOrDefault(i => i.Id == banner.ProductId && i.Status == 0);
                banner.MasterProductId = product.MasterProductId;
               // banner.MasterProductName = product.Name;
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

        [AccessPolicy(PageCode = "SHNBAND004")]
        public ActionResult Delete(int id)
        {
            var banner = db.Banners.FirstOrDefault(i => i.Id == id);
            banner.Status = 2;
            db.Entry(banner).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List", "Banner");
        }

        //[AccessPolicy(PageCode = "")]
        [HttpPost]
        public ActionResult UpdateActive(int Id, int status)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var banner = db.Banners.FirstOrDefault(i => i.Id == Id);
            banner.Status = status;
            banner.UpdatedBy = user.Name;
            banner.DateUpdated = DateTime.Now;
            db.Entry(banner).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNBANE003")]
        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();
            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNBANE003")]
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