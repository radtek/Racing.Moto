using Racing.Moto.Data.Entities;
using Racing.Moto.Services.Caches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Services
{
    public class UserRebateService : BaseServcice
    {
        public List<UserRebate> GetUserRebates(int userId)
        {
            return db.UserRebate.Where(u => u.UserId == userId).OrderBy(u => u.RebateNo).ToList();
        }

        public void UpdateUserRebates(int userId, List<UserRebate> userRebates)
        {
            var dbRebates = db.UserRebate.Where(u => u.UserId == userId).OrderBy(u => u.RebateNo).ToList();
            foreach (var dbRebate in dbRebates)
            {
                var rebate = userRebates.Where(r => r.RebateNo == dbRebate.RebateNo).FirstOrDefault();
                dbRebate.RebateTypeA = rebate.RebateTypeA;
                dbRebate.RebateTypeB = rebate.RebateTypeB;
                dbRebate.RebateTypeC = rebate.RebateTypeC;
            }
            db.SaveChanges();
        }

        #region static

        /// <summary>
        /// 创建用户时, 添加默认的退水
        /// </summary>
        public static List<UserRebate> GetDefaultRebates()
        {
            var rebates = new List<UserRebate>();

            for (var i = 1; i <= 14; i++)
            {
                rebates.Add(new UserRebate
                {
                    RebateNo = i,
                    DefaultRebateType = Data.Enums.RebateType.C,
                    RebateTypeA = AppConfigCache.Rate_Rebate_A,
                    RebateTypeB = AppConfigCache.Rate_Rebate_B,
                    RebateTypeC = AppConfigCache.Rate_Rebate_C,
                });
            }

            return rebates;
        }

        public static decimal GetDefaultRebate(UserRebate userRebate)
        {
            var rebate = 0M;
            switch (userRebate.DefaultRebateType)
            {
                case Data.Enums.RebateType.A: rebate = userRebate.RebateTypeA; break;
                case Data.Enums.RebateType.B: rebate = userRebate.RebateTypeB; break;
                case Data.Enums.RebateType.C: rebate = userRebate.RebateTypeC; break;
            }
            return rebate;
        }

        #endregion
    }
}
