using AutoMapper;
using AutoMapper.QueryableExtensions;
using SSMO.Data;
using SSMO.Data.Enums;
using SSMO.Data.Migrations;
using SSMO.Data.Models;
using SSMO.Models.Documents.DebitNote;
using SSMO.Models.Documents.Purchase;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Models.Reports.Purchase;
using SSMO.Repository;
using SSMO.Services.Products;
using SSMO.Services.SupplierOrders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSMO.Services.Documents.Purchase
{
    public class PurchaseService : IPurchaseService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConfigurationProvider mapper;
        private readonly IProductService productService;
        private readonly ISupplierOrderService supplierOrderService;
        private readonly IProductRepository productRepository;

        public PurchaseService(ApplicationDbContext dbContext, IConfigurationProvider mapper, IProductService productService,
            ISupplierOrderService supplierOrderService, IProductRepository productRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.productService = productService;
            this.supplierOrderService = supplierOrderService;
            this.productRepository = productRepository; 
        }
        public bool CreatePurchaseAsPerSupplierOrder(int id,
            string number, DateTime date, bool paidStatus,
            decimal netWeight, decimal brutoWeight,
            decimal duty, decimal factoring, decimal customsExpenses, decimal fiscalAgentExpenses,
            decimal procentComission, decimal purchaseTransportCost, decimal bankExpenses, decimal otherExpenses, 
            int vat, string truckNumber, string swb,List<PurchaseProductAsSupplierOrderViewModel> products, 
            string incoterms, decimal paidAdvance, DateTime? dateOfPayment, string deliveryAddress)
        {
            var supplierOrder = dbContext.SupplierOrders.FirstOrDefault
                (o => o.Id == id);

            if (supplierOrder == null) { return false; }

            var purchase = new Document
            {
                PurchaseNumber = number,
                Date = date,
                DocumentType = Data.Enums.DocumentTypes.Purchase,
                DeliveryAddress= deliveryAddress,
                SupplierOrder = supplierOrder,
                SupplierOrderId = supplierOrder.Id,
                PaidStatus = paidStatus,
                PaidAvance = paidAdvance,
                DatePaidAmount= dateOfPayment,
                NetWeight = netWeight,
                GrossWeight = brutoWeight,
                Duty = duty,
                Factoring = factoring,
                CustomsExpenses = customsExpenses,                
                FiscalAgentExpenses = fiscalAgentExpenses,
                ProcentComission = procentComission,
                PurchaseTransportCost = purchaseTransportCost,
                BankExpenses = bankExpenses,
                OtherExpenses = otherExpenses,
                CurrencyId = supplierOrder.CurrencyId,
                Incoterms = incoterms,
                Vat = vat,
                TruckNumber = truckNumber,
                Swb= swb,
                SupplierId = supplierOrder.SupplierId,
                MyCompanyId = supplierOrder.MyCompanyId,
                PurchaseProducts = new List<PurchaseProductDetails>(),
                CustomerOrderProducts = new List<CustomerOrderProductDetails>()
            };

            dbContext.Documents.Add(purchase);
            dbContext.SaveChanges();

            foreach (var product in products)
            {
                if(product.OrderedQuantity == 0) { continue; }

                var mainProduct = productRepository.GetMainProduct(product.Id);
                var size = productService.GetSizeName(mainProduct.SizeId);

                mainProduct.PurchaseProductDetails = new List<PurchaseProductDetails>();

                if(product.OrderedQuantity > mainProduct.OrderedQuantity) { return false; }

                var purchaseProduct = new PurchaseProductDetails
                {
                    ProductId = mainProduct.Id,
                    PurchaseInvoiceId = purchase.Id,
                    PurchasePrice = product.PurchasePrice,
                    Pallets = product.Pallets,
                    SheetsPerPallet = product.SheetsPerPallet,
                    Unit = product.Unit,
                    SupplierOrderId = supplierOrder.Id,
                    FscCertificate = product.PurchaseFscCertificate,
                    FscClaim = product.PurchaseFscClaim,
                    Quantity = (decimal) product.OrderedQuantity,
                    VehicleNumber = product.VehicleNumber                    
                };
                purchaseProduct.Amount = purchaseProduct.Quantity*purchaseProduct.PurchasePrice;
                purchaseProduct.TotalSheets = purchaseProduct.Pallets*purchaseProduct.SheetsPerPallet;
                var quantityM3 = productService.ConvertStringSizeToQubicMeters(size);
               purchaseProduct.QuantityM3 = quantityM3 * product.Pallets * product.SheetsPerPallet;

                mainProduct.PurchaseProductDetails.Add(purchaseProduct);
                mainProduct.LoadedQuantityM3 += purchaseProduct.Quantity;
                mainProduct.QuantityLeftForPurchaseLoading = mainProduct.OrderedQuantity - purchaseProduct.Quantity;
                purchase.PurchaseProducts.Add(purchaseProduct);

                var customerOrderProducts = dbContext.CustomerOrderProductDetails
              .Where(c => c.SupplierOrderId == supplierOrder.Id && c.ProductId == mainProduct.Id)
              .FirstOrDefault();

                if(customerOrderProducts != null)
                {
                    purchase.CustomerOrderProducts.Add(customerOrderProducts);
                }               
                purchase.Amount +=  purchaseProduct.Amount;

                if (products.All(u => u.Unit == Unit.m3) || products.All(u => u.Unit == Unit.m2)
                   || products.All(u => u.Unit == Unit.pcs) || products.All(u => u.Unit == Unit.sheets)
                   || products.All(u => u.Unit == Unit.m))
                {
                    purchase.TotalQuantity = (decimal)products.Where(i => i.ProductOrNot == true).Sum(a => a.OrderedQuantity);                   
                }
                else
                if (product.ProductOrNot == true)
                {
                    if(product.Unit == Data.Enums.Unit.m3)
                    {
                        purchase.TotalQuantity += purchaseProduct.Quantity;    
                    }
                    else 
                    {                                            
                        purchase.TotalQuantity += (decimal)purchaseProduct.QuantityM3;
                    }                    
                }            
            }
            
            purchase.VatAmount = purchase.Amount * purchase.Vat / 100;
            purchase.TotalAmount = purchase.Amount + purchase.VatAmount ?? 0;
           
            if (purchase == null)
            {
                return false;
            }

            if (purchase.PaidStatus == true)
            {
                purchase.Balance = 0;
                purchase.PaidAvance = purchase.Amount;
            }
            else
            {
                if(paidAdvance > 0)
                {
                    purchase.Balance = purchase.TotalAmount - paidAdvance;
                }
                else
                {
                    purchase.Balance = purchase.TotalAmount;
                }               
            }

            var expenses = purchase.Duty + purchase.Factoring * purchase.Amount / 100 +
                      purchase.CustomsExpenses + purchase.FiscalAgentExpenses +
                      purchase.ProcentComission * purchase.Amount / 100 + purchase.PurchaseTransportCost +
                      purchase.BankExpenses + purchase.OtherExpenses;         

            foreach (var product in purchase.PurchaseProducts)
            {               
                var mainProduct = productRepository.GetMainProduct(product.ProductId);

                var size = productService.GetSizeName(mainProduct.SizeId);
                var dimensionArray = size.Split('/').ToArray();
                decimal sum = 1M;
                decimal thickness = Math.Round(decimal.Parse(dimensionArray[0]) / 1000, 5); 
                decimal calcMeter = Math.Round(decimal.Parse(dimensionArray[0]) / 1000, 5)* Math.Round(decimal.Parse(dimensionArray[2]) / 1000, 5); ;

                for (int i = 0; i < dimensionArray.Count(); i++)
                {
                    sum *= Math.Round(decimal.Parse(dimensionArray[i]) / 1000, 5);
                }

                if(product.Unit == Data.Enums.Unit.m3)
                {
                    product.CostPrice = (product.Amount + (expenses / purchase.TotalQuantity * product.Quantity)) / product.Quantity;                    
                }
                else
                if (product.Unit == Data.Enums.Unit.m2)
                {
                    product.CostPrice = ((product.Amount + (expenses / purchase.TotalQuantity * product.QuantityM3)) / product.QuantityM3 ?? 0)*thickness;                    
                }
                else if (product.Unit == Data.Enums.Unit.pcs || product.Unit == Data.Enums.Unit.sheets)
                {
                    product.CostPrice = ((product.Amount + (expenses / purchase.TotalQuantity * product.QuantityM3)) / product.QuantityM3 ?? 0) * sum;
                }
                else
                {
                    product.CostPrice = ((product.Amount + (expenses / purchase.TotalQuantity * product.QuantityM3)) / product.QuantityM3 ?? 0) * calcMeter;
                }                
            }

            var productsBySupplierOrder = dbContext.Products
                .Where(p => p.SupplierOrderId == supplierOrder.Id).ToList();

            if(productsBySupplierOrder.Sum(a=>a.OrderedQuantity) == productsBySupplierOrder.Sum(b => b.LoadedQuantityM3))
            {
                supplierOrder.StatusId = dbContext.Statuses
                    .Where(a=>a.Name == "Finished")
                    .Select(i=>i.Id)
                    .FirstOrDefault();
            }
            
            dbContext.SaveChanges();
            return true;
        }

        public bool EditPurchaseInvoice(int id, string number, DateTime date, int supplierOrderId, 
            int vat, decimal netWeight, decimal grossWeight, string truckNumber, string swb, 
            decimal purchaseTransportCost, decimal bankExpenses, decimal duty, decimal customsExpenses, 
            decimal factoring, decimal fiscalAgentExpenses, decimal procentComission, decimal otherExpenses, 
            List<PurchaseProductsForEditFormModel> purchaseProducts, string deliveryAddress)
        {
            if(id == 0) { return false; }   

            var purchaseInvoiceForEdit = dbContext.Documents
                .Where(i=>i.Id == id)
                .FirstOrDefault();

            if(purchaseInvoiceForEdit == null) { return false; }

            purchaseInvoiceForEdit.PurchaseNumber= number;
            purchaseInvoiceForEdit.Date= date;           
            purchaseInvoiceForEdit.Vat= vat;
            purchaseInvoiceForEdit.NetWeight = netWeight;
            purchaseInvoiceForEdit.GrossWeight = grossWeight;
            purchaseInvoiceForEdit.TruckNumber = truckNumber;
            purchaseInvoiceForEdit.Swb= swb;
            purchaseInvoiceForEdit.PurchaseTransportCost= purchaseTransportCost;
            purchaseInvoiceForEdit.BankExpenses= bankExpenses;
            purchaseInvoiceForEdit.Duty= duty;
            purchaseInvoiceForEdit.CustomsExpenses= customsExpenses;
            purchaseInvoiceForEdit.Factoring = factoring;
            purchaseInvoiceForEdit.FiscalAgentExpenses= fiscalAgentExpenses;
            purchaseInvoiceForEdit.ProcentComission= procentComission;
            purchaseInvoiceForEdit.OtherExpenses= otherExpenses;
            purchaseInvoiceForEdit.TotalQuantity = 0;
            purchaseInvoiceForEdit.Amount = 0;
            purchaseInvoiceForEdit.DeliveryAddress= deliveryAddress;
        
            foreach (var product in purchaseProducts)
            {
                var productForEdit = dbContext.PurchaseProductDetails
                   .Where(s => s.Id == product.Id)
                   .FirstOrDefault();

                var mainProduct = productRepository.GetMainProduct(product.ProductId);

                if(mainProduct.OrderedQuantity < product.Quantity)
                {
                    return false;
                }
               
                mainProduct.LoadedQuantityM3 -= productForEdit.Quantity;
                mainProduct.LoadedQuantityM3 += product.Quantity;
                mainProduct.QuantityLeftForPurchaseLoading = mainProduct.OrderedQuantity - mainProduct.LoadedQuantityM3;
                
                productForEdit.PurchasePrice = product.PurchasePrice;
                productForEdit.SheetsPerPallet = product.SheetsPerPallet;   
                productForEdit.Pallets = product.Pallets;
                productForEdit.FscCertificate = product.FscCertificate;
                productForEdit.FscClaim = product.FscClaim;
                productForEdit.Quantity = product.Quantity;
                productForEdit.Amount = product.Quantity * product.PurchasePrice;
                purchaseInvoiceForEdit.Amount += productForEdit.Amount;

                if (purchaseProducts.All(u=>u.Unit == Unit.m3.ToString()) || purchaseProducts.All(u => u.Unit == Unit.m2.ToString())
                    || purchaseProducts.All(u => u.Unit == Unit.pcs.ToString()) || purchaseProducts.All(u => u.Unit == Unit.sheets.ToString())
                    || purchaseProducts.All(u => u.Unit == Unit.m.ToString()))
                {
                    purchaseInvoiceForEdit.TotalQuantity += purchaseProducts.Where(i=>i.ProductOrNot == true).Sum(a => a.Quantity);                    
                }

                else
                {
                    if (product.ProductOrNot == true)
                    {
                        if (productForEdit.Unit == Data.Enums.Unit.m3)
                        {
                            purchaseInvoiceForEdit.TotalQuantity += productForEdit.Quantity;                            
                        }
                        else
                        {
                            var size = productService.GetSizeName(mainProduct.SizeId);
                            var quantityM3 = productService.ConvertStringSizeToQubicMeters(size);
                            productForEdit.QuantityM3 = quantityM3 * productForEdit.Pallets*productForEdit.SheetsPerPallet;
                            purchaseInvoiceForEdit.TotalQuantity += productForEdit.QuantityM3 ?? 0;
                        }
                    }
                }

                if (productForEdit.Unit != Unit.m3)
                {
                    var size = productService.GetSizeName(mainProduct.SizeId);
                    var quantityM3 = productService.ConvertStringSizeToQubicMeters(size);
                    productForEdit.QuantityM3 = quantityM3 * productForEdit.Pallets * productForEdit.SheetsPerPallet;
                }                
            }

            purchaseInvoiceForEdit.VatAmount = purchaseInvoiceForEdit.Vat * purchaseInvoiceForEdit.Amount / 100;
            purchaseInvoiceForEdit.TotalAmount = purchaseInvoiceForEdit.VatAmount ?? 0 + purchaseInvoiceForEdit.Amount;

            dbContext.SaveChanges();

            var expenses = purchaseInvoiceForEdit.Duty + purchaseInvoiceForEdit.Factoring * purchaseInvoiceForEdit.Amount / 100 +
                   purchaseInvoiceForEdit.CustomsExpenses + purchaseInvoiceForEdit.FiscalAgentExpenses +
                   purchaseInvoiceForEdit.ProcentComission * purchaseInvoiceForEdit.Amount / 100 + purchaseInvoiceForEdit.PurchaseTransportCost +
                   purchaseInvoiceForEdit.BankExpenses + purchaseInvoiceForEdit.OtherExpenses;

            foreach (var product in purchaseInvoiceForEdit.PurchaseProducts)
            {
                var mainProduct = productRepository.GetMainProduct(product.ProductId);

                var size = productService.GetSizeName(mainProduct.SizeId);
                var quantityM3 = productService.ConvertStringSizeToQubicMeters(size);
                product.QuantityM3 = quantityM3 * product.Pallets * product.SheetsPerPallet;

                var dimensionArray = size.Split('/').ToArray();
                decimal sum = 1M;
                decimal thickness = Math.Round(decimal.Parse(dimensionArray[0]) / 1000, 5);
                decimal calcMeter = Math.Round(decimal.Parse(dimensionArray[0]) / 1000, 5) * Math.Round(decimal.Parse(dimensionArray[2]) / 1000, 5); 

                for (int i = 0; i < dimensionArray.Count(); i++)
                {
                    sum *= Math.Round(decimal.Parse(dimensionArray[i]) / 1000, 5);
                }
                if (product.Unit == Data.Enums.Unit.m3)
                {
                    product.QuantityM3 = product.Quantity;
                    product.CostPrice = (product.Amount + (expenses / purchaseInvoiceForEdit.TotalQuantity * product.Quantity)) / product.Quantity;
                }
                else
                if (product.Unit == Data.Enums.Unit.m2)
                {
                    product.CostPrice = ((product.Amount + (expenses / purchaseInvoiceForEdit.TotalQuantity * product.QuantityM3)) / product.QuantityM3 ?? 0) * thickness;
                }
                else if (product.Unit == Data.Enums.Unit.pcs || product.Unit == Data.Enums.Unit.sheets)
                {
                    product.CostPrice = ((product.Amount + (expenses / purchaseInvoiceForEdit.TotalQuantity * product.QuantityM3)) / product.QuantityM3 ?? 0) * sum;
                }
                else
                {
                    product.CostPrice = ((product.Amount + (expenses / purchaseInvoiceForEdit.TotalQuantity * product.QuantityM3)) / product.QuantityM3 ?? 0) * calcMeter;
                }
            }

            var productsBySupplierOrder = dbContext.Products
                .Where(p => p.SupplierOrderId == supplierOrderId).ToList();

            var supplierOrder = dbContext.SupplierOrders
                .Where(i=>i.Id == purchaseInvoiceForEdit.SupplierOrderId).FirstOrDefault();

            if (productsBySupplierOrder.Sum(a => a.OrderedQuantity) == productsBySupplierOrder.Sum(b => b.LoadedQuantityM3))
            {
                supplierOrder.StatusId = dbContext.Statuses
                    .Where(a => a.Name == "Finished")
                    .Select(i => i.Id)
                    .FirstOrDefault();
            }

            dbContext.SaveChanges();
            return true;
        }

        public bool EditPurchasePayment
            (string number, bool paidStatus, decimal paidAvance, DateTime datePaidAmount)
        {
            if (number == null)
            {
                return false;
            }

            var purchase = dbContext.Documents
                .Where(type => type.DocumentType == Data.Enums.DocumentTypes.Purchase && type.PurchaseNumber.ToLower() == number.ToLower())
                .FirstOrDefault(); 
           
            purchase.PaidStatus = paidStatus;
            purchase.PaidAvance = paidAvance;
            purchase.DatePaidAmount = datePaidAmount;
            purchase.Balance = purchase.TotalAmount - purchase.PaidAvance;
            if (purchase.Balance == 0)
            {
                purchase.PaidStatus = true;
            }
            else
            {
                purchase.PaidStatus = false;
            }

            return true;
        }
        public EditPurchasePaymentDetails GetPurchaseForPaymentEdit(string number)
        {
            var purchase = dbContext.Documents
                .Where(type => type.DocumentType == Data.Enums.DocumentTypes.Purchase && type.PurchaseNumber.ToLower() == number.ToLower());

            var purchaseForEdit = purchase.ProjectTo<EditPurchasePaymentDetails>(mapper).FirstOrDefault();

            return purchaseForEdit;
        }
        public IEnumerable<PurchaseModelAsPerSpec> GetSupplierOrdersForPurchase(string supplierName
            , int currentpage, int supplierOrdersPerPage)
        {
            if (String.IsNullOrEmpty(supplierName))
            {
                return new List<PurchaseModelAsPerSpec>();
            }

            var supplierId = dbContext.Suppliers.Where(a => a.Name.ToLower() == supplierName.ToLower())
                .Select(a => a.Id).FirstOrDefault();

            var queryOrders = dbContext.SupplierOrders.
                Where(a => a.SupplierId == supplierId && a.Products.Any(a=>a.QuantityLeftForPurchaseLoading>0)).OrderByDescending(a => a.Date);

            var totalOrders = queryOrders.Count();

            var orders = queryOrders.ProjectTo<PurchaseModelAsPerSpec>(this.mapper).ToList();

            var supplierOrdersList = orders.Skip((currentpage - 1) * supplierOrdersPerPage).Take(supplierOrdersPerPage);

            return supplierOrdersList;
        }

        public List<PurchaseProductAsSupplierOrderViewModel> Products(int id)
        {
            
            var products = dbContext.Products.Where(s => s.SupplierOrderId == id);
            var productDetails = products.ProjectTo<PurchaseProductAsSupplierOrderViewModel>(mapper).ToList();
            foreach (var product in productDetails)
            {
                product.Description = productService.GetDescriptionName(product.DescriptionId);
                product.Grade = productService.GetGradeName(product.GradeId);
                product.Size = productService.GetSizeName(product.SizeId);
            }
            return productDetails;
        }

        public PurchaseInvoiceDetailsViewModel PurchaseDetails(int id)
        {
            if(id == 0)
            {
                return null;
            }

            var purchaseInvoice = dbContext.Documents
                .Where(a => a.Id == id)
                .FirstOrDefault();

            var supplierOrderNumber = dbContext.SupplierOrders
                .Where(i=>i.Id == purchaseInvoice.SupplierOrderId)
                .Select(n=>n.Number)
                .FirstOrDefault();

          
            var purchaseForEdit = new PurchaseInvoiceDetailsViewModel
            {
                BankExpenses = purchaseInvoice.BankExpenses,
                CustomsExpenses = purchaseInvoice.CustomsExpenses,
                Date = purchaseInvoice.Date,
                Duty = purchaseInvoice.Duty,
                Factoring = purchaseInvoice.Factoring,
                FiscalAgentExpenses = purchaseInvoice.FiscalAgentExpenses,
                GrossWeight = purchaseInvoice.GrossWeight,
                Incoterms = purchaseInvoice.Incoterms,
                NetWeight = purchaseInvoice.NetWeight,
                DeliveryAddress = purchaseInvoice.DeliveryAddress,
                PurchaseNumber = purchaseInvoice.PurchaseNumber,
                OtherExpenses = purchaseInvoice.OtherExpenses,
                ProcentComission = purchaseInvoice.ProcentComission,
                PurchaseTransportCost = purchaseInvoice.PurchaseTransportCost,
                SupplierOrderId= purchaseInvoice.SupplierOrderId,
                TruckNumber = purchaseInvoice.TruckNumber,
                Swb = purchaseInvoice.Swb,
                Vat = purchaseInvoice.Vat ?? 0,
                TotalAmount = purchaseInvoice.TotalAmount,
                Products = new List<PurchaseProductsDetailsViewModel>()
            };

            purchaseForEdit.SupplierOrderNumber = dbContext.SupplierOrders
                .Where(a => a.Id == purchaseInvoice.SupplierOrderId)
                .Select(n => n.Number)
                .FirstOrDefault();

            var products = dbContext.PurchaseProductDetails
                .Where(pi => pi.PurchaseInvoiceId == id)
                .ToList();

            foreach (var product in products)
            {
                var mainProduct = productRepository.GetMainProduct(product.ProductId);

                purchaseForEdit.Products.Add(
                    new PurchaseProductsDetailsViewModel
                    {
                        Description = productService.GetDescriptionName(mainProduct.DescriptionId),
                        Grade = productService.GetGradeName(mainProduct.GradeId),
                        Size = productService.GetSizeName(mainProduct.SizeId),
                        FscCertificate = product.FscCertificate,
                        FscClaim = product.FscClaim,
                        Pallets = product.Pallets,
                        Quantity = product.Quantity,
                        SheetsPerPallet = product.SheetsPerPallet,
                        Unit = product.Unit.ToString(),
                        PurchasePrice = product.PurchasePrice,
                        CostPrice= product.CostPrice,
                        TotalSheets= product.TotalSheets
                    });
            }
            return purchaseForEdit;
        }

        public EditPurchaseViewModel PurchaseDetailsForEdit(int id)
        {
            if (id == 0)
            {
                return null;
            }

            var purchaseInvoice = dbContext.Documents
                .Where(a => a.Id == id)
                .FirstOrDefault();

            var supplierOrderNumbers = dbContext.SupplierOrders;                               

            var supplierOrdersNumbersUpdate = supplierOrderNumbers.ProjectTo<SupplierOrdersListForPurchaseEditModel>(mapper).ToList();

            var purchaseForEdit = new EditPurchaseViewModel
            {
                BankExpenses = purchaseInvoice.BankExpenses,
                CustomsExpenses = purchaseInvoice.CustomsExpenses,
                Date = purchaseInvoice.Date,
                Duty = purchaseInvoice.Duty,
                Factoring = purchaseInvoice.Factoring,
                FiscalAgentExpenses = purchaseInvoice.FiscalAgentExpenses,
                GrossWeight = purchaseInvoice.GrossWeight,
                Incoterms = purchaseInvoice.Incoterms,
                NetWeight = purchaseInvoice.NetWeight,
                DeliveryAddress = purchaseInvoice.DeliveryAddress,
                PurchaseNumber = purchaseInvoice.PurchaseNumber,
                OtherExpenses = purchaseInvoice.OtherExpenses,
                ProcentComission = purchaseInvoice.ProcentComission,
                PurchaseTransportCost = purchaseInvoice.PurchaseTransportCost,
                SupplierOrderId = purchaseInvoice.SupplierOrderId,
                SupplierOrders = supplierOrdersNumbersUpdate,
                TruckNumber = purchaseInvoice.TruckNumber,
                Swb = purchaseInvoice.Swb,
                Vat = purchaseInvoice.Vat ?? 0,
                PurchaseProducts = new List<PurchaseProductsForEditFormModel>()
            };

            var products = dbContext.PurchaseProductDetails
                .Where(a => a.PurchaseInvoiceId == id);               

            purchaseForEdit.PurchaseProducts = products.ProjectTo< PurchaseProductsForEditFormModel >(mapper).ToList();

            foreach (var product in purchaseForEdit.PurchaseProducts)
            {
                var mainProduct = productRepository.GetMainProduct(product.ProductId);

                product.Description = productService.GetDescriptionName(mainProduct.DescriptionId);
                product.Grade = productService.GetGradeName(mainProduct.GradeId);
                product.Size = productService.GetSizeName(mainProduct.SizeId);
            }

            return purchaseForEdit; 
        }

        public IList<PurchaseProductsForDebitNoteViewModel> PurchaseProducts()
        {
            var productsForSale = dbContext.Products
                .Where(a=>a.LoadedQuantityM3 > 0 && a.SoldQuantity < a.LoadedQuantityM3)
                .SelectMany(p=>p.PurchaseProductDetails)
                .ToList();

            var purchaseProducts = new List<PurchaseProductsForDebitNoteViewModel>();

            foreach (var product in productsForSale)
            {
                var mainProduct = productRepository.GetMainProduct(product.ProductId);

                var customerOrderIdList = dbContext.CustomerOrderProductDetails
                    .Where(i=>i.ProductId == product.ProductId && i.SupplierOrderId == product.SupplierOrderId)
                    .Select(i=>i.CustomerOrderId)
                    .ToList();

                var customerOrder = new List<CustomerOrderNumbersByCustomerViewModel>();

                if (customerOrderIdList != null)
                {                   
                    customerOrder = dbContext.CustomerOrders
                    .Where(i => customerOrderIdList.Contains(i.Id))
                    .Select(n => new CustomerOrderNumbersByCustomerViewModel
                    {
                        Id = n.Id,
                        OrderConfirmationNumber = n.OrderConfirmationNumber,
                        CustomerOrderProductId = n.CustomerOrderProducts.FirstOrDefault().Id,
                    })
                    .ToList();
                }

                var productForDebit = new PurchaseProductsForDebitNoteViewModel
                {
                    Id = product.Id,
                    ProductId = product.ProductId,
                    DescriptionId = mainProduct.DescriptionId,
                    Description = productService.GetDescriptionName(mainProduct.DescriptionId),
                    SizeId = mainProduct.SizeId,
                    Size = productService.GetSizeName(mainProduct.SizeId),
                    GradeId = mainProduct.GradeId,
                    Grade = productService.GetGradeName(mainProduct.GradeId),
                    AvailableQuantity = mainProduct.LoadedQuantityM3 - mainProduct.SoldQuantity,
                    PurchaseInvoicelId = product.PurchaseInvoiceId,
                    Unit = product.Unit,
                    FscSertificate = product.FscCertificate,
                    FscClaim = product.FscClaim,     
                    CustomerOrderDetail = new List<CustomerOrderNumbersByCustomerViewModel>()                                     
                };

                if(customerOrder != null)
                {
                    productForDebit.CustomerOrderDetail = customerOrder;
                }                
                
                productForDebit.ProductFullDescription = String.Join
                    (", ", productForDebit.Description,productForDebit.Size,productForDebit.Grade, productForDebit.AvailableQuantity);
                purchaseProducts.Add(productForDebit);
            }

            return purchaseProducts;
        }
    }
}
