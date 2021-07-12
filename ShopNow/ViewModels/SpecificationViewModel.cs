using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class SpecificationListViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public List<SpecificationList> List { get; set; }
        public class SpecificationList
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }
    }
    public class SpecificationMasterViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }
}