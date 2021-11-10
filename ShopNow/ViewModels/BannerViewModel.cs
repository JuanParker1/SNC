using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class BannerCreateViewModel
    {
        public HttpPostedFileBase BannerImage { get; set; }
        public int ShopId { get; set; }
        public long ProductId { get; set; }
        public long MasterProductId { get; set; }
        public int Position { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime Todate { get; set; }
        public int Days { get; set; }
        public string BannerName { get; set; }
        public long PaymentId { get; set; }
        public Nullable<int> CreditType { get; set; }
        public int OfferQuantityLimit { get; set; }
    }
        public class BannerEditViewModel
    {
        public HttpPostedFileBase BannerImage { get; set; }
        public int Id { get; set; }
        public string ShopId { get; set; }
        public string ShopName { get; set; }
        public long ProductId { get; set; }
        public long MasterProductId { get; set; }
        public string ProductName { get; set; }
        public int Position { get; set; }
        public string Bannerpath { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public int Status { get; set; }
        public int Days { get; set; }
        public string BannerName { get; set; }
        public int? CreditType { get; set; }
    }

    public class BannerListViewModel
    {
        public List<BannerList> List { get; set; }
        public class BannerList
        {
            public int Id { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public long ProductId { get; set; }
            public long MasterProductId { get; set; }
            public string ProductName { get; set; }
            public int Position { get; set; }
            public string BannerName { get; set; }
            public string Bannerpath { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public int Days { get; set; }
            public int? CreditType { get; set; }
            public int Status { get; set; }
        }
        
    }
}