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
    
    public partial class PaymentsData
    {
        public long Id { get; set; }
        public string PaymentId { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public string Entity { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Currency { get; set; }
        public Nullable<int> Status { get; set; }
        public string Order_Id { get; set; }
        public string Invoice_Id { get; set; }
        public string Method { get; set; }
        public Nullable<decimal> Fee { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<System.DateTime> DateEncoded { get; set; }
    }
}
