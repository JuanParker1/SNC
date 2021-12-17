using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ShopNow
{
    public class MvcApplication : System.Web.HttpApplication
    {
        string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        protected void Application_Start()
        {
            
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            // WebApiConfig.Register(GlobalConfiguration.Configuration);
            SqlDependency.Start(connString);

        }
        protected void Application_End()
        {
            //Stop SQL dependency
            SqlDependency.Stop(connString);
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //SqlDependency.Start(connString);
            if (Request.Headers.AllKeys.Contains("Origin") && HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.Flush();
            }
        }
        // void Application_EndRequest(Object sender, EventArgs e)
        //{
        //    //SqlDependency.Stop(connString);
        //    var context = new HttpContextWrapper(Context);
        //    if (context.Response.StatusCode == 503)
        //    {
        //        Response.Redirect("~/ErrorPage/Publish");
        //    }
        //    //else if(context.Response.StatusCode == 400)
        //    //{
        //    //    HttpContext.Current.Response.Redirect("~/ErrorPage/Badrequest");
        //    //}
        //    //else if (context.Response.StatusCode == 404)
        //    //{
        //    //    HttpContext.Current.Response.Redirect("~/ErrorPage/PageNotFound");
        //    //}
        //}
        //protected void Session_End(object sender, EventArgs e)
        //{
        //    Session.Abandon();
        //    Response.RedirectToRoute("Home", "Index");
        //}
    }

}
