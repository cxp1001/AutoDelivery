using AutoDelivery.Domain;

namespace AutoDelivery.Service.MailApp
{
    public interface IMailService : IocTag
    {
        bool SendActiveEmail(string mailTo, string mailTitle, string mailContent);
    }
}