using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStudy.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(string Email, string Subject, string MessageText);
    }
}
