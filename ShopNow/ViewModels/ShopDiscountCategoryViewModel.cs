using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class ShopDiscountCategoryViewModel
    {
        public string Code { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double DiscountPercentage { get; set; }
        public double LoyaltyPointsper100Value { get; set; }
        public double MinimumLoyaltyReducationPercentage { get; set; }
        public string CategoryName1 { get; set; }
        public string CategoryName2 { get; set; }
        public string CategoryName3 { get; set; }
        public string CategoryName4 { get; set; }
        public string CategoryName5 { get; set; }
        public string CategoryName6 { get; set; }
        public string CategoryName7 { get; set; }
        public string CategoryName8 { get; set; }
        public string CategoryName9 { get; set; }
        public string CategoryName10 { get; set; }
        public string OfferCategoryCode { get; set; }
        public string OfferCategoryName { get; set; }
        public int OfferCategoryType { get; set; }

        public List<DiscountCategoryList> List { get; set; }
        public class DiscountCategoryList
        {
            public int Id { get; set; }
            public int ProductId { get; set; }
            public string Code { get; set; }
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public double DiscountPercentage { get; set; }
            public double LoyaltyPointsper100Value { get; set; }
            public double MinimumLoyaltyReducationPercentage { get; set; }
            public string CategoryName { get; set; }
            public string OfferCategoryCode { get; set; }
            public string OfferCategoryName { get; set; }
            public int OfferCategoryType { get; set; }
        }
        public List<CategoryList> CategoryLists { get; set; }
        public class CategoryList
        {
            public int Id { get; set; }
            public double DiscountPercentage { get; set; }
            public string CategoryName { get; set; }
            public int CategoryId { get; set; }
            public int Type { get; set; }
            public int CategoryType { get; set; }
        }
    }
    public class ShopDiscountCategoryEditViewModel
    {
        public string Code { get; set; }
        public string OfferCategoryCode { get; set; }
        public string Name { get; set; }
        public double Percentage { get; set; }
        public int Type { get; set; }
        public int CategoryType { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}