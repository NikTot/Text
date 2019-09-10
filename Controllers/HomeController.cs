using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FTS.Models;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using System.Linq;

namespace FTS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Index page says hello");
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "Files/");
            var file = dir.GetFiles().ToList();
            return View(file);
        }

        public ActionResult List()
        {
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "Files/");
            var file = dir.GetFiles().ToList();
            return PartialView("_List", file);
        }

        public ActionResult Delete(string Name)
        {
            var FileVirtualPath = AppDomain.CurrentDomain.BaseDirectory + "Files/" + Name;
            if (System.IO.File.Exists(FileVirtualPath))
            {
                System.IO.File.Delete(FileVirtualPath);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            var path = AppDomain.CurrentDomain.BaseDirectory + "Files/"; 
            Directory.CreateDirectory(path);
            path = path + file.FileName;

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Download(string Name)
        {
            if (Name == null)
                return Content("filename not present");

            //var FileVirtualPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\" + Name;
            //return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
            var path = AppDomain.CurrentDomain.BaseDirectory + "Files\\" + Name;

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {               
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},                
                {".csv", "text/csv"}
            };
        }
    }
}
