using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class PortionListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<PortionList> List { get; set; }
        public class PortionList
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

    }
    public class PortionMasterViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }
}