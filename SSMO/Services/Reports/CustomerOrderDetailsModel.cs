using System;

namespace SSMO.Services.Reports
{
    public class CustomerOrderDetailsModel
    {
        public int Id { get; init; }
        public int OrderConfirmationNumber { get; set; }
        public DateTime Date { get; set; }
        public string LoadingPlace { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryTerms { get; set; }
        public string MyCompanyName { get; set; }
        public string SupplierOrderNumber { get; set; }
        public string SupplierName { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string CustomerName { get; set; }
        public bool PaidStatus { get; set; }
    }
}
