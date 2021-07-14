using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class DishAddOnViewModel
    {
        public int Id{ get; set; }
        public string Name { get; set; }
        public string MasterProductName { get; set; }
        public string AddOnCategoryName { get; set; }
        public string PortionCode { get; set; }
        public string PortionName { get; set; }
        public string CrustName { get; set; }
        public string Qty { get; set; }
        public string Price { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }
    public class DishAddOnCreateEditViewModel
    {
        public int Id{ get; set; }
        public string Name { get; set; }
        public string MasterProductCode { get; set; }
        public string MasterProductName { get; set; }
        public string AddOnCategoryCode { get; set; }
        public string AddOnCategoryName { get; set; }
        public string PortionCode { get; set; }
        public string PortionName { get; set; }
        public string CrustName { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
    }
    public class DishAddOnListViewModel
    {
        public List<DishAddOnList> List { get; set; }
        public class DishAddOnList
        {
            public int Id{ get; set; }
            public string Name { get; set; }
            public string MasterProductCode { get; set; }
            public string MasterProductName { get; set; }
            public string AddOnCategoryCode { get; set; }
            public string AddOnCategoryName { get; set; }
            public string PortionCode { get; set; }
            public string PortionName { get; set; }
            public string CrustName { get; set; }
            public int Qty { get; set; }
            public double Price { get; set; }
        }
    }
}