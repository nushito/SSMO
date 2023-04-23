using Microsoft.AspNetCore.Mvc.Rendering;
using SSMO.Models.SupplierOrders;
using System;
using System.Collections.Generic;

namespace SSMO.Services.Documents.Purchase
{
    public class PurchaseModelAsPerSpec
    {
        public int Id { get; set; }
        public string SupplierOrderNumber { get; set; }
        public DateTime Date { get; set; }
    }
}
