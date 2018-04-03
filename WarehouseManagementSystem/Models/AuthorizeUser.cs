using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WarehouseManagementSystem.Models
{
    public class AuthorizeUser : AuthorizeAttribute
    {
        public string Webpage { get; set; }
        public string ValidPermissionType { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            IdentityUser identityUser = Models.User.GetCurrentUserSynchronous();

            if (!isAuthorized)
            {
                return false;
            }
            Boolean hasPermission = Models.UserRoles.DoesUserHavePermission(this.Webpage, this.ValidPermissionType, identityUser.Id);
            if (hasPermission)
            {
                return true;
            }
            return false;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var context = filterContext.HttpContext;

            //if user was not logged in
            if (base.AuthorizeCore(context))
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "User",
                            action = "Login"
                        })
                    );
            }
            //if user lacked permissions
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(
                    new
                    {
                        controller = "User",
                        action = "Login"
                    })
                );
            }

        }
    }

}