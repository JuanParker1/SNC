using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class CrustListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ListItem> List { get; set; }
        public class ListItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }

    public class CrustMasterViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }
}