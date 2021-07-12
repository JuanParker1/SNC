//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopNow.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Payment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Payment()
        {
            this.Banners = new HashSet<Banner>();
        }
    
        public int Id { get; set; }
        public string Code { get; set; }
        public string CartCode { get; set; }
        public string CorporateID { get; set; }
        public double Amount { get; set; }
        public string PaymentMode { get; set; }
        public string Key { get; set; }
        public int Status { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
        public string ReferenceCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string Address { get; set; }
        public string GSTINNumber { get; set; }
        public double OriginalAmount { get; set; }
        public double GSTAmount { get; set; }
        public string Currency { get; set; }
        public string CountryName { get; set; }
        public string PaymentResult { get; set; }
        public string Credits { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
        public string CheckSumString { get; set; }
        public string QueryString { get; set; }
        public string OrderNo { get; set; }
        public int PaymentCategoryType { get; set; }
        public int CreditType { get; set; }
        public double ConvenientCharge { get; set; }
        public double PackagingCharge { get; set; }
        public double DelivaryCharge { get; set; }
        public double UpdatedAmount { get; set; }
        public double UpdatedOriginalAmount { get; set; }
        public Nullable<double> refundAmount { get; set; }
        public Nullable<double> Rateperorder { get; set; }
        public string refundRemark { get; set; }
        public int refundStatus { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Banner> Banners { get; set; }
    }
}
