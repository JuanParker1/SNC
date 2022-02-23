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

        [AccessPolicy(PageCode = "SNCPSI225")]
        public ActionResult Index(ProductScheduleIndexViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            model.ListItems = db.ProductSchedules.Where(i => i.Status == 0 && (model.FilterProductId != 0 ? i.ProductId == model.FilterProductId : true))
                .Join(db.Products.Where(i => model.FilterShopId != 0 ? i.ShopId == model.FilterShopId : true).Where(i => i.Status == 0), psc => psc.ProductId, p => p.Id, (psc, p) => new { psc, p })
                .Join(db.MasterProducts, psc => psc.p.MasterProductId, m => m.Id, (psc, m) => new { psc, m })
                .Join(db.Shops.Where(i => (!string.IsNullOrEmpty(model.FilterDistrict)) ? i.DistrictName == model.FilterDistrict : true), psc => psc.psc.p.ShopId, s => s.Id, (psc, s) => new { psc, s })
                .GroupBy(i => i.psc.psc.psc.ProductId)
            .Select(i => new ProductScheduleIndexViewModel.ListItem
            {
                HasSchedule = i.FirstOrDefault().psc.psc.p.HasSchedule ?? false,
                ProductId = i.Key,
                ProductName = i.FirstOrDefault().psc.m.Name + " - " + i.FirstOrDefault().psc.psc.p.ShopName,
                AvailableDays = i.FirstOrDefault().psc.psc.psc.AvailableDays,
                TimeListItems = i.Where(a => a.psc.psc.psc.Status == 0).Select(a => new ProductScheduleIndexViewModel.ListItem.TimeListItem
                {
                    Id = a.psc.psc.psc.Id,
                    OffTime = a.psc.psc.psc.OffTime,
                    OnTime = a.psc.psc.psc.OnTime
                }).ToList()
            }).ToList();
            return View(model);
        }

        [AccessPolicy(PageCode = "SNCPSD226")]
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

        [AccessPolicy(PageCode = "SNCPSUS227")]
        public JsonResult UpdateSchedule(int productid, bool hasSchedule)
        {
            var product = db.Products.FirstOrDefault(i => i.Id == productid && i.Status == 0);
            product.HasSchedule = hasSchedule;
            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AccessPolicy(PageCode = "SNCPSAT228")]
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
                UpdatedBy = user.Name,
                AvailableDays =  db.ProductSchedules.FirstOrDefault(i => i.ProductId == productid).AvailableDays
            };
            db.ProductSchedules.Add(ProductSchedule);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AccessPolicy(PageCode = "SNCPSUT229")]
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

        [AccessPolicy(PageCode = "SNCPSDT230")]
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

        [AccessPolicy(PageCode = "SNCPSAS231")]
        public JsonResult Add(ProductScheduleAddViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            foreach (var proId in model.ProductId)
            {
                var isExist = db.ProductSchedules.Any(i => i.ProductId == proId && i.Status == 0);
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
                            UpdatedBy = user.Name,
                            AvailableDays = string.Join(",", model.AvailableDays)
                        };
                        db.ProductSchedules.Add(productSchedule);
                        db.SaveChanges();
                    }
                }

            }

            var productList = db.Products.Where(i => model.ProductId.Contains(i.Id)).ToList();
            productList.ForEach(i => i.HasSchedule = true);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveAvailableDays(string productId)
        {
            long dId = AdminHelpers.DCodeLong(productId);
            var productSchedule = db.ProductSchedules.Where(i => i.ProductId == dId && i.Status == 0).ToList();
            if (productSchedule.Count() > 0)
            {
                productSchedule.ForEach(i => i.AvailableDays = string.Empty);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult AddUpdateAvailableDays(int productId, string[] AvailableDays)
        {
            var productSchedule = db.ProductSchedules.Where(i => i.ProductId == productId && i.Status == 0).ToList();
            if (productSchedule.Count() > 0)
            {
                productSchedule.ForEach(i => i.AvailableDays = string.Join(",", AvailableDays));
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
                }).Distinct().OrderBy(i => i.text).ToListAsync();

            return Json(new { results = model, pagination = new { more = false } }, JsonRequestBehavior.AllowGet);
        }
    }
}