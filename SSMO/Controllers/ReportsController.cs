using Microsoft.AspNetCore.Mvc;
using SSMO.Models.CustomerOrders;
using SSMO.Services.Reports;

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

        public IActionResult CustomerOrderDetails(int number)
        {

            return View();
        }

        public IActionResult CustomerOrderEdit(int number)
        {

            return View();
        }
    }
}
