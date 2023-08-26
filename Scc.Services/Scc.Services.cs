using Microsoft.Extensions.Logging;
using System.Collections;
using System.Text;
using NLog.Extensions.Logging;
using static System.Environment;
using static System.String;

namespace Scc.Services
{
    public class Logging
    {
        public static Microsoft.Extensions.Logging.ILogger GetLogger(string name)
        {
            var nlogLoggerProvider = new NLogLoggerProvider();
            return nlogLoggerProvider.CreateLogger(name);
        }
    }

    public class LoggerHelper
    {
        /// <summary>
        /// Recurse through the exception chain and form a diagnostic string for 
        /// logging or display.
        /// </summary>
        /// <param name="exc">Start of the exception chain</param>
        /// <param name="sb">Accumulated diagnostic string</param>
        /// 
        public static void ParseException(Exception? exc, StringBuilder sb, string? user = null)
        {
            if (exc != null)
            {
                sb.AppendFormat($"{NewLine}Date: {DateTime.Now}{NewLine}");

                if (!IsNullOrEmpty(user))
                {
                    sb.AppendFormat($"User: {user}{NewLine}");
                }

                sb.AppendFormat($"Message: {exc.Message}{NewLine}");
                sb.AppendFormat($"Source: {exc.Source}{NewLine}");
                sb.AppendFormat($"TargetSite: {exc.TargetSite}{NewLine}");
                sb.AppendFormat($"StackTrace: {exc.StackTrace}{NewLine}");

                if (exc.Data.Count > 0)
                {
                    sb.AppendFormat($"Data: {NewLine}");
                }

                foreach (DictionaryEntry de in exc.Data)
                {
                    sb.AppendFormat($"{de.Key}: {de.Value}{NewLine}");
                }

                sb.Append(NewLine);

                ParseException(exc.InnerException, sb);
            }
        }
    }

    public static class LoggerExtensions
    {
        public static string ParseError(this ILogger log, Exception exc)
        {
            var sb = new StringBuilder();
            LoggerHelper.ParseException(exc, sb);
            return sb.ToString();
        }
    }
}

