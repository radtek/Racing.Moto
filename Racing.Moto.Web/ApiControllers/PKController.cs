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
    public class PKController : ApiController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 取当前期的倍率
        /// </summary>
        //[HttpGet]
        //public ResponseResult GetCurrentPKInfo()
        //{
        //    var result = new ResponseResult();

        //    try
        //    {
        //        var pk = new PKService().GetCurrentPK();
        //        var pkRates = new PKRateService().GetPKRateModels(pk.PKId);
        //        result.Data = new
        //        {
        //            PK = pk,
        //            PKRates = pkRates
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Info(ex);

        //        result.Success = false;
        //        result.Message = MessageConst.System_Error;
        //    }

        //    return result;
        //}

        /// <summary>
        /// 取当前期的倍率
        /// </summary>
        [HttpPost]
        public ResponseResult GetPrevPK()
        {
            var result = new ResponseResult();

            try
            {
                var pk = new PKService().GetPrevPKResult();
                result.Data = pk;
            }
            catch (Exception ex)
            {
                _logger.Info(ex);

                result.Success = false;
                result.Message = MessageConst.System_Error;
            }

            return result;
        }
    }
}
