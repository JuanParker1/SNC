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
    
    public partial class ShopSchedule
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public Nullable<System.TimeSpan> OnTime { get; set; }
        public Nullable<System.TimeSpan> OffTime { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime DateTimeUpdated { get; set; }
        public int Status { get; set; }
    }
}
