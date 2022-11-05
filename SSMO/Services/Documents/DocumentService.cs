using AutoMapper;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Documents.Packing_List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services.Documents
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public DocumentService(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
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
            return dbContext.Documents
                .Where(type => type.DocumentType == Data.Enums.DocumentTypes.Invoice)
                .Select(num => num.DocumentNumber)
                .ToList();
        }

        public ICollection<int> GetPackingList()
        {
            return dbContext.Documents
                .Where(type => type.DocumentType == Data.Enums.DocumentTypes.PackingList)
                .Select(num => num.DocumentNumber)
                .ToList();
        }

        public PackingListForPrintViewModel PackingListForPrint(int packingListNumber)
        {
            var packing = dbContext.Documents
                .Where(num => num.DocumentNumber == packingListNumber && num.DocumentType == Data.Enums.DocumentTypes.PackingList)
                .FirstOrDefault();

            if (packing == null) return null;

            var packinglist = new PackingListForPrintViewModel
            {
                DocumentType = Data.Enums.DocumentTypes.PackingList.ToString(),
                Date = packing.Date,
                DocumentNumber = packingListNumber,
                // CustomerId = packing.CustomerId,
                Incoterms = packing.Incoterms,
                FSCClaim = packing.FSCClaim,
                FSCSertificate = packing.FSCSertificate,
                MyCompanyId = packing.MyCompanyId,
                NetWeight = packing.NetWeight,
                GrossWeight = packing.GrossWeight,
                TruckNumber = packing.TruckNumber,
            };

            var myCompanyAddress = dbContext.Addresses
                .Where(id => id.Id == packing.MyCompany.AddressId)
                .FirstOrDefault();

            packinglist.MyCompanyForPl = new MyCompanyForPackingPrint
            {
                Name = packing.MyCompany.Name,
                EIK = packing.MyCompany.Eik,
                VAT = packing.MyCompany.VAT,
                Street = myCompanyAddress.Street,
                City = myCompanyAddress.City,
                Country = myCompanyAddress.Country
            };

            packinglist.Products = (ICollection<ProductsForPackingListPrint>)mapper.Map<ProductsForPackingListPrint>(packing.Products);

            var customer = dbContext.Customers
                .Where(id => id.Id == packing.CustomerId)
                .FirstOrDefault();

            var customerAddress = dbContext.Addresses
                .Where(id => id.Id == customer.AddressId)
                .FirstOrDefault();

            packinglist.Customer = new CustomerForPackingListPrint
            {
                Name = customer.Name,
                EIK = customer.EIK,
                VAT = customer.VAT,
                RepresentativePerson = customer.RepresentativePerson,
                ClientAddress = new AddressCustomerForPackingList
                {
                    City = customerAddress.City,
                    Country = customerAddress.Country,
                    Street = customerAddress.Street
                }
            };

            return packinglist;
        }
    }
}
