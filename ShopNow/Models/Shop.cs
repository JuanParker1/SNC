//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopNow.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Shop
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
        public int PincodeRateId { get; set; }
        public int AgencyId { get; set; }
        public string AgencyName { get; set; }
        public int Adscore { get; set; }
        public Nullable<bool> IsOnline { get; set; }
        public string GooglePlaceId { get; set; }
        public Nullable<bool> HasSchedule { get; set; }
        public Nullable<System.TimeSpan> NextOnTime { get; set; }
        public int DeliveryType { get; set; }
        public int DeliveryTierType { get; set; }
        public bool IsTrail { get; set; }
        public double ShopPricePercentage { get; set; }
        public int Status { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
