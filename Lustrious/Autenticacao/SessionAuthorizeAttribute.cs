using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Lustrious.Autenticacao
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public string? RoleAnyOf { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;
            var role  = http.Session.GetString(SessionsKeys.UserRole);
            var userId = http.Session.GetInt32(SessionsKeys.UserId);

            if (userId == null)
            {
                // redireciona para a action Login no controller Auth
                context.Result = new RedirectToActionResult("Login", "Auth", null);
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
            base.OnActionExecuting(context);
        }
    }
}
