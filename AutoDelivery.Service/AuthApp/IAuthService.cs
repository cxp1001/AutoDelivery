using AutoDelivery.Core.Extensions;
using AutoDelivery.Domain;
using AutoDelivery.Domain.User;

namespace AutoDelivery.Service.AuthApp
{
    public interface IAuthService:IDependency
    {
        Task<OAuthState> CreateOAuthStateRecord(string shop);
    }
}