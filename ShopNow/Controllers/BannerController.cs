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
        private ShopnowchatEntities db = new ShopnowchatEntities();
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
            model.List = db.Banners.Where(i => i.Status == 0).Select(i => new BannerListViewModel.BannerList {
                BannerName = i.BannerName,
                Bannerpath = i.Bannerpath,
                Id = i.Id,
                Days = i.Days,
                Fromdate = i.Fromdate,
                Position = i.Position,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ShopId = i.ShopId,
                ShopName = i.ShopName,
                Todate = i.Todate,
                CreditType = i.CreditType
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
                Bannerpath = i.Bannerpath,
                Id= i.Id,
                Days = i.Days,
                Fromdate = i.Fromdate,
                Position = i.Position,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ShopId = i.ShopId,
                ShopName = i.ShopName,
                Todate = i.Todate,
                CreditType = i.CreditType
            }).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNBANE003")]
        public ActionResult Edit(string code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var dcode = AdminHelpers.DCode(code);
            var banner = db.Banners.FirstOrDefault(i => i.Code == dcode);
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
            if (banner.ProductCode != null)
            {
                var product = db.Products.FirstOrDefault(i => i.Code == banner.ProductCode && i.Status == 0);
                banner.MasterProductCode = product.MasterProductCode;
                banner.MasterProductName = product.MasterProductName;
            }
            try
            {
                //Banner Image
                if (model.BannerImage != null)
                {
                    uc.UploadMediumFile(model.BannerImage.InputStream, banner.Code + "_" + model.BannerImage.FileName, accesskey, secretkey, "image");  // Upload Medium Image
                    banner.Bannerpath = banner.Code + "_" + model.BannerImage.FileName;
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
        public ActionResult Delete(string code)
        {
            var dCode = AdminHelpers.DCode(code);
            var banner = db.Banners.FirstOrDefault(i => i.Code == dCode);  //Product.Get(dCode);
            banner.Status = 2;
            db.Entry(banner).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List", "Banner");
        }

        [AccessPolicy(PageCode = "SHNBANE003")]
        public async Task<JsonResult> GetShopSelect2(string q = "")
        {
            var model = await db.Shops.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0).Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();
            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNBANE003")]
        public async Task<JsonResult> GetShopProductSelect2(string shopcode, string q = "")
        {
            var model = await db.Products.Where(a => a.ShopCode == shopcode && a.Status == 0)
                .Join(db.MasterProducts.Where(a => a.Name.Contains(q)), p => p.MasterProductCode, m => m.Code, (p, m) => new { p, m })
                .Select(i => new
                {
                    id = i.p.Code,
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