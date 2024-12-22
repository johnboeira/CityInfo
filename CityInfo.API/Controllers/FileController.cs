using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Collections.Generic;

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

    [HttpPost]
    public async Task<ActionResult> CreateFile(IFormFile file)
    {
        if (file.Length == 0 || file.Length > 20971520 || file.ContentType != "application/pdf")
        {
            return BadRequest("no file or an invalid one has been inputted.");
        }

        var path = Path.Combine(
            Directory.GetCurrentDirectory(),
            $"uploaded_file_{Guid.NewGuid()}.pdf");

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok("Your file has been uploaded successfully.");
    }
}