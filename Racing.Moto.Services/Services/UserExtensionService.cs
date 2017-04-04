using Racing.Moto.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Services
{
    public class UserExtensionService : BaseServcice
    {
        public UserExtension GetUserUserExtension(int userId)
        {
            var userExtend = db.UserExtension.Where(u => u.UserId == userId).FirstOrDefault();
            if (userExtend == null)
            {
                userExtend = new UserExtension
                {
                    UserId = userId,
                    Amount = 0
                };

                db.UserExtension.Add(userExtend);
                db.SaveChanges();
            }
            return userExtend;
        }

        /// <summary>
        /// 增加账户金额
        /// </summary>
        public void AddAmount(int userId, decimal amount)
        {
            var userExtend = db.UserExtension.Where(u => u.UserId == userId).FirstOrDefault();
            if (userExtend == null)
            {
                userExtend = new UserExtension
                {
                    UserId = userId,
                    Amount = amount
                };

                db.UserExtension.Add(userExtend);
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
            var userExtend = db.UserExtension.Where(u => u.UserId == userId).FirstOrDefault();
            userExtend.Amount = userExtend.Amount - amount;
            db.SaveChanges();
        }

        /// <summary>
        /// 保存 找回密码时系统生成的验证码
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="validateCode"></param>
        public void SaveValidateCode(string userName, string validateCode)
        {
            var userExtention = db.UserExtension.Where(u => u.User.UserName == userName).FirstOrDefault();
            if (userExtention != null)
            {
                userExtention.ValidateCodeForForgetPwd = validateCode;
                userExtention.ValidateCodeCreateDate = DateTime.Now;

                db.SaveChanges();
            }
        }

        public bool CheckValidateCodeForForgetPwd(string userName, string code)
        {
            var extention = db.UserExtension.Where(u => u.User.UserName == userName && u.ValidateCodeForForgetPwd == code).FirstOrDefault();
            if (extention != null)
            {
                extention.ValidateCodeForForgetPwd = null;
                extention.ValidateCodeCreateDate = null;

                db.SaveChanges();
            }
            return extention != null;
        }
    }
}
