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
    
    public partial class CustomerWalletHistory
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public double Amount { get; set; }
        public int Type { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public string Description { get; set; }
        public int WalletId { get; set; }
        public int Status { get; set; }
        public Nullable<System.DateTime> DateEncoded { get; set; }
    }
}
