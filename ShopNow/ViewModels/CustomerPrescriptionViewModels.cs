using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class PrescriptionItemAddViewModel
    {
        public int PrescriptionId { get; set; }
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public long ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}