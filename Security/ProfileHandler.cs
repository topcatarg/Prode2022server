using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Prode2022Server.Helpers;

namespace Prode2022Server.Security;

public class ProfileHandler : AuthorizationHandler<ProfileIsAdmin>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProfileIsAdmin requirement)
    {
        var isAdmin = context.User.GetClaim(ClaimType.IsAdmin);
        if (isAdmin == "1")
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }

}

public class ProfileIsAdmin : IAuthorizationRequirement
{
    public ProfileIsAdmin() { }

}