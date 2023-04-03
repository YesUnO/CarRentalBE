using CarRentalAPI.Models.File;
using Core.Infrastructure.Files;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController:ControllerBase
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> GetUserDocumentPhoto([FromQuery] GetUserDocumentPhotoModel model)
    {
        var fileStream = await _fileService.GetUserDocumentPhoto(model.UserName,model.UserDocumentImageType);
        return new FileStreamResult(fileStream, "application/octet-stream")
        {
            FileDownloadName = model.UserDocumentImageType.ToString() + ".jpg"
        };
    }

    [HttpPost("{orderId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ViewOwnOrdersPolicy")]
    public async Task<IActionResult> PostCarReturningPhoto([FromRoute] int orderId, [FromForm] PostCarReturningPhotoModel model)
    {
        await _fileService.SaveCarReturningPhotoAsync(model.File, orderId, model.CarReturningImageType);
        return Ok();
        //return BadRequest();
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Customer")]
    public async Task<IActionResult> PostUserDocumentImage([FromForm] PostUserDocumentImageModel model)
    {
        await _fileService.SaveUserDocumentPhotoAsync(model.File, model.UserDocumentImageType);
        return Ok();
        //return BadRequest();
    }
}
