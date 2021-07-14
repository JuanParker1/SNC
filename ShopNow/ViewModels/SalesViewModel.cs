using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class SalesReportViewModel
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string DeliveryAddress { get; set; }
        public string ImagePath { get; set; }
        public string UserEnquiryCode { get; set; }
        public string UserEnquiryName { get; set; }
        public string Qty { get; set; }
        public double Price { get; set; }
        public int CartStatus { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
        public string StartingDate { get; set; }
        public string EndingDate { get; set; }


        public List<SalesReportList> List { get; set; }
        public class SalesReportList
        {
            public int Id { get; set; }
            public string CustomerCode { get; set; }
            public string CustomerName { get; set; }
            public string ShopCode { get; set; }
            public string ShopName { get; set; }
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public string BrandCode { get; set; }
            public string BrandName { get; set; }
            public string CategoryCode { get; set; }
            public string CategoryName { get; set; }
            public string DeliveryAddress { get; set; }
            public string ImagePath { get; set; }
            public string UserEnquiryCode { get; set; }
            public string UserEnquiryName { get; set; }
            public string Qty { get; set; }
            public double Price { get; set; }
            public int CartStatus { get; set; }
            public string UpdatedBy { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
            public DateTime DateUpdated { get; set; }
            public string OrderNo { get; set; }
            public double Amount { get; set; }
        }
    }
}