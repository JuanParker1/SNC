using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System.Data;
using System.Data.OleDb;
using System.IO;
using AutoMapper;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ShopNow.Controllers
{
    public class FAQController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;

        public FAQController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<FAQCreateViewModel, FAQ>();
                config.CreateMap<FAQ, FAQEditViewModel>();
                config.CreateMap<FAQEditViewModel, FAQ>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }
        // GET: FAQ
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new FAQListViewModel();
            model.ListItems = db.FAQs.Where(i => i.Status == 0)
                .Join(db.FAQCategories, f=> f.FAQCategoryId, fc=>fc.Id,(f,fc)=> new { f,fc})
                .Select(i => new FAQListViewModel.ListItem
            {
                Id = i.f.Id,
                Description = i.f.Description,
                FAQCategoryId = i.f.FAQCategoryId,
                FAQCategoryName = i.fc.Name
            }).ToList();

            return View(model);
        }

        public ActionResult Create()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FAQCreateViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);

            FAQ faq = new FAQ();
            faq.FAQCategoryId = model.FAQCategoryId;
            faq.Description = model.Description;
            faq.DateUpdated = DateTime.Now;
            faq.UpdatedBy = user.Name;
            faq.Status = 0;
            db.FAQs.Add(faq);
            db.SaveChanges();

            return RedirectToAction("List");
        }

        public ActionResult Edit(string id)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            int dId = AdminHelpers.DCodeInt(id);
            var faq = db.FAQs.FirstOrDefault(i => i.Id == dId);
            var model = _mapper.Map<FAQ, FAQEditViewModel>(faq);
            model.FAQCategoryName = db.FAQCategories.FirstOrDefault(i=> i.Id == faq.FAQCategoryId).Name;
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FAQEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var faq = db.FAQs.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, faq);
            faq.DateUpdated = DateTime.Now;
            faq.UpdatedBy = user.Name;
            db.Entry(faq).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = AdminHelpers.ECodeInt(model.Id) });
        }

        public JsonResult Delete(string id)
        {
            int dId = AdminHelpers.DCodeInt(id);
            var faq = db.FAQs.FirstOrDefault(i => i.Id == dId);
            if (faq != null)
            {
                faq.Status = 2;
                db.Entry(faq).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetFaqCategorySelect2(string q = "")
        {
            var model = await db.FAQCategories.Where(a => a.Name.Contains(q) && a.Status == 0).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }

    }
}