using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class PlatFormCreditRateListViewModel
    {
        public string Code { get; set; }
        public double RatePerOrder { get; set; }
        public double DailyViewer { get; set; }
        public int Count { get; set; }

        public List<PlatFormCreditRateList> List { get; set; }
        public class PlatFormCreditRateList
        {
            public string Code { get; set; }
            public double RatePerOrder { get; set; }
            public double DailyViewer { get; set; }
        }
    }
}