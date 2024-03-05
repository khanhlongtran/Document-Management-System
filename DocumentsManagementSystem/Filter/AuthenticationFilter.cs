using DocumentsManagementSystem.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DocumentsManagementSystem.Filter
{
    public class AuthenticationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Skip authentication check for actions in the Account controller
            if (context.Controller.GetType() == typeof(LoginController))
            {
                return;
            }

            // Perform authentication check for other controllers
            if (context.HttpContext.Session.GetString("userId") == null)
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }
        }
    }
}
