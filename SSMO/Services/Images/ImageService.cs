using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SSMO.Data;
using SSMO.Data.Models;
using System.IO;

namespace SSMO.Services.Images
{
    public class ImageService : IImageService
    {
        private readonly ApplicationDbContext dbContext;
        public ImageService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task NewImage(IFormFile file)
        {            
            MemoryStream ms = new MemoryStream();
            file.CopyTo(ms);
            
            var image = new Image()
            {
                ImageTitle = file.FileName,
                ImageData = ms.ToArray()                 
            };

            ms.Close();
            ms.Dispose();

            dbContext.Images.Add(image);
            await dbContext.SaveChangesAsync();
        }
    }
}
