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
    
    public partial class GetProductList_Result
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int iBarU { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public double DiscountCategoryPercentage { get; set; }
        public double MRP { get; set; }
        public double Price { get; set; }
        public int Status { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public bool isOnline { get; set; }
        public int ItemId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int ShopCategoryId { get; set; }
        public string ImagePath { get; set; }
        public string ShopImagePath { get; set; }
        public int Meters { get; set; }
        public Nullable<bool> ShopOnline { get; set; }
        public Nullable<System.TimeSpan> NextOnTime { get; set; }
        public int ShopStatus { get; set; }
        public Nullable<bool> IsOffer { get; set; }
        public double Size { get; set; }
        public double Weight { get; set; }
        public bool IsPreorder { get; set; }
        public int PreorderHour { get; set; }
        public bool PriscriptionCategory { get; set; }
    }
}
