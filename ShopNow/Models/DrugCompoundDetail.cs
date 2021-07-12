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
    
    public partial class DrugCompoundDetail
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string AliasName { get; set; }
        public string InteractingCompounds { get; set; }
        public string IntakeIndications { get; set; }
        public string IntakeContraindication { get; set; }
        public string MechanismOfAction { get; set; }
        public string IndicationTreatmentAgeGroup { get; set; }
        public string CompoundUsageReasons { get; set; }
        public bool Alcohol { get; set; }
        public bool Pregnancy { get; set; }
        public bool Breastfeeding { get; set; }
        public bool Driving { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public System.DateTime DateEncoded { get; set; }
        public System.DateTime DateUpdated { get; set; }
    }
}
