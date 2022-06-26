using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Data.Models;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Customers;
using SSMO.Models.Products;
using SSMO.Services;
using SSMO.Services.Customer;
using SSMO.Services.MyCompany;
using SSMO.Services.Product;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace SSMO.Controllers
{
    public class CustomerOrdersController : Controller
    {
        private readonly ISupplierService supplierService;
        private readonly ICurrency currency;
        private readonly IMycompanyService myCompanyService;
        private readonly ICustomerService customerService;
        private readonly IProductService productService;
        private readonly IMapper mapper;

        public CustomerOrdersController(ISupplierService supplierService,
           ICurrency currency,
           IMycompanyService myCompanyService,
           ICustomerService customerService,
           IProductService productService, IMapper  mapper
           )
        {
            this.supplierService = supplierService;
            this.currency = currency;
            this.myCompanyService = myCompanyService;
            this.customerService = customerService;
            this.productService = productService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddCustomerOrder()
        {


            return View(
                new CustomerOrderViewModel
                {
                    Currencies = currency.AllCurrency(),
                    Customers = customerService.CustomersData(),
                    MyCompanies =myCompanyService.GetAllCompanies(),
                    Suppliers = supplierService.GetSuppliers(),
                    Products = new List<ProductViewModel>()
                 }

                ) ;
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddCustomerOrder(CustomerOrderViewModel customermodel)

        {
            if (!ModelState.IsValid)
            {
                new CustomerOrderViewModel
                {
                    Currencies = currency.AllCurrency(),
                    Customers = customerService.CustomersData(),
                    MyCompanies = myCompanyService.GetAllCompanies(),
                    Suppliers = supplierService.GetSuppliers(),
                    Products = new List<ProductViewModel>()
                };

                new ProductViewModel
                {
                    Descriptions = productService.GetDescriptions(),
                    Grades = productService.GetGrades(),
                    Sizes = productService.GetSizes()
                };
            }



            //check customerID
            //check mycompany 
           

            var customerOrder = new CustomerOrder
                  { Id = customermodel.Id,
                Number = customermodel.Number,
            
                Amount = customermodel.Amount,
                    Balance = customermodel.Balance,
                    ClientId = customermodel.ClientId,
                  CurrencyId = customermodel.CurrencyId,
                  DeliveryTerms = customermodel.DeliveryTerms,
                 
                  PaidAmountStatus = customermodel.PaidAmountStatus,
                  PaidAvance = customermodel.PaidAvance,
              
            };

            

            return RedirectToAction("Home");
        }

    }
}
