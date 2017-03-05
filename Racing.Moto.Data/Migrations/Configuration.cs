namespace Racing.Moto.Data.Migrations
{
    using Core.Crypto;
    using Entities;
    using Enums;
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

            #region 第一次执行
            // role
            //context.Role.AddOrUpdate(
            //  p => p.RoleName,
            //  new Role { RoleName = "管理员" },
            //  new Role { RoleName = "总代理" },
            //  new Role { RoleName = "代理" },
            //  new Role { RoleName = "会员" }
            //);

            //// config
            //context.AppConfig.AddOrUpdate(
            //  p => p.Name,
            //  new AppConfig { Name = "Racing_Opening_Seconds", Value = (60 * 8).ToString() },// 开盘时长, 按秒记录, 默认8m
            //  new AppConfig { Name = "Racing_Close_Seconds", Value = (60 * 1).ToString() }, // 封盘时长, 按秒记录, 默认1m
            //  new AppConfig { Name = "Racing_Game_Seconds", Value = "50" },// 比赛时长, 按秒记录, 50s
            //  new AppConfig { Name = "Racing_Lottery_Seconds", Value = "10" },// 开奖时长, 按秒记录, 10s

            //  new AppConfig { Name = "Rate_Admin", Value = "0.2" },// 管理员利润比率,吃二出八
            //  new AppConfig { Name = "Rate_Return", Value = "0.04" }, // 退水，总代理+代理+会员=4%
            //  new AppConfig { Name = "Rate_Rebate_A", Value = "0.04" }, // 退水，A盘
            //  new AppConfig { Name = "Rate_Rebate_B", Value = "0.03" }, // 退水，B盘
            //  new AppConfig { Name = "Rate_Rebate_C", Value = "0.02" }, // 退水，C盘
            //  new AppConfig { Name = "Rate_Rebate_MaxBetAmount", Value = "50000" }, // 退水，单注限额
            //  new AppConfig { Name = "Rate_Rebate_MaxPKAmount", Value = "100000" }, // 退水，单期限额

            //  new AppConfig { Name = "News_Announcement", Value = "1" },// 公告
            //  new AppConfig { Name = "News_Marquee", Value = "2" } // 跑马灯
            //);

            #region 倍率
            //倍率
            //var rate = 9.6M;
            //var big = 1.9M;
            //var small = 1.9M;
            //var odd = 1.9M;
            //var even = 1.9M;
            //context.Rate.AddOrUpdate(
            //  new Rate { RateType = RateType.Arena, Rank = 1, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Arena, Rank = 2, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Arena, Rank = 3, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Arena, Rank = 4, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Arena, Rank = 5, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Arena, Rank = 6, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Arena, Rank = 7, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Arena, Rank = 8, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Arena, Rank = 9, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Arena, Rank = 10, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },

            //  new Rate { RateType = RateType.Casino1, Rank = 1, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino1, Rank = 2, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino1, Rank = 3, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino1, Rank = 4, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino1, Rank = 5, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino1, Rank = 6, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino1, Rank = 7, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino1, Rank = 8, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino1, Rank = 9, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino1, Rank = 10, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },

            //  new Rate { RateType = RateType.Casino2, Rank = 1, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino2, Rank = 2, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino2, Rank = 3, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino2, Rank = 4, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino2, Rank = 5, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino2, Rank = 6, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino2, Rank = 7, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino2, Rank = 8, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino2, Rank = 9, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino2, Rank = 10, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },

            //  new Rate { RateType = RateType.Casino3, Rank = 1, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino3, Rank = 2, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino3, Rank = 3, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino3, Rank = 4, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino3, Rank = 5, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino3, Rank = 6, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino3, Rank = 7, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino3, Rank = 8, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino3, Rank = 9, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even },
            //  new Rate { RateType = RateType.Casino3, Rank = 10, Rate1 = rate, Rate2 = rate, Rate3 = rate, Rate4 = rate, Rate5 = rate, Rate6 = rate, Rate7 = rate, Rate8 = rate, Rate9 = rate, Rate10 = rate, Big = big, Small = small, Odd = odd, Even = even }

            //);
            #endregion

            #endregion

            #region 第二次执行

            //// admin 
            //context.User.AddOrUpdate(
            //  p => p.UserName,
            //  new User
            //  {
            //      UserName = "Admin",
            //      Password = CryptoUtils.Encrypt("Admin001"),
            //      Enabled = true,
            //      IsLocked = false,
            //      CreateDate = DateTime.Now,
            //      FailedPasswordAttemptWindowStart = DateTime.Now,
            //      UserExtension = new UserExtension
            //      {
            //          Amount = 10000000
            //      },
            //      UserRoles = new List<UserRole>
            //      {
            //          new UserRole { RoleId = 1 }
            //      }
            //  }
            //);

            ////menu
            //context.Menu.AddOrUpdate(
            //  p => p.MenuName,
            //  new Menu { MenuName = "速度与激情", MenuUrl = "/Manage/Rule", Visible = true, DisplayOrder = 1, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } },
            //  new Menu { MenuName = "重庆时时彩", MenuUrl = "javascript:;", Visible = true, DisplayOrder = 2, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } },
            //  new Menu { MenuName = "广东快乐十分", MenuUrl = "javascript:;", Visible = true, DisplayOrder = 3, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } },
            //  new Menu { MenuName = "江苏骰子", MenuUrl = "javascript:;", Visible = true, DisplayOrder = 4, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } },
            //  new Menu { MenuName = "幸运农场", MenuUrl = "javascript:;", Visible = true, DisplayOrder = 5, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } },
            //  new Menu { MenuName = "快乐8", MenuUrl = "javascript:;", Visible = true, DisplayOrder = 6, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } }
            //);

            #endregion

            #region 第三次执行
            //// 站内消息
            //context.Post.AddOrUpdate(
            //  p => p.Title,
            //  new Post { Title = "公告", PostContent = "公告", PostStatus = Enums.PostStatus.Pass, CreateTime = DateTime.Now, UserId = 1 },
            //  new Post { Title = "跑马灯", PostContent = "跑马灯", PostStatus = Enums.PostStatus.Pass, CreateTime = DateTime.Now, UserId = 1 }
            //);

            ////menu
            //var menu = context.Menu.Where(m => m.MenuName == "速度与激情").FirstOrDefault();
            //context.Menu.AddOrUpdate(
            //  p => p.MenuName,
            //  new Menu { MenuName = "信用资料", MenuUrl = "/Manage/Credit", Visible = true, DisplayOrder = 1, ParentMenuId = menu.MenuId, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } },
            //  new Menu { MenuName = "修改密码", MenuUrl = "/Manage/ChangePassword", Visible = true, DisplayOrder = 2, ParentMenuId = menu.MenuId, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } },
            //  new Menu { MenuName = "未结明细", MenuUrl = "/Manage/Outstanding", Visible = true, DisplayOrder = 3, ParentMenuId = menu.MenuId, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } },
            //  new Menu { MenuName = "今日已结", MenuUrl = "/Manage/Payment", Visible = true, DisplayOrder = 4, ParentMenuId = menu.MenuId, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } },
            //  new Menu { MenuName = "规则", MenuUrl = "/Manage/Rule", Visible = true, DisplayOrder = 5, ParentMenuId = menu.MenuId, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } }
            //);
            #endregion
        }
    }
}
