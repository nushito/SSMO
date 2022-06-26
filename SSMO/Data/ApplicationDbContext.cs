using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
                 .HasOne(a => a.SupplierOrder)
                 .WithOne(a => a.CustomerOrder)
                 .HasForeignKey<SupplierOrder>(a => a.CustomerOrderId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CustomerOrder>()
               .Property(a => a.SubTotal)
               .HasColumnType("decimal");

            builder.Entity<CustomerOrder>()
                .Property(a => a.Total)
                .HasColumnType("decimal");

            builder.Entity<CustomerOrder>()
              .Property(a => a.Amount)
              .HasColumnType("decimal");
            builder.Entity<CustomerOrder>()
              .Property(a => a.Balance)
              .HasColumnType("decimal");
            builder.Entity<CustomerOrder>()
              .Property(a => a.PaidAvance)
              .HasColumnType("decimal");
            builder.Entity<CustomerOrder>()
              .Property(a => a.TotalQuantity)
              .HasColumnType("decimal");
            builder.Entity<CustomerOrder>()
              .Property(a => a.TotalAmount)
              .HasColumnType("decimal");

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
                .WithMany(a=>a.CustomerOrders)
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
                  .HasColumnType("decimal");

            builder.Entity<SupplierOrder>()
                .Property(a => a.PaidAvance)
                .HasColumnType("decimal");

            builder.Entity<SupplierOrder>()
                .Property(a => a.Balance)
                .HasColumnType("decimal");

            builder.Entity<SupplierOrder>()
                .Property(a => a.Amount)
                .HasColumnType("decimal");

            builder.Entity<ServiceOrder>()
                .Property(a => a.Cost)
                .HasColumnType("decimal");

            builder.Entity<SupplierOrder>()
                .HasOne(a => a.CustomerOrder)
                .WithOne(a => a.SupplierOrder)
                .HasForeignKey<CustomerOrder>(a => a.SupplierOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SupplierOrder>()
                .Property(a => a.Status)
                .HasConversion<string>();

            builder.Entity<Product>()
                .HasOne(a => a.Supplier)
                .WithMany(a => a.Products)
                .HasForeignKey(a => a.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                   .HasOne(a => a.Description)
                   .WithMany(a => a.Products)
                   .HasForeignKey(a => a.DescriptionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .Property(a => a.QuantityM2)
                .HasColumnType("decimal");

            builder.Entity<Product>()
                .Property(a => a.Price)
                .HasColumnType("decimal");

            builder.Entity<Product>()
                .Property(a => a.OrderedQuantity)
                .HasColumnType("decimal");

            builder.Entity<Product>()
                .Property(a => a.LoadedQuantityM3)
                .HasColumnType("decimal");

            builder.Entity<Product>()
                .Property(a => a.Amount)
                .HasColumnType("decimal");

            builder.Entity<Document>()
                .HasKey(a => a.Id);


            builder.Entity<Document>()
                .Property(a => a.PurchaseTransportCost)
                .HasColumnType("decimal");

            builder.Entity<Document>()
                .Property(a => a.ProcentComission)
                .HasColumnType("decimal");

            builder.Entity<Document>()
                .Property(a => a.PaidAvance)
                .HasColumnType("decimal");

            builder.Entity<Document>()
                .Property(a => a.OtherExpenses)
                .HasColumnType("decimal");

            builder.Entity<Document>()
                .Property(a => a.NetWeight)
                .HasColumnType("decimal");

            builder.Entity<Document>()
                 .Property(a => a.GrossWeight)
                 .HasColumnType("decimal");

            builder.Entity<Document>()
                .Property(a => a.FiscalAgentExpenses)
                .HasColumnType("decimal");

            builder.Entity<Document>()
                .Property(a => a.Factoring)
                .HasColumnType("decimal");
            builder.Entity<Document>()
                .Property(a => a.Duty)
                .HasColumnType("decimal");
            builder.Entity<Document>()
                .Property(a => a.DeliveryTrasnportCost)
                .HasColumnType("decimal");
            builder.Entity<Document>()
                .Property(a => a.CurrencyExchangeRateUsdToBGN)
                .HasColumnType("decimal");
            builder.Entity<Document>()
                .Property(a => a.CustomsExpenses)
                .HasColumnType("decimal");
            builder.Entity<Document>()
                .Property(a => a.BankExpenses)
                .HasColumnType("decimal");
            builder.Entity<Document>()
                .Property(a => a.Balance)
                .HasColumnType("decimal");

            builder.Entity<Document>()
                .Property(a => a.DocumentType)
                .HasConversion<string>();


            builder.Entity<Supplier>()
                .HasMany(a => a.Products)
                .WithOne(a => a.Supplier)
                .HasForeignKey(a => a.SupplierId)
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
