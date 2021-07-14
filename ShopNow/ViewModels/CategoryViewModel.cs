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
        public string ProductType { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }

}

