using System.Threading.Tasks;

namespace SSMO.Services.PDF
{
    public interface IHtmlToPdfConverter
    {
        byte[] Convert(string htmlCode);
        object Convert(Task<string> stringForPrint);
    }
}
