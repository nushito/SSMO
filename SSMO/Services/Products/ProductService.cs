using AutoMapper;
using AutoMapper.QueryableExtensions;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Products;
using SSMO.Models.Reports.ProductsStock;
using SSMO.Models.Reports.SupplierOrderReportForEdit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSMO.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConfigurationProvider mapper;

        public ProductService(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper.ConfigurationProvider;
        }

        public void CreateProduct(ProductCustomerFormModel model, int customerorderId)
        {
            var description = dbContext.Descriptions.Where(a => a.Name == model.Description).FirstOrDefault();

            var size = dbContext.Sizes.Where(a => a.Name == model.Size).FirstOrDefault();
            var grade = dbContext.Grades.Where(a => a.Name == model.Grade).FirstOrDefault();

            var product = new Product
            {
                Amount = model.Amount,
                Description = description,
                Grade = grade,
                Size = size,
                FSCClaim = model.FSCClaim,
                FSCSertificate = model.FSCSertificate,
                Price = model.Price,
                Pallets = model.Pallets,
                SheetsPerPallet = model.SheetsPerPallet,
                CustomerOrderId = customerorderId,
                QuantityM3 = model.QuantityM3
            };

            var dimensionArray = size.Name.Split('/').ToArray();
            var countArray = dimensionArray.Count();
            decimal sum = 1M;

            for (int i = 0; i < countArray; i++)
            {
                sum *= Math.Round(decimal.Parse(dimensionArray[i]) / 1000, 4);
            }
            product.TotalSheets = product.Pallets * product.SheetsPerPallet;

            if (model.QuantityM3 != 0)
            {
                product.OrderedQuantity = model.QuantityM3;
            }
            else
            {
                product.OrderedQuantity = Math.Round(sum * product.TotalSheets, 4);
            }
            product.Amount = Math.Round(product.Price * product.OrderedQuantity, 4);
            dbContext.Products.Add(product);
            dbContext.SaveChanges();


            var order = dbContext.CustomerOrders.Where(a => a.Id == customerorderId).FirstOrDefault();
            order.Amount += product.Amount;

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

        public bool EditProduct(int id, int customerorderId,
            int supplierOrderId,
           string description, string grade,
            string size, string fscClaim, string fscCertificate,
            int pallets, int sheetsPerPallet, decimal purchasePrice, decimal quantityM3)
        {


            var product = dbContext.Products.Find(id);
            var descriptionEdit = dbContext.Descriptions.Where(a => a.Name == description).Select(a => a.Id).FirstOrDefault();
            var gradeEdit = dbContext.Grades.Where(a => a.Name == grade).Select(a => a.Id).FirstOrDefault();
            var sizeEdit = dbContext.Sizes.Where(a => a.Name == size).Select(a => a.Id).FirstOrDefault();

            if (product == null)
            {
                return false;
            }

            product.DescriptionId = descriptionEdit;
            product.GradeId = gradeEdit;
            product.SizeId = sizeEdit;
            product.FSCClaim = fscClaim;
            product.FSCSertificate = fscCertificate;
            product.Pallets = pallets;
            product.SheetsPerPallet = sheetsPerPallet;
            product.PurchasePrice = purchasePrice;
            product.SupplierOrderId = supplierOrderId;

            var dimensionArray = size.Split('/').ToArray();
            var countArray = dimensionArray.Count();
            decimal sum = 1M;

            for (int i = 0; i < countArray; i++)
            {
                sum *= Math.Round(decimal.Parse(dimensionArray[i]) / 1000, 4);
            }

            product.TotalSheets = product.Pallets * product.SheetsPerPallet;
            if (quantityM3 != 0)
            {
                product.OrderedQuantity = quantityM3;
            }
            else
            {
                product.OrderedQuantity = Math.Round(sum * product.TotalSheets, 4);
            }

            product.PurchaseAmount = Math.Round(product.PurchasePrice * product.OrderedQuantity, 4);
            product.Amount = Math.Round(product.Price * product.OrderedQuantity, 4);

            var spOrder = dbContext.SupplierOrders.Where(i => i.Id == supplierOrderId).FirstOrDefault();

            spOrder.Amount += product.PurchaseAmount;
            spOrder.Products.Add(product);

            dbContext.SaveChanges();

            return true;
        }

        public IEnumerable<ProductSupplierDetails> Details(int customerId)
        {
            var products = dbContext.Products
                .Where(a => a.CustomerOrderId == customerId)
                .ProjectTo<ProductSupplierDetails>(mapper)
                .ToList();

            foreach (var item in products)
            {
                item.Description = GetDescriptionName(item.DescriptionId);
                item.Grade = GetGradeName(item.GradeId);
                item.Size = GetSizeName(item.SizeId);
            }

            return products;
        }

        public ICollection<ProductCustomerFormModel> DetailsPerCustomerOrder(int customerId)
        {
            var products = dbContext.Products
                .Where(a => a.CustomerOrderId == customerId)
                .ProjectTo<ProductCustomerFormModel>(mapper)
                .ToList();

            foreach (var item in products)
            {
                item.Description = GetDescriptionName(item.DescriptionId);
                item.Grade = GetGradeName(item.GradeId);
                item.Size = GetSizeName(item.SizeId);
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

        public decimal CalculateDeliveryCostOfTheProductInCo(decimal quantity, decimal totalQuantity, decimal deliveryCost)
        {
            var cost = deliveryCost / totalQuantity * quantity;
            return cost;
        }

        public ICollection<ProductsForEditSupplierOrder> ProductsDetailsPerSupplierOrder(int supplierOrderId)
        {
            var customerOrderId = dbContext.SupplierOrders
                .Where(num => num.Id == supplierOrderId)
                .Select(id => id.CustomerOrderId)
                .ToList();

            var products = dbContext.Products
                 .Where(a => customerOrderId.Contains(a.CustomerOrderId))
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

        public void ClearProductQuantityWhenDealIsFinished(int productId)
        {
            var product = dbContext.Products
                .Where(id => id.Id == productId)
                .FirstOrDefault();
            if (product != null)
            {
                product.LoadedQuantityM3 = 0;
            }
            dbContext.SaveChanges();
        }

        public IEnumerable<ProductAvailabilityDetailsViewModel> ProductsOnStock
            (int descriptionId, int gradeId, int sizeId, int currentPage, int productsPerPage)
        {
            var products = dbContext.Products
               .Where(d => d.LoadedQuantityM3 != 0)
               .ToList();

            if (descriptionId != 0 && gradeId != 0 && sizeId != 0)
            {
                products = products
                         .Where(d => d.DescriptionId == descriptionId && d.GradeId == gradeId && d.SizeId == sizeId)
                         .ToList();

            }

            var productsOnStok = new List<ProductAvailabilityDetailsViewModel>();
            string supplierOrderDeliveryAddress = null;

            foreach (var product in products)
            {
                var statusId = dbContext.Statuses
                    .Where(n=>n.Name == "Active")
                    .Select(id=>id.Id)
                    .FirstOrDefault();

                var order = dbContext.CustomerOrders
                    .Where(pr => pr.Id == product.CustomerOrderId && pr.StatusId == statusId)
                    .FirstOrDefault();

                if(order == null) return new List<ProductAvailabilityDetailsViewModel>();

                var purchase = dbContext.Documents
                    .Where(p=>p.Id == product.DocumentId)
                    .FirstOrDefault();

                if (purchase != null)
                {
                     supplierOrderDeliveryAddress = dbContext.SupplierOrders
                        .Where(dI => dI.Documents.Any(p => p.Id == purchase.Id))
                        .Select(ad => ad.DeliveryAddress)
                        .FirstOrDefault();
                }

                var customerName = dbContext.Customers
                    .Where(o => o.Id == order.CustomerId)
                    .Select(n=>n.Name)
                    .FirstOrDefault();

                var supplierName = dbContext.Suppliers
                    .Where(d => d.Documents.Any(o => o.CustomerOrderId == order.Id))
                    .Select(n => n.Name)
                    .FirstOrDefault();

                var availableProduct = new ProductAvailabilityDetailsViewModel
                {
                    CostPrice = product.CostPrice,
                    CustomerOrderId = product.CustomerOrderId,
                    LoadedQuantity = product.LoadedQuantityM3,
                    OrderedQuantity = product.OrderedQuantity,
                    FSCClaim = product.FSCClaim,
                    FSCSertificate = product.FSCSertificate,
                    DeliveryAddress = supplierOrderDeliveryAddress,
                    CustomerOrderNumber = order.OrderConfirmationNumber,
                    CustomerName = customerName,
                    PurchasePrice = product.PurchasePrice,
                    Pallets = product.Pallets,
                    SheetsPerPallet = product.SheetsPerPallet,
                    OrderDate = order.Date,
                    PurchaseDate = purchase.Date,
                    PurchaseNumber = purchase.Number,
                    Price = product.Price,
                    SupplierName = supplierName
                };
                productsOnStok.Add(availableProduct);
            }

            var productCollection = productsOnStok.Skip((currentPage - 1) * productsPerPage).Take(productsPerPage);

            return productCollection;
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
    }
}
