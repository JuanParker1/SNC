using ShopNow.Controllers;
using ShopNow.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace ShopNow.Filters
{
    public class FrontPageActionFilter : FilterAttribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            dynamic controller;
            string controllerName = filterContext.RequestContext.HttpContext.Request.RawUrl.Split('/')[1].ToLower();
            switch (controllerName)
            {
                case "":
                    controller = (HomeController)filterContext.Controller;
                    break;              
            }
             //ApplicationDbContext _db = controller._db;
           // filterContext.Controller.ViewBag.CategoryAndSubCategory = Category.GetList();
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
            //  throw new System.NotImplementedException();
        }
    }

}