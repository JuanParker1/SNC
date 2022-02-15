using ShopNow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class FAQCategoryController : Controller
    {
        private sncEntities db = new sncEntities();

        public ActionResult List()
        {
            return View();
        }
    }
}