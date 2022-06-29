namespace AutoDelivery.Domain.Result
{
    public class ShopifyResult
    {
            public int Status { get; set; }
            public string ErrorMessage { get; set; }
            public DateTimeOffset Time { get; set; }
            public string RedirectPath { get; set; }
            public string Data { get; set; }
            
            
          
          
    }
}