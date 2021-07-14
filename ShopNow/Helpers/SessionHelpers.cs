using System;
using System.Linq;
using System.Net;
using System.IO;
using ShopNow.Models;
using System.Web;

namespace ShopNow.Helpers
{
    public class Sessions
    {
     
        public class User
        {
           
            public int Id { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
            public string ImagePath { get; set; }
            public string CountryCode { get; set; }
            public string CountryName { get; set; }
            public string StateCode { get; set; }
            public string StateName { get; set; }
            public string DistrictCode { get; set; }
            public string DistrictName { get; set; }
            public string StreetCode { get; set; }
            public string StreetName { get; set; }
            public string Address { get; set; }
            public string Access { get; set; }
            public string PinCode { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public int Status { get; set; }

           

            public bool HasAccess(string pageCode)
            {
                return Access.Contains(pageCode) || Access.Contains("*");
            }
        }

      

        public static void HandleContentRequests(HttpContext current)
        {

            var urlString = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            var ignoreAuthString = new System.Text.RegularExpressions.Regex("_|/api/|/cdn/|/fonts/|/bundles/|/plugins/|/image/|/account/login|/account/register|.jpeg|.jpg|.png|.gif|.css|.js|/get");
            var authString = "";

            if (ignoreAuthString.IsMatch(urlString))
            {
                return;
            }

            if (urlString.Contains("u_"))
            {
                authString = urlString.Substring(urlString.IndexOf("u_=") + 3);
            }

            if (current.Request.IsAuthenticated)
            {
                if (current?.Session?["USER"] == null)
                {
                    Set(current.User.Identity.Name);
                }
            }
            else
            {
                current.Response.Redirect($"/Home/Index?returnUrl={urlString}&u_={authString}");
            }
        }


        public static bool Set(string username)
        {
            try
            {
                ShopnowchatEntities db = new ShopnowchatEntities();
                System.Web.HttpContext.Current.Session.Timeout = 120;
                User user = System.Web.HttpContext.Current.Session == null ? null : ((ShopNow.Helpers.Sessions.User)System.Web.HttpContext.Current.Session["USER"]);

                if ((user?.Name ?? string.Empty) == username)
                {
                    System.Web.HttpContext.Current.Session["USER"] = user;
                    return true;
                }
 
               

                user = db.Customers.Where(i => i.PhoneNumber == username && i.Status == 0 && i.Position ==4)
                     .Select(i => new Helpers.Sessions.User
                     {
                         Code = i.Code,
                         Name = i.Name,
                         
                     }).FirstOrDefault();

                    //if(user==null)
                    //{
                    //    user = db.Shops.Where(i => i.PhoneNumber == username && i.Status == 0)
                    //                     .Select(i => new Helpers.Sessions.User
                    //                     {
                    //                         Code = i.Code,
                    //                         Name = i.Name,

                    //                     }).FirstOrDefault();
                    //    System.Web.HttpContext.Current.Session["USER"] = user;
                    //    return true;
                    //}
                    System.Web.HttpContext.Current.Session["USER"] = user;
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SetBrandOwnerUser(string username)
        {
            try
            {
                User user = System.Web.HttpContext.Current.Session == null ? null : ((ShopNow.Helpers.Sessions.User)System.Web.HttpContext.Current.Session["BRANDUSER"]);

                if ((user?.Name ?? string.Empty) == username)
                {
                    System.Web.HttpContext.Current.Session["BRANDUSER"] = user;
                    return true;
                }

                ShopnowchatEntities db = new ShopnowchatEntities();

                user = db.BrandOwners.Where(i => i.PhoneNumber == username && i.Status == 0)
                     .Select(i => new Helpers.Sessions.User
                     {
                         Code = i.Code,
                         Name = i.Name,

                     }).FirstOrDefault();

                System.Web.HttpContext.Current.Session["BRANDUSER"] = user;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool SetMarketingUser(string username)
        {
            try
            {
                User user = System.Web.HttpContext.Current.Session == null ? null : ((ShopNow.Helpers.Sessions.User)System.Web.HttpContext.Current.Session["BRANDUSER"]);

                if ((user?.Name ?? string.Empty) == username)
                {
                    System.Web.HttpContext.Current.Session["MARKETINGUSER"] = user;
                    return true;
                }

                ShopnowchatEntities db = new ShopnowchatEntities();

                user = db.MarketingAgents.Where(i => i.PhoneNumber == username && i.Status == 0)
                     .Select(i => new Helpers.Sessions.User
                     {
                         Code = i.Code,
                         Name = i.Name,

                     }).FirstOrDefault();

                System.Web.HttpContext.Current.Session["MARKETINGUSER"] = user;
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}