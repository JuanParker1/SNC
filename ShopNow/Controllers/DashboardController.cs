using ShopNow.Filters;
using ShopNow.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    // [Authorize]
    public class DashboardController : Controller
    {
        private sncEntities _db = new sncEntities();

        [AccessPolicy(PageCode = "SHNDASI001")]
        public ActionResult Index()
        {
            //ShopnowchatEntities db = new ShopnowchatEntities();
            //var phoneNumber = Session["PhoneNumber"];
            var dat = DateTime.Now;
            ViewBag.Name = ((ShopNow.Helpers.Sessions.User)System.Web.HttpContext.Current.Session["USER"]).Name;
            ViewBag.customer = _db.Customers.Where(i => i.Status == 0).Count();
            ViewBag.Shops = _db.Shops.Where(i => i.Status == 0).Count();
            ViewBag.Delivery = _db.DeliveryBoys.Where(i => i.Status == 0).Count();
            ViewBag.Order = _db.Orders.Where(i => i.Status == 0 && i.OrderNumber != 0 && i.Status != 7 && i.Status != 6).Count();
            //var authu = "";

            //var user = Customer.GetPhoneNumber(phoneNumber.ToString());
            //var autht = DateTime.Now.ToString("yyyy-MM-dd_HH:mm");
            //var enc = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{authu}_{autht}"));
            //var auth = $"u_={enc}";

            //var employees = db.Customers.OrderBy(i => i.Name).Select(i => new SelectListItem
            //{
            //    Value = i.Code,
            //    Text = i.Name
            //});
            //if (user != null){
            //     authu = user.PhoneNumber;
            //    var cust = db.Customers.FirstOrDefault(i => i.Code == user.Code && i.Status == 0);
            //    var model = db.Customers.Where(i => i.Code == user.Code && i.Status == 0)
            //                 .Select(e => new EmployeeDetailsViewModel
            //                 {
            //                     Name = e.Name,
            //                     Code = e.Code,
            //                 }).FirstOrDefault();
            //    ViewBag.Auth = auth;
            //    ViewBag.Employees = employees;
            //    return View(model);
            //}
            //else
            //{
            //    var shop = Shop.GetPhoneNumber(phoneNumber.ToString());
            //     authu = shop.PhoneNumber;
            //    var cust = db.Shops.FirstOrDefault(i => i.Code == shop.Code && i.Status == 0);
            //    var model = db.Shops.Where(i => i.Code == shop.Code && i.Status == 0)
            //                  .Select(e => new EmployeeDetailsViewModel
            //                  {
            //                      Name = e.Name,
            //                      Code = e.Code,
            //                  }).FirstOrDefault();
            //    ViewBag.Auth = auth;
            //    ViewBag.Employees = employees;
            //    return View(model);
            //}
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}