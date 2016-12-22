using Racing.Moto.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Services
{
    public class AppConfigService : BaseServcice
    {
        public List<AppConfig> GetAll()
        {
            return db.AppConfig.ToList();
        }
    }
}
