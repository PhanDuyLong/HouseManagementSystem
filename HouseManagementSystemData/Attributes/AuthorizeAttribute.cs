using HMS.Data.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace HMS.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var accountViewModel = (CreateAccountViewModel)context.HttpContext.Items["Account"];
            if (accountViewModel == null)
            {
                // not logged in
                //context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
