using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class AccountsBillWiseReportViewModel
    {
        public int MonthFilter { get; set; }
        public int YearFilter { get; set; }
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int SNo { get; set; }
            public DateTime Date { get; set; }
            public string OrderNumber { get; set; }
            public string ShopName { get; set; }
            public string GSTNumber { get; set; }
            public double MenuPrice { get; set; }
            public double ShopBillAmount { get; set; }
            public double PriceDifference { get; set; }
            public double CustomerPaidAmount { get; set; }
            public double RefundAmount { get; set; }
            public double FinalAmount { get; set; }
            public double DeliveryAmountFromCustomer { get; set; }
            public double DeliveryDiscount { get; set; }
            public double TotalDeliveryCharge { get; set; }
            public double DeliveryChargePaidToDeliveryBoy { get; set; }
            public double DeliveryChargeProfit { get; set; }
            public double AmountProfit { get; set; }
            public double WalletUsed { get; set; }
        }
    }
}