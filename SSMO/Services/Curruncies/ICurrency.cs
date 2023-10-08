using SSMO.Services.Curruncies;
using System.Collections.Generic;

namespace SSMO.Services
{
    public interface ICurrency
    {

        public ICollection<string> GetCurrencyList();
        //  public int GetCurrencyId(string a);
        public ICollection<GetCurrencyModel> AllCurrency();

        public string GetCurrency(int id);
    }
}
