﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class MainPageModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Customisation { get; set; }
        public string ColorCode { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Specification { get; set; }
        public string ImagePath { get; set; }
        public string ImagePathLarge1 { get; set; }
        public string ImagePathLarge2 { get; set; }
        public string ImagePathLarge3 { get; set; }
        public string ImagePathLarge4 { get; set; }
        public string ImagePathLarge5 { get; set; }
        public string ProductType { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string Price { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }

    public class GroceryPageModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryCode1 { get; set; }
        public string SubCategoryName1 { get; set; }
        public string SubCategoryName2 { get; set; }
        public string weight { get; set; }
        public string SizeLB { get; set; }
        public string BrandName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Specification { get; set; }
        public string ImagePath { get; set; }
        public string ImagePathLarge1 { get; set; }
        public string ImagePathLarge2 { get; set; }
        public string ImagePathLarge3 { get; set; }
        public string ImagePathLarge4 { get; set; }
        public string ImagePathLarge5 { get; set; }
        public string ProductType { get; set; }
        public string ASIN { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string Price { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }

        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public string MeasurementUnitCode { get; set; }
        public string MeasurementUnitName { get; set; }

        public string BrandCode { get; set; }

    }
    public class MasterProductCreateEditViewModel
    {
        public HttpPostedFileBase ProductImage1 { get; set; }
        public HttpPostedFileBase ProductImage2 { get; set; }
        public HttpPostedFileBase ProductImage3 { get; set; }
        public HttpPostedFileBase ProductImage4 { get; set; }
        public HttpPostedFileBase ProductImage5 { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string[] CategoryCode { get; set; }
        public string CategoryCode1 { get; set; }
        public string CategoryName { get; set; }
        public bool Customisation { get; set; }
        public string ColorCode { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string ImagePath { get; set; }
        public double Price { get; set; }
        public string ProductType { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string ImagePathLarge1 { get; set; }
        public string ImagePathLarge2 { get; set; }
        public string ImagePathLarge3 { get; set; }
        public string ImagePathLarge4 { get; set; }
        public string ImagePathLarge5 { get; set; }
        public int Adscore { get; set; }
    }

    public class MasterProductListViewModel
    {
        public List<MasterProductList> List { get; set; }
        public int CurrentPageIndex { get; set; }
        public int ItemCount { get; set; }
        public int PageCount { get; set; }
        public int maxRows { get; set; }
        public class MasterProductList
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string CategoryName { get; set; }
            public string BrandName { get; set; }
            public string ProductType { get; set; }

        }
        public List<PendingList> Lists { get; set; }
        public class PendingList
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string ItemId { get; set; }
            public string ProductType { get; set; }
        }
        public List<MappedList> MappedLists { get; set; }
        public class MappedList
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string MasterProductCode { get; set; }
            public string MasterProductName { get; set; }
            public string ProductType { get; set; }
        }
    }
  
    public class MedicalDrugViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string DrugMeasurementUnitCode { get; set; }
        public string DrugMeasurementUnitName { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
        public string ImagePath { get; set; }
        public string ImagePathLarge1 { get; set; }
        public string ImagePathLarge2 { get; set; }
        public string ImagePathLarge3 { get; set; }
        public string ImagePathLarge4 { get; set; }
        public string ImagePathLarge5 { get; set; }
        public string Price { get; set; }
        public string PriscriptionCategory { get; set; }
        public string DrugCompoundDetailCode { get; set; }
        public string CombinationDrugCompound { get; set; }
        public string iBarU { get; set; }
        public string OriginCountry { get; set; }
        public string Manufacturer { get; set; }
        public string weight { get; set; }
        public string SizeLB { get; set; }
        public string ProductType { get; set; }
        public string PackageName { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }
    public class MedicalDrugCreateEditViewModel
    {
        public HttpPostedFileBase ProductImage1 { get; set; }
        public HttpPostedFileBase ProductImage2 { get; set; }
        public HttpPostedFileBase ProductImage3 { get; set; }
        public HttpPostedFileBase ProductImage4 { get; set; }
        public HttpPostedFileBase ProductImage5 { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string[] CategoryCode { get; set; }
        public string CategoryCode1 { get; set; }
        public string CategoryName { get; set; }
        public string MeasurementUnitCode { get; set; }
        public string MeasurementUnitName { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
        public string ImagePathLarge1 { get; set; }
        public string ImagePathLarge2 { get; set; }
        public string ImagePathLarge3 { get; set; }
        public string ImagePathLarge4 { get; set; }
        public string ImagePathLarge5 { get; set; }
        public double Price { get; set; }
        public int Adscore { get; set; }
        public bool PriscriptionCategory { get; set; }
        public string[] DrugCompoundDetailCode { get; set; }
        public string DrugCompoundDetailCode1 { get; set; }
        public string CombinationDrugCompound { get; set; }
        public string iBarU { get; set; }
        public string OriginCountry { get; set; }
        public string Manufacturer { get; set; }
        public double weight { get; set; }
        public string SizeLB { get; set; }
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
    }

    public class MedicalDrugListViewModel
    {
        public List<MedicalDrugList> List { get; set; }
        public class MedicalDrugList
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Combination { get; set; }
            public string CategoryName { get; set; }
            public string BrandName { get; set; }
            public string ProductType { get; set; }
        }
    }

    public class ItemMappingViewModel
    {
        public string Code { get; set; }
        public string ItemId { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string MasterProductCode { get; set; }
        public string MasterProductName { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string BrandCode { get; set; }
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
        public string MainSNCode { get; set; }
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
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string IsCheck { get; set; }

        public List<ItemMappingList> List { get; set; }
        public class ItemMappingList
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string ImagePath { get; set; }
            public string LongDescription { get; set; }
        }
    }

    public class MasterFMCGCreateEditViewModel
    {
        public HttpPostedFileBase FMCGLargeImage1 { get; set; }
        public HttpPostedFileBase FMCGLargeImage2 { get; set; }
        public HttpPostedFileBase FMCGLargeImage3 { get; set; }
        public HttpPostedFileBase FMCGLargeImage4 { get; set; }
        public HttpPostedFileBase FMCGLargeImage5 { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string[] CategoryCode { get; set; }
        public string CategoryCode1 { get; set; }
        public string CategoryName { get; set; }
        public string[] SubCategoryCode { get; set; }
        public string SubCategoryCode1 { get; set; }
        public string SubCategoryName { get; set; }
        public string[] NextSubCategoryCode { get; set; }
        public string NextSubCategoryCode1 { get; set; }
        public string NextSubCategoryName { get; set; }
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public string MeasurementUnitCode { get; set; }
        public string MeasurementUnitName { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public int Adscore { get; set; }
        public string ImagePath { get; set; }
        public string ImagePathLarge1 { get; set; }
        public string ImagePathLarge2 { get; set; }
        public string ImagePathLarge3 { get; set; }
        public string ImagePathLarge4 { get; set; }
        public string ImagePathLarge5 { get; set; }
        public double Price { get; set; }
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
    }

    public class MasterFoodCreateViewModel
    {
        public HttpPostedFileBase ProductImage1 { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string ProductType { get; set; }
        public int MinSelectionLimit { get; set; }
        public int MaxSelectionLimit { get; set; }
        public string ColorCode { get; set; }
        public bool Customisation { get; set; }
        public double Price { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string ImagePathLarge1 { get; set; }
        public int Adscore { get; set; }

        public List<AddonList> AddonLists { get; set; }
        public class AddonList
        {
            public string Code { get; set; }
            public string AddOnItemName { get; set; }
            public string MasterProductCode { get; set; }
            public string MasterProductName { get; set; }
            public string AddOnCategoryCode { get; set; }
            public string AddOnCategoryName { get; set; }
            public string CrustName { get; set; }
            public string PortionCode { get; set; }
            public string PortionName { get; set; }
            public int MinSelectionLimit { get; set; }
            public int MaxSelectionLimit { get; set; }
            public double PortionPrice { get; set; }
            public double AddOnsPrice { get; set; }
            public double CrustPrice { get; set; }
            public int AddOnType { get; set; }
            public Nullable<int> MasterProductId { get; set; }
        }
    }
    public class MasterFoodEditViewModel
    {
        public HttpPostedFileBase ProductImage1 { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string ProductType { get; set; }
        public int MinSelectionLimit { get; set; }
        public int MaxSelectionLimit { get; set; }
        public string ColorCode { get; set; }
        public bool Customisation { get; set; }
        public double Price { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string ImagePathLarge1 { get; set; }
        public int Adscore { get; set; }

        public List<AddonList> AddonLists { get; set; }
        public class AddonList
        {
            public string Code { get; set; }
            public string AddOnItemName { get; set; }
            public string MasterProductCode { get; set; }
            public string MasterProductName { get; set; }
            public string AddOnCategoryCode { get; set; }
            public string AddOnCategoryName { get; set; }
            public string CrustName { get; set; }
            public string PortionCode { get; set; }
            public string PortionName { get; set; }
            public int MinSelectionLimit { get; set; }
            public int MaxSelectionLimit { get; set; }
            public double PortionPrice { get; set; }
            public double AddOnsPrice { get; set; }
            public double CrustPrice { get; set; }
            public int AddOnType { get; set; }
            public Nullable<int> MasterProductId { get; set; }
        }
    }

    public class MasterAddOnsCreateViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string AddOnItemName { get; set; }
        public string MasterProductCode { get; set; }
        public string MasterProductName { get; set; }
        public string AddOnCategoryCode { get; set; }
        public string AddOnCategoryName { get; set; }
        public string CrustName { get; set; }
        public string PortionCode { get; set; }
        public string PortionName { get; set; }
        public int MinSelectionLimit { get; set; }
        public int MaxSelectionLimit { get; set; }
        public double PortionPrice { get; set; }
        public double AddOnsPrice { get; set; }
        public double CrustPrice { get; set; }
        public int AddOnType { get; set; }
        public Nullable<int> MasterProductId { get; set; }

        public List<DishList> DishLists { get; set; }
        public class DishList
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string AddOnItemName { get; set; }
            public string MasterProductCode { get; set; }
            public string MasterProductName { get; set; }
            public string AddOnCategoryCode { get; set; }
            public string AddOnCategoryName { get; set; }
            public string CrustName { get; set; }
            public string PortionCode { get; set; }
            public string PortionName { get; set; }
            public int MinSelectionLimit { get; set; }
            public int MaxSelectionLimit { get; set; }
            public double PortionPrice { get; set; }
            public double AddOnsPrice { get; set; }
            public double CrustPrice { get; set; }
            public int AddOnType { get; set; }
            public Nullable<int> MasterProductId { get; set; }
        }
    }
}