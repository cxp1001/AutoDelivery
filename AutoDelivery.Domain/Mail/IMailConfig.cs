namespace AutoDelivery.Domain.Mail
{
    public interface IMailConfig
    {
         string MailServiceName { get; set; }
        string MailSTMPServer { get; set; }
        string MailAccount { get; set; }
        string MAilPassword { get; set; }
    }
}