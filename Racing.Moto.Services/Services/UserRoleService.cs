using Racing.Moto.Data.Entities;
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
            return db.UserRole.Include(nameof(UserRole.Role)).Where(r => r.UserId == userId).ToList();
        }
    }
}
