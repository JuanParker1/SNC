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
    
    public partial class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long MasterProductId { get; set; }
        public int CategoryId { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int ShopCategoryId { get; set; }
        public string ShopCategoryName { get; set; }
        public string GTIN { get; set; }
        public string UPC { get; set; }
        public string GTIN14 { get; set; }
        public string EAN { get; set; }
        public string ISBN { get; set; }
        public double Price { get; set; }
        public int Qty { get; set; }
        public string ProductTypeName { get; set; }
        public int ProductTypeId { get; set; }
        public int MinSelectionLimit { get; set; }
        public int MaxSelectionLimit { get; set; }
        public bool Customisation { get; set; }
        public double MenuPrice { get; set; }
        public int IBarU { get; set; }
        public int ItemId { get; set; }
        public double Percentage { get; set; }
        public int DiscountCategoryId { get; set; }
        public string DiscountCategoryName { get; set; }
        public int DataEntry { get; set; }
        public int AppliesOnline { get; set; }
        public bool IsOnline { get; set; }
        public int HoldOnStok { get; set; }
        public int PackingType { get; set; }
        public double TaxPercentage { get; set; }
        public double SpecialCostOfDelivery { get; set; }
        public int OutletId { get; set; }
        public string ItemTimeStamp { get; set; }
        public double LoyaltyPoints { get; set; }
        public double PackingCharge { get; set; }
        public Nullable<int> BrandOwnerMiddlePercentage { get; set; }
        public Nullable<double> ShopOwnerPrice { get; set; }
        public Nullable<bool> HasSchedule { get; set; }
        public Nullable<System.TimeSpan> NextOnTime { get; set; }
        public bool IsPreorder { get; set; }
        public int PreorderHour { get; set; }
        public int OfferQuantityLimit { get; set; }
        public Nullable<System.DateTime> MappedDate { get; set; }
        public int Status { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
