using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class ShopCategoryListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<ShopCategoryList> List { get; set; }
        public class ShopCategoryList
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

    }
    public class ShopCategoryMasterViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }
}