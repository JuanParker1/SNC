using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class ProductDishAddOnViewModel
    {
       // public long Id { get; set; }
        public int Id { get; set; }
        public string AddOnItemName { get; set; }
        public Nullable<long> MasterProductId { get; set; }
        public string MasterProductName { get; set; }
        public int AddOnCategoryId { get; set; }
        public string AddOnCategoryName { get; set; }
        public string CrustName { get; set; }
        public int PortionId { get; set; }
        public string PortionName { get; set; }
        public string MinSelectionLimit { get; set; }
        public string MaxSelectionLimit { get; set; }
        public string PortionPrice { get; set; }
        public string AddOnsPrice { get; set; }
        public string CrustPrice { get; set; }
        public string AddOnType { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }
}