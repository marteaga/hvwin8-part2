using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using Microsoft.Health;
using Microsoft.Health.Web;
using Microsoft.Health.Web.Authentication;
using Samples.HvMvc.Models;

namespace Samples.HvMvc.Areas.api.Controllers
{
    /// <summary>
    /// Account controller for doctors, primarly for win 8 app so they can access all users in the system
    /// </summary>
    public class DoctorAccountController : Controller
    {
        /// <summary>
        /// Logins a user via a http post
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns>status</returns>
        [HttpPost]
        public ActionResult Login(string name, string password)
        {
            var status = "The user name or password provided is incorrect.";
            if (Membership.ValidateUser(name, password))
            {
                FormsAuthentication.SetAuthCookie(name, true);
                status = "ok";
            }

            // If we got this far, something failed, redisplay form
            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Signs out the currently signed in user
        /// </summary>
        /// <returns></returns>
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Helper method for WinJS app to check authorization
        /// </summary>
        /// <param name="val"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [AuthorizeRole(Roles = "Doctor")]
        [HttpPost]
        public ActionResult Ping()
        {
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets a list of users in the system
        /// </summary>
        /// <returns></returns>
        [AuthorizeRole(Roles = "Doctor")]
        public ActionResult GetUserList()
        {
            var ret = new { status = "ok" };

            // get a list of users
            var context = new HVDbContext();
            var users = (from t in context.HealthVaultUsers
                         select new
                         {
                             t.Id,
                             t.RecordId,
                             t.Name,
                         }).ToList();

            // compose the response
            return Json(new
            {
                status = ret.status,
                users = users.Select(a => new { a.Id, a.Name, imageUrl = HVUserImageHelper.Default.GetImageUrl(a.RecordId) }).ToList(),
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the user data stored in healthvault
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HandleError]
        public ActionResult GetUserData(int userId = -1)
        {

            // just do a basic check
            if (userId == -1)
                return Json(new { status = "error", msg = "userId not sent" }, JsonRequestBehavior.AllowGet);

            // try to find the user
            var context = new HVDbContext();
            var user = (from t in context.HealthVaultUsers
                        where t.Id == userId
                        select t).FirstOrDefault();

            // if no user is found return error
            if (user == null)
                return Json(new { status = "error", msg = "userId not found" }, JsonRequestBehavior.AllowGet);

            // extract the token and make the request to health vault for all the data
            var authToken = user.WCToken;

            // register the type in the HV SDK
            ItemTypeManager.RegisterTypeHandler(HVJournalEntry.TypeId, typeof(HVJournalEntry), true);


            // create the appropriate objects for health vault
            var appId = HealthApplicationConfiguration.Current.ApplicationId;
            WebApplicationCredential cred = new WebApplicationCredential(
                appId,
                authToken,
                HealthApplicationConfiguration.Current.ApplicationCertificate);

            // setup the user
            WebApplicationConnection connection = new WebApplicationConnection(appId, cred);
            PersonInfo personInfo = null;
            try
            {
                personInfo = HealthVaultPlatform.GetPersonInfo(connection);
            }
            catch
            {
                return Json(new { status = "error", msg = "Unable to connect to HealthVault service" }, JsonRequestBehavior.AllowGet);
            }

            // get the selected record
            var authRecord = personInfo.SelectedRecord;

            // make sure there is a record returned
            if (authRecord == null)
                return Json(new { status = "error", msg = "cannot get selected record" }, JsonRequestBehavior.AllowGet);

            // before we add make sure we still have permission to read
            var result = authRecord.QueryPermissionsByTypes(new List<Guid>() { HVJournalEntry.TypeId }).FirstOrDefault();
            if (!result.Value.OnlineAccessPermissions.HasFlag(HealthRecordItemPermissions.Read))
                return Json(new { status = "error", msg = "unable to create record as no permission is given from health vault" }, JsonRequestBehavior.AllowGet);

            // search hv for the records
            HealthRecordSearcher searcher = authRecord.CreateSearcher();
            HealthRecordFilter filter = new HealthRecordFilter(HVJournalEntry.TypeId);
            searcher.Filters.Add(filter);
            HealthRecordItemCollection entries = searcher.GetMatchingItems()[0];
            var ret = entries.Cast<HVJournalEntry>().ToList().Select(t => t.JournalEntry);

            return Json(new { status = "ok", data = ret }, JsonRequestBehavior.AllowGet);
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

#if DEBUG
        /// <summary>
        /// If you deploy this to production you should use ssl as passwords will be plain text
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ActionResult CreateDoctorUser(string name, string password, string email)
        {
            MembershipCreateStatus status;
            var user = Membership.CreateUser(name, password, email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out status);

            if (status == MembershipCreateStatus.Success)
            {
                return Json(new { status = status.ToString() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = ErrorCodeToString(status) }, JsonRequestBehavior.AllowGet);
            }
        }

        private const string DOCTOR_ROLE = "Doctor";
        public ActionResult AddUserToDoctorRole(string username)
        {
            // enable rolemanager to use this
            if (!System.Web.Security.Roles.RoleExists("Doctor"))
                System.Web.Security.Roles.CreateRole("Doctor");

            try
            {
                System.Web.Security.Roles.AddUserToRole(username, DOCTOR_ROLE);
                return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
#endif
    }
}
