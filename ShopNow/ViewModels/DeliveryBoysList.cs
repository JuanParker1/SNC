using System.Collections.Generic;

namespace ShopNow.ViewModels
{
    public class DeliveryBoysList
    {
       
      
            public int Id { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public int Active { get; set; }
            public string ImagePath { get; set; }
            public List<DeliveryBoysList> List { get; set; }
    }
}