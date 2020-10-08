using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CFB.Application.Services
{
    public interface IStorageService
    {
        Task UploadAsync(IFormFile formFile, string nameWithoutExtension = null);
        Task DeleteAsync(string fileName);
    }

    public class StorageService : IStorageService
    {
        private readonly IHostingEnvironment _env;

        public StorageService(IHostingEnvironment env)
        {
            _env = env;
        }

        public Task DeleteAsync(string fileName)
        {
            var path = Path.Combine(_env.WebRootPath, "uploads").ToLower();
            var file = Directory.GetFiles(path)
                .FirstOrDefault(f => f.Contains(fileName));

            if (file != null)
            {
                File.Delete(file);
            }

            return Task.CompletedTask;
        }

        public async Task UploadAsync(IFormFile formFile, string nameWithoutExtension = null)
        {
            var extension = Path.GetExtension(formFile.FileName);
            var fileName = $"{nameWithoutExtension ?? Guid.NewGuid().ToString()}{extension}";
            var path = Path.Combine(_env.WebRootPath, "uploads").ToLower();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fullFileLocation = Path.Combine(path, fileName).ToLower();

            using (var fileStream = new FileStream(fullFileLocation, FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream);
            } 
        }
    }
}
