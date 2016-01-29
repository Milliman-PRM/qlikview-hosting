using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Milliman.Entities.Models
{
    [Serializable()]
    [Table("qlickviewsessionlog", Schema = "public")]
    public class SessionLog
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("document")]
        public string Document { get; set; }

        [Column("exitreason")]
        public string ExitReason { get; set; }

        [Column("sessionstarttime")]
        public DateTime? SessionStartTime { get; set; }

        [Column("sessionduration")]
        public int? SessionDuration { get; set; }
        
        [Column("sessionendreason")]
        public string SessionEndReason { get; set; }

        [Column("cpuspents")]
        public double? CpuSpentS { get; set; }

        [Column("identifyinguser")]
        public string IdentifyingUser { get; set; }

        [Column("clienttype")]
        public string ClientType { get; set; }

        [Column("clientaddress")]
        public string ClientAddress { get; set; }

        [Column("caltype")]
        public string CalType { get; set; }

        [Column("calusagecount")]
        public int? CalUsageCount { get; set; }

        [Column("browser")]
        public string Browser { get; set; }

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
