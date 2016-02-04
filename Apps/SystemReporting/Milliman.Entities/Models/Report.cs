using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemReporting.Entities.Models
{
    [Serializable()]
    [Table("report", Schema = "public")]
    public class Report
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("reportname")]
        public string ReportName { get; set; }

        [Column("reportdescription")]
        public string ReportDesctiption { get; set; }

        // Navigation property 
        public virtual ICollection<AuditLog> ListAuditLog { get; set; }
        public virtual ICollection<SessionLog> ListSessionLog { get; set; }

        public Report() { }
        public Report(Report r)
        {
            Id = r.Id;
            ReportName = r.ReportName;
            ListAuditLog = new List<AuditLog>();
            ListSessionLog = new List<SessionLog>();
        }
    }
}
