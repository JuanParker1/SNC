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
    
    public partial class OtpVerification
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string ReferenceCode { get; set; }
        public string Otp { get; set; }
        public bool Verify { get; set; }
        public int Status { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
        public string ShopCode { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string OrderNo { get; set; }
    }
}
