using Racing.Moto.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data
{
    public class RacingDbContext : DbContext
    {
        #region CTORs
        public RacingDbContext() : base("name=RacingDbContext")
        {
            Configuration.ProxyCreationEnabled = false;
#if TRACE
            Database.Log = LogSql;
#endif
        }
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //modelBuilder.Entity<Container>().Property(e => e.PlanTotalRecAmount).HasPrecision(18, 4);
        }

        #region Logging
        void LogSql(string sql)
        {
            System.Diagnostics.Trace.TraceInformation(sql);
        }
        #endregion

        #region Entities
        public virtual DbSet<AppConfig> AppConfig { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Rate> Rate { get; set; }
        public virtual DbSet<PK> PK { get; set; }
        public virtual DbSet<PKUser> PKUser { get; set; }
        public virtual DbSet<Bet> Bet { get; set; }
        #endregion
    }
}
