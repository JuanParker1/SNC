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
    
    public partial class MasterProduct
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
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
        public string SizeLB { get; set; }
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
        public string ASIN { get; set; }
        public string ImagePath1 { get; set; }
        public string ImagePath2 { get; set; }
        public string ImagePath3 { get; set; }
        public string ImagePath4 { get; set; }
        public string ImagePath5 { get; set; }
        public int Adscore { get; set; }
        public string NickName { get; set; }
        public int Status { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
