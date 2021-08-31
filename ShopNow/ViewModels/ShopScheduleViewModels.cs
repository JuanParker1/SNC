using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class ShopScheduleIndexViewModel
    {
        public int FilterShopId { get; set; }
        public string FilterShopName { get; set; }
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public bool HasSchedule { get; set; }

            public List<TimeListItem> TimeListItems { get; set; }
            public class TimeListItem
            {
                public int Id { get; set; }
                public TimeSpan? OnTime { get; set; }
                public TimeSpan ?OffTime { get; set; }
            }
        }
    }

    public class ShopScheduleAddViewModel
    {
        public int ShopId { get; set; }
        public List<TimeListItem> TimeListItems { get; set; }
        public class TimeListItem
        {
            public TimeSpan OnTime { get; set; }
            public TimeSpan OffTime { get; set; }
        }
    }
}