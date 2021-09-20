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
            public int Id{ get; set; }
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
        public int Id{ get; set; }
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
        public int Id { get; set; }
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
        public string PinCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LandMark { get; set; }
        public int AddressType { get; set; }
        public string FlatNo { get; set; }
        public int Position { get; set; }
        public string ImageAadharPath { get; set; }
        public string AadharName { get; set; }
        public string AadharNumber { get; set; }
        public bool AgeVerify { get; set; }
        public bool AadharVerify { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string FcmTocken { get; set; }
        public string IpAddress { get; set; }
        public double WalletAmount { get; set; }
        public double PenaltyAmount { get; set; }
        public double DeliveryWaitingCharge { get; set; }
        public Nullable<bool> IsReferred { get; set; }
        public string ReferralNumber { get; set; }
        public int Status { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
    }

    //Api

    public class CustomerPasswordViewModel
    {
        public int CustomerId { get; set; }
        public int StaffId { get; set; }
        public string Password { get; set; }
        
    }

    public class CustomerCreateViewModel
    {
        public int Id { get; set; }
        public string ImageAadharPath { get; set; }
        public string AadharName { get; set; }
        public string AadharNumber { get; set; }
        public DateTime? DOB { get; set; }
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
        public int Id { get; set; }
        public string Name { get; set; }
        public int CustomerId{ get; set; }
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
        public int Id { get; set; }
        public int CustomerId { get; set; }
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
            public int Id{ get; set; }
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

