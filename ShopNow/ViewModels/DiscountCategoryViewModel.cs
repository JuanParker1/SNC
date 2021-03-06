using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class DiscountCategoryListViewModel
    {
        public List<DiscountCategoryList> List { get; set; }
        public class DiscountCategoryList
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public double Percentage { get; set; }
        }
    }
}