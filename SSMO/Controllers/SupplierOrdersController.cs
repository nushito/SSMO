using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSMO.Data;
using SSMO.Data.Enums;
using SSMO.Data.Models;
using SSMO.Infrastructure;
using SSMO.Models.Documents;
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
           ICustomerOrderService cusomerOrderService, 
           IStatusService statusService,
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
            ViewBag.NumberExist = 1;
            return View(new SupplierOrderFormModel
            {
                Currencies = currency.AllCurrency(),
                MyCompanies = myCompanyService.GetAllCompanies(),
                Suppliers = supplierService.GetSuppliers(),
                Statuses = statusService.GetAllStatus(),   
                ProductList= new List<ProductSupplierFormModel> { },
                SupplierFscCertificate = supplierService.SuppliersFscCertificates()
            });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult AddSupplierConfirmation(SupplierOrderFormModel model, IFormCollection collections)
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
                    ProductList = new List<ProductSupplierFormModel> { },
                    SupplierFscCertificate = supplierService.SuppliersFscCertificates()
                };
            };

            if (!supplierService.GetSuppliers().Any())
            {
                return RedirectToAction("AddSupplier", "SuppliersController");
            };

            model.ProductList = new List<ProductSupplierFormModel>();

            var supplierOrderId = supplierOrderService.CreateSupplierOrder
                                  (model.MyCompanyId, model.SupplierId, model.Date,
                                   model.Number, model.StatusId,
                                   model.CurrencyId, model.FscClaim, model.VAT ?? 0, model.DatePaidAmount,
                                   model.PaidAvance, model.PaidStatus, model.LoadingAddress, model.DeliveryAddress,
                                   model.DeliveryTerms);
            ViewBag.NumberExist = 1;
          
            int loopsNum = 0;

            foreach (var key in collections.Keys)
            {
                if (key.Contains("Description"))
                {
                    loopsNum++;
                }
            }

            for (int i = 1; i <= loopsNum; i++)
            {
                var description = collections["DescriptionId[" + i + "]"];
                var grade = collections["GradeId[" + i + "]"];
                var size = collections["SizeId[" + i + "]"];
                var unit = collections["Unit[" + i + "]"];
                var price = collections["Price[" + i + "]"].ToString();
                var fscClaim = collections["FscClaim[" + i + "]"];
                var fscCertificate = collections["FscCertificate[" + i + "]"];
                var pallets = collections["Pallets["+i+ "]"];   
                var sheetsPerPallet = collections["SheetsPerPallet[" + i+ "]"];
                var quantity = collections["Quantity[" + i + "]"].ToString();
                var product = new ProductSupplierFormModel
                {
                    DescriptionId = int.Parse(description.ToString()),
                    GradeId = int.Parse(grade.ToString()),
                    SizeId = int.Parse(size.ToString()),
                    Unit = unit,
                    PurchasePrice = decimal.Parse(price.ToString()),
                    FscClaim = fscClaim,
                    SupplierFscCertNumber = fscCertificate,
                    Pallets = int.Parse(pallets.ToString()),
                    SheetsPerPallet = int.Parse(sheetsPerPallet.ToString()),
                    Quantity = decimal.Parse(quantity.ToString())
                };
                model.ProductList.Add(product);
            }

            foreach (var product in model.ProductList)
            {
                productService.CreateProduct(product, supplierOrderId);                             
            }
            supplierOrderService.TotalAmountAndQuantitySum(supplierOrderId);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        //public IActionResult AddProducts(int supplierOrderId)
        //{
        //    string userId = this.User.UserId();
        //    var myCompaniesUserId = myCompanyService.GetCompaniesUserId();

        //    if(!myCompaniesUserId.Contains(userId)) return BadRequest();

        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        new ProductSupplierFormModel
        //        {
        //            Descriptions = (IEnumerable<Models.Reports.ProductsStock.DescriptionForProductSearchModel>)productService.GetDescriptions(),
        //            Grades = (IEnumerable<Models.Reports.ProductsStock.GradeForProductSearchModel>)productService.GetGrades(),
        //            Sizes = (IEnumerable<Models.Reports.ProductsStock.SizeForProductSearchModel>)productService.GetSizes(),
        //            Units = productService.GetUnits()
        //        };
        //    }
        //    var count = int.Parse(TempData["Count"].ToString());
        //    ViewBag.FscCert = supplierService.GetSupplierFscCertificateByOrderId(supplierOrderId);

        //    var products = new List<ProductSupplierFormModel>();
        //    for (int i = 0; i < count; i++)
        //    {
        //        var product = new ProductSupplierFormModel()
        //        {
        //            Descriptions = (IEnumerable<Models.Reports.ProductsStock.DescriptionForProductSearchModel>)productService.GetDescriptions(),
        //            Grades = (IEnumerable<Models.Reports.ProductsStock.GradeForProductSearchModel>)productService.GetGrades(),
        //            Sizes = (IEnumerable<Models.Reports.ProductsStock.SizeForProductSearchModel>)productService.GetSizes(),
        //            Units = productService.GetUnits()
        //        };
        //        products.Add(product);
        //    }
        //    return View(products);
        //}

        //[HttpPost]
        //[Authorize]
        //public IActionResult AddProducts(
        //  int supplierOrderId, List<ProductSupplierFormModel> productmodel)
        //{
        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        var products = new List<ProductSupplierFormModel>();

        //        foreach (var item in products)
        //        {
        //            var product = new ProductSupplierFormModel()
        //            {
        //                Descriptions = (IEnumerable<Models.Reports.ProductsStock.DescriptionForProductSearchModel>)productService.GetDescriptions(),
        //                Grades = (IEnumerable<Models.Reports.ProductsStock.GradeForProductSearchModel>)productService.GetGrades(),
        //                Sizes = (IEnumerable<Models.Reports.ProductsStock.SizeForProductSearchModel>)productService.GetSizes(),
        //                Units = productService.GetUnits()
        //            };
        //            products.Add(product);
        //        }
        //    }

        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        return BadRequest();
        //    }

        //    //if (!productmodel.Any())
        //    //{
        //    //    return RedirectToAction("AddCustomerOrder", "CustomerOrdersController", customerorderId);
        //    //}
        //    foreach (var product in productmodel)
        //    {
        //        productService.CreateProduct(product, supplierOrderId);
        //        //product.SupplierFscCertNumber = supplierFscCert;              
        //    }

        //    supplierOrderService.TotalAmountAndQuantitySum(supplierOrderId);
        //    return RedirectToAction("PrintSupplierOrder", supplierOrderId);
        //}


        public IActionResult PrintSupplierOrder(int supplierOrderId)
        {

            return RedirectToAction("Index", "Home");
        }

    }
}
