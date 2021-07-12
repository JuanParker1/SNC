using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class BillCreateEditViewModel
    {
        public string Code { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public int NameOfBill { get; set; }
        public double ConvenientCharge { get; set; }
        public string PlatformCreditRateCode { get; set; }
        public double PlatformCreditRate { get; set; }
        public double DeliveryChargeKM { get; set; }
        public double DeliveryChargeOneKM { get; set; }
        public double DeliveryChargeKM1 { get; set; }
        public double DeliveryChargeOneKM1 { get; set; }
        public double DeliveryChargeCustomer { get; set; }
        public int DeliveryRateSet { get; set; }
        public int DeliveryRateSet1 { get; set; }
        public double PackingCharge { get; set; }
        public double PackingCharge1 { get; set; }
        public double TotalAmount { get; set; }
        public int Distance { get; set; }
        public int Type { get; set; }
        public int ItemType { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }

        public int GeneralCount { get; set; }
        public int SpecialCount { get; set; }
    }
    public class BillListViewModel
    {
        public List<BillList> List { get; set; }
        public class BillList
        {
            public string Code { get; set; }
            public string CustomerCode { get; set; }
            public string CustomerName { get; set; }
            public string ShopCode { get; set; }
            public string ShopName { get; set; }
            public int NameOfBill { get; set; }
            public double ConvenientCharge { get; set; }
            public double PlatformCreditRateCode { get; set; }
            public double PlatformCreditRate { get; set; }
            public double DeliveryChargeKM { get; set; }
            public double DeliveryChargeOneKM { get; set; }
            public double DeliveryChargeCustomer { get; set; }
            public int DeliveryRateSet { get; set; }
            public double PackingCharge { get; set; }
            public double TotalAmount { get; set; }
            public int Distance { get; set; }
            public int Type { get; set; }
            public int ItemType { get; set; }
            public string ItemTypeText
            {
                get
                {
                    switch (this.ItemType)
                    {
                        case 0:
                            return "Single Item";
                        case 1:
                            return "Total Item";
                        default:
                            return "N/A";
                    }
                }
            }
        }
    }
    public class DeliveryListViewModel
    {
        public int GeneralCount1 { get; set; }
        public int GeneralCount2 { get; set; }
        public int GeneralCount3 { get; set; }
        public int SpecialCount1 { get; set; }
        public int SpecialCount2 { get; set; }
        public int SpecialCount3 { get; set; }
        public List<DeliveryList> List { get; set; }
        public class DeliveryList
        {
            public string Code { get; set; }
            public string CustomerCode { get; set; }
            public string CustomerName { get; set; }
            public string ShopCode { get; set; }
            public string ShopName { get; set; }
            public int NameOfBill { get; set; }
            public double PlatformCreditRateCode { get; set; }
            public double PlatformCreditRate { get; set; }
            public double DeliveryChargeKM { get; set; }
            public double DeliveryChargeOneKM { get; set; }
            public int DeliveryRateSet { get; set; }
            public double TotalAmount { get; set; }
            public int Distance { get; set; }
            public int Type { get; set; }
        }
    }

    //Api


    public class BillApiListViewModel
    {
        public List<BillList> List { get; set; }
        public class BillList
        {
            public string Code { get; set; }
            public string CustomerCode { get; set; }
            public string CustomerName { get; set; }
            public string ShopCode { get; set; }
            public string ShopName { get; set; }
            public int NameOfBill { get; set; }
            public double ConvenientCharge { get; set; }
            public double ConvenientChargeRange { get; set; }
            public double DeliveryChargeKM { get; set; }
            public double DeliveryChargeOneKM { get; set; }
            public double DeliveryChargeCustomer { get; set; }
            public int DeliveryRateSet { get; set; }
            public double PackingCharge { get; set; }
            public double TotalAmount { get; set; }
            public int Distance { get; set; }
            public int ItemType { get; set; }
        }
    }

    public class BillUpdate
    {
        public string Code { get; set; }
        public string CustomerCode { get; set; }
        public double ConvenientChargeRange { get; set; }
        public double DeliveryChargeCustomer { get; set; }
        public double PackingCharge { get; set; }
        public int ItemType { get; set; }
    }
}