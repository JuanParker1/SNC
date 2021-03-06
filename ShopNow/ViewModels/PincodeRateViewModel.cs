using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class PincodeRateListViewModel
    {
        //public int Id { get; set; }
        //public string PinCode { get; set; }
        //public int DeliveryRateSet { get; set; }
        //public int Remarks { get; set; }

        public List<PincodeRateList> List { get; set; }
        public class PincodeRateList
        {
            public int Id { get; set; }
            public string Pincode { get; set; }
            public int Type { get; set; }
            public int Tier { get; set; }
            public string Remarks { get; set; }
            public int Status { get; set; }
        }
        
    }
}