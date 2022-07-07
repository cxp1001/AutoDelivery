using AutoDelivery.Core.Extensions;
using AutoDelivery.Domain;

namespace AutoDelivery.Service.MailApp
{
    public interface IMailService : IDependency
    {
        bool SendActiveEmail(string mailTo, string mailTitle, string mailContent);
    }
}