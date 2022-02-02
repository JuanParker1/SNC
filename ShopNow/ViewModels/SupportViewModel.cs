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
        public int CustomerPrescriptionCount { get; set; }

        public int ShopOnlineCount { get; set; }
        public int ShopOfflineCount { get; set; }

        //Verification Count
        public int CustomerAadhaarVerifyCount { get; set; }
        public int ShopOnBoardingVerifyCount { get; set; }
        public int DeliveryBoyVerifyCount { get; set; }
        public int BannerPendingCount { get; set; }

        // Error Count
        public int UnMappedCount { get; set; }
        public int OrderMissedCount { get; set; }
        public int ProductUnMappedCount { get; set; }

        public int ServiceCount { get; set; }


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

    //public class UnMappedListViewModel
    //{
    //    public List<UnMappedList> List { get; set; }
    //    public class UnMappedList
    //    {
    //        public int SlNo { get; set; }
    //        public long Id { get; set; }
    //        public string Name { get; set; }
    //        public int MasterProductId { get; set; }
    //        public string MasterProductName { get; set; }
    //        public string ShopId { get; set; }
    //        public string ShopName { get; set; }
    //        public DateTime DateUpdated { get; set; }
    //    }
    //}
    //public class OrderMissedListViewModel
    //{
    //    public double CartTotalPrice { get; set; }
    //    public string OrderNo { get; set; }
    //    public string ShopName { get; set; }
    //    public string Distance { get; set; }

    //    // Payment
    //    public double Amount { get; set; }
    //    public string ReferenceCode { get; set; }
    //    public double PackagingCharge { get; set; }

    //    // PaymentDatas
    //    public string paymentId { get; set; }
    //    public string order_id { get; set; }
    //    public string method { get; set; }
    //    public decimal? fee { get; set; }
    //    public decimal? tax { get; set; }

    //    // ShopCharge
    //    public double GrossDeliveryCharge { get; set; }
    //    public double ShopDeliveryDiscount { get; set; }
    //    public double NetDeliveryCharge { get; set; }

    //    public List<OrderMissedList> List { get; set; }
    //    public class OrderMissedList
    //    {
    //        public int SlNo { get; set; }
    //        public int OrderNo { get; set; }
    //        public long Id { get; set; }
    //        public string PaymentMode { get; set; }
    //        public DateTime DateEncoded { get; set; }
    //        public bool HasPayment { get; set; }
    //        public string PhoneNumber { get; set; }
    //        public string ShopName { get; set; }
    //        public string Amount { get; set; }

    //    }
    //}

    public class ProductUnMappedList
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public DateTime? MappedDate { get; set; }
            public long Id { get; set; }
            public string Name { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public double MenuPrice { get; set; }
            public double SellingPrice { get; set; }
            public int Quantity { get; set; }
            public int Status { get; set; }
            public int ItemId { get; set; }
        }
        public List<CountListItem> CountListItems { get; set; }
        public class CountListItem
        {
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public int Count { get; set; }
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
        // Order
        public int PaymentType { get; set; }
        public double TotalPrice { get; set; }
        public int OrderNumber { get; set; }
        public string ShopName { get; set; }
        public string Distance { get; set; }
        public double DeliveryCharge { get; set; }
        public double ShopDeliveryDiscount { get; set; }
        public double NetDeliveryCharge { get; set; }

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
       

        public List<OrderMissedList> List { get; set; }
        public class OrderMissedList
        {
            public long Id { get; set; }
            public double OrderNumber { get; set; }
            public System.DateTime DateEncoded { get; set; }
            public string PaymentMode { get; set; }
            public string PhoneNumber { get; set; }
            public string ShopName { get; set; }
            public string Amount { get; set; }
            public double TotalPrice { get; set; }
        }
    }
}