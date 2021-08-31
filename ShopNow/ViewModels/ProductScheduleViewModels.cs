using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class ProductScheduleIndexViewModel
    {
        public int FilterProductId { get; set; }
        public string FilterProductName { get; set; }
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public long ProductId { get; set; }
            public string ProductName { get; set; }
            public bool HasSchedule { get; set; }

            public List<TimeListItem> TimeListItems { get; set; }
            public class TimeListItem
            {
                public int Id { get; set; }
                public TimeSpan? OnTime { get; set; }
                public TimeSpan? OffTime { get; set; }
            }
        }
    }

    public class ProductScheduleAddViewModel
    {
        public long[] ProductId { get; set; }
        public List<TimeListItem> TimeListItems { get; set; }
        public class TimeListItem
        {
            public TimeSpan OnTime { get; set; }
            public TimeSpan OffTime { get; set; }
        }
    }
}