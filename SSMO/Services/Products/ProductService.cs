﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Presentation;
using Humanizer.DateTimeHumanizeStrategy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSMO.Data;
using SSMO.Data.Enums;
using SSMO.Data.Models;
using SSMO.Models.Descriptions;
using SSMO.Models.Documents.CreditNote;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Grades;
using SSMO.Models.Products;
using SSMO.Models.Reports.CreditNote;
using SSMO.Models.Reports.DebitNote;
using SSMO.Models.Reports.FSC;
using SSMO.Models.Reports.ProductsStock;
using SSMO.Models.Reports.SupplierOrderReportForEdit;
using SSMO.Models.Sizes;
using SSMO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Description = SSMO.Data.Models.Description;
using Size = SSMO.Data.Models.Size;

namespace SSMO.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConfigurationProvider mapper;
        private readonly IProductRepository productRepository;

        public ProductService(ApplicationDbContext dbContext, IMapper mapper, IProductRepository productRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper.ConfigurationProvider;
            this.productRepository = productRepository;
        }
        public void CreateProduct(ProductSupplierFormModel model, int supplierOrderId)
        {
            var description = dbContext.Descriptions.Where(a => a.Id == model.DescriptionId).Select(n=>n.Name).FirstOrDefault();
            var size = dbContext.Sizes.Where(a => a.Id == model.SizeId).Select(n=>n.Name).FirstOrDefault();
            var grade = dbContext.Grades.Where(a => a.Id == model.GradeId).Select(n=>n.Name).FirstOrDefault();

            if (String.IsNullOrEmpty(model.PurchaseFscCertificate))
            {
                model.PurchaseFscCertificate = null;
            }

            var product = new Product
            {
                DescriptionId = model.DescriptionId,
                GradeId = model.GradeId,
                SizeId = model.SizeId,
                PurchaseFscClaim = model.FscClaim,
                PurchaseFscCertificate = model.PurchaseFscCertificate,
                PurchasePrice = model.PurchasePrice,
                Pallets = model.Pallets,
                SheetsPerPallet = model.SheetsPerPallet,
                SupplierOrderId = supplierOrderId,               
                Unit = Enum.Parse<Unit>(model.Unit),
                TotalSheets = model.Pallets*model.SheetsPerPallet,
                CustomerOrderProductDetails = new List<CustomerOrderProductDetails>(),
                InvoiceProductDetails = new List<InvoiceProductDetails>(),
                PurchaseProductDetails = new List<PurchaseProductDetails>()
            };                        

            if(model.Quantity == 0)
            {
                var sum = ConvertStringSizeToQubicMeters(size);
               
               product.OrderedQuantity = Math.Round(sum * model.Pallets * model.SheetsPerPallet, 5);
                
            }
            else
            {
                product.OrderedQuantity = model.Quantity;
            }

            product.QuantityAvailableForCustomerOrder = product.OrderedQuantity;
            product.QuantityLeftForPurchaseLoading = product.OrderedQuantity;
            product.PurchaseAmount = Math.Round(model.PurchasePrice * product.OrderedQuantity, 5);
           
            var order = dbContext.SupplierOrders.Where(a => a.Id == supplierOrderId).FirstOrDefault();           
            order.Amount += product.PurchaseAmount;
            order.Products.Add(product);

            dbContext.SaveChanges();
        }
        public void AddDescription(string name, string bgName)
        {
            dbContext.Descriptions.Add(new Description { Name = name, BgName = bgName });
            dbContext.SaveChanges();
            return;
        }
        public void AddGrade(string name)
        {
            dbContext.Grades.Add(new Grade { Name = name });
            dbContext.SaveChanges();
            return;
        }
        public void AddSize(string name)
        {
            dbContext.Sizes.Add(new Size { Name = name });
            dbContext.SaveChanges();
            return;
        }
        public bool DescriptionExist(string name)
        {
            var check = dbContext.Descriptions.Where(a => a.Name.ToLower() == name.ToLower()).FirstOrDefault();

            if (check == null)
            {
                return false;
            }

            return true;
        }
        public bool GradeExist(string name)
        {
            var check = dbContext.Grades.Where(a => a.Name.ToLower() == name.ToLower()).FirstOrDefault();

            if (check == null)
            {
                return false;
            }

            return true;
        }
        public bool SizeExist(string name)
        {
            var check = dbContext.Sizes.Where(a => a.Name.ToLower() == name.ToLower()).FirstOrDefault();

            if (check == null)
            {
                return false;
            }

            return true;
        }
        public IEnumerable<string> GetDescriptions()
        {
            return dbContext.Descriptions.Select(a => a.Name).ToList();
        }
        public IEnumerable<string> GetSizes()
        {
            return dbContext.Sizes.Select(a => a.Name).ToList();
        }
        public IEnumerable<string> GetGrades()
        {
            return dbContext.Grades.Select(a => a.Name).ToList();
        }
        public bool CreateCustomerOrderProduct(int id, int customerorderId,
            int supplierOrderId, string description, string grade,
            string size, string fscCert, string fscClaim,
            int pallets, int sheetsPerPallet, decimal price, decimal orderedQuantity, string unit)
        {
            var product = dbContext.Products.Find(id);
           
            if (product == null || product.QuantityAvailableForCustomerOrder < orderedQuantity || product.QuantityAvailableForCustomerOrder <= 0)
            {
                return false;
            }

            product.CustomerOrderProductDetails= new List<CustomerOrderProductDetails>();

            var customerOrderProduct = new CustomerOrderProductDetails()
            {
            ProductId = product.Id,
            CustomerOrderId= customerorderId,
            FscClaim = fscClaim,
            FscCertificate = fscCert,
            Pallets = pallets,
            SheetsPerPallet = sheetsPerPallet,
            Quantity= orderedQuantity,
            SellPrice = price,
            SupplierOrderId = supplierOrderId,
            Unit = Enum.Parse<Unit>(unit),
            TotalSheets = product.Pallets * product.SheetsPerPallet,
            Amount = Math.Round(price * orderedQuantity, 4),
            AutstandingQuantity= orderedQuantity
            };

            product.CustomerOrderProductDetails.Add(customerOrderProduct);
            
            product.QuantityAvailableForCustomerOrder -= customerOrderProduct.Quantity;
            
            var customerOrder = dbContext.CustomerOrders.Where(i => i.Id == customerorderId).FirstOrDefault();
            // customerOrder.CustomerOrderProducts = new List<CustomerOrderProductDetails>();  

            customerOrder.Amount += customerOrderProduct.Amount;
            // customerOrder.CustomerOrderProducts.Add(customerOrderProduct);

            var purchaseProductId = dbContext.PurchaseProductDetails
                .Where(i => i.SupplierOrderId == supplierOrderId && i.ProductId == product.Id)
                .Select(i => i.Id)
                .FirstOrDefault();

            var purchase  = dbContext.Documents
                .Where(s=>s.PurchaseProducts.Select(i=>i.Id).Contains(purchaseProductId))
                .FirstOrDefault();

            if(purchase != null && !purchase.CustomerOrderProducts.Contains(customerOrderProduct))
            {
              purchase.CustomerOrderProducts.Add(customerOrderProduct);
            }
           
            dbContext.SaveChanges();

            return true;
        }

        public IEnumerable<ProductPerSupplierOrderDetails> Details(List<int> supplierOrderserId)
        {
            var products = dbContext.Products
                .Where(a => supplierOrderserId.Contains(a.SupplierOrderId ?? 0) && a.QuantityAvailableForCustomerOrder > 0.0001m)
                .ProjectTo<ProductPerSupplierOrderDetails>(mapper)
                .ToList();

            foreach (var item in products)
            { 
                item.Description = GetDescriptionName(item.DescriptionId);
                item.Grade = GetGradeName(item.GradeId);
                item.Size = GetSizeName(item.SizeId);

                if(item.QuantityAvailableForCustomerOrder < item.OrderedQuantity)
                {
                    var pallets = dbContext.CustomerOrderProductDetails
                        .Where(i => i.ProductId == item.Id)
                        .Select(p => p.Pallets)
                        .FirstOrDefault();

                    item.Pallets -= pallets;
                }
            }

            return products;
        }

        public ICollection<ProductCustomerFormModel> DetailsPerCustomerOrder(int customerId)
        {
            var products = dbContext.CustomerOrderProductDetails
                .Where(a => a.CustomerOrderId == customerId)
                .ProjectTo<ProductCustomerFormModel>(mapper)
                .ToList();

            foreach (var product in products)
            {
                var mainProduct = dbContext.Products
                    .Where(i => i.Id == product.ProductId)
                    .FirstOrDefault();

                product.Description = GetDescriptionName(mainProduct.DescriptionId);
                product.Grade = GetGradeName(mainProduct.GradeId);
                product.Size = GetSizeName(mainProduct.SizeId);

            }
            return products;
        }

        public ICollection<string> GetFascCertMyCompany()
        {
            var fscCert = dbContext.MyCompanies.Select(f => f.FSCSertificate).ToList();
            return fscCert;
        }
        public string GetDescriptionName(int id)
        {
            var name = dbContext.Descriptions
               .Where(a => a.Id == id)
               .Select(a => a.Name)
               .FirstOrDefault();
            return name;
        }
        public string GetGradeName(int id)
        {
            var name = dbContext.Grades
                .Where(a => a.Id == id)
                .Select(a => a.Name)
                .FirstOrDefault();
            return name;
        }
        public string GetSizeName(int id)
        {
            var name = dbContext.Sizes
                .Where(a => a.Id == id)
                .Select(a => a.Name)
                .FirstOrDefault();
            return name;
        }
        public decimal CalculateDeliveryCostOfTheProductInCo
            (decimal quantity, decimal quantityM3, decimal totalQuantity, decimal deliveryCost, Unit unit, string size)
        {
            var cost = deliveryCost / totalQuantity * quantityM3;

            var dimensionArray = size.Split('/').ToArray();
            decimal sum = 1M;
            decimal thickness = Math.Round(decimal.Parse(dimensionArray[0]) / 1000, 5);
            decimal calcMeter = Math.Round(decimal.Parse(dimensionArray[0]) / 1000, 5) * 
                Math.Round(decimal.Parse(dimensionArray[2]) / 1000, 5);

            for (int i = 0; i < dimensionArray.Count(); i++)
            {
                sum *= Math.Round(decimal.Parse(dimensionArray[i]) / 1000, 5);
            }

            if (unit == Data.Enums.Unit.m2)
            {
                cost *= thickness;
            }
            else if (unit == Data.Enums.Unit.pcs || unit == Data.Enums.Unit.sheets)
            {
                cost *= sum;
            }
            else if(unit == Data.Enums.Unit.m)
            {
                cost *= calcMeter;
            }

            return cost;
        }
        public ICollection<ProductsForEditSupplierOrder> ProductsDetailsPerSupplierOrder(int supplierOrderId)
        {
            
            var products = dbContext.Products
                 .Where(a => a.SupplierOrderId == supplierOrderId)
                 .ProjectTo<ProductsForEditSupplierOrder>(mapper)
                 .ToList();

            foreach (var item in products)
            {
                item.Description = GetDescriptionName(item.DescriptionId);
                item.Grade = GetGradeName(item.GradeId);
                item.Size = GetSizeName(item.SizeId);              
            }
            return products;       
        }
        public void ClearProductQuantityWhenDealIsFinished
            (int productId, decimal quantity, decimal oldQuantity)
        {
            var product = dbContext.Products
                .Where(id => id.Id == productId)
                .FirstOrDefault();

            if (product != null)
            {
                product.QuantityAvailableForCustomerOrder += quantity - oldQuantity;
            }

            dbContext.SaveChanges();
        }
        public ProductsAvailabilityCollectionViewModel ProductsOnStock
            (int? descriptionId, int? gradeId, int? sizeId, int currentPage = 1, int productsPerPage = int.MaxValue)
        {
            var products = dbContext.Products
               .Where(d => d.LoadedQuantityM3 != 0 && d.SoldQuantity < d.OrderedQuantity);
               

            if (descriptionId != null)
            {
                products = products
                         .Where(d => d.DescriptionId == descriptionId);
            }

            if (gradeId != null)
            {
                products = products
                         .Where(d => d.GradeId == gradeId);
            }

            if ( sizeId != null)
            {
                products = products
                         .Where(d => d.SizeId == sizeId);

            }

            var productsOnStok = new List<ProductAvailabilityDetailsViewModel>();          

            foreach (var product in products.ToList())
            {
                var statusId = dbContext.Statuses
                    .Where(n=>n.Name == "Active")
                    .Select(id=>id.Id)
                    .FirstOrDefault();

                var supplierOrder = dbContext.SupplierOrders
                       .Where(s => s.Id == product.SupplierOrderId)
                       .FirstOrDefault();

                var supplierName = dbContext.Suppliers
                    .Where(i=>i.Id == supplierOrder.SupplierId)
                    .Select(n=>n.Name)
                    .FirstOrDefault();

                var availableProduct = new ProductAvailabilityDetailsViewModel
                {
                    Description = GetDescriptionName(product.DescriptionId),
                    Size = GetSizeName(product.SizeId),
                    Grade = GetGradeName(product.GradeId),
                    LoadedQuantity = product.LoadedQuantityM3,
                    QuantityOnStock = product.LoadedQuantityM3 - product.SoldQuantity,        
                    SupplierName = supplierName,
                    CustomerProductsDetails = new List<ProductDetailsForEachCustomerOrderViewModel>(),
                    PurchaseProductDetails = new List<PurchaseProductDetailsListViewModel>()
                };
               
                var purchaseProduct = dbContext.PurchaseProductDetails
                      .Where(p => p.ProductId == product.Id                       
                       && p.SupplierOrderId == supplierOrder.Id)
                      .ToList();
               
                var customerProducts = dbContext.CustomerOrderProductDetails
                      .Where(pr => pr.SupplierOrderId == supplierOrder.Id && 
                      pr.ProductId == product.Id)
                      .ToList();               

                foreach (var purchaseDetail in purchaseProduct)
                {                   
                    var purchase = dbContext.Documents
                        .Where(i => i.Id == purchaseDetail.PurchaseInvoiceId)
                        .FirstOrDefault();
                   
                    var productDetail = new PurchaseProductDetailsListViewModel
                    {
                        CostPrice = purchaseDetail.CostPrice,                       
                        FSCClaim = purchaseDetail.FscClaim,
                        FSCSertificate = purchaseDetail.FscCertificate,
                        DeliveryAddress = purchase.DeliveryAddress,                     
                        PurchasePrice = product.PurchasePrice,
                        Pallets = product.Pallets,
                        SheetsPerPallet = product.SheetsPerPallet,                       
                        PurchaseDate = purchase.Date,
                        PurchaseNumber = purchase.PurchaseNumber,                       
                        Unit = product.Unit.ToString(),
                        LoadedQuantity = purchaseDetail.Quantity
                    };
                    
                    availableProduct.PurchaseProductDetails.Add(productDetail);
                }

                foreach (var order in customerProducts)
                {
                    var customerOrder = dbContext.CustomerOrders
                    .Where(i => i.Id == order.CustomerOrderId)
                    .FirstOrDefault();

                    var customerName = dbContext.Customers
                      .Where(o => o.Id == customerOrder.CustomerId)
                      .Select(n => n.Name)
                      .FirstOrDefault();

                    var customerDetail = new ProductDetailsForEachCustomerOrderViewModel
                    { CustomerName= customerName,
                     CustomerOrderNumber = customerOrder.OrderConfirmationNumber,
                     CustomerPoNumber = customerOrder.CustomerPoNumber,
                     DeliveryAddress= customerOrder.DeliveryAddress,
                      Price= order.SellPrice
                    };

                    availableProduct.CustomerProductsDetails.Add(customerDetail);
                }
                productsOnStok.Add(availableProduct);
            }

            var productCollection = productsOnStok.Skip((currentPage - 1) * productsPerPage).Take(productsPerPage);

            var productListAndCount = new ProductsAvailabilityCollectionViewModel
            {
                 Products= productCollection,
                 TotalProducts = productsOnStok.Count()
            };

            return productListAndCount;
        }
        public ICollection<DescriptionForProductSearchModel> DescriptionIdAndNameList()
        {
            return dbContext.Descriptions
                .Select(n=> new DescriptionForProductSearchModel
                {
                    Id = n.Id,
                    Name = n.Name
                }).ToList();
        }
        public ICollection<SizeForProductSearchModel> SizeIdAndNameList()
        {
            return dbContext.Sizes
               .Select(n => new SizeForProductSearchModel
               {
                   Id = n.Id,
                   Name = n.Name
               }).ToList();
        }
        public ICollection<GradeForProductSearchModel> GradeIdAndNameList()
        {
            return dbContext.Grades
                .Select(n => new GradeForProductSearchModel
                {
                    Id = n.Id,
                    Name = n.Name
                }).ToList();
        }
        public void ResetToNullLoadingQuantityIfPurchaseIsChanged(int productId)
        {
            var product = dbContext.Products
                .Where(i => i.Id == productId) 
                .FirstOrDefault();

            product.LoadedQuantityM3 = 0;   
        }
        public void NewLoadingQuantityToEditPurchase(int productId, int purchaseId)
        {
            var product = dbContext.Products
                .Where(i => i.Id == productId).FirstOrDefault();

            product.LoadedQuantityM3 = product.OrderedQuantity;
            dbContext.SaveChanges();
        }
        public ICollection<string> GetUnits()
        {
           return Enum.GetNames(typeof(Unit));           
        }
        public ICollection<string> FscClaimList()
        {

           return dbContext.PurchaseProductDetails
                .Select(f=>f.FscClaim)
                .Distinct()
                .ToList();
        }
        public List<ProductsForInvoiceViewModel> ProductsForInvoice(List<int> customerOrders)
        {
           var customerOrdersIdList = dbContext.CustomerOrders
                .Where(i=> customerOrders.Contains(i.Id))
                .Select(i=>i.Id) 
                .ToList();

            var products = dbContext.CustomerOrderProductDetails
                .Where(i => customerOrdersIdList.Contains(i.CustomerOrderId) && i.AutstandingQuantity > 0)
                .ToList();

            var invoicedProducts = new List<ProductsForInvoiceViewModel>();

            foreach (var product in products)
            {
                var mainProduct = dbContext.Products
                    .Where(i=>i.Id == product.ProductId).FirstOrDefault();
                //TODO dali invoicedquantity da e = loadedquantity?
                if (mainProduct.LoadedQuantityM3 > 0)
                {
                    var productForInvoice = new ProductsForInvoiceViewModel
                    {
                        Id = product.Id,
                        CustomerOrderId = product.CustomerOrderId,
                        ProductId = product.ProductId,
                        Unit = product.Unit,
                        Pallets = product.Pallets,
                        SheetsPerPallet = product.SheetsPerPallet,
                        InvoicedQuantity = mainProduct.LoadedQuantityM3 - mainProduct.SoldQuantity,
                        SellPrice = product.SellPrice,
                        FscCertificate = product.FscCertificate,
                        FscClaim = product.FscClaim,
                        PurchaseCostPrice = new List<PurchaseProductCostPriceViewModel>() ,
                        Descriptions = new List<DescriptionForInvoiceViewModel>(),
                        Sizes = new List<SizeForInvoiceViewModel>(),
                        Grades = new List<GradeForInvoiceViewModel>()
                    };

                    var descriptionId = dbContext.Products
                   .Where(i => i.Id == mainProduct.Id)
                   .Select(d => d.DescriptionId).FirstOrDefault();

                    var gradeId = dbContext.Products
                        .Where(i => i.Id == mainProduct.Id)
                        .Select(d => d.GradeId).FirstOrDefault();

                    var sizeId = dbContext.Products
                        .Where(i => i.Id == mainProduct.Id)
                        .Select(d => d.SizeId).FirstOrDefault();


                    productForInvoice.Description = GetDescriptionName(descriptionId);
                    productForInvoice.Grade = GetGradeName(gradeId);
                    productForInvoice.Size = GetSizeName(sizeId);
                    productForInvoice.DescriptionId = descriptionId;
                    productForInvoice.GradeId = gradeId;
                    productForInvoice.SizeId = sizeId;

                    var descriptions = new DescriptionForInvoiceViewModel
                    {
                        Id = descriptionId,
                        Name = GetDescriptionName(descriptionId)
                    };

                    var grades = new GradeForInvoiceViewModel
                    {
                        Id = gradeId,
                        Name = GetGradeName(gradeId)
                    };

                    var sizes = new SizeForInvoiceViewModel
                    {
                        Id = sizeId,
                        Name = GetSizeName(sizeId)
                    };

                    productForInvoice.Descriptions.Add(descriptions);
                    productForInvoice.Grades.Add(grades);
                    productForInvoice.Sizes.Add(sizes);

                    var purchaseProductCostPrice = dbContext.PurchaseProductDetails
                        .Where(pi => pi.ProductId == mainProduct.Id)
                        .Select(c => new PurchaseProductCostPriceViewModel
                        {
                            PurchaseCostPriceId = c.Id,
                            CostPrice = c.CostPrice,
                        }).ToList();

                    productForInvoice.PurchaseCostPrice = purchaseProductCostPrice;

                    invoicedProducts.Add(productForInvoice);
                }
            }
            return invoicedProducts;           
        }
        public void CreateNewProductOnEditSupplierOrder(NewProductsForSupplierOrderModel modelProduct)
        {
            if (modelProduct == null) return;

            var product = new Product
            {
                DescriptionId = modelProduct.DescriptionId,
                GradeId = modelProduct.GradeId,
                SizeId = modelProduct.SizeId,
                PurchaseFscClaim = modelProduct.FscClaim,
                PurchaseFscCertificate = modelProduct.FscCertificate,
                PurchasePrice = modelProduct.PurchasePrice,
                Pallets = modelProduct.Pallets,
                SheetsPerPallet = modelProduct.SheetsPerPallet,
                SupplierOrderId = modelProduct.SupplierOrderId,
                OrderedQuantity = modelProduct.Quantity,
                Unit = Enum.Parse<Unit>(modelProduct.Unit),
                TotalSheets = modelProduct.Pallets * modelProduct.SheetsPerPallet
            };
            
            product.QuantityAvailableForCustomerOrder = modelProduct.Quantity;
            product.QuantityLeftForPurchaseLoading = modelProduct.Quantity;
            product.PurchaseAmount = Math.Round(modelProduct.PurchasePrice * modelProduct.Quantity, 4);

           // dbContext.Products.Add(product);

            var order = dbContext.SupplierOrders.Where(a => a.Id == modelProduct.SupplierOrderId).FirstOrDefault();
            order.Amount += product.PurchaseAmount;
            order.Products.Add(product);

            dbContext.SaveChanges();
        }
        public bool AddNewProductsToEditedInvoice(int id, List<ProductsForInvoiceViewModel> products)
        {
            if(id == 0) return false;

            var invoice = dbContext.Documents
                .Where(i => i.Id == id)
                .FirstOrDefault();

            foreach (var product in products)
            {
                if(product.InvoicedQuantity == 0) continue;

                var mainProduct = dbContext.Products
                    .Where(co => co.Id == product.ProductId).FirstOrDefault();

                mainProduct.SoldQuantity += product.InvoicedQuantity;

                var customerOrderProduct = dbContext.CustomerOrderProductDetails
                    .Where(i => i.ProductId == product.ProductId && i.CustomerOrderId == product.CustomerOrderId)
                    .FirstOrDefault();

                var customerOrder = dbContext.CustomerOrders
                    .Where(i => i.Id == product.CustomerOrderId)
                    .FirstOrDefault();

                if(invoice.CustomerId != customerOrder.CustomerId)
                {
                    invoice.CustomerId= customerOrder.CustomerId;
                }

                customerOrderProduct.AutstandingQuantity -= product.InvoicedQuantity;

                var purchaseProductDetail = dbContext.PurchaseProductDetails
                    .Where(a => a.Id == product.PurchaseCostPriceId).FirstOrDefault();

                var supplierOrder = dbContext.SupplierOrders
                    .Where(a => a.Id == purchaseProductDetail.SupplierOrderId).FirstOrDefault();

                var invoiceProduct = new InvoiceProductDetails
                {
                    CustomerOrderId = product.CustomerOrderId,
                    Pallets = product.Pallets,
                    SheetsPerPallet = product.SheetsPerPallet,
                    FscCertificate = product.FscCertificate,
                    FscClaim = product.FscClaim,
                    ProductId = product.ProductId,
                    PurchaseProductDetailsId = purchaseProductDetail.Id,
                    SellPrice = product.SellPrice,
                    Unit = product.Unit,
                    VehicleNumber = product.VehicleNumber,
                    InvoicedQuantity = product.InvoicedQuantity,
                    BgPrice = product.SellPrice * invoice.CurrencyExchangeRateUsdToBGN,
                    CustomerOrderProductDetailsId = customerOrderProduct.Id,

                };

                invoiceProduct.TotalSheets = product.Pallets * product.SheetsPerPallet;
                invoiceProduct.Amount = invoiceProduct.SellPrice * invoiceProduct.InvoicedQuantity;
                invoiceProduct.BgAmount = invoiceProduct.BgPrice * invoiceProduct.InvoicedQuantity;
               
                invoice.SupplierOrderId = supplierOrder.Id;
                invoice.InvoiceProducts.Add(invoiceProduct);
                invoiceProduct.CustomerOrderProductDetailsId = customerOrderProduct.Id;

                dbContext.SaveChanges();
            }

            foreach (var product in invoice.InvoiceProducts)
            {
                var purchaseProductDetailCostPrice = dbContext.PurchaseProductDetails
                    .Where(a => a.Id == product.PurchaseProductDetailsId)
                    .Select(p=>p.CostPrice)
                    .FirstOrDefault();

                var mainProduct = dbContext.Products
                    .Where(i => i.Id == product.ProductId)
                    .FirstOrDefault();

                var size = GetSizeName(mainProduct.SizeId);

                product.DeliveryCost = CalculateDeliveryCostOfTheProductInCo
                   (product.InvoicedQuantity,product.QuantityM3ForCalc, invoice.TotalQuantity, 
                   invoice.DeliveryTrasnportCost,product.Unit,size);

                product.Profit = (product.SellPrice - purchaseProductDetailCostPrice) * 
                    product.InvoicedQuantity - product.DeliveryCost;
            }

            dbContext.SaveChanges();
            return true;
        }
        public List<ProductForCreditNoteViewModelPerInvoice> ProductsForCreditNotePerInvoice(int invoiceId)
        {
            if(invoiceId == 0) return null;

            var products = dbContext.InvoiceProductDetails
                .Where(i => i.InvoiceId == invoiceId)
                .ToList();

            var productsForEdit = new List<ProductForCreditNoteViewModelPerInvoice>();    

            foreach (var product in products)
            {
                var mainProduct = dbContext.Products
                    .Where(i => i.Id == product.ProductId)
                    .FirstOrDefault();

                productsForEdit.Add(new ProductForCreditNoteViewModelPerInvoice
                {
                    ProductId = product.ProductId,
                    FscClaim = product.FscClaim,
                    FscCertificate = product.FscCertificate,
                    SellPrice = product.SellPrice,
                    InvoicedQuantity = product.InvoicedQuantity,
                    Pallets = product.Pallets,
                    SheetsPerPallet = product.SheetsPerPallet,
                    Unit = product.Unit,
                    DescriptionId = mainProduct.DescriptionId,
                    GradeId = mainProduct.GradeId,
                    SizeId = mainProduct.SizeId,
                    Description = GetDescriptionName(mainProduct.DescriptionId),
                    Grade = GetGradeName(mainProduct.GradeId),
                    Size = GetSizeName(mainProduct.SizeId)
                });
            }

            return productsForEdit;
        }

        public decimal ConvertStringSizeToQubicMeters(string size)
        {
            var dimensionArray = size.Split('/').ToArray();           
            decimal sum = 1M;

            for (int i = 0; i < dimensionArray.Count(); i++)
            {
                sum *= Math.Round(decimal.Parse(dimensionArray[i]) / 1000, 5);
            }

            return sum;
        }

        public void ReviseAutstandingQuantity(int customerOrderDetailId, decimal quantity)
        {
            var productFromCustomerOrder = dbContext.CustomerOrderProductDetails
                .Where(i => i.Id == customerOrderDetailId)
                .FirstOrDefault();

            productFromCustomerOrder.AutstandingQuantity -= quantity;
        }

        public List<NewProductsFromOrderEditedDebitNoteViewModel> ProductsForDebitNotePerInvoice(int invoiceId)
        {
            var productsFromInvoice = dbContext.InvoiceProductDetails
                .Where(i => i.InvoiceId == invoiceId)
                .ToList();

            if(productsFromInvoice == null)
            {
                return new List<NewProductsFromOrderEditedDebitNoteViewModel>();
            }

            var products = new List<NewProductsFromOrderEditedDebitNoteViewModel>();

            foreach (var product in productsFromInvoice)
            {
                var mainProduct = productRepository.GetMainProduct(product.ProductId);

                var invoicedProduct = new NewProductsFromOrderEditedDebitNoteViewModel
                {
                    ProductId = product.ProductId,
                    Id = product.Id,
                    CustomerOrderId = product.CustomerOrderId ?? 0,
                    DebitNoteId = product.DebitNoteId,
                    InvoicedQuantity = product.InvoicedQuantity,
                    DeliveryCost = product.DeliveryCost,
                    DescriptionId = mainProduct.DescriptionId,
                    Description = GetDescriptionName(mainProduct.DescriptionId),
                    SizeId = mainProduct.SizeId,
                    Size = GetSizeName(mainProduct.SizeId),
                    GradeId = mainProduct.GradeId,
                    Grade = GetGradeName(mainProduct.GradeId),
                    FscCertificate = product.FscCertificate,
                    FscClaim = product.FscClaim,
                    Pallets = product.Pallets,
                    SheetsPerPallet = product.SheetsPerPallet,
                    SellPrice = product.SellPrice,
                    Unit = product.Unit,
                    Profit = product.Profit,
                    PurchaseProductDetailsId = product.PurchaseProductDetailsId ?? 0
                };

                products.Add(invoicedProduct); 
            }

            return products;
        }

        public List<DescriptionsViewModel> DescriptionCollection()
        {
            return dbContext.Descriptions
                .Select(n => new DescriptionsViewModel
                {
                    Id = n.Id,
                    Name = n.Name
                }).ToList();
        }

        public List<SizeViewModel> SizeCollection()
        {
            return dbContext.Sizes
                .Select(n => new SizeViewModel
                {
                    Id = n.Id,
                    Name = n.Name
                }).ToList();
        }

        public List<GradeViewModel> GradeCollection()
        {
            return dbContext.Grades
                .Select(n => new GradeViewModel
                {
                    Id = n.Id,
                    Name = n.Name
                }).ToList();
        }

        public ICollection<PurchaseProductFscCollectionViewModel> PurchaseProductFscCollection
            (int myCompany, DateTime startDate, DateTime endDate, string fscClaim)
        {
            var purchaseInvoices = dbContext.Documents
                .Where(m => m.MyCompanyId == myCompany 
                && m.DocumentType == DocumentTypes.Purchase
                && m.Date >= startDate
                && m.Date <= endDate)
                .Select(i => i.Id)
                .ToList();

            var products = dbContext.PurchaseProductDetails
                .Where(p=> purchaseInvoices.Contains(p.PurchaseInvoiceId) && !String.IsNullOrEmpty(p.FscClaim) && p.FscClaim != "-");

            if (fscClaim != "-")
            {
                if (!String.IsNullOrEmpty(fscClaim))
                    products = products.Where(f=>f.FscClaim == fscClaim);
            }


            var productsCollection = products.ToList();

            var purchaseProducts = new List<PurchaseProductFscCollectionViewModel>();    

            foreach (var product in productsCollection)
            {
                var mainProduct = productRepository.GetMainProduct(product.ProductId);
                var document = dbContext.Documents
                    .Where(i => i.Id == product.PurchaseInvoiceId)
                    .FirstOrDefault();

                var supplierName = dbContext.Suppliers
                    .Where(i => i.Id == document.SupplierId)
                    .Select(n => n.Name)
                    .FirstOrDefault();

                var purchaseProduct = new PurchaseProductFscCollectionViewModel
                {
                    DescriptionId = mainProduct.DescriptionId,
                    FscClaim = product.FscClaim,
                    PurchaseInvoice = document.PurchaseNumber,
                    PurchaseDate = document.Date,
                    Quantity = product.Quantity,
                    SupplierName = supplierName,
                    Transport = document.TruckNumber,
                    Unit = product.Unit.ToString()
                };
                purchaseProduct.Description = GetDescriptionName(mainProduct.DescriptionId);
                purchaseProducts.Add(purchaseProduct);
            }

           return purchaseProducts;
        }

        public ICollection<SoldProductsFscCollectionViewModel> SoldProductFscCollection
            (int myCompany, DateTime startDate, DateTime endDate, string fscClaim)
        {
            var invoices = dbContext.Documents
                .Where(m=>m.MyCompanyId == myCompany 
                && m.DocumentType != DocumentTypes.Purchase
                && m.Date >= startDate 
                && m.Date <= endDate)
                .Select(i=>i.Id)
                .ToList();

            var products = dbContext.InvoiceProductDetails           
                .Where(i=> invoices.Contains(i.InvoiceId)
                && !String.IsNullOrEmpty(i.FscClaim) && i.FscClaim != "-");

            if (fscClaim != "-")
            { if(!String.IsNullOrEmpty(fscClaim))
                products = products.Where(f => f.FscClaim == fscClaim);
            }

            var soldProducts = new List<SoldProductsFscCollectionViewModel>();  

            foreach (var product in products.ToList())
            {
                var mainProduct = productRepository.GetMainProduct(product.ProductId);
                var invoice = dbContext.Documents
                    .Where(i => i.Id == product.InvoiceId || i.Id == product.CreditNoteId || i.Id == product.DebitNoteId)
                    .FirstOrDefault();

                var customerName = dbContext.Customers
                    .Where(i=>i.Id == invoice.CustomerId)
                    .Select(n=>n.Name)
                    .FirstOrDefault();

                var purchaseInvoiceId = dbContext.PurchaseProductDetails
                    .Where(i=>i.Id == product.PurchaseProductDetailsId)
                    .Select(n=>n.PurchaseInvoiceId)
                    .FirstOrDefault();

                var purchaseInvoice = dbContext.Documents
                    .Where(i => i.Id == purchaseInvoiceId)
                    .FirstOrDefault();

                var supplier = dbContext.Suppliers
                    .Where(a => a.Id == purchaseInvoice.SupplierId)
                    .Select(a => a.Name)
                    .FirstOrDefault();

                var soldProduct = new SoldProductsFscCollectionViewModel
                {
                    DescriptionId = mainProduct.DescriptionId,
                    CustomerName = customerName,
                    Date = invoice.Date,
                    Description = GetDescriptionName(mainProduct.DescriptionId),
                    FscClaim = product.FscClaim,
                    InvoiceNumber = invoice.DocumentNumber,
                    Transport = invoice.TruckNumber,
                    Unit = product.Unit.ToString(),
                    PurchaseInvoice = purchaseInvoice.PurchaseNumber,
                    Supplier = supplier
                };

                if(invoice.DocumentType == DocumentTypes.Invoice)
                {
                    soldProduct.Quantity = product.InvoicedQuantity;
                }
                else if(invoice.DocumentType == DocumentTypes.CreditNote)
                {
                    soldProduct.Quantity = product.CreditNoteQuantity;
                }
                else if(invoice.DocumentType == DocumentTypes.DebitNote)
                {
                    soldProduct.Quantity = product.DebitNoteQuantity;
                }
                soldProduct.Description = GetDescriptionName(mainProduct.DescriptionId);    
                soldProducts.Add(soldProduct);
            }

            return soldProducts;
        }

        public void CalculateCostPriceInDiffCurrency(int purchaseId, string action)
        {
            var purchase = dbContext.Documents.Find(purchaseId);
            if(purchase == null) { return; }    

           var payments = dbContext.Payments
                .Where(d=>d.DocumentId == purchaseId)
                .ToList();

            var purchaseProducts = dbContext.PurchaseProductDetails
                .Where(i => i.PurchaseInvoiceId == purchaseId)
                .ToList();
          
            var paymentsSum = payments.Select(a=>a.NewAmountPerExchangeRate).Sum();
            decimal calcNumber = 0.0m;
            
            var expenses = purchase.Duty + purchase.Factoring * purchase.Amount / 100 +
                      purchase.CustomsExpenses + purchase.FiscalAgentExpenses +
                      purchase.ProcentComission * purchase.Amount / 100 + purchase.PurchaseTransportCost +
                      purchase.BankExpenses + purchase.OtherExpenses;

            foreach (var product in purchaseProducts)
            {
                var mainProduct = productRepository.GetMainProduct(product.ProductId);

                var size = GetSizeName(mainProduct.SizeId);
                var dimensionArray = size.Split('/').ToArray();
                decimal sum = 1M;
                decimal thickness = Math.Round(decimal.Parse(dimensionArray[0]) / 1000, 5);
                decimal calcMeter = Math.Round(decimal.Parse(dimensionArray[0]) / 1000, 5) * Math.Round(decimal.Parse(dimensionArray[2]) / 1000, 5); ;
                decimal feetTom3 = product.Quantity * 0.0929m * thickness;

                switch (action)
                {
                    case "/": calcNumber = 1 / (paymentsSum / purchase.Amount ?? 0);product.Amount /= calcNumber; break;
                    case "*": calcNumber = paymentsSum / purchase.Amount ?? 0;product.Amount *= calcNumber; break;
                    default: break;
                }


                for (int i = 0; i < dimensionArray.Count(); i++)
                {
                    sum *= Math.Round(decimal.Parse(dimensionArray[i]) / 1000, 5);
                }

                if (product.Unit == Data.Enums.Unit.m3)
                {
                    product.CostPrice = (product.Amount + (expenses / purchase.TotalQuantity * product.Quantity)) / product.Quantity;
                }
                else
                if (product.Unit == Data.Enums.Unit.m2)
                {
                    product.CostPrice = ((product.Amount + (expenses / purchase.TotalQuantity * product.QuantityM3)) / product.QuantityM3 ?? 0) * thickness;
                }
                else if (product.Unit == Data.Enums.Unit.pcs || product.Unit == Data.Enums.Unit.sheets)
                {
                    product.CostPrice = ((product.Amount + (expenses / purchase.TotalQuantity * product.QuantityM3)) / product.QuantityM3 ?? 0) * sum;
                }
                else if (product.Unit == Data.Enums.Unit.sqfeet)
                {
                    product.CostPrice = ((product.Amount + (expenses / purchase.TotalQuantity * product.QuantityM3)) / product.QuantityM3 ?? 0) / feetTom3;
                }
                else
                {
                    product.CostPrice = ((product.Amount + (expenses / purchase.TotalQuantity * product.QuantityM3)) / product.QuantityM3 ?? 0) * calcMeter;
                }
            }
        }
    }
}

