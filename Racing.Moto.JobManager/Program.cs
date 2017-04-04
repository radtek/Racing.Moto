using NLog;
using Racing.Moto.JobManager.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.JobManager
{
    class Program
    {
        #region console
        //static void Main(string[] args)
        //{
        //    try
        //    {
        //        JobScheduler.Start();
        //    }
        //    catch(Exception ex)
        //    {
        //        LogManager.GetCurrentClassLogger().Info(ex);
        //    }
        //}

        #endregion


        #region service
        static void Main()
        {
            // set the current directory to the same directory as your windows service
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new RacingMotoJobService()
            };
            ServiceBase.Run(ServicesToRun);
        }

        #endregion
    }
}
