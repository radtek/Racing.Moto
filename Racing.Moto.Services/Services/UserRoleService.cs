using Racing.Moto.Data;
using Racing.Moto.Data.Entities;
using Racing.Moto.Services.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Services
{
    public class UserRoleService : BaseServcice
    {
        public List<UserRole> GetUserRoles(int userId)
        {
            using (var db = new RacingDbContext())
            {
                return db.UserRole.Include(nameof(UserRole.Role)).Where(r => r.UserId == userId).ToList();
            }
        }
        public List<UserRole> GetUserRoles(string userName)
        {
            using (var db = new RacingDbContext())
            {
                return db.UserRole.Where(r => r.User.UserName == userName).ToList();
            }
        }

        public bool IsMember(string userName)
        {
            using (var db = new RacingDbContext())
            {
                return db.UserRole.Where(r => r.User.UserName == userName && r.RoleId == RoleConst.Role_Id_Member).Any();
            }
        }
        public bool IsAdmin(string userName)
        {
            using (var db = new RacingDbContext())
            {
                return db.UserRole.Where(r => r.User.UserName == userName && r.RoleId == RoleConst.Role_Id_Admin).Any();
            }
        }
        public bool IsAdmin(List<UserRole> userRoles)
        {
            return userRoles.Where(r => r.RoleId == RoleConst.Role_Id_Admin).Any();
        }
    }
}
