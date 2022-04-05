using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class WalletSendAmountViewModel
    {
        public int CustomerGroup { get; set; }
        public double Amount { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}