using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class AppDetailIndexViewModel
    {
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int Id { get; set; }
            public int DeviceType { get; set; }
            public string Name { get; set; }
            public string Version { get; set; }
            public DateTime DateUpdated { get; set; }

            public string Device
            {
                get
                {
                    switch (this.DeviceType)
                    {
                        case 2:
                            return "IOS";
                        default:
                            return "Android";
                    }
                }
            }
        }
    }
}