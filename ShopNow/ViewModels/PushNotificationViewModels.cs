using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class PushNotificationIndexViewModel
    {
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int Index { get; set; }
            public int Count { get; set; }
            public string DistrictName { get; set; }
        }
    }
}