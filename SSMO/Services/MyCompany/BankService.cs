
namespace SSMO.Services.MyCompany
{
    using DocumentFormat.OpenXml.Office2010.Excel;
    using SSMO.Data;
    using SSMO.Data.Models;
    using SSMO.Models.CustomerOrders;
    using SSMO.Models.Documents.Invoice;
    using System.Collections.Generic;
    using System.Linq;
  
    public class BankService : IBankService
    {
        private readonly ApplicationDbContext dbContext;
        public BankService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public int Create
            (int currencyId, string bankName, string iban, string swift, string address, string companyName, int companyId)
        {

            var newBank = new BankDetails
            {
                 CurrencyId = currencyId,                
                 BankName = bankName,
                 Iban = iban,
                 Swift = swift,
                 Address = address,
                 Company = new MyCompany
                 {
                     Name = companyName,
                     Id = companyId
                 }
        };

             var company = dbContext.MyCompanies
                .Where(a => a.Name.ToLower() == companyName.ToLower())
                .FirstOrDefault();

            company.BankDetails.Add(newBank);

            dbContext.SaveChanges();
            return newBank.Id;
           
        }

        public ICollection<BankDetailsViewModel> GetMyBanks(int customerOrderId)
        {
            var companyId = dbContext.CustomerOrders
                .Where(i => i.Id == customerOrderId)
                .Select(m => m.MyCompanyId)
                .FirstOrDefault();

            var bankList = dbContext.BankDetails
                .Where(c => c.CompanyId == companyId)
                .ToList();

            var bankDetails = new List<BankDetailsViewModel>();

            foreach (var bank in bankList)
            {
                var currency = dbContext.Currencies
                    .Where(i => i.Id == bank.CurrencyId)
                    .Select(n => n.Name)
                    .FirstOrDefault();

                bankDetails.Add(new BankDetailsViewModel
                {
                    Id = bank.Id,
                    BankName = bank.BankName,
                    CurrencyName = currency,
                    CurrencyId = bank.CurrencyId,
                    Iban = bank.Iban,
                    Swift = bank.Swift,
                });
            }

            return bankDetails;
        }
    }
}
