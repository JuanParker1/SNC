using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class DeliveryChargeCreateViewModel
    {
        public double ChargeUpto5Km { get; set; }
        public double ChargePerKm { get; set; }
        public double ChargeUpto5Km1 { get; set; }
        public double ChargePerKm1 { get; set; }
        public int Type { get; set; }
        public int TireType { get; set; }
        public int VehicleType { get; set; }
    }
    public class DeliveryChargeEditViewModel
    {
        public int Id { get; set; }
        public double ChargeUpto5Km { get; set; }
        public double ChargePerKm { get; set; }
        public double ChargeUpto5Km1 { get; set; }
        public double ChargePerKm1 { get; set; }
        public int Type { get; set; }
        public int TireType { get; set; }
        public int VehicleType { get; set; }
    }

    public class DeliveryChargeListViewModel
    {
        public List<DeliveryList> List { get; set; }
        public class DeliveryList
        {
            public int Id { get; set; }
            public double ChargeUpto5Km { get; set; }
            public double ChargePerKm { get; set; }
            public int Type { get; set; }
            public int TireType { get; set; }
            public int VehicleType { get; set; }
            public string TypeText
            {
                get
                {
                    switch (this.Type)
                    {
                        case 0:
                            return "General";
                        case 1:
                            return "Special";
                        default:
                            return "N/A";
                    }
                }
            }
            public string TireTypeText
            {
                get
                {
                    switch (this.TireType)
                    {
                        case 1:
                            return "I Tier";
                        case 2:
                            return "II Tier";
                        case 3:
                            return "III Tier";
                        default:
                            return "N/A";
                    }
                }
            }
            public string VehicleTypeText
            {
                get
                {
                    switch (this.VehicleType)
                    {
                        case 1:
                            return "Bike";
                        case 2:
                            return "Carrier Bike";
                        case 3:
                            return "Auto";
                        default:
                            return "N/A";
                    }
                }
            }
           
        }
    }

    // Delivery Charge Assign

    public class DeliveryChargeAssignCreateViewModel
    {
        public double ChargeUpto5Km { get; set; }
        public double ChargePerKm { get; set; }
        public double ChargeUpto5Km1 { get; set; }
        public double ChargePerKm1 { get; set; }
        public int Type { get; set; }
        public int TireType { get; set; }
        public int VehicleType { get; set; }
    }
    public class DeliveryChargeAssignEditViewModel
    {
        public int Id { get; set; }
        public double ChargeUpto5Km { get; set; }
        public double ChargePerKm { get; set; }
        public double ChargeUpto5Km1 { get; set; }
        public double ChargePerKm1 { get; set; }
        public int Type { get; set; }
        public int TireType { get; set; }
        public int VehicleType { get; set; }
    }

    public class DeliveryChargeAssignListViewModel
    {
        public List<DeliveryAssignList> List { get; set; }
        public class DeliveryAssignList
        {
            public int DeliveryChargeId { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public double ChargeUpto5Km { get; set; }
            public double ChargePerKm { get; set; }
            public int Type { get; set; }
            public int TireType { get; set; }
            public int VehicleType { get; set; }
            public string TypeText
            {
                get
                {
                    switch (this.Type)
                    {
                        case 0:
                            return "General";
                        case 1:
                            return "Special";
                        default:
                            return "N/A";
                    }
                }
            }
            public string TireTypeText
            {
                get
                {
                    switch (this.TireType)
                    {
                        case 1:
                            return "I Tier";
                        case 2:
                            return "II Tier";
                        case 3:
                            return "III Tier";
                        default:
                            return "N/A";
                    }
                }
            }
            public string VehicleTypeText
            {
                get
                {
                    switch (this.VehicleType)
                    {
                        case 1:
                            return "Bike";
                        case 2:
                            return "Carrier Bike";
                        case 3:
                            return "Auto";
                        default:
                            return "N/A";
                    }
                }
            }

        }

    }
}