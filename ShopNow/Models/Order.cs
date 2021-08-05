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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }
    
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
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Nullable<double> Distance { get; set; }
        public int ShopPaymentStatus { get; set; }
        public int DeliveryBoyPaymentStatus { get; set; }
        public int DeliveryOrderPaymentStatus { get; set; }
        public double RatePerOrder { get; set; }
        public double PenaltyAmount { get; set; }
        public string PenaltyRemark { get; set; }
        public double WaitingCharge { get; set; }
        public int WaitingTime { get; set; }
        public string WaitingRemark { get; set; }
        public Nullable<System.DateTime> OrderReadyTime { get; set; }
        public Nullable<System.DateTime> DeliveryBoyShopReachTime { get; set; }
        public Nullable<System.DateTime> OrderPickupTime { get; set; }
        public System.DateTime DeliveryLocationReachTime { get; set; }
        public Nullable<System.DateTime> DeliveredTime { get; set; }
        public int Status { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual Customer Customer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
