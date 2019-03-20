using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStudy.Services.Implementations
{
    //This class writes logs into the specified logfile
    public class FileLogger : ILogger
    {
        private readonly string filePath;
        private static readonly object locker = new object();

        public FileLogger(string filePath)
        {
            this.filePath = filePath;
        }


        //unnnecessary for us
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }


        //does our FileLogger available for using?
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                //block below code scope for only one current thread (to prevent concurrency from parallel threads)
                lock (locker)
                {
                    File.AppendAllText(filePath, $"{DateTime.Now}:  {formatter(state, exception) + Environment.NewLine}");
                }
            }
        }
    }
}
