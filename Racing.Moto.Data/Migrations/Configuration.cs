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

            #region ��һ��ִ��
            // role
            //context.Role.AddOrUpdate(
            //  p => p.RoleName,
            //  new Role { RoleName = "����Ա" },
            //  new Role { RoleName = "�ܴ���" },
            //  new Role { RoleName = "����" },
            //  new Role { RoleName = "��Ա" }
            //);

            //// config
            //context.AppConfig.AddOrUpdate(
            //  p => p.Name,
            //  new AppConfig { Name = "Racing_Opening_Seconds", Value = (60 * 8).ToString() },// ����ʱ��, �����¼, Ĭ��8m
            //  new AppConfig { Name = "Racing_Close_Seconds", Value = (60 * 1).ToString() }, // ����ʱ��, �����¼, Ĭ��1m
            //  new AppConfig { Name = "Racing_Game_Seconds", Value = "50" },// ����ʱ��, �����¼, 50s
            //  new AppConfig { Name = "Racing_Lottery_Seconds", Value = "10" },// ����ʱ��, �����¼, 10s

            //  new AppConfig { Name = "Rate_Admin", Value = "0.2" },// ����Ա�������,�Զ�����
            //  new AppConfig { Name = "Rate_Return", Value = "0.04" }, // ��ˮ���ܴ���+����+��Ա=4%
            //  new AppConfig { Name = "Rate_Rebate_A", Value = "0.04" }, // ��ˮ��A��
            //  new AppConfig { Name = "Rate_Rebate_B", Value = "0.03" }, // ��ˮ��B��
            //  new AppConfig { Name = "Rate_Rebate_C", Value = "0.02" }, // ��ˮ��C��
            //  new AppConfig { Name = "Rate_Rebate_MaxBetAmount", Value = "50000" }, // ��ˮ����ע�޶�
            //  new AppConfig { Name = "Rate_Rebate_MaxPKAmount", Value = "100000" }, // ��ˮ�������޶�

            //  new AppConfig { Name = "News_Announcement", Value = "1" },// ����
            //  new AppConfig { Name = "News_Marquee", Value = "2" } // �����
            //);

            #region ����
            //����
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

            #region �ڶ���ִ��

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
            //  new Menu { MenuName = "�ٶ��뼤��", MenuUrl = "/Manage/Rule", Visible = true, DisplayOrder = 1, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } },
            //  new Menu { MenuName = "����ʱʱ��", MenuUrl = "javascript:;", Visible = true, DisplayOrder = 2, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } },
            //  new Menu { MenuName = "�㶫����ʮ��", MenuUrl = "javascript:;", Visible = true, DisplayOrder = 3, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } },
            //  new Menu { MenuName = "��������", MenuUrl = "javascript:;", Visible = true, DisplayOrder = 4, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } },
            //  new Menu { MenuName = "����ũ��", MenuUrl = "javascript:;", Visible = true, DisplayOrder = 5, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } },
            //  new Menu { MenuName = "����8", MenuUrl = "javascript:;", Visible = true, DisplayOrder = 6, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } }
            //);

            #endregion

            #region ������ִ��
            //// վ����Ϣ
            //context.Post.AddOrUpdate(
            //  p => p.Title,
            //  new Post { Title = "����", PostContent = "����", PostStatus = Enums.PostStatus.Pass, CreateTime = DateTime.Now, UserId = 1 },
            //  new Post { Title = "�����", PostContent = "�����", PostStatus = Enums.PostStatus.Pass, CreateTime = DateTime.Now, UserId = 1 }
            //);

            ////menu
            //var menu = context.Menu.Where(m => m.MenuName == "�ٶ��뼤��").FirstOrDefault();
            //context.Menu.AddOrUpdate(
            //  p => p.MenuName,
            //  new Menu { MenuName = "��������", MenuUrl = "/Manage/Credit", Visible = true, DisplayOrder = 1, ParentMenuId = menu.MenuId, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } },
            //  new Menu { MenuName = "�޸�����", MenuUrl = "/Manage/ChangePassword", Visible = true, DisplayOrder = 2, ParentMenuId = menu.MenuId, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } },
            //  new Menu { MenuName = "δ����ϸ", MenuUrl = "/Manage/Outstanding", Visible = true, DisplayOrder = 3, ParentMenuId = menu.MenuId, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } },
            //  new Menu { MenuName = "�����ѽ�", MenuUrl = "/Manage/Payment", Visible = true, DisplayOrder = 4, ParentMenuId = menu.MenuId, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } },
            //  new Menu { MenuName = "����", MenuUrl = "/Manage/Rule", Visible = true, DisplayOrder = 5, ParentMenuId = menu.MenuId, MenuRoles = new List<MenuRole> { new MenuRole { RoleId = 1 }, new MenuRole { RoleId = 2 }, new MenuRole { RoleId = 3 }, new MenuRole { RoleId = 4 } } }
            //);
            #endregion
        }
    }
}
