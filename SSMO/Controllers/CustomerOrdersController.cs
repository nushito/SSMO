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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Controllers
{
    public class CustomerOrdersController : Controller
    {
        private readonly ISupplierService supplierService;
        private readonly ICurrency currency;
        private readonly IMycompanyService myCompanyService;
        private readonly ICustomerService customerService;
        private readonly IProductService productService;

        public CustomerOrdersController(ISupplierService supplierService,
           ICurrency currency,
           IMycompanyService myCompanyService,
           ICustomerService customerService,
           IProductService productService
           )
        {
            this.supplierService = supplierService;
            this.currency = currency;
            this.myCompanyService = myCompanyService;
            this.customerService = customerService;
            this.productService = productService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddCustomerOrder()
        {


            return View(
                new CustomerOrderViewModel
                {
                    Currencies = currency.AllCurrency(),
                    Customers = (IEnumerable<AddCustomerFormModel>)customerService.AllCustomers(),
                    MyCompanies = (IEnumerable<Models.MyCompany.MyCompanyFormModel>)myCompanyService.GetCompany(),
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
                    Customers = (IEnumerable<AddCustomerFormModel>)customerService.AllCustomers(),
                    MyCompanies = (IEnumerable<Models.MyCompany.MyCompanyFormModel>)myCompanyService.GetCompany(),
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
            { Amount = customermodel.Amount,
             Balance = customermodel.Balance,
               ClientId = customermodel.ClientId,
              CurrencyId = customermodel.CurrencyId,
               DeliveryTerms = customermodel.DeliveryTerms,
                Number = customermodel.Number,
                 PaidAmountStatus = customermodel.PaidAmountStatus,
                  PaidAvance = customermodel.PaidAvance,
              
            };

            

            return RedirectToAction("Home");
        }

    }
}
