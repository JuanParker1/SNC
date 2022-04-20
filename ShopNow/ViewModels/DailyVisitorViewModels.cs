using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class DailyVisitorListViewModel
    {
        public DateTime? StartDateFilter { get; set; }
        public DateTime? EndDateFilter { get; set; }
        public int AndroidHomeCount { get; set; }
        public int IOSHomeCount { get; set; }
        public List<ListItem> ListItems { get; set; } 
        public class ListItem
        {
            public int CustomerId { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public DateTime DateUpdated { get; set; }
            public int Count { get; set; }
            public int OrderCount { get; set; }
            public double ConversionRate { get; set; }
        }
    }
}