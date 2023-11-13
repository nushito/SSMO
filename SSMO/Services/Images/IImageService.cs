using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SSMO.Services.Images
{
    public interface IImageService
    {
       Task NewImage(IFormFile file);
    }
}
