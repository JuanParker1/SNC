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
        private ShopnowchatEntities db = new ShopnowchatEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        public NextSubCategoryController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<NextSubCategory, NextSubCategoryViewModel.NextSubCategoryList>();
                config.CreateMap<NextSubCategory, NextSubCategoryEditViewModel>();
                config.CreateMap<NextSubCategoryEditViewModel, NextSubCategory>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [AccessPolicy(PageCode = "SHNNSCL001")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from s in db.NextSubCategories
                        select s).OrderBy(s => s.Name).Where(i => i.Status == 0).ToList();
            return View(List);

        }

        [AccessPolicy(PageCode = "SHNNSCS002")]
        public JsonResult Save(string name = "", int type = 0, int subcategorycode =0, string subcategoryname = "", int adscore = 1)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            string message = "";
            string message1 = "";
            var nextsubcategoryname = db.NextSubCategories.FirstOrDefault(i => i.Name == name && i.Status == 0);// NextSubCategory.GetName(name);
            if (nextsubcategoryname == null)
            {
                var nextsubcategory = new NextSubCategory();
                nextsubcategory.Name = name;
                nextsubcategory.ProductTypeId = type;
                nextsubcategory.SubCategoryId = subcategorycode;
                nextsubcategory.SubCategoryName = subcategoryname;
                nextsubcategory.Adscore = adscore;
                nextsubcategory.CreatedBy = user.Name;
                nextsubcategory.UpdatedBy = user.Name;
                nextsubcategory.DateEncoded = DateTime.Now;
                nextsubcategory.DateUpdated = DateTime.Now;
                nextsubcategory.Status = 0;
                db.NextSubCategories.Add(nextsubcategory);
                db.SaveChanges();
                // string code = NextSubCategory.Add(nextsubcategory, out int error);
                IsAdded = nextsubcategory.Id != 0 ? true : false;
                message = name + " Successfully Added";
            }
            else
            {
                message1 = name + " Already Exist!";
            }
            return Json(new { IsAdded = IsAdded, message = message, message1 = message1 }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNNSCD004")]
        public JsonResult Delete(int code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var nextsubcategory = db.NextSubCategories.FirstOrDefault(i => i.Id == code);// NextSubCategory.Get(code);
            if (nextsubcategory != null)
            {
                nextsubcategory.Status = 2;
                nextsubcategory.DateUpdated = DateTime.Now;
                nextsubcategory.UpdatedBy = user.Name;
                db.Entry(nextsubcategory).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNNSCE003")]
        public ActionResult Edit(string code)
        {
            var dcode = AdminHelpers.DCodeInt(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (dcode==0)
                return HttpNotFound();
            var nextSubCategory = db.NextSubCategories.FirstOrDefault(i => i.Id == dcode);// NextSubCategory.Get(dcode);
            var model = _mapper.Map<NextSubCategory, NextSubCategoryEditViewModel>(nextSubCategory);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNNSCE003")]
        public ActionResult Edit(NextSubCategoryEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            NextSubCategory nextSubCategory = db.NextSubCategories.FirstOrDefault(i => i.Id == model.Id);// NextSubCategory.Get(model.Code);
            _mapper.Map(model, nextSubCategory);
            nextSubCategory.DateUpdated = DateTime.Now;
            nextSubCategory.UpdatedBy = user.Name;
            nextSubCategory.DateUpdated = DateTime.Now;
            db.Entry(nextSubCategory).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            // NextSubCategory.Edit(nextSubCategory, out int errorcode);
            return RedirectToAction("List");
        }

        public async Task<JsonResult> GetSubCategorySelect2(string q = "")
        {
            var model = await db.SubCategories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "FMCG").Select(i => new
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

    }

}