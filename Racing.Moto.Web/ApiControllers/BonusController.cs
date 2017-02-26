using NLog;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Models;
using Racing.Moto.Services;
using Racing.Moto.Services.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Racing.Moto.Web.ApiControllers
{
    public class BonusController : ApiController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 当前局比赛完成后, 获取奖金数据
        /// </summary>
        [HttpPost]
        public ResponseResult GetBonus(BonusSearchModel model)
        {
            var result = new ResponseResult();

            try
            {
                // TODO 改成 异步
                result.Data = new PKBonusService().GetPKBonus(model.PKId, model.UserId);
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                result.Success = false;
                result.Message = MessageConst.System_Error;
            }

            return result;
        }
    }
}
