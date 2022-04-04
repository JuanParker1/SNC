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
            public int Type { get; set; }
            public string DistrictName { get; set; }
        }
    }

    public class PushNotificationListViewModel
    {
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int Index { get; set; }
            public string DistrictName { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string EncodedBy { get; set; }
            public DateTime DateEncoded { get; set; }
        }
    }
}