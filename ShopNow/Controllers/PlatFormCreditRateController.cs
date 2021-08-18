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
        private sncEntities db = new sncEntities();
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
            return View(model);
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
            
           IsAdded = platFormCreaditRate.Id !=0 ? true : false;
            message = RatePerOrder + " Successfully Added";

            return Json(new { IsAdded = IsAdded, message = message }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPFCE003")]
        public JsonResult Edit(int id, double ratePerOrder, int dailyViewer)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            string message = "";
            PlatFormCreditRate platFormCR = db.PlatFormCreditRates.FirstOrDefault(i => i.Id == id);
            if (platFormCR != null)
            {
                platFormCR.Status = 2;
                db.Entry(platFormCR).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var platFormCreditRate = new PlatFormCreditRate();
                platFormCreditRate.RatePerOrder = ratePerOrder;
                platFormCreditRate.DailyViewer = dailyViewer;
                platFormCreditRate.CreatedBy = user.Name;
                platFormCreditRate.UpdatedBy = user.Name;
                platFormCreditRate.Status = 0;
                platFormCreditRate.DateEncoded = DateTime.Now;
                platFormCreditRate.DateUpdated = DateTime.Now;
                db.PlatFormCreditRates.Add(platFormCreditRate);
                db.SaveChanges();
                message = ratePerOrder + " Updated Successfully";
            }
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNPFCD004")]
        public JsonResult Delete(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var platFormCreaditRate = db.PlatFormCreditRates.FirstOrDefault(i => i.Id == id);
            if (platFormCreaditRate != null)
            {
                platFormCreaditRate.Status = 2;
                platFormCreaditRate.DateUpdated = DateTime.Now;
                platFormCreaditRate.UpdatedBy = user.Name;
                db.Entry(platFormCreaditRate).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
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