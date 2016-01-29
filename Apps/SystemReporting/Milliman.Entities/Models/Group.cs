using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milliman.Entities.Models
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

        // Navigation property 
        public virtual ICollection<IisLog> IisLog { get; set; }
        public virtual ICollection<AuditLog> AuditLog { get; set; }
        public virtual ICollection<SessionLog> SessionLog { get; set; }
    }
}
