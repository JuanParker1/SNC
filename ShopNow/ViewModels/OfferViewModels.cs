using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class OfferListViewModel
    {
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int OwnerType { get; set; }
            public int DiscountType { get; set; }
            public int Type { get; set; }
            public double Percentage { get; set; }
            public int QuantityLimit { get; set; }
            public int AmountLimit { get; set; }
            public int CustomerCountLimit { get; set; }
            public bool IsForFirstOrder { get; set; }
            public bool IsForOnlinePayment { get; set; }
            public bool IsForBlackListAbusers { get; set; }

            public string OwnerTypeText
            {
                get
                {
                    switch (this.OwnerType)
                    {
                        case 1:
                            return "Shop Now Chat";
                        case 2:
                            return "Shop Owner";
                        case 3:
                            return "Brand Owner";
                        default:
                            return "N/A";
                    }
                }
            }

            public string DiscountTypeText
            {
                get
                {
                    switch (this.DiscountType)
                    {
                        case 1:
                            return "Direct Discount";
                        case 2:
                            return "Wallet Cashback";
                        default:
                            return "N/A";
                    }
                }
            }

            public string TypeText
            {
                get
                {
                    switch (this.Type)
                    {
                        case 1:
                            return "Cart Offer";
                        case 2:
                            return "Item Offer";
                        default:
                            return "N/A";
                    }
                }
            }
        }
    }

    public class OfferCreateViewModel
    {
        public string Name { get; set; }
        public int OwnerType { get; set; }
        public int DiscountType { get; set; }
        public int Type { get; set; }
        public double Percentage { get; set; }
        public int QuantityLimit { get; set; }
        public int AmountLimit { get; set; }
        public int CustomerCountLimit { get; set; }
        public bool IsForFirstOrder { get; set; }
        public bool IsForOnlinePayment { get; set; }
        public bool IsForBlackListAbusers { get; set; }
    }
}