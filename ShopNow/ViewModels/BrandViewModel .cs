using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class BrandListViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ProductType { get; set; }

        public List<BrandList> List { get; set; }
        public class BrandList
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string ProductType { get; set; }
        }
    }
    public class BrandMasterViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ProductType { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }

}

