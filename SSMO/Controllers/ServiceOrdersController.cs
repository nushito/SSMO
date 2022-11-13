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

            return View(new ServiceOrderFormModel
            {
                SupplierNames = supplierService.GetSupplierNames()
            }); ;
        }
        [HttpPost]
        [Authorize]
        public IActionResult AddServiceOrder(ServiceOrderFormModel model)
        {
            return View();
        }

        public JsonResult GetPurchaseDocumentNumbers(string name)
        {

            return Json( name);
        }
    }
}
