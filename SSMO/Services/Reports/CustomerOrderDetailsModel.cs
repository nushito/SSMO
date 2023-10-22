using System;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Services.Reports
{
    public class CustomerOrderDetailsModel
    {
        public int Id { get; init; }
        public int OrderConfirmationNumber { get; set; }
        public string CustomerPoNumber { get; set; }
        public DateTime Date { get; set; }
        public string LoadingPlace { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryTerms { get; set; }
        public int MyCompanyId { get; set; }
        public string MyCompanyName { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string CustomerName { get; set; }
        public bool PaidStatus { get; set; }
        public ICollection<ProductsForCustomerOrderDetailsViewModel> Products { get; set; }
        public int FIscalAgentId { get; set; }
        public string FiscalAgentName { get; set; }
    }
}
