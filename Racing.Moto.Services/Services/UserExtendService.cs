using Racing.Moto.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Services
{
    public class UserExtendService : BaseServcice
    {
        public UserExtension GetUserUserExtension(int userId)
        {
            var userExtend = db.UserExtend.Where(u => u.UserId == userId).FirstOrDefault();
            if (userExtend == null)
            {
                userExtend = new UserExtension
                {
                    UserId = userId,
                    Amount = 0
                };

                db.UserExtend.Add(userExtend);
                db.SaveChanges();
            }
            return userExtend;
        }

        /// <summary>
        /// 增加账户金额
        /// </summary>
        public void AddAmount(int userId, decimal amount)
        {
            var userExtend = db.UserExtend.Where(u => u.UserId == userId).FirstOrDefault();
            if (userExtend == null)
            {
                userExtend = new UserExtension
                {
                    UserId = userId,
                    Amount = amount
                };

                db.UserExtend.Add(userExtend);
            }
            else
            {
                userExtend.Amount += amount;
            }
            db.SaveChanges();
        }

        /// <summary>
        /// 减少账户金额
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="amount"></param>
        public void MinusAmount(int userId, decimal amount)
        {
            var userExtend = db.UserExtend.Where(u => u.UserId == userId).FirstOrDefault();
            userExtend.Amount = userExtend.Amount - amount;
            db.SaveChanges();
        }
    }
}
