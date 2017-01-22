namespace Racing.Moto.Data.Migrations
{
    using Core.Crypto;
    using Entities;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Racing.Moto.Data.RacingDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Racing.Moto.Data.RacingDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );


            context.Role.AddOrUpdate(
              p => p.RoleName,
              new Role { RoleName = "管理员" },
              new Role { RoleName = "总代理" },
              new Role { RoleName = "代理" },
              new Role { RoleName = "会员" }
            );

            context.User.AddOrUpdate(
              p => p.UserName,
              new User { UserName = "Admin", Password = CryptoUtils.Encrypt("Admin001"), Enabled = true, IsLocked = false, CreateDate = DateTime.Now, FailedPasswordAttemptWindowStart = DateTime.Now }
            );

            context.AppConfig.AddOrUpdate(
              p => p.Name,
              new AppConfig { Name = "Racing_Opening_Seconds", Value = (60 * 8).ToString() },// 开盘时长, 按秒记录, 默认8m
              new AppConfig { Name = "Racing_Close_Seconds", Value = (60 * 1).ToString() }, // 封盘时长, 按秒记录, 默认1m
              new AppConfig { Name = "Racing_Game_Seconds", Value = "50" },// 比赛时长, 按秒记录, 50s
              new AppConfig { Name = "Racing_Lottery_Seconds", Value = "10" },// 开奖时长, 按秒记录, 10s

              new AppConfig { Name = "Rate_Admin", Value = "0.2" },// 管理员利润比率,吃二出八
              new AppConfig { Name = "Rate_Return", Value = "0.04" } // 退水，总代理+代理+会员=4%
            );

            //倍率
            var num = 9.6M;
            var big = 1.9M;
            var small = 1.9M;
            var odd = 1.9M;
            var even = 1.9M;
            context.Rate.AddOrUpdate(
              p => p.Rank,
              new Rate { Rank = 1, Number1 = num, Number2 = num, Number3 = num, Number4 = num, Number5 = num, Number6 = num, Number7 = num, Number8 = num, Number9 = num, Number10 = num, Big = big, Small = small, Odd = odd, Even = even },
              new Rate { Rank = 2, Number1 = num, Number2 = num, Number3 = num, Number4 = num, Number5 = num, Number6 = num, Number7 = num, Number8 = num, Number9 = num, Number10 = num, Big = big, Small = small, Odd = odd, Even = even },
              new Rate { Rank = 3, Number1 = num, Number2 = num, Number3 = num, Number4 = num, Number5 = num, Number6 = num, Number7 = num, Number8 = num, Number9 = num, Number10 = num, Big = big, Small = small, Odd = odd, Even = even },
              new Rate { Rank = 4, Number1 = num, Number2 = num, Number3 = num, Number4 = num, Number5 = num, Number6 = num, Number7 = num, Number8 = num, Number9 = num, Number10 = num, Big = big, Small = small, Odd = odd, Even = even },
              new Rate { Rank = 5, Number1 = num, Number2 = num, Number3 = num, Number4 = num, Number5 = num, Number6 = num, Number7 = num, Number8 = num, Number9 = num, Number10 = num, Big = big, Small = small, Odd = odd, Even = even },
              new Rate { Rank = 6, Number1 = num, Number2 = num, Number3 = num, Number4 = num, Number5 = num, Number6 = num, Number7 = num, Number8 = num, Number9 = num, Number10 = num, Big = big, Small = small, Odd = odd, Even = even },
              new Rate { Rank = 7, Number1 = num, Number2 = num, Number3 = num, Number4 = num, Number5 = num, Number6 = num, Number7 = num, Number8 = num, Number9 = num, Number10 = num, Big = big, Small = small, Odd = odd, Even = even },
              new Rate { Rank = 8, Number1 = num, Number2 = num, Number3 = num, Number4 = num, Number5 = num, Number6 = num, Number7 = num, Number8 = num, Number9 = num, Number10 = num, Big = big, Small = small, Odd = odd, Even = even },
              new Rate { Rank = 9, Number1 = num, Number2 = num, Number3 = num, Number4 = num, Number5 = num, Number6 = num, Number7 = num, Number8 = num, Number9 = num, Number10 = num, Big = big, Small = small, Odd = odd, Even = even },
              new Rate { Rank = 10, Number1 = num, Number2 = num, Number3 = num, Number4 = num, Number5 = num, Number6 = num, Number7 = num, Number8 = num, Number9 = num, Number10 = num, Big = big, Small = small, Odd = odd, Even = even }
            );
        }
    }
}
