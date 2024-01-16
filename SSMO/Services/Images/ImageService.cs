using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SSMO.Data;
using SSMO.Data.Models;
using System.IO;
using SSMO.Models.Image;
using DevExpress.Data.ODataLinq.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;


namespace SSMO.Services.Images
{
    public class ImageService : IImageService
    {
        private readonly ApplicationDbContext dbContext;
        public ImageService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public string FooterUrl(int footer)
        {
            Image img = dbContext.Images
                .Where(i => i.Id == footer)
                .FirstOrDefault();
        
           string imageBase64Data = Convert.ToBase64String(img.ImageData);
           string footerURL = string.Format("data:application/octet-stream;base64,{0}", imageBase64Data);
       
            return footerURL;
        }

        public string HeaderUrl(int header)
        {
            Image img = dbContext.Images
                .Where(i => i.Id == header)
                .FirstOrDefault();

            string imageBase64Data = Convert.ToBase64String(img.ImageData);
            string headerURL = string.Format("data:image;base64,{0}", imageBase64Data);

            return headerURL; 
        }

        public ICollection<ImageModelViewForAllDocuments> ImageCollection(int company)
        {
            return dbContext.Images
                .Where(m=>m.MyCompanyId == company)
                .Select(n=> new ImageModelViewForAllDocuments
                {
                    Id= n.Id,
                     ImageTitle= n.ImageTitle,
                      ImageData = n.ImageData,
                }).ToList();
        }

        public async Task NewImage(IFormFile file, int mycompany)
        {            
            MemoryStream ms = new MemoryStream();
            file.CopyTo(ms);
            
            var image = new Image()
            {
                ImageTitle = file.FileName,
                ImageData = ms.ToArray(),
                MyCompanyId = mycompany
            };

            ms.Close();
            ms.Dispose();

            await dbContext.Images.AddAsync(image);           
            await dbContext.SaveChangesAsync();
        }
    }
}
