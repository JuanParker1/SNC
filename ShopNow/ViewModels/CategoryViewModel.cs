using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{

    public class CategoryListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProductType { get; set; }
        public int OrderNo { get; set; }
        public List<CategoryList> List { get; set; }
        public class CategoryList
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ProductType { get; set; }
            public int OrderNo { get; set; }
        }

    }
    public class CategoryMasterViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderNo { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }
    public class CategoryCreateViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderNo { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public string ImagePath { get; set; }
        public HttpPostedFileBase CategoryImage { get; set; }
       
    }
    public class CategoryEditViewModel
    {
        public int Id { get; set; }
        public string editName { get; set; }
        public int editOrderNo { get; set; }
        public int editProductTypeId { get; set; }
        public string editProductTypeName { get; set; }
        public string ImagePath { get; set; }
        public HttpPostedFileBase editCategoryImage { get; set; }
    }
}

