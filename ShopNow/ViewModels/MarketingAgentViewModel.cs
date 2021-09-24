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
        public int Id { get; set; }
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
            public int Id { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
        }
    }

    public class FranchiseAssignViewModel
    {
        public int[] DeliveryBoyIds { get; set; }
        public int[] ShopIds { get; set; }
        public int MarketingAgentId { get; set; }
        public string MarketingAgentName { get; set; }
       
    }

    public class FranchiseAssignUpdateViewModel
    {
        public string DeliveryBoyIds { get; set; }
        public string ShopIds { get; set; }
        public int MarketingAgentId { get; set; }
        public string MarketingAgentName { get; set; }

    }
    public class FranchiseListViewModel
    {
        public List<FranchiseList> Lists { get; set; }
        public class FranchiseList
        {
            public int MarketingAgentId { get; set; }
            public string MarketingAgentName { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public int DeliveryBoyId { get; set; }
            public string DeliveryBoyName { get; set; }
        }

    }

}