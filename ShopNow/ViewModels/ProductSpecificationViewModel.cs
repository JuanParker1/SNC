using System.Collections.Generic;

namespace ShopNow.ViewModels
{
    public class ProductSpecificationViewModel
    {
        public int Id { get; set; }
        public string MasterProductName { get; set; }
        public string SpecificationName { get; set; }
        public string Value { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }
    public class ProductSpecificationCreateViewModel
    {
        public int MasterProductId { get; set; }
        public string MasterProductName { get; set; }
        public string SpecificationId { get; set; }
        public string SpecificationName { get; set; }
        public string Value { get; set; }
    }

    public class ProductSpecificationEditViewModel: ProductSpecificationCreateViewModel
    {
        public int Id { get; set; }
    }

    public class ProductSpecificationListViewModel
    {
        public List<ProductSpecificationList> List { get; set; }
        public class ProductSpecificationList
        {
            public int Id { get; set; }
            public string MasterProductId { get; set; }
            public string MasterProductName { get; set; }
            public string SpecificationId { get; set; }
            public string SpecificationName { get; set; }
            public string Value { get; set; }
            public string CreatedBy { get; set; }
            public string UpdatedBy { get; set; }
        }
    }
}