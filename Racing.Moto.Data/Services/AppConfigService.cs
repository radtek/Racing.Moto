﻿using Racing.Moto.Data.Entities;
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
        public AppConfig Add(string name, string val)
        {
            var appConfig = new AppConfig();
            appConfig.Name = name;
            appConfig.Value = val;

            appConfig = db.AppConfig.Add(appConfig);
            db.SaveChanges();

            return appConfig;
        }

        public void Update(string name, string val)
        {
            var appConfig = db.AppConfig.Where(a => a.Name == name).FirstOrDefault();
            if (appConfig != null)
            {
                appConfig.Value = val;
                db.SaveChanges();
            }
        }

        public void Delete(string name)
        {
            var appConfig = db.AppConfig.Where(a => a.Name == name).FirstOrDefault();
            if (appConfig != null)
            {
                db.AppConfig.Remove(appConfig);
                db.SaveChanges();
            }
        }
    }
}
