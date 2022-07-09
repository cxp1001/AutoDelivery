namespace AutoDelivery.Domain
{
    public class MailContent:BaseModel
    {
        public string MailTitle { get; set; }
        public string ContentBegin { get; set; }
        public string ContentEnding { get; set; }
        
    
        
    }
}