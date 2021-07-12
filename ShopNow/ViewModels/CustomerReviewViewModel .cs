using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class ShopReviewViewModel
    {

        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerRemark { get; set; }
        public int Rating { get; set; }
    }

    public class ShopReviewUpdateViewModel
    {
        public string Code { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerRemark { get; set; }
        public int Rating { get; set; }
    }

    public class ReviewListViewModel
    {
        public List<ReviewlList> CustomerList { get; set; }
        public List<ReviewlList> ReviewlLists { get; set; }
        public class ReviewlList
        {
            public string Code { get; set; }
            public string ShopName { get; set; }
            public string CustomerName { get; set; }
            public string CustomerRemark { get; set; }
            public int ? Rating { get; set; }

        }
    }
}

