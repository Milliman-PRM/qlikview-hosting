using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemReporting.Entities.Models
{
    [Serializable()]
    public class ReportType
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("keywords")]
        public string Keywords { get; set; }

        public ReportType() { }

        public ReportType(ReportType rt)
        {
            Type = rt.Type;
            Keywords = rt.Keywords;
        }
    }
}
