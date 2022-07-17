using AutoDelivery.Core.Extensions;
using AutoDelivery.Domain;
using System.Net.Mail;

namespace AutoDelivery.Service.MailApp
{
    public interface IMailService : IDependency
    {
        Task<MailContent> ConfigMailContent(int userId, int productId, MailContent mailContent);
        Task<MailMessage> GenerateMail(int productId, List<Serial> serials);
        bool SendActiveEmail(string mailTo, string mailTitle, string mailContent);
    }
}