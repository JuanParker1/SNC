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
    
    public partial class ApiSetting
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int ShopCategoryId { get; set; }
        public string ProviderName { get; set; }
        public string Version { get; set; }
        public string Url { get; set; }
        public string AuthName { get; set; }
        public string AuthKey { get; set; }
        public string Remark { get; set; }
        public int Category { get; set; }
        public int BranchId { get; set; }
        public int OutletId { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
        public int Status { get; set; }
    }
}
