using AutoMapper;
using AutoMapper.QueryableExtensions;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Products;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSMO.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfigurationProvider mapper;

        public  ProductService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            this.mapper = mapper.ConfigurationProvider;
        }

        public void CreateProduct(ProductViewModel model,int customerorderId)
        {
            var description = _dbContext.Descriptions.Where(a => a.Name == model.Description).FirstOrDefault();            
            
            var size = _dbContext.Sizes.Where(a => a.Name == model.Size).FirstOrDefault();
            var grade = _dbContext.Grades.Where(a => a.Name == model.Grade).FirstOrDefault();

            var product = new Product
            {
                Amount = model.Amount,
                Description = description,
                Grade = grade,
                Size = size,
                 FSCClaim = model.FSCClaim,
                FSCSertificate = model.FSCSertificate,
            // OrderedQuantity = model.Cubic,
             Price = model.Price,
              Pallets = model.Pallets,
              SheetsPerPallet = model.SheetsPerPallet,
               CustomerOrderId = customerorderId, 
             
            };

           

            var dimensionArray = size.Name.Split('/').ToArray();
            var countArray = dimensionArray.Count();
            decimal sum = 1M;

            for (int i = 0; i < countArray; i++)
            {
                sum*= Math.Round(decimal.Parse(dimensionArray[i])/1000,4);
            }

            product.TotalSheets = product.Pallets * product.SheetsPerPallet;
            product.OrderedQuantity = Math.Round( sum * product.TotalSheets,4);
            product.Amount = Math.Round(product.Price * product.OrderedQuantity, 4);
            _dbContext.Products.Add(product);
            var order = _dbContext.CustomerOrders.Where(a => a.Id == customerorderId).FirstOrDefault();
            order.Products.ToList().Add(product);

            order.Amount += product.Amount;
            _dbContext.SaveChanges();
        }

        public void AddDescription(string name)
        {
            _dbContext.Descriptions.Add(new Description { Name = name });
            _dbContext.SaveChanges();
            return;
        }

        public void AddGrade(string name)
        {
            _dbContext.Grades.Add(new Grade { Name = name });
            _dbContext.SaveChanges();
            return;
        }

        public void AddSize(string name)
        {
            _dbContext.Sizes.Add(new Size { Name = name });
            _dbContext.SaveChanges();
            return;
        }

        public bool DescriptionExist(string name)
        {
            var check = _dbContext.Descriptions.Where(a=>a.Name.ToLower() == name.ToLower()).FirstOrDefault();

           if(check == null)
            {
                return false;
            }

            return true;
        }

        public bool GradeExist(string name)
        {
            var check = _dbContext.Grades.Where(a => a.Name.ToLower() == name.ToLower()).FirstOrDefault();

            if (check == null)
            {
                return false;
            }

            return true;
        }

        public bool SizeExist(string name)
        {
            var check = _dbContext.Sizes.Where(a => a.Name.ToLower() == name.ToLower()).FirstOrDefault();

            if (check == null)
            {
                return false;
            }

            return true;
        }



      public  IEnumerable<string> GetDescriptions()
        {
            return _dbContext.Descriptions.Select(a=>a.Name).ToList();
        }

        public IEnumerable<string> GetSizes()
        {
            return _dbContext.Sizes.Select(a => a.Name).ToList();
        }

        public IEnumerable<string> GetGrades()
        {
            return _dbContext.Grades.Select(a => a.Name).ToList();
        }

        public bool Edit(int id, int customerorderId,
            int supplierOrderId,
           string description, string grade,
            string size, string fscClaim, string fscCertificate,
            int pallets, int sheetsPerPallet, decimal purchasePrice)
        {


            var product = _dbContext.Products.Find(id);
            var descriptionEdit = _dbContext.Descriptions.Where(a => a.Name == description).Select(a=>a.Id).FirstOrDefault();
            var gradeEdit = _dbContext.Grades.Where(a => a.Name == grade).Select(a => a.Id).FirstOrDefault();
            var sizeEdit = _dbContext.Sizes.Where(a => a.Name == size).Select(a => a.Id).FirstOrDefault();

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


            var dimensionArray = size.Split('/').ToArray();
            var countArray = dimensionArray.Count();
            decimal sum = 1M;

            for (int i = 0; i < countArray; i++)
            {
                sum *= Math.Round(decimal.Parse(dimensionArray[i]) / 1000, 4);
            }

            product.TotalSheets = product.Pallets * product.SheetsPerPallet;
            product.LoadedQuantityM3 = Math.Round(sum * product.TotalSheets, 4);
        
            product.PurchaseAmount = Math.Round(product.PurchasePrice * product.OrderedQuantity, 4);
            product.Amount = Math.Round(product.Price * product.OrderedQuantity, 4);

            var spOrder = _dbContext.SupplierOrders.Find(supplierOrderId);

            spOrder.Amount += product.PurchaseAmount;
           
            spOrder.Products.Add(product);
            
            _dbContext.SaveChanges();

            return true;
        }

        public IEnumerable<ProductSupplierDetails> Details(int customerId)
        {
            var products = _dbContext.Products
                .Where(a => a.CustomerOrderId == customerId)
                .ProjectTo<ProductSupplierDetails>(mapper)
                .ToList();

            foreach (var item in products)
            {
                var descriptionName = _dbContext.Descriptions
               .Where(a => a.Id == item.DescriptionId)
               .Select(a => a.Name)
               .FirstOrDefault();

                var gradeName = _dbContext.Grades
               .Where(a => a.Id == item.GradeId)
               .Select(a => a.Name)
               .FirstOrDefault();

                var sizeName = _dbContext.Sizes
                     .Where(a => a.Id == item.SizeId)
               .Select(a => a.Name)
               .FirstOrDefault();

                item.Description = descriptionName;
                item.Grade = gradeName;
                item.Size = sizeName;
            }

            return products;

        }
        

         
    }
}
