namespace AutoDelivery.Domain
{
    public class MailContent : BaseModel
    {
        public string MailTitle { get; set; }
        public string MainContent { get; set; }
    }
}