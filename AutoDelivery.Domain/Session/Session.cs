using AutoDelivery.Domain.User;

namespace AutoDelivery.Domain.Session
{
    public class Session
    {
        public int UserId { get; set; }
        public string ShopifyShopDomain { get; set; }
        public bool IsSubscribed { get; set; }

        public Session()
        {
            
        }

        public Session(UserAccount user)
        {
            UserId = user.Id;
            ShopifyShopDomain = user.ShopifyShopDomain;
            IsSubscribed = user.ShopifyChargeId.HasValue;
        }
        
        
        
        
        
        
    }
}