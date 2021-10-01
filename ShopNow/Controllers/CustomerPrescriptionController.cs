﻿using ShopNow.Filters;
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

        [AccessPolicy(PageCode = "")]
        public ActionResult List()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var list = db.CustomerPrescriptions.OrderByDescending(i => i.Id).ToList();
            return View(list);
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

    }
}