using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class DeliveryBoyCreateEditViewModel
    {
        public HttpPostedFileBase DeliveryBoyImage { get; set; }
        public HttpPostedFileBase DrivingLicenseImage { get; set; }
        public HttpPostedFileBase DrivingLicensePdf { get; set; }
        public HttpPostedFileBase BankPassbookImage { get; set; }
        public HttpPostedFileBase BankPassbookPdf { get; set; }
        public HttpPostedFileBase CVPdf { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string MarketingAgentCode { get; set; }
        public string MarketingAgentName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string StaffCode { get; set; }
        public string StaffName { get; set; }
        public string ImagePath { get; set; }
        public string CVPath { get; set; }
        public string DrivingLicenseImagePath { get; set; }
        public string BankPassbookPath { get; set; }
        public string LicenseNumber { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string StreetName { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
        public int DeliveryBoyShopCount { get; set; }
        public int Count { get; set; }
        public bool LicenseApprove { get; set; }
        public bool AccountApprove { get; set; }

    }
    public class DeliveryBoyListViewModel
    {
        public List<DeliveryBoyList> List { get; set; }
        public class DeliveryBoyList
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string ShopCode { get; set; }
            public string ShopName { get; set; }
            public string ImagePath { get; set; }
        }
    }

    public class DeliveryBoyPlacesListViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string StaffCode { get; set; }
        public string StaffName { get; set; }
        public string ImagePath { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string StreetName { get; set; }
        public string Address { get; set; }
        public int WorkType { get; set; }
        public string PinCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public TimeSpan WorkStartTime { get; set; }
        public TimeSpan WorkEndTime { get; set; }
        public int DeliveryBoyShopCount { get; set; }

        public List<Places> List { get; set; }
        public class Places
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string ImagePath { get; set; }
            public string Address { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public int Status { get; set; }
            public double Meters { get; set; }
        }
    }

    public class FranchiseViewModel
    {
        public string DeliveryBoyCode { get; set; }
        public string DeliveryBoyName { get; set; }
        public string MarketingAgentCode { get; set; }
        public string MarketingAgentName { get; set; }
        public List<FranchiseList> Lists { get; set; }
        public class FranchiseList
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string MarketingAgentCode { get; set; }
            public string MarketingAgentName { get; set; }
        }
    }

    public class DeliveryBoyCreditAmountViewModel
    {
        public string Code { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string DeliveryBoyCode { get; set; }
        public string DeliveryBoyName { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string OrderNo { get; set; }
        public int CartStatus { get; set; }
        public double GrossDeliveryCharge { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
        public List<CreditAmountList> List { get; set; }
        public class CreditAmountList
        {
            public string Code { get; set; }
            public string CustomerCode { get; set; }
            public string CustomerName { get; set; }
            public string DeliveryBoyCode { get; set; }
            public string DeliveryBoyName { get; set; }
            public string OrderNo { get; set; }
            public int CartStatus { get; set; }
            public double GrossDeliveryCharge { get; set; }
            public double TotalAmount { get; set; }
        }
    }

    //Api
    public class DeliveryBoyCreateViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
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
        public string CVPath { get; set; }
        public string DrivingLicenseImagePath { get; set; }
        public string BankPassbookPath { get; set; }
        public string LicenseNumber { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSCCode { get; set; }
    }

    public class DeliveryBoyExistViewModel
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class DeliveryBoyViewModel
    {
        public int DeliveryBoyId { get; set; }
        public string DeliveryBoyName { get; set; }
        public string DeliveryBoyPhoneNumber { get; set; }
        public int CartStatus { get; set; }
        public double ShopLatitude { get; set; }
        public double ShopLongitude { get; set; }
        public double CustomerLatitude { get; set; }
        public double CustomerLongitude { get; set; }
    }

    public class DeliveryBoyApiListViewModel
    {
        public List<DeliveryBoyViewModel> Lists { get; set; }
        public class DeliveryBoyViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int DelivaryCustomerId { get; set; }
            public string DeilvaryCustomerName { get; set; }
            public string DeilvaryName { get; set; }
            public string DeilvaryPhoneNumber { get; set; }
            public string Address { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Meters { get; set; }
            public int Status { get; set; }
        }
    }
}

