﻿using CoreStudy.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStudy.Services.Implementations
{
    public class EmailSender : IEmailSender
    {
        #region DI
        private readonly IConfiguration configuration;

        public EmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        #endregion


        public async Task SendAsync(string userEmail, string Subject, string MessageText)
        {
            string address = configuration["Email:address"];
            string login = configuration["Email:login"];
            string psw = configuration["Email:password"];

            MimeMessage emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("CoreStudy app", address));
            emailMessage.To.Add(new MailboxAddress("", userEmail));
            emailMessage.Subject = Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = MessageText };

            using (SmtpClient client = new SmtpClient())
            {
                await client.ConnectAsync(host: "smtp.yandex.ru", port: 465, useSsl: true);
                await client.AuthenticateAsync(userName: login, password: psw);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(quit: true);
            }
        }
    }
}
