using SSMO.Models.FscTexts;
using System.Collections.Generic;

namespace SSMO.Services.FscTextDocuments
{
    public interface IFscTextService
    {
        public void AddFscText(string textEng, string textBg);
        public ICollection<FscTextViewModel> GetAllFscTexts();
    }
}
