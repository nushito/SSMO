using SSMO.Models.Documents.Invoice;
using SSMO.Models.Reports.Invoice;

namespace SSMO.Services
{
    public class ClientService
    {
        private static InvoiceDetailsViewModel _clientModel;
        public static void AddClient(InvoiceDetailsViewModel clientModel)
        {
            _clientModel = clientModel;
        }
        public static InvoiceDetailsViewModel GetClient()
        {
            return _clientModel;
        }
    }
}
