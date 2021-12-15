using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopNow.Models
{
    public class stockmodel
    {
        public int shopid { get; set; }
        public string errorMessage { get; set; }
        public string ShopName { get; set; }
        public HttpPostedFileBase excel_file { get; set; }

    }
}