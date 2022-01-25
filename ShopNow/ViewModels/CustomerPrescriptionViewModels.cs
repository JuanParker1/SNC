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

    public class PrescriptionOrderListViewModel
    {
        public List<PrescriptionOrderList> PrescriptionOrderLists { get; set; }
        public class PrescriptionOrderList
        {
            public int No { get; set; }
            public long Id { get; set; }
            public int OrderNumber { get; set; }
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
        //Prescription
        public int PrescriptionId { get; set; }
        public string AudioPath { get; set; }
        public List<ImagePathList> ImagePathLists { get; set; }
        public class ImagePathList
        {
            public string ImagePath { get; set; }
        }

        //AddToCart
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
        public double ToPay { get; set; }
        public double GrossDeliveryCharge { get; set; }
        public double ShopDeliveryDiscount { get; set; }
        public double NetDeliveryCharge { get; set; }
        public double ConvenientCharge { get; set; }
        public double PackingCharge { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
        public double Distance { get; set; }

        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int ItemId { get; set; }
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
        }
    }
    public class CustomerPrescriptionCancelViewModel
    {
        public int Id { get; set; }
    }
}