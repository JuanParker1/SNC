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
    
    public partial class DistanceSetting
    {
        public int Id { get; set; }
        public double Distance { get; set; }
        public Nullable<System.TimeSpan> Time { get; set; }
        public int Status { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDateTime { get; set; }
    }
}
