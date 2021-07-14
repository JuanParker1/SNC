using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShopNow.Models;

namespace ShopNow.ViewModels
{
   

    public class EmployeeDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string StateCode { get; set; }
        public string StateName { get; set; }
        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public string StreetCode { get; set; }
        public string StreetName { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
    }

    

}