using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace AdminDashboard.Helpers
{
    public class PictureSettings
    {
        public static string UploadFile(IFormFile formFile, string folderName)
        {
            string folderPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Store.API", "wwwroot" ,"images", folderName);

            string fileName = $"{Guid.NewGuid()}_{formFile.FileName}";

            string filePath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                formFile.CopyTo(fileStream);
            }

            return $"/images/products/{fileName}";
        }

        public static bool DeleteFile(string filePath)
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));

            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file {filePath}: {ex.Message}");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
