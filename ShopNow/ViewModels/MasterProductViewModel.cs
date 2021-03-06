using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class MasterDishUploadViewModel
    {
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public string Price { get; set; }
        public string Customisation { get; set; }
        public string ColorCode { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string ImagePath1 { get; set; }
        public int Adscore { get; set; }
        public string NickName { get; set; }
        public int Status { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }

    public class MasterProductListViewModel
    {
        public List<MasterProductList> List { get; set; }
        public int CurrentPageIndex { get; set; }
        public int ItemCount { get; set; }
        public int PageCount { get; set; }
        public int maxRows { get; set; }
        public int ShopId { get; set; }
        public class MasterProductList
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string CategoryName { get; set; }
            public string BrandName { get; set; }
            public string ProductType { get; set; }
            public string DrugCompoundDetailName { get; set; }
            public string ProductTypeName { get; set; }
        }
        public List<PendingList> Lists { get; set; }
        public class PendingList
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public int ItemId { get; set; }
            public string ProductTypeName { get; set; }
        }
        public List<MappedList> MappedLists { get; set; }
        public class MappedList
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string MasterProductName { get; set; }
            public string ProductTypeName { get; set; }
        }
    }
  
    public class MasterProductMedicalListViewModel
    {
        public List<MasterProductMedicalList> List { get; set; }
        public int CurrentPageIndex { get; set; }
        public int ItemCount { get; set; }
        public int PageCount { get; set; }
        public int maxRows { get; set; }
        public class MasterProductMedicalList
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string CategoryName { get; set; }
            public string BrandName { get; set; }
            public string ProductType { get; set; }
            public string DrugCompoundDetailName { get; set; }
            public string ProductTypeName { get; set; }
        }
    }

    public class MasterProductFMCGListViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<MasterProductFMCGList> Lists { get; set; }
        public class MasterProductFMCGList
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string CategoryName { get; set; }
            public string BrandName { get; set; }
            public string ProductTypeName { get; set; }
        }
    }

    public class MasterProductFoodListViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public List<MasterProductFoodList> list { get; set; }
        public class MasterProductFoodList
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string CategoryName { get; set; }
            public double Price { get; set; }
            public string ProductTypeName { get; set; }
        }
    }

    public class MasterMedicalUploadViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Price { get; set; }
        public int MeasurementUnitId { get; set; }
        public string MeasurementUnitName { get; set; }
        public string PriscriptionCategory { get; set; }
        public string Weight { get; set; }
        public string SizeLBH { get; set; }
        public string DrugCompoundDetailIds { get; set; }
        public string DrugCompoundDetailName { get; set; }
        public string IBarU { get; set; }
        public string OriginCountry { get; set; }
        public string Manufacturer { get; set; }
        public Nullable<int> PackageId { get; set; }
        public string PackageName { get; set; }
        public string ImagePath1 { get; set; }
        public string ImagePath2 { get; set; }
        public string ImagePath3 { get; set; }
        public string ImagePath4 { get; set; }
        public string ImagePath5 { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }

    public class MedicalDrugListViewModel
    {
        public List<MedicalDrugList> List { get; set; }
        public class MedicalDrugList
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string Combination { get; set; }
            public string CategoryName { get; set; }
            public string BrandName { get; set; }
            public string ProductType { get; set; }
        }
    }

    public class ItemMappingViewModel
    {
        public long Id { get; set; }
        public string ItemId { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public int MasterProductId { get; set; }
        public string MasterProductName { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int DrugMeasurementUnitId { get; set; }
        public string DrugMeasurementUnitName { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Specification { get; set; }
        public int ShopCategoryId { get; set; }
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
        public double Weight { get; set; }
        public string SizeLBH { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string IsCheck { get; set; }

        public List<ItemMappingList> List { get; set; }
        public class ItemMappingList
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string ImagePath { get; set; }
            public string LongDescription { get; set; }
        }

        //
        public int PendingCount { get; set; }
        public int MappedCount { get; set; }
    }

    // Master Dish - 1
    public class MasterFoodCreateViewModel
    {
        public HttpPostedFileBase DishImage { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int[] TagCategoryId { get; set; }
        public double Price { get; set; }
        public bool Customisation { get; set; }
        public string ColorCode { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string ImagePath1 { get; set; }
        public int Adscore { get; set; }
        public string NickName { get; set; }
    }

    public class MasterFoodEditViewModel
    {
        public HttpPostedFileBase DishImage { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string TagCategory { get; set; }
        public int[] TagCategoryId { get; set; }
        public string TagCategoryName { get; set; }
        public double Price { get; set; }
        public bool Customisation { get; set; }
        public string ColorCode { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string ImagePath1 { get; set; }
        public int Adscore { get; set; }
        public string NickName { get; set; }
        public List<TagCategoryList> TagCategoryLists { get; set; }
        public class TagCategoryList
        {
            public int TagCategoryId { get; set; }
            public string TagCategoryName { get; set; }
        }
        public List<AddonList> AddonLists { get; set; }
        public class AddonList
        {
            public long Id { get; set; }
            public string AddOnItemName { get; set; }
            public Nullable<long> MasterProductId { get; set; }
            public string MasterProductName { get; set; }
            public int AddOnCategoryId { get; set; }
            public string AddOnCategoryName { get; set; }
            public string CrustName { get; set; }
            public int PortionId { get; set; }
            public string PortionName { get; set; }
            public int MinSelectionLimit { get; set; }
            public int MaxSelectionLimit { get; set; }
            public double PortionPrice { get; set; }
            public double AddOnsPrice { get; set; }
            public double CrustPrice { get; set; }
            public int AddOnType { get; set; }
        }
    }

    public class MasterAddOnsCreateViewModel
    {
        public int Index { get; set; }
        public long Id { get; set; }
        public string AddOnItemName { get; set; }
        public int MasterProductId { get; set; }
        public string MasterProductName { get; set; }
        public int AddOnCategoryId { get; set; }
        public string AddOnCategoryName { get; set; }
        public int CrustId { get; set; }
        public string CrustName { get; set; }
        public int PortionId { get; set; }
        public string PortionName { get; set; }
        public int MinSelectionLimit { get; set; }
        public int MaxSelectionLimit { get; set; }
        public double PortionPrice { get; set; }
        public double AddOnsPrice { get; set; }
        public double CrustPrice { get; set; }
        public int AddOnType { get; set; }
      //  public Nullable<int> MasterProductId { get; set; }

        public List<DishList> DishLists { get; set; }
        public class DishList
        {
            public long Id { get; set; }
            public string AddOnItemName { get; set; }
            public int MasterProductId { get; set; }
            public string MasterProductName { get; set; }
            public int AddOnCategoryId { get; set; }
            public string AddOnCategoryName { get; set; }
            public string CrustName { get; set; }
            public int PortionId { get; set; }
            public string PortionName { get; set; }
            public int MinSelectionLimit { get; set; }
            public int MaxSelectionLimit { get; set; }
            public double PortionPrice { get; set; }
            public double AddOnsPrice { get; set; }
            public double CrustPrice { get; set; }
            public int AddOnType { get; set; }
        }
    }

    // Master FMCG - 2
    public class MasterFMCGCreateViewModel
    {
        public HttpPostedFileBase FMCGImage1 { get; set; }
        public HttpPostedFileBase FMCGImage2 { get; set; }
        public HttpPostedFileBase FMCGImage3 { get; set; }
        public HttpPostedFileBase FMCGImage4 { get; set; }
        public HttpPostedFileBase FMCGImage5 { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string ASIN { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public int NextSubCategoryId { get; set; }
        public string NextSubCategoryName { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public int Adscore { get; set; }
        public double Price { get; set; }
        public double Weight { get; set; }
        public double SizeLWH { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int MeasurementUnitId { get; set; }
        public string MeasurementUnitName { get; set; }
        public Nullable<int> PackageId { get; set; }
        public string PackageName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string NickName { get; set; }
    }

    public class MasterFMCGEditViewModel
    {
        public HttpPostedFileBase FMCGImage1 { get; set; }
        public HttpPostedFileBase FMCGImage2 { get; set; }
        public HttpPostedFileBase FMCGImage3 { get; set; }
        public HttpPostedFileBase FMCGImage4 { get; set; }
        public HttpPostedFileBase FMCGImage5 { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string ASIN { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string TagCategoryName { get; set; }
        public string TagCategory { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public int NextSubCategoryId { get; set; }
        public string NextSubCategoryName { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public int Adscore { get; set; }
        public double Price { get; set; }
        public double Weight { get; set; }
        public double SizeLWH { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int MeasurementUnitId { get; set; }
        public string MeasurementUnitName { get; set; }
        public Nullable<int> PackageId { get; set; }
        public string PackageName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string NickName { get; set; }
        public string ImagePath1 { get; set; }
        public string ImagePath2 { get; set; }
        public string ImagePath3 { get; set; }
        public string ImagePath4 { get; set; }
        public string ImagePath5 { get; set; }
    }

    public class MasterFMCGUploadViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public string Price { get; set; }
        public int MeasurementUnitId { get; set; }
        public string MeasurementUnitName { get; set; }
        public string Weight { get; set; }
        public string SizeLBH { get; set; }
        public Nullable<int> PackageId { get; set; }
        public string PackageName { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public int SubCategoryId { get; set; }
        public int NextSubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string NextSubCategoryName { get; set; }
        public string ASIN { get; set; }
        public string ImagePath1 { get; set; }
        public string ImagePath2 { get; set; }
        public string ImagePath3 { get; set; }
        public string ImagePath4 { get; set; }
        public string ImagePath5 { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }

    // Master Medical - 3
    public class MasterMedicalCreateViewModel
    {
        public HttpPostedFileBase MedicalImage1 { get; set; }
        public HttpPostedFileBase MedicalImage2 { get; set; }
        public HttpPostedFileBase MedicalImage3 { get; set; }
        public HttpPostedFileBase MedicalImage4 { get; set; }
        public HttpPostedFileBase MedicalImage5 { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int MeasurementUnitId { get; set; }
        public string MeasurementUnitName { get; set; }
        public string[] DrugCompoundDetailIds { get; set; }
        public string DrugCompoundDetailName { get; set; }
        public bool PriscriptionCategory { get; set; }
        public string Manufacturer { get; set; }
        public string OriginCountry { get; set; }
        public Nullable<int> IBarU { get; set; }
        public double Weight { get; set; }
        public double SizeLWH { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Price { get; set; }
        public int Adscore { get; set; }
        public Nullable<int> PackageId { get; set; }
        public string PackageName { get; set; }
        public string GoogleTaxonomyCode { get; set; }
    }

    public class MasterMedicalEditViewModel
    {
        public HttpPostedFileBase MedicalImage1 { get; set; }
        public HttpPostedFileBase MedicalImage2 { get; set; }
        public HttpPostedFileBase MedicalImage3 { get; set; }
        public HttpPostedFileBase MedicalImage4 { get; set; }
        public HttpPostedFileBase MedicalImage5 { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string TagCategory { get; set; }
        public string TagCategoryName { get; set; }
        public int MeasurementUnitId { get; set; }
        public string MeasurementUnitName { get; set; }
        public string DrugCompoundDetailIds1 { get; set; }
        public string[] DrugCompoundDetailIds { get; set; }
        public string DrugCompoundDetailName { get; set; }
        public bool PriscriptionCategory { get; set; }
        public string Manufacturer { get; set; }
        public string OriginCountry { get; set; }
        public Nullable<int> IBarU { get; set; }
        public double Weight { get; set; }
        public double SizeLWH { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Price { get; set; }
        public int Adscore { get; set; }
        public Nullable<int> PackageId { get; set; }
        public string PackageName { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string ImagePath1 { get; set; }
        public string ImagePath2 { get; set; }
        public string ImagePath3 { get; set; }
        public string ImagePath4 { get; set; }
        public string ImagePath5 { get; set; }
    }

    // Master Electronic - 4
    public class MasterElectronicListViewModel
    {
        public string Name { get; set; }
        public string BrandName { get; set; }
        public List<MasterElectronicList> list { get; set; }
        public class MasterElectronicList
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string BrandName { get; set; }
            public string CategoryName { get; set; }
            public string ProductTypeName { get; set; }
        }
    }

    public class MasterElectronicCreateViewModel
    {
        public HttpPostedFileBase ElectronicImage1 { get; set; }
        public HttpPostedFileBase ElectronicImage2 { get; set; }
        public HttpPostedFileBase ElectronicImage3 { get; set; }
        public HttpPostedFileBase ElectronicImage4 { get; set; }
        public HttpPostedFileBase ElectronicImage5 { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public double Price { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string NickName { get; set; }
        public int Adscore { get; set; }
    }

    public class MasterElectronicEditViewModel
    {
        public HttpPostedFileBase ElectronicImage1 { get; set; }
        public HttpPostedFileBase ElectronicImage2 { get; set; }
        public HttpPostedFileBase ElectronicImage3 { get; set; }
        public HttpPostedFileBase ElectronicImage4 { get; set; }
        public HttpPostedFileBase ElectronicImage5 { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string TagCategory { get; set; }
        public string TagCategoryName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public double Price { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public string ImagePath1 { get; set; }
        public string ImagePath2 { get; set; }
        public string ImagePath3 { get; set; }
        public string ImagePath4 { get; set; }
        public string ImagePath5 { get; set; }
        public int Adscore { get; set; }
        public string NickName { get; set; }
    }

    public class MasterElectronicUploadViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public double Price { get; set; }
        public bool Customisation { get; set; }
        public string ColorCode { get; set; }
        public int MeasurementUnitId { get; set; }
        public string MeasurementUnitName { get; set; }
        public bool PriscriptionCategory { get; set; }
        public double Weight { get; set; }
        public string SizeLBH { get; set; }
        public string DrugCompoundDetailIds { get; set; }
        public string DrugCompoundDetailName { get; set; }
        public Nullable<int> IBarU { get; set; }
        public string OriginCountry { get; set; }
        public string Manufacturer { get; set; }
        public Nullable<int> PackageId { get; set; }
        public string PackageName { get; set; }
        public string GoogleTaxonomyCode { get; set; }
        public int SubCategoryId { get; set; }
        public int NextSubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string NextSubCategoryName { get; set; }
        public string ASIN { get; set; }
        public string ImagePath1 { get; set; }
        public string ImagePath2 { get; set; }
        public string ImagePath3 { get; set; }
        public string ImagePath4 { get; set; }
        public string ImagePath5 { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }

    public class TagCategorySessionList
    {
        public int Id { get; set; }
        public int Type { get; set; }
    }
}