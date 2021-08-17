using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class PaymentCreateViewModel
    {
        public string OrderId { get; set; }
        public string ShippingDetailCode { get; set; }
        public string MemberCode { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string OrderCode { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentType { get; set; }
        public string MID { get; set; }
        public string WEBSITE { get; set; }
        public string INDUSTRY_TYPE_ID { get; set; }
        public string CHANNEL_ID { get; set; }
        public string MOBILE_NO { get; set; }
        public string EMAIL { get; set; }
    }

    public class OtpViewModel
    {
        public long Id { get; set; }
        public int ShopId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Otp { get; set; }
        public string ErrorMessage { get; set; }
        public bool Verify { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
    }

    public class PaymentReportViewModel
    {
        public string StartingDate { get; set; }
        public string EndingDate { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public List<PaymentReportList> List { get; set; }
        public class PaymentReportList
        {
            public long Id { get; set; }
            public string CorporateID { get; set; }
            public string ReferenceCode { get; set; }
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public string Address { get; set; }
            public string GSTINNumber { get; set; }
            public double OriginalAmount { get; set; }
            public double GSTAmount { get; set; }
            public double Amount { get; set; }
            public string PaymentMode { get; set; }
            public string PaymentResult { get; set; }
            public string CountryName { get; set; }
            public string Credits { get; set; }
            public DateTime DateEncoded { get; set; }
            public int OrderNumber { get; set; }
        }
    }

    public class PlatformCreditReportViewModel
    {
        public string StartingDate { get; set; }
        public string EndingDate { get; set; }
        public List<PlatformCreditReportList> List { get; set; }
        public class PlatformCreditReportList
        {
            public long Id { get; set; }
            public int OrderNumber { get; set; }
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public int CartStatus { get; set; }
            public double RatePerOrder { get; set; }
            public DateTime DateEncoded { get; set; }
        }
    }

    public class DeliveryCreditReportViewModel
    {
        public string StartingDate { get; set; }
        public string EndingDate { get; set; }
        public List<DeliveryCreditReportList> List { get; set; }
        public class DeliveryCreditReportList
        {
            public long Id { get; set; }
            public int OrderNumber { get; set; }
            public int CartStatus { get; set; }
            public double DeliveryCharge { get; set; }
        }
    }

    //Api
    public class CreditPaymentViewModel
    {
      //  public List<CreditPaymentListViewModel> List { get; set; }
       // public class CreditPaymentListViewModel
       // {
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public int CreditType { get; set; }
            public double Credits { get; set; }
       // }
    }

    public class PaymentUpdatedApiViewModel
    {        
        public int OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public double UpdatedAmount { get; set; }
        public double UpdatedOriginalAmount { get; set; }
        public double RefundAmount { get; set; }
        public string RefundRemark { get; set; }

    }

    public class paymentsEntries
    {
        public long Id { get; set; }
        public int OrderNumber { get; set; }
        public string paymentId { get; set; }
    }

    public class razorpayOrderCreate
    {        
        public string Price { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
    }

    public class PaymentCreateApiViewModel
    {
        public int OrderId { get; set; }
        public int OrderNumber { get; set; }
        public string CorporateID { get; set; }
        public string ReferenceCode { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string Address { get; set; }
        public string GSTINNumber { get; set; }
        public double OriginalAmount { get; set; }
        public double GSTAmount { get; set; }
        public double Amount { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentResult { get; set; }
        public string Key { get; set; }
        public  int CreditType { get; set; }
        public double ConvenientCharge { get; set; }
        public double PackagingCharge { get; set; }
        public double GrossDeliveryCharge { get; set; }
        public double ShopDeliveryDiscount { get; set; }
        public double NetDeliveryCharge { get; set; }
        public double NetTotal { get; set; }
        public int PaymentCategoryType { get; set; }
        //public string Currency { get; set; }
        //public string CountryName { get; set; }
        //public string CheckSumString { get; set; }
        //public string QueryString { get; set; }
    }

    public class ShopPaymentListViewModel
    {
        public DateTime? EarningDate { get; set; }
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int ShopId { get; set; }
            public string ShopName { get; set; }
            public string ShopOwnerPhoneNumber { get; set; }
            public string TransactionType { get; set; }
            public double FinalAmount { get; set; }
            public DateTime PaymentDate { get; set; }
            public string AccountName { get; set; }
            public string AccountNumber { get; set; }
            public string PaymentId { get; set; }
            public string IfscCode { get; set; }
            public string AccountType { get; set; }
            public int CartStatus { get; set; }
            public int ShopPaymentStatus { get; set; }
            
            //for excel
            public string Identifier { get; set; }
            public string EmailID { get; set; }
            public string EmailBody { get; set; }
            public string DebitAccountNo { get; set; }
            public string CNR { get; set; }
            public string ReceiverIFSC { get; set; }
            public int ReceiverACType
            {
                get
                {
                    switch (this.AccountType)
                    {
                        case "SA":
                            return 10;
                        default:
                            return 11;
                    }
                }
            }
            public string Remarks { get; set; }
            public string PhoneNo { get; set; }
        }
    }

    public class DeliveryBoyPaymentListViewModel
    {
        public DateTime? EarningDate { get; set; }
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int DeliveryBoyId { get; set; }
            public string DeliveryBoyName { get; set; }
            public string DeliveryBoyPhoneNumber { get; set; }
            public string TransactionType { get; set; }
            public double Amount { get; set; }
            public DateTime PaymentDate { get; set; }
            public string AccountName { get; set; }
            public string AccountNumber { get; set; }
            public string PaymentId { get; set; }
            public string IfscCode { get; set; }
            public string AccountType { get; set; }
            public int OrderNo { get; set; }
            public int CartStatus { get; set; }
            public int DeliveryBoyPaymentStatus { get; set; }

            //for excel
            public string Identifier { get; set; }
            public string EmailID { get; set; }
            public string EmailBody { get; set; }
            public int ReceiverACType
            {
                get
                {
                    switch (this.AccountType)
                    {
                        case "SA":
                            return 10;
                        default:
                            return 11;
                    }
                }
            }
        }
    }

    public class RetailerPaymentListViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public long Id { get; set; }
            public int No { get; set; }
            public DateTime? OrderDate { get; set; }
            public string ShopName { get; set; }
            public int OrderNumber { get; set; }
            public string PaymentType { get; set; }
            public string PaymentId { get; set; }
            public double? OrderFirstAmount { get; set; }
            public string RefundeRemark { get; set; }
            public double? RefundAmount { get; set; }
            public int RefundStatus { get; set; }
            public string RefundStatusText
            {
                get
                {
                    switch (this.RefundStatus)
                    {
                        case 1:
                            return "N/A";
                        case 2:
                            return "Failed";
                        default:
                            return "Success";
                    }
                }
            }
            public double? PaidAmount { get; set; }
            public decimal? TransactionFee { get; set; }
            public decimal? TransactionTax { get; set; }
            public double? PaymentAmount { get; set; }
            public DateTime? PaymentDate { get; set; }
            public int ShopPaymentStatus { get; set; }
            public string ShopPaymentStatusText
            {
                get
                {
                    switch (this.ShopPaymentStatus)
                    {
                        case 0:
                            return "Pending";
                        case 1:
                            return "Paid";
                        default:
                            return "N/A";
                    }
                }
            }
        }
    }
}

