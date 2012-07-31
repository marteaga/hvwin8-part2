using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Providers;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using Samples.HvMvc.Models;

namespace Samples.HvMvc
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // set the certification file name so the HealthVault Sdk can find it
            ConfigurationManager.AppSettings["ApplicationCertificateFileName"] = Server.MapPath(ConfigurationManager.AppSettings["ApplicationCertificateFileName"]);

            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                HVPrincipal hvPrin = serializer.Deserialize<HVPrincipal>(authTicket.UserData);
                if (hvPrin != null)
                {
                    hvPrin.CreateGenericIdentity();
                    HttpContext.Current.User = hvPrin;
                    Context.User = hvPrin;
                    Thread.CurrentPrincipal = hvPrin;
                }
            }
        }

    }

}