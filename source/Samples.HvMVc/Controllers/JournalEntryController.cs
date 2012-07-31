using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Health;
using Microsoft.Health.Web;
using Microsoft.Health.Web.Authentication;
using Samples.HvMvc.Models;

namespace Samples.HvMvc.Controllers
{
    [Authorize]
    public class JournalEntryController : Controller
    {
        private HVDbContext db = new HVDbContext();

        //
        // GET: /JournalEntry/

        public ActionResult Index()
        {
            // register the custom type
            ItemTypeManager.RegisterTypeHandler(HVJournalEntry.TypeId, typeof(HVJournalEntry), true);

            // get the user
            var hvUser = (User as HVPrincipal);
            if (hvUser != null)
            {
                // get the auth token
                var authToken = hvUser.AuthToken;

                // create the appropriate objects for health vault
                var appId = HealthApplicationConfiguration.Current.ApplicationId;
                WebApplicationCredential cred = new WebApplicationCredential(
                    appId,
                    authToken,
                    HealthApplicationConfiguration.Current.ApplicationCertificate);

                // setup the user
                WebApplicationConnection connection = new WebApplicationConnection(appId, cred);
                PersonInfo personInfo = null;
                personInfo = HealthVaultPlatform.GetPersonInfo(connection);

                // before we add make sure we still have permission to add 
                var result = personInfo.SelectedRecord.QueryPermissionsByTypes(new List<Guid>() { HVJournalEntry.TypeId }).FirstOrDefault();
                if (!result.Value.OnlineAccessPermissions.HasFlag(HealthRecordItemPermissions.Read))
                    throw new ArgumentNullException("unable to create record as no permission is given from health vault");

                // search hv for the records
                HealthRecordSearcher searcher = personInfo.SelectedRecord.CreateSearcher();
                HealthRecordFilter filter = new HealthRecordFilter(HVJournalEntry.TypeId);
                searcher.Filters.Add(filter);

                // get the matching items
                HealthRecordItemCollection entries = searcher.GetMatchingItems()[0];

                // compile a list of journalEntryItems only
                var items = entries.Cast<HVJournalEntry>().ToList();
                var ret = new List<JournalEntry>(items.Count());
                foreach (var t in items)
                {
                    var je = t.JournalEntry;
                    je.HvId = t.Key.ToString();
                    ret.Add(je);
                }

                // return the list to the view
                return View(ret);
            }
            else
            {
                // if we make it here there is nothing to display
                return View(new List<JournalEntry>(0));
            }
        }

        //
        // GET: /JournalEntry/Details/5

        public ActionResult Details(string id)
        {
            JournalEntry journalentry = db.JournalEntries.Find(id);
            if (journalentry == null)
            {
                return HttpNotFound();
            }
            return View(journalentry);
        }

        //
        // GET: /JournalEntry/Create

        public ActionResult Create()
        {
            var e = new JournalEntry()
            {
                EntryDate = DateTime.Now,
                Created = DateTime.Now,
                UserId = Guid.Parse(User.Identity.Name),
                RecordId = Guid.Parse(User.Identity.Name)
            };
            return View(e);
        }

        //
        // POST: /JournalEntry/Create

        [HttpPost]
        public ActionResult Create(JournalEntry journalentry)
        {
            if (ModelState.IsValid)
            {
                // TODO use an Azure Queue that will be monitored by a worker role
                //add the custom type for health vault
                ItemTypeManager.RegisterTypeHandler(HVJournalEntry.TypeId, typeof(HVJournalEntry), true);

                // get the authed user
                var authorizedUser = (User as HVPrincipal);
                if (authorizedUser != null)
                {
                    //get the auth token
                    var authToken = authorizedUser.AuthToken;

                    // create the appropriate objects for health vault
                    var appId = HealthApplicationConfiguration.Current.ApplicationId;
                    WebApplicationCredential cred = new WebApplicationCredential(
                        appId,
                        authToken,
                        HealthApplicationConfiguration.Current.ApplicationCertificate);

                    // setup the user
                    WebApplicationConnection connection = new WebApplicationConnection(appId, cred);
                    PersonInfo personInfo = HealthVaultPlatform.GetPersonInfo(connection);

                    // before we add make sure we still have permission to add 
                    var result = personInfo.SelectedRecord.QueryPermissionsByTypes(new List<Guid>() { HVJournalEntry.TypeId }).FirstOrDefault();
                    if (!result.Value.OnlineAccessPermissions.HasFlag(HealthRecordItemPermissions.Create))
                        throw new ArgumentNullException("unable to create record as no permission is given from health vault");

                    //Now add to the HV system
                    personInfo.SelectedRecord.NewItem(new HVJournalEntry(journalentry));

                    // redirect 
                    return RedirectToAction("Index");
                }
            }

            return View(journalentry);
        }

        //
        // GET: /JournalEntry/Edit/5

        public ActionResult Edit(string id)
        {
            return RedirectToAction("Index");
        }

        //
        // POST: /JournalEntry/Edit/5

        [HttpPost]
        public ActionResult Edit(JournalEntry journalentry)
        {
            return RedirectToAction("Index");
        }

        //
        // GET: /JournalEntry/Delete/5

        public ActionResult Delete(string id)
        {
            // create the item key 
            var t = id.Split(',');
            var key = new HealthRecordItemKey(Guid.Parse(t[0]), Guid.Parse(t[1]));

            // get the user
            var hvUser = (User as HVPrincipal);
            if (hvUser != null)
            {
                // get the auth token
                var authToken = hvUser.AuthToken;

                // create the appropriate objects for health vault
                var appId = HealthApplicationConfiguration.Current.ApplicationId;
                WebApplicationCredential cred = new WebApplicationCredential(
                    appId,
                    authToken,
                    HealthApplicationConfiguration.Current.ApplicationCertificate);

                // setup the user
                WebApplicationConnection connection = new WebApplicationConnection(appId, cred);
                PersonInfo personInfo = null;
                personInfo = HealthVaultPlatform.GetPersonInfo(connection);

                // delete the record
                personInfo.SelectedRecord.RemoveItem(key);
            }

            // redirect 
            return RedirectToAction("Index");
        }

        //
        // POST: /JournalEntry/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


    }
}