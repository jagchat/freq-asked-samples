
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Helpers
{
    public class FileProcessor
    {

        public FileProcessor(IHostingEnvironment env)
        {
            hostingEnvironment = env;
        }

        private IHostingEnvironment hostingEnvironment;


        public async Task SaveJsonToAppFolder(string appVirtualFolderPath, string fileName, string jsonContent)
        {
            var pathToFile = hostingEnvironment.ContentRootPath + appVirtualFolderPath.Replace("/", Path.DirectorySeparatorChar.ToString())
            + fileName;

            using (StreamWriter s = File.CreateText(pathToFile))
            {
                await s.WriteAsync(jsonContent);
            }

        }

        public async Task SaveJsonToWwwFolder(string virtualFolderPath, string fileName, string jsonContent)
        {
            var pathToFile = System.IO.Path.Combine(hostingEnvironment.WebRootPath , virtualFolderPath , fileName);

            using (StreamWriter s = File.CreateText(pathToFile))
            {
                await s.WriteAsync(jsonContent);
            }

        }


    }
    
}
