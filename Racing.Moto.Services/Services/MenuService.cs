using Racing.Moto.Data;
using Racing.Moto.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Services
{
    public class MenuService : BaseServcice
    {
        public List<Menu> GetMenuByRoles(List<int> roleIds)
        {
            using (var db = new RacingDbContext())
            {
                return db.Menu.Where(m => m.MenuRoles.Where(mr => roleIds.Contains(mr.RoleId)).Any()).ToList();
            }
        }
    }
}
