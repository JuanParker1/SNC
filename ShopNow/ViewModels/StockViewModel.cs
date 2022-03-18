using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class StockViewModel
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string errorMessage { get; set; }
        public HttpPostedFileBase excel_file { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
        public System.Data.DataTable DataTable { get; set; }
    }
}