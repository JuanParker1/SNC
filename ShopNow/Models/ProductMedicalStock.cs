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
    
    public partial class ProductMedicalStock
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double Stock { get; set; }
        public string SupplierName { get; set; }
        public double MRP { get; set; }
        public double SalePrice { get; set; }
        public double TaxPercentage { get; set; }
        public double DiscountPercentage { get; set; }
        public double LoyaltyPointsper100Value { get; set; }
        public double MinimumLoyaltyReducationPercentage { get; set; }
        public double SpecialCostOfDelivery { get; set; }
        public int OutLetId { get; set; }
        public double SpecialPrice { get; set; }
        public int MinSaleQty { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
        public string CategoryName1 { get; set; }
        public string CategoryName2 { get; set; }
        public string CategoryName3 { get; set; }
        public string CategoryName4 { get; set; }
        public string CategoryName5 { get; set; }
        public string CategoryName6 { get; set; }
        public string CategoryName7 { get; set; }
        public string CategoryName8 { get; set; }
        public string CategoryName9 { get; set; }
        public string CategoryName10 { get; set; }
        public string ItemTimeStamp { get; set; }
        public string OfferCategoryCode { get; set; }
        public string OfferCategoryName { get; set; }
        public int OfferCategoryType { get; set; }
        public string ItemId { get; set; }
        public Nullable<int> productid { get; set; }
        public Nullable<double> HoldStock { get; set; }
    
        public virtual Product Product { get; set; }
    }
}
