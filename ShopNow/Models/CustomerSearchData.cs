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
    
    public partial class CustomerSearchData
    {
        public long Id { get; set; }
        public string SearchKeyword { get; set; }
        public int ResultCount { get; set; }
        public string LinkedMasterProductIds { get; set; }
        public string LinkedMasterProductName { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public int Status { get; set; }
    }
}
