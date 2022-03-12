using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using ShopNow.Models;

namespace ShopNow.ViewModels
{
    public class CartDetailsViewModel
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
        public string ShopAddress { get; set; }
        public int TotalProduct { get; set; }
        public int TotalQuantity { get; set; }
        public double TotalPrice { get; set; }
        public double WalletAmount { get; set; }
        public double NetTotal { get; set; }
        public int OrderNumber { get; set; }
        public int DeliveryBoyId { get; set; }
        public string DeliveryBoyName { get; set; }
        public string DeliveryBoyPhoneNumber { get; set; }
        public int DeliveryBoyIsAssign { get; set; }
        public int DeliveryBoyOnWork { get; set; }
        public double DeliveryCharge { get; set; }
        public double ShopDeliveryDiscount { get; set; }
        public double NetDeliveryCharge { get; set; }
        public double Convinenientcharge { get; set; }
        public double Packingcharge { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Distance { get; set; }
        public int ShopPaymentStatus { get; set; }
        public int DeliveryBoyPaymentStatus { get; set; }
        public int DeliveryOrderPaymentStatus { get; set; }
        public double RatePerOrder { get; set; }
        public double PenaltyAmount { get; set; }
        public string PenaltyRemark { get; set; }
        public double WaitingCharge { get; set; }
        public int WaitingTime { get; set; }
        public string WaitingRemark { get; set; }
        public DateTime? ShopAcceptedTime { get; set; }
        public DateTime? OrderReadyTime { get; set; }
        public DateTime? DeliveryBoyShopReachTime { get; set; }
        public DateTime? OrderPickupTime { get; set; }
        public DateTime? DeliveryLocationReachTime { get; set; }
        public DateTime? DeliveredTime { get; set; }
        public int? OfferId { get; set; }
        public int? ProductFreeOfferId { get; set; }
        public double OfferAmount { get; set; }
        public bool? IsPreorder { get; set; }
        public DateTime? PreorderDeliveryDateTime { get; set; }
        public string RouteAudioPath { get; set; }
        public string Remarks { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public double? RefundAmount{ get; set; }
        public string RefundRemark{ get; set; }
        public string PaymentMode{ get; set; }
        public double TipsAmount{ get; set; }
        public bool IsPrescriptionOrder { get; set; }
        public string PrescriptionImagePath { get; set; }
        public double TotalShopPrice { get; set; }
        public bool IsPickupDrop { get; set; }
        public string CustomerPrescriptionRemarks { get; set; }
        public double PickupLatitude { get; set; }
        public double PickupLongitude { get; set; }

        public DateTime? PickupDateTime { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int DeliverySlotType { get; set; }

        public List<ImagePathList> ImagePathLists { get; set; }
        public class ImagePathList
        {
            public string ImagePath { get; set; }
        }
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public long Id { get; set; }
            public string ProductName { get; set; }
            public string BrandName { get; set; }
            public string CategoryName { get; set; }
            public string ImagePath { get; set; }
            public int Quantity { get; set; }
            public double UnitPrice { get; set; }
            public double Price { get; set; }
            public int AddonType { get; set; }
            public bool HasAddon { get; set; }
            public string UpdateRemarks { get; set; }
            public string UpdatedBy { get; set; }
            public DateTime? UpdatedTime { get; set; }

            public List<AddonListItem> AddonListItems { get; set; }
            public class AddonListItem
            {
                public string PortionName { get; set; }
                public double PortionPrice { get; set; }
                public string CrustName { get; set; }
                public string AddonName { get; set; }
                public double AddonPrice { get; set; }
            }
        }
    }

    public class CartHistoryViewModel
    {
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
            public   int BrandId { get; set; }
            public string BrandName { get; set; }
            public string CategoryCode { get; set; }
            public string CategoryName { get; set; }
            public string ImagePath { get; set; }
            public int OrderNumber { get; set; }
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
        public double NetTotal { get; set; }
        public int OrderNumber { get; set; }
        public int DeliveryBoyId { get; set; }
        public string DeliveryBoyName { get; set; }
        public string DeliveryBoyPhoneNumber { get; set; }
        public double DeliveryCharge { get; set; }
        public double ShopDeliveryDiscount { get; set; }
        public double NetDeliveryCharge { get; set; }
        public double Convinenientcharge { get; set; }
        public double Packingcharge { get; set; }
        public string PaymentMode { get; set; }
        public int Status { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
        public int isAssign { get; set; }
        public int OnWork { get; set; }
        public double PenaltyAmount { get; set; }
        public double WaitingCharge { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string PrescriptionImagePath { get; set; }
        public double Amount { get; set; }
        public double? RefundAmount { get; set; }
        public string RefundRemark { get; set; }
        public List<ImagePathList> ImagePathLists { get; set; }
        public string BillNo { get; set; }
        public double BillAmount { get; set; }
        public double DifferenceAmount { get; set; }
        public double DifferencePercentage { get; set; }
        public bool IsPickupDrop { get; set; }
        public string Remarks { get; set; }
        public class ImagePathList
        {
            public string ImagePath { get; set; }
        }
        public List<TodayList> TodayLists { get; set; }
        public class TodayList
        {
            public int No { get; set; }
            public long Id { get; set; }
            public string OrderNumber { get; set; }
            public string ShopName { get; set; }
            public string DeliveryAddress { get; set; }
            public string ShopPhoneNumber { get; set; }
            public string DeliveryBoyName { get; set; }
            public string PaymentMode { get; set; }
            public double Amount { get; set; }
            public string CustomerPhoneNumber { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public double? RefundAmount { get; set; }
            public string RefundRemark { get; set; }
            public bool IsPickupDrop { get; set; }
            public string OrderStatusText
            {
                get
                {
                    switch (this.Status)
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
                        case 8:
                            return "Order Ready";
                        case 9:
                            return "Customer Cancelled";
                        case 10:
                            return "Customer Not Pickup";
                        default:
                            return "N/A";
                    }
                }
            }
        }

        public List<CartPendingList> CartPendingLists { get; set; }
        public class CartPendingList
        {
            public int No { get; set; }
            public long Id { get; set; }
            public string OrderNumber { get; set; }
            public string ShopName { get; set; }
            public string DeliveryAddress { get; set; }
            public string ShopOwnerPhoneNumber { get; set; }
            public string DeliveryBoyName { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public double Price { get; set; }
            public double? RefundAmount { get; set; }
            public string RefundRemark { get; set; }
            public string PaymentMode { get; set; }
            public bool IsPickupDrop { get; set; }
        }

        public List<CartPreparedList> CartPreparedLists { get; set; }
        public class CartPreparedList
        {
            public int No { get; set; }
            public long Id { get; set; }
            public string OrderNumber { get; set; }
            public string ShopName { get; set; }
            public string DeliveryAddress { get; set; }
            public string ShopPhoneNumber { get; set; }
            public string DeliveryBoyPhoneNumber { get; set; }
            public string DeliveryBoyName { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public double Price { get; set; }
            public double? RefundAmount { get; set; }
            public string RefundRemark { get; set; }
            public string PaymentMode { get; set; }
            public bool IsPickupDrop { get; set; }
        }

        public List<DeliveryAgentAssignedList> DeliveryAgentAssignedLists { get; set; }
        public class DeliveryAgentAssignedList
        {
            public int No { get; set; }
            public long Id { get; set; }
            public string OrderNumber { get; set; }
            public string ShopName { get; set; }
            public string DeliveryBoyPhoneNumber { get; set; }
            public string DeliveryBoyName { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public string PaymentMode { get; set; }
            public double Price { get; set; }
            public double? RefundAmount { get; set; }
            public string RefundRemark { get; set; }
            public bool IsPickupDrop { get; set; }
        }

        public List<PickupList> PickupLists { get; set; }
        public class PickupList
        {
            public int No { get; set; }
            public long Id { get; set; }
            public string OrderNumber { get; set; }
            public string ShopName { get; set; }
            public string DeliveryBoyPhoneNumber { get; set; }
            public string DeliveryBoyName { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public string PaymentMode { get; set; }
            public double Price { get; set; }
            public double Amount { get; set; }
            public double? RefundAmount { get; set; }
            public string RefundRemark { get; set; }
            public bool IsPickupDrop { get; set; }
        }

        public List<OntheWayList> OntheWayLists { get; set; }
        public class OntheWayList
        {
            public int No { get; set; }
            public long Id { get; set; }
            public string OrderNumber { get; set; }
            public string ShopName { get; set; }
            public string DeliveryBoyPhoneNumber { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public string PaymentMode { get; set; }
            public double Price { get; set; }
            public double Amount { get; set; }
            public double? RefundAmount { get; set; }
            public string RefundRemark { get; set; }
            public bool IsPickupDrop { get; set; }
        }

        public List<DeliveredList> DeliveredLists { get; set; }
        public class DeliveredList
        {
            public int No { get; set; }
            public long Id { get; set; }
            public string OrderNumber { get; set; }
            public string ShopName { get; set; }
            public string CustomerPhoneNumber { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public DateTime DateUpdated { get; set; }
            public DateTime? DeliveredTime { get; set; }
            public string PaymentMode { get; set; }
            public double Price { get; set; }
            public double Amount { get; set; }
            public double? RefundAmount { get; set; }
            public string RefundRemark { get; set; }
            public double ShopAcceptedTime { get; set; }
            public double OrderPeriod { get; set; }
            public bool IsPickupDrop { get; set; }
        }

        public List<CancelledList> CancelledLists { get; set; }
        public class CancelledList
        {
            public int No { get; set; }
            public long Id { get; set; }
            public string OrderNumber { get; set; }
            public string ShopName { get; set; }
            public string CustomerPhoneNumber { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public string PaymentMode { get; set; }
            public double Price { get; set; }
            public double Amount { get; set; }
            public double? RefundAmount { get; set; }
            public string RefundRemark { get; set; }
            public double ShopCancelPeriod { get; set; }
            public DateTime? ShopCancelledTime { get; set; }
            public double OrderPeriod { get; set; }
            public string CancelledRemark { get; set; }
        }

        public List<CustomerCancelledList> CustomerCancelledLists { get; set; }
        public class CustomerCancelledList
        {
            public int No { get; set; }
            public long Id { get; set; }
            public string OrderNumber { get; set; }
            public string ShopName { get; set; }
            public string CustomerPhoneNumber { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public string PaymentMode { get; set; }
            public double Price { get; set; }
            public double Amount { get; set; }
            public double? RefundAmount { get; set; }
            public string RefundRemark { get; set; }
            public double CustomerCancelPeriod { get; set; }
            public DateTime? CustomerCancelledTime { get; set; }
            public double OrderPeriod { get; set; }
        }

        public List<CustomerNotPickupList> CustomerNotPickupLists { get; set; }
        public class CustomerNotPickupList
        {
            public int No { get; set; }
            public long Id { get; set; }
            public string OrderNumber { get; set; }
            public string ShopName { get; set; }
            public string CustomerPhoneNumber { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public DateTime DateUpdated { get; set; }
            public string PaymentMode { get; set; }
            public double Price { get; set; }
            public double Amount { get; set; }
            public double? RefundAmount { get; set; }
            public string RefundRemark { get; set; }
            public double ShopAcceptedTime { get; set; }
            public double OrderPeriod { get; set; }
        }

        public List<CartList> List { get; set; }
        public class CartList
        {
            public long Id { get; set; }
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public string CustomerPhoneNumber { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public int OrderNumber { get; set; }
            public long ProductId { get; set; }
            public string ProductName { get; set; }
            public string ShopPhoneNumber { get; set; }
            public string ShopOwnerPhoneNumber { get; set; }
            public string PhoneNumber { get; set; }
            public int BrandId { get; set; }
            public string BrandName { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string DeliveryAddress { get; set; }
            public string ImagePath { get; set; }
            public int DeliveryBoyId { get; set; }
            public string DeliveryBoyName { get; set; }
            public string DeliveryBoyPhoneNumber { get; set; }
            public int Qty { get; set; }
            public double Price { get; set; }
            public string CreatedBy { get; set; }
            public string UpdatedBy { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public DateTime DateUpdated { get; set; }
            public int isAssign { get; set; }
            public int OnWork { get; set; }
            public double SinglePrice { get; set; }
            public double? RefundAmount { get; set; }
            public string RefundRemark { get; set; }
            public string PaymentMode { get; set; }
            public double Amount { get; set; }
            public double MRPPrice { get; set; }
            public Nullable<System.DateTime> ShopCancelledTime { get; set; }
            public double ShopAcceptedTime { get; set; }
            public double OrderPeriod { get; set; }
            public string OrderStatusText
            {
                get
                {
                    switch (this.Status)
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
                        case 8:
                            return "Order Ready";
                        case 9:
                            return "Customer Cancelled";
                        case 10:
                            return "Customer Not Pickup";
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
        public long OrderId { get; set; }
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
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string StartingDate { get; set; }
        public string EndingDate { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int YearFilter { get; set; }
        public int StartMonthFilter { get; set; }
        public int DeliveryBoyId { get; set; }
        public string DeliveryBoyName { get; set; }
        public List<CartReportList> List { get; set; }
        public class CartReportList
        {
            public long Id { get; set; }
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string PhoneNumber { get; set; }
            public int BrandId { get; set; }
            public string BrandName { get; set; }
            public string CategoryCode { get; set; }
            public string CategoryName { get; set; }
            public string DeliveryAddress { get; set; }
            public string ImagePath { get; set; }
            public int DeliveryBoyId { get; set; }
            public string DeliveryBoyName { get; set; }
            public string DeliveryBoyPhoneNumber { get; set; }
            public string UserEnquiryCode { get; set; }
            public string UserEnquiryName { get; set; }
            public int OrderNumber { get; set; }
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
        public List<DeliveredReportList> DeliveredReportLists { get; set; }
        public class DeliveredReportList {
            public int No { get; set; }
            public long Id { get; set; }
            public string OrderNumber { get; set; }
            public string ShopName { get; set; }
            public string PhoneNumber { get; set; }
            public string DeliveryAddress { get; set; }
            public System.DateTime DateEncoded { get; set; }
            public string PaymentMode { get; set; }
            public double Amount { get; set; }
        }
        public List<CancelledReportList> CancelledReportLists { get; set; }
        public class CancelledReportList
        {
            public long Id { get; set; }
            public string OrderNumber { get; set; }
            public string ShopName { get; set; }
            public string PhoneNumber { get; set; }
            public string DeliveryAddress { get; set; }
            public string CancelledRemark { get; set; }
            public System.DateTime DateEncoded { get; set; }
        }
        public List<DeliveryBoyPaymentStatusList> DeliveryBoyPaymentStatusLists { get; set; }
        public class DeliveryBoyPaymentStatusList
        {
            public long Id { get; set; }
            public int OrderNumber { get; set; }
            public System.DateTime DateEncoded { get; set; }
            public int DeliveryBoyId { get; set; }
            public string DeliveryBoyName { get; set; }
            public string DeliveryBoyPhoneNumber { get; set; }
            public double DeliveryCharge { get; set; }
            public int DeliveryBoyPaymentStatus { get; set; }
            public double Distance { get; set; }
            public string ShopName { get; set; }
        }

        public List<PickUpDropReportList> PickUpDropReportLists { get; set; }
        public class PickUpDropReportList
        {
            public int No { get; set; }
            public long Id { get; set; }
            public string OrderNumber { get; set; }
            public string ShopName { get; set; }
            public string PhoneNumber { get; set; }
            public string DeliveryAddress { get; set; }
            public System.DateTime DateEncoded { get; set; }
            public string PaymentMode { get; set; }
            public double Amount { get; set; }
        }
    }

    public class ShopDeliveredAmountReportViewModel
    {
        public List<ShopDeliveredAmountReportList> List { get; set; }
        public class ShopDeliveredAmountReportList
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public double OriginalAmount { get; set; }
            public double GSTAmount { get; set; }
            public DateTime DateEncoded { get; set; }
            public DateTime DateUpdated { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
    }

    public class OrderRatioViewModel
    {
        public int MonthFilter { get; set; }
        public int YearFilter { get; set; }
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public string Date { get; set; }
            public int TotalOrder { get; set; }
            public int NewOrder { get; set; }
            public int DeliveredOrder { get; set; }
            public int CancelOrder { get; set; }
            public int ResTotal { get; set; }
            public int VegTotal { get; set; }
            public int MedicalTotal { get; set; }
            public int ResNewOrder { get; set; }
            public int VegNewOrder { get; set; }
            public int MedicalNewOrder { get; set; }
            public int ResCancelOrder { get; set; }
            public int VegCancelOrder { get; set; }
            public int MedicalCancelOrder { get; set; }
            public int CustomerCount { get; set; }
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
        public int OrderNumber { get; set; }
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
        public int OrderNumber { get; set; }
        public string DeliveryAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Distance { get; set; }
        public string ReferralNumber { get; set; }
        public int? OfferId { get; set; }
        public int? ProductFreeOfferId { get; set; }
        public bool? IsPreorder { get; set; }
        public DateTime? PreorderDeliveryDateTime { get; set; }
        public string RouteAudioPath { get; set; }
        public string Remarks { get; set; }
        public string PrescriptionImagePath { get; set; }
        public string PaymentMode { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int DeliverySlotType { get; set; }

        public List<ListItem> ListItems { get; set; } 
        public class ListItem
        {
            public long ProductId { get; set; }
            public string ProductName { get; set; }
            public int BrandId { get; set; }
            public string BrandName { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string ImagePath { get; set; }
            public int Quantity { get; set; }
            public double UnitPrice { get; set; }
            public double Price { get; set; }
            public double MRPPrice { get; set; }
            public double ShopPrice { get; set; }
            public int ItemId { get; set; }
            public bool HasAddon { get; set; }
            public int AddOnType { get; set; }
            public int AddOnIndex { get; set; }

            public List<AddOnListItem> AddOnListItems { get; set; }
            public class AddOnListItem
            {
                public int Index { get; set; }
                public long ProductId { get; set; }
                public int AddonId { get; set; }
                public string AddonName { get; set; }
                public double AddonPrice { get; set; }
                public int CrustId { get; set; }
                public string CrustName { get; set; }
                public int PortionId { get; set; }
                public double PortionPrice { get; set; }
                public string PortionName { get; set; }
            }
        }
    }

    public class CartDelivaryShopApiViewModel
    {
        public List<CartList> List { get; set; }
        public class CartList
        {
            public int Id { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public int OrderNumber { get; set; }
            public string Qty { get; set; }
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
        public double TotalAmount { get; set; }
        public List<CartList> List { get; set; }
        public class CartList
        {
            public int OrderNumber { get; set; }
            public double GrossDeliveryCharge { get; set; }
            public double DeliveryCharge { get; set; }
            public int CartStatus { get; set; }
            public DateTime DateEncoded { get; set; }
            public double ShopLatitude { get; set; }
            public double ShopLongitude { get; set; }
            public double CustomerLatitude { get; set; }
            public double CustomerLongitude { get; set; }
            public double TipAmount { get; set; }
            public double Distance { get; set; }
        }
    }

    public class DelivaryBoyPayoutReportViewModel
    {        
        public List<PayoutOut> List { get; set; }
        public class PayoutOut
        {
            //public string Date { get; set; }
            public DateTime Date { get; set; }
            public double TotalAmount { get; set; }
            public double PaidAmount { get; set; }
            public double Amount { get; set; }
            public double TipAmount { get; set; }
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
            public int OrderNumber { get; set; }
            public double GrossDeliveryCharge { get; set; }
            public double Amount { get; set; }
            public int DeliveryBoyPaymentStatus { get; set; }
            public int CartStatus { get; set; }
            public DateTime DateEncoded { get; set; }
            //public string Date { get; set; }
        }
    }

    public class ShopOrderAmountApiViewModel
    {
        public double TotalAmount { get; set; }
        public int ShopPaymentStatus { get; set; }
        public List<CartList> List { get; set; }
        public class CartList
        {
            public int OrderNumber { get; set; }
            public double Amount { get; set; }
            public int ShopPaymentStatus { get; set; }
            public int CartStatus { get; set; }
            public DateTime DateEncoded { get; set; }
            //public string Date { get; set; }
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
            public int OrderNumber { get; set; }
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
            public long Id { get; set; }
            public string ShopName { get; set; }
            public int OrderNumber { get; set; }
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
            public int OrderNumber { get; set; }
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
            public long Id { get; set; }
            public int OrderNumber { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public long ProductId { get; set; }
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
            public double WalletAmount { get; set; }
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
            public long Id { get; set; }
            public int OrderNumber { get; set; }
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

    public class BatchOrderListViewModel
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public long Id { get; set; }
            public string ShopName { get; set; }
            public int OrderNumber { get; set; }
            public string DeliveryAddress { get; set; }
            public string ShopOwnerPhoneNumber { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public string PaymentMode { get; set; }
            public string DeliveryBoyName { get; set; }
            public double Price { get; set; }
            public double CustomerLatitude { get; set; }
            public double CustomerLongitude { get; set; }
            public double ShopLatitude { get; set; }
            public double ShopLongitude { get; set; }
        }
    }

    public class MultipleOrderAssignDeliveryBoyViewModel
    {
        public int DeliveryBoyId { get; set; }
        public List<OrderList> OrderLists { get; set; }
        public class OrderList
        {
            public long OrderId { get; set; }
        }
    }

    public class OrderListViewModel
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string District { get; set; }
        public int Status { get; set; }
        public bool IsPickupDrop { get; set; }
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int No { get; set; }
            public long Id { get; set; }
            public string OrderNumber { get; set; }
            public string ShopName { get; set; }
            public string DeliveryAddress { get; set; }
            public string ShopPhoneNumber { get; set; }
            public string DeliveryBoyName { get; set; }
            public string PaymentMode { get; set; }
            public double Amount { get; set; }
            public string CustomerPhoneNumber { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public double? RefundAmount { get; set; }
            public string RefundRemark { get; set; }
            public bool IsPickupDrop { get; set; }
            public string ShopDistrict { get; set; }
            public string OrderStatusText
            {
                get
                {
                    switch (this.Status)
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
                        case 8:
                            return "Order Ready";
                        case 9:
                            return "Customer Cancelled";
                        case 10:
                            return "Customer Not Pickup";
                        default:
                            return "N/A";
                    }
                }
            }
        }
    }
}
