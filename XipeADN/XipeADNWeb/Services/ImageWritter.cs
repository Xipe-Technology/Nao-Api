using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace XipeADNWeb.Services
{
    public class ImageWriter : IImageWriter
    {
        public async Task<string> UploadImage(IFormFile file)
        {
            if (CheckIfImageFile(file))
            {
                return await WriteFile(file);
            }
            return null;
        }

        /// <summary>
        /// Method to check if file is image file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool CheckIfImageFile(IFormFile file)
        {
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            return WriterHelper.GetImageFormat(fileBytes) != WriterHelper.ImageFormat.unknown;
        }

        /// <summary>
        /// Method to write file onto the disk
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<string> WriteFile(IFormFile file)
        {
            string fileName;
            var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
            fileName = Guid.NewGuid().ToString() + extension;
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
            using (var bits = new FileStream(path, FileMode.Create))
                await file.CopyToAsync(bits);
            return fileName;
        }
    }
}
