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
    
    public partial class OrderItemAddon
    {
        public long Id { get; set; }
        public long OrderItemId { get; set; }
        public int AddonId { get; set; }
        public string AddonName { get; set; }
        public double AddonPrice { get; set; }
        public int CrustId { get; set; }
        public string CrustName { get; set; }
        public int PortionId { get; set; }
        public double PortionPrice { get; set; }
        public string PortionName { get; set; }
        public int Status { get; set; }
    }
}
