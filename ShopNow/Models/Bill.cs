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
    
    public partial class Bill
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int NameOfBill { get; set; }
        public double ConvenientCharge { get; set; }
        public double PackingCharge { get; set; }
        public double TotalAmount { get; set; }
        public int Distance { get; set; }
        public int ItemType { get; set; }
        public double DeliveryChargeKM { get; set; }
        public double DeliveryChargeOneKM { get; set; }
        public double DeliveryChargeCustomer { get; set; }
        public int DeliveryRateSet { get; set; }
        public double PlatformCreditRate { get; set; }
        public int PlatformCreditRateId { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
        public int VehicleType { get; set; }
    }
}
