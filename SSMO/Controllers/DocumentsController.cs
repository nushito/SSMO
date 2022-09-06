using Microsoft.AspNetCore.Mvc;
using SSMO.Models.Documents.Purchase;
using SSMO.Services.Documents.Purchase;
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

        [HttpGet]
        public IActionResult PurchaseDetails(string supplierOrderNumber)
        {
            return View(new PurchaseDetailsFormModel
            {
                SupplierOrderNumber = supplierOrderNumber
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PurchaseDetails(string supplierOrderNumber, PurchaseDetailsFormModel model)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            if (string.IsNullOrEmpty(supplierOrderNumber))
            {
                return BadRequest();
            }

            if(model.GrossWeight < model.NetWeight)
            {
                return View(model);
            }

            var purchase = purchaseService.CreatePurchaseAsPerSupplierOrder(
                supplierOrderNumber, model.Number, model.Date,
                model.PaidAvance, model.DatePaidAmount, model.PaidStatus, model.NetWeight,
                model.GrossWeight, model.Duty, model.Factoring,model.CustomsExpenses, model.FiscalAgentExpenses,
                model.ProcentComission, model.PurchaseTransportCost, model.BankExpenses, model.OtherExpenses);

            if (!purchase)
            {
                return View();
            }

            return View(model);
        }


        public IActionResult Invoice()
        {
            return View();
        }

    }
}
