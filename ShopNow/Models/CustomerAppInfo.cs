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
    
    public partial class CustomerAppInfo
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string AppName { get; set; }
        public string AppId { get; set; }
        public string AppBuild { get; set; }
        public string Version { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
    }
}
