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
    public class CustomerPrescriptionController : Controller
    {
        private sncEntities db = new sncEntities();

        [AccessPolicy(PageCode = "SNCCPL111")]
        public ActionResult List()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var list = db.CustomerPrescriptions.OrderByDescending(i => i.Id).ToList();
            var model = new CustomerPrescriptionWebListViewModel();
            model.ListItems = db.CustomerPrescriptions.OrderByDescending(i => i.Id)
                .GroupJoin(db.CustomerPrescriptionImages, cp => cp.Id, cpi => cpi.CustomerPrescriptionId, (cp, cpi) => new { cp, cpi })
                .Select(i => new CustomerPrescriptionWebListViewModel.ListItem
                {
                    Id = i.cp.Id,
                    AudioPath = i.cp.AudioPath,
                    CustomerId = i.cp.CustomerId,
                    CustomerName = i.cp.CustomerName,
                    CustomerPhoneNumber = i.cp.CustomerPhoneNumber,
                    ImagePath = i.cp.ImagePath,
                    Remarks = i.cp.Remarks,
                    ShopId = i.cp.ShopId,
                    DateEncoded = i.cp.DateEncoded,
                    Status = i.cp.Status,
                    ImagePathLists = i.cpi.Select(a => new CustomerPrescriptionWebListViewModel.ListItem.ImagePathList
                    {
                        ImagePath = a.ImagePath
                    }).ToList()
                }).ToList();
            return View(model);
        }

        public JsonResult AddPrescriptionItem(PrescriptionItemAddViewModel model)
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            foreach (var item in model.ListItems)
            {
                var prescriptionItem = new CustomerPrescriptionItem
                {
                    CreatedBy = user.Name,
                    CustomerPrescriptionId = model.PrescriptionId,
                    DateEncoded = DateTime.Now,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Status = 0
                };
                db.CustomerPrescriptionItems.Add(prescriptionItem);
                db.SaveChanges();
            }

            var prescription = db.CustomerPrescriptions.FirstOrDefault(i => i.Id == model.PrescriptionId);
            if (prescription != null)
            {
                prescription.Status = 1;
                db.Entry(prescription).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetItemList(int id)
        {
            var list = db.CustomerPrescriptionItems.Where(i => i.CustomerPrescriptionId == id)
                .Join(db.Products, cp => cp.ProductId, p => p.Id, (cp, p) => new { cp, p })
                .Join(db.MasterProducts, cp => cp.p.MasterProductId, m => m.Id, (cp, m) => new { cp, m })
                .Select(i => new
                {
                    ProductName = i.m.Name,
                    Quantity = i.cp.cp.Quantity
                }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

    }
}