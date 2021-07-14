using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class RefundPendingViewModel
    {
        public DateTime? OrderDate { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string ErrorMessage { get; set; }

        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public string PaymentId { get; set; }
            public string OrderNo { get; set; }
            public string CustomerName { get; set; }
            public string CustomerPhoneNo { get; set; }
            public double? Amount { get; set; }
            public string Remark { get; set; }
        }
    }

    public class RefundHistoryViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CustomerPhoneNo { get; set; }

        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public string ShopName { get; set; }
            public DateTime? OrderDate { get; set; }
            public string PaymentId { get; set; }
            public int? OrderNo { get; set; }
            public string CustomerName { get; set; }
            public string CustomerPhoneNo { get; set; }
            public double? Amount { get; set; }
            public string Remark { get; set; }
            public string RefundId { get; set; }
            public DateTime? RefundDate { get; set; }
        }
    }
}