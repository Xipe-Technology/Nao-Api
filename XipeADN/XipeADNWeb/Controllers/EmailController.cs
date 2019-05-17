using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using FirebaseNet.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using RestSharp.Authenticators;
using XipeADNWeb.Data;
using XipeADNWeb.Entities;
using XipeADNWeb.Models;
using XipeADNWeb.Services;

namespace XipeADNWeb.Controllers
{
    [ /*Authorize, */ Route("api/[controller]")]
    public class EmailController : Controller
    {
        private readonly XipeADNDbContext _db;
        private readonly string  ServerApiKey = "AAAAN43ncTQ:APA91bEiKe1rDrG7xrICNrt7gSA1lMG7MDim1bPk1SKAvWnRIe7f6Dfm-XRdX85s5EnFNqRhRVfuL0CfVzH9ICEKNGT6PFNhyTb_JJz8lCZpbEH2meDi8QEZvlvjymJ7KS_TA779Rds4";

        public EmailController(IUserService userService, XipeADNDbContext db, UserManager<User> userManager)
        {
            _db = db;
        }

        public static IRestResponse SendSimpleMessage(Models.Message message)
        {
            RestClient client = new RestClient
            {
                BaseUrl = new Uri("https://api.mailgun.net/v3"),
                Authenticator = new HttpBasicAuthenticator("api", "key-9b7b33bcc4285cc7337a96e3609a61ec")
            };
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "mails.inovercy.com", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "<contact@naomarketplace.com>");
            request.AddParameter("to", message.Destination);
            request.AddParameter("subject", message.Subject);
            request.AddParameter("html", message.Body);
            request.Method = Method.POST;
            return client.Execute(request);
        }

    }
}
