using Newtonsoft.Json;
using NLog;
using Quartz;
using Racing.Moto.Core.Utils;
using Racing.Moto.Data.Models;
using Racing.Moto.Game.Data.Constants;
using Racing.Moto.Game.Data.Entities;
using Racing.Moto.Game.Data.Services;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Racing.Moto.JobManager.Jobs
{
    /*
     *   DisallowConcurrentExecution
         禁止并发执行多个相同定义的JobDetail, 
         这个注解是加在Job类上的, 但意思并不是不能同时执行多个Job, 而是不能并发执行同一个Job Definition(由JobDetail定义), 但是可以同时执行多个不同的JobDetail
    */
    [DisallowConcurrentExecution]
    public class PkGameDummyUserJob : IJob
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 虚拟用户
        /// 每隔10秒执行一次
        /// </summary>
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var startInfo = "[PkGameDummyUserJob] Start at : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine(startInfo);
                _logger.Info(startInfo);

                Run();
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }
        }

        public void Run()
        {
            try
            {
                var racingGameWebUrl = System.Configuration.ConfigurationManager.AppSettings["RacingGameWebUrl"];
                RestClient client = new RestClient(racingGameWebUrl);
                var request = new RestRequest("/api/OnlineUser/GenerateDummyUsers", Method.POST);
                //request.AddJsonBody(model);

                var response = client.Execute(request);

                if (response != null && !string.IsNullOrEmpty(response.Content))
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult>(response.Content);

                    if (!result.Success)
                    {
                        _logger.Info(response.Content);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }
        }
    }
}