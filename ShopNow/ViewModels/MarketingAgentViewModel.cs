using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class AgencyCreateViewModel
    {
        public HttpPostedFileBase PanImage { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PanNumber { get; set; }
        public string ImagePanPath { get; set; }
    }


    public class AgencyEditViewModel
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

    public class AgencyListViewModel
    {
        public List<AgencyList> List { get; set; }
        public class AgencyList
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
        }
    }

    public class AgencyAssignViewModel
    {
        public int[] DeliveryBoyIds { get; set; }
        public int[] ShopIds { get; set; }
        public int MarketingAgentId { get; set; }
        public string MarketingAgentName { get; set; }
       
    }

    public class AgencyAssignUpdateViewModel
    {
        public string DeliveryBoyIds { get; set; }
        public string ShopIds { get; set; }
        public int MarketingAgentId { get; set; }
        public string MarketingAgentName { get; set; }

    }
    public class AgencyAssignListViewModel
    {
        public List<AgencyList> Lists { get; set; }
        public class AgencyList
        {
            public int MarketingAgentId { get; set; }
            public string MarketingAgentName { get; set; }

            public List<ShopListItem> ShopListItems { get; set; }
            public class ShopListItem
            {
                public int ShopId { get; set; }
                public string ShopName { get; set; }
            }

            public List<DeliveryBoyListItem> DeliveryBoyListItems { get; set; }
            public class DeliveryBoyListItem
            {
                public int DeliveryBoyId { get; set; }
                public string DeliveryBoyName { get; set; }
            }
        }

    }

}