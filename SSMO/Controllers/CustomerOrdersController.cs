using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Data.Models;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Products;
using SSMO.Services;
using SSMO.Services.Customer;
using SSMO.Services.MyCompany;
using SSMO.Services.Products;
using System.Collections.Generic;
using AutoMapper;
using SSMO.Data;
using SSMO.Services.CustomerOrderService;
using System.Linq;
using SSMO.Services.Reports;
using System;
using SSMO.Services.Status;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using SSMO.Infrastructure;

namespace SSMO.Controllers
{
    public class CustomerOrdersController : Controller
    {
        private readonly ISupplierService supplierService;
        private readonly ICurrency currency;
        private readonly IMycompanyService myCompanyService;
        private readonly ICustomerService customerService;
        private readonly IProductService productService;
        private readonly ICustomerOrderService customerOrderService;
        private readonly IMapper mapper;
        private readonly IReportsService reportService;
        private readonly IStatusService statusService;
        public CustomerOrdersController(ISupplierService supplierService,
           ICurrency currency,
           IMycompanyService myCompanyService,
           ICustomerService customerService,
           IProductService productService, IMapper mapper,
 ApplicationDbContext dbContext, ICustomerOrderService cusomerOrderService,
 IReportsService reportService, IStatusService statusService)
        {
            this.supplierService = supplierService;
            this.currency = currency;
            this.myCompanyService = myCompanyService;
            this.customerService = customerService;
            this.productService = productService;
            this.mapper = mapper;
            this.customerOrderService = cusomerOrderService;
            this.reportService = reportService;
            this.statusService = statusService;
        }


        [HttpGet]
        [Authorize]
        public IActionResult AddCustomerOrder()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (customerOrderService.AnyCustomerOrderExist())
            {
                ViewBag.NumberExist = 1;

            }
            return View(
                new CustomerOrderViewModel
                {
                    Currencies = currency.AllCurrency(),
                    Customers = customerService.CustomersData(),
                    MyCompanies = myCompanyService.GetAllCompanies(),
                    Suppliers = supplierService.GetSuppliers(),
                    Products = new List<ProductCustomerFormModel>(),
                    Statuses = statusService.GetAllStatus()
                }

                );
        }


        [HttpPost]
        [Authorize]
        public IActionResult AddCustomerOrder(CustomerOrderViewModel customermodel)

        {
            string userId = this.User.UserId();
            string userIdMyCompany = myCompanyService.GetUserIdMyCompanyById(customermodel.CustomerId);

            if (userIdMyCompany != userId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                new CustomerOrderViewModel
                {
                    Currencies = currency.AllCurrency(),
                    Customers = customerService.CustomersData(),
                    MyCompanies = myCompanyService.GetAllCompanies(),
                    Suppliers = supplierService.GetSuppliers(),
                    Products = new List<ProductCustomerFormModel>(),
                    Statuses = statusService.GetAllStatus()
                };

                new ProductCustomerFormModel
                {
                    Descriptions = productService.GetDescriptions(),
                    Grades = productService.GetGrades(),
                    Sizes = productService.GetSizes()
                };
            }

            int customerorderId;
            if (!customerOrderService.AnyCustomerOrderExist())
            {
                customerorderId = customerOrderService.CreateFirstOrder
                                (customermodel.OrderConfirmationNumber,
                                 customermodel.CustomerPoNumber,
                                 customermodel.Date,
                                 customermodel.CustomerId,
                                 customermodel.MyCompanyId,
                                 customermodel.DeliveryTerms,
                                 customermodel.LoadingPlace,
                                 customermodel.DeliveryAddress,
                                 customermodel.CurrencyId,
                                 customermodel.Origin,
                                 customermodel.PaidAmountStatus,
                                  customermodel.Vat ?? 0);
                ViewBag.NumberExist = 0;
            }
            else
            {
                customerorderId = customerOrderService.CreateOrder
                                 (customermodel.CustomerPoNumber,
                                 customermodel.Date,
                                 customermodel.CustomerId,
                                 customermodel.MyCompanyId,
                                 customermodel.DeliveryTerms,
                                 customermodel.LoadingPlace,
                                 customermodel.DeliveryAddress,
                                 customermodel.CurrencyId,
                                 customermodel.Origin,
                                 customermodel.PaidAmountStatus,
                                 customermodel.Vat ?? 0);
                ViewBag.NumberExist = 1;
            }

            TempData["Count"] = customermodel.ProductsCount;

            return RedirectToAction("AddOrderProducts", new { CustomerOrderId = customerorderId });
        }



        public IActionResult AddOrderProducts()
        {

            var count = int.Parse(TempData["Count"].ToString());

            var products = new List<ProductCustomerFormModel>();
            for (int i = 0; i < count; i++)
            {
                var product = new ProductCustomerFormModel()
                {
                    Descriptions = productService.GetDescriptions(),
                    Grades = productService.GetGrades(),
                    Sizes = productService.GetSizes()

                };
                products.Add(product);
            }

            return View(products);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddOrderProducts(int customerorderId,
           IEnumerable<ProductCustomerFormModel> model)
        {
            // var count = int.Parse(TempData["Count"].ToString());

            if (!ModelState.IsValid)
            {
                var products = new List<ProductCustomerFormModel>();

                foreach (var item in products)
                {
                    var product = new ProductCustomerFormModel()
                    {
                        Descriptions = productService.GetDescriptions(),
                        Grades = productService.GetGrades(),
                        Sizes = productService.GetSizes()

                    };
                    products.Add(product);
                }

            }

            if (!model.Any())
            {
                return View(model);
            }

            foreach (var item in model)
            {
                productService.CreateProduct(item, customerorderId);
            }

            customerOrderService.CustomerOrderCounting(customerorderId);

            return RedirectToAction("PrintCustomerOrder");
        }


        public IActionResult PrintCustomerOrder()
        {
            return RedirectToAction("Index", "Home");
        }


    }
}
