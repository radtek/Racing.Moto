using Racing.Moto.Core.Crypto;
using Racing.Moto.Core.Extentions;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Enums;
using Racing.Moto.Data.Models;
using Racing.Moto.Services.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Services
{
    public class UserService : BaseServcice
    {
        public PagerResult<User> GetUsers(UserSearchModel searchModel)
        {
            var query = db.User
                .Include(nameof(User.UserExtension))
                .Include(nameof(User.ParentUser))
                .Include(nameof(User.UserRoles))
                .Include(nameof(User.UserRoles) + "." + nameof(UserRole.Role))
                .Where(u => u.Enabled);
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

        public List<User> GetUsers(List<string> userNames)
        {
            return db.User
                .Include(nameof(User.UserExtension))
                .Include(nameof(User.UserRoles))
                .Include(nameof(User.UserRoles) + "." + nameof(UserRole.Role))
                .Where(u => userNames.Contains(u.UserName)).ToList();
        }

        public User GetUser(int userId)
        {
            return db.User
                .Include(nameof(User.UserExtension))
                .Include(nameof(User.UserRoles))
                .Include(nameof(User.UserRoles) + "." + nameof(UserRole.Role))
                .Where(u => u.UserId == userId).FirstOrDefault();
        }

        public void LockUser(int userId, bool locked)
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

        public void EnableUser(int userId, bool enabled)
        {
            var dbUser = db.User.Where(u => u.UserId == userId).FirstOrDefault();
            if (dbUser != null)
            {
                dbUser.Enabled = enabled;

                db.SaveChanges();
            }
        }

        public bool ExistUserName(string userName)
        {
            return db.User.Where(u => u.UserName == userName).Any();
        }

        public ResponseResult SaveUser(int roleId, User user)
        {
            var response = new ResponseResult();

            if (user.UserId == 0)
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

                db.User.Add(user);
            }
            else
            {
                var dbUser = db.User.Include(nameof(User.UserExtension)).Where(u => u.UserId == user.UserId).FirstOrDefault();
                if (dbUser != null)
                {
                    dbUser.UserName = user.UserName;
                    dbUser.Password = CryptoUtils.Encrypt(user.Password);
                    dbUser.UserExtension.Amount = user.UserExtension.Amount;
                    dbUser.UserExtension.Rebate = user.UserExtension.Rebate;
                }
            }

            db.SaveChanges();

            response.Data = user;

            return response;
        }
    }
}
