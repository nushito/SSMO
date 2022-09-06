using SSMO.Services.Curruncies;
using System.Collections.Generic;

namespace SSMO.Services
{
    public interface ICurrency
    {

        public ICollection<string> GetCurrency();
        //  public int GetCurrencyId(string a);
        public IEnumerable<GetCurrencyModel> AllCurrency();
    }
}
