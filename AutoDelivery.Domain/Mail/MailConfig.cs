namespace AutoDelivery.Domain.Mail
{
    public class MailConfig : BaseModel, IMailConfig
    {

        public string MailServiceName { get; set; }
        public string MailSTMPServer { get; set; }
        public string MailAccount { get; set; }
        public string MAilPassword { get; set; }

    }
}