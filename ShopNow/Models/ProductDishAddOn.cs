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
    
    public partial class ProductDishAddOn
    {
        public int Id { get; set; }
        public string AddOnItemName { get; set; }
        public Nullable<int> MasterProductId { get; set; }
        public string MasterProductName { get; set; }
        public int AddOnCategoryId { get; set; }
        public string AddOnCategoryName { get; set; }
        public string CrustName { get; set; }
        public int Status { get; set; }
        public int PortionId { get; set; }
        public string PortionName { get; set; }
        public int MinSelectionLimit { get; set; }
        public int MaxSelectionLimit { get; set; }
        public double PortionPrice { get; set; }
        public double AddOnsPrice { get; set; }
        public double CrustPrice { get; set; }
        public int AddOnType { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
