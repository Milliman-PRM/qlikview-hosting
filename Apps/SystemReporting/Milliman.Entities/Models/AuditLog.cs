using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Milliman.Entities.Models
{
    [Serializable()]
    [Table("qlickviewauditlog", Schema = "public")]
    public class AuditLog
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("serverstarted")]
        public DateTime? ServerStarted { get; set; }
        [Column("timestamp")]
        public DateTime? Timestamp { get; set; }
        [Column("document")]
        public string Document { get; set; }
        [Column("eventtype")]
        public string EventType { get; set; }
        [Column("username")]
        public string UserName { get; set; }
        [Column("message")]
        public string Message { get; set; }
        [Column("isreduced")]
        public bool IsReduced { get; set; }

        [Column("fk_user_id")]//referencing the user for the FK purpose - it has to be nullable since it can be null
        public int? fk_user_id { get; set; }
        [Column("fk_group_id")]//referencing the group for the FK purpose
        public int? fk_group_id { get; set; }
        [Column("fk_report_id")]//referencing the Report for the FK purpose
        public int? fk_report_id { get; set; }

        [Column("adddate")]
        public DateTime AddDate { get; set; }

        // Foreign key 
        public virtual User User { get; set; }
        public virtual Report Report { get; set; }
        public virtual Group Group { get; set; }
    }
}
