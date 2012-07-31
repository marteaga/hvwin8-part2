using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Samples.HvMvc.Models;

namespace Samples.HvMvc
{
    /// <summary>
    /// Basically overides the authorize attribute so we can authorize the user based on the role
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthorizeRole : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // first try and authorize the user
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            IPrincipal user = httpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                // now check to see if the user in the doctor role
                var context = new HVDbContext();

                // check to see if the role exists
                var roles = this.Roles.Split(',');
                if (roles.Length > 0)
                {
                    // find the roles in the database
                    foreach (var r in roles)
                    {
                        var role = context.Roles.Where(t => t.RoleName.Equals(r)).FirstOrDefault();
                        if (role != null)
                        {
                            var userdb = context.Users.Where(t => t.UserName.Equals(user.Identity.Name)).FirstOrDefault();
                            if (userdb != null)
                            {
                                // now find the role and user association
                                var ru = context.UserRoles.Where(t => t.RoleId.Equals(role.RoleId) && t.UserId.Equals(userdb.UserId)).FirstOrDefault();
                                if (ru != null)
                                {
                                    // we found the association so we are good
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            // if we make it here user is not authenticated
            return false;
        }
    }
}