using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShopNow.Filters;
using ShopNow.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class SubCategoryController : Controller
    {
        private ShopnowchatEntities db = new ShopnowchatEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;

        public SubCategoryController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                //config.CreateMap<SubCategory, SubCategoryListViewModel.SubCategoryList>();
                config.CreateMap<SubCategory, SubCategoryEditViewModel>();
                config.CreateMap<SubCategoryEditViewModel, SubCategory>();
            });
            _mapper = _mapperConfiguration.CreateMapper();

        }

        [AccessPolicy(PageCode = "SHNSUCL001")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var List = (from s in db.SubCategories
                        select s).OrderBy(s => s.Name).Where(i => i.Status == 0).ToList();
            return View(List);

        }

        [AccessPolicy(PageCode = "SHNSUCS002")]
        public JsonResult Save(SubCategory subCategory)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            string message = "";
            string message1 = "";
            var subcategoryname = db.SubCategories.Where(i => i.Name == subCategory.Name && i.ProductTypeId == subCategory.ProductTypeId).FirstOrDefault();
            if (subcategoryname == null)
            {
                subCategory.CreatedBy = user.Name;
                subCategory.UpdatedBy = user.Name;
                subCategory.Status = 0;
                subCategory.DateEncoded = DateTime.Now;
                subCategory.DateUpdated = DateTime.Now;
                db.SubCategories.Add(subCategory);
                db.SaveChanges();
                IsAdded = subCategory.Id != 0 ? true : false;
                message = subCategory.Name + " Successfully Added";
            }
            else
            {
                message1 = subCategory.Name + " Already Exist!";
            }
            return Json(new { IsAdded = IsAdded, message = message, message1 = message1 }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSUCE003")]
        public ActionResult Edit(string Id)
        {
            var dcode = AdminHelpers.DCodeInt(Id);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (dcode==0)
                return HttpNotFound();
            var subCategory = db.SubCategories.FirstOrDefault(i => i.Id == dcode);
            var model = _mapper.Map<SubCategory, SubCategoryEditViewModel>(subCategory);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNSUCE003")]
        public ActionResult Edit(SubCategoryEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            SubCategory subCategory = db.SubCategories.FirstOrDefault(i => i.Id == model.Id);
            _mapper.Map(model, subCategory);
            subCategory.DateUpdated = DateTime.Now;
            subCategory.UpdatedBy = user.Name;
            db.Entry(subCategory).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNSUCD004")]
        public ActionResult Delete(int id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var subcategory = db.SubCategories.FirstOrDefault(i => i.Id == id);
            if (subcategory != null)
            {
                subcategory.Status = 2;
                subcategory.DateUpdated = DateTime.Now;
                subcategory.UpdatedBy = user.Name;
                db.Entry(subcategory).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNSUCL001")]
        public async Task<JsonResult> GetCategorySelect2(string q = "")
        {
            var model = await db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductTypeId == 2).Select(i => new
            {
                id = i.Id,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }
    }
}