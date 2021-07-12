using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.IO;
using ExcelDataReader;
using ShopNow.Helpers;

namespace ShopNow.Controllers
{
    public class SubCategoryController : Controller
    {
        private ShopnowchatEntities db = new ShopnowchatEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;
        private const string _prefix = "SCT";
        private static string _generatedCode
        {
            get
            {
                return ShopNow.Helpers.DRC.Generate(_prefix);
            }
        }
        public SubCategoryController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<SubCategory, SubCategoryListViewModel.SubCategoryList>();
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
            //var user = ((Helpers.Sessions.User)Session["USER"]);
            //ViewBag.Name = user.Name;
            //var model = new SubCategoryListViewModel();

            //model.List = SubCategory.GetList().AsQueryable().ProjectTo<SubCategoryListViewModel.SubCategoryList>(_mapperConfiguration).OrderBy(i => i.Name).ToList();

            //return View(model);
        }

        [AccessPolicy(PageCode = "SHNSUCS002")]
        public JsonResult Save(string name = "", string type = "", string categorycode = "", string categoryname = "", int adscore = 1)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            string message = "";
            string message1 = "";
            var subcategoryname = db.SubCategories.Where(i => i.Name == name && i.ProductType == type).FirstOrDefault(); //SubCategory.GetNameType(name, type);
            if (subcategoryname == null)
            {
                var subcategory = new SubCategory();
                subcategory.Name = name;
                subcategory.ProductType = type;
                subcategory.CategoryCode = categorycode;
                subcategory.CategoryName = categoryname;
                subcategory.Adscore = adscore;
                subcategory.CreatedBy = user.Name;
                subcategory.UpdatedBy = user.Name;
                subcategory.Code = _generatedCode;
                subcategory.Status = 0;
                subcategory.DateEncoded = DateTime.Now;
                subcategory.DateUpdated = DateTime.Now;
                db.SubCategories.Add(subcategory);
                db.SaveChanges();
                //string code = SubCategory.Add(subcategory, out int error);
                IsAdded = subcategory.Code != String.Empty ? true : false;
                message = name + " Successfully Added";
            }
            else
            {
                message1 = name + " Already Exist!";
            }
            return Json(new { IsAdded = IsAdded, message = message, message1 = message1 }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSUCE003")]
        public ActionResult Edit(string code)
        {
            var dcode = AdminHelpers.DCode(code);
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            if (String.IsNullOrEmpty(dcode))
                return HttpNotFound();
            var subCategory = db.SubCategories.FirstOrDefault(i => i.Code == code); // SubCategory.Get(dcode);
            var model = _mapper.Map<SubCategory, SubCategoryEditViewModel>(subCategory);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessPolicy(PageCode = "SHNSUCE003")]
        public ActionResult Edit(SubCategoryEditViewModel model)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            SubCategory subCategory = db.SubCategories.FirstOrDefault(i => i.Code == model.Code); // SubCategory.Get(model.Code);
            _mapper.Map(model, subCategory);
            subCategory.DateUpdated = DateTime.Now;
            subCategory.UpdatedBy = user.Name;
            db.Entry(subCategory).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
           // SubCategory.Edit(subCategory, out int errorcode);
            return RedirectToAction("List");
        }

        [AccessPolicy(PageCode = "SHNSUCD004")]
        public JsonResult Delete(string code)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var subcategory = db.SubCategories.FirstOrDefault(i => i.Code == code);  //SubCategory.Get(code);
            if (subcategory != null)
            {
                subcategory.Status = 2;
                subcategory.DateUpdated = DateTime.Now;
                subcategory.UpdatedBy = user.Name;
                db.Entry(subcategory).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SHNSUCL001")]
        public async Task<JsonResult> GetCategorySelect2(string q = "")
        {
            var model = await db.Categories.OrderBy(i => i.Name).Where(a => a.Name.Contains(q) && a.Status == 0 && a.ProductType == "FMCG").Select(i => new
            {
                id = i.Code,
                text = i.Name
            }).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }
    }
}