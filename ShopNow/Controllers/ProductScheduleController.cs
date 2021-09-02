using ShopNow.Filters;
using ShopNow.Helpers;
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
    public class ProductScheduleController : Controller
    {
        private sncEntities db = new sncEntities();

        [AccessPolicy(PageCode = "")]
        public ActionResult Index(ProductScheduleIndexViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.ProductSchedules.Where(i => i.Status == 0 && (model.FilterProductId != 0 ? i.ProductId == model.FilterProductId : true))
                .Join(db.Products, psc => psc.ProductId, p => p.Id, (psc, p) => new { psc, p })
                .Join(db.MasterProducts, psc => psc.p.MasterProductId, m => m.Id, (psc, m) => new { psc, m })
                .GroupBy(i => i.psc.psc.ProductId)
            .Select(i => new ProductScheduleIndexViewModel.ListItem
            {
                HasSchedule = i.FirstOrDefault().psc.p.HasSchedule ?? false,
                ProductId = i.Key,
                ProductName = i.FirstOrDefault().m.Name + " - " + i.FirstOrDefault().psc.p.ShopName,
                TimeListItems = i.Where(a => a.psc.psc.Status == 0).Select(a => new ProductScheduleIndexViewModel.ListItem.TimeListItem
                {
                    Id= a.psc.psc.Id,
                    OffTime = a.psc.psc.OffTime,
                    OnTime = a.psc.psc.OnTime
                }).ToList()
            }).ToList();
            return View(model);
        }

        public JsonResult Add(ProductScheduleAddViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            foreach (var proId in model.ProductId)
            {
                var isExist = db.ProductSchedules.Any(i => i.ProductId == proId && i.Status==0);
                if (!isExist)
                {
                    foreach (var item in model.TimeListItems)
                    {
                        var productSchedule = new ProductSchedule
                        {
                            DateTimeUpdated = DateTime.Now,
                            OffTime = item.OffTime,
                            OnTime = item.OnTime,
                            ProductId = proId,
                            Status = 0,
                            UpdatedBy = user.Name
                        };
                        db.ProductSchedules.Add(productSchedule);
                        db.SaveChanges();
                    }
                }
                //return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult Delete(string productId)
        {
            long dId = AdminHelpers.DCodeLong(productId);
            var productSchedules = db.ProductSchedules.Where(i => i.ProductId == dId).ToList();
            if (productSchedules.Count() > 0)
            {
                productSchedules.ForEach(i => i.Status = 2);
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public JsonResult UpdateSchedule(int productid, bool hasSchedule)
        {
            var product = db.Products.FirstOrDefault(i => i.Id == productid && i.Status == 0);
            product.HasSchedule = hasSchedule;
            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult AddTiming(long productid, TimeSpan onTime, TimeSpan offTime)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var ProductSchedule = new ProductSchedule
            {
                DateTimeUpdated = DateTime.Now,
                OffTime = offTime,
                OnTime = onTime,
                ProductId = productid,
                Status = 0,
                UpdatedBy = user.Name
            };
            db.ProductSchedules.Add(ProductSchedule);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult UpdateTiming(int id, TimeSpan onTime, TimeSpan offTime)
        {
            var user = ((Helpers.Sessions.User)Session["USER"]);
            var productSchedule = db.ProductSchedules.FirstOrDefault(i => i.Id == id);
            productSchedule.OnTime = onTime;
            productSchedule.OffTime = offTime;
            productSchedule.UpdatedBy = user.Name;
            db.Entry(productSchedule).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AccessPolicy(PageCode = "")]
        public ActionResult DeleteTiming(string id)
        {
            int dId = AdminHelpers.DCodeInt(id);
            var productSchedule = db.ProductSchedules.FirstOrDefault(i => i.Id == dId);
            if (productSchedule != null)
            {
                productSchedule.Status = 2;
                db.Entry(productSchedule).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public async Task<JsonResult> GetScheduleProductSelect2(string q = "")
        {
            var model = await db.ProductSchedules.Where(a => a.Status == 0)
                .Join(db.Products,psc => psc.ProductId, p => p.Id, (psc, p) => new { psc, p })
                .Join(db.MasterProducts.Where(a => a.Name.Contains(q)), psc => psc.p.MasterProductId, m => m.Id, (psc, m) => new { psc, m })
                .Select(i => new
                {
                    id = i.psc.p.Id,
                    text = i.m.Name
                }).OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }
    }
}