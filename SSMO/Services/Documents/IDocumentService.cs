using SSMO.Data.Models;
using SSMO.Models.Documents;
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

        public PackingListForPrintViewModel PackingListForPrint(int packingListNumber);

        public void CreateBgInvoice(int documentNumberId);
        public void EditBgInvoice(int documentNumber);
        public CreditAndDebitNoteViewModel PrintCreditAndDebitNote(int id);

    }
}
