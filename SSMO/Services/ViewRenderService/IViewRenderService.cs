using System.Threading.Tasks;

namespace SSMO.Services.ViewRenderService
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}
