using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopNow.Models;

namespace ShopNow.Filters
{
    public class AccessPolicyAttribute : ActionFilterAttribute
    {
        private sncEntities db = new sncEntities();
        public string PageCode { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!string.IsNullOrEmpty(PageCode))
            {
            var customer = ((ShopNow.Helpers.Sessions.User)System.Web.HttpContext.Current.Session["USER"]);
                if(customer != null)
                {
                    var roles = db.AccessPolicies.Where(i => i.CustomerId.Equals(customer.Id) && i.Status == 0).Select(s => s.PageCode);

                    if (roles != null)
                    {
                        if (roles.Contains(PageCode))
                            return;

                        filterContext.Result = new RedirectResult("~/Home/Access");
                    }
                    else
                        filterContext.Result = new RedirectResult("~/Home/Access");
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/Home/Index");
                }
               
               
            }
        }
    }
}