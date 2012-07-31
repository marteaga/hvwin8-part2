using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Samples.HvMvc.Models
{
    public class HVDbContext : DbContext
    {
        public HVDbContext()
            : base("DefaultConnection")
        {
            //Database.SetInitializer<HVDbContext>(new DropCreateDatabaseIfModelChanges<HVDbContext>()); 
        }

        /// <summary>
        /// The users in the system
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// The roles in the system
        /// </summary>
        public DbSet<Roles> Roles { get; set; }

        /// <summary>
        /// The roles associated with users
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Journal Entries in the system
        /// </summary>
        public DbSet<JournalEntry> JournalEntries { get; set; }

/// <summary>
/// HealthVault users
/// </summary>
public DbSet<HealthVaultUser> HealthVaultUsers { get; set; }
    }
}