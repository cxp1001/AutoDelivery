using AutoDelivery.Domain;
using AutoDelivery.Domain.Session;
using AutoDelivery.Domain.User;
using Microsoft.AspNetCore.Http;
using ShopifySharp;

namespace AutoDelivery.Service.ShopifyApp
{
    public interface IShopifyService : IocTag
    {
        Task<bool> CheckShopAsync(Session session, string shop);
        Task<OAuthState> CheckTokenAsync(string state);
        Task DeleteTokenAsync(OAuthState dbToken);
        Task<UserAccount> UserUpdate(string shop, Shop shopData, string accessToken);
    }
}