using AutoMapper;
using ShopNow.Filters;
using ShopNow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class FAQCategoryController : Controller
    {
        private sncEntities db = new sncEntities();
        //private IMapper _mapper;
        //private MapperConfiguration _mapperConfiguration;

        //public FAQCategoryController()
        //{
        //    _mapperConfiguration = new MapperConfiguration(config =>
        //    {
        //        config.CreateMap<FAQCategory, FAQCategory>();
        //    });
        //    _mapper = _mapperConfiguration.CreateMapper();
        //}

        [AccessPolicy(PageCode = "SNCFCL321")]
        public ActionResult List()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["User"]);
            ViewBag.Name = user.Name;
            var list = (from q in db.FAQCategories
                        select q).OrderBy(q => q.Name).Where(i => i.Status == 0).ToList();
            return View(list);
        }

        [AccessPolicy(PageCode = "SNCFCS322")]
        public JsonResult Save(string name)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            string message = "";
            string message1 = "";
            var faqCategoryname = db.FAQCategories.FirstOrDefault(i => i.Name == name && i.Status == 0);
            FAQCategory fAQCategory = new FAQCategory();
            if (faqCategoryname == null)
            {
                fAQCategory.Name = name;
                fAQCategory.UpdatedBy = user.Name;
                fAQCategory.Status = 0;
                fAQCategory.DateUpdated = DateTime.Now;
                db.FAQCategories.Add(fAQCategory);
                db.SaveChanges();
                IsAdded = fAQCategory.Id != 0 ? true : false;
                message = fAQCategory.Name + " Successfully Added";
            }
            else
            {
                message1 = fAQCategory.Name + " Already Exist!";
            }
            return Json(new { IsAdded = IsAdded, message = message, message1 = message1 }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SNCFCE323")]
        public JsonResult Edit(FAQCategory faqCategorymodel)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            string message = "";
            FAQCategory faqCategory = db.FAQCategories.Where(f => f.Id == faqCategorymodel.Id).FirstOrDefault();
            if (faqCategory != null)
            {
                faqCategory.Name = faqCategorymodel.Name;
                faqCategory.UpdatedBy = user.Name;
                faqCategory.DateUpdated = DateTime.Now;
                db.Entry(faqCategory).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                message = faqCategorymodel.Name + " Updated Successfully";
            }
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SNCFCD324")]
        public JsonResult Delete(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var faqCategory = db.FAQCategories.Where(q => q.Id == Id).FirstOrDefault();
            if (faqCategory != null)
            {
                faqCategory.Status = 2;
                faqCategory.DateUpdated = DateTime.Now;
                faqCategory.UpdatedBy = user.Name;
                db.Entry(faqCategory).State = System.Data.Entity.EntityState.Modified;
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