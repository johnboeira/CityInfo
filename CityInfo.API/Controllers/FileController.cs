using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers;

[Route("api/files")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

    public FileController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
    {
        _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider
            ?? throw new ArgumentNullException(nameof(fileExtensionContentTypeProvider));
    }

    [HttpGet("{fileId}")]
    public ActionResult GetFile(string fileId)
    {
        var pathFile = "aspnet-core.pdf";

        if (!System.IO.File.Exists(pathFile))
        {
            return NotFound();
        }

        if (!_fileExtensionContentTypeProvider.TryGetContentType(pathFile, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        var bytes = System.IO.File.ReadAllBytes(pathFile);
        return File(bytes, contentType, Path.GetFileName(pathFile));
    }
}