using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class SubCategoryListViewModel
    {
        public List<SubCategoryList> List { get; set; }
        public class SubCategoryList
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string CategoryCode { get; set; }
            public string CategoryName { get; set; }
            public string ProductType { get; set; }
            public int Adscore { get; set; }
        }
    }

    public class SubCategoryEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string ProductType { get; set; }
        public int Adscore { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}