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
    public class CrustController : Controller
    {
        private sncEntities db = new sncEntities();
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;

        public CrustController()
        {
            _mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<Crust, CrustListViewModel.ListItem>();

            });
            _mapper = _mapperConfiguration.CreateMapper();

        }

        [AccessPolicy(PageCode = "")]
        public ActionResult List()
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new CrustListViewModel();

            model.List = db.Crusts.Where(i => i.Status == 0).ToList().AsQueryable().ProjectTo<CrustListViewModel.ListItem>(_mapperConfiguration).OrderBy(i => i.Name).ToList();

            return View(model.List);
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult Save(string name = "")
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            bool IsAdded = false;
            string message = "";
            string message1 = "";
            var crustName = db.Crusts.FirstOrDefault(i => i.Name == name && i.Status == 0);
            if (crustName == null)
            {
                var crust = new Crust();
                crust.Name = name;
                crust.CreatedBy = user.Name;
                crust.UpdatedBy = user.Name;
                crust.Status = 0;
                crust.DateEncoded = DateTime.Now;
                crust.DateUpdated = DateTime.Now;
                db.Crusts.Add(crust);
                db.SaveChanges();

                IsAdded = crust.Id != 0 ? true : false;
                message = name + " Successfully Added";
            }
            else
            {
                message1 = name + " Already Exist!";
            }

            return Json(new { IsAdded = IsAdded, message = message, message1 = message1 }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult Edit(int Id, string name)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            string message = "";
            Crust crust = db.Crusts.FirstOrDefault(i => i.Id == Id);
            if (crust != null)
            {
                crust.Name = name;
                crust.DateUpdated = DateTime.Now;
                crust.UpdatedBy = user.Name;
                db.Entry(crust).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                message = name + " Updated Successfully";
            }
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult Delete(int Id)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var crust = db.Crusts.FirstOrDefault(i => i.Id == Id);
            if (crust != null)
            {
                crust.Status = 2;
                crust.DateUpdated = DateTime.Now;
                crust.UpdatedBy = user.Name;
                db.Entry(crust).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}