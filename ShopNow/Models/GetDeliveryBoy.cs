using System.Collections.Generic;

namespace ShopNow.Models
{
    public class GetDeliveryBoy
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Active { get; set; }
        public string ImagePath { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public List<GetDeliveryBoy> LstDeliveryBoy { get; set; }
    }
}