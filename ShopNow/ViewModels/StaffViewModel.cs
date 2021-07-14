using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{

    public class StaffCreateEditViewModel
    {
        public HttpPostedFileBase StaffImage { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string ImagePath { get; set; }
        public string IpAddress { get; set; }
        public string Password { get; set; }

        public List<ShopList> List { get; set; }
        public class ShopList
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string CountryName { get; set; }
            public string StateName { get; set; }
            public string DistrictName { get; set; }
            public string StreetName { get; set; }
            public string Address { get; set; }
            public string PinCode { get; set; }
           
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

