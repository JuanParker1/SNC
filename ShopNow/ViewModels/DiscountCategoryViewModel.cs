﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class DiscountCategoryListViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public double Percentage { get; set; }


        public List<DiscountCategoryList> List { get; set; }
        public class DiscountCategoryList
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public double Percentage { get; set; }
        }
    }
}