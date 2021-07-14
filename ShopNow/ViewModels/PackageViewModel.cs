using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    
    public class PackageListViewModel
    {
        public List<PackageList> List { get; set; }
        public class PackageList
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Type { get; set; }
        }
    }
    public class PackageMasterViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }
}