namespace AutoDelivery.Domain.Result
{
    public class Result
    {
        
            public int Status { get; set; }
            public string ErrorMessage { get; set; }
            public DateTimeOffset Time { get; set; }
            public Object? Data { get; set; } = null;
            public int ResultCount { get; set; } = 0;
    }
}