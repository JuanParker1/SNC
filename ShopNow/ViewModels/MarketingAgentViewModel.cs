using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class MarketingAgentRegisterViewModel
    {
        public HttpPostedFileBase PanImage { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PanNumber { get; set; }
        public string ImagePanPath { get; set; }
    }

    public class MarketingAgentLoginViewModel
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
       
    }

    public class MarketingAgentUpdationViewModel
    {
        public HttpPostedFileBase PanImage { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PanNumber { get; set; }
        public string ImagePanPath { get; set; }
    }

    public class MarketingAgentListViewModel
    {
        public List<MarketingAgentList> List { get; set; }
        public class MarketingAgentList
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
        }
    }
}