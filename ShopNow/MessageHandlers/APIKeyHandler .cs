using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using ShopNow.Helpers;

namespace ShopNow.MessageHandlers
{

    public class APIKeyHandler : ActionFilterAttribute
    {
        private const string Key = "AdminShopNowChatIn123";
        private const string Key1 = "AdminShopNow123";
        private const string Key2 = "AdminShopNowPay123";
        private const string Key3 = "astechCall";
        public string ApiKey { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!string.IsNullOrEmpty(ApiKey))
            {
                IEnumerable<string> headerValues = filterContext.RequestContext.HttpContext.Request.Headers.GetValues("X-ApiKey");
                if (headerValues != null)
                {
                    var data = headerValues.FirstOrDefault();
                    var id = AdminHelpers.DCode(data);
                    if (Key == id)
                    {
                        return;
                    }else if (Key1 == id)
                    {
                        return;
                    }
                    else if (Key2 == id)
                    {
                        return;
                    }
                    else if (Key3 == id)
                    {
                        return;
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult("~/Home/ApiAccess");
                    }
                }
                else
                {                 
                    filterContext.Result= new RedirectResult("~/Home/ApiAccess");
                }
               
            }
        }

     
    }
    
}
