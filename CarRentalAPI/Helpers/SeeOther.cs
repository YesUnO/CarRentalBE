namespace CarRentalAPI.Helpers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class SeeOther : IActionResult
    {
        private readonly string _location;

        public SeeOther(string location)
        {
            _location = location;
        }

        //public override void ExecuteResult(ActionContext context)
        //{
        //    var response = context.HttpContext.Response;
        //    response.StatusCode = StatusCodes.Status303SeeOther;
        //    response.Headers.Location = _location;
        //}

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.StatusCode = StatusCodes.Status303SeeOther;
            response.Headers.Location = _location;

             await Task.CompletedTask;
        }
    }
}
