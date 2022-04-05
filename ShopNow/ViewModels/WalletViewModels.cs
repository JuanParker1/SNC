using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class WalletIndexViewModel
    {
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int CustomerGroup { get; set; }
            public double Amount { get; set; }
            public DateTime? ExpiryDate { get; set; }
            public DateTime DateEncoded { get; set; }
            public string Encodedby { get; set; }

            public string CustomerGroupText
            {
                get
                {
                    switch (this.CustomerGroup)
                    {
                        case 1:
                            return "Medical Group";
                        case 2:
                            return "Grocery Group";
                        case 3:
                            return "Restaurant Group";
                        case 4:
                            return "Supermarket Group";
                        case 5:
                            return "No Order Group";
                        case 6:
                            return "Last 10 days No Order Group";
                        case 7:
                            return "Last 10 days only Medical order Group";
                        case 8:
                            return "Last 10 days only Grocery order Group";
                        case 9:
                            return "Last 10 days only Restaurant order Group";
                        case 10:
                            return "Last 10 days only Supermarket order Group";
                        default:
                            return "N/A";
                    }
                }
            }
        }
    }

    public class WalletSendAmountViewModel
    {
        public int CustomerGroup { get; set; }
        public double Amount { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}