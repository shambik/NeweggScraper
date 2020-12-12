using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Mail;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Windows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Gmail.v1.Data;
using System.Threading;

namespace NeweggScraper.Utils.Mail
{
    public class Email
    {
         

        public void SendEmail(string to)
        {
            try
            {
                var service = GmailServiceInit();
                InitMailStructAndSend(to, service);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        private void InitMailStructAndSend(string to,  GmailService service)
        {

            try
            {
                string plainText = "To: " + to + "," + "\r\n" +
                                   "Subject: [Newegg] Product Results\r\n" +
                                   "Content-Type: text/html; charset=us-ascii\r\n\r\n" +
                                   Scraper.SearchResults;

                var newMsg = new Google.Apis.Gmail.v1.Data.Message();

                newMsg.Raw = Base64UrlEncode(plainText.ToString());
                service.Users.Messages.Send(newMsg, "me").Execute();
            }
            catch (Exception e)
            {
                MessageBox.Show("Please provide at least one mail address\nor Uncheck \"Notify Me\" check box");
                throw;
            }
        }

        private GmailService GmailServiceInit()
        {
            string credPath = "token.json";
            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {                  
                    ClientId = "Your Client ID",
                    ClientSecret = "Tour Client Secret"
                },
                new[]
                {
                    GmailService.Scope.GmailSend,
                },
                "user",
                CancellationToken.None,               
                new FileDataStore(credPath, true)).Result;

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });
            return service;
        }

        private string Base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            var msg = System.Convert.ToBase64String(inputBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
            return msg;
        }
    }
}
