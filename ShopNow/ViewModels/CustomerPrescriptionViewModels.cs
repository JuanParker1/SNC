using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class CustomerPrescriptionWebListViewModel
    {
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int Id { get; set; }
            public int ShopId { get; set; }
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public string CustomerPhoneNumber { get; set; }
            public string ImagePath { get; set; }
            public string AudioPath { get; set; }
            public string Remarks { get; set; }
            public DateTime DateEncoded { get; set; }
            public int Status { get; set; }

            public List<ImagePathList> ImagePathLists { get; set; }
            public class ImagePathList
            {
                public string ImagePath { get; set; }
            }
        }
    }

    public class PrescriptionItemAddViewModel
    {
        public int PrescriptionId { get; set; }
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public long ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }

    public class AddToCartViewModel
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }


        // Shop
        public string ShopName { get; set; }
        public string ShopPhoneNumber { get; set; }
        public string ShopImagePath { get; set; }
        public string ShopAddress { get; set; }

        // Order
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
}