using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class MeasurementUnitListViewModel
    {
        public List<UnitList> List { get; set; }
        public class UnitList
        {
            public string Code { get; set; }
            public string UnitName { get; set; }
            public string UnitSymbol { get; set; }
            public string UnitType { get; set; }
            public string ConversionUnit { get; set; }
            public string ConversionFormula { get; set; }
            public int Type { get; set; }
        }
    }
    public class MeasurementUnitCreateEditViewModel
    {
        public string Code { get; set; }
        public string UnitName { get; set; }
        public string UnitSymbol { get; set; }
        public string UnitType { get; set; }
        public string ConversionUnit { get; set; }
        public string ConversionFormula { get; set; }
        public int Type { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
    }
    public class MeasurementUnitMasterViewModel
    {
        public string Code { get; set; }
        public string UnitName { get; set; }
        public string UnitSymbol { get; set; }
        public string UnitType { get; set; }
        public string ConversionUnit { get; set; }
        public string ConversionFormula { get; set; }
        public int Type { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }
}