using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class ProductSpecificationViewModel
    {
        public string Code { get; set; }
        public string MasterProductName { get; set; }
        public string SpecificationName { get; set; }
        public string Value { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }
    public class ProductSpecificationCreateEditViewModel
    {
        public string Code { get; set; }
        public string MasterProductCode { get; set; }
        public string MasterProductName { get; set; }
        public string SpecificationCode { get; set; }
        public string SpecificationName { get; set; }
        public string Value { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
    }
    public class ProductSpecificationListViewModel
    {
        public List<ProductSpecificationList> List { get; set; }
        public class ProductSpecificationList
        {
            public string Code { get; set; }
            public string MasterProductCode { get; set; }
            public string MasterProductName { get; set; }
            public string SpecificationCode { get; set; }
            public string SpecificationName { get; set; }
            public string Value { get; set; }
            public string CreatedBy { get; set; }
            public string UpdatedBy { get; set; }
        }
    }
}