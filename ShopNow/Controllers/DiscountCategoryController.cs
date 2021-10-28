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

namespace ShopNow.Controllers
{
    public class DiscountCategoryController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;

        public DiscountCategoryController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<DiscountCategory, DiscountCategoryListViewModel.DiscountCategoryList>();

            });
            _mapper = _mapperConfiguration.CreateMapper();

        }

        [AccessPolicy(PageCode = "SNCDCL125")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DiscountCategoryListViewModel();

            model.List = db.DiscountCategories.Where(i => i.Status == 0).ToList().AsQueryable().ProjectTo<DiscountCategoryListViewModel.DiscountCategoryList>(_mapperConfiguration).ToList();
           
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCDCS126")]
        public JsonResult Save(string Name, double Percentage, int ShopId, string ShopName)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            string message = "";
            string message1 = "";
            var discountcategoryname = db.DiscountCategories.FirstOrDefault(i => i.Name == Name && i.Status == 0);// DiscountCategory.GetName(Name);
            if (discountcategoryname == null)
            {
                var discountCategory = new DiscountCategory();
                discountCategory.Name = Name;
                discountCategory.Percentage = Percentage;
                discountCategory.ShopId = ShopId;
                discountCategory.ShopName = ShopName;
                discountCategory.CreatedBy = user.Name;
                discountCategory.UpdatedBy = user.Name;
                discountCategory.Status = 0;
                discountCategory.DateEncoded = DateTime.Now;
                discountCategory.DateUpdated = DateTime.Now;
                db.DiscountCategories.Add(discountCategory);
                db.SaveChanges();
                IsAdded = discountCategory.Id != 0 ? true : false;
                message = Name + " Successfully Added";
            }
            else
            {
                message1 = Name + " Already Exist!";
            }
            return Json(new { IsAdded = IsAdded, message = message, message1 = message1 }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SNCDCE127")]
        public JsonResult Edit(int Id, string name, double percentage, int shopid, string shopname)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            string message = "";
            DiscountCategory discountCategory = db.DiscountCategories.FirstOrDefault(i => i.Id == Id);
            if (discountCategory != null)
            {
                discountCategory.Name = name;
                discountCategory.Percentage = percentage;
                discountCategory.ShopId = shopid;
                discountCategory.ShopName = shopname;
                discountCategory.DateUpdated = DateTime.Now;
                discountCategory.UpdatedBy = user.Name;
                discountCategory.DateUpdated = DateTime.Now;
                db.Entry(discountCategory).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //bool result = DiscountCategory.Edit(discountCategory, out int error);
                message = name + " Updated Successfully";
            }
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SNCDCD128")]
        public JsonResult Delete(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var discountCategory = db.DiscountCategories.FirstOrDefault(i => i.Id == Id);
            if (discountCategory != null)
            {
                discountCategory.Status = 2;
                discountCategory.DateUpdated = DateTime.Now;
                discountCategory.UpdatedBy = user.Name;
                db.Entry(discountCategory).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetListSelect2(string q = "")
        {
            var model = await db.Shops.Where(a => a.Name.Contains(q)).OrderBy(i => i.Name).Select(i => new
            {
                id = i.Id,
                text = i.Name
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