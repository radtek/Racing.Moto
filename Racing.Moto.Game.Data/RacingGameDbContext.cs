using Racing.Moto.Game.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Game.Data
{
    public class RacingGameDbContext : DbContext
    {
        #region CTORs
        public RacingGameDbContext() : base("name=RacingGameDbContext")
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


            modelBuilder.Entity<User>().Property(e => e.Amount).HasPrecision(18, 4);

        }

        #region Logging
        void LogSql(string sql)
        {
            System.Diagnostics.Trace.TraceInformation(sql);
        }
        #endregion

        #region Entities
        //public virtual DbSet<AppConfig> AppConfig { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<PK> PK { get; set; }
        #endregion
    }
}
