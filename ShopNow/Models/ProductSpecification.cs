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
    
    public partial class ProductSpecification
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int SpecificationId { get; set; }
        public string SpecificationName { get; set; }
        public int MasterProductId { get; set; }
        public string MasterProductName { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime DateUpdated { get; set; }
        public System.DateTime DateEncoded { get; set; }
    }
}