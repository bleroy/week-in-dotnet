using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Reflection;
using WeekInDotnet.Attributes;

namespace WeekInDotnet.Filters
{
    public class LoginRequiredFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDescriptor != null &&
                actionDescriptor.MethodInfo.GetCustomAttributes<LogInRequiredAttribute>().Any())
            {
                if (context.HttpContext.User.Identity.IsAuthenticated) return;

                context.Result = new RedirectResult("/login");
            }
        }
    }
}
