using CoreStudy.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStudy.Services.Interfaces
{
    public interface IGetFileLoggerProvider
    {
        FileLoggerProvider GetProvider(string pathToLogFile);
    }
}
