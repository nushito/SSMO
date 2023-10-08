
namespace SSMO.Services.MyCompany
{
   public interface IBankService
    {
        public int Create(
            int currencyId,            
            string bankName, 
            string iban, 
            string swift, 
            string address, 
            string companyName, 
            int companyId);
    }
}
