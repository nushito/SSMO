using SIA.Data.Enums;
using SSMO.Data.Enums;
using SSMO.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Data.Models
{
   public class Product
    {
        public Product()
        {
           
            CustomerOrders = new HashSet<CustomerOrder>();
           
        }
        
        public int Id { get; init; }
        [Required]
      //  public DescriptionEnum Description { get; set; }
        public int DescriptionId { get; set; }
        [Required]
        public Description Description { get; set; }
        [Required]
        public Size Size { get; set; }
        [Required]
        public Grade Grade { get; set; }
        [Required]
        public decimal OrderedQuantity { get; set; }
        public decimal LoadedQuantityM3 { get; set; }
        public decimal QuantityM2 { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public int TotalSheets { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; }

        [Required]
       // public ICollection<ProductSpecification> ProductSpecifications { get; set; }
      
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        //public int CustomerOrderId { get; set; }
        //public CustomerOrder CustomerOrder { get; set; }

        public ICollection<CustomerOrder> CustomerOrders { get; set; }
        public ICollection<SupplierOrder> SupplierOrders { get; set; }
    }
}
