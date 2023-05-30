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
using SSMO.Services.SupplierOrders;
using System.Text.Json;

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
        private readonly ISupplierOrderService supplierOrderService;
        public CustomerOrdersController(ISupplierService supplierService,
           ICurrency currency,
           IMycompanyService myCompanyService,
           ICustomerService customerService,
           IProductService productService, IMapper mapper,
 ApplicationDbContext dbContext, ICustomerOrderService cusomerOrderService,
 IReportsService reportService, IStatusService statusService, ISupplierOrderService supplierOrderService)
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
            this.supplierOrderService = supplierOrderService;   
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
            else
            {
                ViewBag.NumberExist = 0;
            }

            var customerOrderDetails = new CustomerOrderViewModel
            {
                Currencies = currency.AllCurrency(),
                Customers = customerService.CustomersData(),
                MyCompanies = myCompanyService.GetAllCompanies(),
                Products = new List<ProductCustomerFormModel>(),
                Statuses = statusService.GetAllStatus(),
                SupplierOrdersBySupplier = supplierOrderService.SuppliersAndOrders()
        };
            return View(customerOrderDetails);
        }
      
        [HttpPost]
        [Authorize]
        public IActionResult AddCustomerOrder(CustomerOrderViewModel customermodel)
        {
            string userId = this.User.UserId();
            string userIdMyCompany = myCompanyService.GetUserIdMyCompanyById(customermodel.MyCompanyId);

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
                   // Suppliers = supplierService.GetSuppliers(),
                    Products = new List<ProductCustomerFormModel>(),
                    Statuses = statusService.GetAllStatus(),
                    SupplierOrdersBySupplier = supplierOrderService.SuppliersAndOrders()
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
                                 customermodel.Vat ?? 0, customermodel.StatusId, 
                                 (List<int>)customermodel.SelectedSupplierOrders);
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
                                 customermodel.Vat ?? 0, customermodel.StatusId, 
                                 (List<int>)customermodel.SelectedSupplierOrders);
                ViewBag.NumberExist = 1;
            }
            return RedirectToAction("AddOrderProducts", new { selectedSupplierOrders = customermodel.SelectedSupplierOrders, customerorderId = customerorderId }) ;
        }

        public IActionResult AddOrderProducts(List<int> selectedSupplierOrders, int customerorderId)
        {
            string userId = this.User.UserId();
            var myCompanyUsersId = myCompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

            var products = productService.Details(selectedSupplierOrders);

            if (!products.Any())
            {
                return RedirectToAction("AddCustomerOrder", "CustomerOrdersController", customerorderId);
            }

            var listProducts = new List<ProductCustomerFormModel>();
            
            foreach (var product in products)
            {
                var productSupp = new ProductCustomerFormModel
                {        
                    Id = product.Id,          
                    Description = product.Description,
                    Grade = product.Grade,
                    Size = product.Size,
                    DescriptionId = product.DescriptionId,
                    GradeId = product.GradeId,
                    SizeId = product.SizeId,
                    FSCSertificate = product.PurchaseFscCertificate,
                    FSCClaim = product.FSCClaim,                    
                    Pallets = product.Pallets,
                    SheetsPerPallet = product.SheetsPerPallet,
                    Descriptions = productService.GetDescriptions(),
                    Grades = productService.GetGrades(),
                    Sizes = productService.GetSizes(),
                    SupplierOrderId = product.SupplierOrderId,
                    Quantity = product.OrderedQuantity,
                    Unit = product.Unit,
                    CustomerOrderId = customerorderId
                };
                listProducts.Add(productSupp);
            };

            return View(listProducts);

        }

        [HttpPost]
        [Authorize]
        public IActionResult AddOrderProducts(IEnumerable<ProductCustomerFormModel> model, int customerorderId)
        {
            // var count = int.Parse(TempData["Count"].ToString());
            string userId = this.User.UserId();
            var myCompanyUsersId = myCompanyService.GetCompaniesUserId();
            if (!myCompanyUsersId.Contains(userId)) { return BadRequest(); }

            if (!ModelState.IsValid)
            {
               new ProductCustomerFormModel()
                    {
                        Descriptions = productService.GetDescriptions(),
                        Grades = productService.GetGrades(),
                        Sizes = productService.GetSizes(),
                        Units = productService.GetUnits()
                    };
            }

            if (!model.Any())
            {
                return View(model);
            }

            foreach (var product in model)
            {       
                if(product.Quantity == 0) { continue; }
                var check = productService.CreateCustomerOrderProduct(product.Id, customerorderId, product.SupplierOrderId, product.Description,
                             product.Grade, product.Size, product.FSCSertificate, product.FSCClaim, product.Pallets,
                             product.SheetsPerPallet, product.SellPrice, product.Quantity, product.Unit);

                if (!check)
                {
                    return BadRequest();
                }

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
