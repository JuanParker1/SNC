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
    
    public partial class ShopBillDetail
    {
        public int Id { get; set; }
        public string BillNo { get; set; }
        public double BillAmount { get; set; }
        public int OrderNumber { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public string UpdatedBy { get; set; }
    }
}