using Microsoft.AspNetCore.Mvc;
using SSMO.Data;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Reports;
using SSMO.Services.Reports;
using System;
using System.Globalization;
using System.Linq;

namespace SSMO.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportsService service;
        private readonly ApplicationDbContext dbContext;

        public ReportsController(IReportsService service, ApplicationDbContext dbContext)
        {
            this.service = service;
            this.dbContext = dbContext;
        }


        public IActionResult AllCustomerOrders(CustomerOrderReportAll model)
        {

            //var orders = dbContext.CustomerOrders.AsQueryable();

            var customerNames = dbContext.Customers
                .Select(a => a.Name).ToList();

            var customerOrderCollection = service.AllCustomerOrders(
                model.CustomerName, 
                model.CurrentPage,CustomerOrderReportAll.CustomerOrdersPerPage);

            model.CustomerOrderCollection = customerOrderCollection;    

            model.CustomerNames = customerNames;

          
            return View(model);
        }

        public IActionResult CustomerOrderDetails(int id)
        {
           var order = service.Details(id);

            return View(order);
        }

        public IActionResult CustomerOrderEdit(int id,CustomerOrderViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
           
        
            var editOrder = service.Edit
                (id, model.Number,
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
    }
}
