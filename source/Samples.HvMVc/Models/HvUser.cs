using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Samples.HvMvc.Models
{
    /// <summary>
    /// Represents a healthvault user
    /// </summary>
    public class HealthVaultUser
    {
        /// <summary>
        /// The Id for the record
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the healthvault user since the username in asp.net membership is a guid
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The HealthVault person id 
        /// </summary>
        public Guid PersonId { get; set; }

        /// <summary>
        /// The record id for the health person record, not the same as personId
        /// </summary>
        public Guid RecordId { get; set; }

        /// <summary>
        /// The serialized person info object
        /// </summary>
        public string PersonInfoObject { get; set; }

        /// <summary>
        /// The token returned by health vault.  
        /// </summary>
        public string WCToken { get; set; }

        /// <summary>
        /// Determins whether there is access to the record
        /// </summary>
        public string HealthRecordState { get; set; }

        
    }
}