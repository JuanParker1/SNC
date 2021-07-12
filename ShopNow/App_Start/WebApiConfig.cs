﻿using ShopNow.MessageHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace ShopNow
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {


            var cors = new EnableCorsAttribute("*", "*", "*");
            
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.EnableCors(cors);
         
        }
    }
}
