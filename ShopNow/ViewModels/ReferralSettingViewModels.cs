using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class ReferralSettingIndexViewModel
    {
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int Id { get; set; }
            public string ShopDistrict { get; set; }
            public int PaymentMode { get; set; }
            public double Amount { get; set; }

            public string PaymentModeText
            {
                get
                {
                    switch (this.PaymentMode)
                    {
                        case 1:
                            return "Online Payment";
                        case 2:
                            return "Cash on Hand";
                        default:
                            return "Both";
                    }
                }
            }
        }
    }

    public class ReferralSettingCreateViewModel
    {
        public string ShopDistrict { get; set; }
        public int PaymentMode { get; set; }
        public double Amount { get; set; }
    }

    public class ReferralSettingEditViewModel : ReferralSettingCreateViewModel
    {
        public int Id { get; set; }
    }
}