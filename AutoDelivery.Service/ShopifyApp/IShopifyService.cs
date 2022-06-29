using AutoDelivery.Domain;
using AutoDelivery.Domain.Session;
using AutoDelivery.Domain.User;
using Microsoft.AspNetCore.Http;
using ShopifySharp;

namespace AutoDelivery.Service.ShopifyApp
{
    public interface IShopifyService : IocTag
    {
       Task<OAuthState> CheckTokenAsync(string state);
        Task DeleteTokenAsync(OAuthState dbToken);
        Task<int> HandShakeAsync(string shop, Session session, HttpContext httpContext);
        Task<UserAccount> UserUpdate(string shop, Shop shopData, string accessToken);

    }
}