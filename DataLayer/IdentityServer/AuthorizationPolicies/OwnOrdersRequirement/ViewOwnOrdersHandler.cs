using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DataLayer.IdentityServer.AuthorizationPolicies.OwnOrdersRequirement;

public class ViewOwnOrdersHandler : AuthorizationHandler<ViewOwnOrdersRequirement>
{
    private readonly ApplicationDbContext _dbContext;

    public ViewOwnOrdersHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   ViewOwnOrdersRequirement requirement)
    {
        if (context.User == null || !context.User.Identity.IsAuthenticated)
        {
            return;
        }

        int orderId; 
        if (context.Resource is HttpContext httpContext)
        {
            if (!Int32.TryParse((string)httpContext.GetRouteValue("orderId"), out orderId))
            {
                return;
            }
        }
        else
        {
            return;
        }

        // Check if the user has a claim that matches the order's customer ID
        var email = context.User.FindFirstValue(ClaimTypes.Email);
        var customer = await _dbContext.ApplicationUsers.Include(x=>x.Orders).FirstOrDefaultAsync(x => x.Email == email);
        if (customer == null || !customer.Orders.Any(x => x.Id == orderId)) 
        { 
            return; 
        }

        // If the user has a claim that matches the order's customer ID, mark the requirement as satisfied
        context.Succeed(requirement);

        return;
    }
}
