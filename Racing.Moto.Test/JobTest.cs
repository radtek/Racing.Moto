using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using Racing.Moto.Core.Utils;
using Racing.Moto.Services;
using Racing.Moto.JobManager.Jobs;

namespace Racing.Moto.Test
{
    [TestClass]
    public class JobTest
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();  //Logger对象代表与当前类相关联的日志消息的来源  

        [TestMethod]
        public void JobServiceTest()
        {
            //var pkService = new PKService();
            //var bonusService = new PKBonusService();

            //var pk = pkService.GetPK(1956);
            //// 生成奖金
            ////bonusService.GenerateBonus(pk);
            //// 生成退水
            //bonusService.GenerateRebate(pk);

            ////new RebateJob().Execute(null);

            //new PkJob().Execute(null);

            //new RankJob().Run();

            new BetService().CalculateRanks(4900);

            //RebateJob rebateJob = new RebateJob();
            //rebateJob.Execute(null);
        }

        [TestMethod]
        public void CalculateRanksTest()
        {

            new BetService().CalculateRanks(36634);
        }

        [TestMethod]
        public void BonusJobTest()
        {

            new BonusJob().Run();
        }

        [TestMethod]
        public void RebateJobTest()
        {

            //new RebateJob().Execute(null);

            var pk = new PKService().GetPK(31448);
            new PKBonusService().GenerateRebate(pk);
        }

        [TestMethod]
        public void WechatJobTest()
        {

            //new WechatJob().Run();

            new WechatJob().GetBonusAndRebateAmount(100000056);
        }

        #region Game
        [TestMethod]
        public void PkGameRankJobTest()
        {

            new PkGameRankJob().Run();
        }

        [TestMethod]
        public void PkGameBonusJobTest()
        {

            new PkGameBonusJob().Run();
        }

        [TestMethod]
        public void PkGameDummyUserJobTest()
        {

            new PkGameDummyUserJob().Run();
        }
        #endregion
    }
}
