using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class BillingChargeCreateViewModel
    {
        public int ShopId { get; set; }
        public double ConvenientCharge { get; set; }
        public double PackingCharge { get; set; }
        public int ItemType { get; set; }
        public double DeliveryDiscountPercentage { get; set; }
    }
    public class BillingChargeEditViewModel
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public double ConvenientCharge { get; set; }
        public double PackingCharge { get; set; }
        public int ItemType { get; set; }
        public double DeliveryDiscountPercentage { get; set; }
    }

    public class BillingChargeListViewModel
    {
        public List<BillList> List { get; set; }
        public class BillList
        {
            public int Id { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public double ConvenientCharge { get; set; }
            public double PackingCharge { get; set; }
            public int ItemType { get; set; }
            public double DeliveryDiscountPercentage { get; set; }
            public string ItemTypeText
            {
                get
                {
                    switch (this.ItemType)
                    {
                        case 0:
                            return "Item wise";
                        case 1:
                            return "Total Item";
                        default:
                            return "N/A";
                    }
                }
            }
        }
        
    }

    public class BillingChargeEmptyListViewModel
    {
        public List<EmptyBillList> EmptyList { get; set; }
        public class EmptyBillList
        {
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public double PackingCharge { get; set; }
            public double ConvenientCharge { get; set; }
            public bool HasBilling { get; set; }
        }
    }
}