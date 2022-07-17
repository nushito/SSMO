using Microsoft.AspNetCore.Mvc;
using SSMO.Models.CustomerOrders;
using SSMO.Services.Reports;
using System;
using System.Globalization;

namespace SSMO.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportsService service;

        public ReportsController(IReportsService service)
        {
            this.service = service;
        }
    
        public IActionResult AllCustomerOrders(CustomerOrderReportAll model)
        {
           

          model.CustomerOrderCollection = (System.Collections.Generic.ICollection<CustomerOrderReport>)service.AllCustomerOrders(model.Search);

           var listOfOrders = model.CustomerOrderCollection;

            return View(listOfOrders);
        }

        public IActionResult Details(int id)
        {
           var order = service.Details(id);

            return View(order);
        }

        public IActionResult Edit(int id,CustomerOrderViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
           
        
            var editOrder = service.Edit
                (id, model.Number,
                model.Date,
                model.ClientId,
                model.MyCompanyId,
                model.DeliveryTerms,
                model.LoadingPlace,
                model.DeliveryAddress,
                model.CurrencyId,
                model.Status, model.FscClaim, model.FscCertificate);

            if (!editOrder)
            {
                return BadRequest();
            }

            return View();
        }
    }
}
