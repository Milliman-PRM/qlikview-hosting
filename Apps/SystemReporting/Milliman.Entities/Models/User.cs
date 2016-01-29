using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Milliman.Entities.Models
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

        // Navigation property 
        public virtual ICollection<IisLog> IisLog { get; set; }
        public virtual ICollection<AuditLog> AuditLog { get; set; }
        public virtual ICollection<SessionLog> SessionLog { get; set; }
    }
}
