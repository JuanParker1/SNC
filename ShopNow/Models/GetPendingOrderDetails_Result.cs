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
    
    public partial class GetPendingOrderDetails_Result
    {
        public int OrderNumber { get; set; }
        public string Name { get; set; }
        public double TotalPrice { get; set; }
        public string DeliveryAddress { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string DateEncoded { get; set; }
        public long Id { get; set; }
        public double RefundAmount { get; set; }
        public string RefundRemark { get; set; }
        public int ShopId { get; set; }
    }
}
