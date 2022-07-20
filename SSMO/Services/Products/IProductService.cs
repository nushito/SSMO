using SSMO.Models.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services.Products
{
    public interface IProductService
    {
        public void CreateProduct(ProductViewModel model, int customerorderId);
        public bool DescriptionExist(string name);
        public void AddDescription(string name);
        public void AddGrade(string name);
        public bool GradeExist(string name);
        public void AddSize(string name);
        public bool SizeExist(string name);
        public IEnumerable<string> GetDescriptions();
        public IEnumerable<string> GetSizes();
        public IEnumerable<string> GetGrades();
    }
}
