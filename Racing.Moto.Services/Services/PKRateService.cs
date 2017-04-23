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
    public class PKRateService : BaseServcice
    {
        public List<PKRate> GetPKRates(int pkId)
        {
            using (var db = new RacingDbContext())
            {
                return db.PKRate.Where(r => r.PKId == pkId).ToList();
            }
        }


        public List<PKRateModel> GetPKRateModels(int pkId)
        {
            using (var db = new RacingDbContext())
            {
                var pkRateModels = new List<PKRateModel>();

                var allPKRates = db.PKRate.Where(r => r.PKId == pkId).ToList();

                if (allPKRates.Count > 0)
                {
                    for (var i = 1; i <= 10; i++)//名次
                    {
                        var pkRates = allPKRates.Where(r => r.Rank == i).ToList();
                        var model = ConvertToPKRateModel(i, pkRates);
                        pkRateModels.Add(model);
                    }
                }

                return pkRateModels;
            }
        }


        private PKRateModel ConvertToPKRateModel(int rank, List<PKRate> pkRates)
        {
            return new PKRateModel
            {
                Rank = rank,
                Rate1 = pkRates.Where(r => r.Num == 1).First().Rate,
                Rate2 = pkRates.Where(r => r.Num == 2).First().Rate,
                Rate3 = pkRates.Where(r => r.Num == 3).First().Rate,
                Rate4 = pkRates.Where(r => r.Num == 4).First().Rate,
                Rate5 = pkRates.Where(r => r.Num == 5).First().Rate,
                Rate6 = pkRates.Where(r => r.Num == 6).First().Rate,
                Rate7 = pkRates.Where(r => r.Num == 7).First().Rate,
                Rate8 = pkRates.Where(r => r.Num == 8).First().Rate,
                Rate9 = pkRates.Where(r => r.Num == 9).First().Rate,
                Rate10 = pkRates.Where(r => r.Num == 10).First().Rate,
                Rate11 = pkRates.Where(r => r.Num == 11).First().Rate,
                Rate12 = pkRates.Where(r => r.Num == 12).First().Rate,
                Rate13 = pkRates.Where(r => r.Num == 13).First().Rate,
                Rate14 = pkRates.Where(r => r.Num == 14).First().Rate
            };
        }
    }
}
