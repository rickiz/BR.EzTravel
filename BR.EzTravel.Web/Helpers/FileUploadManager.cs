using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using BR.EzTravel.Web.Properties;

namespace BR.EzTravel.Web.Helpers
{
    public class FileUploadManager
    {
        public HttpPostedFileBase File { get; set; }
        public string OriginalFileName { get; set; }
        public string NewFileName { get; set; }
        public List<string> ValidFileExtensions { get; set; }
        public string UploadPath { get; set; }
        public string SavePathWithFileName { get; set; }

        public FileUploadManager(HttpPostedFileBase file) : this()
        {
            File = file;
        }

        public FileUploadManager()
        {
            ValidFileExtensions = new List<string>() { ".jpg", ".png", ".bmp", ".gif", ".tiff", ".jpeg" };

            if (Settings.Default.ImageUploadPath.StartsWith("~"))
                UploadPath = HttpContext.Current.Server.MapPath(Settings.Default.ImageUploadPath);
            else
                UploadPath = Settings.Default.ImageUploadPath;
        }

        public void Save()
        {
            if (File == null || File.ContentLength <= 0)
                return;
                //throw new ApplicationException("File not found.");

            OriginalFileName = Path.GetFileName(File.FileName);
            var oriFileNameWithoutExtension = Path.GetFileNameWithoutExtension(File.FileName);
            var extension = Path.GetExtension(OriginalFileName).ToLower();

            if (!ValidFileExtensions.Contains(extension))
                throw new ApplicationException("Invalid File extension.");

            NewFileName = string.Format("{0}_{1}{2}",
                oriFileNameWithoutExtension, DateTime.Now.ToString("yyyyMMddHHmmss"), extension);

            SavePathWithFileName = Path.Combine(UploadPath, NewFileName);

            File.SaveAs(SavePathWithFileName);
        }

        public void Delete(string fileName)
        {
            var path = Path.Combine(UploadPath, fileName);
        }

        public static string UploadAndSave(HttpPostedFileBase file)
        {
            var fileManger = new FileUploadManager(file);
            fileManger.Save();

            return fileManger.NewFileName;
        }

        public static void DeleteFile(string fileName)
        {
            try
            {
                var fileManger = new FileUploadManager();
                fileManger.Delete(fileName);
            }
            catch (Exception ex)
            {
                ex.Log();
            }            
        }
    }
}
