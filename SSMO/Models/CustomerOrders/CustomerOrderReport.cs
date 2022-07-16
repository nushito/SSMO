using System;

namespace SSMO.Models.CustomerOrders
{
    public class CustomerOrderReport
    {
        public int Id { get; set; }
        public string Number { get; set; }
      
        public DateTime Date { get; set; }
        public string LoadingPlace { get; set; }
     
        public string DeliveryAddress { get; set; }
        public string Status { get; set; }
    }
}
