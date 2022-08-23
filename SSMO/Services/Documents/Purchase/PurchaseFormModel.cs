using SSMO.Services.Supplier;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Services.Documents.Purchase
{
    public class PurchaseFormModel
    {
        public string Number { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public int SupplierId { get; set; }
        public string Supplier { get; set; }
        public IEnumerable<AllSuppliers> Suppliers { get; set; }
    }
}
