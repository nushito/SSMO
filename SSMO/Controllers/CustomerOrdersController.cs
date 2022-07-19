﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Data.Models;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Products;
using SSMO.Services;
using SSMO.Services.Customer;
using SSMO.Services.MyCompany;
using SSMO.Services.Product;
using System.Collections.Generic;
using AutoMapper;
using SSMO.Data;
using SSMO.Services.CustomerOrderService;
using System.Linq;
using SSMO.Services.Reports;
using System;

namespace SSMO.Controllers
{
    public class CustomerOrdersController : Controller
    {
        private readonly ISupplierService supplierService;
        private readonly ICurrency currency;
        private readonly IMycompanyService myCompanyService;
        private readonly ICustomerService customerService;
        private readonly IProductService productService;
        private readonly ICustomerOrderService cusomerOrderService;
        private readonly IMapper mapper;
        private readonly IReportsService reportService;
        private readonly ApplicationDbContext dbContext;
        public CustomerOrdersController(ISupplierService supplierService,
           ICurrency currency,
           IMycompanyService myCompanyService,
           ICustomerService customerService,
           IProductService productService, IMapper  mapper,
 ApplicationDbContext dbContext, ICustomerOrderService cusomerOrderService, IReportsService reportService)
        {
            this.supplierService = supplierService;
            this.currency = currency;
            this.myCompanyService = myCompanyService;
            this.customerService = customerService;
            this.productService = productService;
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.cusomerOrderService = cusomerOrderService;
            this.reportService = reportService; 
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
                    Products = new List<ProductViewModel>(),
                    
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
                    Id = customermodel.Id,  
                    Currencies = currency.AllCurrency(),
                    Customers = customerService.CustomersData(),
                    MyCompanies = myCompanyService.GetAllCompanies(),
                    Suppliers = supplierService.GetSuppliers(),
                    Products = new List<ProductViewModel>(),
                    
                };

                new ProductViewModel
                {
                    Descriptions = productService.GetDescriptions(),
                    Grades = productService.GetGrades(),
                    Sizes = productService.GetSizes()
                };
            }
         

            var customerorderId = cusomerOrderService.CreateOrder
                (customermodel.Number, 
                customermodel.Date,
                customermodel.ClientId,
                customermodel.MyCompanyId,
                customermodel.DeliveryTerms,
                customermodel.LoadingPlace,
                customermodel.DeliveryAddress,
                customermodel.CurrencyId);

            ViewBag.ProductsCount = customermodel.ProductsCount;
            TempData["Count"] = customermodel.ProductsCount;  

            return RedirectToAction("AddOrderProducts", new { CustomerOrderId = customerorderId, ViewBag.ProductsCount });
        }



        public IActionResult AddOrderProducts()
        {

            var count = int.Parse(TempData["Count"].ToString());
                     
            var products = new List<ProductViewModel>();
            for (int i = 0; i < count; i++)
            {
               var product = new ProductViewModel()
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
           IList<ProductViewModel> model)
        {
           // var count = int.Parse(TempData["Count"].ToString());

            if (!ModelState.IsValid)
            {
                

                var products = new List<ProductViewModel>();
                foreach (var item in products)
                {
                    var product = new ProductViewModel()
                    {
                        Descriptions = productService.GetDescriptions(),
                        Grades = productService.GetGrades(),
                        Sizes = productService.GetSizes()

                    };
                    products.Add(product);
                }
                //for (int i = 0; i < count; i++)
                //{
                    
                //}
            }

            if(model.Count == 0)
            {
                return View(model);
            }

          //  var count = ViewBag.ProductsCount;

            var thisorder = reportService.Details(customerorderId);
       

            foreach (var item in model)
            {
                item.Amount = Math.Round( item.CostPrice * item.Cubic,4);
                thisorder.Products.Add(item);
                dbContext.SaveChanges();
            }
         
            //thisPurchase.Products = listProducts;
            //dbContext.SaveChanges();


            return RedirectToAction("Home");
        }

    }
}