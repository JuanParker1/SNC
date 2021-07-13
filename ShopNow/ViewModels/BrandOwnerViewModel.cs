using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class BrandOwnerRegisterEditViewModel
    {
        public HttpPostedFileBase BrandOwnerImage { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string StreetName { get; set; }
        public string Address { get; set; }
        public int AddressType { get; set; }
        public string LandMark { get; set; }
        public string FlatNo { get; set; }
        public string PinCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ImageAuthoriseBrandPath { get; set; }
        public string AuthorisedDistributorNumber { get; set; }
        public string AuthorisedBrandName { get; set; }
        public string AuthorisedBrandCode { get; set; }
        public string AuthorisedDistributorStatus { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string AcountType { get; set; }
        public string IFSCCode { get; set; }
        public string SwiftCode { get; set; }
        public string UPIID { get; set; }
    }
    public class BrandOwnerLoginViewModel
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }

}
