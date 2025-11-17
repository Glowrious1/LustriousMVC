using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lustrious.Autenticacao
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public string? RoleAnyOf { get; set; }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var http = context.HttpContext;
            var role  = http.Session.GetString(SessionsKeys.UserRole);
            var userId = http.Session.GetString(SessionsKeys.UserId);

            if (userId == null)
            {
                context.Result = new RedirectToActionResult("Login", "Autenticacao", null);
                return;
            }
            if (!string.IsNullOrWhiteSpace(RoleAnyOf))
            {
                var allowedRoles = RoleAnyOf.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (!allowedRoles.Contains(role))
                {                    
                    context.Result = new RedirectToActionResult("AcessoNegado", "Auth", null);
                    return;
                }
            }
            base.OnActionExecuted(context);
        }
    }
}
