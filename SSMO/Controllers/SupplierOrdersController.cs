using Microsoft.AspNetCore.Mvc;

namespace SSMO.Controllers
{
    public class SupplierOrdersController : Controller
    {
        public IActionResult AddSupplierConfirmation()
        {
            return View();
        }
    }
}
