using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SSMO.Models.Image;

namespace SSMO.Services.Images
{
    public interface IImageService
    {
       Task NewImage(IFormFile file,int mycompany);
       ICollection<ImageModelViewForAllDocuments> ImageCollection(int company);
       string FooterUrl(int footer);
       string HeaderUrl(int header);       
    }
}
