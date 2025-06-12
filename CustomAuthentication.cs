using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

public class CustomAuthentication : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        //  Skip authorization if [AllowAnonymous] is present
        var endpoint = context.ActionDescriptor as ControllerActionDescriptor;
        if (endpoint != null)
        {
            var allowAnonymous = endpoint.MethodInfo
                .GetCustomAttributes(typeof(AllowAnonymousAttribute), true)
                .Any();

            if (allowAnonymous)
            {
                base.OnActionExecuting(context);
                return;
            }
        }

        //  Session-based check
        var usermail = context.HttpContext.Session.GetString("usermailAddress");
        if (string.IsNullOrEmpty(usermail))
        {
            string path = context.HttpContext.Request.Path.ToString();
            context.HttpContext.Session.SetString("ReturnUrl", path);

            context.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                { "controller", "Login" },
                { "action", "LoginIndex" }
            });
        }

        base.OnActionExecuting(context);
    }
}
