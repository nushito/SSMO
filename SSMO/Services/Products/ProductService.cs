using AutoMapper;
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
        private readonly IMapper mapper;

        public  ProductService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            this.mapper = mapper;
        }

        public void CreateProduct(ProductViewModel model)
        {
            var description = new Description
            {
                Name = model.Description
            
            };
            var size = new Size
            {
                Name = model.Size
            };
            var grade = new Grade
            {
                Name = model.Grade
            };
            var product = new Product
            {
                Amount = model.Amount,
                Description = description,
                Grade = grade,
                Size = size,
                 FSCClaim = model.FSCClaim,
     FSCSertificate = model.FSCSertificate,
 OrderedQuantity = model.Cubic,
 Price = model.CostPrice
  

            };
            _dbContext.Products.Add(product);   
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
    }
}
