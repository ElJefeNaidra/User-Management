using Microsoft.AspNetCore.Mvc;
using SIDPSF.Common.DataAccess;
using static SIDPSF.Common.StringLocalisation.DatabaseResourceLocalisationProvider;
using static SIDPSF.Features.Administration.ConfigurationProvider;

namespace SIDPSF.Features.Administration.Document
{
    [TypeFilter(typeof(RequestAuthorisationBasicFilter))]

    public partial class DocumentController : Controller
    {
        private readonly DBContext _dbContext;
        private readonly ResourceRepository _resourceRepo;
        private readonly IHttpContextAccessor _accessor;
        private readonly SystemConfigService _configService;

        public DocumentController(DBContext dbContext, ResourceRepository resourceRepo, IHttpContextAccessor accessor, SystemConfigService configService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _resourceRepo = resourceRepo ?? throw new ArgumentNullException(nameof(resourceRepo));
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        }

        [HttpGet]
        public IActionResult Download(string Document)
        {
            var filePath = _configService.SystemPathsConfig.PathUpload + Document;

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(); // Return a 404 Not Found response if the file doesn't exist
            }

            var contentType = "application/octet-stream"; // Default to binary stream
            var extension = Path.GetExtension(Document).ToLowerInvariant();

            if (extension == ".html" || extension == ".htm")
            {
                contentType = "text/html"; // Set MIME type for HTML files
            }

            // Serve the file
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType, contentType == "application/octet-stream" ? Document : null);
        }

        [HttpGet]
        public IActionResult DownloadGeneric(string Document)
        {
            if (!System.IO.File.Exists(Document))
            {
                return NotFound(); // Return a 404 Not Found response if the file doesn't exist
            }

            var contentType = "application/octet-stream"; // Default to binary stream
            var extension = Path.GetExtension(Document).ToLowerInvariant();

            if (extension == ".html" || extension == ".htm")
            {
                contentType = "text/html"; // Set MIME type for HTML files
            }

            // Serve the file
            byte[] fileBytes = System.IO.File.ReadAllBytes(Document);
            string fileName = Path.GetFileName(Document);
            return File(fileBytes, contentType, contentType == "application/octet-stream" ? fileName : null);
        }

    }

    public class FileService
    {
        public static string SaveFile(IFormFile file, string targetDirectory)
        {
            string fileExtension = Path.GetExtension(file.FileName);
            string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            string filePath = Path.Combine(targetDirectory, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return uniqueFileName;
        }
    }
}
