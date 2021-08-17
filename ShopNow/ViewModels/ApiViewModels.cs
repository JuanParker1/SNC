using ShopNow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class GetAllOrderListViewModel
    {
        public List<OrderList> OrderLists { get; set; }
        public class OrderList
        {
            public long Id { get; set; }
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public string CustomerPhoneNumber { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public string DeliveryAddress { get; set; }
            public string ShopPhoneNumber { get; set; }
            public string ShopOwnerPhoneNumber { get; set; }
            public int TotalProduct { get; set; }
            public int TotalQuantity { get; set; }
            public double TotalPrice { get; set; }
            public int OrderNumber { get; set; }
            public int DeliveryBoyId { get; set; }
            public string DeliveryBoyName { get; set; }
            public string DeliveryBoyPhoneNumber { get; set; }
            public double DeliveryCharge { get; set; }
            public double ShopDeliveryDiscount { get; set; }
            public double NetDeliveryCharge { get; set; }
            public double Convinenientcharge { get; set; }
            public double Packingcharge { get; set; }
            public double PenaltyAmount { get; set; }
            public string PenaltyRemark { get; set; }
            public double WaitingCharge { get; set; }
            public string WaitingRemark { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public double? RefundAmount { get; set; }
            public string RefundRemark { get; set; }
            public string PaymentMode { get; set; }
            public int Onwork { get; set; }
            public List<OrderItem> OrderItemList { get; set; }
        }
    }

    public class TodayDeliveryListViewModel
    {
        public List<OrderList> ResturantList { get; set; }
        public List<OrderList> OtherList { get; set; }
        public class OrderList
        {
            public string ShopName { get; set; }
            public string ShopAddress { get; set; }
            public string ShopPhoneNumber { get; set; }
            public double ShopLatitude { get; set; }
            public double ShopLongitude { get; set; }
            public double CustomerLatitude { get; set; }
            public double CustomerLongitude { get; set; }
            public int OrderNumber { get; set; }
           // public double Amount { get; set; }
            public string CustomerPhoneNumber { get; set; }
            public string CustomerName { get; set; }
            public string DeliveryAddress { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public string PaymentMode { get; set; }
            public int OnWork { get; set; }
            public int TotalProduct { get; set; }
            public int TotalQuantity { get; set; }
            public double TotalPrice { get; set; }
            public double DeliveryCharge { get; set; }
            public double ShopDeliveryDiscount { get; set; }
            public double NetDeliveryCharge { get; set; }
            public double Convinenientcharge { get; set; }
            public double Packingcharge { get; set; }
            public Double? RefundAmount { get; set; }
            public string RefundRemark { get; set; }
            public List<OrderItem> OrderItemList { get; set; }
           
        }
    }
}