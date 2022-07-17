using AutoDelivery.Domain;
using AutoDelivery.Service.MailApp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AutoDelivery.Api.Controllers
{

    public class MailConfigController : BaseController
    {
        private readonly IMailService _mailService;
        public MailConfigController(IMailService mailService)
        {
            this._mailService = mailService;

        }

        [HttpPost]
        public async Task<IActionResult> ConfigMail(int productId, [FromBody] string content, int userId = 4)
        {

            MailContent mailContent = JsonConvert.DeserializeObject<MailContent>(content);
            var mailConfig = await _mailService.ConfigMailContent(userId, productId, mailContent);
            return Ok(
                JsonConvert.SerializeObject(mailConfig)
            );
        }





    }
}