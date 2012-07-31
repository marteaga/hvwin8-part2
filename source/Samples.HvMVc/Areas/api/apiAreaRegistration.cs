using System.Web.Mvc;

namespace Samples.HvMvc.Areas.api
{
    public class apiAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "api";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "api_default",
                "api/v1/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
