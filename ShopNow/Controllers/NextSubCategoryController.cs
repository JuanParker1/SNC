using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class NextSubCategoryController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;

        public NextSubCategoryController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                //config.CreateMap<NextSubCategory, NextSubCategoryViewModel.NextSubCategoryList>();
                config.CreateMap<NextSubCategory, NextSubCategoryEditViewModel>();
                config.CreateMap<NextSubCategoryEditViewModel, NextSubCategory>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SNCNSL163")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from s in db.NextSubCategories
                        select s).OrderBy(s => s.Name).Where(i => i.Status == 0).ToList();
            return View(List);

        }

        [AccessPolicy(PageCode = "SNCNSS164")]
        public JsonResult Save(NextSubCategory nextSubCategory)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            string message = "";
            string message1 = "";
            var nextsubcategoryname = db.NextSubCategories.FirstOrDefault(i => i.Name == nextSubCategory.Name && i.Status == 0);
            if (nextsubcategoryname == null)
            {
                nextSubCategory.CreatedBy = user.Name;
                nextSubCategory.UpdatedBy = user.Name;
                nextSubCategory.DateEncoded = DateTime.Now;
                nextSubCategory.DateUpdated = DateTime.Now;
                nextSubCategory.Status = 0;
                db.NextSubCategories.Add(nextSubCategory);
                db.SaveChanges();
                IsAdded = nextSubCategory.Id != 0 ? true : false;
                message = nextSubCategory.Name + " Successfully Added";
            }
            else
            {
                message1 = nextSubCategory.Name + " Already Exist!";
            }
            return Json(new { IsAdded = IsAdded, message = message, message1 = message1 }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SNCNSD165")]
        public JsonResult Delete(string Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var dId = AdminHelpers.DCodeInt(Id);
            var nextsubcategory = db.NextSubCategories.FirstOrDefault(i => i.Id == dId);
            if (nextsubcategory != null)
            {
                nextsubcategory.Status = 2;
                nextsubcategory.DateUpdated = DateTime.Now;
                nextsubcategory.UpdatedBy = user.Name;
                db.Entry(nextsubcategory).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SNCNSE166")]
        public ActionResult Edit(string id)
        {
            var dcode = AdminHelpers.DCodeInt(id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var nextSubCategory = db.NextSubCategories.FirstOrDefault(i => i.Id == dcode);
            var model = _mapper.Map<NextSubCategory, NextSubCategoryEditViewModel>(nextSubCategory);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SNCNSE166")]
        public ActionResult Edit(NextSubCategoryEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            NextSubCategory nextSubCategory = db.NextSubCategories.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, nextSubCategory);
            nextSubCategory.DateUpdated = DateTime.Now;
            nextSubCategory.UpdatedBy = user.Name;
            nextSubCategory.DateUpdated = DateTime.Now;
            db.Entry(nextSubCategory).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List");
        }

        public async Task<JsonResult> GetSubCategorySelect2(string q = "")
        {
            var model = await db.SubCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 2).Select(i => new
            {
                id = i.Id,
                text = i.Name,
                ProductTypeId = i.ProductTypeId,
                ProductTypeName = i.ProductTypeId
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