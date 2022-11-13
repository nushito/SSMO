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
        public DbSet<Currency> Currencys { get; set; }
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


        protected override void OnModelCreating(ModelBuilder builder)
        {

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
                 .HasPrecision(18, 4);

            builder.Entity<SupplierOrder>()
              .Property(a => a.NetWeight)
              .HasColumnType("decimal")
              .HasPrecision(18, 3);

            builder.Entity<SupplierOrder>()
              .Property(a => a.GrossWeight)
              .HasColumnType("decimal")
              .HasPrecision(18, 3);

            builder.Entity<SupplierOrder>()
                .HasOne(a => a.CustomerOrder)
                .WithMany(a => a.SupplierOrder)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SupplierOrder>()
                .HasOne(a => a.Status)
                .WithMany(a => a.SupplierOrders)
               .OnDelete(DeleteBehavior.Restrict);

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

            builder.Entity<Product>()
                .Property(a => a.QuantityM2)
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
                .Property(a => a.OrderedQuantity)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Product>()
                .Property(a => a.LoadedQuantityM3)
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
               .HasPrecision(18, 5);

            builder.Entity<Document>()
                .Property(a => a.DocumentType)
                .HasConversion(a => a.ToString(), a => (DocumentTypes)Enum.Parse(typeof(DocumentTypes), a));

            builder.Entity<Document>()
                .Property(a => a.Amount)
                .HasColumnType("decimal")
                .HasPrecision(18, 5);

            builder.Entity<Document>()
                .HasOne(s => s.SupplierOrder)
                .WithMany(d => d.Documents)
                .HasForeignKey(s => s.SupplierOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Document>()
               .HasOne(s => s.CustomerOrder)
               .WithMany(d => d.Documents)
               .HasForeignKey(s => s.CustomerOrderId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Document>()
               .HasOne(s => s.Supplier)
               .WithMany(d => d.Documents)
               .HasForeignKey(s => s.SupplierId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Supplier>()
                .HasOne(a => a.Address)
                .WithMany(a => a.Suppliers)
                .HasForeignKey(a => a.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Customer>()
                .HasOne(a => a.ClientAddress)
                .WithMany(a => a.Customers)
                .HasForeignKey(a => a.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

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

            base.OnModelCreating(builder);
        }

    }
}
