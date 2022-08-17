using System;

namespace SSMO.Services.Reports
{
    public class CustomerOrderDetailsModel
    {
        public int Id { get; init; }
        public string Number { get; set; }

        public DateTime Date { get; set; }
        public string LoadingPlace { get; set; }
        public string DeliveryAddress { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
    }
}
