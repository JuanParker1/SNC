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
            public double NetTotal { get; set; }
            public double WalletAmount { get; set; }
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
            public double NetTotal { get; set; }
            public double WalletAmount { get; set; }
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

    public class OfferApiListViewModel
    {
        public List<OfferListItem> OfferListItems { get; set; }
        public class OfferListItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int OwnerType { get; set; }
            public string OfferCode { get; set; }
            public int DiscountType { get; set; }
            public int Type { get; set; }
            public double Percentage { get; set; }
            public int QuantityLimit { get; set; }
            public int AmountLimit { get; set; }
            public int CustomerCountLimit { get; set; }
            public double MinimumPurchaseAmount { get; set; }
            public bool IsForFirstOrder { get; set; }
            public bool IsForOnlinePayment { get; set; }
            public bool IsForBlackListAbusers { get; set; }
            public Nullable<int> BrandId { get; set; }

            public List<ShopListItem> ShopListItems { get; set; }
            public List<ProductListItem> ProductListItems { get; set; }

            public class ProductListItem
            {
                public long Id { get; set; }
            }
            public class ShopListItem
            {
                public int Id { get; set; }
            }

        }
    }

    public class AchievementApiListViewModel
    {
        public List<AchievementListItem> AchievementListItems { get; set; }
        public class AchievementListItem
        {
            public int Id { get; set; }
            public string ShopDistrict { get; set; }
            public string Name { get; set; }
            public int CountType { get; set; }
            public int CountValue { get; set; }
            public double Amount { get; set; }
            public bool HasAccept { get; set; }
            public int DayLimit { get; set; }
            public int ActivateType { get; set; }
            public int ActivateAfterId { get; set; }
            public int RepeatCount { get; set; }
            public bool IsForBlackListAbusers { get; set; }

            public List<ShopListItem> ShopListItems { get; set; }
            public List<ProductListItem> ProductListItems { get; set; }

            public class ProductListItem
            {
                public long Id { get; set; }
            }
            public class ShopListItem
            {
                public int Id { get; set; }
            }

        }
    }


    public class OrderDetailsApiViewModel
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public string ShopImagePath { get; set; }
        public int ShopCategoryId { get; set; }
        public string ShopCategoryName { get; set; }
        public string ShopPhoneNumber { get; set; }
        public double ShopLatitude { get; set; }
        public double ShopLongitude { get; set; }
        public int ShopReview { get; set; }
        public bool? IsShopOnline { get; set; }
        public double ShopRating { get; set; }
        public string DeliveryAddress { get; set; }
        public List<OrderItemList> OrderItemLists { get; set; }
        public List<MedicalOrderItemList> MedicalOrderItemLists { get; set; }

        public class OrderItemList
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string ImagePath { get; set; }
            public int Quantity { get; set; }
            public double Price { get; set; }
            public double TotalPrice { get; set; }
            public string ColorCode { get; set; }
            public bool Customisation { get; set; }
            public int Status { get; set; }
            public bool? IsProductOnline { get; set; }
            public double DiscountCategoryPercentage { get; set; }
        }

        public class MedicalOrderItemList
        {
            public long ProductId { get; set; }
            public string ProductName { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public double DiscountCategoryPercentage { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public string ImagePath { get; set; }
            public string ImagePathLarge1 { get; set; }
            public string ImagePathLarge2 { get; set; }
            public string ImagePathLarge3 { get; set; }
            public string ImagePathLarge4 { get; set; }
            public string ImagePathLarge5 { get; set; }
            public int Itemid { get; set; }
            public int Quantity { get; set; }
            public int Qty { get; set; }
            public double Price { get; set; }
            public double SalePrice { get; set; }
            public double MRP { get; set; }
            public int Status { get; set; }
            public int IBarU { get; set; }
            public bool? IsProductOnline { get; set; }
        }
    }
}