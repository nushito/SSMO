using System.Threading.Tasks;

namespace SSMO.Services.PDF
{
    public interface IViewRenderService
    {

        Task<string> RenderToStringAsync(string viewName, object model);

    }
}
