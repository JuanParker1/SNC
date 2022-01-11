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
    
    public partial class GetShopCategoryProducts_Result
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string ImagePath { get; set; }
        public string ImagePathLarge1 { get; set; }
        public string ImagePathLarge2 { get; set; }
        public string ImagePathLarge3 { get; set; }
        public string ImagePathLarge4 { get; set; }
        public string ImagePathLarge5 { get; set; }
        public int iBarU { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public double DiscountCategoryPercentage { get; set; }
        public double MRP { get; set; }
        public double SalePrice { get; set; }
        public int Status { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public int Itemid { get; set; }
        public bool IsOnline { get; set; }
        public Nullable<System.TimeSpan> NextOnTime { get; set; }
        public double Size { get; set; }
        public double Weight { get; set; }
        public bool IsPreorder { get; set; }
        public int PreorderHour { get; set; }
        public int OfferQuantityLimit { get; set; }
        public int IsLiked { get; set; }
        public string LikeText { get; set; }
        public int OutletId { get; set; }
    }
}
