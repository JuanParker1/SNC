using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class AgencyCreateViewModel
    {
        public HttpPostedFileBase AgencyImage { get; set; }
        public HttpPostedFileBase PanImage { get; set; }
        public HttpPostedFileBase BankPassbookImage { get; set; }
        public HttpPostedFileBase BankPassbookPdf { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int CustomerId { get; set; }
        public string ImagePath { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string StreetName { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string PanNumber { get; set; }
        public string ImagePanPath { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public string SwiftCode { get; set; }
        public string BankPassbookPath { get; set; }
        public string UPIID { get; set; }
    }


    public class AgencyEditViewModel
    {
        public HttpPostedFileBase AgencyImage { get; set; }
        public HttpPostedFileBase PanImage { get; set; }
        public HttpPostedFileBase BankPassbookImage { get; set; }
        public HttpPostedFileBase BankPassbookPdf { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string StreetName { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string PanNumber { get; set; }
        public string ImagePanPath { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public string SwiftCode { get; set; }
        public string BankPassbookPath { get; set; }
        public string UPIID { get; set; }
        public int Status { get; set; }
        public string Password { get; set; }
        public int CustomerId { get; set; }
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
        public int AgencyId { get; set; }
        public string AgencyName { get; set; }

    }

    public class AgencyAssignListViewModel
    {
        public int FilterAgencyId { get; set; }
        public string FilterAgencyName { get; set; }
        public int[] editDeliveryBoyIds { get; set; }
        public int[] editShopIds { get; set; }
        public int editAgencyId { get; set; }
        public string editAgencyName { get; set; }
        public List<AgencyList> Lists { get; set; }
        public class AgencyList
        {
            public int AgencyId { get; set; }
            public string AgencyName { get; set; }

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