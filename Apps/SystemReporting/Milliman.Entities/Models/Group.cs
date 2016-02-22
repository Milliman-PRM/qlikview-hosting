using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemReporting.Entities.Models
{
    [Serializable()]
    [Table("group", Schema = "public")]
    public class Group
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
                
        [Column("groupname")]
        public string GroupName { get; set; }

        [Column("groupdescription")]
        public string Description { get; set; }

        [Column("adddate")]
        public DateTime AddDate { get; set; }

        // Navigation property 
        public virtual ICollection<IisLog> ListIisLog { get; set; }
        public virtual ICollection<AuditLog> ListAuditLog { get; set; }
        public virtual ICollection<SessionLog> ListSessionLog { get; set; }

        public Group() { }
        public Group(Group g)
        {
            Id = g.Id;
            GroupName = g.GroupName;
            ListIisLog = new List<IisLog>();
            ListAuditLog = new List<AuditLog>();
            ListSessionLog = new List<SessionLog>();
        }
    }
}
