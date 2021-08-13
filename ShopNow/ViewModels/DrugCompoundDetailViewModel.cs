using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class DrugCompoundDetailListViewModel
    {
        public List<DrugCompoundList> List { get; set; }
        public class DrugCompoundList
        {
            public int Id{ get; set; }
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
        }
    }
    public class DrugCompoundDetailCreateEditViewModel
    {
        public int Id{ get; set; }
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
    }
    public class DrugCompoundDetailMasterViewModel
    {
        public int Id{ get; set; }
        public string AliasName { get; set; }
        public string InteractingCompounds { get; set; }
        public string IntakeIndications { get; set; }
        public string IntakeContraindication { get; set; }
        public string MechanismOfAction { get; set; }
        public string IndicationTreatmentAgeGroup { get; set; }
        public string CompoundUsageReasons { get; set; }
        public string Alcohol { get; set; }
        public string Pregnancy { get; set; }
        public string Breastfeeding { get; set; }
        public string Driving { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }
}