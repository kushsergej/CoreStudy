using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStudy.Services.Implementations
{
    //This class returns ILogger instance (i.e. FileLogger), containg path to logfile
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string path;

        public FileLoggerProvider(string path)
        {
            this.path = path;
        }


        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(path);
        }


        public void Dispose()
        {
        }
    }
}
