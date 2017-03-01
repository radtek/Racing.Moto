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

            #region Rate
            modelBuilder.Entity<Rate>().Property(e => e.Rate1).HasPrecision(18, 4);
            modelBuilder.Entity<Rate>().Property(e => e.Rate2).HasPrecision(18, 4);
            modelBuilder.Entity<Rate>().Property(e => e.Rate3).HasPrecision(18, 4);
            modelBuilder.Entity<Rate>().Property(e => e.Rate4).HasPrecision(18, 4);
            modelBuilder.Entity<Rate>().Property(e => e.Rate5).HasPrecision(18, 4);
            modelBuilder.Entity<Rate>().Property(e => e.Rate6).HasPrecision(18, 4);
            modelBuilder.Entity<Rate>().Property(e => e.Rate7).HasPrecision(18, 4);
            modelBuilder.Entity<Rate>().Property(e => e.Rate8).HasPrecision(18, 4);
            modelBuilder.Entity<Rate>().Property(e => e.Rate9).HasPrecision(18, 4);
            modelBuilder.Entity<Rate>().Property(e => e.Rate10).HasPrecision(18, 4);
            modelBuilder.Entity<Rate>().Property(e => e.Big).HasPrecision(18, 4);
            modelBuilder.Entity<Rate>().Property(e => e.Small).HasPrecision(18, 4);
            modelBuilder.Entity<Rate>().Property(e => e.Odd).HasPrecision(18, 4);
            modelBuilder.Entity<Rate>().Property(e => e.Even).HasPrecision(18, 4);
            #endregion

            #region PKRate
            modelBuilder.Entity<PKRate>().Property(e => e.Rate).HasPrecision(18, 4);
            //modelBuilder.Entity<PKRate>().Property(e => e.Rate1).HasPrecision(18, 4);
            //modelBuilder.Entity<PKRate>().Property(e => e.Rate2).HasPrecision(18, 4);
            //modelBuilder.Entity<PKRate>().Property(e => e.Rate3).HasPrecision(18, 4);
            //modelBuilder.Entity<PKRate>().Property(e => e.Rate4).HasPrecision(18, 4);
            //modelBuilder.Entity<PKRate>().Property(e => e.Rate5).HasPrecision(18, 4);
            //modelBuilder.Entity<PKRate>().Property(e => e.Rate6).HasPrecision(18, 4);
            //modelBuilder.Entity<PKRate>().Property(e => e.Rate7).HasPrecision(18, 4);
            //modelBuilder.Entity<PKRate>().Property(e => e.Rate8).HasPrecision(18, 4);
            //modelBuilder.Entity<PKRate>().Property(e => e.Rate9).HasPrecision(18, 4);
            //modelBuilder.Entity<PKRate>().Property(e => e.Rate10).HasPrecision(18, 4);
            //modelBuilder.Entity<PKRate>().Property(e => e.Big).HasPrecision(18, 4);
            //modelBuilder.Entity<PKRate>().Property(e => e.Small).HasPrecision(18, 4);
            //modelBuilder.Entity<PKRate>().Property(e => e.Odd).HasPrecision(18, 4);
            //modelBuilder.Entity<PKRate>().Property(e => e.Even).HasPrecision(18, 4);
            #endregion

            #region UserExtension
            modelBuilder.Entity<UserExtension>().Property(e => e.Amount).HasPrecision(18, 4);
            #endregion

            modelBuilder.Entity<Bet>().Property(e => e.Amount).HasPrecision(18, 4);
            modelBuilder.Entity<PKBonus>().Property(e => e.Amount).HasPrecision(18, 4);

            #region Rebate
            modelBuilder.Entity<UserRebate>().Property(e => e.RebateTypeA).HasPrecision(18, 4);
            modelBuilder.Entity<UserRebate>().Property(e => e.RebateTypeB).HasPrecision(18, 4);
            modelBuilder.Entity<UserRebate>().Property(e => e.RebateTypeC).HasPrecision(18, 4);
            modelBuilder.Entity<UserRebate>().Property(e => e.MaxPKAmount).HasPrecision(18, 4);
            modelBuilder.Entity<UserRebate>().Property(e => e.MaxBetAmount).HasPrecision(18, 4);
            #endregion

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
        public virtual DbSet<UserExtension> UserExtension { get; set; }
        public virtual DbSet<UserRebate> UserRebate { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Rate> Rate { get; set; }
        public virtual DbSet<PK> PK { get; set; }
        public virtual DbSet<PKRate> PKRate { get; set; }
        public virtual DbSet<Bet> Bet { get; set; }
        public virtual DbSet<BetItem> BetItem { get; set; }
        public virtual DbSet<PKBonus> PKBonus { get; set; }
        public virtual DbSet<Post> Post { get; set; }
        public virtual DbSet<PKLog> Log { get; set; }

        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<MenuRole> MenuRole { get; set; }

        public virtual DbSet<LoginLog> LoginLog { get; set; }
        #endregion
    }
}
