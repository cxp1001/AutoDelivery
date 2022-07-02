using AutoDelivery.Service.MailApp;
using Microsoft.AspNetCore.Mvc;

namespace AutoDelivery.Api.Controllers
{

    public class MailController : BaseController
    {
        private readonly IMailService _mailService;
        public MailController(IMailService mailService)
        {
            this._mailService = mailService;
        }

       [HttpPost]
        public  bool SendMailAsync( string mailTo, string mailTitle, string mailContent)
        {
            return  _mailService.SendActiveEmail( mailTo, mailTitle, mailContent);
        }
    }
}