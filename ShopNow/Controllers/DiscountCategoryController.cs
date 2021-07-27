using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShopNow.Filters;
using ShopNow.Models;
using ShopNow.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [AccessPolicy(PageCode = "SHNDCAL001")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new DiscountCategoryListViewModel();

            model.List = db.DiscountCategories.Where(i => i.Status == 0).ToList().AsQueryable().ProjectTo<DiscountCategoryListViewModel.DiscountCategoryList>(_mapperConfiguration).ToList();
           
            return View(model);
        }

        [AccessPolicy(PageCode = "SHNDCAS002")]
        public JsonResult Save(string Name, double Percentage)
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

        [AccessPolicy(PageCode = "SHNDCAE003")]
        public JsonResult Edit(int Id, string name, double percentage)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            string message = "";
            DiscountCategory discountCategory = db.DiscountCategories.FirstOrDefault(i => i.Id == Id);
            if (discountCategory != null)
            {
                discountCategory.Name = name;
                discountCategory.Percentage = percentage;
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

        [AccessPolicy(PageCode = "SHNDCAD004")]
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
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

    }
}