using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class SupportViewModel
    {
        // Live Ordering Count
        public int ShopAcceptanceCount { get; set; }
        public int DeliveryAcceptanceCount { get; set; }
        public int ShopPickupCount { get; set; }
        public int CustomerDeliveryCount { get; set; }
        public int OrderswithoutDeliveryboyCount { get; set; }
        public int ShopCount { get; set; }
        public int CustomerCount { get; set; }
        public int OrderCount { get; set; }
        public int DeliveryBoyLiveCount { get; set; }
        public int RefundCount { get; set; }
        public int ShopLowCreditCount { get; set; }

        //Error Count
        public int UnMappedCount { get; set; }
        public int OrderMissedCount { get; set; }

        //Verification Count
        public int CustomerAadhaarVerifyCount { get; set; }
        public int ShopOnBoardingVerifyCount { get; set; }
        public int DeliveryBoyVerifyCount { get; set; }
        public int BannerPendingCount { get; set; }
    }

    public class DeliveryBoyAssignViewModel
    {

        public List<AssignList> List { get; set; }
        public class AssignList
        {
            public long Id { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public int OrderNo { get; set; }
            public int CartStatus { get; set; }
            public DateTime DateEncoded { get; set; }
        }
        public List<DeliveryBoy> DeliveryBoyList { get; set; }
        public class DeliveryBoy
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
    public class UnMappedListViewModel
    {
        public List<UnMappedList> List { get; set; }
        public class UnMappedList
        {
            public int SlNo { get; set; }
            public long Id { get; set; }
            public string Name { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public System.DateTime DateUpdated { get; set; }
        }
    }
    public class OrderMissedListViewModel
    {
        public double CartTotalPrice { get; set; }
        public int OrderNumber { get; set; }
        public string ShopName { get; set; }
        public string Distance { get; set; }

        // Payment
        public double Amount { get; set; }
        public string ReferenceCode { get; set; }
        public double PackingCharge { get; set; }

        // PaymentDatas
        
        public string PaymentId { get; set; }
        public string Order_Id { get; set; }
        public string Method { get; set; }
        public Nullable<decimal> Fee { get; set; }
        public Nullable<decimal> Tax { get; set; }

        // ShopCharge
        public double GrossDeliveryCharge { get; set; }
        public double ShopDeliveryDiscount { get; set; }
        public double NetDeliveryCharge { get; set; }

        public List<OrderMissedList> List { get; set; }
        public class OrderMissedList
        {
            public int SlNo { get; set; }
            public double OrderNumber { get; set; }
            public long Id { get; set; }
            public System.DateTime DateEncoded { get; set; }
            public bool HasPayment { get; set; }
            public string PaymentMode { get; set; }
            public string PhoneNumber { get; set; }
            public string ShopName { get; set; }
            public string Amount { get; set; }

        }
    }
}