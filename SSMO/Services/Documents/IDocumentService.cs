using SSMO.Data.Models;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Documents;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Documents.Packing_List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services.Documents
{
    public interface IDocumentService
    {
       // public int GetLastNumInvoice();
        public string GetLastNumOrder();
       
        public ICollection<int> GetBgInvoices();

        public ICollection<int> GetPackingList();

        public ICollection<int> GetInvoiceList();

        public PackingListForPrintViewModel PackingListForPrint(int packingListNumber);

        public void CreateBgInvoice(int documentNumberId);
        public void EditBgInvoice(int documentNumber);
        public CreditAndDebitNoteViewModel PrintCreditAndDebitNote(int id);
        public List<InvoiceBankDetailsViewModel> BankDetails(int id);

        public void AddFiscalAgent(string name, string bgName, string details, string bgDetails);
        public ICollection<FiscalAgentViewModel> GetFiscalAgents();

    }
}
