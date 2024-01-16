using SSMO.Models.CustomerOrders;
using SSMO.Models.Documents;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Documents.Packing_List;
using System;
using System.Collections.Generic;

namespace SSMO.Services.Documents
{
    public interface IDocumentService
    {
       // public int GetLastNumInvoice();
        public string GetLastNumOrder();       
        public ICollection<int> GetBgInvoices(int myCompanyId);
        public ICollection<int> GetPackingList();
        public PackingListForPrintViewModel PackingListForPrint(int packingListNumber);

        public void CreateBgInvoice(int documentNumberId, int myCompanyId);
        public void EditBgInvoice(int documentNumber, int myCompanyId);
        public CreditAndDebitNoteViewModel PrintCreditAndDebitNote(int id);
        public List<InvoiceBankDetailsViewModel> BankDetails(int id);
        public void AddFiscalAgent
            (string name, string bgName, string details, string bgDetails,string userId);
        public ICollection<FiscalAgentViewModel> GetFiscalAgents();

    }
}
