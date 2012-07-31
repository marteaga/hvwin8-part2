using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Health;
using Microsoft.Health.ItemTypes;
using Samples.HvMvc.Models;

namespace Samples.HvMvc
{
    /// <summary>
    /// Journal entry for health vault
    /// </summary>
    public class HVJournalEntry : HealthRecordItem
    {
        private static Guid m_typeId = new Guid("a5033c9d-08cf-4204-9bd3-cb412ce39fc0");
        /// <summary>
        /// Type id for the recird item.  NOTE any custom type must have this type id
        /// http://msdn.microsoft.com/en-us/healthvault/bb968869
        public static new Guid TypeId { get { return m_typeId; } }

        private HealthServiceDateTime m_when;

        public HVJournalEntry()
            : base(new Guid("a5033c9d-08cf-4204-9bd3-cb412ce39fc0"))
        {
        }

        public HVJournalEntry(JournalEntry entry)
            : base(new Guid("a5033c9d-08cf-4204-9bd3-cb412ce39fc0"))
        {
            m_when = new HealthServiceDateTime(DateTime.Now);

            // clone the entry coming in without reference to object
            JournalEntry = new JournalEntry()
            {
                Created = entry.Created,
                EntryDate = entry.EntryDate,
                Id = entry.Id,
                IsDay = entry.IsDay,
                Latitude = entry.Latitude,
                Longitude = entry.Longitude,
                MissedWorkSchool = entry.MissedWorkSchool,
                PeakFlowReading = entry.PeakFlowReading,
                RecordId = entry.RecordId,
                SawDoctor = entry.SawDoctor,
                SawEmergency = entry.SawEmergency,
                UserId = entry.UserId
            };
        }

        /// <summary>
        /// Journal Entry item
        /// </summary>
        public JournalEntry JournalEntry { get; set; }

        /// <summary>
        /// The app id or name used for the xml submitted to healthvault
        /// </summary>
        internal string AppId { get { return "RBHealthJournal"; } }

        /// <summary>
        /// Summary to incoude in the xml
        /// </summary>
        internal virtual string Summary { get { return "health record item from RB Asthma Journal"; } }

        /// <summary>
        /// The format tag to use
        /// </summary>
        internal virtual string FormatTag { get { return "JournalEntryItem"; } }

        /// <summary>
        /// The element tag to use for the journal entry
        /// </summary>
        internal virtual string ElementTag { get { return "Data"; } }

        /// <summary>
        /// Writes the Xml to be save to health vault
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("app-specific");
            {
                writer.WriteStartElement("format-appid");
                writer.WriteValue(this.AppId);
                writer.WriteEndElement();
                writer.WriteStartElement("format-tag");
                writer.WriteValue("PeakZone");
                writer.WriteEndElement();
                m_when.WriteXml("when", writer);
                writer.WriteStartElement("summary");
                writer.WriteValue(this.Summary);
                writer.WriteEndElement();

                // start writing out the custom object
                writer.WriteStartElement(this.ElementTag);
                writer.WriteValue(this.SerializeEntry());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Parses the xml from the healthvault system
        /// </summary>
        /// <param name="typeSpecificXml"></param>
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator navigator = typeSpecificXml.CreateNavigator();
            navigator = navigator.SelectSingleNode("app-specific");
            XPathNavigator when = navigator.SelectSingleNode("when");
            m_when = new HealthServiceDateTime();

            m_when.ParseXml(when);

            XPathNavigator formatAppid = navigator.SelectSingleNode("format-appid");
            string appid = formatAppid.Value;

            XPathNavigator peakZone = navigator.SelectSingleNode(ElementTag);
            var data = peakZone.Value;
            this.JournalEntry = DeserializeEntry(data);
        }


        /// <summary>
        /// Serialized the journal entry
        /// </summary>
        /// <returns>json representation</returns>
        private string SerializeEntry()
        {
            if (JournalEntry == null)
                throw new ArgumentException("JournalEntry cannot be null");

            return new JavaScriptSerializer().Serialize(JournalEntry);
        }

        private JournalEntry DeserializeEntry(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("data cannot be null");

            return new JavaScriptSerializer().Deserialize<JournalEntry>(data);
        }
    }
}