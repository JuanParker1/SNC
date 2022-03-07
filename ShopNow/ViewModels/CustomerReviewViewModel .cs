using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class ShopReviewViewModel
    {

        public string ShopId { get; set; }
        public string ShopName { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerRemark { get; set; }
        public int Rating { get; set; }
    }

    public class ShopReviewUpdateViewModel
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerRemark { get; set; }
        public int Rating { get; set; }
    }

    public class ReviewListViewModel
    {
        public List<ReviewlList> CustomerList { get; set; }
        public List<ReviewlList> ReviewlLists { get; set; }
        public class ReviewlList
        {
            public long Id { get; set; }
            public string ShopName { get; set; }
            public string CustomerName { get; set; }
            public string CustomerRemark { get; set; }
            public int ? Rating { get; set; }
            public DateTime Date { get; set; }
            public string ReplyText { get; set; }
            public DateTime ReplyDate { get; set; }
            public int ReplyId { get; set; }
        }
    }
}

