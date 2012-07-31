using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using Microsoft.Health;
using Samples.HvMvc.Models;

namespace Samples.HvMvc
{
    /// <summary>
    /// Class to hold the authenticated user from Health Vault for forms authentication
    /// </summary>
    public class HVPrincipal : IPrincipal
    {
        public HVPrincipal()
        {
        }

        public HVPrincipal(MembershipUser aspUser, PersonInfo pi, string authtoken)
        {
            // null checking
            if (aspUser == null)
                throw new ArgumentNullException("aspUser cannot be null");

            //create the identity
            Identity = new GenericIdentity(aspUser.UserName);

            //save some values for easy retrieving from Database
            Name = pi.Name;
            PersonId = pi.PersonId;
            UserName = pi.PersonId.ToString();
            UserId = pi.PersonId;
            AuthToken = authtoken;
            if (pi.SelectedRecord == null)
                RecordId = pi.AuthorizedRecords.FirstOrDefault().Value.Id;
            else
                RecordId = pi.SelectedRecord.Id;
        }

        /// <summary>
        /// The auth token from HealthVault
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// The name of the healthvault user since the username in asp.net membership is a guid
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The HealthVault person id 
        /// </summary>
        public Guid PersonId { get; set; }

        /// <summary>
        /// The username assigned to the user
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The user id in the system
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Generic identity object
        /// </summary>
        [ScriptIgnoreAttribute]
        public IIdentity Identity { get; set; }

        /// <summary>
        /// Determins if the user is in a role or not
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool IsInRole(string roleName)
        {
            return false;
        }

        /// <summary>
        /// Creates a generic idenity basically used when deserialized from json
        /// </summary>
        public void CreateGenericIdentity()
        {
            Identity = new GenericIdentity(UserName);
        }

        /// <summary>
        /// This is the id from the Selected record as it is different from the Person Info
        /// </summary>
        public Guid RecordId { get; set; }


    }
}