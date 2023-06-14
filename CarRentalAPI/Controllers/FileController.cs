using Core.ControllerModels.File;
using Core.Infrastructure.Files;
using Duende.Bff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRentalAPI.Controllers;

[Route("api/[controller]")]
[BffApi]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly ILogger<FileController> _logger;

    public FileController(IFileService fileService, ILogger<FileController> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserDocumentPhoto([FromQuery] GetUserDocumentPhotoModel model)
    {
        var fileStream = await _fileService.GetUserDocumentPhoto(model.Mail, model.UserDocumentImageType);
        return new FileStreamResult(fileStream, "application/octet-stream")
        {
            FileDownloadName = model.UserDocumentImageType.ToString() + ".jpg"
        };
    }

    [HttpPost("{orderId}")]
    [Authorize(Policy = "ViewOwnOrdersPolicy", Roles = "Customer")]
    public async Task<IActionResult> PostCarReturningPhoto([FromRoute] int orderId, [FromForm] PostCarReturningPhotoModel model)
    {
        try
        {
            var loggedinUserMail = HttpContext.User.FindFirstValue(ClaimTypes.Email);

            await _fileService.SaveCarReturningPhotoAsync(model.File, orderId, model.CarReturningImageType, loggedinUserMail);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> PostUserDocumentImage([FromForm] PostUserDocumentImageModel model)
    {
        try
        {
            var loggedinUserMail = HttpContext.User.FindFirstValue(ClaimTypes.Email);

            await _fileService.SaveUserDocumentPhotoAsync(model.File, model.UserDocumentImageType, loggedinUserMail);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,ex.Message);
            return BadRequest(ex.Message);
        }
    }
}
