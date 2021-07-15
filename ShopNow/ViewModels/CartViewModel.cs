using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShopNow.Models;

namespace ShopNow.ViewModels
{
    public class CartDetailsViewModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string PhoneNumber { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string DeliveryAddress { get; set; }
        public string ImagePath { get; set; }
        public string DeliveryBoyCode { get; set; }
        public string DeliveryBoyName { get; set; }
        public string DeliveryBoyPhoneNumber { get; set; }
        public string UserEnquiryCode { get; set; }
        public string UserEnquiryName { get; set; }
        public string OrderNo { get; set; }
        public string Qty { get; set; }
        public double Price { get; set; }
        public int CartStatus { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
    }


    public class CartHistoryViewModel
    {
        public List<CartList> List { get; set; }
        public class CartList
        {
            public int Id { get; set; }
            public string CustomerCode { get; set; }
            public string CustomerName { get; set; }
            public string ShopCode { get; set; }
            public string ShopName { get; set; }
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public string BrandCode { get; set; }
            public string BrandName { get; set; }
            public string CategoryCode { get; set; }
            public string CategoryName { get; set; }
            public string ImagePath { get; set; }
            public string OrderNo { get; set; }
            public string Qty { get; set; }
            public double Price { get; set; }
            public int CartStatus { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public string DateOfOrder { get; set; }
        }
    }

    public class CartListViewModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string PhoneNumber { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string DeliveryAddress { get; set; }
        public string ImagePath { get; set; }
        public int DeliveryBoyId { get; set; }
        public string DeliveryBoyName { get; set; }
        public string UserEnquiryCode { get; set; }
        public string UserEnquiryName { get; set; }
        public int OrderNo { get; set; }
        public string Qty { get; set; }
        public double Price { get; set; }
        public int CartStatus { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
        public int isAssign { get; set; }
        public int OnWork { get; set; }
        public double Amount { get; set; }
        public double ConvenientCharge { get; set; }
        public double PackagingCharge { get; set; }
        public double DelivaryCharge { get; set; }

        public List<CartList> List { get; set; }
        public class CartList
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string PhoneNumber { get; set; }
            public int BrandId { get; set; }
            public string BrandName { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string DeliveryAddress { get; set; }
            public string ImagePath { get; set; }
            public int DeliveryBoyId { get; set; }
            public string DeliveryBoyName { get; set; }
            public string DeliveryPhoneNumber { get; set; }
            public string UserEnquiryCode { get; set; }
            public string UserEnquiryName { get; set; }
            public int OrderNo { get; set; }
            public int Qty { get; set; }
            public double Price { get; set; }
            public int CartStatus { get; set; }
            public string CreatedBy { get; set; }
            public string UpdatedBy { get; set; }
            public int Status { get; set; }
            public string Date { get; set; }
            public DateTime DateEncoded { get; set; }
            public DateTime DateUpdated { get; set; }
            public int isAssign { get; set; }
            public int OnWork { get; set; }
            public double SinglePrice { get; set; }
            public double? RefundAmount { get; set; }
            public string RefundRemark { get; set; }
            public string PaymentMode { get; set; }
            public double Amount { get; set; }

            public string CartStatusText
            {
                get
                {
                    switch (this.CartStatus)
                    {
                        case 2:
                            return "Pending";
                        case 3:
                            return "Order is being Prepared";
                        case 4:
                            return "Assigned for Delivery. Waiting for Pickup";
                        case 5:
                            return "On the Way to Delivery";
                        case 6:
                            return "Delivered";
                        case 7:
                            return "Cancelled";
                        default:
                            return "N/A";
                    }
                }
            }
        }
    }

    public class CartAssignDeliveryBoyViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int DeliveryBoyId { get; set; }

        public List<CartAssignList> Lists { get; set; }
        public class CartAssignList
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string DeliveryBoyName { get; set; }
            public double Meters { get; set; }
            public int Status { get; set; }
            public double Amount { get; set; }
        }
    }

    public class CartReportViewModel
    {
        public string StartingDate { get; set; }
        public string EndingDate { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }

        public int YearFilter { get; set; }
        public int StartMonthFilter { get; set; }
        public List<CartReportList> List { get; set; }
        public class CartReportList
        {
            public int Id { get; set; }
            public string CustomerCode { get; set; }
            public string CustomerName { get; set; }
            public string ShopCode { get; set; }
            public string ShopName { get; set; }
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public string PhoneNumber { get; set; }
            public string BrandCode { get; set; }
            public string BrandName { get; set; }
            public string CategoryCode { get; set; }
            public string CategoryName { get; set; }
            public string DeliveryAddress { get; set; }
            public string ImagePath { get; set; }
            public string DeliveryBoyCode { get; set; }
            public string DeliveryBoyName { get; set; }
            public string DeliveryBoyPhoneNumber { get; set; }
            public string UserEnquiryCode { get; set; }
            public string UserEnquiryName { get; set; }
            public string OrderNo { get; set; }
            public string Qty { get; set; }
            public double Price { get; set; }
            public int CartStatus { get; set; }
            public string CreatedBy { get; set; }
            public string UpdatedBy { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public DateTime DateUpdated { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public double TotalAmount { get; set; }
            public double Amount { get; set; }
            public double OriginalAmount { get; set; }
            public double GrossDeliveryCharge { get; set; }
            public int ShopPaymentStatus { get; set; }
            public int DeliveryBoyPaymentStatus { get; set; }
            public int DeliveryOrderPaymentStatus { get; set; }
            public int DeliveryRateSet { get; set; }
            public double Kilometer { get; set; }
        }
    }

    public class ShopDeliveredAmountReportViewModel
    {
        public List<ShopDeliveredAmountReportList> List { get; set; }
        public class ShopDeliveredAmountReportList
        {
            public int Id { get; set; }
            public string CustomerCode { get; set; }
            public string CustomerName { get; set; }
            public string ShopCode { get; set; }
            public string ShopName { get; set; }

            public double OriginalAmount { get; set; }
            public double GSTAmount { get; set; }


            public DateTime DateEncoded { get; set; }
            public DateTime DateUpdated { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
    }


    //Api
    public class CartCreateViewModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string PhoneNumber { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string OrderNo { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string DeliveryAddress { get; set; }
        public double SinglePrice { get; set; }
        public string ImagePath { get; set; }
        public string Qty { get; set; }
        public double Price { get; set; }
        public int ItemId { get; set; }
    }

    public class OrderCreateViewModel
    {
        public int CustomerId { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string PhoneNumber { get; set; }
        public string OrderNo { get; set; }
        public string DeliveryAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public List<ListItem> ListItems { get; set; } 
        public class ListItem
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int BrandId { get; set; }
            public string BrandName { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string ImagePath { get; set; }
            public int Quantity { get; set; }
            public double UnitPrice { get; set; }
            public double Price { get; set; }
            public int ItemId { get; set; }
        }
    }

    public class CartDelivaryShopApiViewModel
    {


        public List<CartList> List { get; set; }
        public class CartList
        {
            public int Id { get; set; }
            public string ShopCode { get; set; }
            public string ShopName { get; set; }
            public string OrderNo { get; set; }
            public string Qty { get; set; }
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public double Price { get; set; }
            public string PhoneNumber { get; set; }
            public string CustomerName { get; set; }
            public string DeliveryAddress { get; set; }
            public List<OrderItem> OrderList { get; set; }
        }
    }

    public class DelivaryBoyReportViewModel
    {
        public double EarningOfToday { get; set; }
        public List<CartList> List { get; set; }
        public class CartList
        {

            public int OrderNo { get; set; }
            public double GrossDeliveryCharge { get; set; }
            public int CartStatus { get; set; }
            public DateTime DateEncoded { get; set; }
            public double ShopLatitude { get; set; }
            public double ShopLongitude { get; set; }
            public double CustomerLatitude { get; set; }
            public double CustomerLongitude { get; set; }
            public string Date { get; set; }
        }
    }
    public class DelivaryBoyPayoutReportViewModel
    {
        
        public List<PayoutOut> List { get; set; }
        public class PayoutOut
        {

            public string Date { get; set; }
            public DateTime date { get; set; }
            public double totalamount { get; set; }
            public double paidamount { get; set; }

        }
    }

    public class DelivaryCreditAmountApiViewModel
    {
        public double TotalAmount { get; set; }
        public double EarningOfToday { get; set; }
        public double TargetAmount { get; set; }
        public int DeliveryPaymentStatus { get; set; }
        public List<CartList> List { get; set; }
        public class CartList
        {

            public int OrderNo { get; set; }
            public double GrossDeliveryCharge { get; set; }

            public string Amount { get; set; }
            public int DeliveryBoyPaymentStatus { get; set; }
            public int CartStatus { get; set; }
            public DateTime DateEncoded { get; set; }
            public string Date { get; set; }
        }
    }

    public class ShopOrderAmountApiViewModel
    {
        public double TotalAmount { get; set; }
        public int ShopPaymentStatus { get; set; }
        public List<CartList> List { get; set; }
        public class CartList
        {

            public int OrderNo { get; set; }
            public string Amount { get; set; }
            public int ShopPaymentStatus { get; set; }
            public int CartStatus { get; set; }
            public DateTime DateEncoded { get; set; }
            public string Date { get; set; }
        }
    }

    public class CartDelivaryListApiViewModel
    {
        public List<CartList> ResturantList { get; set; }
        public List<CartList> OtherList { get; set; }
        public class CartList
        {
            public string ShopName { get; set; }
            public string ShopAddress { get; set; }
            public string ShopPhoneNumber { get; set; }
            public double ShopLatitude { get; set; }
            public double ShopLongitude { get; set; }
            public double CustomerLatitude { get; set; }
            public double CustomerLongitude { get; set; }
            public int OrderNo { get; set; }
            public double Amount { get; set; }
            public string CustomerPhoneNumber { get; set; }
            public string CustomerName { get; set; }
            public string DeliveryAddress { get; set; }
            public int CartStatus { get; set; }
            public DateTime DateEncoded { get; set; }
            public string PaymentMode { get; set; }
            public string Date { get; set; }
            public int OnWork { get; set; }
            public Double? RefundAmount { get; set; }
            public string RefundRemark { get; set; }
            public List<OrderItem> OrderList { get; set; }
        }
    }

    public class CartDelivaredListApiViewModel
    {


        public List<CartList> List { get; set; }
        public class CartList
        {
            public int Id { get; set; }
            public string ShopName { get; set; }
            public int OrderNo { get; set; }
            public string ProductName { get; set; }
            public double Price { get; set; }
            public int CartStatus { get; set; }
            public string Date { get; set; }
            public DateTime DateUpdated { get; set; }

        }
    }

    public class CartCancelListApiViewModel
    {


        public List<CartList> List { get; set; }
        public class CartList
        {
            public int Id { get; set; }
            public string OrderNo { get; set; }
            public string ShopName { get; set; }
            public string ProductName { get; set; }
            public string PhoneNumber { get; set; }
            public double Price { get; set; }
            public string CustomerName { get; set; }
            public string Qty { get; set; }
            public string Date { get; set; }
            public List<OrderItem> OrderList { get; set; }
            public DateTime DateEncoded { get; set; }
        }
    }

    public class CartAcceptListApiViewModel
    {
        public List<CartList> List { get; set; }
        public class CartList
        {
            public int Id { get; set; }
            public int OrderNo { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string PhoneNumber { get; set; }
            public string PaymentMode { get; set; }
            public double Price { get; set; }
            public double SinglePrice { get; set; }
            public string CustomerName { get; set; }
            public string DeliveryAddress { get; set; }
            public double OriginalAmount { get; set; }
            public double ShopLatitude { get; set; }
            public double ShopLongitude { get; set; }
            public double PackingCharge { get; set; }
            public double ConvinenientCharge { get; set; }
            public double Amount { get; set; }
            public double GrossDeliveryCharge { get; set; }
            public double ShopDeliveryDiscount { get; set; }
            public double NetDeliveryCharge { get; set; }
            public int Qty { get; set; }
            public int CartStatus { get; set; }           
            public string Date { get; set; }
            public List<OrderItem> OrderList { get; set; }
            public DateTime DateEncoded { get; set; }
        }
    }

    public class CartListApiViewModel
    {


        public List<CartList> List { get; set; }
        public class CartList
        {
            public int Id { get; set; }
            public int OrderNo { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public string ShopPhoneNumber { get; set; }
            public string PaymentMode { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string PhoneNumber { get; set; }
            public double Price { get; set; }
            public string Otp { get; set; }
            public string CustomerName { get; set; }
            public string DeliveryAddress { get; set; }
            public int DeliveryBoyId { get; set; }
            public string DeliveryBoyName { get; set; }
            public string DeliveryBoyPhoneNumber { get; set; }
            public double OriginalAmount { get; set; }
            public double ShopLatitude { get; set; }
            public double ShopLongitude { get; set; }
            public double ? PackingCharge { get; set; }
            public double ConvinenientCharge { get; set; }
            public double Amount { get; set; }
            public double GrossDeliveryCharge { get; set; }
            public double ShopDeliveryDiscount { get; set; }
            public double NetDeliveryCharge { get; set; }
            public int Qty { get; set; }
            public int CartStatus { get; set; }
            public string Date { get; set; }
            public int OnWork { get; set; }
            public List<OrderItem> OrderList { get; set; }
            public DateTime DateEncoded { get; set; }

            public Double ? RfAmount { get; set; }
            public string RefundRemark { get; set; }
        }
    }

}
