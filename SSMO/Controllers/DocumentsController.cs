using Microsoft.AspNetCore.Mvc;
using SSMO.Models.Documents.Purchase;
using SSMO.Services.Documents;
using SSMO.Services.SupplierOrders;

namespace SSMO.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly ISupplierOrderService supplierOrderService;
        private readonly IPurchaseService purchaseService;
  
        public DocumentsController
            (ISupplierOrderService supplierOrderService, 
            IPurchaseService purchaseService)
        {
            this.supplierOrderService = supplierOrderService;
            this.purchaseService = purchaseService;
        }


        public IActionResult AddPurchase(SupplierOrderListModel model)
        {
            var suppliersList = this.supplierOrderService.GetSuppliers();

            var supplierOrdersList = this.purchaseService.GetSupplierOrders(
                model.SupplierName, model.CurrentPage, SupplierOrderListModel.SupplierOrdersPerPage);

            model.SupplierOrderNumbers = supplierOrdersList;
            model.SupplierNames = suppliersList;

            return View(model);  
        }


        public IActionResult PurchaseDetails(string number)
        {
            return View();
        }

    }
}
