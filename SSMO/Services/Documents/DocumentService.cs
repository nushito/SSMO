using SSMO.Data;
using SSMO.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services.Documents
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext dbContext;

        public DocumentService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

     

        public string GetLastNumOrder()
        {
            var n = dbContext.CustomerOrders.OrderByDescending(a => a.CustomerPoNumber).Select(a => a.CustomerPoNumber).FirstOrDefault();
            return n;
        }

       

        //public ICollection<int> GetInvoicesNumbers(string customer)
        //{
        //    return dbContext.Invoices.Where(x => x.Client.Name == customer)
        //        .Select(a=>a.Number).ToList();
        //}

       

        public ICollection<int> GetInvoices()
        {
            throw new NotImplementedException();
        }

    }
}
