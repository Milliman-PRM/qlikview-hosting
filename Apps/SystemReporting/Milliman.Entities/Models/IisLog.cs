using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemReporting.Entities.Models
{
    [Serializable()]
    [Table("iislog", Schema = "public")]
    public class IisLog
    {
        // Primary key 
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("useraccessdatetime")]
        public DateTime? UserAccessDatetime { get; set; }

        [Column("clientipaddress")]
        public string ClientIpAddress { get; set; }

        //[Column("username")]
        //public string UserName { get; set; }

        [Column("serveripaddress")]
        public string ServerIPAddress { get; set; }

        [Column("portnumber")]
        public int? PortNumber { get; set; }

        [Column("commandsentmethod")]
        public string CommandSentMethod { get; set; }

        [Column("stepuri")]
        public string StepURI { get; set; }

        [Column("queryuri")]
        public string QueryURI { get; set; }

        [Column("statuscode")]
        public int? StatusCode { get; set; }

        [Column("substatuscode")]
        public int? SubStatusCode { get; set; }

        [Column("win32statuscode")]
        public int? Win32StatusCode { get; set; }

        [Column("responsetime")]
        public int? ResponseTime { get; set; }

        [Column("useragent")]
        public string UserAgent { get; set; }

        [Column("clientreferer")]
        public string ClientReferer { get; set; }

        [Column("browser")]
        public string Browser { get; set; }

        [Column("eventtype")]
        public string EventType { get; set; }

        [Column("adddate")]
        public DateTime AddDate { get; set; }

        // Foreign key can be null
        [Column("fk_user_id")]
        public int? fk_user_id { get; set; }

        // Foreign key can be null
        [Column("fk_group_id")]
        public int? fk_group_id { get; set; }
        
        // Navigation properties 
        public virtual User User { get; set; }
        public virtual Group Group { get; set; }

        public IisLog() { }
        public IisLog(IisLog i)
        {
            Id = i.Id;
            UserAccessDatetime = i.UserAccessDatetime;
            ClientIpAddress = i.ClientIpAddress;
            ServerIPAddress = i.ServerIPAddress;
            PortNumber = i.PortNumber;
            CommandSentMethod = i.CommandSentMethod;
            StepURI = i.StepURI;
            QueryURI = i.QueryURI;
            StatusCode = i.StatusCode;
            SubStatusCode = i.SubStatusCode;
            Win32StatusCode = i.Win32StatusCode;
            ResponseTime = i.ResponseTime;
            UserAgent = i.UserAgent;
            ClientReferer = i.ClientReferer;
            Browser = i.Browser;
            EventType = i.EventType;
            AddDate = i.AddDate;
            fk_user_id =	i.fk_user_id;
            fk_group_id =	i.fk_group_id;
            User = i.User;
            Group = i.Group;
        }
}
}
