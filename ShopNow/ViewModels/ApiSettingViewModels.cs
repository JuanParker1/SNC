using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class ApiSettingListViewModel
    {
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int Id { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public string ProviderName { get; set; }
            public string Version { get; set; }
            public string Url { get; set; }
            public int Category { get; set; }
            public int OutletId { get; set; }
        }
    }

    public class ApiSettingCreateViewModel
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string ProviderName { get; set; }
        public string Version { get; set; }
        public string Url { get; set; }
        public string AuthName { get; set; }
        public string AuthKey { get; set; }
        public string Remark { get; set; }
        public int Category { get; set; }
        public int OutletId { get; set; }
    }

    public class ApiSettingEditViewModel : ApiSettingCreateViewModel
    {
        public int Id { get; set; }
    }
}