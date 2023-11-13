using System;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Reports.PrrobaCascadeDropDown
{
    public class CustomerOrderListViewBySupplier
    {
        public int OrderConfirmationNumber { get; set; }
        public string CustomerPoNumber { get; set; }
     
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string LoadingPlace { get; set; }
        public string DeliveryAddress { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public bool PaidAmountStatus { get; set; }
    }
}
