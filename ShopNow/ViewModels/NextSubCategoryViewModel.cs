using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class NextSubCategoryViewModel
    {
        public List<NextSubCategoryList> List { get; set; }
        public class NextSubCategoryList
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string SubCategoryCode { get; set; }
            public string SubCategoryName { get; set; }
            public string ProductType { get; set; }
            public int Adscore { get; set; }
        }
    }

    public class NextSubCategoryEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SubCategoryCode { get; set; }
        public string SubCategoryName { get; set; }
        public string ProductType { get; set; }
        public int Adscore { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}