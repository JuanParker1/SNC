using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class ProductMedicalStockCreateViewModel
    {
        public string Code { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double Stock { get; set; }
        public string SupplierName { get; set; }
        public double MRP { get; set; }
        public double SalePrice { get; set; }
        public double TaxPercentage { get; set; }
        public double DiscountPercentage { get; set; }
        public double LoyaltyPointsper100Value { get; set; }
        public double MinimumLoyaltyReducationPercentage { get; set; }
        public double SpecialCostOfDelivery { get; set; }
        public int OutLetId { get; set; }
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
        public string ItemTimeStamp { get; set; }
        public double SpecialPrice { get; set; }
        public int MinSaleQty { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
    }

    public class ProductMedicalStockListViewModel
    {
        public List<ProductMedicalStockList> List { get; set; }
        public class ProductMedicalStockList
        { 
        public string Code { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double Stock { get; set; }
        public string SupplierName { get; set; }
        public double MRP { get; set; }
        public double SalePrice { get; set; }
        public double TaxPercentage { get; set; }
        public double DiscountPercentage { get; set; }
        public double LoyaltyPointsper100Value { get; set; }
        public double MinimumLoyaltyReducationPercentage { get; set; }
        public double SpecialCostOfDelivery { get; set; }
        public int OutLetId { get; set; }
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
        public string ItemTimeStamp { get; set; }
        public double SpecialPrice { get; set; }
        public int MinSaleQty { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
        }
    }


}