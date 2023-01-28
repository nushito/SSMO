﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Data;
using SSMO.Data.Enums;
using SSMO.Infrastructure;
using SSMO.Models.Products;
using SSMO.Models.SupplierOrders;
using SSMO.Services;
using SSMO.Services.Customer;
using SSMO.Services.CustomerOrderService;
using SSMO.Services.MyCompany;
using SSMO.Services.Products;
using SSMO.Services.Status;
using SSMO.Services.SupplierOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SSMO.Controllers
{
    public class SupplierOrdersController : Controller
    {
        private readonly ISupplierService supplierService;
        private readonly ICurrency currency;
        private readonly IMycompanyService myCompanyService;
        private readonly ICustomerService customerService;
        private readonly IProductService productService;
        private readonly ICustomerOrderService cusomerOrderService;
        private readonly IMapper mapper;
        private readonly ApplicationDbContext dbContext;
        private readonly IStatusService statusService;
        private readonly ISupplierOrderService supplierOrderService;
        public SupplierOrdersController(ISupplierService supplierService,
           ICurrency currency,
           IMycompanyService myCompanyService,
           ICustomerService customerService,
           IProductService productService, IMapper mapper,
 ApplicationDbContext dbContext,
 ICustomerOrderService cusomerOrderService, IStatusService statusService,
            ISupplierOrderService supplierOrderService)
        {
            this.supplierService = supplierService;
            this.currency = currency;
            this.myCompanyService = myCompanyService;
            this.customerService = customerService;
            this.productService = productService;
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.cusomerOrderService = cusomerOrderService;
            this.statusService = statusService;
            this.supplierOrderService = supplierOrderService;
        }



        [HttpGet]
        [Authorize]
        public IActionResult AddSupplierConfirmation()
        {
            return View(new SupplierOrderFormModel
            {
                Currencies = currency.AllCurrency(),
                MyCompanies = myCompanyService.GetAllCompanies(),
                Suppliers = supplierService.GetSuppliers(),
                Statuses = statusService.GetAllStatus(),
                CustomerOrders = cusomerOrderService.AllCustomerOrderNumbers()
            });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult AddSupplierConfirmation(SupplierOrderFormModel model)
        {
            string userId = this.User.UserId();
            string userIdMyCompany = myCompanyService.GetUserIdMyCompanyById(model.MyCompanyId);

            if (userIdMyCompany != userId)
            {
                return BadRequest();
            }

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                new SupplierOrderFormModel
                {
                    Currencies = currency.AllCurrency(),
                    MyCompanies = myCompanyService.GetAllCompanies(),
                    Suppliers = supplierService.GetSuppliers(),
                    Statuses = statusService.GetAllStatus(),
                    CustomerOrders = cusomerOrderService.AllCustomerOrderNumbers()
                };

            };

            if (!supplierService.GetSuppliers().Any())
            {
                return RedirectToAction("AddSupplier", "SuppliersController");
            };

            if (!cusomerOrderService.CheckOrderNumberExist(model.CustomerOrderNumber))
            {
                return BadRequest();
            };

            var thisCustomerOrder = cusomerOrderService.OrderPerNumber(model.CustomerOrderNumber);
            var customerorderId = thisCustomerOrder.Id;
            var supplierOrderId = supplierOrderService.CreateSupplierOrder
                                  (model.MyCompanyId, model.SupplierId, model.Date,
                                   model.Number, model.CustomerOrderNumber, model.StatusId,
                                   model.CurrencyId, model.FscClaim, model.VAT ?? 0, model.DatePaidAmount,
                                   model.PaidAvance, model.PaidStatus, model.LoadingAddress, model.DeliveryAddress,
                                   model.DeliveryTerms);

            return RedirectToAction("EditProductAsPerSupplier", new { customerOrderId = customerorderId, supplierOrderId = supplierOrderId });
        }

        [HttpGet]
        public IActionResult EditProductAsPerSupplier(
            int customerorderId, int supplierOrderId)
        {
            string userId = this.User.UserId();
            var myCompaniesUserId = myCompanyService.GetCompaniesUserId();

            if(!myCompaniesUserId.Contains(userId)) return BadRequest();

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                new ProductSupplierFormModel
                {
                    Descriptions = productService.GetDescriptions(),
                    Grades = productService.GetGrades(),
                    Sizes = productService.GetSizes(),
                    Units = productService.GetUnits()
                };
            }

            var corder = productService.Details(customerorderId);

            if (!corder.Any())
            {
                return RedirectToAction("AddCustomerOrder", "CustomerOrdersController", customerorderId);
            }

            var listProducts = new List<ProductSupplierFormModel>();
            var supplierFscCert = supplierService.GetSupplierFscCertificateByOrderId(supplierOrderId);

            foreach (var product in corder)
            {
                var productSupp = new ProductSupplierFormModel
                {
                    Id = product.Id,
                    Description = product.Description,
                    Grade = product.Grade,
                    Size = product.Size,
                    DescriptionId = product.DescriptionId,
                    GradeId = product.GradeId,
                    SizeId = product.SizeId,
                    PurchaseFscCertificate = product.PurchaseFscCertificate,
                    PurchaseFscClaim = product.PurchaseFscClaim,
                    SupplierFscCertNumber = supplierFscCert,
                    Pallets = product.Pallets,
                    SheetsPerPallet = product.SheetsPerPallet,
                    Descriptions = productService.GetDescriptions(),
                    Grades = productService.GetGrades(),
                    Sizes = productService.GetSizes(),
                    CustomerOrderId = customerorderId,
                    SupplierOrderId = supplierOrderId,
                    QuantityM3 = product.QuantityM3,
                    Unit = product.Unit
                };

                listProducts.Add(productSupp);
            };

            return View(listProducts);
        }

        [HttpPost]
        [Authorize]
        public IActionResult EditProductAsPerSupplier(
          int customerorderId, int supplierOrderId, List<ProductSupplierFormModel> productmodel)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                new ProductSupplierFormModel
                {
                    Descriptions = productService.GetDescriptions(),
                    Grades = productService.GetGrades(),
                    Sizes = productService.GetSizes(),
                    Units= productService.GetUnits()
                };
            }

            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest();
            }

            if (!productmodel.Any())
            {
                return RedirectToAction("AddCustomerOrder", "CustomerOrdersController", customerorderId);
            }

            var supplierFscCert = supplierService.GetSupplierFscCertificateByOrderId(supplierOrderId);

            foreach (var product in productmodel)
            {
                product.SupplierFscCertNumber = supplierFscCert;
                var check = productService.EditProduct(product.Id, customerorderId, supplierOrderId, product.Description, product.Grade,
                             product.Size, product.PurchaseFscCertificate, product.PurchaseFscClaim,
                             product.Pallets, product.SheetsPerPallet,
                             product.PurchasePrice, product.QuantityM3, product.Unit);

                if (!check)
                {
                    return BadRequest();
                }

            }

            supplierOrderService.TotalAmountAndQuantitySum(supplierOrderId);

            return RedirectToAction("PrintSupplierOrder", supplierOrderId);
        }


        public IActionResult PrintSupplierOrder(int supplierOrderId)
        {

            return RedirectToAction("Index", "Home");
        }

    }
}
