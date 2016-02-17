using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemReporting.Entities.Models
{
    [Serializable()]
    [Table("user", Schema = "public")]
    public class User
    {
        // Primary key
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("username")]
        public string UserName { get; set; }

        [Column("adddate")]
        public DateTime AddDate { get; set; }

        // Navigation property 
        public virtual ICollection<IisLog> ListIisLog { get; set; }
        //public virtual IisLog IisLog { get; set; }
        public virtual ICollection<AuditLog> ListAuditLog { get; set; }
        public virtual ICollection<SessionLog> ListSessionLog { get; set; }

        public User() { }
        public User(User u)
        {
            Id = u.Id;
            UserName = u.UserName;
            ListIisLog = new List<IisLog>();
            ListAuditLog = new List<AuditLog>();
            ListSessionLog = new List<SessionLog>();
        }
    }
}
