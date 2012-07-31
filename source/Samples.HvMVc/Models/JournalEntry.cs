using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Samples.HvMvc.Models
{
    /// <summary>
    /// Journal entry in the system
    /// </summary>
    public class JournalEntry
    {
        public JournalEntry()
        {
            RecordId = Guid.Empty;
            UserId = Guid.Empty;
            Created = DateTime.Now.Date;
            EntryDate = DateTime.Now.Date;
            Latitude = -1;
            Longitude = -1;
        }

        /// <summary>
        /// The id of the journal entry
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// the Guid user id assocociated with the record
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The health vault record ID this entry is assocociated with
        /// </summary>
        [DisplayName("Create Record For: ")]
        [Description("The current selected HealthVault record the journal entry will be assigned to")]
        public Guid RecordId { get; set; }

        /// <summary>
        /// Determins if person missed school or work
        /// </summary>
        [DisplayName("Did you miss school or work?")]
        public bool MissedWorkSchool { get; set; }

        /// <summary>
        /// Determins if user saw a doctor
        /// </summary>
        [DisplayName("Did you see a doctor?")]
        public bool SawDoctor { get; set; }

        /// <summary>
        /// Determines if user went to the emergency clinic because of symptons
        /// </summary>
        [DisplayName("Did you go to the emergency?")]
        public bool SawEmergency { get; set; }

        /// <summary>
        /// The peak flow reading for the entry
        /// <remarks>-1 is not entered should be a number from 0 to 500</remarks>
        /// </summary>
        [DisplayName("Enter your peak flow reading if there is one?")]
        public int PeakFlowReading { get; set; }

        /// <summary>
        /// Determines if the entry was done during the day or not
        /// </summary>
        [DisplayName("Was this during the day?")]
        public bool IsDay { get; set; }

        /// <summary>
        /// The location of the entry
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The location of the entry
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Date record was created
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Date for the entry
        /// </summary>
        [DisplayName("What date is this entry for?")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime EntryDate { get; set; }

        /// <summary>
        /// The key and record id of the linked record (or thing as HV Likes to call it) in HealthVault
        /// </summary>
        public string HvId { get; set; }
    }
}