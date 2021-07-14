using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class PlatFormCreditRateController : Controller
    {
        private ShopnowchatEntities db = new ShopnowchatEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;

        public PlatFormCreditRateController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<PlatFormCreditRate, PlatFormCreditRateListViewModel.PlatFormCreditRateList>();

            });
            _mapper = _mapperConfiguration.CreateMapper();

        }
        [AccessPolicy(PageCode = "SHNPFCL001")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new PlatFormCreditRateListViewModel();

            model.List = db.PlatFormCreditRates.Where(i => i.Status == 0).ToList().AsQueryable().ProjectTo<PlatFormCreditRateListViewModel.PlatFormCreditRateList>(_mapperConfiguration).ToList();
            model.Count = db.PlatFormCreditRates.Where(i => i.Status == 0).Count();
            ViewBag.Count = db.PlatFormCreditRates.Where(i => i.Status == 0).Count();
            return View(model.List);
        }

        [AccessPolicy(PageCode = "SHNPFCS002")]
        public JsonResult Save(double RatePerOrder, int DailyViewer)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            string message = "";
            var platFormCreaditRate = new PlatFormCreditRate();
            platFormCreaditRate.RatePerOrder = RatePerOrder;
            platFormCreaditRate.DailyViewer = DailyViewer;
            platFormCreaditRate.CreatedBy = user.Name;
            platFormCreaditRate.UpdatedBy = user.Name;
            platFormCreaditRate.Status = 0;
            platFormCreaditRate.DateEncoded = DateTime.Now;
            platFormCreaditRate.DateUpdated = DateTime.Now;
            db.PlatFormCreditRates.Add(platFormCreaditRate);

            db.SaveChanges();

           // string code = PlatFormCreditRate.Add(platFormCreaditRate, out int error);
           IsAdded = platFormCreaditRate.Id !=0 ? true : false;
            message = RatePerOrder + " Successfully Added";

            return Json(new { IsAdded = IsAdded, message = message }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPFCE003")]
        public JsonResult Edit(int code, double ratePerOrder, int dailyViewer)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            string message = "";
            PlatFormCreditRate platFormCR = db.PlatFormCreditRates.FirstOrDefault(i => i.Id == code);// PlatFormCreditRate.Get(code);
            if (platFormCR != null)
            {
                platFormCR.RatePerOrder = ratePerOrder;
                platFormCR.DailyViewer = dailyViewer;
                platFormCR.DateUpdated = DateTime.Now;
                platFormCR.UpdatedBy = user.Name;
                platFormCR.DateUpdated = DateTime.Now;
                db.Entry(platFormCR).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                // bool result = PlatFormCreditRate.Edit(platFormCR, out int error);
                message = ratePerOrder + " Updated Successfully";
            }
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPFCD004")]
        public JsonResult Delete(int code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var platFormCreaditRate = db.PlatFormCreditRates.FirstOrDefault(i => i.Id == code);// PlatFormCreditRate.Get(code);
            if (platFormCreaditRate != null)
            {
                platFormCreaditRate.Status = 2;
                platFormCreaditRate.DateUpdated = DateTime.Now;
                platFormCreaditRate.UpdatedBy = user.Name;
                db.Entry(platFormCreaditRate).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
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