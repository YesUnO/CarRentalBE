using Microsoft.AspNetCore.Http;

namespace Core.ControllerModels.Car;

public class AddProfilePicToCarRequestModel
{
    public IFormFile File { get; set; }

}
