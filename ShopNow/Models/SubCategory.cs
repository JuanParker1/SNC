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
    
    public partial class SubCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ProductType { get; set; }
        public int Adscore { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
    }
}
