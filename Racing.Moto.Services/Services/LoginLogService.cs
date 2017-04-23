using Racing.Moto.Core.Extentions;
using Racing.Moto.Data;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Services
{
    public class LoginLogService : BaseServcice
    {
        public void AddLoginLog(LoginLog loginLog)
        {
            using (var db = new RacingDbContext())
            {
                if (loginLog.LoginTime == DateTime.MinValue)
                {
                    loginLog.LoginTime = DateTime.Now;
                }

                db.LoginLog.Add(loginLog);
                db.SaveChanges();
            }
        }

        public PagerResult<LoginLog> GetLoginLogRecords(SearchModel searchModel)
        {
            using (var db = new RacingDbContext())
            {
                var logs = db.LoginLog.OrderByDescending(l => l.LoginLogId).Pager<LoginLog>(searchModel.PageIndex, searchModel.PageSize);

                return logs;
            }
        }
    }
}
