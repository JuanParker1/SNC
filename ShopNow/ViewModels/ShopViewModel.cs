using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class ShopRegisterViewModel
    {
        public HttpPostedFileBase AadharPdf { get; set; }
        public HttpPostedFileBase FSSAIPdf { get; set; }
        public HttpPostedFileBase DrugPdf { get; set; }
        public HttpPostedFileBase EstablishPdf { get; set; }
        public HttpPostedFileBase OtherLicensePdf { get; set; }
        public HttpPostedFileBase AuthorisedDistributorPdf { get; set; }
        public HttpPostedFileBase GSTINPdf { get; set; }
        public HttpPostedFileBase PanPdf { get; set; }
        public HttpPostedFileBase AccountPdf { get; set; }
        public HttpPostedFileBase AadharImage { get; set; }
        public HttpPostedFileBase ShopImage { get; set; }
        public HttpPostedFileBase LogoImage { get; set; }
        public HttpPostedFileBase BannerImage { get; set; }
        public HttpPostedFileBase LicenseImage { get; set; }
        public HttpPostedFileBase PanImage { get; set; }
        public HttpPostedFileBase AccountImage { get; set; }
        public HttpPostedFileBase GSTINImage { get; set; }
        public HttpPostedFileBase FSSAIImage { get; set; }
        public HttpPostedFileBase DrugImage { get; set; }
        public HttpPostedFileBase EstablishImage { get; set; }
        public HttpPostedFileBase OtherLicenseImage { get; set; }
        public HttpPostedFileBase AuthorisedDistributorImage { get; set; }
        public string Otp { get; set; }
        
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string ImagePath { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string StreetName { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool AccessStaff { get; set; }
        public int StaffMembers { get; set; }
        public string MemberCode { get; set; }
        public string Email { get; set; }
        public int CustomerReview { get; set; }
        public double Rating { get; set; }
        public string ImageLogoPath { get; set; }
        public string OwnerPhoneNumber { get; set; }
        public string ManualPhoneNumber { get; set; }
        public string OfficialEmail { get; set; }
        public string Website { get; set; }
        public bool Verify { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string IFSCCode { get; set; }
        public string ImageAccountPath { get; set; }
        public string ImageGSTINPath { get; set; }
        public string ImagePanPath { get; set; }
        public string ImageFSSAIPath { get; set; }
        public string ImageDrugPath { get; set; }
        public string ImageEstablishPath { get; set; }
        public string ImageOtherLicensePath { get; set; }
        public string FSSAINumber { get; set; }
        public string DrugNumber { get; set; }
        public string EstablishNumber { get; set; }
        public string OtherLicenseNumber { get; set; }
        public string ImageAadharPath { get; set; }
        public string AadharNumber { get; set; }
        public string ImageAuthoriseBrandPath { get; set; }
        public string AadharName { get; set; }
        public string GSTINNumber { get; set; }
        public string PanNumber { get; set; }
        public string OtherLicenseName { get; set; }
        public string AuthorisedDistributorNumber { get; set; }
        public Nullable<int> AuthorisedBrandId { get; set; }
        public string AuthorisedBrandName { get; set; }
        public string AcountType { get; set; }
        public string SwiftCode { get; set; }
        public string UPIID { get; set; }
        public string FSSAIStatus { get; set; }
        public string DrugStatus { get; set; }
        public string AuthorisedDistributorStatus { get; set; }
        public string AccountNumber { get; set; }
        public int ShopCategoryId { get; set; }
        public string ShopCategoryName { get; set; }
        public int PincodeRateId { get; set; }
        public int PincodeRateDeliveryRateSet { get; set; }
        public int MarketingAgentId { get; set; }
        public string MarketingAgentName { get; set; }
        public int Adscore { get; set; }
        public Nullable<bool> IsOnline { get; set; }
        public string GooglePlaceId { get; set; }
        public int Count { get; set; }
    }

    public class ShopEditViewModel
    {
        public HttpPostedFileBase AadharPdf { get; set; }
        public HttpPostedFileBase FSSAIPdf { get; set; }
        public HttpPostedFileBase DrugPdf { get; set; }
        public HttpPostedFileBase EstablishPdf { get; set; }
        public HttpPostedFileBase OtherLicensePdf { get; set; }
        public HttpPostedFileBase AuthorisedDistributorPdf { get; set; }
        public HttpPostedFileBase GSTINPdf { get; set; }
        public HttpPostedFileBase PanPdf { get; set; }
        public HttpPostedFileBase AccountPdf { get; set; }
        public HttpPostedFileBase AadharImage { get; set; }
        public HttpPostedFileBase ShopImage { get; set; }
        public HttpPostedFileBase LogoImage { get; set; }
        public HttpPostedFileBase BannerImage { get; set; }
        public HttpPostedFileBase LicenseImage { get; set; }
        public HttpPostedFileBase PanImage { get; set; }
        public HttpPostedFileBase AccountImage { get; set; }
        public HttpPostedFileBase GSTINImage { get; set; }
        public HttpPostedFileBase FSSAIImage { get; set; }
        public HttpPostedFileBase DrugImage { get; set; }
        public HttpPostedFileBase EstablishImage { get; set; }
        public HttpPostedFileBase OtherLicenseImage { get; set; }
        public HttpPostedFileBase AuthorisedDistributorImage { get; set; }
        public string Otp { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string ImagePath { get; set; }
        public int CustomerId { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string StreetName { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool AccessStaff { get; set; }
        public int StaffMembers { get; set; }
        public string MemberCode { get; set; }
        public string Email { get; set; }
        public int CustomerReview { get; set; }
        public double Rating { get; set; }
        public string ImageLogoPath { get; set; }
        public string OwnerPhoneNumber { get; set; }
        public string OfficialEmail { get; set; }
        public string Website { get; set; }
        public bool Verify { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string IFSCCode { get; set; }
        public string ImageAccountPath { get; set; }
        public string ImageGSTINPath { get; set; }
        public string ImagePanPath { get; set; }
        public string ImageFSSAIPath { get; set; }
        public string ImageDrugPath { get; set; }
        public string ImageEstablishPath { get; set; }
        public string ImageOtherLicensePath { get; set; }
        public string FSSAINumber { get; set; }
        public string DrugNumber { get; set; }
        public string EstablishNumber { get; set; }
        public string OtherLicenseNumber { get; set; }
        public string ImageAadharPath { get; set; }
        public string AadharNumber { get; set; }
        public string ImageAuthoriseBrandPath { get; set; }
        public string AadharName { get; set; }
        public string GSTINNumber { get; set; }
        public string PanNumber { get; set; }
        public string OtherLicenseName { get; set; }
        public string AuthorisedDistributorNumber { get; set; }
        public Nullable<int> AuthorisedBrandId { get; set; }
        public string AuthorisedBrandName { get; set; }
        public string AcountType { get; set; }
        public string SwiftCode { get; set; }
        public string UPIID { get; set; }
        public string FSSAIStatus { get; set; }
        public string DrugStatus { get; set; }
        public string AuthorisedDistributorStatus { get; set; }
        public string AccountNumber { get; set; }
        public int ShopCategoryId { get; set; }
        public string ShopCategoryName { get; set; }
        public int PincodeRateId { get; set; }
        public int PincodeRateDeliveryRateSet { get; set; }
        public int MarketingAgentId { get; set; }
        public string MarketingAgentName { get; set; }
        public int Adscore { get; set; }
        public Nullable<bool> IsOnline { get; set; }
        public string GooglePlaceId { get; set; }
        public int Status { get; set; }
        public string Password { get; set; }
        public int Count { get; set; }
        public bool FSSIApprove { get; set; }
        public bool DrugApprove { get; set; }
        public bool EstablishApprove { get; set; }
        public bool OtherApprove { get; set; }
        public bool AuthApprove { get; set; }
        public bool GstinApprove { get; set; }
        public bool PanApprove { get; set; }
        public bool AccountApprove { get; set; }
        public int Type { get; set; }
        public bool PhoneVerify { get; set; }
        public int DeliveryType { get; set; }
        public int DeliveryTierType { get; set; }
        public double PlatformCredit { get; set; }
        public double DeliveryCredit { get; set; }
    }

    public class ShopListViewModel
    {
        public List<ShopList> List { get; set; }
        public class ShopList
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ShopCategoryName { get; set; }
            public string ImagePath { get; set; }
            public string Email { get; set; }
            public string PinCode { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string PhoneNumber { get; set; }
            public string OwnerPhoneNumber { get; set; }
            public string ManualPhoneNumber { get; set; }
            public string Address { get; set; }
            public string CustomerName { get; set; }
            public string StateName { get; set; }
            public string DistrictName { get; set; }
            public string ShopCategoryId { get; set; }
            public string MemberCode { get; set; }
            public string FSSAIStatus { get; set; }
            public string DrugStatus { get; set; }
            public string AuthorisedDistributorStatus { get; set; }
            public DateTime DateEncoded { get; set; }
        }

    }

    public class ShopFranchiseViewModel
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int MarketingAgentId { get; set; }//clarify
        public string MarketingAgentName { get; set; }
        public List<FranchiseList> List { get; set; }
        public class FranchiseList
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int MarketingAgentId { get; set; }
            public string MarketingAgentName { get; set; }
        }
    }

    public class ShopCreditViewModel
    {
        public List<ListItem> ListItems { get; set; }
        public List<List> Lists { get; set; }
        public class ListItem
        {
            public int Id { get; set; }
            public string ShopName { get; set; }
            public string ShopOwnerName{ get; set; }
            public string ShopOwnerPhoneNumber { get; set; }
            public double PlatformCredit { get; set; }
            public double DeliveryCredit { get; set; }
            public string PlatformCreditCssColor { get; set; }
            public string DeliveryCreditCssColor { get; set; }
        }
        public class List
        {
            public long Id { get; set; }
            public string ShopName { get; set; }
            public string ShopOwnerName { get; set; }
            public string ShopOwnerPhoneNumber { get; set; }
            public double Amount { get; set; }
            public int CreditType { get; set; }
            public string CreditTypeText
            {
                get
                {
                    if (CreditType == 0)
                    {
                        return "Platform Credits";
                    }
                    else
                    {
                        return "DeliveryCredits";
                    }
                }
            }
        }
    }

    //Api
    public class ShopCreateViewModel
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string ImagePath { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string StreetName { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool AccessStaff { get; set; }
        public int StaffMembers { get; set; }
        public string MemberCode { get; set; }
        public string Email { get; set; }
        public int CustomerReview { get; set; }
        public double Rating { get; set; }
        public string ImageLogoPath { get; set; }
        public string OwnerPhoneNumber { get; set; }
        public string ManualPhoneNumber { get; set; }
        public string OfficialEmail { get; set; }
        public string Website { get; set; }
        public bool Verify { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string IFSCCode { get; set; }
        public string ImageAccountPath { get; set; }
        public string ImageGSTINPath { get; set; }
        public string ImagePanPath { get; set; }
        public string ImageFSSAIPath { get; set; }
        public string ImageDrugPath { get; set; }
        public string ImageEstablishPath { get; set; }
        public string ImageOtherLicensePath { get; set; }
        public string FSSAINumber { get; set; }
        public string DrugNumber { get; set; }
        public string EstablishNumber { get; set; }
        public string OtherLicenseNumber { get; set; }
        public string ImageAadharPath { get; set; }
        public string AadharNumber { get; set; }
        public string ImageAuthoriseBrandPath { get; set; }
        public string AadharName { get; set; }
        public string GSTINNumber { get; set; }
        public string PanNumber { get; set; }
        public string OtherLicenseName { get; set; }
        public string AuthorisedDistributorNumber { get; set; }
        public Nullable<int> AuthorisedBrandId { get; set; }
        public string AuthorisedBrandName { get; set; }
        public string AcountType { get; set; }
        public string SwiftCode { get; set; }
        public string UPIID { get; set; }
        public string FSSAIStatus { get; set; }
        public string DrugStatus { get; set; }
        public string AuthorisedDistributorStatus { get; set; }
        public string AccountNumber { get; set; }
        public int ShopCategoryId { get; set; }
        public string ShopCategoryName { get; set; }
        public Nullable<int> PincodeRateId { get; set; }
        public int PincodeRateDeliveryRateSet { get; set; }
        public string MarketingAgentId { get; set; }
        public string MarketingAgentName { get; set; }
        public int Adscore { get; set; }
        public Nullable<bool> isOnline { get; set; }
        public string GooglePlaceId { get; set; }
    }


    public class ShopUpdateViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string ImagePath { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string StreetName { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool AccessStaff { get; set; }
        public int StaffMembers { get; set; }
        public string MemberCode { get; set; }
        public string Email { get; set; }
        public int CustomerReview { get; set; }
        public double Rating { get; set; }
        public string ImageLogoPath { get; set; }
        public string OwnerPhoneNumber { get; set; }
        public string ManualPhoneNumber { get; set; }
        public string OfficialEmail { get; set; }
        public string Website { get; set; }
        public bool Verify { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string IFSCCode { get; set; }
        public string ImageAccountPath { get; set; }
        public string ImageGSTINPath { get; set; }
        public string ImagePanPath { get; set; }
        public string ImageFSSAIPath { get; set; }
        public string ImageDrugPath { get; set; }
        public string ImageEstablishPath { get; set; }
        public string ImageOtherLicensePath { get; set; }
        public string FSSAINumber { get; set; }
        public string DrugNumber { get; set; }
        public string EstablishNumber { get; set; }
        public string OtherLicenseNumber { get; set; }
        public string ImageAadharPath { get; set; }
        public string AadharNumber { get; set; }
        public string ImageAuthoriseBrandPath { get; set; }
        public string AadharName { get; set; }
        public string GSTINNumber { get; set; }
        public string PanNumber { get; set; }
        public string OtherLicenseName { get; set; }
        public string AuthorisedDistributorNumber { get; set; }
        public Nullable<int> AuthorisedBrandId { get; set; }
        public string AuthorisedBrandName { get; set; }
        public string AcountType { get; set; }
        public string SwiftCode { get; set; }
        public string UPIID { get; set; }
        public string FSSAIStatus { get; set; }
        public string DrugStatus { get; set; }
        public string AuthorisedDistributorStatus { get; set; }
        public string AccountNumber { get; set; }
        public int ShopCategoryId { get; set; }
        public string ShopCategoryName { get; set; }
        public Nullable<int> PincodeRateId { get; set; }
        public int PincodeRateDeliveryRateSet { get; set; }
        public string MarketingAgentId { get; set; }
        public string MarketingAgentName { get; set; }
        public int Adscore { get; set; }
        public Nullable<bool> isOnline { get; set; }
        public string GooglePlaceId { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }


    public class ShopSingleUpdateViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryName { get; set; }
        public string StreetName { get; set; }
        public string Address { get; set; }
        public string ImagePath { get; set; }
        public string MondayOpenTime { get; set; }
        public string TuesdayOpenTime { get; set; }
        public string WednesdayOpenTime { get; set; }
        public string ThursdayOpenTime { get; set; }
        public string FridayOpenTime { get; set; }
        public string SaturdayOpenTime { get; set; }
        public string SundayOpenTime { get; set; }
        public string MondayCloseTime { get; set; }
        public string TuesdayCloseTime { get; set; }
        public string WednesdayCloseTime { get; set; }
        public string ThursdayCloseTime { get; set; }
        public string FridayCloseTime { get; set; }
        public string SaturdayCloseTime { get; set; }
        public string SundayCloseTime { get; set; }
        public string ImageAadharPath { get; set; }
        public string ImageAccountPath { get; set; }
        public string ImageGSTINPath { get; set; }
        public string ImagePanPath { get; set; }
        public string ImageFSSAIPath { get; set; }
        public string ImageDrugPath { get; set; }
        public string ImageEstablishPath { get; set; }
        public string ImageOtherLicensePath { get; set; }
        public string ImageAuthoriseBrandPath { get; set; }
        public string ImageAuthoriseDealerPath { get; set; }
        public string EstablishNumber { get; set; }
        public string OtherLicenseNumber { get; set; }
        public string AadharName { get; set; }
        public string AadharNumber { get; set; }
        public string FSSAINumber { get; set; }
        public string DrugNumber { get; set; }
        public string GSTINNumber { get; set; }
        public string PanNumber { get; set; }
        public string GSTStateCode { get; set; }
        public string GSTStateName { get; set; }
        public string OtherLicenseName { get; set; }
        public string AuthorisedDistributorNumber { get; set; }
        public string AuthorisedBrandCode { get; set; }
        public string AuthorisedBrandName { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string AcountType { get; set; }
        public string IFSCCode { get; set; }
        public string SwiftCode { get; set; }
        public string UPIID { get; set; }
        public string DistributorNumber { get; set; }
    }


    public class ShopSingleEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryName { get; set; }
        public string StreetName { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public string IpAddress { get; set; }
        public string ImagePath { get; set; }
        public string MondayOpenTime { get; set; }
        public string TuesdayOpenTime { get; set; }
        public string WednesdayOpenTime { get; set; }
        public string ThursdayOpenTime { get; set; }
        public string FridayOpenTime { get; set; }
        public string SaturdayOpenTime { get; set; }
        public string SundayOpenTime { get; set; }
        public string MondayCloseTime { get; set; }
        public string TuesdayCloseTime { get; set; }
        public string WednesdayCloseTime { get; set; }
        public string ThursdayCloseTime { get; set; }
        public string FridayCloseTime { get; set; }
        public string SaturdayCloseTime { get; set; }
        public string SundayCloseTime { get; set; }
        public string ImageAadharPath { get; set; }
        public string ImageAccountPath { get; set; }
        public string ImageGSTINPath { get; set; }
        public string ImagePanPath { get; set; }
        public string ImageFSSAIPath { get; set; }
        public string ImageDrugPath { get; set; }
        public string ImageEstablishPath { get; set; }
        public string ImageOtherLicensePath { get; set; }
        public string ImageAuthoriseBrandPath { get; set; }
        public int CustomerReview { get; set; }
        public double Rating { get; set; }
        public string EstablishNumber { get; set; }
        public string OtherLicenseNumber { get; set; }
        public string AadharName { get; set; }
        public string AadharNumber { get; set; }
        public string FSSAINumber { get; set; }
        public string DrugNumber { get; set; }
        public string GSTINNumber { get; set; }
        public string PanNumber { get; set; }
        public string GSTStateCode { get; set; }
        public string GSTStateName { get; set; }
        public string OtherLicenseName { get; set; }
        public string AuthorisedDistributorNumber { get; set; }
        public int AuthorisedBrandId { get; set; }
        public string AuthorisedBrandName { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string AcountType { get; set; }
        public string IFSCCode { get; set; }
        public string SwiftCode { get; set; }
        public string UPIID { get; set; }
        public string DistributorNumber { get; set; }
    }
    public class ShopAllListViewModel
    {
        public List<ShopList> List { get; set; }
        public List<ShopList> ResturantList { get; set; }
        public List<ShopList> SuperMarketList { get; set; }
        public List<ShopList> GroceriesList { get; set; }
        public List<ShopList> HealthList { get; set; }
        public List<ShopList> ElectronicsList { get; set; }
        public List<ShopList> ServicesList { get; set; }
        public List<ShopList> OtherList { get; set; }

        public class ShopList
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int CustomerReview { get; set; }
            public double Rating { get; set; }
            public string PhoneNumber { get; set; }
            public string ImageLogoPath { get; set; }
            public string ImageBannerPath { get; set; }
            public string ImagePath { get; set; }
            public string ImageAuthoriseBrandPath { get; set; }
            public string AuthorisedBrandName { get; set; }
            public string Email { get; set; }
            public string CountryName { get; set; }
            public string StateName { get; set; }
            public string DistrictName { get; set; }
            public string StreetName { get; set; }
            public string Address { get; set; }
            public string ShopCategoryId { get; set; }
            public string ShopCategoryName { get; set; }

        }

    }

    public class CustomerShopAllListViewModel
    {
        public List<ShopList> List { get; set; } 
        public class ShopList
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Otp { get; set; }
            public int CustomerId { get; set; }
            public string Password { get; set; }
            public bool? isOnline { get; set; }
            public double Rating { get; set; }
            public string PhoneNumber { get; set; }
            public string ImagePath { get; set; }
            public string DistrictName { get; set; }
            public int ShopCategoryId { get; set; }
            public string ShopCategoryName { get; set; }
            public int Status { get; set; }
            public bool Verify { get; set; }
            public bool OtpVerify { get; set; }
            public string DateEncoded { get; set; }
            public TimeSpan? NextOnTime { get; set; }
        }
        public class VerifyList
        {
            public int CustomerId { get; set; }
            public string PhoneNumber { get; set; }
            public string Otp { get; set; }
            public bool Verify { get; set; }
            public DateTime DateEncoded { get; set; }
        }

    }
    public class ShopActiveOrInViewModel
    {
        public int ShopId { get; set; }
        public int CustomerId { get; set; }
        public int State { get; set; }
    }
    public class ShopDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CustomerReview { get; set; }
        public double Rating { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public bool? IsOnline { get; set; }
        public TimeSpan? NextOnTime { get; set; }
        public List<ProductList> ProductLists { get; set; }
        public List<CategoryList> CategoryLists { get; set; }
        public List<CategoryList> TrendingCategoryLists { get; set; }
        public class CategoryList
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ImagePath { get; set; }
            public int OrderNo { get; set; }
        }
        public class ProductList
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public int? CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string ColorCode { get; set; }
            public double Price { get; set; }
            public string ImagePath { get; set; }
            public int Status { get; set; }
            public bool Customisation { get; set; }
            public bool IsOnline { get; set; }
            public TimeSpan? NextOnTime { get; set; }
            public double DiscountCategoryPercentage { get; set; }
            public bool IsOffer { get; set; }
            public double Size { get; set; }
            public double Weight { get; set; }
            public bool IsPreorder { get; set; }
            public int PreorderHour { get; set; }
            public int OfferQuantityLimit { get; set; }
            public bool IsLiked { get; set; }
            public string LikeText { get; set; }
        }
        public class DrugCompundDetailList
        {

            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public string ShopCode { get; set; }
            public string ShopName { get; set; }
            public string CategoryCode { get; set; }
            public string CategoryName { get; set; }
            public string ColorCode { get; set; }
            public double Price { get; set; }
            public string ImagePath { get; set; }
            public int Status { get; set; }
            public double MRP { get; set; }
            public double SalePrice { get; set; }
            public int Quantity { get; set; }
        }
    }
    public class ShopCategoryViewModel
    {
        public List<DrugCompundDetailList> DrugLists { get; set; }
        public class DrugCompundDetailList
        {
            public int ProductId { get; set; }
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public string ShopCode { get; set; }
            public string ShopName { get; set; }
            public string CategoryCode { get; set; }
            public string iBarU { get; set; }
            public string CategoryName { get; set; }
            public string ColorCode { get; set; }
            public double Price { get; set; }
            public string DiscountCategoryCode { get; set; }
            public string DiscountCategoryName { get; set; }
            public double DiscountCategoryPercentage { get; set; }
            public int DiscountType { get; set; }
            public string ImagePath1 { get; set; }
            public string ImagePath2 { get; set; }
            public string ImagePath3 { get; set; }
            public string ImagePath4 { get; set; }
            public string ImagePath5 { get; set; }
            public int Status { get; set; }
            public double MRP { get; set; }
            public double SalePrice { get; set; }
            public double Quantity { get; set; }
            public string Itemid { get; set; }
        }
    }

    public class SingleShopImgeUpdateViewModel
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
    }

    public class ShopApiReportsViewModel
    {
        public List<EarningListItem> ListItems { get; set; }
        public class EarningListItem
        {
            //public string Date { get; set; }
            public DateTime DateEncoded { get; set; }
            public double Earning { get; set; }
            public double Pending { get; set; }
            public double Paid { get; set; }
        }

        public List<RefundListItem> RefundLists { get; set; }
        public class RefundListItem
        {
           // public string Date { get; set; }
            public DateTime DateEncoded { get; set; }
            public double Refund { get; set; }
            public double Earning { get; set; }
            public double DeliveryCredits { get; set; }
            public int OrderNo { get; set; }
        }
    }
}