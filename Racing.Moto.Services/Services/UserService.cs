using Racing.Moto.Core.Crypto;
using Racing.Moto.Core.Extentions;
using Racing.Moto.Data;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Enums;
using Racing.Moto.Data.Models;
using Racing.Moto.Services.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Services
{
    public class UserService : BaseServcice
    {
        public PagerResult<User> GetUsers(UserSearchModel searchModel)
        {
            using (var db = new RacingDbContext())
            {
                var query = db.User
                    .Include(nameof(User.UserExtension))
                    .Include(nameof(User.ParentUser))
                    //.Include(nameof(User.UserRoles))
                    //.Include(nameof(User.UserRoles) + "." + nameof(UserRole.Role))
                    .Where(u => u.Enabled);
                if (searchModel.FatherUserId.HasValue && searchModel.FatherUserId.Value > 0)
                {
                    query = query.Where(u => u.ParentUserId == searchModel.FatherUserId.Value);
                }
                if (searchModel.GrandFatherUserId.HasValue && searchModel.GrandFatherUserId.Value > 0)
                {
                    query = query.Where(u => u.ParentUser.ParentUserId == searchModel.GrandFatherUserId.Value);
                }
                if (searchModel.IsLocked.HasValue)
                {
                    query = query.Where(u => u.IsLocked == searchModel.IsLocked.Value);
                }
                if (!string.IsNullOrEmpty(searchModel.UserName))
                {
                    query = query.Where(u => u.UserName.Contains(searchModel.UserName));
                }

                if (searchModel.UserType != UserType.All)
                {
                    query = query.Where(u => u.UserRoles.Where(ur => ur.RoleId == searchModel.UserType).Any());
                }

                var users = query.OrderBy(u => u.UserName).Pager<User>(searchModel.PageIndex, searchModel.PageSize);

                return users;
            }
        }

        /// <summary>
        /// 判断当前登录用户是否可以查询用户列表
        ///     1.管理员无限制 
        ///     2.总代理可以查看自己下边的代理 /会员
        ///     3.代理可以查看自己下边的会员 
        /// </summary>
        /// <param name="loginUserId">登录用户</param>
        /// <param name="searchModel">查询Model</param>
        /// <returns></returns>
        public bool SearchEnabled(int loginUserId, UserSearchModel searchModel)
        {
            using (var db = new RacingDbContext())
            {
                var enabled = false;

                /*
                    ParentUserId/GrandFatherUserId:
                        1. 总代理查看代理/会员: FatherUserId == loginUserId/GrandFatherUserId == loginUserId
                        2. 代理查看会员: FatherUserId == loginUserId
                 */
                // 登录用户查询自己下边的节点
                if (searchModel.FatherUserId.HasValue && loginUserId == searchModel.FatherUserId.Value || searchModel.GrandFatherUserId.HasValue && loginUserId == searchModel.GrandFatherUserId.Value)
                {
                    enabled = true;
                }
                else
                {
                    var loginUserRoles = db.UserRole.Where(ur => ur.UserId == loginUserId).ToList();

                    var isAdmin = loginUserRoles.Any(u => u.RoleId == RoleConst.Role_Id_Admin);
                    if (isAdmin)
                    {
                        enabled = true;
                    }
                    else if (searchModel.GrandFatherUserId.HasValue && searchModel.GrandFatherUserId.Value > 0)//非管理员只能查看自己的子节点, 既ParentUserId必须有值
                    {
                        // GrandFatherUserId == loginUserId
                        var parentUser = db.User.Where(u => u.UserId == searchModel.GrandFatherUserId).FirstOrDefault();
                        if (parentUser != null && parentUser.ParentUserId == loginUserId)
                        {
                            enabled = true;
                        }
                    }
                }

                return enabled;
            }
        }

        public bool IsAdmin(int userId)
        {
            using (var db = new RacingDbContext())
            {
                return db.UserRole.Where(ur => ur.UserId == userId && ur.RoleId == RoleConst.Role_Id_Admin).Any();
            }
        }

        public List<User> GetUsers(List<string> userNames)
        {
            using (var db = new RacingDbContext())
            {
                return db.User
                    .Include(nameof(User.UserExtension))
                    .Include(nameof(User.UserRoles))
                    .Include(nameof(User.UserRoles) + "." + nameof(UserRole.Role))
                    .Where(u => userNames.Contains(u.UserName)).ToList();
            }
        }

        public User GetUserOnly(int userId)
        {
            using (var db = new RacingDbContext())
            {
                return db.User.Where(u => u.UserId == userId).FirstOrDefault();
            }
        }

        public User GetUser(int userId)
        {
            using (var db = new RacingDbContext())
            {
                return db.User
                    .Include(nameof(User.UserExtension))
                    .Include(nameof(User.UserRoles))
                    .Include(nameof(User.UserRoles) + "." + nameof(UserRole.Role))
                    .Where(u => u.UserId == userId).FirstOrDefault();
            }
        }

        public User GetUserByUserName(string userName)
        {
            using (var db = new RacingDbContext())
            {
                return db.User.Where(u => u.UserName == userName).FirstOrDefault();
            }
        }


        public void LockUser(int userId, bool locked)
        {
            using (var db = new RacingDbContext())
            {
                var dbUser = db.User.Where(u => u.UserId == userId).FirstOrDefault();
                if (dbUser != null)
                {
                    dbUser.IsLocked = locked;
                    if (locked) dbUser.LockedDate = DateTime.Now;
                    else dbUser.LockedDate = null;

                    db.SaveChanges();
                }
            }
        }

        public void EnableUser(int userId, bool enabled)
        {
            using (var db = new RacingDbContext())
            {
                var dbUser = db.User.Include(nameof(User.UserRoles)).Where(u => u.UserId == userId).FirstOrDefault();
                if (dbUser != null)
                {
                    dbUser.Enabled = enabled;

                    db.SaveChanges();


                    // 更新父User.UserExtension的AgentCount/MemberCount
                    if (enabled)
                    {
                        UpdateParentUserExtension(dbUser, dbUser.UserRoles.First().RoleId, UserOperation.Add);
                    }
                    else
                    {
                        UpdateParentUserExtension(dbUser, dbUser.UserRoles.First().RoleId, UserOperation.Delete);
                    }
                }
            }
        }

        public bool ExistUserName(string userName)
        {
            using (var db = new RacingDbContext())
            {
                return db.User.Where(u => u.UserName == userName).Any();
            }
        }

        /// <summary>
        /// 添加/修改 User
        /// </summary>
        /// <param name="roleId">角色</param>
        /// <param name="user">用户信息</param>
        public ResponseResult SaveUser(int roleId, User user, RebateType defaultRebateType, decimal? rechargeAmount)
        {
            using (var db = new RacingDbContext())
            {
                var response = new ResponseResult();

                UserOperation userOperation = user.UserId == 0 ? UserOperation.Add : UserOperation.Edit;

                if (userOperation == UserOperation.Add)
                {
                    var existUserName = db.User.Where(u => u.UserName.ToLower() == user.UserName.ToLower()).Any();

                    if (existUserName)
                    {
                        response.Success = false;
                        response.Message = MessageConst.USER_EXIST_USERNAME;
                        return response;
                    }

                    user.CreateDate = DateTime.Now;
                    user.Enabled = true;
                    user.IsLocked = user.IsLocked;
                    user.Password = CryptoUtils.Encrypt(user.Password);
                    user.FailedPasswordAttemptWindowStart = DateTime.Parse("1900-01-01");
                    user.UserRoles = new List<UserRole> { new UserRole { RoleId = roleId } };
                    user.UserRebates = UserRebateService.GetDefaultRebates();   // 退水
                    user.DefaultRebateType = defaultRebateType;// 默认退水

                    // UserExtension
                    user.UserExtension = new UserExtension
                    {
                        Amount = rechargeAmount ?? 0
                    };

                    db.User.Add(user);
                }
                else
                {
                    var dbUser = db.User
                        .Include(nameof(User.UserExtension))
                        .Include(nameof(User.UserRebates))
                        .Where(u => u.UserId == user.UserId).FirstOrDefault();
                    if (dbUser != null)
                    {
                        dbUser.ParentUserId = user.ParentUserId;
                        dbUser.UserName = user.UserName;
                        dbUser.Password = !string.IsNullOrEmpty(user.Password) ? CryptoUtils.Encrypt(user.Password) : dbUser.Password;
                        dbUser.IsLocked = user.IsLocked;
                        dbUser.UserExtension.Amount += rechargeAmount ?? 0;
                        dbUser.DefaultRebateType = defaultRebateType;// 默认退水

                        if (dbUser.UserRebates == null || dbUser.UserRebates.Count() == 0)
                        {
                            var userRebates = UserRebateService.GetDefaultRebates();// 退水
                            userRebates.ForEach(r => r.UserId = dbUser.UserId);
                            db.UserRebate.AddRange(userRebates);
                        }
                    }
                }

                db.SaveChanges();

                // 更新父User.UserExtension的AgentCount/MemberCount
                if (userOperation == UserOperation.Add)
                {
                    UpdateUserExtension(user, roleId);
                    UpdateParentUserExtension(user, roleId, UserOperation.Add);
                }

                response.Data = user;

                return response;
            }
        }

        /// <summary>
        /// 更新父User.UserExtension的AgentCount/MemberCount
        /// </summary>
        /// <param name="user">当前用户</param>
        /// <param name="roleId">当前用户的角色Id</param>
        /// <param name="userOperation">操作: 增/删/改</param>
        public void UpdateParentUserExtension(User user, int roleId, UserOperation userOperation)
        {
            using (var db = new RacingDbContext())
            {
                var parentUserExtension = db.UserExtension
                    .Include(nameof(UserExtension.User))
                    .Where(u => u.UserId == user.ParentUserId).FirstOrDefault();

                if (parentUserExtension != null)
                {
                    switch (userOperation)
                    {
                        case UserOperation.Add: //新增用户
                            switch (roleId)
                            {
                                case RoleConst.Role_Id_General_Agent:   //新增用户是总代理
                                    parentUserExtension.AgentCount = parentUserExtension.AgentCount + 1;
                                    break;
                                case RoleConst.Role_Id_Agent:   //新增用户是代理
                                    parentUserExtension.AgentCount = parentUserExtension.AgentCount + 1;
                                    break;
                                case RoleConst.Role_Id_Member:   //新增用户是会员
                                                                 //更新代理
                                    parentUserExtension.MemberCount = parentUserExtension.MemberCount + 1;
                                    //更新总代理
                                    UpdateParentUserExtension(parentUserExtension.User, RoleConst.Role_Id_Member, UserOperation.Add);
                                    break;
                            }
                            break;
                        case UserOperation.Edit: //修改用户
                            break;
                        case UserOperation.Delete: //删除用户
                            switch (roleId)
                            {
                                case RoleConst.Role_Id_General_Agent:   //删除用户是总代理
                                    parentUserExtension.AgentCount = parentUserExtension.AgentCount - 1;
                                    break;
                                case RoleConst.Role_Id_Agent:   //删除用户是代理
                                    parentUserExtension.AgentCount = parentUserExtension.AgentCount - 1;
                                    parentUserExtension.MemberCount = parentUserExtension.MemberCount - user.UserExtension.MemberCount;
                                    break;
                                case RoleConst.Role_Id_Member:   //删除用户是会员
                                    parentUserExtension.MemberCount = parentUserExtension.MemberCount - 1;
                                    //更新总代理
                                    UpdateParentUserExtension(parentUserExtension.User, RoleConst.Role_Id_Member, UserOperation.Delete);
                                    break;
                            }
                            break;

                    }

                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 更新用户扩展 UserExtension
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleId"></param>
        public void UpdateUserExtension(User user, int roleId)
        {
            using (var db = new RacingDbContext())
            {
                var userExtension = db.UserExtension.Where(u => u.UserId == user.UserId).FirstOrDefault();
                if (userExtension != null)
                {
                    switch (roleId)
                    {
                        case RoleConst.Role_Id_General_Agent:
                            userExtension.GeneralAgentUserId = user.UserId;
                            userExtension.AgentUserId = user.UserId;
                            break;
                        case RoleConst.Role_Id_Agent:
                            userExtension.GeneralAgentUserId = user.ParentUserId;
                            userExtension.AgentUserId = user.UserId;
                            break;
                        case RoleConst.Role_Id_Member:
                            userExtension.AgentUserId = user.ParentUserId;
                            if (user.ParentUserId.HasValue)
                            {
                                userExtension.GeneralAgentUserId = db.User.Where(agent => agent.UserId == user.ParentUserId).Select(u => u.ParentUserId).First();
                            }
                            break;
                    }
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 添加代理/会员时取父亲节点
        /// </summary>
        /// <param name="roleId">角色类型</param>
        public List<User> GetParentUsers(int roleId)
        {
            using (var db = new RacingDbContext())
            {
                var parentRoleId = 0;
                switch (roleId)
                {
                    case RoleConst.Role_Id_General_Agent:   //总代理
                        parentRoleId = RoleConst.Role_Id_Admin;
                        break;
                    case RoleConst.Role_Id_Agent:   //删除用户是代理
                        parentRoleId = RoleConst.Role_Id_General_Agent;
                        break;
                    case RoleConst.Role_Id_Member:   //删除用户是会员
                        parentRoleId = RoleConst.Role_Id_Agent;
                        break;
                }

                return parentRoleId > 0
                    ? db.User.Where(u => u.Enabled && u.UserRoles.Where(ur => ur.RoleId == parentRoleId).Any()).ToList()
                    : new List<User>();
            }
        }

        /// <summary>
        /// 用户资金信息
        /// </summary>
        /// <param name="userId">userId</param>
        public UserCreditModel GetUserCredit(int userId)
        {
            using (var db = new RacingDbContext())
            {
                var model = new UserCreditModel();

                // 今日消费
                model.TodayBetAmount = db.Bet.Where(b => b.UserId == userId && DbFunctions.DiffDays(b.PK.EndTime, DateTime.Now) == 0)
                    .Select(b => b.Amount).DefaultIfEmpty(0m).Sum();

                // 未开奖消费
                model.NotBonusAmount = db.Bet.Where(b => b.UserId == userId && b.PK.EndTime > DateTime.Now)
                    .Select(b => b.Amount).DefaultIfEmpty(0m).Sum();

                // 今日返点
                model.TodayRebateAmount = db.PKBonus.Where(b => b.UserId == userId && DbFunctions.DiffDays(b.PK.EndTime, DateTime.Now) == 0 && b.BonusType == BonusType.Rebate)
                    .Select(b => b.Amount).DefaultIfEmpty(0m).Sum();

                // 今日利润
                model.TodayProfitAmount = db.PKBonus.Where(b => b.UserId == userId && DbFunctions.DiffDays(b.PK.EndTime, DateTime.Now) == 0 && b.BonusType == BonusType.Bonus)
                    .Select(b => b.Amount).DefaultIfEmpty(0m).Sum();

                // 今日盈亏
                model.TodayProfitAndLossAmount = model.TodayRebateAmount + model.TodayProfitAmount - model.TodayBetAmount;

                // 退水+限额
                //model.User = new UserService().GetUser(userId);
                model.UserRebates = new UserRebateService().GetUserRebates(userId);

                return model;
            }
        }

        public void SaveEmail(int userId, string email)
        {
            using (var db = new RacingDbContext())
            {
                var user = db.User.Where(u => u.UserId == userId).First();
                user.Email = email;
                db.SaveChanges();
            }
        }

        public void ChangePassword(string userName, string password)
        {
            using (var db = new RacingDbContext())
            {
                var user = db.User.Where(u => u.UserName == userName).First();
                user.Password = CryptoUtils.Encrypt(password);
                db.SaveChanges();
            }
        }

        public int? GetParentUserId(int userId)
        {
            using (var db = new RacingDbContext())
            {
                return db.User.Where(u => u.UserId == userId).Select(u => u.ParentUserId).FirstOrDefault();
            }
        }
    }
}
