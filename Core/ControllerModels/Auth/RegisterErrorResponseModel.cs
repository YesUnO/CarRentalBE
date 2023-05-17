

namespace Core.ControllerModels.Auth
{
    public class RegisterErrorResponseModel
    {
        public IEnumerable<string>? Password { get; set; }
        public IEnumerable<string>? Username { get; set; }
        public IEnumerable<string>? Email { get; set; }
    }
}
