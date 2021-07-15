using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShopNow.Models;

namespace ShopNow.ViewModels
{
    public class ProductItemListViewModel
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }

        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int Id { get; set; }
            public int ProductTypeId { get; set; }
            public string ProductTypeName { get; set; }
            public string Name { get; set; }
            public string CategoryName { get; set; }
            public string BrandName { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public double DiscountCategoryPercentage { get; set; }
        }
    }

    public class ProductCreateViewModel
    {
        public HttpPostedFileBase ProductImage1 { get; set; }
        public HttpPostedFileBase ProductImage2 { get; set; }
        public HttpPostedFileBase ProductImage3 { get; set; }
        public HttpPostedFileBase ProductImage4 { get; set; }
        public HttpPostedFileBase ProductImage5 { get; set; }
        public string Name { get; set; }
        public int MasterProductId { get; set; }
        public string MasterProductName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Specification { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string ProductType { get; set; }
        public int ShopCategoryId { get; set; }
        public string ShopCategoryName { get; set; }
        public string MainSNCode { get; set; }
        public string GTIN { get; set; }
        public string UPC { get; set; }
        public string GTIN14 { get; set; }
        public string EAN { get; set; }
        public string ISBN { get; set; }
        public string ImagePath { get; set; }
        public double Price { get; set; }
        public double MenuPrice { get; set; }
        public int Qty { get; set; }
        public int Percentage { get; set; }
        public int Status { get; set; }
        public bool Customisation { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string Product { get; set; }
        public string Entry { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string ImagePathLarge1 { get; set; }
        public string ImagePathLarge2 { get; set; }
        public string ImagePathLarge3 { get; set; }
        public string ImagePathLarge4 { get; set; }
        public string ImagePathLarge5 { get; set; }
        public Nullable<int> PackagingType { get; set; }
        public Nullable<double> PackagingCharge { get; set; }

        // Default Medical Product
        public string MasterProductCode1 { get; set; }
        public string MasterProductName1 { get; set; }
        public string CategoryCode1 { get; set; }
        public string CategoryName1 { get; set; }
        public string BrandCode1 { get; set; }
        public string BrandName1 { get; set; }
        public double Price1 { get; set; }
        public double MenuPrice1 { get; set; }
        public int Qty1 { get; set; }
        public int Percentage1 { get; set; }
        public string DrugMeasurementUnitCode { get; set; }
        public string DrugMeasurementUnitName { get; set; }
        public bool PriscriptionCategory { get; set; }
        public string[] DrugCompoundDetailCode { get; set; }
        public string CombinationDrugCompound { get; set; }
        public string[] DCDCode { get; set; }
        public string iBarU { get; set; }
        public string Manufacturer { get; set; }
        public string OriginCountry { get; set; }
        public double weight { get; set; }
        public string SizeLB { get; set; }
        public string DiscountCategoryCode { get; set; }
        public string DiscountCategoryName { get; set; }
        public double DiscountCategoryPercentage { get; set; }
        public string GoogleTaxonomyCode1 { get; set; }

        // Manual Medical Product
        public string MasterProductCode2 { get; set; }
        public string MasterProductName2 { get; set; }
        public string CategoryCode2 { get; set; }
        public string CategoryName2 { get; set; }
        public string BrandCode2 { get; set; }
        public string BrandName2 { get; set; }
        public double Price2 { get; set; }
        public double MenuPrice2 { get; set; }
        public int Qty2 { get; set; }
        public int Percentage2 { get; set; }
        public string DrugMeasurementUnitCode1 { get; set; }
        public string DrugMeasurementUnitName1 { get; set; }
        public bool PriscriptionCategory1 { get; set; }
        public string[] DrugCompoundDetailCode1 { get; set; }
        public string CombinationDrugCompound1 { get; set; }
        public string iBarU1 { get; set; }
        public string Manufacturer1 { get; set; }
        public string OriginCountry1 { get; set; }
        public double weight1 { get; set; }
        public string SizeLB1 { get; set; }
        public string DiscountCategoryCode1 { get; set; }
        public string DiscountCategoryName1 { get; set; }
        public double DiscountCategoryPercentage1 { get; set; }
        public string GoogleTaxonomyCode2 { get; set; }

        public List<MedicalStockList> MedicalStockLists { get; set; }
        public class MedicalStockList
        {
             
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
            public double SpecialPrice { get; set; }
            public int MinSaleQty { get; set; }
        }
    }

    public class ProductEditViewModel
    {
        public HttpPostedFileBase ProductImage1 { get; set; }
        public HttpPostedFileBase ProductImage2 { get; set; }
        public HttpPostedFileBase ProductImage3 { get; set; }
        public HttpPostedFileBase ProductImage4 { get; set; }
        public HttpPostedFileBase ProductImage5 { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int MasterProductId { get; set; }
        public string MasterProductName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Specification { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string ProductType { get; set; }
        public int ShopCategoryId { get; set; }
        public string ShopCategoryName { get; set; }
        public string MainSNCode { get; set; }
        public string GTIN { get; set; }
        public string UPC { get; set; }
        public string GTIN12 { get; set; }
        public string GTIN13 { get; set; }
        public string GTIN14 { get; set; }
        public string EAN { get; set; }
        public string ISBN { get; set; }
        public string ImagePath { get; set; }
        public double Price { get; set; }
        public double MenuPrice { get; set; }
        public int Qty { get; set; }
        public int Percentage { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool Customisation { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string ImagePathLarge1 { get; set; }
        public string ImagePathLarge2 { get; set; }
        public string ImagePathLarge3 { get; set; }
        public string ImagePathLarge4 { get; set; }
        public string ImagePathLarge5 { get; set; }

        public List<SpecificationList> SpecificationLists { get; set; }
        public class SpecificationList
        {
            public int Id { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public int MasterProductId { get; set; }
            public string MasterProductName { get; set; }
            public int SpecificationId { get; set; }
            public string SpecificationName { get; set; }
            public string Value { get; set; }
        }

        // Medical Product
        public string DrugMeasurementUnitCode { get; set; }
        public string DrugMeasurementUnitName { get; set; }
        public bool PriscriptionCategory { get; set; }
        public string[] DrugCompoundDetailCode { get; set; }
        public string DrugCompoundDetailCode1 { get; set; }
        public string CombinationDrugCompound { get; set; }
        public string iBarU { get; set; }
        public string Manufacturer { get; set; }
        public string OriginCountry { get; set; }
        public double weight { get; set; }
        public string SizeLB { get; set; }
        public string DiscountCategoryCode { get; set; }
        public string DiscountCategoryName { get; set; }
        public double DiscountCategoryPercentage { get; set; }
        public int DiscountType { get; set; }
        public int DiscountCategoryType { get; set; }

        public List<MedicalStockList> MedicalStockLists { get; set; }
        public class MedicalStockList
        {
             
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
            public double SpecialPrice { get; set; }
            public int MinSaleQty { get; set; }
        }
    }

    public class MedicalCreateViewModel
    {
         
        public string Name { get; set; }
        public int MasterProductId { get; set; }
        public string MasterProductName { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string BrandId { get; set; }
        public string BrandName { get; set; }
        public string ProductType { get; set; }
        public double Price { get; set; }
        public double MenuPrice { get; set; }
        public int Qty { get; set; }
        public int Percentage { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string ImagePathLarge1 { get; set; }
        public string ImagePathLarge2 { get; set; }
        public string ImagePathLarge3 { get; set; }
        public string ImagePathLarge4 { get; set; }
        public string ImagePathLarge5 { get; set; }
        public string DrugMeasurementUnitCode { get; set; }
        public string DrugMeasurementUnitName { get; set; }
        public bool PriscriptionCategory { get; set; }
        public string DrugCompoundDetailCode { get; set; }
        public string CombinationDrugCompound { get; set; }
        public string iBarU { get; set; }
        public string Manufacturer { get; set; }
        public string OriginCountry { get; set; }
        public double weight { get; set; }
        public string SizeLB { get; set; }
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public int DiscountCategoryId { get; set; }
        public string DiscountCategoryName { get; set; }
        public double DiscountCategoryPercentage { get; set; }
        public int DiscountType { get; set; }
        public int DiscountCategoryType { get; set; }
       
        public List<MedicalStockList> MedicalStockLists { get; set; }
        public class MedicalStockList
        {
            public string ProductId { get; set; }
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
            public double SpecialPrice { get; set; }
            public int MinSaleQty { get; set; }
            public Nullable<int> productid { get; set; }
        }
    }

    public class MedicalEditViewModel
    {
        public int Id { get; set; }
        public int MasterProductId { get; set; }
        public string MasterProductName { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public double Price { get; set; }
        public double MenuPrice { get; set; }
        public int Qty { get; set; }
        public int Percentage { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string ImagePathLarge1 { get; set; }
        public string ImagePathLarge2 { get; set; }
        public string ImagePathLarge3 { get; set; }
        public string ImagePathLarge4 { get; set; }
        public string ImagePathLarge5 { get; set; }
        public string DrugMeasurementUnitCode { get; set; }
        public string DrugMeasurementUnitName { get; set; }
        public bool PriscriptionCategory { get; set; }
        public string DrugCompoundDetailCode { get; set; }
        public string CombinationDrugCompound { get; set; }
        public string iBarU { get; set; }
        public string Manufacturer { get; set; }
        public string OriginCountry { get; set; }
        public double weight { get; set; }
        public string SizeLB { get; set; }
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public string DiscountCategoryCode { get; set; }
        public string DiscountCategoryName { get; set; }
        public double DiscountCategoryPercentage { get; set; }
        public int DiscountType { get; set; }
        public int DiscountCategoryType { get; set; }
        public Nullable<int> PackagingType { get; set; }
        public Nullable<double> PackagingCharge { get; set; }

        public List<MedicalStockList> MedicalStockLists { get; set; }
        public class MedicalStockList
        {
             
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
            public double SpecialPrice { get; set; }
            public int MinSaleQty { get; set; }
            public Nullable<int> productid { get; set; }
        }
    }

    public class ElectronicListViewModel
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }

        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
             public int Id { get; set; }
            public string Name { get; set; }
            public string CategoryName { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public double Percentage { get; set; }
        }
    }

    public class FMCGListViewModel
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }

        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
             
            public int Id { get; set; }
            public string Name { get; set; }
            public string CategoryName { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public double Percentage { get; set; }
        }
    }

    public class MedicalListViewModel
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }

        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string CategoryName { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public double Percentage { get; set; }
        }
    }

    public class FoodListViewModel
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }

        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string CategoryName { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public double Percentage { get; set; }
        }
    }

   
    public class ServiceCreateEditViewModel
    {
         public int Id { get; set; }
        public string Name { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ProductType { get; set; }
        public string ImagePath { get; set; }
        public double Price { get; set; }
        public int Qty { get; set; }
    }

    public class ProductListViewModel
    {
        public List<ProductList> List { get; set; }
        public class ProductList
        {
             
            public string Name { get; set; }
            public string MasterProductName { get; set; }
            public string CategoryName { get; set; }
            public string BrandName { get; set; }
            public string ShopName { get; set; }
            public string ImagePath { get; set; }
            public string ProductType { get; set; }
            public string ShopCategoryName { get; set; }
        }
        public List<MasterDishList> MasterDishLists { get; set; }
        public class MasterDishList
        {
             
            public string Name { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public bool Customisation { get; set; }
            public string ColorCode { get; set; }
            public int BrandId { get; set; }
            public string BrandName { get; set; }
            public string ShortDescription { get; set; }
            public string LongDescription { get; set; }
            public string ImagePath { get; set; }
            public double Price { get; set; }
            public string ProductType { get; set; }
        }
    }

    public class AddOnsCreateViewModel
    {
        public int Id { get; set; }
         
        public string Name { get; set; }
        public string AddOnCategoryCode { get; set; }
        public string AddOnCategoryName { get; set; }
        public string PortionCode { get; set; }
        public string PortionName { get; set; }
        public string CrustName { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public int MinSelectionLimit { get; set; }
        public int MaxSelectionLimit { get; set; }
        public double PortionPrice { get; set; }
        public double AddOnsPrice { get; set; }
        public double CrustPrice { get; set; }
        public int MasterProductId { get; set; }
        public string MasterProductName { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
      
        public List<DishList> DishLists { get; set; }
        public class DishList
        {
            public int Id { get; set; }
             
            public string Name { get; set; }
            public int MasterProductId { get; set; }
            public string MasterProductName { get; set; }
            public int AddOnCategoryId{ get; set; }
            public string AddOnCategoryName { get; set; }
            public int PortionId { get; set; }
            public string PortionName { get; set; }
            public string CrustName { get; set; }
            public int Qty { get; set; }
            public double Price { get; set; }
            public int MinSelectionLimit { get; set; }
            public int MaxSelectionLimit { get; set; }
            public double PortionPrice { get; set; }
            public double AddOnsPrice { get; set; }
            public double CrustPrice { get; set; }
            public string CreatedBy { get; set; }
            public string UpdatedBy { get; set; }
        }
    }

    public class SpecificationCreateViewModel
    {
        public List<List> Lists { get; set; }
        public class List
        {
            public int MasterProductId { get; set; }
            public string MasterProductName { get; set; }
            public int SpecificationId { get; set; }
            public string SpecificationName { get; set; }
            public string Value { get; set; }
        }
    }

    public class DefaultMedicalStockViewModel
    {
        public int Id { get; set; }
         
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
        public double SpecialPrice { get; set; }
        public int MinSaleQty { get; set; }
    }

    public class ManualMedicalStockViewModel
    {
        public int Id { get; set; }
         
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double Stock1 { get; set; }
        public string SupplierName1 { get; set; }
        public double MRP1 { get; set; }
        public double SalePrice1 { get; set; }
        public double TaxPercentage1 { get; set; }
        public double DiscountPercentage1 { get; set; }
        public double LoyaltyPointsper100Value1 { get; set; }
        public double MinimumLoyaltyReducationPercentage1 { get; set; }
        public double SpecialCostOfDelivery1 { get; set; }
        public int OutLetId1 { get; set; }
        public double SpecialPrice1 { get; set; }
        public int MinSaleQty1 { get; set; }
    }

    public class ProductMappingViewModel
    {

        public List<List> Lists { get; set; }
        public class List
        {
            public string ItemId { get; set; }
            public string Name { get; set; }
            public int MasterProductId { get; set; }
            public string MasterProductName { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public int BrandId { get; set; }
            public string BrandName { get; set; }
            public string DrugMeasurementUnitCode { get; set; }
            public string DrugMeasurementUnitName { get; set; }
            public string GoogleTaxonomyCode { get; set; }
            public string ShortDescription { get; set; }
            public string LongDescription { get; set; }
            public string Specification { get; set; }
            public string ShopCategoryCode { get; set; }
            public string ShopCategoryName { get; set; }
            public string ProductType { get; set; }
            public int MinSelectionLimit { get; set; }
            public int MaxSelectionLimit { get; set; }
            public string ColorCode { get; set; }
            public string GTIN { get; set; }
            public string UPC { get; set; }
            public string GTIN12 { get; set; }
            public string GTIN13 { get; set; }
            public string GTIN14 { get; set; }
            public string EAN { get; set; }
            public string ISBN { get; set; }
            public string ImagePath { get; set; }
            public bool Customisation { get; set; }
            public double Price { get; set; }
            public double MenuPrice { get; set; }
            public int Qty { get; set; }
            public int Percentage { get; set; }
            public int DataEntry { get; set; }
            public string DiscountCategoryCode { get; set; }
            public string DiscountCategoryName { get; set; }
            public double DiscountCategoryPercentage { get; set; }
            public bool PriscriptionCategory { get; set; }
            public string DrugCompoundDetailCode { get; set; }
            public string CombinationDrugCompound { get; set; }
            public string iBarU { get; set; }
            public string Manufacturer { get; set; }
            public string OriginCountry { get; set; }
            public double weight { get; set; }
            public string SizeLB { get; set; }
        }

    }

    public class FMCGCreateEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MasterProductId { get; set; }
        public string MasterProductName { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string ShopCategoryCode { get; set; }
        public string ShopCategoryName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryCode { get; set; }
        public string SubCategoryName { get; set; }
        public string NextSubCategoryCode { get; set; }
        public string NextSubCategoryName { get; set; }
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public string MeasurementUnitCode { get; set; }
        public string MeasurementUnitName { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string ImagePath { get; set; }
        public string ImagePathLarge1 { get; set; }
        public string ImagePathLarge2 { get; set; }
        public string ImagePathLarge3 { get; set; }
        public string ImagePathLarge4 { get; set; }
        public string ImagePathLarge5 { get; set; }
        public double Price { get; set; }
        public double MenuPrice { get; set; }
        public string ASIN { get; set; }
        public double weight { get; set; }
        public string SizeLB { get; set; }
        public string ProductType { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public string iBarU { get; set; }
        public string OriginCountry { get; set; }
        public string Manufacturer { get; set; }
        public Nullable<int> shopid { get; set; }
        public int Qty { get; set; }
        public int Percentage { get; set; }
        public Nullable<int> PackagingType { get; set; }
        public Nullable<double> PackagingCharge { get; set; }

    }

    public class ElectronicCreateEditViewModel
    {
         public int Id { get; set; }
        public string Name { get; set; }
        public int MasterProductId { get; set; }
        public string MasterProductName { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string ShopCategoryCode { get; set; }
        public string ShopCategoryName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryCode { get; set; }
        public string SubCategoryName { get; set; }
        public string NextSubCategoryCode { get; set; }
        public string NextSubCategoryName { get; set; }
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public string MeasurementUnitCode { get; set; }
        public string MeasurementUnitName { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string ImagePath { get; set; }
        public double Price { get; set; }
        public double MenuPrice { get; set; }
        public string ASIN { get; set; }
        public double weight { get; set; }
        public string SizeLB { get; set; }
        public string ProductType { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public string iBarU { get; set; }
        public string OriginCountry { get; set; }
        public string Manufacturer { get; set; }
        public Nullable<int> shopid { get; set; }
        public int Qty { get; set; }
        public int Percentage { get; set; }
        public Nullable<int> PackagingType { get; set; }
        public Nullable<double> PackagingCharge { get; set; }

    }
    public class FoodCreateViewModel
    {
         
        public string Name { get; set; }
        public string NickName { get; set; }
        public int MasterProductId { get; set; }
        public string MasterProductName { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ShopCategoryCode { get; set; }
        public string ShopCategoryName { get; set; }
        public string ProductType { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public int MinSelectionLimit { get; set; }
        public int MaxSelectionLimit { get; set; }
        public string ColorCode { get; set; }
        public string ImagePath { get; set; }
        public bool Customisation { get; set; }
        public double Price { get; set; }
        public double MenuPrice { get; set; }
        public int Qty { get; set; }
        public int Percentage { get; set; }
        public int Status { get; set; }
        public string Entry { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string ImagePathLarge1 { get; set; }
        public Nullable<int> PackagingType { get; set; }
        public Nullable<double> PackagingCharge { get; set; }
    }
    public class FoodEditViewModel
    {
        public int Id { get; set; }
         
        public string Name { get; set; }
        public string NickName { get; set; }
        public int MasterProductId { get; set; }
        public string MasterProductName { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ShopCategoryCode { get; set; }
        public string ShopCategoryName { get; set; }
        public string ProductType { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public int MinSelectionLimit { get; set; }
        public int MaxSelectionLimit { get; set; }
        public string ColorCode { get; set; }
        public string ImagePath { get; set; }
        public bool Customisation { get; set; }
        public double Price { get; set; }
        public double MenuPrice { get; set; }
        public int Qty { get; set; }
        public int Percentage { get; set; }
        public int Status { get; set; }
        public string Entry { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string ImagePathLarge1 { get; set; }
        public Nullable<int> PackagingType { get; set; }
        public Nullable<double> PackagingCharge { get; set; }
    }

    public class ShopAddOnSessionAddViewModel
    {
        public int Id { get; set; }
        public double PortionPrice { get; set; }
        public double AddOnsPrice { get; set; }
        public double CrustPrice { get; set; }
    }

    public class ShopAddOnSessionEditViewModel
    {
         public int Id { get; set; }
        public double PortionPrice { get; set; }
        public double AddOnsPrice { get; set; }
        public double CrustPrice { get; set; }
        public bool IsActive { get; set; }
    }


    //Api

    public class ProductSearchViewModel
    {
        public List<ProductLists> ProductList { get; set; }
        public List<ShopLists> ShopList { get; set; }
        public class ProductLists
        {
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public string ImagePath { get; set; }
            public string ShopImagePath { get; set; }
            public int ShopId { get; set; }
           public string ShopName { get; set; }
            public int CategoryId { get; set; }
            public bool ? isOnline { get; set; }
            public string CategoryName { get; set; }
            public string DiscountCategoryCode { get; set; }
            public string DiscountCategoryName { get; set; }
            public double DiscountCategoryPercentage { get; set; }
            public int DiscountCategoryType { get; set; }
            public int DiscountType { get; set; }
            public string iBarU { get; set; }
            public double Price { get; set; }
            public double MRP { get; set; }
            public double Quantity { get; set; }
            public int Status { get; set; }
            public string ShopCategoryCode { get; set; }
            public string ItemId { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Meters { get; set; }
            public Boolean ShopOnline { get; set; }
            public int ShopStatus { get; set; }
        }

        public class ShopLists
        {

            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public string ImagePath { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string DistrictName { get; set; }
            public double Rating { get; set; }
            public int ShopCategoryId { get; set; }
            public double Meters { get; set; }
            public Boolean ? ShopOnline { get; set; }
            public int ShopStatus { get; set; }
        }
    }

    public class ProductDetailsViewModel
    {
         
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Specification { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string ProductType { get; set; }
        public string ShopCategoryCode { get; set; }
        public string ShopCategoryName { get; set; }
        public string MainSNCode { get; set; }     
        public string ImagePath { get; set; }
        public double Price { get; set; }
        public int Qty { get; set; }
  
    }

    public class ProductQuickUpdateViewModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public double Price { get; set; }
        public double MenuPrice { get; set; }
        public int Qty { get; set; }

    }

    public class ActiveProductListViewModel
    {
        public List<ProductList> List { get; set; }
        public class ProductList
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public double Price { get; set; }
            public double MenuPrice { get; set; }
            public int Qty { get; set; }
            public int Status { get; set; }
        }
       
    }

    public class ProductActiveOrInViewModel
    {
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int State { get; set; }
    }

    public class ProductStockCheckViewModel
    { 
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public string ItemId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
        }
    }


}

