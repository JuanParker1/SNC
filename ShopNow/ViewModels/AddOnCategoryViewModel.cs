using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class AddOnCategoryListViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public List<AddOnCategoryList> List { get; set; }
        public class AddOnCategoryList
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

    }
    public class AddOnCategoryMasterViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }
}