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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Shop()
        {
            this.Products = new HashSet<Product>();
        }
    
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string ImagePath { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string StreetName { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public string IpAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool AccessStaff { get; set; }
        public int StaffMembers { get; set; }
        public string MemberCode { get; set; }
        public int Status { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public string Email { get; set; }
        public int CustomerReview { get; set; }
        public double Rating { get; set; }
        public string ImageLogoPath { get; set; }
        public string ImageBannerPath { get; set; }
        public string MondayOpenTime { get; set; }
        public string TuesdayOpenTime { get; set; }
        public string WednesdayOpenTime { get; set; }
        public string ThursdayOpenTime { get; set; }
        public string FridayOpenTime { get; set; }
        public string SaturdayOpenTime { get; set; }
        public string SundayOpenTime { get; set; }
        public string OwnerPhoneNumber { get; set; }
        public string ManualPhoneNumber { get; set; }
        public string OfficialEmail { get; set; }
        public string Website { get; set; }
        public string ManualWebsite { get; set; }
        public string MondayCloseTime { get; set; }
        public string TuesdayCloseTime { get; set; }
        public string WednesdayCloseTime { get; set; }
        public string ThursdayCloseTime { get; set; }
        public string FridayCloseTime { get; set; }
        public string SaturdayCloseTime { get; set; }
        public string SundayCloseTime { get; set; }
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
        public string GSTStateCode { get; set; }
        public string GSTStateName { get; set; }
        public string OtherLicenseName { get; set; }
        public string AuthorisedDistributorNumber { get; set; }
        public string AuthorisedBrandCode { get; set; }
        public string AuthorisedBrandName { get; set; }
        public string AcountType { get; set; }
        public string SwiftCode { get; set; }
        public string UPIID { get; set; }
        public string FSSAIStatus { get; set; }
        public string DrugStatus { get; set; }
        public string AuthorisedDistributorStatus { get; set; }
        public string AccountNumber { get; set; }
        public System.DateTime DateUpdated { get; set; }
        public string ShopCategoryCode { get; set; }
        public string ShopCategoryName { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string PincodeRateCode { get; set; }
        public int PincodeRateDeliveryRateSet { get; set; }
        public string MarketingAgentCode { get; set; }
        public string MarketingAgentName { get; set; }
        public int Adscore { get; set; }
        public Nullable<bool> isOnline { get; set; }
        public string GooglePlaceId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
