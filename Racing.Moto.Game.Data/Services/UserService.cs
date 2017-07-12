using Racing.Moto.Core.Extentions;
using Racing.Moto.Game.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Game.Data.Services
{
    public class UserService
    {
        /// <summary>
        /// 生成头像
        /// </summary>
        /// <param name="userName"></param>
        public void SaveAvatar(string userName)
        {
            using (var db = new RacingGameDbContext())
            {
                var user = db.User.Where(u => u.UserName == userName).FirstOrDefault();
                if (user != null)
                {
                    user.Avatar = GetRandomAvatar();
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 取随机头像, 目前有17个头像
        /// </summary>
        /// <returns></returns>
        private string GetRandomAvatar()
        {
            var radom = new Random();

            var rmd = radom.Next(1, 17);

            return string.Format("/Img/avatars/user{0}.jpg", rmd);
        }
    }
}