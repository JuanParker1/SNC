using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class CustomerGiftCardListViewModel
    {
        public List<ListItem> ListItems { get; set; } 
        public class ListItem
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }
            public string CustomerPhoneNumber { get; set; }
            public string GiftCardCode { get; set; }
            public double Amount { get; set; }
            public DateTime ExpiryDate { get; set; }
            public int Status { get; set; }

            public string StatusText
            {
                get
                {
                    switch (this.Status)
                    {
                        case 1:
                            return "Applied";
                        default:
                            return "Pending";
                    }
                }
            }

            public string StatusTextColor
            {
                get
                {
                    switch (this.Status)
                    {
                        case 1:
                            return "text-success";
                        default:
                            return "text-primary";
                    }
                }
            }
        }
    }

    public class CustomerGiftCardAddViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public double Amount { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}