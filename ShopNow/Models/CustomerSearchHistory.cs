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
    
    public partial class CustomerSearchHistory
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int SearchId { get; set; }
        public string SearchText { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
    }
}
