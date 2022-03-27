using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Logging
{

    public enum Category { Debug, Info, Warn, Exception }
    public enum Priority { High, Medium, Low }
    public interface ILoggerFacade 
    {
        void Log(string message, Category category, Priority priority = Priority.High);
    }
    public class Log4NetLoggerAdaptor : ILoggerFacade
    {

        public Log4NetLoggerAdaptor()
        {
            try
            {
                //使用配置文件
                log4net.Config.XmlConfigurator.Configure();
               
            }
            catch
            {
                //代码配置
                RollingFileAppender appender = new RollingFileAppender();
                appender.Name = "LogFileAppenderByDate";
                appender.File = "Log\\";
                appender.LockingModel = new FileAppender.MinimalLock();
                appender.DatePattern = "yyyy-MM-dd.LOG";
                appender.StaticLogFileName = false;
                appender.RollingStyle = RollingFileAppender.RollingMode.Date;
                appender.AppendToFile = true;
                PatternLayout layout = new PatternLayout("发生时间:%d %n事件级别:%level %n相关类名:%c%n程序文件:%F 第%L行%n日志内容:%m%n-----------------------------------------%n%n");
                appender.Layout = layout;
                BasicConfigurator.Configure(appender);
                appender.ActivateOptions();
            }
        }

        /// <summary>
        /// Info委托
        /// </summary>
        /// <param name="message">日志信息</param>
        private delegate void DInfo(object message);

        /// <summary>
        /// Error委托
        /// </summary>
        /// <param name="message">日志信息</param>
        private delegate void DError(object message, Exception ex);


        private static DInfo Debug
        {

            get { 
                
                return LogManager.GetLogger((new StackTrace()).GetFrame(2).GetMethod().DeclaringType).Debug; 
            }
        }

        /// <summary>
        /// Info
        /// </summary>
        private static  DInfo Info
        {
            get { return LogManager.GetLogger((new StackTrace()).GetFrame(2).GetMethod().DeclaringType).Info; }
        }

        /// <summary>
        /// Warn
        /// </summary>
        private static DError Warn
        {
            get { return LogManager.GetLogger((new StackTrace()).GetFrame(2).GetMethod().DeclaringType).Warn; }
        }

        /// <summary>
        /// Error
        /// </summary>
        private static DError Error
        {
            get { return LogManager.GetLogger((new StackTrace()).GetFrame(2).GetMethod().DeclaringType).Error; }
        }

        private static log4net.ILog _logger = log4net.LogManager.GetLogger("Logger");
        public void Log(string message, Category category, Priority priority= Priority.High)
        {
            switch (category)
            {
                case Category.Debug:

                    Debug(message);
                    //_logger.Debug(message);
                    break;
                case Category.Exception:
                    Error(message, null);
                    break;
                case Category.Info:
                    Info(message);
                    break;
                case Category.Warn:
                    Warn(message, null);
                    break;
                default:
                    Info(message);
                    break;
            }
        }
    }
}
