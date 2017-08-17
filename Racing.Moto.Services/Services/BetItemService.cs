using Racing.Moto.Data;
using Racing.Moto.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Services
{
    public class BetItemService
    {
        public List<BetItem> GetNotSyncedBetItems()
        {
            using (var db = new RacingDbContext())
            {
                return db.BetItem
                    .Include(nameof(BetItem.Bet))
                    .Where(b => b.Bet.PK.IsBonused && b.Bet.PK.IsRebated && b.IsSynced.HasValue && b.IsSynced.Value == false).ToList();
            }
        }
        public List<long> GetNotSyncedOrderNos()
        {
            using (var db = new RacingDbContext())
            {
                return db.BetItem
                    .Where(b => b.Bet.PK.IsBonused && b.Bet.PK.IsRebated && b.IsSynced.HasValue && b.IsSynced.Value == false && b.OrderNo.HasValue)
                    .Select(b => b.OrderNo.Value).ToList();
            }
        }

        public List<BetItem> GetBetItemsByOrderNo(long orderNo)
        {
            using (var db = new RacingDbContext())
            {
                return db.BetItem
                    .Include(nameof(BetItem.Bet))
                    .Where(b => b.OrderNo == orderNo).ToList();
            }
        }

        public void UpdateIsSynced(long orderNo, bool isSynced)
        {
            using (var db = new RacingDbContext())
            {
                var betItems = db.BetItem.Where(b => b.OrderNo == orderNo).ToList();
                foreach (var item in betItems)
                {
                    item.IsSynced = isSynced;
                }
                db.SaveChanges();
            }
        }
    }
}
