using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class ServiceListViewModel
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int Status { get; set; }
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public string PickupAddress { get; set; }
            public string DeliveryAddress { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public double Amount { get; set; }
            public double DeliveryCharge { get; set; }
            public string Distance { get; set; }
            public string Remarks { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public int OrderNumber { get; set; }
            public string ShopName { get; set; }
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
        }
    }

    public class ServiceCreateViewModel
    {
        public int CustomerId { get; set; }
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
        public List<DistanceList> DistanceLists { get; set; }
        public class DistanceList
        {
            public int OrderNumber { get; set; }
            public double ShopLatitude { get; set; }
            public double ShopLongitude { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
    }
}