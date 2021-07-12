using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class SupportViewModel
    {
        // Live Ordering Count
        public int ShopAcceptanceCount { get; set; }
        public int DeliveryAcceptanceCount { get; set; }
        public int ShopPickupCount { get; set; }
        public int CustomerDeliveryCount { get; set; }
        public int OrderswithoutDeliveryboyCount { get; set; }
        public int ShopCount { get; set; }
        public int CustomerCount { get; set; }
        public int OrderCount { get; set; }
        public int DeliveryBoyLiveCount { get; set; }
        public int RefundCount { get; set; }


        //Verification Count
        public int CustomerAadhaarVerifyCount { get; set; }
        public int ShopOnBoardingVerifyCount { get; set; }
        public int DeliveryBoyVerifyCount { get; set; }
        public int BannerPendingCount { get; set; }
    }

    public class DeliveryBoyAssignViewModel
    {

        public List<AssignList> List { get; set; }
        public class AssignList
        {
            public string Code { get; set; }
            public string ShopCode { get; set; }
            public string ShopName { get; set; }
            public string OrderNo { get; set; }
            public int CartStatus { get; set; }
            public DateTime DateEncoded { get; set; }
        }
        public List<DeliveryBoy> DeliveryBoyList { get; set; }
        public class DeliveryBoy
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }
    }
}