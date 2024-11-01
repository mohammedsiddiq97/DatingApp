using API.Extensions;
using DatingApp.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            // Proceed with the action
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultContext.HttpContext.User.GetUserId();

            // Access the repository and update the last active time
            var repo = resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();
            var user = await repo.GetUser(userId);

            // Check if the user exists
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {userId} not found.");
            }

            user.LastActive = DateTime.UtcNow;
            await repo.SaveAll();
        }catch(Exception ex)
        {

        }

    }
}
