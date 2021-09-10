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
}