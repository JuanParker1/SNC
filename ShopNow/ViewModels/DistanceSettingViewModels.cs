using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class DistanceSettingIndexViewModel
    {
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int Id { get; set; }
            public double Distance { get; set; }
            public TimeSpan? Time { get; set; }
            public string TimeText { get; set; }
            public int Status { get; set; }
            public string UpdatedBy { get; set; }
            public DateTime UpdatedDateTime { get; set; }
        }
    }
}