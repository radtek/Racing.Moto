using Racing.Moto.Core.Configurations;
using Racing.Moto.Core.Crypto;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

namespace Racing.Moto.Data.Membership
{
    public class SqlMembershipProvider : MembershipProvider
    {
        protected virtual NameValueCollection Settings { get; set; }

        /// <summary>
        /// true: hashed; false: symmetric encrypted
        /// </summary>
        protected virtual bool HashedPassword
        {
            get { return Settings["hashedPassword"] == null ? true : bool.Parse(Settings["hashedPassword"]); }
        }

        /// <summary>
        /// 0: this function is disabled; other #: account is locked after # of failed password attempts within the specified window
        /// </summary>
        protected virtual int FailedPasswordAttemptCount
        {
            get { return Settings["failedPasswordAttemptCount"] == null ? 0 : int.Parse(Settings["failedPasswordAttemptCount"]); }
        }

        /// <summary>
        /// time window for failed password attempts (in minutes).
        /// </summary>
        protected virtual int FailedPasswordAttemptWindow
        {
            get { return Settings["failedPasswordAttemptWindow"] == null ? 0 : int.Parse(Settings["failedPasswordAttemptWindow"]); }
        }

        public override void Config(NameValueCollection settings)
        {
            Settings = settings;
        }

        public static MembershipProvider Provider
        {
            get
            {
                MembershipProvider provider;

                ProviderSection config = (ProviderSection)ConfigurationManager.GetSection("racingMoto/membership");

                ProviderCollection providers = new ProviderCollection();

                // use the ProvidersHelper class to call Initialize() on each provider
                ProvidersHelper.InstantiateProviders(config.Providers, providers, typeof(MembershipProvider));

                provider = (MembershipProvider)providers[config.DefaultProvider];

                provider.Config(config.Providers[config.DefaultProvider].Parameters);

                return provider;
            }
        }

        public override User GetUser(string username, bool includeRoles)
        {
            using (var context = new RacingDbContext())
            {
                var user = context.User.FirstOrDefault(u => u.UserName == username);
                if (user != null)
                {
                    if (includeRoles)
                    {
                        context.UserRole.Where(ur => ur.UserId == user.UserId).ToList();
                        if (user.UserRoles != null)
                        {
                            var roleIds = user.UserRoles.Select(ur => ur.RoleId);
                            context.Role.Where(r => roleIds.Contains(r.RoleId)).ToList();
                        }
                    }
                }
                return user;
            }
        }

        public override User GetUser(int userId, bool includeRoles)
        {
            using (var context = new RacingDbContext())
            {
                var user = context.User.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    if (includeRoles)
                    {
                        context.UserRole.Where(ur => ur.UserId == user.UserId).ToList();
                        if (user.UserRoles != null)
                        {
                            var roleIds = user.UserRoles.Select(ur => ur.RoleId);
                            context.Role.Where(r => roleIds.Contains(r.RoleId)).ToList();
                        }
                    }
                }
                return user;
            }
        }

        public override List<User> GetAllUsers(bool includeRoles)
        {
            using (var context = new RacingDbContext())
            {
                if (includeRoles)
                {
                    context.UserRole.ToList();
                    context.Role.ToList();
                }
                return context.User.ToList();
            }
        }

        public override bool Exsits(string username)
        {
            using (var context = new RacingDbContext())
            {
                return context.User.Count(u => u.UserName == username) > 0;
            }
        }

        /// <returns>
        ///  0: is valid
        ///  1: user doesn't exist
        ///  2: invalid password
        ///  3: locked
        ///  4: disabled
        /// </returns>
        public override LoginStatus Authenticate(string username, string password)
        {
            int userId = -1;

            var loginStatus = Authenticate(username, password, out userId);

            return loginStatus;
        }

        /// <returns>
        ///  0: is valid
        ///  1: user doesn't exist
        ///  2: invalid password
        ///  3: locked
        ///  4: disabled
        /// </returns>
        protected virtual LoginStatus Authenticate(string username, string password, out int userId)
        {
            userId = -1;

            using (var context = new RacingDbContext())
            {
                var user = context.User.FirstOrDefault(u => u.UserName == username);

                if (user == null) return LoginStatus.InvalidUserName; // user doesn't exist

                if (user.IsLocked) return LoginStatus.IsLocked; // if account is locked, there is no need to check password

                // check password
                bool invalidPassword = false;
                if (HashedPassword)
                {
                    if (CryptoUtils.ComputeHash(password) != user.Password) invalidPassword = true;
                }
                else
                {
                    if (CryptoUtils.Decrypt(user.Password) != password) invalidPassword = true;
                }

                if (invalidPassword)
                {
                    if (FailedPasswordAttemptCount > 0)
                    {
                        DateTime now = DateTime.Now;

                        TimeSpan ts = now - user.FailedPasswordAttemptWindowStart;
                        if (ts.TotalMinutes <= FailedPasswordAttemptWindow)
                        {
                            if (user.FailedPasswordAttemptCount >= FailedPasswordAttemptCount)
                            {
                                user.IsLocked = true;
                                user.LockedDate = now;
                            }
                            else
                            {
                                if (user.FailedPasswordAttemptCount == 0)
                                {
                                    user.FailedPasswordAttemptWindowStart = now;
                                }
                                user.FailedPasswordAttemptCount += 1;
                            }

                            context.SaveChanges();
                        }


                    }

                    return LoginStatus.InvalidPassword;
                }

                //if (!user.Enabled) return LoginStatus.Disabled;

                if (user.FailedPasswordAttemptCount > 0)
                {
                    user.FailedPasswordAttemptCount = 0;

                    context.SaveChanges();
                }

                userId = user.UserId;
            }

            return LoginStatus.Success;
        }

        /// <returns>
        ///  0: valid
        ///  1: user doesn't exist
        ///  2: wrong password
        /// </returns>
        public override int ChangePassword(string username, string oldPassword, string newPassword)
        {
            using (var context = new RacingDbContext())
            {
                var user = context.User.FirstOrDefault(u => u.UserName == username);

                if (user == null) return 1;

                //var encryptOldPassword = HashedPassword ? CryptoUtils.ComputeHash(oldPassword) : CryptoUtils.Encrypt(oldPassword);
                //if (user.Password != encryptOldPassword) return 2;
                if (HashedPassword)
                {
                    if (user.Password != CryptoUtils.ComputeHash(oldPassword)) return 2;
                }
                else
                {
                    if (oldPassword != CryptoUtils.Decrypt(user.Password)) return 2;
                }

                var encryptNewPassword = HashedPassword ? CryptoUtils.ComputeHash(newPassword) : CryptoUtils.Encrypt(newPassword);
                user.Password = encryptNewPassword;

                context.SaveChanges();
            }

            return 0;
        }

        /// <returns>
        ///  0: valid
        ///  1: user doesn't exist
        /// </returns>
        public override int ChangePassword(string username, string newPassword)
        {
            using (var context = new RacingDbContext())
            {
                var user = context.User.FirstOrDefault(u => u.UserName == username);

                if (user == null) return 1;

                var encryptNewPassword = HashedPassword ? CryptoUtils.ComputeHash(newPassword) : CryptoUtils.Encrypt(newPassword);
                user.Password = encryptNewPassword;

                context.SaveChanges();
            }

            return 0;
        }

        /// <returns>
        ///  0: valid
        /// -1: user already exists
        /// </returns>
        public override int CreateUser(User user)
        {
            using (var context = new RacingDbContext())
            {
                if (context.User.Any(u => u.UserName == user.UserName)) return -1;

                User dbUser = new User
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Password = string.IsNullOrEmpty(user.Password) ? null : (HashedPassword ? CryptoUtils.ComputeHash(user.Password) : CryptoUtils.Encrypt(user.Password)),
                    Enabled = user.Enabled,
                    IsLocked = user.IsLocked,
                    LockedDate = user.IsLocked ? new Nullable<DateTime>(DateTime.Now) : null,
                    FailedPasswordAttemptCount = 0,
                    FailedPasswordAttemptWindowStart = DateTime.Parse("1900-01-01"),
                    CreateDate = DateTime.Now,
                    LastLoginDate = DateTime.Now
                };

                context.User.Add(dbUser);

                if (user.UserRoles != null)
                {
                    dbUser.UserRoles = new List<UserRole>();
                    foreach (var ur in user.UserRoles)
                    {
                        dbUser.UserRoles.Add(new UserRole
                        {
                            User = dbUser,
                            RoleId = ur.RoleId
                        });
                    }
                }

                context.SaveChanges();

                user.UserId = dbUser.UserId;
            }

            return 0;
        }

        /// <returns>
        ///  0: valid
        /// -1: user doesn't exist
        /// </returns>
        public override int UpdateUser(User user, bool updateRoles)
        {
            using (var context = new RacingDbContext())
            {
                var dbUser = GetUser(user.UserId, false);

                if (dbUser == null) return -1;

                if (!string.IsNullOrEmpty(user.Password)) dbUser.Password = HashedPassword ? CryptoUtils.ComputeHash(user.Password) : CryptoUtils.Encrypt(user.Password);

                if (dbUser.IsLocked != user.IsLocked)
                {
                    if (user.IsLocked)
                    {
                        dbUser.IsLocked = true;
                        dbUser.LockedDate = DateTime.Now;
                    }
                    else
                    {
                        dbUser.IsLocked = false;
                        dbUser.LockedDate = null;
                        dbUser.FailedPasswordAttemptCount = 0;
                    }
                }
                dbUser.Enabled = user.Enabled;

                if (updateRoles)
                {
                    //if (dbUser.UserRoles == null) dbUser.UserRoles = new List<UserRole>();
                    //else dbUser.UserRoles.Clear();

                    var userRoles = context.UserRole.Where(ur => ur.UserId == user.UserId).ToList();
                    foreach (var userRole in userRoles)
                    {
                        context.UserRole.Remove(userRole);
                    }

                    foreach (var ur in user.UserRoles)
                    {
                        //dbUser.UserRoles.Add(new UserRole
                        //{
                        //    User = dbUser,
                        //    RoleId = ur.RoleId,
                        //    CreateDate = DateTime.Now
                        //});
                        context.UserRole.Add(new UserRole
                        {
                            UserId = ur.UserId,
                            RoleId = ur.RoleId
                        });
                    }
                }

                context.SaveChanges();
            }

            return 0;
        }

        /// <returns>
        ///  0: valid
        /// -1: user doesn't exist
        /// </returns>
        public override int LockUser(int userId, bool locked)
        {
            using (var context = new RacingDbContext())
            {
                var user = context.User.FirstOrDefault(u => u.UserId == userId);

                if (user == null) return -1;

                user.IsLocked = locked;
                if (locked) user.LockedDate = DateTime.Now;
                else user.LockedDate = null;

                context.SaveChanges();
            }

            return 0;
        }

        /// <returns>
        ///  0: valid
        /// -1: user doesn't exist
        /// </returns>
        public override int EnableUser(int userId, bool enabled)
        {
            using (var context = new RacingDbContext())
            {
                var user = context.User.FirstOrDefault(u => u.UserId == userId);

                if (user == null) return -1;

                user.Enabled = enabled;

                context.SaveChanges();
            }

            return 0;
        }

        /// <returns>
        ///  0: valid
        /// -1: user doesn't exist
        /// </returns>
        public override int DeleteUser(int userId)
        {
            using (var context = new RacingDbContext())
            {
                var user = context.User.Where(u => u.UserId == userId).Select(u => new { u.UserId }).FirstOrDefault();
                if (user == null) return -1;

                if (context.Database.Connection.State != System.Data.ConnectionState.Open) context.Database.Connection.Open();
                var trans = context.Database.Connection.BeginTransaction();

                context.Database.ExecuteSqlCommand("DELETE FROM dbo.UserRoles WHERE UserId = @UserId\n"
                    + "DELETE FROM dbo.Users WHERE UserId = @UserId", new DbParameter[] { new SqlParameter("@UserId", userId) });

                trans.Commit();
                context.Database.Connection.Close();
            }
            return 0;
        }

        public override bool IsInRole(string userName, params int[] roleIds)
        {
            using (var context = new RacingDbContext())
            {
                return context.User.Any(u => u.UserName == userName && u.UserRoles.Any(ur => roleIds.Contains(ur.RoleId)));
            }
        }

        public override object Extension(int type, object obj)
        {
            return null;
        }

        /// <returns>
        ///  0: valid
        /// -1: user doesn't exist
        /// </returns>
        public override LoginStatus SignIn(string username, bool isPersistent = false, int? timeout = null)
        {
            var user = GetUser(username, false);
            if (user == null) return LoginStatus.InvalidUserName;

            SetAuthenticationCookie(user.UserId, user.UserName, isPersistent, timeout);

            AuditLogin(user.UserId);

            return LoginStatus.Success;
        }

        /// <returns>
        ///  0: valid
        /// -1: user doesn't exist
        /// -2: invalid password
        /// -3: locked
        /// -4: disabled
        /// </returns>
        public override LoginStatus SignIn(string username, string password, bool isPersistent = false, int? timeout = null)
        {
            int userId;
            var result = Authenticate(username, password, out userId);
            if (result == 0)
            {
                SetAuthenticationCookie(userId, username, isPersistent, timeout);

                AuditLogin(userId);

                return 0;
            }
            else return result;
        }

        protected virtual void SetAuthenticationCookie(int userId, string username, bool isPersistent = false, int? timeout = null)
        {
            var authConfig = (AuthenticationSection)ConfigurationManager.GetSection("system.web/authentication");

            DateTime issueDate = DateTime.Now;
            DateTime expiration;
            if (timeout.HasValue) expiration = issueDate.AddMinutes(timeout.Value);
            else expiration = issueDate.Add(authConfig.Forms.Timeout);
            
            var ticket = new FormsAuthenticationTicket(2, username.ToLower(), issueDate, expiration, isPersistent, Guid.NewGuid().ToString("N"));

            var cookie = new HttpCookie(authConfig.Forms.Name, FormsAuthentication.Encrypt(ticket));
            cookie.Domain = authConfig.Forms.Domain;
            if (isPersistent) cookie.Expires = expiration;
            cookie.HttpOnly = true;
            cookie.Path = authConfig.Forms.Path;

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        protected virtual void AuditLogin(int userId)
        {
            using (var context = new RacingDbContext())
            {
                var user = context.User.Where(u => u.UserId == userId).FirstOrDefault();
                user.LastLoginDate = DateTime.Now;

                context.SaveChanges();
            }
        }

        public override void SignOut()
        {
            System.Web.Security.FormsAuthentication.SignOut();
        }
    }
}
