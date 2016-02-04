using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemReporting.Entities.Models
{
    [Serializable()]
    [Table("qlickviewauditlog", Schema = "public")]
    public class AuditLog
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("useraccessdatetime")]
        public DateTime? UserAccessDatetime { get; set; }

        [Column("document")]
        public string Document { get; set; }

        [Column("eventtype")]
        public string EventType { get; set; }

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

        public AuditLog() { }
        public AuditLog(AuditLog a)
        {
            Id = a.Id;
            UserAccessDatetime = a.UserAccessDatetime;
            Document = a.Document;
            EventType = a.EventType;
            Message = a.Message;
            IsReduced = a.IsReduced;
            fk_user_id = a.fk_user_id;
            fk_group_id = a.fk_group_id;
            fk_report_id = a.fk_report_id;
            AddDate = a.AddDate;
            User = a.User;
            Report = a.Report;
            Group = a.Group;
        }
    }
}
