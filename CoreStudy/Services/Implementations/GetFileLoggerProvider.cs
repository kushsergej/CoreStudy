using CoreStudy.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStudy.Services.Implementations
{
    public class GetFileLoggerProvider : IGetFileLoggerProvider
    {
        public FileLoggerProvider GetProvider(string pathToLogFile)
        {
            string fullPathToLogFile = Path.Combine(
                Directory.GetCurrentDirectory(),
                pathToLogFile);

            return new FileLoggerProvider(fullPathToLogFile);
        }
    }
}
