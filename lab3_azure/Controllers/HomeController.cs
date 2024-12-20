using lab3_azure.Models;
using lab3_azure.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;

namespace lab3_azure.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlobUploadService _blobUploadService;

        public HomeController(BlobUploadService blobUploadService)
        {
            _blobUploadService = blobUploadService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.BlobList = _blobUploadService.GetBlobList();
            return View();
        }
        [HttpPost]
        public IActionResult Index(IFormFile file)
        {
            ViewBag.BlobList = _blobUploadService.GetBlobList();
            var fileStream = file.OpenReadStream();
            _blobUploadService.UploadFile(file.FileName, fileStream);
            fileStream.Close();
            return View();
        }

        public IActionResult Download([FromQuery] string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File name is required.");
            }

            try
            {
                var fileStream = _blobUploadService.DownloadBlob(fileName);
                return File(fileStream, "application/octet-stream", fileName);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
