using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using System.Reflection;
using System.Xml;

namespace Pro.Common
{
    public static class LogHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LoggerManager));// = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        public static void InitLogHelper(string foldeLog = "")
        {

            var xmlStr = File.ReadAllText("log4net.config");
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlStr);
            var items = xmlDocument.GetElementsByTagName("log4net");
            XmlElement newLog4net = null;
            foreach (var item in items)
            {
                newLog4net = (System.Xml.XmlElement)(item);
                break;
            }
            if (!String.IsNullOrEmpty(foldeLog))
            {
                for (int i = 0; i < newLog4net.ChildNodes.Count; i++)
                {
                    XmlNode child = newLog4net.ChildNodes[i];
                    if (child.Name == "appender")
                    {
                        for (int ii = 0; ii < child.ChildNodes.Count; ii++)
                        {
                            XmlNode child2 = child.ChildNodes[ii];
                            if (child2.Name == "file")
                            {
                                child2.Attributes["value"].Value = foldeLog;
                                break;
                            }
                        }
                    }
                }
            }
            XmlConfigurator.Configure(logRepository, newLog4net);
        }
        public static void Error(Exception ex, string format = "", params object[] args)
        {
            var stacktrace = string.IsNullOrEmpty(ex.StackTrace) ? "" : "\r\n" + ex.StackTrace;
            Log.Error(ex.Message + string.Format(format, args) + stacktrace);
        }

        public static void Error(string text)
        {
            Log.Error(text);
        }

        public static void Error(string format, params object[] args)
        {
            Log.ErrorFormat(format, args);
        }

        public static void Debug(string text)
        {
            Log.Debug(text);
        }

        public static void Debug(string format, params object[] args)
        {
            Log.DebugFormat(format, args);
        }

        public static void Info(string text)
        {
            Log.Info(text);
        }

        public static void Info(string format, params object[] args)
        {
            Log.InfoFormat(format, args);
        }

        public static void Warn(string text)
        {
            Log.Warn(text);
        }

        public static void Warn(string format, params object[] args)
        {
            Log.WarnFormat(format, args);
        }
    }
}
