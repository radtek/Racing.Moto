using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using Racing.Moto.Core.Utils;
using Racing.Moto.Services;
using Racing.Moto.Web.Jobs;

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

            //var pk = pkService.GetPK(87);
            //// 生成奖金
            //bonusService.GenerateBonus(pk);

            new RebateJob().Execute(null);
        }
    }
}
