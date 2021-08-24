using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class AppDetailController : Controller
    {
        private sncEntities db = new sncEntities();

        [AccessPolicy(PageCode = "")]
        public ActionResult Index()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new AppDetailIndexViewModel();
            model.ListItems = db.AppDetails.Where(i => i.Status == 0)
            .Select(i => new AppDetailIndexViewModel.ListItem
            {
                Id = i.Id,
                Name = i.Name,
                Version = i.Version,
                DateUpdated = i.DateUpdated
            }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "")]
        [HttpPost]
        public ActionResult Create(string name, string version)
        {
            var isExist = db.AppDetails.Any(i => i.Name == name);
            if (!isExist)
            {
                var appDetail = new AppDetail
                {
                    DateUpdated = DateTime.Now,
                    Name = name,
                    Status = 0,
                    Version = version
                };
                db.AppDetails.Add(appDetail);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult Delete(string id)
        {
            int dId = AdminHelpers.DCodeInt(id);
            var appDetail = db.AppDetails.FirstOrDefault(i => i.Id == dId);
            if (appDetail != null)
            {
                appDetail.Status = 2;
                db.Entry(appDetail).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        [HttpPost]
        public ActionResult Edit(int id, string name, string version)
        {

            var appDetail = db.AppDetails.FirstOrDefault(i => i.Id == id);
            appDetail.Name = name;
            appDetail.Version = version;
            appDetail.DateUpdated = DateTime.Now;
            db.Entry(appDetail).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}