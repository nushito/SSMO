using SSMO.Data.Models;
using System;

namespace SSMO.Models.Reports.ServiceOrders
{
    public class ServiceOrderCollectionDetailViewModel
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public int TransportCompanyId { get; set; }
        public string TransportCompanyName { get; set; }
        public decimal Cost { get; set; }
        public string LoadingAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public int CustomerOrderNumber { get; set; }
        public int? CustomerOrderId { get; set; }
        public string Customer { get; set; }
        public int? SupplierOrderId { get; set; }        
        public string SupplierOrderNumber { get; set; }
        public string Supplier { get; set; }
    }
}
