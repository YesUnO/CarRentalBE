using DataLayer.Entities.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.IdentityServer.AuthorizationPolicies.OwnOrdersRequirement
{
    public class ViewOwnOrdersHandler : AuthorizationHandler<ViewOwnOrdersRequirement, Order>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public ViewOwnOrdersHandler(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       ViewOwnOrdersRequirement requirement,
                                                       Order order)
        {
            if (context.User == null || order == null)
            {
                return;
            }

            // Check if the user has a claim that matches the order's customer ID
            var identityUser = await _userManager.GetUserAsync(context.User);
            var customer = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.IdentityUser == identityUser);
            if (customer == null || customer.Id != order.Customer.Id) 
            { 
                return; 
            }

            // If the user has a claim that matches the order's customer ID, mark the requirement as satisfied
            context.Succeed(requirement);

            return;
        }
    }
}
