using DatingApp.API.Data.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.API.Helpers
{
    public class LastActiveUserFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedContext = await next();

            var repository = executedContext.HttpContext.RequestServices
                .GetRequiredService<IDatingRepository>();

            var userId = int.Parse(executedContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var user = await repository.GetUser(userId);
            user.LastActive = DateTime.Now;

            await repository.SaveAll();
        }
    }
}
