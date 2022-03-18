using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class DashboardIndexViewModel
    {
        public int MonthFilter { get; set; }
        public int YearFilter { get; set; }

        public int OrderJanCount { get; set; }
        public int OrderFebCount { get; set; }
        public int OrderMarCount { get; set; }
        public int OrderAprCount { get; set; }
        public int OrderMayCount { get; set; }
        public int OrderJunCount { get; set; }
        public int OrderJulCount { get; set; }
        public int OrderAugCount { get; set; }
        public int OrderSepCount { get; set; }
        public int OrderOctCount { get; set; }
        public int OrderNovCount { get; set; }
        public int OrderDecCount { get; set; }

        public int OrderRestaurantCount { get; set; }
        public int OrderMedicalCount { get; set; }
        public int OrderMeatAndVegCount { get; set; }
        public int OrderSupermarketCount { get; set; }

        public int Order5thDay { get; set; }
        public int Order4thDayCount { get; set; }
        public int Order3rdDayCount { get; set; }
        public int Order2ndDayCount { get; set; }
        public int OrderTodayCount { get; set; }
    }
}