using Racing.Moto.Core.Crypto;
using Racing.Moto.Core.Extentions;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Enums;
using Racing.Moto.Data.Models;
using Racing.Moto.Services.Constants;
using System;
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
                .AsQueryable();

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

        public bool ExistUserName(string userName)
        {
            return db.User.Where(u => u.UserName == userName).Any();
        }

        public ResponseResult UpdateUser(User user)
        {
            var respons = new ResponseResult();

            var existUserName = db.User.Where(u => u.UserId != user.UserId && u.UserName == user.UserName).Any();

            if (existUserName)
            {
                respons.Success = false;
                respons.Message = MessageConst.USER_EXIST_USERNAME;
                return respons;
            }

            var dbUser = db.User.Include(nameof(User.UserExtension)).Where(u => u.UserId == user.UserId).FirstOrDefault();
            if (dbUser != null)
            {
                dbUser.UserName = user.UserName;
                dbUser.Password = CryptoUtils.Encrypt(user.Password);
                dbUser.UserExtension.Amount = user.UserExtension.Amount;
                dbUser.UserExtension.Rebate = user.UserExtension.Rebate;
            }
            db.SaveChanges();

            return respons;
        }
    }
}
