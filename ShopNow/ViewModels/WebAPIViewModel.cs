using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class WebApiCreateViewModel
    {
        public int CategoryType { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string Link { get; set; }
        public string AuthName { get; set; }
        public string AuthKey { get; set; }
    }

    public class WebApiTSViewModel
    {
        public string timeSpan { get; set; }
        public string LasttimeSpan { get; set; }

    }


    public class RootObject
    {
        public items[] items { get; set; }
    }
    public class items
    {
        public string itemId { get; set; }
        public string itemName { get; set; }      
        public string iBarU { get; set; }
        public string status { get; set; }
        public string manufacturer { get; set; }
        public string appliesOnline { get; set; }
        public stocks[] stock { get; set; }

    }
    public class stocks
    {
        public string stock { get; set; }
        public string supplierName { get; set; }
        public string mrp { get; set; }
        public string salePrice { get; set; }
        public string taxPercentage { get; set; }
        public string discountpercentage { get; set; }
        public string loyaltypointsper100value { get; set; }
        public string minimumloyaltyreductionpercentage { get; set; }
        public string specialcostfordelivery { get; set; }
        public string  outletId { get; set; }
        public string specialPrice { get; set; }
        public string minSaleQuantity { get; set; }
        public string Cat1 { get; set; }
        public string Cat2 { get; set; }
        public string Cat3 { get; set; }
        public string Cat4 { get; set; }
        public string Cat5 { get; set; }
        public string Cat6 { get; set; }
        public string Cat7 { get; set; }
        public string Cat8 { get; set; }
        public string Cat9 { get; set; }
        public string Cat10 { get; set; }
        public string itemTimeStamp { get; set; }
    }

    public class TimeSpanViewModel
    {      
            public string ItemTimeStamp { get; set; }
       
    }

    }