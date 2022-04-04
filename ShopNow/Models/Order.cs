//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopNow.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        public long Id { get; set; }
        public int OrderNumber { get; set; }
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
        public double WalletAmount { get; set; }
        public double NetTotal { get; set; }
        public int DeliveryBoyId { get; set; }
        public string DeliveryBoyName { get; set; }
        public string DeliveryBoyPhoneNumber { get; set; }
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
        public Nullable<System.DateTime> ShopAcceptedTime { get; set; }
        public Nullable<System.DateTime> OrderReadyTime { get; set; }
        public Nullable<System.DateTime> DeliveryBoyShopReachTime { get; set; }
        public Nullable<System.DateTime> OrderPickupTime { get; set; }
        public Nullable<System.DateTime> DeliveryLocationReachTime { get; set; }
        public Nullable<System.DateTime> DeliveredTime { get; set; }
        public Nullable<int> OfferId { get; set; }
        public Nullable<int> ProductFreeOfferId { get; set; }
        public double OfferAmount { get; set; }
        public Nullable<bool> IsPreorder { get; set; }
        public Nullable<System.DateTime> PreorderDeliveryDateTime { get; set; }
        public string RouteAudioPath { get; set; }
        public string Remarks { get; set; }
        public string PrescriptionImagePath { get; set; }
        public string CancelledRemark { get; set; }
        public string PaymentMode { get; set; }
        public double TipsAmount { get; set; }
        public double TotalShopPrice { get; set; }
        public double TotalMRPPrice { get; set; }
        public double ActualShopPayment { get; set; }
        public int PaymentModeType { get; set; }
        public Nullable<bool> IsCallActive { get; set; }
        public bool IsPrescriptionOrder { get; set; }
        public int CustomerPrescriptionId { get; set; }
        public bool IsPickupDrop { get; set; }
        public string PickupAddress { get; set; }
        public double PickupLatitude { get; set; }
        public double PickupLongitude { get; set; }
        public double DBDeliveredLatitude { get; set; }
        public double DBDeliveredLongitude { get; set; }
        public int DeliveryBoyOnWork { get; set; }
        public int UploadType { get; set; }
        public int UploadId { get; set; }
        public int CustomerAddressId { get; set; }
        public Nullable<System.DateTime> PickupDateTime { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public int DeliverySlotType { get; set; }
        public int DeliveryRatePercentageId { get; set; }
        public double DeliveryRatePercentage { get; set; }
        public int Status { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
