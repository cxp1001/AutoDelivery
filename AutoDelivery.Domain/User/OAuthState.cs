namespace AutoDelivery.Domain.User
{
    public class OAuthState : BaseModel
    {
        public DateTimeOffset DateCreated { get; set; }
        public string Token { get; set; }
        public string ShopifyShopDomain { get; set; }
    }
}