using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace Samples.HvMvc.Models
{
    /// <summary>
    /// A user record in the database
    /// </summary>
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        //public string Name { get; set; }
    }

    /// <summary>
    /// A record in the roles table
    /// </summary>
    public class Roles
    {
        [Key]
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
    }

    /// <summary>
    /// Represents the roles for a user
    /// </summary>
    [Table("UsersInRoles")]
    public class UserRole
    {
        [Key]
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }
    }


}