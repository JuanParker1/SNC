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
            public DateTime? OrderReadyTime { get; set; }
            public double? RefundAmount { get; set; }
            public string RefundRemark { get; set; }
            public string PaymentMode { get; set; }
            public int Onwork { get; set; }
            public bool? IsPreorder { get; set; }
            public DateTime? PreorderDeliveryDateTime { get; set; }
            public List<OrderItemList> OrderItemLists { get; set; }
            public class OrderItemList
            {
                public int ShopId { get; set; }
                public string ShopName { get; set; }
                public long OrderId { get; set; }
                public int OrdeNumber { get; set; }
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
                public Nullable<int> OfferId { get; set; }
                public double OfferAmount { get; set; }
                public double MiddlePrice { get; set; }
                public bool HasAddon { get; set; }
                public int AddOnType { get; set; }

                public List<OrderItemAddonList> OrderItemAddonLists { get; set; }
                public class OrderItemAddonList
                {
                    public long OrderItemId { get; set; }
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
            public string Description { get; set; }
            public Nullable<int> BrandId { get; set; }

            //public List<ShopListItem> ShopListItems { get; set; }
            //public List<ProductListItem> ProductListItems { get; set; }

            //public class ProductListItem
            //{
            //    public long Id { get; set; }
            //}
            //public class ShopListItem
            //{
            //    public int Id { get; set; }
            //}

        }
    }

    public class OfferRelatedApiListViewModel
    {
        public List<ShopOfferListItem> ShopOfferListItems { get; set; }
        public class ShopOfferListItem
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
            public string Description { get; set; }
            public Nullable<int> BrandId { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public string ShopImage { get; set; }
        }

        public List<ProductOfferListItem> ProductOfferListItems { get; set; }
        public class ProductOfferListItem
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
            public string Description { get; set; }
            public Nullable<int> BrandId { get; set; }
            public long ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductImage { get; set; }
        }
    }

    public class CartOfferApiListViewModel
    {
        public List<OfferListItem> OfferListItems { get; set; }
        public class OfferListItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string OfferCode { get; set; }
            public int DiscountType { get; set; }
            public double Percentage { get; set; }
            public int AmountLimit { get; set; }
            public double MinimumPurchaseAmount { get; set; }
            public string Description { get; set; }
        }
    }

    public class ProductFreeOfferApiViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OfferCode { get; set; }
        public double MinimumPurchaseAmount { get; set; }
        public string Description { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
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
            public bool IsCustomerAccepted { get; set; }
            public string ExpiryDate { get; set; }
            public string Description { get; set; }

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
            public int ShopId { get; set; }
            public string ShopName { get; set; }
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
            public double Size { get; set; }
            public double Weight { get; set; }
            public bool IsPreorder { get; set; }
            public int PreorderHour { get; set; }

            public List<OrderItemAddonList> OrderItemAddonLists { get; set; }
            public class OrderItemAddonList
            {
                public long OrderItemId { get; set; }
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
            public double Size { get; set; }
            public double Weight { get; set; }
            public bool IsPreorder { get; set; }
            public int PreorderHour { get; set; }
        }
    }

    public class OldCartCheckViewModel
    {
        public int ShopId { get; set; }
        public List<ProductListItem> ProductListItems { get; set; }
        public class ProductListItem
        {
            public long Id { get; set; }
        }
    }

    public class OldCartResponseViewModel
    {
        public int ShopId { get; set; }
        public bool? ShopIsOnline { get; set; }
        public TimeSpan? ShopNextOnTime { get; set; }
        public List<ProductListItem> ProductListItems { get; set; }
        public class ProductListItem
        {
            public long Id { get; set; }
            public bool? IsOnline { get; set; }
            public bool IsActive { get; set; }
            public double Price { get; set; }
            public TimeSpan? NextOnTime { get; set; }
        }
    }

    public class WalletHistoryViewModel
    {
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public string Text { get; set; }
            public double Amount { get; set; }
            public int Type { get; set; } //1-Credit,2-Debit
            public DateTime Date { get; set; }
        }
    }

    public class ApiProductAddonViewModel
    {
        public int Type { get; set; }
        public int MinLimit { get; set; }
        public int MaxLimit { get; set; }

        public List<PortionListItem> PortionListItems { get; set; }
        public class PortionListItem {
            public int Index { get; set; }
            public int AddonId { get; set; }
            public int PortionId { get; set; }
            public int CrustId { get; set; }
            public string PortionName { get; set; }
            public double PortionPrice { get; set; }
        }

        public List<AddonListItem> AddonListItems { get; set; }
        public class AddonListItem
        {
            public int Index { get; set; }
            public int AddonId { get; set; }
            public int PortionId { get; set; }
            public string AddonName { get; set; }
            public double AddonPrice { get; set; }
            public string AddonCategoryName { get; set; }
            public string ColorCode { get; set; }
            public int CrustId { get; set; }
        }

        public List<CrustListItem> CrustListItems { get; set; }
        public class CrustListItem
        {
            public int CrustId { get; set; }
            public int AddonId { get; set; }
            public int Index { get; set; }
            public string CrustName { get; set; }
        }
    }

    public class BillingDeliveryChargeViewModel
    {
        public int BillingId { get; set; }
        public double ConvenientCharge { get; set; }
        public double ConvenientChargeRange { get; set; }
        public double DeliveryChargeKM { get; set; }
        public double DeliveryChargeOneKM { get; set; }
        public double PackingCharge { get; set; }
        public int ItemType { get; set; }
        public int DeliveryMode { get; set; }
        public double DeliveryDiscountPercentage { get; set; }
        public int Distance { get; set; }
    }

    public class SavePrescriptionViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string ImagePath { get; set; }
        public string AudioPath { get; set; }
        public string Remarks { get; set; }
    }

    public class CustomerPrescriptionListViewModel
    {
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int Id { get; set; }
            public string ImagePath { get; set; }
            public string AudioPath { get; set; }
            public string Remarks { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
        }
    }

    public class PrescriptionDetailsApiViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
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
       // public string DeliveryAddress { get; set; }
        public List<ItemList> ItemLists { get; set; }
        
        public class ItemList
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
            public double Size { get; set; }
            public double Weight { get; set; }
        }
    }
}