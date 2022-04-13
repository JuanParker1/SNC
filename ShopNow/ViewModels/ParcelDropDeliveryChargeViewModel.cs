using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class ParcelDropDeliveryListViewModel
    {
        public List<ParcelDropDeliveryList> List { get; set; }
        public class ParcelDropDeliveryList
        {
            public int Id { get; set; }
            public double ChargeUpto5Kms { get; set; }
            public double ChargePerKm { get; set; }
            public double ChargeAbove15Kms { get; set; }
            public int Type { get; set; }
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

    public class ParcelDropDeliveryCreateViewModel
    {
        public double ChargeUpto5Kms { get; set; }
        public double ChargePerKm { get; set; }
        public double ChargeAbove15Kms { get; set; }
        public double ChargeUpto5Kms1 { get; set; }
        public double ChargePerKm1 { get; set; }
        public double ChargeAbove15Kms1 { get; set; }
        public int Type { get; set; }
        public int VehicleType { get; set; }
    }

    public class ParcelDropDeliveryEditViewModel
    {
        public int Id { get; set; }
        public double ChargeUpto5Kms { get; set; }
        public double ChargePerKm { get; set; }
        public double ChargeAbove15Kms { get; set; }
        public double ChargeUpto5Kms1 { get; set; }
        public double ChargePerKm1 { get; set; }
        public double ChargeAbove15Kms1 { get; set; }
        public int Type { get; set; }
        public int VehicleType { get; set; }
    }

    public class ParcelDropDeliveryChargeAssignListViewModel
    {
        public List<ParcelDropDeliveryAssignList> List { get; set; }
        public class ParcelDropDeliveryAssignList
        {
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public double ChargeUpto5Kms { get; set; }
            public double ChargePerKm { get; set; }
            public double ChargeAbove15Kms { get; set; }
            public int Type { get; set; }
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

    public class ParcelDropDeliveryChargeAssignCreateViewModel
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int Type { get; set; }
    }

}