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
    
    public partial class PushNotification
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string District { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string RedirectUrl { get; set; }
        public int Status { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public string EncodedBy { get; set; }
    }
}