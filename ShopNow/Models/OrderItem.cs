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
    
    public partial class OrderItem
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public int OrdeNumber { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ImagePath { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Price { get; set; }
        public double MRPPrice { get; set; }
        public double ShopPrice { get; set; }
        public Nullable<int> OfferId { get; set; }
        public double OfferAmount { get; set; }
        public double MiddlePrice { get; set; }
        public bool HasAddon { get; set; }
        public int AddOnType { get; set; }
        public int Status { get; set; }
        public string UpdateRemarks { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedTime { get; set; }
    }
}
