using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DataLayer.IdentityServer.AuthorizationPolicies.OwnOrdersRequirement;

public class ViewOwnOrdersHandler : AuthorizationHandler<ViewOwnOrdersRequirement>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;

    public ViewOwnOrdersHandler(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
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
        var identityUser = await _userManager.FindByEmailAsync(email);
        var customer = await _dbContext.ApplicationUsers.Include(x=>x.Orders).FirstOrDefaultAsync(x => x.IdentityUser == identityUser);
        if (customer == null || !customer.Orders.Any(x => x.Id == orderId)) 
        { 
            return; 
        }

        // If the user has a claim that matches the order's customer ID, mark the requirement as satisfied
        context.Succeed(requirement);

        return;
    }
}
