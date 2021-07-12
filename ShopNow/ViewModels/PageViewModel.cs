using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class PageCreateEditViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int Status { get; set; }
        public DateTime DateEncoded { get; set; }
        public DateTime DateUpdated { get; set; }
    }
    public class PageListViewModel
    {
        public List<PageList> List { get; set; }
        public class PageList
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }
    }
    public class PageMasterViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public System.Data.DataTable DataTable { get; set; }
        public string button { get; set; }
        public string Filename { get; set; }
    }

}

