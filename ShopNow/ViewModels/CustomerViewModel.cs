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
            public int No{ get; set; }
            public int Id{ get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string CurrentPassword { get; set; }
            public string StateName { get; set; }
            public string DistrictName { get; set; }
            public string Address { get; set; }
            public string AppInfo { get; set; }
            public DateTime DateEncoded { get; set; }

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
        public string ImageAadharPath { get; set; }
        public string AadharName { get; set; }
        public string AadharNumber { get; set; }
        public bool AgeVerify { get; set; }
        public bool AadharVerify { get; set; }
        public DateTime? DOB { get; set; }
        public double WalletAmount { get; set; }
        public double PenaltyAmount { get; set; }
        public double DeliveryWaitingCharge { get; set; }
        public int Status { get; set; }
        public int TotalOrderCount { get; set; }
        public int DeliveredOrderCount { get; set; }
        public int CancelOrderCount { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public string AppVersion { get; set; }

        public List<OrderListItem> OrderListItems { get; set; }
        public class OrderListItem
        {
            public DateTime DateEncoded { get; set; }
            public double Amount { get; set; }
            public int QuantityCount { get; set; }
            public int ProductCount { get; set; }
            public int OrderNumber { get; set; }
            public string ShopName { get; set; }
            public int Status { get; set; }
            public string StatusText
            {
                get
                {
                    switch (this.Status)
                    {
                        case 6:
                            return "Delivered";
                        case 7:
                            return "Cancelled by Shop/Admin";
                        case 9:
                            return "Cancelled by Customer";
                        case 10:
                            return "Not Picked up";
                        default:
                            return "N/A";
                    }
                }
            }
            public string StatusTextColor
            {
                get
                {
                    switch (this.Status)
                    {
                        case 6:
                            return "text-success";
                        default:
                            return "text-danger";
                    }
                }
            }

        }
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
        public string AlternateNumber { get; set; }
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
        public string AlternateNumber { get; set; }
    }
}

