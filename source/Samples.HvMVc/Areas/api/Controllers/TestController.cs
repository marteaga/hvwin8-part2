using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using Microsoft.Health;
using Microsoft.Health.PatientConnect;
using Microsoft.Health.Web;
using Microsoft.Health.Web.Authentication;

namespace Samples.HvMvc.Areas.api.Controllers
{
    
    public class TestController : Controller
    {
        [AuthorizeRole(Roles = "Doctor")]
        public ActionResult GetAuthorizedAccounts()
        {
            var ret = new { status = "ok" };


            var appId = HealthApplicationConfiguration.Current.ApplicationId;
            var offline = new OfflineWebApplicationConnection(appId,
                WebApplicationConfiguration.HealthServiceUrl.ToString(), Guid.Empty);
            offline.Authenticate();

            var coll = PatientConnection.GetValidatedConnections(offline);

            var res = from t in coll
                      select new
                      {
                          personId = t.PersonId,
                          recordId = t.RecordId
                      };

            return Json(new { status = ret.status, results = res } , JsonRequestBehavior.AllowGet);
        }
    }
}
