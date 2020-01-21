using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using WebApp.Helpers;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class LogController : Controller
    {
        private readonly IHostingEnvironment _env;
        private FileProcessor _fileProcessor;

        public LogController(IHostingEnvironment env, FileProcessor fp)
        {
            _env = env;
            _fileProcessor = fp;
        }

        public IActionResult Write()
        {
            WriteToFile();
            return Ok(null);
        }

        [HttpPost]
        [ActionName("Write")]
        public IActionResult WritePost()
        {
            WriteToFile();
            return Ok(null);
        }


        [HttpPut]
        [ActionName("Write")]
        public IActionResult WritePut()
        {
            WriteToFile();
            return Ok(null);
        }

        private void WriteToFile()
        {
            var filename = DateTime.Now.Ticks; //for now
            using (var reader = new StreamReader(Request.Body))
            {
                var body = reader.ReadToEnd();
                _fileProcessor.SaveJsonToWwwFolder("logs", $"{filename}.txt", body).Wait();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
