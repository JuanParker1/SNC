using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{

    public class StaffCreateEditViewModel
    {
        public HttpPostedFileBase StaffImage { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string ImagePath { get; set; }
        public string IpAddress { get; set; }
        public string Password { get; set; }

        public List<ShopList> List { get; set; }
        public class ShopList
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string CountryName { get; set; }
            public string StateName { get; set; }
            public string DistrictName { get; set; }
            public string StreetName { get; set; }
            public string Address { get; set; }
            public string PinCode { get; set; }
            public string MondayOpenTime { get; set; }
            public string TuesdayOpenTime { get; set; }
            public string WednesdayOpenTime { get; set; }
            public string ThursdayOpenTime { get; set; }
            public string FridayOpenTime { get; set; }
            public string SaturdayOpenTime { get; set; }
            public string SundayOpenTime { get; set; }
            public string MondayCloseTime { get; set; }
            public string TuesdayCloseTime { get; set; }
            public string WednesdayCloseTime { get; set; }
            public string ThursdayCloseTime { get; set; }
            public string FridayCloseTime { get; set; }
            public string SaturdayCloseTime { get; set; }
            public string SundayCloseTime { get; set; }
        }
    }  
    public class StaffListViewModel
    {
        public List<StaffList> List { get; set; }
        public class StaffList
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string CustomerCode { get; set; }
            public string CustomerName { get; set; }
            public string ShopCode { get; set; }
            public string ShopName { get; set; }
            public string ImagePath { get; set; }
            public string IpAddress { get; set; }
        }
    }

}

