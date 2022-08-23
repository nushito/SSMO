using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Data;
using SSMO.Models.Products;
using SSMO.Models.SupplierOrders;
using SSMO.Services;
using SSMO.Services.Customer;
using SSMO.Services.CustomerOrderService;
using SSMO.Services.MyCompany;
using SSMO.Services.Products;
using SSMO.Services.Status;
using SSMO.Services.SupplierOrders;
using System.Collections.Generic;
using System.Linq;

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
 ApplicationDbContext dbContext, ICustomerOrderService cusomerOrderService,  IStatusService statusService,
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
                Statuses = statusService.GetAllStatus()

            });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult AddSupplierConfirmation(SupplierOrderFormModel model)
        {


            if (!ModelState.IsValid)
            {
                new SupplierOrderFormModel
                {
                    Currencies = currency.AllCurrency(),
                    MyCompanies = myCompanyService.GetAllCompanies(),
                    Suppliers = supplierService.GetSuppliers(),
                    Statuses = statusService.GetAllStatus()

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

            var supplierOrderId = supplierOrderService.CreateSupplierOrder(model.MyCompanyId, model.SupplierId,model.Date,
                model.Number,model.CustomerOrderNumber, model.StatusId, model.CurrencyId, model.VAT??0);

            var thisCustomerOrder = cusomerOrderService.OrderPerNumber(model.CustomerOrderNumber);
            var customerorderId = thisCustomerOrder.Id;

           return RedirectToAction("EditProductAsPerSupplier", new {customerOrderId = customerorderId, supplierOrderId = supplierOrderId} );
        }


        [HttpGet]
        public IActionResult EditProductAsPerSupplier(
            int customerorderId)
        {
            if (!ModelState.IsValid)
            {
                new ProductSupplierFormModel
                {
                    Descriptions = productService.GetDescriptions(),
                    Grades = productService.GetGrades(),
                    Sizes = productService.GetSizes()
                };
            }


            var corder = productService.Details(customerorderId);

            if (!corder.Any())
            {
                return RedirectToAction("AddCustomerOrder", "CustomerOrdersController", customerorderId);
            }

            var listProducts = new List<ProductSupplierFormModel>();

            foreach (var product in corder)
            {
              var productSupp =  new ProductSupplierFormModel
              {
                    Id = product.Id,  
                    Description = product.Description,  
                    Grade = product.Grade,
                    Size = product.Size,
                    DescriptionId = product.DescriptionId,
                    GradeId = product.GradeId,
                    SizeId = product.SizeId,
                    FSCClaim = product.FSCClaim,    
                    FSCSertificate = product.FSCSertificate,
                    Pallets = product.Pallets,
                    SheetsPerPallet = product.Pallets,
                    Descriptions = productService.GetDescriptions(),
                    Grades = productService.GetGrades(),
                    Sizes = productService.GetSizes()

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
            if (!ModelState.IsValid)
            {
                new ProductSupplierFormModel
                {
                    Descriptions = productService.GetDescriptions(),
                    Grades = productService.GetGrades(),
                    Sizes = productService.GetSizes()
                };
            }

            if (!productmodel.Any())
            {
                return RedirectToAction("AddCustomerOrder", "CustomerOrdersController", customerorderId);
            }

            foreach (var product in productmodel)
            {
              var check =  productService.EditProduct(product.Id, customerorderId, supplierOrderId,product.Description, product.Grade, product.Size 
                   , product.FSCClaim, product.FSCSertificate, product.Pallets, product.SheetsPerPallet,
                    product.PurchasePrice);

                if(!check )
                {
                    return BadRequest();
                }

            }

            supplierOrderService.TotalAmountSum(supplierOrderId);

            return RedirectToAction("PrintSupplierOrder", supplierOrderId);
        }

        public IActionResult PrintSupplierOrder(int supplierOrderId)
        {

            return RedirectToAction("Index", "Home");
        }

    }
}
