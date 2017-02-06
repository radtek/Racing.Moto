using Racing.Moto.Core.Extentions;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Enums;
using Racing.Moto.Data.Models;
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
            var roleId = (int)searchModel.UserType;

            var query = db.User
                .Include(nameof(User.UserExtend))
                .Include(nameof(User.UserRoles))
                .Include(nameof(User.UserRoles) + "." + nameof(UserRole.Role))
                .AsQueryable();

            if (searchModel.UserType != UserType.All)
            {
                query = query.Where(u => u.UserRoles.Where(ur => ur.RoleId == roleId).Any());
            }

            var users = query.OrderBy(u => u.UserName).Pager<User>(searchModel.PageIndex, searchModel.PageSize);

            return users;
        }
    }
}
