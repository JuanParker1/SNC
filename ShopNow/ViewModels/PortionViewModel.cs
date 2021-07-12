using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class PortionListViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public List<PortionList> List { get; set; }
        public class PortionList
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

    }
    public class PortionMasterViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }
}