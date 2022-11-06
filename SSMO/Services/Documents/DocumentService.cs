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
            //TODO change invoice with packing
            return dbContext.Documents
                .Where(type => type.DocumentType == Data.Enums.DocumentTypes.Invoice)
                .Select(num => num.DocumentNumber)
                .ToList();
        }

        public PackingListForPrintViewModel PackingListForPrint(int packingListNumber)
        {
            if (packingListNumber == 0) return null;
            //TODO change invoice with packing after testing
            var packingList = dbContext.Documents
                .Where(num => num.DocumentNumber == packingListNumber && num.DocumentType == Data.Enums.DocumentTypes.Invoice)
                .FirstOrDefault();

            var products = dbContext.Products
                .Where(co => co.CustomerOrderId == packingList.CustomerOrderId)
                .ToList();

            var packinglist = new PackingListForPrintViewModel
            {
                DocumentType = Data.Enums.DocumentTypes.PackingList.ToString(),
                Date = packingList.Date,
                DocumentNumber = packingListNumber,
                CustomerId = packingList.CustomerId,
                Incoterms = packingList.Incoterms,
                FSCClaim = packingList.FSCClaim,
                FSCSertificate = packingList.FSCSertificate,
                MyCompanyId = packingList.MyCompanyId,
                NetWeight = packingList.NetWeight,
                GrossWeight = packingList.GrossWeight,
                TruckNumber = packingList.TruckNumber,
            };

            var myCompany = dbContext.MyCompanies
                .Where(i=>i.Id == packingList.MyCompanyId)
                .FirstOrDefault();

            var myCompanyAddress = dbContext.Addresses
               .Where(id => id.Id == myCompany.AddressId)
               .FirstOrDefault();

            packinglist.MyCompanyForPl = new MyCompanyForPackingPrint
            {
                Name = myCompany.Name,
                EIK = myCompany.Eik,
                VAT = myCompany.VAT,
                Street = myCompanyAddress.Street,
                City = myCompanyAddress.City,
                Country = myCompanyAddress.Country
            };

          //  packinglist.Products = mapper.Map<ICollection<ProductsForPackingListPrint>>(products);
            foreach (var item in products)
            {

                packinglist.Products.Add(new ProductsForPackingListPrint
                {
                    DescriptionName = dbContext.Descriptions.Where(i => i.Id == item.DescriptionId).Select(n => n.Name).FirstOrDefault(),
                    GradeName = dbContext.Descriptions.Where(i => i.Id == item.GradeId).Select(n => n.Name).FirstOrDefault(),
                    SizeName = dbContext.Descriptions.Where(i => i.Id == item.SizeId).Select(n => n.Name).FirstOrDefault(),
                    FSCClaim = item.FSCClaim,
                    FSCSertificate = item.FSCSertificate,
                    Pallets = item.Pallets,
                    SheetsPerPallet = item.SheetsPerPallet,
                    OrderedQuantity = item.LoadedQuantityM3
                }); 
            }

            var customer = dbContext.Customers
                .Where(id => id.Id == packinglist.CustomerId)
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
