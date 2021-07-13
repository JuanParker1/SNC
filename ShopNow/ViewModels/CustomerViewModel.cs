using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class CustomerListViewModel
    {
        public List<CustomerList> List { get; set; }
        public class CustomerList
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            public string ImagePath { get; set; }
            public string CountryName { get; set; }
            public string StateName { get; set; }
            public string DistrictName { get; set; }
            public string Address { get; set; }
        }

    }

    public class CustomerEditViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string DOB { get; set; }
        public string ImagePath { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string StreetName { get; set; }
        public string ImageAadharPath { get; set; }
        public string AadharName { get; set; }
        public string AadharNumber { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int AddressType { get; set; }
        public string LandMark { get; set; }
        public string FlatNo { get; set; }
        public int Position { get; set; }
        public int Status { get; set; }
        public bool AgeVerify { get; set; }
        public bool AadharVerify { get; set; }
        public DateTime DateUpdated { get; set; }
    }

    public class CustomerDetailsViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string DOB { get; set; }
        public string ImagePath { get; set; }
        public string ImageAadharPath { get; set; }
        public string AadharName { get; set; }
        public string AadharNumber { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string StreetName { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<AddressList> List { get; set; }
        public class AddressList
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string CountryName { get; set; }
            public string StateName { get; set; }
            public string DistrictName { get; set; }
            public string StreetName { get; set; }
            public string Address { get; set; }
            public string PinCode { get; set; }
        }
    }

    //Api

    public class CustomerPasswordViewModel
    {
        public string CustomerCode { get; set; }
        public string StaffCode { get; set; }
        public string Password { get; set; }
        
    }

    public class CustomerCreateViewModel
    {
        public string Code { get; set; }
        public string ImageAadharPath { get; set; }
        public string AadharName { get; set; }
        public string AadharNumber { get; set; }
        public string DOB { get; set; }
        public bool AgeVerify { get; set; }
        public int AddressType { get; set; }
        public string Name { get; set; }
        public string LandMark { get; set; }
        public string FlatNo { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string StreetName { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class CustomerAddOnsAddressViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int AddressType { get; set; }
        public string LandMark { get; set; }
        public string FlatNo { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string StreetName { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class CustomerAddressViewModel
    {
        public string Code { get; set; }
        public string CustomerCode { get; set; }
        public string Name { get; set; }
        public int AddressType { get; set; }
        public string LandMark { get; set; }
        public string FlatNo { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }
        public string StreetName { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
    public class CustomerAddressListViewModel
    {
        public List<CustomerList> List { get; set; }
        //public List<CustomerList> HomeList { get; set; }
        //public List<CustomerList> WorkList { get; set; }
        //public List<CustomerList> ResidentialList { get; set; }
        //public List<CustomerList> TemporaryList { get; set; }
        //public List<CustomerList> RecipientList { get; set; }
        //public List<CustomerList> OtherList { get; set; }
        public class CustomerList
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public int AddressType { get; set; }
            public string CountryName { get; set; }
            public string StateName { get; set; }
            public string DistrictName { get; set; }
            public string StreetName { get; set; }
            public string Address { get; set; }
            public string LandMark { get; set; }
            public string FlatNo { get; set; }
            public string PinCode { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

    }
    public class CustomerProfileViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public string ImageAadharPath { get; set; }
        public string AadharName { get; set; }
        public string AadharNumber { get; set; }
        public bool AgeVerify { get; set; }
        public bool AadharVerify { get; set; }
        public string DOB { get; set; }
    }
}
