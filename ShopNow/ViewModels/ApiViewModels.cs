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
            public double TipsAmount { get; set; }
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
            public string Otp { get; set; }
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
                public int OfferQuantityLimit { get; set; }
                public int OutletId { get; set; }

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
        public List<OrderList> List { get; set; }
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
            public string CustomerAlternatePhoneNumber { get; set; }
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
            public double TipAmount { get; set; }
           public bool IsPickupDrop { get; set; }
           public int CustomerAddressId { get; set; }
           public string RouteAudioPath { get; set; }
           public string Remarks { get; set; }
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
            public bool IsCompleted { get; set; }
            public bool IsParentCompleted { get; set; }
            public string CompleteText { get; set; }
            public String AcheivementStartDate { get; set; }
            public String AcheivementEndDate { get; set; }
            public String CurrentDate { get; set; }

            public int AcheivementTotalCount { get; set; }
            public int AcheivementRemaingCount { get; set; }

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
            public int AddOnType { get; set; }
            public bool HasAddon { get; set; }
            public int OfferQuantityLimit { get; set; }
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
            public int OfferQuantityLimit { get; set; }
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
        public double WalletAmount { get; set; }
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public string Description { get; set; }
            public double Amount { get; set; }
            public int Type { get; set; } //1-Credit,2-Debit
            public DateTime? Date { get; set; }
            public DateTime? ExpiryDate { get; set; }
            public int Status { get; set; }
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
        public string Remark { get; set; }
        //public double CancellationCharges { get; set; }
    }

    public class SavePrescriptionViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string ImagePath { get; set; }
        public string AudioPath { get; set; }
        public string Remarks { get; set; }
        public int ShopId { get; set; }
        public string DeliveryAddress { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
        public int Type { get; set; } //0-Prescription,1-grocery

        public List<ImageListItem> ImageListItems { get; set; }
        public class ImageListItem
        {
            public string ImagePath { get; set; }
        }
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

    public class ProductDetailsApiViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public double? DiscountCategoryPercentage { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int ShopCategoryId { get; set; }
        public string ShopCategoryName { get; set; }
        public double MenuPrice { get; set; }
        public double Price { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string ColorCode { get; set; }
        public string ImagePath { get; set; }
        public string ImagePath1 { get; set; }
        public string ImagePath2 { get; set; }
        public string ImagePath3 { get; set; }
        public string ImagePath4 { get; set; }
        public string ImagePath5 { get; set; }
        public int Status { get; set; }
        public bool Customisation { get; set; }
        public bool IsOnline { get; set; }
        public TimeSpan? NextOnTime { get; set; }
        public double Size { get; set; }
        public double Weight { get; set; }
        public bool IsPreorder { get; set; }
        public int PreorderHour { get; set; }
        public int OfferQuantityLimit { get; set; }
        public bool? ShopIsOnline { get; set; }
        public TimeSpan? ShopNextOnTime { get; set; }

        public List<SimilarProductsListItem> SimilarProductsListItems { get; set; }
        public class SimilarProductsListItem
        {
            public string Name { get; set; }
            public double Distance { get; set; }
            public string ShopName { get; set; }
            public double MenuPrice { get; set; }
            public double Price { get; set; }
            public double ShopPrice { get; set; }
            public double DiscountPercentage { get; set; }

            public long ProductId { get; set; }
            public int ShopId { get; set; }
            public int ShopCategoryId { get; set; }
            public string ShopImagePath { get; set; }
            public string ShopPhoneNumber { get; set; }
            public bool? ShopIsOnline { get; set; }
            public string ShopAddress { get; set; }
            public double ShopLatitude { get; set; }
            public double ShopLongitude { get; set; }
            public double ShopStatus { get; set; }
            public TimeSpan? ShopNextOnTime { get; set; }
            public bool IsOnline { get; set; }
            public TimeSpan? NextOnTime { get; set; }
        }
    }

    public class MedicalProductDetailsApiViewModel
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public int iBarU { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public double? DiscountCategoryPercentage { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int ShopCategoryId { get; set; }
        public string ShopCategoryName { get; set; }
        public double MRP { get; set; }
        public double SalePrice { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string ColorCode { get; set; }
        public string DrugCompoundDetailIds { get; set; }
        public string DrugCompoundDetailName { get; set; }
        public string ImagePath { get; set; }
        public string ImagePath1 { get; set; }
        public string ImagePath2 { get; set; }
        public string ImagePath3 { get; set; }
        public string ImagePath4 { get; set; }
        public string ImagePath5 { get; set; }
        public int Status { get; set; }
        public bool Customisation { get; set; }
        public bool IsOnline { get; set; }
        public TimeSpan? NextOnTime { get; set; }
        public double Size { get; set; }
        public double Weight { get; set; }
        public bool IsPreorder { get; set; }
        public int PreorderHour { get; set; }
        public int OfferQuantityLimit { get; set; }
        public int Quantity { get; set; }
        public int Itemid { get; set; }
        public int OutletId { get; set; }
        public bool PriscriptionCategory { get; set; }
        public bool? ShopIsOnline { get; set; }
        public TimeSpan? ShopNextOnTime { get; set; }

        public List<SimilarProductsListItem> SimilarProductsListItems { get; set; }
        public class SimilarProductsListItem
        {
            public string Name { get; set; }
            public double Distance { get; set; }
            public string ShopName { get; set; }
            public double MenuPrice { get; set; }
            public double Price { get; set; }
            public double ShopPrice { get; set; }
            public double DiscountPercentage { get; set; }

            public long ProductId { get; set; }
            public int ShopId { get; set; }
            public int ShopCategoryId { get; set; }
            public string ShopImagePath { get; set; }
            public string ShopPhoneNumber { get; set; }
            public bool? ShopIsOnline { get; set; }
            public TimeSpan? ShopNextOnTime { get; set; }
            public string ShopAddress { get; set; }
            public double ShopLatitude { get; set; }
            public double ShopLongitude { get; set; }
            public double ShopStatus { get; set; }
            public bool IsOnline { get; set; }
            public TimeSpan? NextOnTime { get; set; }
        }
    }

    public class CustomerFavoriteAddUpdateViewModel
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public bool IsFavorite { get; set; }

    }

    public class CustomerFavoriteListApiViewModel
    {
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public long ProductId { get; set; }
            public string ProductName { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public double Percentage { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public string ImagePath { get; set; }
            public int Itemid { get; set; }
            public int Quantity { get; set; }
            public double Price { get; set; }
            public double MRP { get; set; }
        }
    }

    public class TopCategoriesAndProductsViewModel
    {
        public List<CategoryListItem> CategoryListItems { get; set; }
        public List<ProductListItem> ProductListItems { get; set; }
        public class CategoryListItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ImagePath { get; set; }
        }

        public class ProductListItem
        {
            public long ProductId { get; set; }
            public string ProductName { get; set; }
            public int? iBarU { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string BrandName { get; set; }
            public double? DiscountCategoryPercentage { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public int ShopCategoryId { get; set; }
            public string ShopCategoryName { get; set; }
            public double MRP { get; set; }
            public double SalePrice { get; set; }
            public string ShortDescription { get; set; }
            public string LongDescription { get; set; }
            public string ColorCode { get; set; }
            public string DrugCompoundDetailIds { get; set; }
            public string DrugCompoundDetailName { get; set; }
            public string ImagePath { get; set; }
            public string ImagePath1 { get; set; }
            public string ImagePath2 { get; set; }
            public string ImagePath3 { get; set; }
            public string ImagePath4 { get; set; }
            public string ImagePath5 { get; set; }
            public int Status { get; set; }
            public bool Customisation { get; set; }
            public bool IsOnline { get; set; }
            public TimeSpan? NextOnTime { get; set; }
            public double Size { get; set; }
            public double Weight { get; set; }
            public bool IsPreorder { get; set; }
            public int PreorderHour { get; set; }
            public int OfferQuantityLimit { get; set; }
            public int Quantity { get; set; }
            public int Itemid { get; set; }
            public bool PriscriptionCategory { get; set; }
            public bool? ShopIsOnline { get; set; }
            public TimeSpan? ShopNextOnTime { get; set; }
        }
    }

    public class SaveCallRecordViewModel
    {
        public string from { get; set; }
        public string to { get; set; }
        public string callduration { get; set; }
        public string callid { get; set; }
        public string Status { get; set; }
        public string recordId { get; set; }
        public DateTime calldate { get; set; }
        public int OrderNo { get; set; }
        public int calltype { get; set; }
    }

    public class SaveFCMTokenViewModel
    {
        public int CustomerId { get; set; }
        public string Token { get; set; }
    }

    public class SaveCustomerDeviceAppInfoViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string AppName { get; set; }
        public string AppId { get; set; }
        public string AppBuild { get; set; }
        public string Version { get; set; }
        public string PhoneModel { get; set; }
        public string Platform { get; set; }
        public string OSVersion { get; set; }
        public string Manufacturer { get; set; }

    }

    public class SaveOnlinePaymentViewModel
    {
        public int OrderNumber { get; set; }
        public string ReferenceCode { get; set; }
        public double TipsAmount { get; set; }
    }


    public class AutoCompleteAllSearchResult
    {
        public List<AllPreferedText_Result> PreferedText { get; set; }
        public List<AllShop_Result> Shop { get; set; }
        public List<AllProduct_Result> Products { get; set; }

        public partial class AllPreferedText_Result
        {
            public string correctword { get; set; }

        }
        public partial class AllShop_Result
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string ImagePath { get; set; }
            public int ShopCategoryId { get; set; }
            public bool OnlineStatus { get; set; }
            public string ShopAddress { get; set; }
            public string StreetName { get; set; }
            public double ShopLatitude { get; set; }
            public double ShopLongitude { get; set; }
            public int Status { get; set; }
            public double Rating { get; set; }
            public double ReviewCount { get; set; }
        }
        public partial class AllProduct_Result
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string ImagePath { get; set; }
            public int ShopCategoryId { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public string ShopImagePath { get; set; }
            public string ShopAddress { get; set; }
            public double ShopLatitude { get; set; }
            public double ShopLongitude { get; set; }
            public bool OnlineStatus { get; set; }
            public int Status { get; set; }
            public double Rating { get; set; }
            public double ReviewCount { get; set; }
            public int ShopCount { get; set; }
            public double StartPrice { get; set; }
            public string ColorCode { get; set; }
        }
    }

    public class SuperMarketCategoryList
    {
        public List<ListItem> TrendingList { get; set; }
        public List<ListItem> AllList { get; set; }
        public class ListItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ImagePath { get; set; }
            public int OrderNo { get; set; }
        }
    }

    public class CustomerBankDetailsCreateViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string AccountNumber { get; set; }
        public string IFSCCode { get; set; }
    }

    public class ShopCreateParcelDropViewModel
    {
        public int ShopId { get; set; }
        public string PickupAddress { get; set; }
        public double PickupLatitude { get; set; }
        public double PickupLongitude { get; set; }
        public string DeliveryAddress { get; set; }
        public double DeliveryLatitude { get; set; }
        public double DeliveryLongitude { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public double Amount { get; set; }
        public double DeliveryCharge { get; set; }
        public double Distance { get; set; }
        public string Remarks { get; set; }
        public int OrderNumber { get; set; }
        public DateTime? PickupDateTime { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int DeliverySlotType { get; set; }
    }

    public class RequestUpiPaymentLink
    {
        public CustomerDetails customer { get; set; }
        public Notify notify { get; set; }
        public bool upi_link { get; set; }
        public double amount { get; set; }
        public string currency { get; set; }
        public int expire_by { get; set; }
        public string reference_id { get; set; }
        public string description { get; set; }
        public string callback_url { get; set; }
        public string callback_method { get; set; }
        public class CustomerDetails
        {
            public string name { get; set; }
            public string contact { get; set; }
            public string email { get; set; }
        }

        public class Notify
        {
            public bool sms { get; set; }
            public bool email { get; set; }
        }

    }

    public class CreateRazorPayUpiPaymentLink
    {
        public double Amount { get; set; }
        public int OrderNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class ShopParcelDropListViewModel
    {
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public string PickupAddress { get; set; }
            public string DeliveryAddress { get; set; }
            public string ShopName { get; set; }
            public string CustomerName { get; set; }
            public string CustomerPhoneNumber { get; set; }
            public double Amount { get; set; }
            public double DeliveryCharge { get; set; }
            public string Distance { get; set; }
            public string Remarks { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public int OrderNumber { get; set; }
            public double RefundAmount { get; set; }
            public string RefundRemarks { get; set; }
            public string StatusText
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
            public string StatusTextColor
            {
                get
                {
                    switch (this.Status)
                    {
                        case 2:
                            return "text-primary";
                        case 3:
                            return "text-primary";
                        case 4:
                            return "text-warning";
                        case 5:
                            return "text-warning";
                        case 6:
                            return "text-success";
                        case 7:
                            return "text-danger";
                        case 8:
                            return "text-warning";
                        case 9:
                            return "text-danger";
                        case 10:
                            return "text-danger";
                        default:
                            return "text-danger";
                    }
                }
            }
        }
    }

    public class AddShopStaffViewModel
    {
        public string ShopOwnerName { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string ImagePath { get; set; }
        public string Password { get; set; }
        public int ShopId { get; set; }
    }
}