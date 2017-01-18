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
using System.Web.Http;

namespace Racing.Moto.Web.ApiControllers
{
    public class PKRateController : ApiController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 取当前期的倍率
        /// </summary>
        /// 
        [HttpGet]
        public ResponseResult GetCurrentPKRates()
        {
            var result = new ResponseResult();

            try
            {
                result.Data = new PKRateService().GetCurrentPKRateModels();
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
