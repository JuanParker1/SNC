using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    //public class SubCategoryListViewModel
    //{
    //    public List<SubCategoryList> List { get; set; }
    //    public class SubCategoryList
    //    {
    //        public string Code { get; set; }
    //        public string Name { get; set; }
    //        public string CategoryCode { get; set; }
    //        public string CategoryName { get; set; }
    //        public string ProductType { get; set; }
    //        public int Adscore { get; set; }
    //    }
    //}

    public class SubCategoryEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public int Adscore { get; set; }
    }
}