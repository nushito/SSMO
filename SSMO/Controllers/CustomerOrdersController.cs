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
using SSMO.Infrastructure;
using SSMO.Services.SupplierOrders;
using System.Text.Json;
using SSMO.Services.Documents;
using SSMO.Services.FscTextDocuments;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using DocumentFormat.OpenXml.InkML;
using System.Text.RegularExpressions;
using System.Text;


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
        private readonly IDocumentService documentService;
        private readonly IFscTextService fscTextService;
        public CustomerOrdersController(ISupplierService supplierService,
           ICurrency currency,
           IMycompanyService myCompanyService,
           ICustomerService customerService,
           IProductService productService, IMapper mapper,
           ICustomerOrderService cusomerOrderService,
            IReportsService reportService, IStatusService statusService,
            ISupplierOrderService supplierOrderService, IDocumentService
             documentService, IFscTextService fscTextService)
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
            this.documentService = documentService;
            this.fscTextService = fscTextService;
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
                BankDetails = customerOrderService.GetBanks(),
                SupplierOrdersBySupplier = supplierOrderService.SuppliersAndOrders(),
                FiscalAgents = documentService.GetFiscalAgents(),
                FscTexts = fscTextService.GetAllFscTexts()
            };

            return View(customerOrderDetails);
        }
      
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddCustomerOrder(CustomerOrderViewModel customermodel)
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
                    BankDetails = customerOrderService.GetBanks(),
                    Products = new List<ProductCustomerFormModel>(),
                    Statuses = statusService.GetAllStatus(),
                    SupplierOrdersBySupplier = supplierOrderService.SuppliersAndOrders(),
                    FiscalAgents = documentService.GetFiscalAgents(),
                    FscTexts = fscTextService.GetAllFscTexts()
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
                customerorderId = await customerOrderService.CreateFirstOrder
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
                                 customermodel.Vat ?? 0, customermodel.StatusId, 
                                 (List<int>)customermodel.SelectedSupplierOrders,
                                 customermodel.Comment, customermodel.ChosenBanks, 
                                 customermodel.Type, customermodel.FiscalAgentId,
                                 customermodel.DealType, customermodel.DealDescription, 
                                 customermodel.FscText, customermodel.PaymentTerms,
                                 customermodel.Eta, customermodel.Etd);
                ViewBag.NumberExist = 0;
            }
            else
            {
                customerorderId = await customerOrderService.CreateOrder
                                 (customermodel.CustomerPoNumber,
                                 customermodel.Date,
                                 customermodel.CustomerId,
                                 customermodel.MyCompanyId,
                                 customermodel.DeliveryTerms,
                                 customermodel.LoadingPlace,
                                 customermodel.DeliveryAddress,
                                 customermodel.CurrencyId,
                                 customermodel.Origin,                                 
                                 customermodel.Vat ?? 0, customermodel.StatusId, 
                                 (List<int>)customermodel.SelectedSupplierOrders,
                                 customermodel.Comment,customermodel.ChosenBanks, 
                                 customermodel.Type, customermodel.FiscalAgentId,
                                  customermodel.DealType, customermodel.DealDescription,
                                  customermodel.FscText, customermodel.PaymentTerms,
                                  customermodel.Eta, customermodel.Etd);
                ViewBag.NumberExist = 1;
            }
            return RedirectToAction("AddOrderProducts", 
                new { selectedSupplierOrders = customermodel.SelectedSupplierOrders,
                    customerorderId
                }) ;
        }

        public IActionResult AddOrderProducts
            (List<int> selectedSupplierOrders, int customerorderId)
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
                    FscCertificate = product.PurchaseFscCertificate,
                    FSCClaim = product.PurchaseFscClaim,                    
                    Pallets = product.Pallets,
                    SheetsPerPallet = product.SheetsPerPallet,
                    Descriptions = productService.GetDescriptions(),
                    Grades = productService.GetGrades(),
                    Sizes = productService.GetSizes(),
                    SupplierOrderId = product.SupplierOrderId,
                    Quantity = product.OrderedQuantity,
                    Unit = product.Unit,
                    CustomerOrderId = customerorderId,
                    QuantityAvailableForCustomerOrder = product.QuantityAvailableForCustomerOrder
                };
                listProducts.Add(productSupp);
            };
            return View(listProducts);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddOrderProducts(IEnumerable<ProductCustomerFormModel> model, int customerorderId)
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
                if(product.QuantityAvailableForCustomerOrder == 0) { continue; }
                else { product.Quantity = product.QuantityAvailableForCustomerOrder; }

                var check = await productService.CreateCustomerOrderProduct
                    (product.Id, customerorderId, product.SupplierOrderId, product.Description,
                    product.Grade, product.Size, product.FscCertificate, product.FSCClaim, product.Pallets,
                    product.SheetsPerPallet, product.SellPrice, product.Quantity, product.Unit);

                if (!check)
                {
                    return BadRequest();
                }
            }
            customerOrderService.CustomerOrderCounting(customerorderId);

            return RedirectToAction("PrintCustomerOrder",new { customerorderId });
        }

        public IActionResult PrintCustomerOrder(int customerorderId)
        {
            var printModel = customerOrderService.GetCustomerOrderPrint(customerorderId);
            ClientService.AddCustomerOrderPrint(printModel);    

            return View(printModel);
        }

        
    }
}
