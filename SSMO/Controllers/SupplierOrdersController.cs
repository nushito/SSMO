using Microsoft.AspNetCore.Mvc;

namespace SSMO.Controllers
{
    public class SupplierOrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
