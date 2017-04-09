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

namespace Racing.Moto.Web.Admin.ApiControllers
{
    public class UserController : ApiController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 添加代理/会员时取父亲节点
        /// </summary>
        [HttpPost]
        public ResponseResult GetParentUsers(int id)
        {
            var result = new ResponseResult();

            try
            {
                result.Data = new UserService().GetParentUsers(id);
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
