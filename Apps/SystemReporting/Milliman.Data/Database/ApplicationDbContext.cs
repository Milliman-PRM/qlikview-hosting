using SystemReporting.Entities.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C = SystemReporting.Utilities.Constants;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

//  IisLogs has One PK and two FK (Group and User)
//  Group has one PK, one Unique
//  User has one PK, one Unique
//  in model creation, we marked them as Optional and make them null able properties in Iislog table.
//  We created a reference in IisLog class for Group and User and in Group and User we added
//  IisLog Navigation Property
//

namespace SystemReporting.Data.Database
{   
    public class ApplicationDbContext : DbContext
    {
        #region Database
        static String ConnectionString = ConfigurationManager.ConnectionStrings["DatabaseContext"].ConnectionString;

        /// <summary>
        /// Constructor
        /// </summary>
        public ApplicationDbContext() :
            base(new NpgsqlConnection(ConnectionString), true)
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
        }

        #endregion

        #region Properties

        public DbSet<IisLog> IisLog { get; set; }
        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<SessionLog> SessionLog { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Report> Report { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<ReportType> ReportType { get; set; }


        #endregion

        /// <summary>
        /// Method is called before the models are created
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var dbSchema = C.DB_POSTGRESQL_NAMESPACE_PUBLIC;
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.HasDefaultSchema(dbSchema);

            modelBuilder.Entity<AuditLog>().ToTable("qvauditlog", dbSchema);
            modelBuilder.Entity<IisLog>().ToTable("iislog", dbSchema);
            modelBuilder.Entity<SessionLog>().ToTable("qvsessionlog", dbSchema);

            ////Foreign Keys
            modelBuilder.Entity<User>().ToTable("user", dbSchema);
            modelBuilder.Entity<Report>().ToTable("report", dbSchema);
            modelBuilder.Entity<Group>().ToTable("group", dbSchema);
            modelBuilder.Entity<ReportType>().ToTable("reporttype", dbSchema);

            modelBuilder.Entity<IisLog>().HasKey(x => x.Id).ToTable("iislog", dbSchema);
            modelBuilder.Entity<IisLog>().HasOptional(c => c.User).WithMany(d => d.ListIisLog).HasForeignKey(c => c.fk_user_id);
            modelBuilder.Entity<IisLog>().HasOptional(c => c.Group).WithMany(d => d.ListIisLog).HasForeignKey(c => c.fk_group_id);
            
            modelBuilder.Entity<AuditLog>().HasKey(x => x.Id).ToTable("qvauditlog", dbSchema);
            modelBuilder.Entity<AuditLog>().HasOptional(c => c.User).WithMany(d => d.ListAuditLog).HasForeignKey(c => c.fk_user_id);
            modelBuilder.Entity<AuditLog>().HasOptional(c => c.Group).WithMany(d => d.ListAuditLog).HasForeignKey(c => c.fk_group_id);
            modelBuilder.Entity<AuditLog>().HasOptional(c => c.Report).WithMany(d => d.ListAuditLog).HasForeignKey(c => c.fk_report_id);
            
            modelBuilder.Entity<SessionLog>().HasKey(x => x.Id).ToTable("qvsessionlog", dbSchema);
            modelBuilder.Entity<SessionLog>().HasOptional(c => c.User).WithMany(d => d.ListSessionLog).HasForeignKey(c => c.fk_user_id);
            modelBuilder.Entity<SessionLog>().HasOptional(c => c.Group).WithMany(d => d.ListSessionLog).HasForeignKey(c => c.fk_group_id);
            modelBuilder.Entity<SessionLog>().HasOptional(c => c.Report).WithMany(d => d.ListSessionLog).HasForeignKey(c => c.fk_report_id);

            base.OnModelCreating(modelBuilder);
        }
    }
}

