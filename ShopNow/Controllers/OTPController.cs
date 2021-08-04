﻿using ShopNow.Filters;
using ShopNow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Controllers
{
    public class OTPController : Controller
    {
        private sncEntities db = new sncEntities();

        [AccessPolicy(PageCode = "")]
        public ActionResult Index()
        {
            var list = db.OtpVerifications.Where(i => i.Verify == false).OrderByDescending(i => i.Id).Take(10).ToList();
            return View(list);
        }
    }
}