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
    public class ServiceController : Controller
    {
        private sncEntities db = new sncEntities();

        [AccessPolicy(PageCode = "")]
        public ActionResult List()
        {
            var user = ((ShopNow.Helpers.Sessions.User)Session["USER"]);
            ViewBag.Name = user.Name;
            var model = new ServiceListViewModel();
            model.ListItems = db.Services.Where(i => i.Status == 0)
                .Select(i => new ServiceListViewModel.ListItem
                {
                    Amount = i.Amount,
                    DateEncoded = i.DateEncoded,
                    DeliveryAddress = i.DeliveryAddress,
                    DeliveryCharge = i.DeliveryCharge,
                    Distance = i.Distance,
                    Name = i.Name,
                    PhoneNumber = i.PhoneNumber,
                    PickupAddress = i.PickupAddress,
                    Remarks = i.Remarks,
                    Status= i.Status
                }).ToList();
            return View(model);
        }
    }
}