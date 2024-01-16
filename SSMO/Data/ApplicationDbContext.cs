using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SSMO.Data.Enums;
using SSMO.Data.Models;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Document = SSMO.Data.Models.Document;

namespace SSMO.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<BankDetails> BankDetails { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<SupplierOrder> SupplierOrders { get; set; }
        public DbSet<MyCompany> MyCompanies { get; set; }
        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Description> Descriptions { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<InvoiceProductDetails> InvoiceProductDetails { get; set; }
        public DbSet<CustomerOrderProductDetails> CustomerOrderProductDetails { get; set; }
        public DbSet<PurchaseProductDetails> PurchaseProductDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<FiscalAgent> FiscalAgents { get; set; }
        public DbSet<FscText> FscTexts { get; set; }
        public DbSet<TransportCompany> TransportCompanies { get; set; }
        public DbSet<Image> Images { get; set; }    
        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<Address>()
                .HasOne(a => a.MyCompany)
                .WithOne(a => a.Address)   
                .HasForeignKey<MyCompany>(a=>a.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Address>()
               .HasOne(a => a.Suppliers)
                .WithOne(a => a.Address)
                .HasForeignKey<Supplier>(a => a.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Address>()
                .HasOne(a => a.Customers)
                .WithOne(a => a.Address)
                .HasForeignKey<Customer>(a => a.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Address>()
                .HasOne(a => a.TransportCompany)
                .WithOne(c=>c.Address)
                .HasForeignKey<TransportCompany>(c=>c.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BankDetails>()
               .HasIndex(a => a.Iban)
               .IsUnique(true);

            builder.Entity<BankDetails>()
                .HasOne(a => a.Currency)
                .WithMany(a => a.BankDetails)
                .HasForeignKey(a => a.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CustomerOrder>()
                .HasOne(a => a.Customer)
                .WithMany(a => a.Orders)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CustomerOrder>()
               .Property(a => a.SubTotal)
               .HasColumnType("decimal")
               .HasPrecision(18, 5);

            builder.Entity<CustomerOrder>()
              .Property(a => a.Amount)
              .HasColumnType("decimal")
              .HasPrecision(18, 5);

            builder.Entity<CustomerOrder>()
              .Property(a => a.Balance)
              .HasColumnType("decimal")
              .HasPrecision(18, 5);

            builder.Entity<CustomerOrder>()
              .Property(a => a.PaidAvance)
              .HasColumnType("decimal")
              .HasPrecision(18, 5);

            builder.Entity<CustomerOrder>()
              .Property(a => a.TotalQuantity)
              .HasColumnType("decimal")
              .HasPrecision(18, 5);

            builder.Entity<CustomerOrder>()
              .Property(a => a.TotalAmount)
              .HasColumnType("decimal")
              .HasPrecision(18, 5);

            builder.Entity<CustomerOrder>()
              .Property(a => a.NetWeight)
              .HasColumnType("decimal")
              .HasPrecision(18, 3);

            builder.Entity<CustomerOrder>()
              .Property(a => a.GrossWeight)
              .HasColumnType("decimal")
              .HasPrecision(18, 3);

            builder.Entity<CustomerOrder>()
                .HasOne(a => a.MyCompany)
                .WithMany(a => a.Orders)
                .HasForeignKey(a => a.MyCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CustomerOrder>()
                .HasOne(a => a.Currency)
                .WithMany(a => a.CustomerOrders)
                .HasForeignKey(a => a.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CustomerOrder>()
                .HasOne(a => a.Status)
                .WithMany(a => a.CustomerOrders)
                .OnDelete(DeleteBehavior.Restrict);       

            builder.Entity<SupplierOrder>()
                .Property(a => a.TotalAmount)
                 .HasColumnType("decimal")
                 .HasPrecision(18, 2);

            builder.Entity<SupplierOrder>()
              .Property(a => a.VatAmount)
               .HasColumnType("decimal")
               .HasPrecision(18, 2);

            builder.Entity<SupplierOrder>()
              .Property(a => a.NetWeight)
              .HasColumnType("decimal")
              .HasPrecision(18, 3);

            builder.Entity<SupplierOrder>()
              .Property(a => a.GrossWeight)
              .HasColumnType("decimal")
              .HasPrecision(18, 3);

            builder.Entity<SupplierOrder>()
                .HasMany(a => a.CustomerOrders)
                .WithMany(a => a.SupplierOrders);              

            //builder.Entity<SupplierOrder>()
            //    .HasOne(a => a.Status)
            //    .WithMany(a => a.SupplierOrders)
            //   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SupplierOrder>()
                .HasOne(a => a.Currency)
                .WithMany(a => a.SupplierOrders)
                .HasForeignKey(a => a.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SupplierOrder>()
                .HasOne(a => a.MyCompany)
                .WithMany(a => a.SupplierOrders)
                .HasForeignKey(a => a.MyCompanyId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<SupplierOrder>()
                  .Property(a => a.TotalQuantity)
                  .HasColumnType("decimal")
                  .HasPrecision(18, 5);

            builder.Entity<SupplierOrder>()
                .Property(a => a.PaidAvance)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<SupplierOrder>()
                .Property(a => a.Balance)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<SupplierOrder>()
                .Property(a => a.Amount)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<ServiceOrder>()
                .Property(a => a.Cost)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<ServiceOrder>()
               .Property(a => a.AmountAfterVat)
               .HasColumnType("decimal")
               .HasPrecision(18, 2);

            builder.Entity<ServiceOrder>()
               .Property(a => a.Balance)
               .HasColumnType("decimal")
               .HasPrecision(18, 2);

            builder.Entity<ServiceOrder>()
                .HasOne(c=>c.Currency)
                .WithMany(s=>s.ServiceOrders)
                .HasForeignKey(c => c.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ServiceOrder>()
                .HasOne(c=>c.MyCompany)
                .WithMany(s=>s.ServiceOrders)
                .HasForeignKey(c => c.MyCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ServiceOrder>()  
                .HasOne(c=>c.Document)
                .WithMany(c=>c.ServiceOrders)
                .HasForeignKey(c => c.DocumentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ServiceOrder>()
                .HasOne(c => c.SupplierOrder)
                .WithMany(c => c.ServiceOrders)
                .HasForeignKey(c => c.SupplierOrderId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ServiceOrder>()
                 .HasOne(c => c.CustomerOrder)
                 .WithMany(c => c.ServiceOrders)
                 .HasForeignKey(c => c.CustomerOrderId)
                 .IsRequired(false)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ServiceOrder>()
                .HasOne(a=>a.TransportCompany) 
                .WithMany(s=>s.ServiceOrders)
                .HasForeignKey(a=>a.TransportCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ServiceOrder>()
                .HasOne(a => a.FiscalAgent)
                .WithMany(s => s.ServiceOrders)
                .HasForeignKey(f => f.FiscalAgentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .Property(a => a.QuantityLeftForPurchaseLoading)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Product>()
                .Property(a => a.Price)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Product>()
                .Property(a => a.PurchasePrice)
                .HasPrecision(18, 4);

            builder.Entity<Product>()
               .Property(a => a.PurchaseAmount)
               .HasPrecision(18, 4);

            builder.Entity<Product>()
                .Property(a => a.CostPrice)
                .HasPrecision(18, 4);

            builder.Entity<Product>()
               .Property(a => a.BgPrice)
               .HasPrecision(18, 4);

            builder.Entity<Product>()
              .Property(a => a.BgAmount)
              .HasPrecision(18, 4);

            builder.Entity<Product>()
                .Property(a => a.OrderedQuantity)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Product>()
                .Property(a => a.LoadedQuantity)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Product>()
                .Property(a => a.QuantityM3)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Product>()
                .Property(a=>a.QuantityAvailableForCustomerOrder)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Product>()
           .Property(a => a.SoldQuantity)
           .HasColumnType("decimal")
           .HasPrecision(18, 5);

            builder.Entity<Product>()
                .Property(a => a.Amount)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Product>()
                .HasOne(a => a.SupplierOrder)
                .WithMany(p => p.Products)
                .HasForeignKey(s => s.SupplierOrderId)
                .IsRequired(false);

            builder.Entity<Product>()
                .Property(p => p.Unit)
                .HasConversion(u => u.ToString(), u => (Unit)Enum.Parse(typeof(Unit), u));

            builder.Entity<Document>()
                .HasOne(p => p.Currency)
                .WithMany(p => p.Documents)
                .HasForeignKey(s => s.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

           
            builder.Entity<Document>()
                .HasOne(i => i.MyCompany)
                .WithMany(a => a.Documents)
                .HasForeignKey(key => key.MyCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Document>()
                .Property(a => a.PurchaseTransportCost)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Document>()
                .Property(a => a.ProcentComission)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Document>()
                .Property(a => a.PaidAvance)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Document>()
                .Property(a => a.OtherExpenses)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Document>()
               .Property(a => a.TotalQuantity)
               .HasColumnType("decimal")
               .HasPrecision(18, 5);

            builder.Entity<Document>()
               .Property(a => a.GrossWeight)
               .HasColumnType("decimal");

            builder.Entity<Document>()
               .Property(a => a.NetWeight)
               .HasColumnType("decimal");

            builder.Entity<Document>()
                .Property(a => a.FiscalAgentExpenses)
                .HasColumnType("decimal");

            builder.Entity<Document>()
                .Property(a => a.Factoring)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Document>()
                .Property(a => a.Duty)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Document>()
                .Property(a => a.DeliveryTrasnportCost)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Document>()
                .Property(a => a.CurrencyExchangeRateUsdToBGN)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Document>()
                .Property(a => a.CustomsExpenses)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Document>()
                .Property(a => a.BankExpenses)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Document>()
                .Property(a => a.Balance)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Document>()
               .Property(a => a.TotalAmount)
               .HasColumnType("decimal")
               .HasPrecision(18, 2);

            builder.Entity<Document>()
              .Property(a => a.CreditNoteTotalAmount)
              .HasColumnType("decimal")
              .HasPrecision(18, 2);

            builder.Entity<Document>()
              .Property(a => a.DebitNoteTotalAmount)
              .HasColumnType("decimal")
              .HasPrecision(18, 2);

            builder.Entity<Document>()
              .Property(a => a.VatAmount)
              .IsRequired(false)
              .HasColumnType("decimal")            
              .HasPrecision(18, 2);

            builder.Entity<Document>()
                .Property(a => a.DocumentType)
                .HasConversion(a => a.ToString(), a => (DocumentTypes)Enum.Parse(typeof(DocumentTypes), a));

            builder.Entity<Document>()
                .Property(a => a.Amount)
                .HasColumnType("decimal")
                .HasPrecision(18, 2);

            builder.Entity<Document>()
                .HasOne(s => s.SupplierOrder)
                .WithMany(d => d.Documents)
                .HasForeignKey(s => s.SupplierOrderId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
         
            builder.Entity<Document>()
               .HasOne(s => s.Supplier)
               .WithMany(d => d.Documents)
               .HasForeignKey(s => s.SupplierId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Document>()
                .HasOne(s => s.CostPriceCurrency)
                .WithMany(p => p.DocumentsNewCurrencyForCostPrice)
                .HasForeignKey(s => s.CostPriceCurrencyId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<Document> ()
            //    .HasOne(s => s.Header)
            //    .WithMany(d=>d.HeaderDocuments)
            //    .HasForeignKey(h=>h.HeaderId)
            //    .IsRequired(false)
            //    .OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<Document>()
            // .HasOne(s => s.Footer)
            // .WithMany(d => d.FooterDocuments)
            // .HasForeignKey(h => h.FooterId)
            // .IsRequired(false)
            // .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Description>()
                .HasMany(a => a.Products)
                .WithOne(a => a.Description)
                .HasForeignKey(a => a.DescriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Currency>()
                  .HasMany(a => a.BankDetails)
                  .WithOne(a => a.Currency)
                  .HasForeignKey(a => a.CurrencyId)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Currency>()
                .HasMany(a => a.CustomerOrders)
                .WithOne(a => a.Currency)
                .HasForeignKey(a => a.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Currency>()
                .HasMany(a=>a.Documents)
                .WithOne(a=>a.Currency)
                .HasForeignKey(a=>a.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);
           
            builder.Entity<CustomerOrderProductDetails>()
                .HasOne(a=>a.Product)
                .WithMany(a=>a.CustomerOrderProductDetails)
                .HasForeignKey(a=>a.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CustomerOrderProductDetails>()
                .HasOne(a=>a.CustomerOrder)
                .WithMany(a=>a.CustomerOrderProducts)
                .HasForeignKey(a=>a.CustomerOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CustomerOrderProductDetails>()
                .Property(a => a.Amount)
                .HasColumnType("decimal")
                .HasPrecision(18,4);

            builder.Entity<CustomerOrderProductDetails>()
                .Property(a => a.Quantity)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<CustomerOrderProductDetails>()
                .Property(a => a.SellPrice)
                .HasColumnType("decimal")
                .HasPrecision(18, 4);

            builder.Entity<CustomerOrderProductDetails>()
             .Property(a => a.AutstandingQuantity)
             .HasColumnType("decimal")
             .HasPrecision(18, 4);


            builder.Entity<PurchaseProductDetails>()
                .HasOne(a => a.Product)
                .WithMany(a => a.PurchaseProductDetails)
                .HasForeignKey(a => a.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            
            builder.Entity<PurchaseProductDetails>()
                .Property(a => a.Amount)
                .HasColumnType("decimal")
                .HasPrecision(18, 4);

            builder.Entity<PurchaseProductDetails>()
                .Property(a => a.CalculationCurrencyPrice)
                .HasColumnType("decimal")
                .IsRequired(false)
                .HasPrecision(18, 4);

            builder.Entity<PurchaseProductDetails>()
                .Property(a => a.Quantity)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<PurchaseProductDetails>()
                .Property(a => a.PurchasePrice)
                .HasColumnType("decimal")
                .HasPrecision(18, 4);

            builder.Entity<PurchaseProductDetails>()
               .Property(a => a.CostPrice)
               .HasColumnType("decimal")
               .HasPrecision(18, 4);

            builder.Entity<PurchaseProductDetails>()
              .Property(a => a.QuantityM3)
              .HasColumnType("decimal")
              .HasPrecision(18, 5);

            builder.Entity<PurchaseProductDetails>()
                .HasOne(a=>a.PurchaseInvoice)
                .WithMany(a => a.PurchaseProducts)
                .HasForeignKey(a=>a.PurchaseInvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PurchaseProductDetails>()
                .HasOne(s=>s.SupplierOrder)
                .WithMany(p=>p.PurchaseProductDetails)
                .HasForeignKey(s=>s.SupplierOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PurchaseProductDetails>()
                .HasOne(s => s.CostPriceCurrency)
                .WithMany(p=>p.PurchaseProducts)
                .HasForeignKey(s=>s.CostPriceCurrencyId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InvoiceProductDetails>()
                .Property(a=>a.SellPrice)
                .HasColumnType("decimal")
                .HasPrecision (18, 5);

            builder.Entity<InvoiceProductDetails>()
                .Property(a => a.Amount)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<InvoiceProductDetails>()
              .Property(a => a.BgAmount)
              .HasColumnType("decimal")
              .HasPrecision(18, 5);

            builder.Entity<InvoiceProductDetails>()
                .Property(a=>a.InvoicedQuantity)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<InvoiceProductDetails>()
               .Property(a => a.QuantityM3ForCalc)
               .HasColumnType("decimal")
               .HasPrecision(18, 5);

            builder.Entity<InvoiceProductDetails>()
                .Property(a => a.DeliveryCost)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<InvoiceProductDetails>()
               .Property(a => a.BgPrice)
               .HasColumnType("decimal")
               .HasPrecision(18, 5);

            builder.Entity<InvoiceProductDetails>()
                .Property(a => a.Profit)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<InvoiceProductDetails>()
                .HasOne(a => a.Invoice)
                .WithMany(a => a.InvoiceProducts)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InvoiceProductDetails>()
             .HasOne(a => a.PurchaseProductDetails)
             .WithMany(a => a.InvoiceProductDetails)
             .HasForeignKey(a=>a.PurchaseProductDetailsId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InvoiceProductDetails>()
             .HasOne(a => a.CustomerOrderProductDetails)
             .WithMany(a => a.InvoiceProductDetails)
             .HasForeignKey(a => a.CustomerOrderProductDetailsId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InvoiceProductDetails>()
               .Property(a => a.DebitNoteQuantity)
               .HasColumnType("decimal")
               .HasPrecision(18, 5);

            builder.Entity<InvoiceProductDetails>()
               .Property(a => a.DebitNoteAmount)
               .HasPrecision(18, 4);

            builder.Entity<InvoiceProductDetails>()
              .Property(a => a.DebitNotePrice)
              .HasColumnType("decimal")
              .HasPrecision(18, 5);

            builder.Entity<InvoiceProductDetails>()
                .Property(a => a.CreditNoteQuantity)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<InvoiceProductDetails>()
               .Property(a => a.CreditNotePrice)
               .HasColumnType("decimal")
               .HasPrecision(18, 5);

            builder.Entity<InvoiceProductDetails>()
              .Property(a => a.CreditNoteProductAmount)
              .HasPrecision(18, 4);

            builder.Entity<InvoiceProductDetails>()
               .Property(a => a.CreditNoteBgPrice)
               .HasPrecision(18, 4);

            builder.Entity<InvoiceProductDetails>()
             .Property(a => a.CreditNoteBgAmount)
             .HasPrecision(18, 4);

            builder.Entity<InvoiceProductDetails>()
               .Property(a => a.DebitNoteBgPrice)
               .HasPrecision(18, 4);

            builder.Entity<InvoiceProductDetails>()
             .Property(a => a.DebitNoteBgAmount)
             .HasPrecision(18, 4);

            builder.Entity<InvoiceProductDetails>()
                .HasOne(p=>p.Product)
                .WithMany(i=>i.InvoiceProductDetails)
                .HasForeignKey(p=>p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InvoiceProductDetails>()
                .HasOne(c => c.CustomerOrder)
                .WithMany(i => i.InvoiceProductDetails)
                .HasForeignKey(f => f.CustomerOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InvoiceProductDetails> ()
                .HasOne(p=>p.CreditNote)
                .WithMany(a=>a.CreditNoteProducts)
                .HasForeignKey(a=>a.CreditNoteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InvoiceProductDetails>()
              .HasOne(p => p.DebitNote)
              .WithMany(a => a.DebitNoteProducts)
              .HasForeignKey(a => a.DebitNoteId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
                .HasOne(s=>s.SupplierOrder)
                .WithMany(p=>p.Payments)
                .HasForeignKey(k=>k.SupplierOrderId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.Entity<Payment>()
          .HasOne(s => s.Document)
          .WithMany(p => p.Payments)
          .HasForeignKey(k => k.DocumentId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired(false);

            builder.Entity<Payment>()
          .HasOne(s => s.CustomerOrder)
          .WithMany(p => p.Payments)
          .HasForeignKey(k => k.CustomerOrderId)
          .OnDelete(DeleteBehavior.Restrict)         
          .IsRequired(false);

            builder.Entity<Payment>()
                .HasOne(s => s.ServiceOrder)
                .WithMany(p => p.Payments)
                .HasForeignKey(k => k.ServiceOrderId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.Entity<Payment>()
                .HasOne(a => a.Currency)
                .WithMany(a => a.Payments)
                .HasForeignKey(k => k.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
                .HasOne(a => a.CurrencyForCalculations)
                .WithMany(a => a.PaymentsWithExchangeRate)
                .HasForeignKey(k => k.CurrencyForCalculationsId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
               .Property(a => a.PaidAmount)
               .HasColumnType("decimal")
               .HasPrecision(18, 2);

            builder.Entity<Payment>()
               .Property(a => a.UsedAmountForCalculation)
               .HasColumnType("decimal")
               .HasPrecision(18, 2);

            builder.Entity<Payment>()
               .Property(a => a.NewAmountPerExchangeRate)
               .HasColumnType("decimal")
               .IsRequired(false)
               .HasPrecision(18, 6);

            builder.Entity<Payment>()
               .Property(a => a.CurruncyRateExchange)
               .HasColumnType("decimal")
               .IsRequired(false)
               .HasPrecision(18, 6);

            builder.Entity<FiscalAgent>()
                .HasMany(d => d.Documents)
                .WithOne(f => f.Fiscalagent)
                .HasForeignKey(a => a.FiscalAgentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FiscalAgent>()
                .HasMany(d => d.CustomerOrders)
                .WithOne(f => f.Fiscalagent)
                .HasForeignKey(a => a.FiscalAgentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FscText>()
                .HasMany(d => d.Documents)
                .WithOne(d => d.FscText)
                .HasForeignKey(d => d.FscTextId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FscText>()
            .HasMany(d => d.CustomerOrders)
            .WithOne(d => d.FscText)
            .HasForeignKey(d => d.FscTextId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Image>()
                .HasOne(m => m.MyCompanyName)
                .WithMany(s => s.Images)
                .HasForeignKey(k => k.MyCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Image>()
                .HasMany(d=>d.FooterDocuments)
                .WithOne(i=>i.Footer)
                .HasForeignKey(i=> i.FooterId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Image>()
                .HasMany(d => d.HeaderDocuments)
                .WithOne(i => i.Header)
                .HasForeignKey(i => i.HeaderId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }

    }
}
