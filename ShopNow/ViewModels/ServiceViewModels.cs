using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class ServiceListViewModel
    {
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public string PickupAddress { get; set; }
            public string DeliveryAddress { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public double Amount { get; set; }
            public double DeliveryCharge { get; set; }
            public double Distance { get; set; }
            public string Remarks { get; set; }
            public int Status { get; set; }
            public DateTime DateEncoded { get; set; }
        }
    }
}