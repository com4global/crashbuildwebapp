using Microsoft.AspNetCore.Http;
using crasbuild.Services;
using Microsoft.AspNetCore.Mvc;

namespace crasbuild.Controllers
{
    public class ImagesController : Controller
    {
        private readonly BlobStorageService _blobStorageService;

        public ImagesController(BlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        // GET: Images/Upload
        public IActionResult Upload()
        {
            return View();
        }

        // POST: Images/Upload
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "No file uploaded.";
                return View();
            }

            var imageUrl = await _blobStorageService.UploadImageAsync(file);
            ViewBag.ImageUrl = imageUrl;
            return View();
        }

        // GET: Images/List
        //public async Task<IActionResult> List()
        //{
        //    var images = await _blobStorageService.ListImagesAsync();
        //    return View(images);
        //}

        public async Task<IActionResult> List(int page = 1, int pageSize = 5)
        {
            var allFiles = await _blobStorageService.ListImagesAsync(); // Fetch all files
            var totalFiles = allFiles.Count; // Total number of files

            var paginatedFiles = allFiles
                .Skip((page - 1) * pageSize) // Skip the items from previous pages
                .Take(pageSize) // Take only the items for the current page
                .ToList();

            // Pass data to the view with pagination metadata
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalFiles / (double)pageSize);

            return View(paginatedFiles); // Pass the paginated files
        }


    }
}
