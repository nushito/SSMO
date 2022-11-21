using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMO.Models.ServiceOrders;
using SSMO.Services;

namespace SSMO.Controllers
{
    public class ServiceOrdersController : Controller
    {
        private readonly ISupplierService supplierService;
        public ServiceOrdersController(ISupplierService supplierService)
        {
            this.supplierService = supplierService;    
        }

        [HttpGet]
        public IActionResult AddServiceOrder()
        {
            var serviceOrderModel = new ServiceOrderFormModel
            {
                SupplierNames = supplierService.GetSupplierNames()
            };

            return View(serviceOrderModel); ;
        }
        [HttpPost]
        [Authorize]
        public IActionResult AddServiceOrder(ServiceOrderFormModel model)
        {
            return View();
        }

        public ActionResult GetPurchaseDocumentNumbers(string name)
        {

            return Json( name);
        }
    }
}
