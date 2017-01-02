﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;

namespace Racing.Moto.Test
{
    [TestClass]
    public class WebTest
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();  //Logger对象代表与当前类相关联的日志消息的来源  

        [TestMethod]
        public void LogTest()
        {
            logger.Trace("输出一条记录信息成功！");//最常见的记录信息，一般用于普通输出     
            logger.Debug("输出一条Debug信息成功！"); //同样是记录信息，不过出现的频率要比Trace少一些，一般用来调试程序     
            logger.Info("输出一条消息类型信息成功！");//信息类型的消息     
            logger.Warn("输出一条警告信息成功");//警告信息，一般用于比较重要的场合     
            logger.Error("输出一条错误信息成功！");//错误信息    
            logger.Fatal("输出一条致命信息成功！");//致命异常信息。一般来讲，发生致命异常之后程序将无法继续执行。 
        }
    }
}
