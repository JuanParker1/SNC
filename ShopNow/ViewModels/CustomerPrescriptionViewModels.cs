using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class CustomerPrescriptionWebListViewModel
    {
        public List<ListItem> ListItems { get; set; }
        public class ListItem
        {
            public int Id { get; set; }
            public int ShopId { get; set; }
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public string CustomerPhoneNumber { get; set; }
            public string ImagePath { get; set; }
            public string AudioPath { get; set; }
            public string Remarks { get; set; }
            public DateTime DateEncoded { get; set; }
            public int Status { get; set; }

            public List<ImagePathList> ImagePathLists { get; set; }
            public class ImagePathList
            {
                public string ImagePath { get; set; }
            }
        }
    }

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