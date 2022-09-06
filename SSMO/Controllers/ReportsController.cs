using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SSMO.Data;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Reports;
using SSMO.Models.Reports.PrrobaCascadeDropDown;
using SSMO.Services;
using SSMO.Services.Customer;
using SSMO.Services.Reports;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SSMO.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportsService service;
        private readonly ICustomerService customerService;
        private readonly ISupplierService supplierService;

        public ReportsController(IReportsService service,
           ICustomerService customerService, ISupplierService supplierService)
        {
            this.service = service;
            this.customerService = customerService;
            this.supplierService = supplierService;
        }


        public IActionResult AllCustomerOrders(CustomerOrderReportAll model)
        {
            //TODO When All are selected page is empty

            var customerNames = customerService.GetCustomerNames();

            var customerOrderCollection = service.AllCustomerOrders(
                model.CustomerName,
                model.CurrentPage, CustomerOrderReportAll.CustomerOrdersPerPage);

            model.CustomerOrderCollection = customerOrderCollection;

            model.CustomerNames = customerNames;


            return View(model);
        }

        public IActionResult CustomerOrderDetails(int id)
        {
            var order = service.Details(id);

            return View(order);
        }

        public IActionResult CustomerOrderEdit(int id, CustomerOrderViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var editOrder = service.Edit
                (id, model.CustomerPoNumber,
                model.Date,
                model.CustomerId,
                model.MyCompanyId,
                model.DeliveryTerms,
                model.LoadingPlace,
                model.DeliveryAddress,
                model.CurrencyId,
                model.Status.Name, model.FscClaim, model.FscCertificate);

            if (!editOrder)
            {
                return BadRequest();
            }

            return View();
        }

        [HttpGet]
        public IActionResult CascadeMenu()
        {
            var customersList = customerService.GetCustomerNamesAndId();

            CascadeViewModel cascade = new CascadeViewModel()
            {
                Customers = customersList,
            };

           
            ViewData["Selectedsupplier"] = 0;
            cascade.ProductList = null;
            return View(cascade);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CasscadeMenu(FormCollection fc, CascadeViewModel model)
        {

            var supplierId = fc["Supplier"];
            ViewData["SelectedSupplier"] = supplierId;
            return View(model);
        }

        public JsonResult GetSupplier(int id)
        {
            //if (id == null)
            //{
            //    id = "0";
            //}

            //var customerId = int.Parse(id.ToString());

            var selectedSuppliers = supplierService.GetSuppliersNames(id);
           return Json(selectedSuppliers);
           // return Json(new SelectList(selectedSuppliers, "SupplierId", "SupplierName"));
        }
    }
}
