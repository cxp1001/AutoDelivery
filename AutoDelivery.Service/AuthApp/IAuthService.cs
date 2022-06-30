using AutoDelivery.Domain;
using AutoDelivery.Domain.User;

namespace AutoDelivery.Service.AuthApp
{
    public interface IAuthService:IocTag
    {
        Task<OAuthState> CreateOAuthStateRecord(string shop);
    }
}