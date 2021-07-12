using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class BannerEditViewModel
    {
        public HttpPostedFileBase BannerImage { get; set; }
        public int Id { get; set; }
        public string Code { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Position { get; set; }
        public string Bannerpath { get; set; }
        public System.DateTime Fromdate { get; set; }
        public System.DateTime Todate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
        public int Days { get; set; }
        public string BannerName { get; set; }
        public int paymentid { get; set; }
        public int? CreditType { get; set; }
    }

    public class BannerListViewModel
    {
        public List<BannerList> List { get; set; }
        public class BannerList
        {
            public string Code { get; set; }
            public string ShopCode { get; set; }
            public string ShopName { get; set; }
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public int Position { get; set; }
            public string BannerName { get; set; }
            public string Bannerpath { get; set; }
            public DateTime Fromdate { get; set; }
            public DateTime Todate { get; set; }
            public int Days { get; set; }
            public int? CreditType { get; set; }
        }
        
    }
}